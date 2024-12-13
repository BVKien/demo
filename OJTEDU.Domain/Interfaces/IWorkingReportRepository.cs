using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IWorkingReportRepository
    {
        /*
         + Working report status:
        1: Active 
        2: Stored
         */

        // Student 
        Task<IEnumerable<WorkingReport>> GetAllByStudentIdAsync(int? userId);
        Task<WorkingReport> CreateWorkingReportAsync(int? userId, WorkingReport? workingReportInfo, string? fileName, string? fileData);
        Task<WorkingReport> UpdateWorkingReportAsync(int? workingReportId, WorkingReport? workingReportInfo, string? fileName);
        Task<WorkingReport> GetWorkingReportDetailAsync(int? workingReportId); // + Mentor 

        //For Dean
        Task<User> GetDeanByUserIdAsync(int userId);
        Task<Student> GetStudentDetailsByIdAsync(int studentId, int userId, string role);
        Task<List<string>> GetWeeksForStudentAsync(int studentId, int? year = null);
        Task<List<WorkingReport>> GetWorkingReportsByStudentIdAsync(
        int internshipId, int userId, string role, string? sortBy, bool? isDescending, string? week, int? year = null);
        Task<bool> UpdateWorkingReportAsync(int workingReportId, int userId, string role, string? feedback, double? score);

        // Mentor 
        Task<IEnumerable<WorkingReport>> GetAllWorkingReportsByStudentId(int? studentId);
        Task<WorkingReport> CreateMentorFeedbackAsync(int? workingReportId, WorkingReport? info);
    }
}
