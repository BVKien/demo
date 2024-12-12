using Microsoft.EntityFrameworkCore;
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
                    CreatedAt = GetVietnamTime(),
                    UpdatedAt = GetVietnamTime()
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

        public async Task<IEnumerable<Notification>> GetAllNotificationsByUserIdAsync(int? userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    throw new Exception("Not found user.");
                }

                // Uni 
                if (user?.Role?.Name == "Admin" || user?.Role?.Name == "DOET"
                    || user?.Role?.Name == "Dean" || user?.Role?.Name == "Lecturer")
                {
                    var uniNotis = await _context.Notifications
                        .Include(n => n.Student).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.Company).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.University).ThenInclude(n => n.Role)
                        .Include(n => n.Application)
                        .Include(n => n.SupportRequest)
                        .Include(n => n.Feedback)
                        .Include(n => n.Feedback)
                        .Include(n => n.Message)
                        .Include(n => n.MessageGroup)
                        .Include(n => n.GroupChat)
                        .Where(n => n.UniversityId == userId)
                        .ToListAsync();

                    return uniNotis;
                }

                // Company 
                if (user?.Role?.Name == "Company" || user?.Role?.Name == "Mentor")
                {
                    var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

                    var companyNotis = await _context.Notifications
                        .Include(n => n.Student).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.Company).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.University).ThenInclude(n => n.Role)
                        .Include(n => n.Application)
                        .Include(n => n.SupportRequest)
                        .Include(n => n.Feedback)
                        .Include(n => n.Feedback)
                        .Include(n => n.Message)
                        .Include(n => n.MessageGroup)
                        .Include(n => n.GroupChat)
                        .Where(n => n.CompanyId == company.CompanyId)
                        .ToListAsync();

                    return companyNotis;
                }

                // Student 
                if (user?.Role?.Name == "Student")
                {
                    var student = await _context.Students.FirstOrDefaultAsync(c => c.UserId == userId);

                    var studentNotis = await _context.Notifications
                        .Include(n => n.Student).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.Company).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.University).ThenInclude(n => n.Role)
                        .Include(n => n.Application)
                        .Include(n => n.SupportRequest)
                        .Include(n => n.Feedback)
                        .Include(n => n.Feedback)
                        .Include(n => n.Message)
                        .Include(n => n.MessageGroup)
                        .Include(n => n.GroupChat)
                        .Where(n => n.StudentId == student.StudentId)
                        .ToListAsync();

                    return studentNotis;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
