using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface INotificationRepository
    {
        /*
         + Noti status
        0: Inactive
        1: Active
         */

        // Student, University, Company
        Task<Notification> CreateNotificationAsync(Notification? info);
        Task<IEnumerable<Notification>> GetAllNotificationsByUserIdAsync(int? userId);
    }
}
