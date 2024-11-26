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
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
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
                    ReportDate = DateTime.Now,
                    FileAttachment = filePath?.Replace("wwwroot", ""),
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
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
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
        public async Task<Student> GetStudentDetailsByIdAsync(int studentId, int userId, string role)
        {
            var query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .Include(s => s.Lecturer)
                .Include(s => s.Address)
                    .ThenInclude(a => a.Ward)
                .Include(s => s.Address.District)
                .Include(s => s.Address.Province)
                .Where(s => s.StudentId == studentId && s.User.Status != "Deleted");

            if (role == "Lecturer")
            {
                query = query.Where(s => s.LecturerId == userId);
            }
            else if (role == "Dean")
            {
                var dean = await GetDeanByUserIdAsync(userId);
                if (dean == null || !dean.MajorId.HasValue)
                {
                    throw new KeyNotFoundException("Don't have student invalid.");
                }
                var deanMajorId = dean.MajorId.Value;
                query = query.Where(s => s.MajorId == deanMajorId);
            }

            var student = await query.FirstOrDefaultAsync();

            return student;
        }
        public async Task<List<WorkingReport>> GetWorkingReportsByStudentIdAsync(
        int studentId,
        int userId,
        string role,
        string? sortBy,
        bool? isDescending)
        {
            // Kiểm tra quyền truy cập
            var student = await GetStudentDetailsByIdAsync(studentId, userId, role);
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found or access denied.");
            }

            // Khởi tạo truy vấn
            IQueryable<WorkingReport> query = _context.WorkingReports
                .Include(w => w.Student)
                .Where(w => w.StudentId == studentId);

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

            var workingReports = await query.ToListAsync();

            if (workingReports == null || !workingReports.Any())
            {
                throw new KeyNotFoundException("No working reports found for the given student.");
            }

            return workingReports;
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
