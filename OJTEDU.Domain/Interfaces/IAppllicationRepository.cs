using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IAppllicationRepository
    {
        /*
         + Application status:
        0: Rejected
        1: Reviewing
        2: Offered - accept từ company -> gửi feedback + bao gồm thông tin liên hệ, vấn đề phát sinh, ngày phỏng vấn... và offer - đợi student phản hồi accept hay reject
        3: Accept Offer
        4: Accepted Internship
        5: Internship Comfirmed
         */

        // Student 
        Task<Appllication> ApplyJobAsync(int? userId, Appllication? applyInfo, string? testFileName);
        Task<Appllication> GetApplicationDetailByIdAsync(int? applicationId); // + Comapny 
        Task<IEnumerable<Appllication>> GetAllApplicationsByUserIdAsync(int? userId);
        Task<bool> CompanyOffersActionsAsync(int? userId, int? applicationId, string? studentRejectReason, string? status);

        // Company 
        Task<IEnumerable<Appllication>> GetAllApplicationsByJobIdAsync(int? jobId);
        Task<bool> StudentApplicationsActionsAsync(int? applicationId, string? feedback, DateTime? interviewDate, string? status);
    }
}
