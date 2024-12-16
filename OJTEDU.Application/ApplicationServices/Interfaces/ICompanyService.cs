using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.CompanyDTO;
using static OJTEDU.Application.DTOs.StudentDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface ICompanyService
    {
        // Admin - DOET
        Task<DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>> GetAllCompaniesForAdminDoetAsync(string? companyName, string? companyCode, string? status, int? provinceId, int? districtId, int? wardId, int pageNumber, int pageSize);
        Task<DataResponse<CompanyDetailForAdminDoetDTO>> GetCompanyDetailForAdminDoetAsync(int? companyId);
        Task<DataResponse<UpdateCompanyForAdminDoetDTO>> UpdateCompanyForAdminDoetAsync(UpdateCompanyForAdminDoetDTO updateCompanyForAdminDoetDTO, int? provinceId, int? districtId, int? wardId, string? addressDetail);

        // Guest 
        Task<PagedResult<List<CompanySearchListForGuestDTO>>> SearchCompaniesAsync(string? name, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize);
        Task<DataResponse<CompanyDetailForGuestDTO>> GetCompanyDetailByCompanyIdAsync(int? companyId);

        // Student 
        Task<PagedResult<List<CompanySearchListForStudentDTO>>> SearchCompaniesForStudentAsync(string? name, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize);
        Task<DataResponse<CompanyDetailForStudentDTO>> GetCompanyDetailByCompanyIdForStudentAsync(int? companyId);

        // Company 
        Task<DataResponse<List<MentorListForCompanyDTO>>> GetMentorsListAsync(int? userId);
        Task<DataResponse<List<MentorsInfoListForCompanyDTO>>> GetAllMentorsInfoAsync();
        Task<DataResponse<UpdateCompanyForCompanyDTO>> UpdateCompanyByUserIdAsync(int? userId, UpdateCompanyForCompanyDTO? updateInformation);
    }
}
