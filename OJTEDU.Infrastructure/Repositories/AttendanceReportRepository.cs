using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class AttendanceReportRepository : IAttendanceReportRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _attendanceReportDirectory = "wwwroot/uploads/attendancereports/files/";

        public AttendanceReportRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;

            if (!Directory.Exists(_attendanceReportDirectory))
            {
                Directory.CreateDirectory(_attendanceReportDirectory);
            }
        }

        public async Task<List<AttendanceReport>> GetAttendanceReportsByStudentIdAsync(int studentId)
        {
            var attendanceReports = await _context.AttendanceReports
                //.Where(ar => ar.StudentId == studentId)
                .Where(ar => ar.InternshipId == studentId)
                //.Include(ar => ar.Mentor)
                //    .ThenInclude(m => m.User)
                //.Include(ar => ar.Student)
                //    .ThenInclude(s => s.User)
                .ToListAsync();

            return attendanceReports;
        }

        // Mentor 
        public async Task<Company> SetCheckInCheckOutTimeAsync(int? userId, Company? info)
        {
            try
            {
                var mentor = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

                if (mentor == null)
                {
                    throw new Exception("Not found mentor.");
                }

                mentor.CheckInTime = info?.CheckInTime;
                mentor.CheckOutTime = info?.CheckOutTime;
                mentor.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return mentor;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> CreateAttendanceReportFileAsync(int? userId, int[]? internshipIds, string? fileName, byte[] fileData)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateAutoAttendanceReportAsync(int? userId, TimeSpan? checkInTime, TimeSpan? checkOutTime)
        {
            try
            {
                var mentor = await _context.Companies
                    .Include(c => c.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (mentor == null)
                {
                    throw new KeyNotFoundException("Mentor not found.");
                }

                var internships = await _context.Internships
                    .Include(i => i.Student)
                        .ThenInclude(s => s.User)
                    .Include(i => i.Company)
                    .Include(i => i.Semester)
                    .Where(i => i.CompanyId == mentor.CompanyId)
                    .ToListAsync();

                var newAttendanceReports = new List<AttendanceReport>();

                foreach (var internship in internships)
                {
                    var latestAttendance = await _context.AttendanceReports
                        .Where(ar => ar.InternshipId == internship.IntershipId)
                        .OrderByDescending(ar => ar.Date)
                        .FirstOrDefaultAsync();

                    DateTime currentDate = DateTime.Now.Date;

                    // Determine new day
                    DateTime newDate = (latestAttendance == null)
                        ? (internship.StartDate?.Date ?? currentDate)
                        : (latestAttendance.Date.Value.Date < currentDate ? currentDate : DateTime.MinValue);

                    if (newDate == DateTime.MinValue || newDate > internship.EndDate)
                    {
                        continue; // No need to create if out of date
                    }

                    // Create
                    var newAttendanceReport = new AttendanceReport
                    {
                        MentorId = mentor.CompanyId,
                        InternshipId = internship.IntershipId,
                        Date = newDate,
                        CheckInTime = checkInTime,
                        CheckOutTime = checkOutTime,
                        Status = "1",
                        EarlyLeave = false,
                        Late = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    newAttendanceReports.Add(newAttendanceReport);
                }

                // Save db changes
                if (newAttendanceReports.Any())
                {
                    await _context.AttendanceReports.AddRangeAsync(newAttendanceReports);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in create auto attendance report async: {ex.Message}");
                throw;
            }
        }

        public async Task<AttendanceReport> UpdateAttendanceReportAsync(int? attendanceReportId, AttendanceReport? info)
        {
            try
            {
                var attendanceReport = await _context.AttendanceReports
                    .FirstOrDefaultAsync(a => a.AttendanceReportId == attendanceReportId);

                if (attendanceReport == null)
                {
                    throw new Exception("Not found attendance report.");
                }

                attendanceReport.CheckInTime = info?.CheckInTime;
                attendanceReport.CheckOutTime = info?.CheckOutTime;
                attendanceReport.Reason = info?.Reason;
                attendanceReport.Status = info?.Status;
                attendanceReport.EarlyLeave = info?.EarlyLeave;
                attendanceReport.Late = info?.Late;
                attendanceReport.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return attendanceReport;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> InsertAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData)
        {
            try
            {
                var mentor = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

                if (mentor == null)
                {
                    throw new Exception("Mentor not found.");
                }

                // Create new file name with format mentorId_timestamp_filename
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var newFileName = $"{mentor.CompanyId}_{timestamp}_{fileName}";
                var filePath = Path.Combine(_attendanceReportDirectory, newFileName);

                var reports = new List<AttendanceReport>();

                // Read fiel by using EPPlus
                using (var package = new ExcelPackage(new MemoryStream(fileData)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Read the first sheet
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Start from line 2 if line 1 is header
                    {
                        try
                        {
                            var attendanceReport = new AttendanceReport
                            {
                                MentorId = mentor.CompanyId,
                                Date = worksheet.Cells[row, 3].GetValue<DateTime>(),
                                CheckInTime = ParseTimeFromExcel(worksheet.Cells[row, 4].Value),
                                CheckOutTime = ParseTimeFromExcel(worksheet.Cells[row, 5].Value),
                                Reason = worksheet.Cells[row, 6].GetValue<string>(),
                                Status = worksheet.Cells[row, 7].GetValue<string>(),
                                Late = worksheet.Cells[row, 8].GetValue<bool>(),
                                EarlyLeave = worksheet.Cells[row, 9].GetValue<bool>(),
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            // Find internship based on code
                            var internshipCode = worksheet.Cells[row, 1].GetValue<string>();
                            var internship = await _context.Internships
                                .FirstOrDefaultAsync(i => i.Code == internshipCode);

                            if (internship != null)
                            {
                                attendanceReport.InternshipId = internship.IntershipId;
                                reports.Add(attendanceReport);
                            }
                            else
                            {
                                Console.WriteLine($"Internship with code {internshipCode} not found.");
                            }
                        }
                        catch (Exception rowEx)
                        {
                            Console.WriteLine($"Error processing row at line {row}: {rowEx.Message}");
                        }
                    }
                }

                // Insert all reports into database
                if (reports.Count > 0)
                {
                    await _context.AttendanceReports.AddRangeAsync(reports);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Excel file: {ex.Message}");
                throw new Exception("An error occurred while inserting attendance reports.");
            }
        }

        private TimeSpan? ParseTimeFromExcel(object cellValue)
        {
            if (cellValue == null)
                return null;

            // Trường hợp 1: Nếu là kiểu DateTime (Excel có thể lưu giờ như một phần ngày)
            if (cellValue is DateTime dateTime)
                return dateTime.TimeOfDay;

            // Trường hợp 2: Nếu là kiểu số thập phân (Excel lưu giờ theo tỷ lệ ngày)
            if (double.TryParse(cellValue.ToString(), out var numericValue))
                return TimeSpan.FromDays(numericValue);

            // Trường hợp 3: Nếu là chuỗi và có thể parse sang TimeSpan
            if (TimeSpan.TryParse(cellValue.ToString(), out var parsedTime))
                return parsedTime;

            return null;
        }

        public async Task<IEnumerable<AttendanceReport>> ListAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData)
        {
            try
            {
                var mentor = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

                if (mentor == null)
                {
                    throw new Exception("Mentor not found.");
                }

                // Create new file name with format mentorId_timestamp_filename
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var newFileName = $"{mentor.CompanyId}_{timestamp}_{fileName}";
                var filePath = Path.Combine(_attendanceReportDirectory, newFileName);

                var attendanceReports = new List<AttendanceReport>();

                using (var package = new ExcelPackage(new MemoryStream(fileData)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var attendanceReport = new AttendanceReport
                            {
                                Date = worksheet.Cells[row, 3].GetValue<DateTime>(),
                                CheckInTime = TimeSpan.TryParse(worksheet.Cells[row, 4].GetValue<string>(), out var checkIn) ? checkIn : (TimeSpan?)null,
                                CheckOutTime = TimeSpan.TryParse(worksheet.Cells[row, 5].GetValue<string>(), out var checkOut) ? checkOut : (TimeSpan?)null,
                                Reason = worksheet.Cells[row, 6].GetValue<string>(),
                                Status = worksheet.Cells[row, 7].GetValue<string>(),
                                Late = worksheet.Cells[row, 8].GetValue<bool>(),
                                EarlyLeave = worksheet.Cells[row, 9].GetValue<bool>()
                            };

                            attendanceReports.Add(attendanceReport);
                        }
                        catch (Exception rowEx)
                        {
                            Console.WriteLine($"Error processing row at line {row}: {rowEx.Message}");
                        }
                    }
                }

                return attendanceReports;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Excel file: {ex.Message}");
                return new List<AttendanceReport>();
            }
        }

        // Mentor, Lecturer
        public async Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsByInternshipIdAsync(int? internshipId)
        {
            try
            {
                var internship = await _context.Internships
                    .Include(i => i.Student)
                    .FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                var attendanceReports = await _context.AttendanceReports
                    .Include(a => a.Mentor).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Internship).ThenInclude(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Where(a => a.InternshipId == internship.IntershipId)
                    .ToListAsync();

                return attendanceReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForMentorAsync(int? userId)
        {
            try
            {
                var mentor = await _context.Companies
                    .Include(i => i.User).ThenInclude(i => i.Role)
                    .FirstOrDefaultAsync(i => i.UserId == userId);

                if (mentor == null)
                {
                    throw new Exception("Not found mentor.");
                }

                var attendanceReports = await _context.AttendanceReports
                    .Include(a => a.Mentor).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Internship).ThenInclude(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Where(a => a.MentorId == mentor.CompanyId)
                    .ToListAsync();

                return attendanceReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Lecturer
        public async Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForLecturerAsync(int? userId)
        {
            try
            {
                var internship = await _context.Internships
                    .Include(i => i.Student)
                    .FirstOrDefaultAsync(i => i.Student.LecturerId == userId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                var attendanceReports = await _context.AttendanceReports
                    .Include(a => a.Mentor).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Internship).ThenInclude(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Where(a => a.InternshipId == internship.IntershipId)
                    .ToListAsync();

                return attendanceReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Student
        public async Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForStudentAsync(int? userId)
        {
            try
            {
                var internship = await _context.Internships
                    .Include(i => i.Student)
                    .FirstOrDefaultAsync(i => i.Student.UserId == userId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                var attendanceReports = await _context.AttendanceReports
                    .Include(a => a.Mentor).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Internship).ThenInclude(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Where(a => a.InternshipId == internship.IntershipId)
                    .ToListAsync();

                return attendanceReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
