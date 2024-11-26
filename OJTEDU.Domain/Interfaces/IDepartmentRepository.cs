using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        // Admin-DOET - Department Management
        Task<IEnumerable<Department>> GetAllDepartmentForAdminDoetAsync(string? departmentCode, string? departmentName, string? status);
        Task<Department> GetDepartmentByIdAsync(int departmentId);
        Task<Department> GetDepartmentByCodeAsync(string? departmentCode);
        Task AddDepartmentAsync(Department department);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(Department department);
        Task<bool> CheckDepartmentDependenciesAsync(int departmentId);

        // Common
        Task<IEnumerable<Department>> GetAllDepartmentForCommonAsync();
    }
}
