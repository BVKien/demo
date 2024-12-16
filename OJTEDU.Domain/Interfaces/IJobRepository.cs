using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IJobRepository
    {
        /*
         + Job status: 
        0: Deleted
        1: Active 
        2: Stored 
         */

        // Student
        Task<Dictionary<int?, int>> GetJobCountsByCompanyIdsAsync(int[] companyIds);
        Task<IEnumerable<Job>> GetAllJobsByCompanyIdAsync(int? companyId);
        Task<(IEnumerable<Job>, int totalRecords)> SearchJobsAsync(int? userId, string? title, int? majorId,
            int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize);
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<Job> GetJobDetailAsync(int? jobId); // + Company 

        // Company 
        Task<IEnumerable<Job>> GetAllJobsByUserIdAsync(int? userId);
        Task<Job> CreateJobAsync(int? userId, string? fileName, Job? info, Address? addressInfo); // Address new or select company address - done 
        Task<Job> UpdateJobAsync(int? userId, int? jobId, string? fileName, Job? info, Address? addressInfo); // check if file path nnull -> note update file paths
                                                                                                              // Thiếu: API xóa mềm lưu trữ job đã xóa mềm
        // === Document ===
        // CRUD document operations for admin
        Task<IEnumerable<Document>> GetAllDocumentsForAdminAsync(string? title, int? roleId, string? status);
        Task<Document> GetDocumentByIdForAdminAsync(int documentId);
        Task<Document> AddDocumentForAdminAsync(Document document, List<int?> roleIds);
        Task<Document> UpdateDocumentForAdminAsync(Document document);
        Task<Document> DeleteDocumentForAdminAsync(int documentId);
        Task UpdateDocumentRolesAsync(int documentId, List<int?> newRoleIds);
        // CRUD document operations for doet
        Task<IEnumerable<Document>> GetAllDocumentsForDoetAsync(string? title, int? roleId, string? status);
        Task<Document> GetDocumentByIdForDoetAsync(int documentId);
        Task<Document> AddDocumentForDoetAsync(Document document, List<int?> roleIds);
        Task<Document> UpdateDocumentForDoetAsync(Document document);
        Task<Document> DeleteDocumentForDoetAsync(int documentId);

        // Common
        Task<IEnumerable<Document>> GetAllDocumentsAsync(string role, string? title);
        Task<Document> GetDocumentDetailAsync(int? documentId, string role);

        // Guest 
        //Task<Document> GetInternshipProcessDocumentAsync();

        // Company 
        Task<IEnumerable<Document>> GetAllDocumentsByUserIdAsync(int? userId);
        Task<Document> CreateDocumentsByUserIdAsync(int? userId, string? fileName, string? fileData, Document? info);
        Task<Document> UpdateDocumentAsync(int? documentId, string? fileName, byte[] fileData, Document? info);
        Task<bool> StoredDocumentsByUserIdAsync(int? documentId);

        // === Student ===
        // Student
        Task<Student> GetStudentDetailByUserIdAsync(int? userId);
        Task<Student> UpdateStudentByUserIdAsync(int? userId, User? updateUser, Student? updateInformation, Address? updateAddress);

        //For Dean
        Task<User> GetDeanByUserIdAsync(int userId);
        Task<IEnumerable<Student>> GetStudentListAsync(
        int userId,
        string role,
        string? code,
        string? studentName,
        string? lecturerName,
        string? majorName,
        string? sortBy,
        bool? isDescending);
        Task<List<Student>> GetStudentsByIdsAsync(List<int> studentIds);
        Task UpdateStudentsAsync(List<Student> students);
        Task<Student> GetStudentDetailsByIdAsync(int studentId, int userId, string role);
        Task<IEnumerable<Student>> GetOjtStudentListAsync(int userId);
        Task<Student> GetStudentByIdAsync(int studentId);
        Task<Major> GetMajorByIdAsync(int majorId);
        Task<Semester> GetSemesterByIdAsync(int semesterId);
        Task UpdateStudentAsync(Student student);

        // === Notificaiton ===
        // Student, University, Company
        Task<Notification> CreateNotificationAsync(Notification? info);
        Task<IEnumerable<Notification>> GetAllNotificationsByUserIdAsync(int? userId);

        // === Attedance ===
        Task<List<AttendanceReport>> GetAttendanceReportsByStudentIdAsync(int studentId); // cân nhắc bỏ 1

        // Mentor 
        Task<Company> SetCheckInCheckOutTimeAsync(int? userId, Company? info);
        Task<bool> CreateAutoAttendanceReportAsync(int? userId, TimeSpan? checkInTime, TimeSpan? checkOutTime);
        Task<AttendanceReport> UpdateAttendanceReportAsync(int? attendanceReportId, AttendanceReport? info);
        Task<bool> CreateAttendanceReportFileAsync(int? userId, int[]? internshipIds, string? fileName, byte[] fileData);
        Task<bool> InsertAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData);
        //Task<IEnumerable<AttendanceReport>> ListAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData);
        Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForMentorAsync(int? userId);

        // Mentor, Lecturer
        Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsByInternshipIdAsync(int? internshipId);

        // Lecturer
        Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForLecturerAsync(int? userId);

        // Student
        Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForStudentAsync(int? userId);

        // ===
        // University, Company
        Task<Evaluation> CreateEvaluationAsync(int? userId, int? internshipId, Evaluation? info);
        Task<Evaluation> UpdateEvaluationAsync(int? userId, int? internshipId, Evaluation? info);

        // University, Company, Student
        Task<Evaluation> GetEvaluationDetailByUserId(int? userId);
        Task<Evaluation> GetEvaluationDetailByInternshipId(int? internshipId);
        Task<Evaluation> GetEvaluationScoreAsync(int? userId);
    }
}
