using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.UserGuideDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IUserGuideService
    {
        // Admin - User Guide Management
        Task<DataResponse<PagedResponse<List<UserGuideListForAdminDTO>>>> GetAllUserGuidesForAdminAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<UserGuideDetailForAdminDTO>> GetUserGuideDetailByIdForAdminAsync(int userGuideId);

        Task<DataResponse<AddUserGuideForAdminDTO>> AddUserGuideForAdminAsync(AddUserGuideForAdminDTO addUserGuideForAdminDTO);

        Task<DataResponse<UpdateUserGuideForAdminDTO>> UpdateUserGuideForAdminAsync(UpdateUserGuideForAdminDTO updateUserGuideForAdminDTO);

        Task<DataResponse<UpdateUserGuideStatusForAdminDTO>> UpdateUserGuideStatusForAdminAsync(UpdateUserGuideStatusForAdminDTO updateUserGuideStatusForAdminDTO);

        Task<DataResponse<DeleteUserGuideForAdminDTO>> DeleteUserGuideForAdminAsync(DeleteUserGuideForAdminDTO deleteUserGuideForAdminDTO);
        Task<DataResponse<List<StatusUserGuideListForAdminDTO>>> GetAllStatusesUserGuideForAdminAsync();

        // Common
        Task<DataResponse<UserGuideDetailForAdminDTO>> GetUserGuideByRoleNameAsync(string roleName);
    }
}
