using Microsoft.AspNetCore.Http;
using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IUsersService
    {
        // Common - Authentication
        //Task<DataResponse<UserReadForAuthDTO>> LoginWithGoogleAsync(string token);
        //Task<DataResponse<UserReadForAuthDTO>> GetAuthenticatedUserInfoAsync(ClaimsPrincipal userClaims);
        //Task<DataResponse<object>> LogoutAsync();

        // Admin - User Management
        Task<DataResponse<PagedResponse<List<UserListForAdminDTO>>>> GetAllUsersForAdminAsync(string? name, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<UserDetailForAdminDTO>> GetUserDetailByIdForAdminAsync(int userId);
        Task<DataResponse<AddUserForAdminDTO>> AddUserForAdminAsync(AddUserForAdminDTO addUserForAdminDTO);
        Task<DataResponse<UpdateUserForAdminDTO>> UpdateUserForAdminAsync(UpdateUserForAdminDTO updateUserForAdminDTO);
        Task<DataResponse<UpdateUserStatusForAdminDTO>> UpdateUserStatusForAdminAsync(UpdateUserStatusForAdminDTO updateUserStatusForAdminDTO);
        Task<DataResponse<DeleteUserForAdminDTO>> SoftDeleteUserForAdminAsync(DeleteUserForAdminDTO deleteUserForAdminDTO);
        Task<DataResponse<MemoryStream>> GenerateUserTemplateForAdminAsync();
        Task<DataResponse<object>> ImportUsersForAdminAsync(IFormFile file);
        Task<DataResponse<List<StatusUserListForAdminDTO>>> GetAllStatusesUserForAdminAsync();


        // Admin - User Stored Management
        Task<DataResponse<PagedResponse<List<UserListForAdminDTO>>>> GetAllUsersStoredForAdmin(string? name, int? roleId, int pageNumber, int pageSize);
        Task<DataResponse<UserDetailForAdminDTO>> GetUserStoredDetailByIdForAdminAsync(int userId);
        Task<DataResponse<DeleteUserForAdminDTO>> HardDeleteUserStoredForAdminAsync(DeleteUserForAdminDTO deleteUserForAdminDTO);
        Task<DataResponse<RestoreUserForAdminDTO>> RestoreUserForAdminAsync(RestoreUserForAdminDTO restoreUserForAdminDTO);

        // DOET - User Management
        Task<DataResponse<PagedResponse<List<UserListForDoetDTO>>>> GetAllUsersForDoetAsync(string? name, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<UserDetailForDoetDTO>> GetUserDetailByIdForDoetAsync(int userId);
        Task<DataResponse<AddUserForDoetDTO>> AddUserForDoetAsync(AddUserForDoetDTO addUserForDoetDTO);
        Task<DataResponse<UpdateUserForDoetDTO>> UpdateUserForDoetAsync(UpdateUserForDoetDTO updateUserForDoetDTO);
        Task<DataResponse<UpdateUserStatusForDoetDTO>> UpdateUserStatusForDoetAsync(UpdateUserStatusForDoetDTO updateUserStatusForDoetDTO);
        Task<DataResponse<DeleteUserForDoetDTO>> SoftDeleteUserForDoetAsync(DeleteUserForDoetDTO deleteUserForDoetDTO);
        Task<DataResponse<MemoryStream>> GenerateUserTemplateForDoetAsync();
        Task<DataResponse<object>> ImportUsersForDoetAsync(IFormFile file);
        Task<DataResponse<List<StatusUserListForDoetDTO>>> GetAllStatusesUserForDoetAsync();

        // Company - User Management
        Task<DataResponse<PagedResponse<List<UserListForCompanyDTO>>>> GetAllUsersForCompanyAsync(int companyId, string? name, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<UserDetailForCompanyDTO>> GetUserDetailByIdForCompanyAsync(int companyId, int userId);
        Task<DataResponse<AddUserForCompanyDTO>> AddUserForCompanyAsync(int companyId, AddUserForCompanyDTO addUserForCompanyDTO);
        Task<DataResponse<UpdateUserForCompanyDTO>> UpdateUserForCompanyAsync(int companyId, UpdateUserForCompanyDTO updateUserForCompanyDTO);
        Task<DataResponse<UpdateUserStatusForCompanyDTO>> UpdateUserStatusForCompanyAsync(int companyId, UpdateUserStatusForCompanyDTO updateUserStatusForCompanyDTO);
        Task<DataResponse<DeleteUserForCompanyDTO>> SoftDeleteUserForCompanyAsync(int companyId, DeleteUserForCompanyDTO deleteUserForCompanyDTO);
        Task<DataResponse<List<StatusUserListForCompanyDTO>>> GetAllStatusesUserForCompanyAsync();

        //For Dean
        // ViewProfileAsync
        Task<DataResponse<UserProfileDto>> ViewProfileAsync();

        // UpdateProfileAsync
        Task<DataResponse<string>> UpdateProfileAsync(UpdateProfileDto dto);

        // CreateLecturerAsync
        Task<DataResponse<string>> CreateLecturerAsync(CreateLecturerDto dto);

        // GetLecturerListForDeanAsync
        Task<DataResponse<PagedResponse<List<LecturerListDto>>>> GetLecturerListForDeanAsync(
        string? name,
        string? userCode,
        string? majorName,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize);

        // GetLecturerDetailsAsync
        Task<DataResponse<LecturerDetailsDto>> GetLecturerDetailsAsync(
        int lecturerId,
        string? studentName,
        string? lecturerName,
        string? semesterName,
        string? sortBy,
        bool? isDescending,
        int pageNumber,
        int pageSize);

        Task<DataResponse<PagedResponse<List<DeanListForAdminDOETDto>>>> GetAllDeansAsync(
        string? userCode,
        string? name,
        string? departmentName,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool? isDescending);
        Task<DataResponse<DeanDetailsDto>> GetDeanDetailsAsync(
        int deanId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool? isDescending,
        string? lecturerName,
        string? studentName,
        int studentPageNumber,
        int studentPageSize);
        Task<DataResponse<string>> AssignLecturersToDeanAsync(AssignLecturersToDeanDto dto);
        Task<DataResponse<PagedResponse<List<LecturerListDto>>>> GetAllLecturersAsync(
        string? userCode,
        string? name,
        string? majorName,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool? isDescending);
        Task<DataResponse<string>> AssignDepartmentToDeanAsync(int deanId, int departmentId);
        Task<DataResponse<string>> AssignMajorToLecturerAsync(int lecturerId, int majorId);

    }
}
