using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IMajorRepository
    {
        // Admin-DOET - Major Management
        Task<IEnumerable<Major>> GetAllMajorForAdminDoetAsync(string? majorCode, string? majorName, string? status, int? departmentId);
        Task<Major> GetMajorByIdAsync(int majorId);
        Task<Major> GetMajorByCodeAsync(string? majorCode);
        Task AddMajorAsync(Major major);
        Task AddMajorsAsync(IEnumerable<Major> majors);
        Task UpdateMajorAsync(Major major);
        Task DeleteMajorAsync(Major major);
        Task<bool> CheckMajorDependenciesAsync(int majorId);

        // Common
        Task<IEnumerable<Major>> GetAllMajorForCommonAsync();

        // Student 
        Task<IEnumerable<Major>> GetAllMajorsAsync();
    }
}
