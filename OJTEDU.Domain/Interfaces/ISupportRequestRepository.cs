using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface ISupportRequestRepository
    {
        /*
        + Support request status:
        0: Rejected
        1: Reviewing
        2: Accepted
        */

        // Student 
        Task<IEnumerable<SupportRequest>> GetAllSupportRequestByUserIdAsync(int? userId);
        Task<SupportRequest> GetSupportRequestDetailAsync(int? supportRequestId);
        Task<SupportRequest> CreateSupportRequestAsync(int? userId, SupportRequest? info);
        Task<bool> DeleteForStoredSupportRequestAsync(int? supportRequestId);
        Task<List<SupportRequest>> GetAllSupportRequestsForDOETAsync(string? studentName, string? universityName, string? statusFilter, string? sortBy, bool? isDescending);
        Task<bool> UpdateSupportRequestForDOETAsync(int supportRequestId, string feedbackContent, int status, int universityUserId);
        Task<bool> DeleteSupportRequestForDOETAsync(int supportRequestId);


        // DOET, Admin
        // Xem danh sách support request 
        // phản hồi 
        // search, filter, phân trang
        //Task<IEnumerable<SupportRequest>> GetAllSupportRequestAsync(); // search, filter, paging 
        //Task<SupportRequest> FeedbackSupportRequestAsync(int? supportRequestId, SupportRequest? info);
        //Task<bool> DeleteSupportRequestAsync(int? supportRequestId);
    }
}
