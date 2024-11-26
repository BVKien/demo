using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.FeedbackDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IFeedbackService
    {
        // Student
        Task<DataResponse<List<FeedbackListForStudentDTO>>> GetAllFeedbacksByStudentIdAsync(int? userId);
        Task<DataResponse<FeedbackDetailForStudentDTO>> GetFeedbackByFeedbackIdAsync(int? feedbackId);
        Task<DataResponse<CreateFeedbackForStudentDTO>> CreateFeedbackAsync(int? userId, CreateFeedbackForStudentDTO? info);
        Task<DataResponse<bool>> DeleteForStoredFeedbackAsync(int? feedbackId);
    }
}
