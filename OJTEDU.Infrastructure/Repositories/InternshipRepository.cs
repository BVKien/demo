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
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
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
    }
}
