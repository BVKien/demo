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
                    .FirstOrDefaultAsync(s => s.UserId == userId);

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

        public async Task<WorkingReport> CreateWorkingReportAsync(int? userId, WorkingReport? workingReportInfo, string? fileName, string? fileData)
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

                //// Create file name format studentId_timestamp_filename
                //var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //var newFileName = fileName != null ? $"{workingReportInfo.StudentId}_{timestamp}_{fileName}" : null;

                //var filePath = newFileName != null ? Path.Combine(_fileDirectory, newFileName) : null;

                //// Save files to folders
                //if (fileData != null && filePath != null)
                //{
                //    await File.WriteAllBytesAsync(filePath, fileData);
                //}

                //// If null 
                //if (fileName == null || fileData == null)
                //{
                //    filePath = null;
                //}

                var workingReport = new WorkingReport
                {
                    MentorId = internship.CompanyId,
                    LecturerId = internship.LecturerId,
                    StudentId = studentExists.StudentId,
                    ReportTitle = workingReportInfo?.ReportTitle,
                    ReportContent = workingReportInfo?.ReportContent,
                    ReportDate = DateTime.Now,
                    FileAttachment = fileData,
                    Status = "1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
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

        public async Task<WorkingReport> UpdateWorkingReportAsync(int? workingReportId, WorkingReport? workingReportInfo, string? fileName)
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

                //// Create file name format studentId_timestamp_filename
                //var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //var newFileName = fileName != null ? $"{workingReport.StudentId}_{timestamp}_{fileName}" : null;

                //var filePath = newFileName != null ? Path.Combine(_fileDirectory, newFileName) : null;

                //// Save files to folders
                //if (fileData != null && filePath != null)
                //{
                //    await File.WriteAllBytesAsync(filePath, fileData);
                //}

                //// If null 
                //if (fileName == null || fileData == null)
                //{
                //    filePath = null;
                //}

                // Update
                workingReport.ReportTitle = workingReportInfo?.ReportTitle;
                workingReport.ReportContent = workingReportInfo?.ReportContent;
                workingReport.FileAttachment = fileName;
                workingReport.UpdatedAt = DateTime.Now;

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
                .Where(i => i.IntershipId == studentId)
                .FirstOrDefaultAsync();

            if (internship == null || internship.StartDate == null || internship.EndDate == null)
            {
                throw new KeyNotFoundException("Internship details not found for the given student.");
            }

            DateTime startDate = internship.StartDate.Value;
            DateTime endDate = internship.EndDate.Value;

            // Nếu năm không được cung cấp, mặc định là năm hiện tại
            year ??= DateTime.Now.Year;

            List<(DateTime WeekStart, string WeekRange, int Year)> weeks = new List<(DateTime, string, int)>();

            // Xác định khoảng năm cần xử lý
            int startYear = startDate.Year;
            int endYear = endDate.Year;

            for (int y = startYear; y <= endYear; y++)
            {
                DateTime validStartDate = startDate.Year < y ? new DateTime(y, 1, 1) : startDate;
                DateTime validEndDate = endDate.Year > y ? new DateTime(y, 12, 31) : endDate;

                // Tính ngày bắt đầu tuần
                DateTime weekStart = validStartDate.AddDays(-(int)validStartDate.DayOfWeek + (int)DayOfWeek.Monday);

                while (weekStart <= validEndDate)
                {
                    DateTime weekEnd = weekStart.AddDays(6);
                    if (weekStart <= validEndDate && weekEnd >= validStartDate)
                    {
                        // Tuần nằm trong khoảng thời gian thực tập
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
            // Bắt đầu truy vấn
            Console.WriteLine($"Starting GetStudentDetailsByIdAsync: StudentId={studentId}, UserId={userId}, Role={role}");

            // Truy vấn Student kèm các thực thể liên quan
            var query = _context.Students
                .Include(s => s.User) // Bao gồm thông tin User liên quan
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .Include(s => s.Lecturer)
                .Include(s => s.Address)
                    .ThenInclude(a => a.Ward)
                .Include(s => s.Address.District)
                .Include(s => s.Address.Province)
                .Where(s => s.StudentId == studentId && s.User.Status != "Deleted");

            Console.WriteLine("Initial query built.");

            // Kiểm tra quyền truy cập nếu vai trò là Lecturer
            if (role == "Lecturer")
            {
                query = query.Where(s => s.LecturerId == userId);
                Console.WriteLine($"Filtering query for Lecturer with UserId={userId}");
            }
            // Kiểm tra quyền truy cập nếu vai trò là Dean
            else if (role == "Dean")
            {
                var dean = await GetDeanByUserIdAsync(userId);
                if (dean == null || !dean.DepartmentId.HasValue)
                {
                    Console.WriteLine("Dean not found or doesn't manage any department.");
                    throw new KeyNotFoundException("Dean not found or doesn't manage any department.");
                }

                Console.WriteLine($"Dean found. DepartmentId={dean.DepartmentId}");

                var majorIdsInDepartment = await _context.Majors
                    .Where(m => m.DepartmentId == dean.DepartmentId)
                    .Select(m => m.MajorId)
                    .ToListAsync();

                if (!majorIdsInDepartment.Any())
                {
                    Console.WriteLine("Dean does not manage any majors.");
                    throw new KeyNotFoundException("Dean does not manage any majors.");
                }

                Console.WriteLine($"Majors managed by Dean: {string.Join(", ", majorIdsInDepartment)}");

                query = query.Where(s => s.MajorId.HasValue && majorIdsInDepartment.Contains(s.MajorId.Value));
            }

            // Log truy vấn cuối cùng trước khi thực thi
            Console.WriteLine($"Final query: StudentId={studentId}, Role={role}");

            // Lấy dữ liệu Student
            var student = await query.FirstOrDefaultAsync();

            if (student == null)
            {
                Console.WriteLine("Student not found or access denied.");
                throw new KeyNotFoundException("Student not found or access denied.");
            }

            // Log thông tin Student trước khi trả về
            Console.WriteLine($"Student found: StudentId={student.StudentId}, UserId={student.User?.UserId}, UserName={student.User?.Name}");

            return student;
        }


        public async Task<List<WorkingReport>> GetWorkingReportsByStudentIdAsync(
            int internshipId, int userId, string role, string? sortBy, bool? isDescending, string? week, int? year = null)
        {
            // Lấy thông tin Internship
            var internship = await _context.Internships
                .Include(i => i.Student) // Bao gồm Student
                    .ThenInclude(s => s.User) // Bao gồm User của Student
                .Include(i => i.Lecturer) // Bao gồm thông tin Lecturer
                .FirstOrDefaultAsync(i => i.IntershipId == internshipId);

            if (internship == null || internship.StudentId == null)
            {
                throw new KeyNotFoundException("Internship not found or does not have a valid StudentId.");
            }

            Console.WriteLine($"InternshipId: {internship.IntershipId}, StudentId: {internship.StudentId}, StudentUserName: {internship.Student.User.Name}");

            // Xác thực quyền truy cập
            var studentDetails = await GetStudentDetailsByIdAsync(internship.StudentId.Value, userId, role);
            if (studentDetails == null)
            {
                throw new KeyNotFoundException("Access denied or student details not found.");
            }

            Console.WriteLine($"StudentId: {studentDetails.StudentId}, UserId: {studentDetails.User.UserId}, UserName: {studentDetails.User.Name}");

            // Lấy thời gian bắt đầu và kết thúc của Internship
            var internshipStart = internship.StartDate.Value;
            var internshipEnd = internship.EndDate.Value.AddDays(1).AddSeconds(-1);

            Console.WriteLine($"Internship Start: {internshipStart}, End: {internshipEnd}");

            // Truy vấn WorkingReports theo StudentId
            IQueryable<WorkingReport> query = _context.WorkingReports
                .Where(w => w.StudentId == internship.StudentId.Value && w.CreatedAt >= internshipStart && w.CreatedAt <= internshipEnd);

            // Xử lý tuần
            if (string.IsNullOrEmpty(week))
            {
                // Nếu không truyền `week`, lấy tuần hiện tại
                DateTime now = DateTime.Now;
                DateTime currentWeekStart = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                DateTime currentWeekEnd = currentWeekStart.AddDays(6).AddDays(1).AddSeconds(-1);

                query = query.Where(w => w.CreatedAt >= currentWeekStart && w.CreatedAt <= currentWeekEnd);
                Console.WriteLine($"Query Filter: CurrentWeek Start: {currentWeekStart}, End: {currentWeekEnd}");
            }
            else
            {
                // Xử lý tuần được truyền
                var weekDates = week.Split(" to ");
                if (weekDates.Length == 2)
                {
                    int targetYear = year ?? DateTime.Now.Year; // Nếu không truyền `year`, mặc định là năm hiện tại
                    DateTime weekStart = DateTime.ParseExact($"{weekDates[0]}/{targetYear}", "dd/MM/yyyy", null);
                    DateTime weekEnd = DateTime.ParseExact($"{weekDates[1]}/{targetYear}", "dd/MM/yyyy", null).AddDays(1).AddSeconds(-1);
                    query = query.Where(w => w.CreatedAt >= weekStart && w.CreatedAt <= weekEnd);
                    Console.WriteLine($"Query Filter: Week Start: {weekStart}, End: {weekEnd}");
                }
                else
                {
                    throw new ArgumentException("Invalid week format. Expected format: 'dd/MM to dd/MM'.");
                }
            }

            // Sắp xếp
            query = sortBy?.ToLower() switch
            {
                "updatedat" => isDescending.GetValueOrDefault()
                    ? query.OrderByDescending(w => w.UpdatedAt)
                    : query.OrderBy(w => w.UpdatedAt),
                "createdat" or _ => isDescending.GetValueOrDefault()
                    ? query.OrderByDescending(w => w.CreatedAt)
                    : query.OrderBy(w => w.CreatedAt),
            };

            Console.WriteLine($"WorkingReports Count: {query.Count()}");

            return await query.ToListAsync();
        }




        public async Task<Internship> GetInternshipByIdAsync(int internshipId)
        {
            return await _context.Internships
                .Include(i => i.Student)
                .FirstOrDefaultAsync(i => i.IntershipId == internshipId);
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

            workingReport.UpdatedAt = DateTime.Now;
            _context.WorkingReports.Update(workingReport);
            await _context.SaveChangesAsync();
            return true;
        }

        // Mentor 
        public async Task<List<WorkingReport>> GetAllWorkingReportsByStudentIdAsync(
            int? studentId, string? sortBy = null, bool? isDescending = null, string? week = null, int? year = null)
        {
            try
            {
                // Kiểm tra sự tồn tại của sinh viên
                var studentExists = await _context.Students
                    .Include(s => s.User).ThenInclude(u => u.Role)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (studentExists == null)
                {
                    throw new KeyNotFoundException("Student not found.");
                }

                IQueryable<WorkingReport> query = _context.WorkingReports
                    .Include(w => w.Mentor).ThenInclude(m => m.User)
                    .Include(w => w.Lecturer)
                    .Include(w => w.Student).ThenInclude(s => s.User)
                    .Where(w => w.StudentId == studentExists.StudentId);

                // Xử lý tuần và năm
                DateTime weekStartDate;
                DateTime weekEndDate;
                int targetYear = year ?? DateTime.Now.Year; // Nếu không truyền năm, mặc định là năm hiện tại

                if (string.IsNullOrEmpty(week))
                {
                    // Nếu không có tuần nào được truyền, lấy tuần hiện tại
                    DateTime now = DateTime.Now;
                    weekStartDate = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                    weekEndDate = weekStartDate.AddDays(6).AddDays(1).AddSeconds(-1);
                }
                else
                {
                    // Xử lý tuần được truyền vào
                    var weekDates = week.Split(" to ");
                    if (weekDates.Length == 2)
                    {
                        weekStartDate = DateTime.ParseExact($"{weekDates[0]}/{targetYear}", "dd/MM/yyyy", null);
                        weekEndDate = DateTime.ParseExact($"{weekDates[1]}/{targetYear}", "dd/MM/yyyy", null).AddDays(1).AddSeconds(-1);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid week format. Expected format: 'dd/MM to dd/MM'.");
                    }
                }

                // Lọc báo cáo theo tuần
                query = query.Where(w => w.CreatedAt >= weekStartDate && w.CreatedAt <= weekEndDate);

                // Sắp xếp kết quả
                query = sortBy?.ToLower() switch
                {
                    "updatedat" => isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(w => w.UpdatedAt)
                        : query.OrderBy(w => w.UpdatedAt),
                    "createdat" or _ => isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(w => w.CreatedAt)
                        : query.OrderBy(w => w.CreatedAt),
                };

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving working reports: {ex.Message}");
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
                workingReport.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return workingReport;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
