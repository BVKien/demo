using Microsoft.AspNetCore.Http;
using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.MajorDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IMajorService
    {
        // Admin-DOET - Major Management
        Task<DataResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>> GetAllMajorForAdminDoetAsync(string? majorCode, string? majorName, string? status, int? departmentId, int pageNumber, int pageSize);
        Task<DataResponse<MajorDetailForAdminDoetDTO>> GetMajorIdDetailByIdForAdminDoetAsync(int majorId);
        Task<DataResponse<AddMajorForAdminDoetDTO>> AddMajorForAdminDoetAsync(AddMajorForAdminDoetDTO addMajorForAdminDoetDTO);
        Task<DataResponse<UpdateMajorForAdminDoetDTO>> UpdateMajorForAdminDoetAsync(UpdateMajorForAdminDoetDTO updateMajorForAdminDoetDTO);
        Task<DataResponse<UpdateMajorStatusForAdminDoetDTO>> UpdateMajorStatusForAdminDoetAsync(UpdateMajorStatusForAdminDoetDTO updateMajorStatusForAdminDoetDTO);
        Task<DataResponse<DeleteMajorForAdminDoetDTO>> DeleteMajorForAdminDoetAsync(DeleteMajorForAdminDoetDTO deleteMajorForAdminDoetDTO);
        Task<DataResponse<MemoryStream>> GenerateMajorTemplateForAdminDoetAsync();
        Task<DataResponse<object>> ImportMajorsForAdminDoetAsync(IFormFile file);
        Task<DataResponse<List<StatusMajorListForAdminDoetDTO>>> GetAllStatusesMajorForAdminDoetAsync();

        // Common
        Task<DataResponse<List<MajorListForCommonDTO>>> GetAllMajorForCommonAsync();

        // Student 
        Task<DataResponse<List<MajorListForStudentDTO>>> GetAllMajorsAsync();
    }
}
