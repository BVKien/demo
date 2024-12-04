using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class InternshipRepository : IInternshipRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public InternshipRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Mentor
        // Fix functions
        public async Task<IEnumerable<Internship>> GetAllInternshipsByUserIdAsync(int? userId)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var mentor = await _context.Companies
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .Where(c => c.UserId == userId)
                    .FirstOrDefaultAsync();

                var internships = await _context.Internships
                    .Include(i => i.Student).ThenInclude(i => i.User).ThenInclude(i => i.Role)
                    .Include(i => i.Company).ThenInclude(i => i.User).ThenInclude(i => i.Role)
                    .Include(i => i.Lecturer).ThenInclude(i => i.Role)
                    .Include(i => i.Job)
                    .Include(i => i.Contract)
                    .Include(i => i.Semester)
                    .Include(i => i.Major)
                    .Include(i => i.Evaluation)
                    .Where(i => i.CompanyId == mentor.CompanyId)
                    .ToListAsync();

                if (internships == null)
                {
                    throw new KeyNotFoundException("Not found internships list that the mentor manage.");
                }

                return internships;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Internship> GetInternshipDetailAsync(int? internshipId)
        {
            try
            {
                var internship = await _context.Internships
                    .Include(i => i.Student).ThenInclude(i => i.User).ThenInclude(i => i.Role)
                    .Include(i => i.Company).ThenInclude(i => i.User).ThenInclude(i => i.Role)
                    .Include(i => i.Lecturer).ThenInclude(i => i.Role)
                    .Include(i => i.Job)
                    .Include(i => i.Contract)
                    .Include(i => i.Semester)
                    .Include(i => i.Major)
                    .Include(i => i.Evaluation)
                    .FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                if (internship == null)
                {
                    throw new KeyNotFoundException("Not found internship detail that mentor manage.");
                }

                return internship;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Company 
        public async Task<IEnumerable<Internship>> GetAllInternshipsByUserIdForCompanyAsync(int? userId)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var job = await _context.Jobs
                    .Include(j => j.Company)
                    .FirstOrDefaultAsync(j => j.Company.UserId == userId);

                if (job == null)
                {
                    throw new KeyNotFoundException("Not found jobs list for company.");
                }

                var internships = await _context.Internships
                    .Include(i => i.Student)
                        .ThenInclude(s => s.User)
                        .ThenInclude(u => u.Role)
                    .Include(i => i.Company)
                        .ThenInclude(c => c.User)
                        .ThenInclude(u => u.Role)
                    .Include(i => i.Lecturer)
                        .ThenInclude(l => l.Role)
                    .Include(i => i.Job)
                    .Include(i => i.Contract)
                    .Include(i => i.Semester)
                    .Include(i => i.Major)
                    .Include(i => i.Evaluation)
                    .Where(i => (i.Company.User != null
                             && i.Company.User.Role.Name == "Mentor"
                             && i.Company.User.ForCompany == userId) || (i.Job.CompanyId == job.CompanyId))
                    .ToListAsync();

                if (internships == null)
                {
                    throw new KeyNotFoundException("Not found internships list for company.");
                }

                return internships;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AssignInternshipsForMentorAsync(int? userId, int? mentorId, int[]? internshipIds)
        {
            try
            {
                var company = await _context.Companies
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (company == null)
                {
                    throw new KeyNotFoundException("Not found company.");
                }

                var mentor = await _context.Companies
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .Where(c => c.CompanyId == mentorId && c.User.ForCompany == userId && c.User.Role.Name == "Mentor")
                    .FirstOrDefaultAsync();

                if (mentor == null)
                {
                    throw new KeyNotFoundException("Not found mentor of company.");
                }

                var internships = await _context.Internships
                    .Include(i => i.Student)
                        .ThenInclude(s => s.User)
                        .ThenInclude(u => u.Role)
                    .Include(i => i.Company)
                        .ThenInclude(c => c.User)
                        .ThenInclude(u => u.Role)
                    .Include(i => i.Lecturer)
                        .ThenInclude(l => l.Role)
                    .Include(i => i.Job)
                    .Include(i => i.Contract)
                    .Include(i => i.Semester)
                    .Include(i => i.Major)
                    .Include(i => i.Evaluation)
                    .Where(i => i.Job.Company.UserId == userId
                             && internshipIds.Contains(i.IntershipId) && i.CompanyId == null)
                    .ToListAsync();

                if (internships == null)
                {
                    throw new KeyNotFoundException("Not found internships list for company.");
                }

                // Update 
                foreach (var internship in internships)
                {
                    internship.CompanyId = mentorId;
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Internship> CreateInternshipAsync(int? studentId)
        {
            try
            {
                var student = await _context.Students
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .FirstOrDefaultAsync(c => c.StudentId == studentId);

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var application = await _context.Appllications.FirstOrDefaultAsync(a => a.StudentId == studentId);

                if (application == null)
                {
                    throw new KeyNotFoundException("Not found application of this student.");
                }

                var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.SemesterId == student.SemesterId);

                if (semester == null)
                {
                    throw new KeyNotFoundException("Not found semester.");
                }

                var internship = new Internship
                {
                    StudentId = studentId,
                    JobId = application.JobId,
                    LecturerId = student.LecturerId,
                    StartDate = semester.StartDate,
                    EndDate = semester.EndDate,
                    Status = "1",
                    SemesterId = student.SemesterId,
                    MajorId = student.MajorId,
                    CreatedAt = GetVietnamTime(),
                    UpdatedAt = GetVietnamTime()
                };

                await _context.Internships.AddAsync(internship);
                await _context.SaveChangesAsync();

                return internship;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Internship>> GetAllInternshipsAsync(
        int userId,
        string role,
        string? searchTerm,
        DateTime? startDate,
        DateTime? endDate,
        string? statusFilter,
        string? sortBy,
        bool isDescending)
        {
            IQueryable<Internship> query = _context.Internships
                .Include(i => i.Student)
                    .ThenInclude(s => s.User)
                .Include(i => i.Student.Lecturer)
                .Include(i => i.Company)
                   .ThenInclude(c => c.User)
                .Include(i => i.Job)
                .Include(i => i.Semester)
                .Include(i => i.Major)
                .Where(i => i.Student.User.Status != "Deleted" && i.Company.User.Status == "Active" && i.Job.Status == "Active" && i.Major.Status == "Active");

            // Access Control
            if (role == "Admin" || role == "DOET")
            {
                // No restrictions
            }
            else if (role == "Lecturer" || role == "Dean")
            {
                query = query.Where(i => i.Student.LecturerId == userId);
            }
            else
            {
                throw new UnauthorizedAccessException("Role not authorized to view internships.");
            }

            // Search across multiple fields (excluding Evaluation Name)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(i =>
                    i.Student.User.Name.ToLower().Contains(searchTerm) ||
                    i.Company.User.Name.ToLower().Contains(searchTerm) ||
                    i.Job.Title.ToLower().Contains(searchTerm) ||
                    i.Student.Lecturer.Name.ToLower().Contains(searchTerm) ||
                    i.Semester.Name.ToLower().Contains(searchTerm) ||
                    i.Code.ToLower().Contains(searchTerm));
            }

            // Filter by Start Date and End Date
            if (startDate.HasValue)
            {
                query = query.Where(i => i.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(i => i.EndDate <= endDate.Value);
            }

            // Filter by Status
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                statusFilter = statusFilter.ToLower();
                query = query.Where(i => i.Status.ToLower().Contains(statusFilter));
            }

            // Sorting
            switch (sortBy?.ToLower())
            {
                case "studentname":
                    query = isDescending ? query.OrderByDescending(i => i.Student.User.Name) : query.OrderBy(i => i.Student.User.Name);
                    break;
                case "companyname":
                    query = isDescending ? query.OrderByDescending(i => i.Company.User.Name) : query.OrderBy(i => i.Company.User.Name);
                    break;
                case "jobname":
                    query = isDescending ? query.OrderByDescending(i => i.Job.Title) : query.OrderBy(i => i.Job.Title);
                    break;
                case "lecturername":
                    query = isDescending ? query.OrderByDescending(i => i.Student.Lecturer.Name) : query.OrderBy(i => i.Student.Lecturer.Name);
                    break;
                case "semestername":
                    query = isDescending ? query.OrderByDescending(i => i.Semester.Name) : query.OrderBy(i => i.Semester.Name);
                    break;
                case "code":
                    query = isDescending ? query.OrderByDescending(i => i.Code) : query.OrderBy(i => i.Code);
                    break;
                case "startdate":
                    query = isDescending ? query.OrderByDescending(i => i.StartDate) : query.OrderBy(i => i.StartDate);
                    break;
                case "enddate":
                    query = isDescending ? query.OrderByDescending(i => i.EndDate) : query.OrderBy(i => i.EndDate);
                    break;
                default:
                    query = query.OrderBy(i => i.Student.User.Name);
                    break;
            }

            // Execute the query and return the list
            return await query.ToListAsync();
        }

        public async Task<(Internship, List<WorkingReport>)> GetInternshipDetailsWithWorkingReportsAsync(int internshipId, int userId, string role)
        {
            var internshipQuery = _context.Internships
                .Include(i => i.Student.User)
                .Include(i => i.Company.User)
                .Include(i => i.Job)
                .Include(i => i.Lecturer)
                .Include(i => i.Contract)
                .Include(i => i.Semester)
                .Include(i => i.Major)
                .Include(i => i.Evaluation)
                .Where(i => i.IntershipId == internshipId);

            if (role == "Lecturer" && role == "Dean")
            {
                internshipQuery = internshipQuery.Where(i => i.LecturerId == userId);
            }

            var internship = await internshipQuery.FirstOrDefaultAsync();
            if (internship == null)
            {
                throw new KeyNotFoundException("Internship not found.");
            }

            // Lấy danh sách WorkingReport liên quan đến Internship
            var workingReports = await _context.WorkingReports
                .Where(w => w.StudentId == internship.StudentId)
                .OrderBy(w => w.CreatedAt)
                .ToListAsync();

            return (internship, workingReports);
        }
        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
