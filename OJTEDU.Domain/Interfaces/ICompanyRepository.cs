using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface ICompanyRepository
    {
        // Admin - DOET
        Task<IEnumerable<Company>> GetAllCompaniesForAdminDoetAsync(string? companyName, string? companyCode, string? status, int? provinceId, int? districtId, int? wardId);
        Task<Company> GetCompanyDetailForAdminDoetAsync(int? companyId);
        Task UpdateCompanyForAdminDoetAsync(Company company);

        // Guest
        Task<(IEnumerable<Company>, int totalRecords)> SearchCompaniesAsync(string? name, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize);
        Task<Company> GetCompanyDetailByCompanyIdAsync(int? companyId);

        // Company 
        Task<IEnumerable<Company>> GetMentorsListAsync(int? userId);
        Task<IEnumerable<Company>> GetAllMentorsInfoAsync();
    }
}
