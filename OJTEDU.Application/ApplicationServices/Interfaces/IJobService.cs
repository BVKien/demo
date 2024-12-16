using Microsoft.AspNetCore.Http;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.AttendanceReportDTO;
using static OJTEDU.Application.DTOs.DocumentDTO;
using static OJTEDU.Application.DTOs.EvaluationDTO;
using static OJTEDU.Application.DTOs.JobDTO;
using static OJTEDU.Application.DTOs.NotificationDTO;
using static OJTEDU.Application.DTOs.StudentDTO;
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
        Task<DataResponse<CreateJobForCompanyDTO>> CreateJobAsync(int? userId, string? fileName, CreateJobForCompanyDTO? info);
        Task<DataResponse<UpdateJobForCompanyDTO>> UpdateJobAsync(int? userId, int? jobId, string? fileName, UpdateJobForCompanyDTO? info);

        // User service
        //Task<DataResponse<UserReadForAuthDTO>> LoginWithGoogleAsync(string token);
        //Task<DataResponse<PagedResponse<List<UserListForAdminDTO>>>> GetAllUsersForAdminAsync(string? name, int? roleId, string? status, int pageNumber, int pageSize);
        //Task<DataResponse<UserReadForAuthDTO>> GetAuthenticatedUserInfoAsync(ClaimsPrincipal userClaims);
        //Task<DataResponse<object>> LogoutAsync();

        // Common - Authentication
        Task<DataResponse<UserReadForAuthDTO>> LoginWithGoogleAsync(string token);
        Task<DataResponse<UserReadForAuthDTO>> GetAuthenticatedUserInfoAsync(ClaimsPrincipal userClaims);
        Task<DataResponse<object>> LogoutAsync();

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

        // === Document ===
        // Admin - Document Management
        Task<DataResponse<PagedResponse<List<DocumentListForAdminDTO>>>> GetAllDocumentsForAdminAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<DocumentDetailForAdminDTO>> GetDocumentDetailByIdForAdminAsync(int documentId);

        Task<DataResponse<AddDocumentForAdminDTO>> AddDocumentForAdminAsync(AddDocumentForAdminDTO addDocumentForAdminDTO);

        Task<DataResponse<UpdateDocumentForAdminDTO>> UpdateDocumentForAdminAsync(UpdateDocumentForAdminDTO updateDocumentForAdminDTO);

        Task<DataResponse<UpdateDocumentStatusForAdminDTO>> UpdateDocumentStatusForAdminAsync(UpdateDocumentStatusForAdminDTO updateDocumentStatusForAdminDTO);

        Task<DataResponse<DeleteDocumentForAdminDTO>> DeleteDocumentForAdminAsync(DeleteDocumentForAdminDTO deleteDocumentForAdminDTO);
        Task<DataResponse<List<StatusDocumentListForAdminDTO>>> GetAllStatusesDocumentForAdminAsync();
        // Doet - Document Management
        Task<DataResponse<PagedResponse<List<DocumentListForDoetDTO>>>> GetAllDocumentsForDoetAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<DocumentDetailForDoetDTO>> GetDocumentDetailByIdForDoetAsync(int documentId);
        Task<DataResponse<AddDocumentForDoetDTO>> AddDocumentForDoetAsync(AddDocumentForDoetDTO addDocumentForDoetDTO);
        Task<DataResponse<UpdateDocumentForDoetDTO>> UpdateDocumentForDoetAsync(UpdateDocumentForDoetDTO updateDocumentForDoetDTO);
        Task<DataResponse<UpdateDocumentStatusForDoetDTO>> UpdateDocumentStatusForDoetAsync(UpdateDocumentStatusForDoetDTO updateDocumentStatusForDoetDTO);
        Task<DataResponse<DeleteDocumentForDoetDTO>> DeleteDocumentForDoetAsync(DeleteDocumentForDoetDTO deleteDocumentForDoetDTO);
        Task<DataResponse<List<StatusDocumentListForDoetDTO>>> GetAllStatusesDocumentForDoetAsync();

        // Common
        Task<DataResponse<PagedResponse<List<DocumentListForCommonDTO>>>> GetAllDocumentsAsync(string role, string? title, int pageNumber, int pageSize);
        Task<DataResponse<DocumentDetailForCommonDTO>> GetDocumentDetailAsync(int documentId, string role);

        // Guest
        //Task<DataResponse<DocumentInternshipProcessForGuestDTO>> GetInternshipProcessDocumentAsync();

        // Company 
        Task<DataResponse<List<DocumentTestFilesListForCompanyDTO>>> GetAllDocumentsByUserIdAsync(int? userId);
        Task<DataResponse<CreateDocumentTestFilesForCompanyDTO>> CreateDocumentsByUserIdAsync(int? userId, string? fileName, string? fileData, CreateDocumentTestFilesForCompanyDTO? info);
        Task<DataResponse<UpdateDocumentTestFilesForCompanyDTO>> UpdateDocumentAsync(int? documentId, string? fileName, byte[] fileData, UpdateDocumentTestFilesForCompanyDTO? info);
        Task<DataResponse<bool>> StoredDocumentsByUserIdAsync(int? documentId);

        // === Student ===
        // Student 
        Task<DataResponse<StudentDetailForStudentDTO>> GetStudentDetailByUserIdAsync(int? userId);

        Task<DataResponse<UpdateStudentForStudentDTO>> UpdateStudentByUserIdAsync(int? userId, UpdateStudentForStudentDTO? updateInformation);
        //For Dean
        // 1. AssignLecturerForStudentsAsync
        Task<DataResponse<string>> AssignLecturerForStudentsAsync(AssignLecturerForStudentDto dto);

        // 2. GetStudentListAsync
        Task<DataResponse<PagedResponse<List<StudentListDto>>>> GetStudentListAsync(
        string? code,
        string? studentName,
        string? lecturerName,
        string? majorName,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool? isDescending);

        // KienBV - fix
        Task<DataResponse<List<StudentListDto>>> GetOjtStudentListAsync();

        // 3. GetStudentDetailsAsync
        Task<DataResponse<StudentDetailsDto>> GetStudentDetailsAsync(int studentId);
        Task<DataResponse<string>> UpdateStudentAsync(int studentId, UpdateStudentDto dto);

        // === Notificaiton ===
        // Uni, Company, Student
        Task<DataResponse<List<NotificationForUniCompanyStudentDTO>>> GetAllNotificationsByUserIdAsync(int? userId);

        // === Attendance ===
        Task<DataResponse<PagedResponse<List<AttendanceReportDto>>>> GetAttendanceReportsByStudentIdAsync(
int studentId, int pageNumber, int pageSize);

        // Mentor 
        Task<DataResponse<SetCheckInCheckOutTimeForMentorDTO>> SetCheckInCheckOutTimeAsync(int? userId, SetCheckInCheckOutTimeForMentorDTO? info);
        Task<DataResponse<bool>> CreateAutoAttendanceReportAsync(int? userId, TimeSpan? checkInTime, TimeSpan? checkOutTime);
        Task<DataResponse<UpdateAttendanceReportForMentorDTO>> UpdateAttendanceReportAsync(int? attendanceReportId, UpdateAttendanceReportForMentorDTO? info);
        Task<DataResponse<bool>> InsertAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData);
        //Task<DataResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>> ListAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData);
        Task<DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>> GetAllAttendanceReportsForMentorAsync(int? userId);

        // Mentor, Lecturer
        Task<DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>> GetAllAttendanceReportsByInternshipIdAsync(int? internshipId);

        // Lecturer
        Task<DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>> GetAllAttendanceReportsForLecturerAsync(int? userId);

        // Student
        Task<DataResponse<List<AttendanceReportsListForStudentDTO>>> GetAllAttendanceReportsForStudentAsync(int? userId);

        // ===
        // University, Company
        Task<DataResponse<CreateEvaluationForUniversityCompanyDTO>> CreateEvaluationAsync(int? userId, int? internshipId, CreateEvaluationForUniversityCompanyDTO? info);

        // University, Company, Student
        Task<DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>> GetEvaluationDetailByUserId(int? userId);
        Task<DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>> GetEvaluationDetailByInternshipId(int? internshipId);
        Task<DataResponse<GetEvaluationStudentDTO>> GetEvaluationScoreAsync(int? userId);
    }
}
