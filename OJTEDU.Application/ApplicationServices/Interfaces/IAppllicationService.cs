using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AppllicationDTO;
using static OJTEDU.Application.DTOs.DocumentDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IAppllicationService
    {
        // Student
        Task<DataResponse<ApplyJobForStudentDTO>> ApplyJobAsync(int? userId, ApplyJobForStudentDTO? applyInfo, string? testFileName);
        Task<DataResponse<AppllicationDetailForStudentDTO>> GetApplicationDetailByIdAsync(int? applicationId);
        Task<DataResponse<List<AppllicationListForStudentDTO>>> GetAllApplicationsByUserIdAsync(int? userId);
        Task<DataResponse<bool>> CompanyOffersActionsAsync(int? userId, int? applicationId, string? studentRejectReason, string? status);

        // Company 
        Task<DataResponse<List<AppllicationListForCompanyDTO>>> GetAllApplicationsByJobIdAsync(int? jobId);
        Task<DataResponse<AppllicationDetailForCompanyDTO>> GetApplicationDetailForCompanyAsync(int? applicationId);
        Task<DataResponse<bool>> StudentApplicationsActionsAsync(int? applicationId, string? feedback, DateTime? interviewDate, string? status);

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
    }
}
