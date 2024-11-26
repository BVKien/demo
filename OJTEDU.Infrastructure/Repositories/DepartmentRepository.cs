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
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public DepartmentRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Admin-DOET - Department Management

        public async Task<IEnumerable<Department>> GetAllDepartmentForAdminDoetAsync(string? departmentCode, string? departmentName, string? status)
        {
            IQueryable<Department> query = _context.Departments;

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(departmentCode))
            {
                departmentCode = departmentCode.ToLower();
                query = query.Where(n => n.DepartmentCode.ToLower().Contains(departmentCode));
            }

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                departmentName = departmentName.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(departmentName));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var departments = await query.ToListAsync();

            if (departments == null)
            {
                throw new KeyNotFoundException("Departments not found.");
            }

            var sortedDepartments = departments.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.UpdatedAt)
                                           .ToList();

            return sortedDepartments;
        }

        public async Task<Department> GetDepartmentByIdAsync(int departmentId)
        {
            return await _context.Departments.FirstOrDefaultAsync(c => c.DepartmentId == departmentId);
        }

        public async Task<Department> GetDepartmentByCodeAsync(string? departmentCode)
        {
            return await _context.Departments.FirstOrDefaultAsync(c => c.DepartmentCode.ToLower() == departmentCode.ToLower());
        }

        public async Task AddDepartmentAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(Department department)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckDepartmentDependenciesAsync(int departmentId)
        {
            bool hasMajors = await _context.Majors.AnyAsync(e => e.DepartmentId == departmentId);
            bool hasUsers = await _context.Users.AnyAsync(p => p.DepartmentId == departmentId);

            return hasMajors || hasUsers;
        }

        // Common
        public async Task<IEnumerable<Department>> GetAllDepartmentForCommonAsync()
        {
            return await _context.Departments.Where(s => s.Status.Equals("Active"))
                                            .OrderByDescending(s => s.CreatedAt)
                                            .ToListAsync();
        }
    }
}
