using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.RoleDTO;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IRoleService
    {
        // Admin 
        Task<DataResponse<List<RoleListForAdminDTO>>> GetAllRolesToAddUpdateForAdminAsync();
        Task<DataResponse<PagedResponse<List<RoleListForAdminDTO>>>> GetAllRolesForAdminAsync(int pageNumber, int pageSize);
        Task<DataResponse<RoleDetailForAdminDTO>> GetRoleDetailByIdForAdminAsync(int roleId);
        Task<DataResponse<AddRoleForAdminDTO>> AddRoleForAdminAsync(AddRoleForAdminDTO addRoleForAdminDTO);
        Task<DataResponse<UpdateRoleForAdminDTO>> UpdateRoleForAdminAsync(UpdateRoleForAdminDTO updateRoleForAdminDTO);
        Task<DataResponse<DeleteRoleForAdminDTO>> DeleteRoleForAdminAsync(DeleteRoleForAdminDTO deleteRoleForAdminDTO);

        // DOET 
        Task<DataResponse<List<RoleListForAdminDTO>>> GetAllRolesToAddUpdateForDoetAsync();
        Task<DataResponse<PagedResponse<List<RoleListForAdminDTO>>>> GetAllRolesForDoetAsync(int pageNumber, int pageSize);

        // Company 
        Task<DataResponse<PagedResponse<List<RoleListForAdminDTO>>>> GetAllRolesForCompanyAsync(int pageNumber, int pageSize);
    }
}
