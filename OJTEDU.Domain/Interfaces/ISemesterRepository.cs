using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface ISemesterRepository
    {
        // Admin-Doet - Semester Management
        Task<IEnumerable<Semester>> GetAllSemesterForAdminDoetAsync(string? semesterCode, string? name, string? status, DateTime? startEventDate, DateTime? endEventDate);
        Task<Semester> GetSemesterByIdAsync(int semesterId);
        Task<Semester> GetSemesterByCodeAsync(string? code);
        Task AddSemesterAsync(Semester semester);
        Task UpdateSemesterAsync(Semester semester);
        Task DeleteSemesterAsync(Semester semester);
        Task<bool> CheckSemesterDependenciesAsync(int semesterId);

        // Common - Semester
        Task<IEnumerable<Semester>> GetAllSemesterForCommonAsync();
    }
}
