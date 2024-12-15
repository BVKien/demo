using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AppllicationDTO;

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
    }
}
