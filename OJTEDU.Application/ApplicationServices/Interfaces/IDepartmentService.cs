using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.DepartmentDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IDepartmentService
    {
        // Admin-DOET - Department Management
        Task<DataResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>> GetAllDepartmentForAdminDoetAsync(string? departmentCode, string? departmentName, string? status, int pageNumber, int pageSize);
        Task<DataResponse<DepartmentDetailForAdminDoetDTO>> GetDepartmentDetailByIdForAdminDoetAsync(int departmentId);
        Task<DataResponse<AddDepartmentForAdminDoetDTO>> AddDepartmentForAdminDoetAsync(AddDepartmentForAdminDoetDTO addDepartmentForAdminDoetDTO);
        Task<DataResponse<UpdateDepartmentForAdminDoetDTO>> UpdateDepartmentForAdminDoetAsync(UpdateDepartmentForAdminDoetDTO updateDepartmentForAdminDoetDTO);
        Task<DataResponse<UpdateDepartmentStatusForAdminDoetDTO>> UpdateDepartmentStatusForAdminDoetAsync(UpdateDepartmentStatusForAdminDoetDTO updateDepartmentStatusForAdminDoetDTO);
        Task<DataResponse<DeleteDepartmentForAdminDoetDTO>> DeleteDepartmentForAdminDoetAsync(DeleteDepartmentForAdminDoetDTO deleteDepartmentForAdminDoetDTO);
        Task<DataResponse<List<StatusDepartmentListForAdminDoetDTO>>> GetAllStatusesDepartmentForAdminDoetAsync();

        // Common
        Task<DataResponse<List<DepartmentListForCommonDTO>>> GetAllDepartmentForCommonAsync();
    }
}
