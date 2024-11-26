using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IWorkingReportService
    {
        // Student 
        Task<DataResponse<List<WorkingReportListForStudentDTO>>> GetAllByStudentIdAsync(int? userId);
        Task<DataResponse<CreateWorkingReportForStudentDTO>> CreateWorkingReportAsync(int? userId, CreateWorkingReportForStudentDTO? workingReportInfo, string? fileName, byte[] fileData);
        Task<DataResponse<UpdateWorkingReportForStudentDTO>> UpdateWorkingReportAsync(int? workingReportId, UpdateWorkingReportForStudentDTO? workingReportInfo, string? fileName, byte[] fileData);
        Task<DataResponse<WorkingReportDetailForStudentDTO>> GetWorkingReportDetailForStudentAsync(int? workingReportId); // + Mentor 

        //For Dean
        Task<DataResponse<WorkingReportResponseDTO>> GetWorkingReportsByStudentIdAsync(
        int studentId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool? isDescending);
        Task<DataResponse<string>> UpdateWorkingReportAsync(GiveFeedbackOrScoreDto dto);

        // Mentor 
        Task<DataResponse<List<WorkingReportListForMentorDTO>>> GetAllWorkingReportsByStudentId(int? studentId);
        Task<DataResponse<CreateFeedbackWorkingReportForMentorDTO>> CreateMentorFeedbackAsync(int? workingReportId, CreateFeedbackWorkingReportForMentorDTO? info);
        Task<DataResponse<WorkingReportDetailForMentorDTO>> GetWorkingReportDetailForMentorAsync(int? workingReportId); // + Mentor 
    }
}
