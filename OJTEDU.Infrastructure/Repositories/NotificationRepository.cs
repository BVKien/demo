using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public NotificationRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Student, University, Company
        public async Task<Notification> CreateNotificationAsync(Notification? info)
        {
            try
            {
                if (info?.NotificationContent == null)
                {
                    throw new Exception("Notification content is required.");
                }

                var noti = new Notification
                {
                    NotificationContent = info?.NotificationContent,
                    Image = info?.Image,
                    StudentId = info?.StudentId,
                    UniversityId = info?.UniversityId,
                    CompanyId = info?.CompanyId,
                    IsRead = info?.IsRead,
                    Status = info?.Status,
                    ApplicationId = info?.ApplicationId,
                    SupportRequestId = info?.SupportRequestId,
                    CompanyProposalId = info?.CompanyProposalId,
                    FeedbackId = info?.FeedbackId,
                    MessageId = info?.MessageId,
                    GroupChatId = info?.GroupChatId,
                    MessageGroupId = info?.MessageGroupId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _context.Notifications.AddAsync(noti);
                await _context.SaveChangesAsync();

                return noti;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
