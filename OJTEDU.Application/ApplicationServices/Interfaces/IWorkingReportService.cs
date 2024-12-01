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
        Task<DataResponse<List<string>>> GetWeeksForStudentAsync(int studentId, int? year = null);
        Task<DataResponse<WorkingReportResponseDTO>> GetWorkingReportsByStudentIdAsync(
        int studentId, string? sortBy, bool? isDescending, string? week, int? year = null);
        Task<DataResponse<string>> UpdateWorkingReportAsync(GiveFeedbackOrScoreDto dto);

        // Mentor 
        Task<DataResponse<List<WorkingReportListForMentorDTO>>> GetAllWorkingReportsByStudentId(int? studentId);
        Task<DataResponse<CreateFeedbackWorkingReportForMentorDTO>> CreateMentorFeedbackAsync(int? workingReportId, CreateFeedbackWorkingReportForMentorDTO? info);
        Task<DataResponse<WorkingReportDetailForMentorDTO>> GetWorkingReportDetailForMentorAsync(int? workingReportId); // + Mentor 
    }
}
