using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.JobDTO;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IJobService
    {
        // Student  
        Task<DataResponse<List<JobListByCompanyIdForStudentDTO>>> GetAllJobsByCompanyIdAsync(int? companyId);

        Task<PagedResult<List<JobListSearchForStudentDTO>>> SearchJobsAsync(int? userId, string? title, int? majorId,
            int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize);


        Task<DataResponse<List<JobListForStudentDTO>>> GetAllJobsAsync();
        Task<DataResponse<JobDetailForStudentDTO>> GetJobDetailAsync(int? jobId);

        // Company 
        Task<DataResponse<List<JobListForCompanyDTO>>> GetAllJobsByUserIdAsync(int? userId);
        Task<DataResponse<JobDetailForCompanyDTO>> GetJobDetailForCompanyAsync(int? jobId);
        Task<DataResponse<CreateJobForCompanyDTO>> CreateJobAsync(int? userId, string? fileName, byte[] fileData, CreateJobForCompanyDTO? info);
        Task<DataResponse<UpdateJobForCompanyDTO>> UpdateJobAsync(int? userId, int? jobId, string? fileName, byte[] fileData, UpdateJobForCompanyDTO? info);

        // User service
        Task<DataResponse<UserReadForAuthDTO>> LoginWithGoogleAsync(string token);
        Task<DataResponse<PagedResponse<List<UserListForAdminDTO>>>> GetAllUsersForAdminAsync(string? name, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<UserReadForAuthDTO>> GetAuthenticatedUserInfoAsync(ClaimsPrincipal userClaims);
        Task<DataResponse<object>> LogoutAsync();
    }
}
