using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IFeedbackRepository
    {
        /*
         + Feedback status:
        0: Deleted(stored)
        1: Successfully
         */

        // Student
        Task<IEnumerable<Feedback>> GetAllFeedbacksByStudentIdAsync(int? userId);
        Task<Feedback> GetFeedbackByFeedbackIdAsync(int? feedbackId);
        Task<Feedback> CreateFeedbackAsync(int? userId, Feedback? feedbackInfo);
        Task<bool> DeleteForStoredFeedbackAsync(int? feedbackId);

        // DOET, Admin 
        // Xem danh sách các feedback gồm thông tin sinh viên, compnay,...
        // search, filter, phân trang
        //Task<IEnumerable<Feedback>> GetAllFeedbackAsync(); // filter by company combobox, major student, search student name, student code,
        //Task<Feedback> DeleteFeedbackAsync(int? feedbackId);
    }
}
