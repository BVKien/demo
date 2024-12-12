using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class WorkingReportRepository : IWorkingReportRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _fileDirectory = "wwwroot/uploads/workingreports/attachmentfiles/";

        public WorkingReportRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;

            if (!Directory.Exists(_fileDirectory))
            {
                Directory.CreateDirectory(_fileDirectory);
            }
        }

        // Student 
        public async Task<IEnumerable<WorkingReport>> GetAllByStudentIdAsync(int? userId)
        {
            try
            {
                bool studentExists = await _context.Users
                    .AnyAsync(s => s.UserId == userId);

                if (!studentExists)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var student = await _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync();

                var workingReports = await _context.WorkingReports
                    .Include(w => w.Mentor)
                        .ThenInclude(w => w.User)
                    .Include(w => w.Lecturer)
                    .Include(s => s.Student)
                        .ThenInclude(w => w.User)
                    .Where(w => w.StudentId == student.StudentId)
                    .ToListAsync();

                return workingReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WorkingReport> CreateWorkingReportAsync(int? userId, WorkingReport? workingReportInfo, string? fileName, byte[] fileData)
        {
            try
            {
                var internship = await _context.Internships
                    .Include(i => i.Student).ThenInclude(i => i.User).ThenInclude(i => i.Role)
                    .Include(i => i.Company).ThenInclude(i => i.User).ThenInclude(i => i.Role)
                    .Include(i => i.Lecturer).ThenInclude(i => i.Role)
                    .Where(i => i.Student.UserId == userId)
                    .FirstOrDefaultAsync();

                if (internship == null)
                {
                    throw new KeyNotFoundException("Not found internship.");
                }

                bool mentorExists = await _context.Companies.AnyAsync(m => m.CompanyId == internship.CompanyId);
                if (!mentorExists)
                {
                    throw new KeyNotFoundException($"Not found mentor.");
                }

                bool lecturerExists = await _context.Users.AnyAsync(l => l.UserId == internship.LecturerId);
                if (!lecturerExists)
                {
                    throw new KeyNotFoundException($"Not found lecturer.");
                }

                var studentExists = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
                if (studentExists == null)
                {
                    throw new KeyNotFoundException($"Not found student.");
                }

                // Create file name format studentId_timestamp_filename
                var timestamp = GetVietnamTime().ToString("yyyyMMddHHmmssfff");
                var newFileName = fileName != null ? $"{workingReportInfo.StudentId}_{timestamp}_{fileName}" : null;

                var filePath = newFileName != null ? Path.Combine(_fileDirectory, newFileName) : null;

                // Save files to folders
                if (fileData != null && filePath != null)
                {
                    await File.WriteAllBytesAsync(filePath, fileData);
                }

                // If null 
                if (fileName == null || fileData == null)
                {
                    filePath = null;
                }

                var workingReport = new WorkingReport
                {
                    MentorId = internship.CompanyId,
                    LecturerId = internship.LecturerId,
                    StudentId = studentExists.StudentId,
                    ReportTitle = workingReportInfo?.ReportTitle,
                    ReportContent = workingReportInfo?.ReportContent,
                    ReportDate = GetVietnamTime(),
                    FileAttachment = filePath?.Replace("wwwroot", ""),
                    Status = "1",
                    CreatedAt = GetVietnamTime(),
                    UpdatedAt = GetVietnamTime(),
                };

                _context.WorkingReports.Add(workingReport);
                await _context.SaveChangesAsync();

                return workingReport;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WorkingReport> UpdateWorkingReportAsync(int? workingReportId, WorkingReport? workingReportInfo, string? fileName, byte[] fileData)
        {
            try
            {
                var workingReport = await _context.WorkingReports
                    .Where(w => w.WorkingReportId == workingReportId)
                    .FirstOrDefaultAsync();

                if (workingReport == null)
                {
                    throw new KeyNotFoundException("Not found working report.");
                }

                // Create file name format studentId_timestamp_filename
                var timestamp = GetVietnamTime().ToString("yyyyMMddHHmmssfff");
                var newFileName = fileName != null ? $"{workingReport.StudentId}_{timestamp}_{fileName}" : null;

                var filePath = newFileName != null ? Path.Combine(_fileDirectory, newFileName) : null;

                // Save files to folders
                if (fileData != null && filePath != null)
                {
                    await File.WriteAllBytesAsync(filePath, fileData);
                }

                // If null 
                if (fileName == null || fileData == null)
                {
                    filePath = null;
                }

                // Update
                workingReport.ReportTitle = workingReportInfo?.ReportTitle;
                workingReport.ReportContent = workingReportInfo?.ReportContent;
                workingReport.FileAttachment = filePath?.Replace("wwwroot", "");
                workingReport.UpdatedAt = GetVietnamTime();

                _context.WorkingReports.Update(workingReport);
                await _context.SaveChangesAsync();

                return workingReport;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WorkingReport> GetWorkingReportDetailAsync(int? workingReportId)
        {
            try
            {
                var workingReport = await _context.WorkingReports
                    .Include(w => w.Mentor)
                        .ThenInclude(w => w.User)
                    .Include(w => w.Lecturer)
                    .Include(s => s.Student)
                        .ThenInclude(w => w.User)
                    .Where(w => w.WorkingReportId == workingReportId)
                    .FirstOrDefaultAsync();

                if (workingReport == null)
                {
                    throw new KeyNotFoundException("Not found working report.");
                }

                return workingReport;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //For Dean
        public async Task<User> GetDeanByUserIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Dean");
        }

        public async Task<List<string>> GetWeeksForStudentAsync(int studentId, int? year = null)
        {
            var internship = await _context.Internships
                .Where(i => i.StudentId == studentId)
                .FirstOrDefaultAsync();

            if (internship == null || internship.StartDate == null || internship.EndDate == null)
            {
                throw new KeyNotFoundException("Internship details not found for the given student.");
            }

            DateTime startDate = internship.StartDate.Value;
            DateTime endDate = internship.EndDate.Value;

            // Nếu năm không được cung cấp, mặc định là năm hiện tại theo giờ Việt Nam
            if (!year.HasValue)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime currentVietnamTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, vietnamTimeZone);
                year = currentVietnamTime.Year;
            }

            List<(DateTime WeekStart, string WeekRange, int Year)> weeks = new List<(DateTime, string, int)>();

            // Tạo danh sách các năm trong khoảng thời gian thực tập
            int startYear = startDate.Year;
            int endYear = endDate.Year;

            for (int y = startYear; y <= endYear; y++)
            {
                DateTime validStartDate = startDate.Year < y ? new DateTime(y, 1, 1) : startDate;
                DateTime validEndDate = endDate.Year > y ? new DateTime(y, 12, 31) : endDate;

                DateTime weekStart = validStartDate.AddDays(-(int)validStartDate.DayOfWeek + (int)DayOfWeek.Monday);

                while (weekStart <= validEndDate)
                {
                    DateTime weekEnd = weekStart.AddDays(6);
                    if (weekStart <= validEndDate && weekEnd >= validStartDate)
                    {
                        // Week overlaps with the internship period
                        string weekRange = $"{weekStart:dd/MM} to {weekEnd:dd/MM}";
                        weeks.Add((weekStart, weekRange, y));
                    }
                    weekStart = weekStart.AddDays(7);
                }
            }

            // Sắp xếp danh sách tuần theo ngày bắt đầu
            weeks = weeks.OrderBy(w => w.WeekStart).ToList();

            // Trả về danh sách chuỗi tuần (không bao gồm năm)
            return weeks.Select(w => w.WeekRange).ToList();
        }


        public async Task<Student> GetStudentDetailsByIdAsync(int studentId, int userId, string role)
        {
            // Khởi tạo query lấy thông tin sinh viên
            var query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .Include(s => s.Lecturer)
                .Include(s => s.Address)
                    .ThenInclude(a => a.Ward)
                .Include(s => s.Address.District)
                .Include(s => s.Address.Province)
                .Where(s => s.StudentId == studentId && s.User.Status != "Deleted" && s.Major.Status == "Active");

            // Logic cho Lecturer
            if (role == "Lecturer")
            {
                query = query.Where(s => s.LecturerId == userId);
            }
            // Logic cho Dean
            else if (role == "Dean")
            {
                // Lấy thông tin Dean
                var dean = await GetDeanByUserIdAsync(userId);
                if (dean == null || !dean.DepartmentId.HasValue)
                {
                    throw new KeyNotFoundException("Dean not found or doesn't manage any department.");
                }

                // Lấy danh sách MajorId thuộc Department mà Dean quản lý
                var majorIdsInDepartment = await _context.Majors
                    .Where(m => m.DepartmentId == dean.DepartmentId)
                    .Select(m => m.MajorId)
                    .ToListAsync();

                if (!majorIdsInDepartment.Any())
                {
                    throw new KeyNotFoundException("Dean does not manage any majors.");
                }

                // Kiểm tra MajorId của sinh viên có thuộc MajorId trong Department không
                query = query.Where(s => s.MajorId.HasValue && majorIdsInDepartment.Contains(s.MajorId.Value));
            }

            // Lấy sinh viên đầu tiên phù hợp
            var student = await query.FirstOrDefaultAsync();

            // Nếu không tìm thấy sinh viên
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found or access denied.");
            }

            return student;
        }

        public async Task<List<WorkingReport>> GetWorkingReportsByStudentIdAsync(
        int internshipId, int userId, string role, string? sortBy, bool? isDescending, string? week, int? year = null)
        {
            // Lấy thông tin Internship và StudentId từ InternshipId
            var internship = await _context.Internships
                .Include(i => i.Student)
                .FirstOrDefaultAsync(i => i.IntershipId == internshipId);

            if (internship == null || internship.StartDate == null || internship.EndDate == null)
            {
                throw new KeyNotFoundException("Internship not found or invalid start/end date.");
            }

            if (!internship.StudentId.HasValue)
            {
                throw new InvalidOperationException("StudentId is null for the given internship.");
            }

            int studentId = internship.StudentId.Value;

            // Kiểm tra quyền truy cập
            var student = await GetStudentDetailsByIdAsync(studentId, userId, role);
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found or access denied.");
            }

            DateTime internshipStart = internship.StartDate.Value;
            DateTime internshipEnd = internship.EndDate.Value.AddDays(1).AddSeconds(-1);

            IQueryable<WorkingReport> query = _context.WorkingReports
                .Include(w => w.Student)
                .Where(w => w.StudentId == studentId && w.CreatedAt >= internshipStart && w.CreatedAt <= internshipEnd);

            // Nếu năm không được cung cấp, mặc định là năm hiện tại theo giờ Việt Nam
            if (!year.HasValue)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime currentVietnamTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, vietnamTimeZone);
                year = currentVietnamTime.Year;
            }

            // Mặc định lấy tuần hiện tại nếu không có tuần nào được chọn
            if (string.IsNullOrEmpty(week))
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime currentVietnamTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, vietnamTimeZone);

                DateTime currentWeekStart = currentVietnamTime.AddDays(-(int)currentVietnamTime.DayOfWeek + (int)DayOfWeek.Monday);
                DateTime currentWeekEnd = currentWeekStart.AddDays(6).AddDays(1).AddSeconds(-1);

                query = query.Where(w => w.CreatedAt >= currentWeekStart && w.CreatedAt <= currentWeekEnd);
            }
            else
            {
                var weekDates = week.Split(" to ");
                if (weekDates.Length == 2)
                {
                    DateTime weekStart, weekEnd;

                    // Xử lý nếu StartDate và EndDate nằm trong hai năm khác nhau
                    if (internshipStart.Year != internshipEnd.Year)
                    {
                        // Xác định năm dựa trên tháng của tuần
                        int weekStartMonth = int.Parse(weekDates[0].Split('/')[1]);
                        int weekEndMonth = int.Parse(weekDates[1].Split('/')[1]);

                        int weekYear = weekStartMonth >= internshipStart.Month ? internshipStart.Year : internshipEnd.Year;

                        weekStart = DateTime.ParseExact($"{weekDates[0]}/{weekYear}", "dd/MM/yyyy", null);
                        weekEnd = DateTime.ParseExact($"{weekDates[1]}/{weekYear}", "dd/MM/yyyy", null).AddDays(1).AddSeconds(-1);
                    }
                    else
                    {
                        weekStart = DateTime.ParseExact($"{weekDates[0]}/{internshipStart.Year}", "dd/MM/yyyy", null);
                        weekEnd = DateTime.ParseExact($"{weekDates[1]}/{internshipStart.Year}", "dd/MM/yyyy", null).AddDays(1).AddSeconds(-1);
                    }

                    query = query.Where(w => w.CreatedAt >= weekStart && w.CreatedAt <= weekEnd);
                }
                else
                {
                    throw new ArgumentException("Invalid week format. Expected format: 'dd/MM to dd/MM'.");
                }
            }

            // Sắp xếp
            switch (sortBy?.ToLower())
            {
                case "updatedat":
                    query = isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(w => w.UpdatedAt)
                        : query.OrderBy(w => w.UpdatedAt);
                    break;
                case "createdat":
                default:
                    query = isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(w => w.CreatedAt)
                        : query.OrderBy(w => w.CreatedAt);
                    break;
            }

            return await query.ToListAsync();
        }


        public async Task<bool> UpdateWorkingReportAsync(int workingReportId, int userId, string role, string? feedback, double? score)
        {
            var workingReport = await _context.WorkingReports
                .Include(wr => wr.Student)
                .FirstOrDefaultAsync(wr => wr.WorkingReportId == workingReportId);

            if (workingReport == null || (string.IsNullOrEmpty(feedback) && !score.HasValue))
            {
                return false;
            }

            if (score < 0 || score > 10)
            {
                throw new KeyNotFoundException($"Inavlid, score must be eaqual or larger than 0 and smaller than 10. ");
            }

            if (!string.IsNullOrEmpty(feedback)) workingReport.FeedbackFromLecturer = feedback;
            if (score.HasValue) workingReport.LecturerScore = score != null ? Math.Round(score.Value, 2) : 0;

            workingReport.UpdatedAt = GetVietnamTime();
            _context.WorkingReports.Update(workingReport);
            await _context.SaveChangesAsync();
            return true;
        }

        // Mentor 
        public async Task<IEnumerable<WorkingReport>> GetAllWorkingReportsByStudentId(int? studentId)
        {
            try
            {
                var studentExists = await _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (studentExists == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var workingReports = await _context.WorkingReports
                    .Include(w => w.Mentor)
                        .ThenInclude(w => w.User)
                    .Include(w => w.Lecturer)
                    .Include(s => s.Student)
                        .ThenInclude(w => w.User)
                    .Where(w => w.StudentId == studentExists.StudentId)
                    .ToListAsync();

                return workingReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WorkingReport> CreateMentorFeedbackAsync(int? workingReportId, WorkingReport? info)
        {
            try
            {
                var workingReport = await _context.WorkingReports
                    .Include(w => w.Mentor)
                        .ThenInclude(w => w.User)
                    .Include(w => w.Lecturer)
                    .Include(s => s.Student)
                        .ThenInclude(w => w.User)
                    .Where(w => w.WorkingReportId == workingReportId)
                    .FirstOrDefaultAsync();

                if (workingReport == null)
                {
                    throw new KeyNotFoundException("Not found working report.");
                }

                if (info?.MentorScore < 0 || info?.MentorScore > 10)
                {
                    throw new KeyNotFoundException("Inavlid, score must be eaqual or larger than 0 and smaller than 10.");
                }

                // Feedback 
                workingReport.FeedbackFromMentor = info?.FeedbackFromMentor;
                // Round MentorScore to 2 decimal places
                workingReport.MentorScore = info?.MentorScore != null ? Math.Round(info.MentorScore.Value, 2) : 0;
                workingReport.UpdatedAt = GetVietnamTime();

                await _context.SaveChangesAsync();

                return workingReport;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
