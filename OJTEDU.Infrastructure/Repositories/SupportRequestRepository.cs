using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;

namespace OJTEDU.Infrastructure.Repositories
{
    public class SupportRequestRepository : ISupportRequestRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly INotificationRepository _notificationRepository;
        public SupportRequestRepository(OJTEDU_DB_V1Context context, INotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
        }

        // Student
        public async Task<IEnumerable<SupportRequest>> GetAllSupportRequestByUserIdAsync(int? userId)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

                var supportRequests = await _context.SupportRequests
                    .Include(f => f.Student).ThenInclude(s => s.User).ThenInclude(s => s.Role)
                    .Include(f => f.University).ThenInclude(s => s.Role)
                    .Where(f => f.StudentId == student.StudentId)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();

                return supportRequests;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SupportRequest> GetSupportRequestDetailAsync(int? supportRequestId)
        {
            try
            {
                var supportRequest = await _context.SupportRequests
                    .Include(f => f.Student).ThenInclude(s => s.User).ThenInclude(s => s.Role)
                    .Include(f => f.University).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync(f => f.SupportRequestId == supportRequestId);

                if (supportRequest == null)
                {
                    throw new KeyNotFoundException("Not found support request.");
                }

                return supportRequest;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SupportRequest> CreateSupportRequestAsync(int? userId, SupportRequest? info)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var doet = await _context.Users.FirstOrDefaultAsync(u => u.Role.Name == "DOET");

                if (doet == null)
                {
                    throw new KeyNotFoundException("Not found Department Of Education and Training information.");
                }

                var student = await _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                // Create 
                var supportRequest = new SupportRequest
                {
                    StudentId = student.StudentId,
                    UniversityId = doet.UserId,
                    RequestTitle = info?.RequestTitle,
                    RequestContent = info?.RequestContent,
                    Status = "1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                await _context.SupportRequests.AddAsync(supportRequest);
                await _context.SaveChangesAsync();

                // Notification 
                var notificationContent = $"{student?.User?.Name} has submitted a support request to the {doet?.Name}.";
                var notiInfo = new Notification
                {
                    NotificationContent = notificationContent,
                    Image = student?.User?.Image,
                    StudentId = student?.StudentId,
                    UniversityId = doet?.UserId,
                    SupportRequestId = supportRequest?.SupportRequestId
                };

                await _notificationRepository.CreateNotificationAsync(notiInfo);

                return supportRequest;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteForStoredSupportRequestAsync(int? supportRequestId)
        {
            try
            {
                var supportRequest = await _context.SupportRequests
                    .FirstOrDefaultAsync(f => f.SupportRequestId == supportRequestId && f.Status == "1" && f.DeletedAt == null);

                if (supportRequest == null)
                {
                    throw new KeyNotFoundException("Not found support request.");
                }

                if (DateTime.Now - supportRequest.CreatedAt >= TimeSpan.FromHours(24))
                {
                    throw new Exception($"Cannot delete support request. The valid time is out of 24 hours.");
                }

                var doet = await _context.Users.FirstOrDefaultAsync(u => u.Role.Name == "DOET");

                if (doet == null)
                {
                    throw new KeyNotFoundException("Not found Department Of Education and Training information.");
                }

                var student = await _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == supportRequest.StudentId);

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                // Stored 
                supportRequest.Status = "0";
                supportRequest.UpdatedAt = DateTime.Now;
                supportRequest.DeletedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var notificationContent = $"{student?.User?.Name} has deleted a support request to the {doet?.Name}.";
                var notiInfo = new Notification
                {
                    NotificationContent = notificationContent,
                    Image = student?.User?.Image,
                    StudentId = student?.StudentId,
                    UniversityId = doet?.UserId,
                    SupportRequestId = supportRequest?.SupportRequestId
                };

                await _notificationRepository.CreateNotificationAsync(notiInfo);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //DOET
        public async Task<List<SupportRequest>> GetAllSupportRequestsForDOETAsync(string? studentName, string? universityName, string? statusFilter, string? sortBy, bool? isDescending)
        {
            IQueryable<SupportRequest> query = _context.SupportRequests
                .Include(sr => sr.Student) // Kết nối đến Student
                    .ThenInclude(s => s.User)
                .Include(s => s.University);


            if (!string.IsNullOrWhiteSpace(universityName))
            {
                universityName = universityName.ToLower();
                query = query.Where(sr => sr.University.Name.ToLower().Contains(universityName));
            }


            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                statusFilter = statusFilter.ToLower();
                query = query.Where(sr => sr.Status.ToLower() == statusFilter);
            }
            switch (sortBy?.ToLower())
            {
                case "studentname":
                    query = query.OrderBy(sr => sr.Student.User.Name);
                    break;
                case "universityname":
                    query = query.OrderBy(sr => sr.University.Name);
                    break;
                case "status":
                    query = query.OrderBy(sr => sr.Status);
                    break;
                case "createdat":
                default:
                    query = query.OrderByDescending(sr => sr.CreatedAt); // Default giảm dần cho createdAt
                    break;
            }

            // Điều chỉnh hướng sắp xếp nếu `isDescending` được chỉ định.
            if (isDescending.HasValue && isDescending.Value)
            {
                query = query.Reverse(); // Đảo ngược hướng sắp xếp nếu `isDescending = true`.
            }

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateSupportRequestForDOETAsync(int supportRequestId, string feedbackContent, int status, int universityUserId)
        {
            var supportRequest = await _context.SupportRequests
                .FirstOrDefaultAsync(sr => sr.SupportRequestId == supportRequestId);

            if (supportRequest == null)
            {
                return false;
            }

            var doet = await _context.Users.FirstOrDefaultAsync(u => u.Role.Name == "DOET");

            if (doet == null)
            {
                throw new KeyNotFoundException("Not found Department Of Education and Training information.");
            }

            var student = await _context.Students
                .Include(s => s.User).ThenInclude(s => s.Role)
                .FirstOrDefaultAsync(s => s.UserId == supportRequest.StudentId);

            if (student == null)
            {
                throw new KeyNotFoundException("Not found student.");
            }

            supportRequest.FeedbackContent = feedbackContent;
            supportRequest.Status = status.ToString();
            supportRequest.UniversityId = universityUserId; // Update university ID
            supportRequest.UpdatedAt = DateTime.Now;

            _context.SupportRequests.Update(supportRequest);
            await _context.SaveChangesAsync();

            var notificationContent = $"{doet?.Name} has responded to your support request. Please check the response for further details.";
            var notiInfo = new Notification
            {
                NotificationContent = notificationContent,
                Image = doet?.Image,
                StudentId = student?.StudentId,
                UniversityId = doet?.UserId,
                SupportRequestId = supportRequest?.SupportRequestId
            };

            await _notificationRepository.CreateNotificationAsync(notiInfo);

            return true;
        }

        public async Task<bool> DeleteSupportRequestForDOETAsync(int supportRequestId)
        {
            var supportRequest = await _context.SupportRequests
                .FirstOrDefaultAsync(sr => sr.SupportRequestId == supportRequestId);

            if (supportRequest == null || supportRequest.Status == "0") // Không xóa nếu status = 0
            {
                return false;
            }

            _context.SupportRequests.Remove(supportRequest);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
