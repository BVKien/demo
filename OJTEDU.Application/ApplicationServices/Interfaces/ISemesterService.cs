using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.SemesterDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface ISemesterService
    {
        // Admin-Doet - Status List
        Task<DataResponse<List<StatusSemesterListForAdminDoetDTO>>> GetAllStatusesSemesterForAdminDoetAsync();

        // Admin-Doet - Semester Management
        Task<DataResponse<PagedResponse<List<SemesterListForAdminDoetDTO>>>> GetAllSemesterForAdminDoetAsync(string? semesterCode, string? name, string? status, DateTime? startEventDate, DateTime? endEventDate, int pageNumber, int pageSize);
        Task<DataResponse<SemesterDetailForAdminDoetDTO>> GetSemesterDetailByIdForAdminDoetAsync(int semesterId);
        Task<DataResponse<AddSemesterForAdminDoetDTO>> AddSemesterForAdminDoetAsync(AddSemesterForAdminDoetDTO addSemesterForAdminDoetDTO);
        Task<DataResponse<UpdateSemesterForAdminDoetDTO>> UpdateSemesterForAdminDoetAsync(UpdateSemesterForAdminDoetDTO updateSemesterForAdminDoetDTO);
        Task<DataResponse<UpdateSemesterStatusForAdminDoetDTO>> UpdateSemesterStatusForAdminDoetAsync(UpdateSemesterStatusForAdminDoetDTO updateSemesterStatusForAdminDoetDTO);
        Task<DataResponse<DeleteSemesterForAdminDoetDTO>> DeleteSemesterForAdminDoetAsync(DeleteSemesterForAdminDoetDTO deleteSemesterForAdminDoetDTO);

        // Common - Semester 
        Task<DataResponse<List<SemesterListForCommonDTO>>> GetAllSemesterForCommonAsync();
    }
}
