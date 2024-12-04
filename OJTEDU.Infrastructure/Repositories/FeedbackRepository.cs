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
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly INotificationRepository _notificationRepository;
        public FeedbackRepository(OJTEDU_DB_V1Context context, INotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
        }

        // Student 
        public async Task<IEnumerable<Feedback>> GetAllFeedbacksByStudentIdAsync(int? userId)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

                var feedbacks = await _context.Feedbacks
                    .Include(f => f.Student).ThenInclude(s => s.User).ThenInclude(s => s.Role)
                    .Include(f => f.Company).ThenInclude(s => s.User).ThenInclude(s => s.Role)
                    .Include(f => f.University).ThenInclude(s => s.Role)
                    .Where(f => f.StudentId == student.StudentId)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();

                return feedbacks;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Feedback> GetFeedbackByFeedbackIdAsync(int? feedbackId)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .Include(f => f.Student).ThenInclude(s => s.User).ThenInclude(s => s.Role)
                    .Include(f => f.Company).ThenInclude(s => s.User).ThenInclude(s => s.Role)
                    .Include(f => f.University).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

                if (feedback == null)
                {
                    throw new KeyNotFoundException("Not found feddback.");
                }

                return feedback;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Show infomation company, doet for student
        public async Task<Feedback> CreateFeedbackAsync(int? userId, Feedback? info)
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
                    throw new KeyNotFoundException("Not found student with student.");
                }

                var internship = await _context.Internships.FirstOrDefaultAsync(i => i.StudentId == student.StudentId);

                if (internship == null)
                {
                    throw new KeyNotFoundException("Not found internship.");
                }

                var mentor = await _context.Companies
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .FirstOrDefaultAsync(c => c.CompanyId == internship.CompanyId);

                if (mentor == null)
                {
                    throw new KeyNotFoundException("Not found mentor.");
                }

                var company = await _context.Companies
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .FirstOrDefaultAsync(c => c.UserId == mentor.User.ForCompany && c.User.Role.Name == "Company");

                if (company == null)
                {
                    throw new KeyNotFoundException("Not found company.");
                }

                // Create 
                var feedback = new Feedback
                {
                    StudentId = student.StudentId,
                    CompanyId = company.CompanyId,
                    UniversityId = doet.UserId,
                    FeedbackContent = info?.FeedbackContent,
                    Status = "1",
                    CreatedAt = GetVietnamTime(),
                    UpdatedAt = GetVietnamTime(),
                };

                await _context.Feedbacks.AddAsync(feedback);
                await _context.SaveChangesAsync();

                // Notification 
                var notificationContent = $"{student?.User?.Name} has submitted feedback about their internship at {company?.User?.Name} to {doet?.Name}.";
                var notiInfo = new Notification
                {
                    NotificationContent = notificationContent,
                    Image = student?.User?.Image,
                    StudentId = student?.StudentId,
                    UniversityId = doet?.UserId,
                    CompanyId = company?.CompanyId,
                    FeedbackId = feedback?.FeedbackId,
                };

                await _notificationRepository.CreateNotificationAsync(notiInfo);

                return feedback;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteForStoredFeedbackAsync(int? feedbackId)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId && f.Status == "1" && f.DeletedAt == null);

                if (feedback == null)
                {
                    throw new KeyNotFoundException("Not found feedback.");
                }

                if (GetVietnamTime() - feedback.CreatedAt >= TimeSpan.FromHours(24))
                {
                    throw new Exception("Cannot delete feedback. The valid time is out of 24 hours.");
                }

                var doet = await _context.Users.FirstOrDefaultAsync(u => u.Role.Name == "DOET");

                if (doet == null)
                {
                    throw new KeyNotFoundException("Not found Department Of Education and Training information.");
                }

                var student = await _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync(s => s.StudentId == feedback.StudentId);

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.CompanyId == feedback.CompanyId);

                if (company == null)
                {
                    throw new KeyNotFoundException("Not found company.");
                }

                // Stored 
                feedback.Status = "0";
                feedback.UpdatedAt = GetVietnamTime();
                feedback.DeletedAt = GetVietnamTime();

                await _context.SaveChangesAsync();

                // Notification 
                var notificationContent = $"{student?.User?.Name} has submitted feedback about their internship at {company?.User?.Name} to {doet?.Name}.";
                var notiInfo = new Notification
                {
                    NotificationContent = notificationContent,
                    Image = student?.User?.Image,
                    StudentId = student?.StudentId,
                    UniversityId = doet?.UserId,
                    CompanyId = company?.CompanyId,
                    FeedbackId = feedback?.FeedbackId,
                };

                await _notificationRepository.CreateNotificationAsync(notiInfo);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private DateTime GetVietnamTime()
        {
            return DateTime.UtcNow.AddHours(7);
        }
    }
}
