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
    public class SemesterRepository : ISemesterRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public SemesterRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Admin-Doet - Semester Management
        public async Task<IEnumerable<Semester>> GetAllSemesterForAdminDoetAsync(string? semesterCode, string? name, string? status, DateTime? startEventDate, DateTime? endEventDate)
        {
            IQueryable<Semester> query = _context.Semesters;

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(semesterCode))
            {
                semesterCode = semesterCode.ToLower();
                query = query.Where(n => n.SemesterCode.ToLower().Contains(semesterCode));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Tìm kiếm theo startEventDate và endEventDate
            if (startEventDate.HasValue && endEventDate.HasValue)
            {
                // Tìm các Semester có khoảng thời gian giao với khoảng startDate-endDate
                query = query.Where(s =>
                    (s.StartDate >= startEventDate && s.StartDate <= endEventDate) || // Bắt đầu trong khoảng tìm kiếm
                    (s.EndDate >= startEventDate && s.EndDate <= endEventDate) ||     // Kết thúc trong khoảng tìm kiếm
                    (s.StartDate <= startEventDate && s.EndDate >= endEventDate)      // Bao phủ toàn bộ khoảng tìm kiếm
                );
            }
            else if (startEventDate.HasValue)
            {
                // Tìm các Semester bắt đầu sau hoặc vào ngày startDate
                query = query.Where(s => s.StartDate >= startEventDate);
            }
            else if (endEventDate.HasValue)
            {
                // Tìm các Semester kết thúc trước hoặc vào ngày endDate
                query = query.Where(s => s.EndDate <= endEventDate);
            }

            // Fetch the filtered result from the database
            var semesters = await query.ToListAsync();

            if (semesters == null)
            {
                throw new KeyNotFoundException("Semesters not found.");
            }

            var sortedSemesters = semesters.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.UpdatedAt)
                                           .ToList();

            return sortedSemesters;
        }

        public async Task<Semester> GetSemesterByIdAsync(int semesterId)
        {
            return await _context.Semesters.FirstOrDefaultAsync(c => c.SemesterId == semesterId);
        }

        public async Task<Semester> GetSemesterByCodeAsync(string? code)
        {
            return await _context.Semesters.FirstOrDefaultAsync(c => c.SemesterCode.ToLower() == code.ToLower());
        }

        public async Task AddSemesterAsync(Semester semester)
        {
            await _context.Semesters.AddAsync(semester);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSemesterAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSemesterAsync(Semester semester)
        {
            _context.Semesters.Remove(semester);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckSemesterDependenciesAsync(int semesterId)
        {
            bool hasStudents = await _context.Students.AnyAsync(e => e.SemesterId == semesterId);
            bool hasInternships = await _context.Internships.AnyAsync(p => p.SemesterId == semesterId);

            return hasStudents || hasInternships;
        }

        // Common - Semester 
        public async Task<IEnumerable<Semester>> GetAllSemesterForCommonAsync()
        {
            return await _context.Semesters.Where(s => s.Status.Equals("Active"))
                                            .OrderByDescending(s => s.CreatedAt)
                                            .ToListAsync();
        }
    }
}
