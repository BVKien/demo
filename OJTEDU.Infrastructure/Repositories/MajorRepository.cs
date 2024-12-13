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
    public class MajorRepository : IMajorRepository
    {
        private readonly OJTEDU_DB_V1Context _context;

        public MajorRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Admin-DOET
        public async Task<IEnumerable<Major>> GetAllMajorForAdminDoetAsync(string? majorCode, string? majorName, string? status, int? departmentId)
        {
            IQueryable<Major> query = _context.Majors.Include(m => m.Department);

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(majorCode))
            {
                majorCode = majorCode.ToLower();
                query = query.Where(n => n.MajorCode.ToLower().Contains(majorCode));
            }

            if (!string.IsNullOrWhiteSpace(majorName))
            {
                majorName = majorName.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(majorName));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(u => u.DepartmentId == departmentId.Value);
            }

            // Fetch the filtered result from the database
            var majors = await query.ToListAsync();

            if (majors == null)
            {
                throw new KeyNotFoundException("Majors not found.");
            }

            var sortedMajors = majors.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.UpdatedAt)
                                           .ToList();

            return sortedMajors;
        }

        public async Task<Major> GetMajorByIdAsync(int majorId)
        {
            return await _context.Majors.Include(m => m.Department)
                                        .FirstOrDefaultAsync(c => c.MajorId == majorId);
        }

        public async Task<Major> GetMajorByCodeAsync(string? majorCode)
        {
            return await _context.Majors.Include(m => m.Department)
                                        .FirstOrDefaultAsync(c => c.MajorCode.ToLower() == majorCode.ToLower());
        }
        public async Task AddMajorAsync(Major major)
        {
            await _context.Majors.AddAsync(major);
            await _context.SaveChangesAsync();
        }

        public async Task AddMajorsAsync(IEnumerable<Major> majors)
        {
            await _context.Majors.AddRangeAsync(majors);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMajorAsync(Major major)
        {
            _context.Majors.Update(major);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMajorAsync(Major major)
        {
            _context.Majors.Remove(major);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CheckMajorDependenciesAsync(int majorId)
        {
            bool hasUsers = await _context.Users.AnyAsync(p => p.MajorId == majorId);
            bool hasStudents = await _context.Students.AnyAsync(p => p.MajorId == majorId);
            bool hasInterships = await _context.Internships.AnyAsync(p => p.MajorId == majorId);
            bool hasJobs = await _context.Jobs.AnyAsync(p => p.MajorId == majorId);

            return hasUsers || hasStudents || hasInterships || hasJobs;
        }

        // Common
        public async Task<IEnumerable<Major>> GetAllMajorForCommonAsync()
        {
            return await _context.Majors.Where(m => m.Status.Equals("Active")).OrderByDescending(s => s.CreatedAt).ToListAsync();
        }

        // Stduent
        public async Task<IEnumerable<Major>> GetAllMajorsAsync()
        {
            try
            {
                var majors = await _context.Majors
                    .ToListAsync();

                return majors;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving majors. " + ex.Message);
            }
        }
    }
}
