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
    public class GroupChatRepository : IGroupChatRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly INotificationRepository _notificationRepository;
        public GroupChatRepository(OJTEDU_DB_V1Context context, INotificationRepository notificationRepository)
        {
            _context = context;
            _notificationRepository = notificationRepository;
        }

        // Admin, DOET, Dean, Lecturer, Mentor
        public async Task<GroupChat> CreateGroupChatAsync(int? userId, GroupChat? groupChatInfo)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                // Mentor 
                var mentor = await _context.Companies.Where(m => m.UserId == userId).FirstOrDefaultAsync();
                if (mentor != null)
                {
                    groupChatInfo.MentorId = mentor.CompanyId;

                    // Create group chat 
                    var groupChatMentor = new GroupChat
                    {
                        GroupName = groupChatInfo.GroupName,
                        MentorId = groupChatInfo.MentorId,
                        IsAdmin = true,
                        Status = "1",
                        CreatedAt = GetVietnamTime(),
                        UpdatedAt = GetVietnamTime(),
                    };

                    _context.GroupChats.Add(groupChatMentor);
                    await _context.SaveChangesAsync();

                    // Create new meber infomation
                    var memberInfo = new MessageGroup
                    {
                        GroupChatId = groupChatMentor.GroupChatId,
                        MentorId = groupChatInfo.MentorId,
                        JoinAt = GetVietnamTime(),
                        IsAdmin = true,
                        IsRead = false,
                        Status = "1",
                        CreatedAt = GetVietnamTime(),
                        UpdatedAt = GetVietnamTime(),
                    };

                    _context.MessageGroups.Add(memberInfo);
                    await _context.SaveChangesAsync();

                    // Notification 
                    var notificationContent = $"{mentor?.User?.Name} has created a new group chat named {groupChatMentor?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = mentor?.User?.Image,
                        CompanyId = mentor?.CompanyId,
                        GroupChatId = groupChatMentor?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return groupChatMentor;
                }

                // University 
                var university = await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
                if (university != null)
                {
                    groupChatInfo.UniversityId = university.UserId;
                }

                // Create group chat 
                var groupChat = new GroupChat
                {
                    GroupName = groupChatInfo.GroupName,
                    UniversityId = groupChatInfo.UniversityId,
                    IsAdmin = true,
                    Status = "1",
                    CreatedAt = GetVietnamTime(),
                    UpdatedAt = GetVietnamTime(),
                };

                _context.GroupChats.Add(groupChat);
                await _context.SaveChangesAsync();

                // Create new meber infomation
                var uniMemberInfo = new MessageGroup
                {
                    GroupChatId = groupChat.GroupChatId,
                    UniversityId = groupChat.UniversityId,
                    JoinAt = GetVietnamTime(),
                    IsAdmin = true,
                    IsRead = false,
                    Status = "1",
                    CreatedAt = GetVietnamTime(),
                    UpdatedAt = GetVietnamTime(),
                };

                _context.MessageGroups.Add(uniMemberInfo);
                await _context.SaveChangesAsync();

                // Notification 
                var notificationContentValid = $"{university?.Name} has created a new group chat named {groupChat?.GroupName}.";
                var notiInfoValid = new Notification
                {
                    NotificationContent = notificationContentValid,
                    Image = university?.Image,
                    UniversityId = university?.UserId,
                    GroupChatId = groupChat?.GroupChatId,
                };

                await _notificationRepository.CreateNotificationAsync(notiInfoValid);

                return groupChat;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<GroupChat>> SearchGroupChatByNameAsync(int? userId, string? groupName)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var query = _context.GroupChats.AsQueryable();

                // Mentor 
                var mentor = await _context.Companies.Where(m => m.UserId == userId).FirstOrDefaultAsync();
                if (mentor != null)
                {
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        query = query.Where(g => g.GroupName.Contains(groupName) && g.MentorId == mentor.CompanyId);
                    }

                    if (string.IsNullOrEmpty(groupName))
                    {
                        query = query.Where(g => g.MentorId == mentor.CompanyId);
                    }

                    var mentorGroupChats = await query.ToListAsync();

                    return mentorGroupChats;
                }

                // University 
                var university = await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(groupName))
                {
                    query = query.Where(g => g.GroupName.Contains(groupName) && g.UniversityId == userId);
                }

                if (string.IsNullOrEmpty(groupName))
                {
                    query = query.Where(g => g.UniversityId == userId);
                }

                var groupChats = await query.ToListAsync();

                return groupChats;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<GroupChat>> GetAllGroupChatByUserIdAsync(int? userId)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var query = _context.GroupChats.AsQueryable();

                // Mentor 
                var mentor = await _context.Companies.Where(m => m.UserId == userId).FirstOrDefaultAsync();
                if (mentor != null)
                {
                    query = query.Where(g => g.MentorId == mentor.CompanyId);

                    var mentorGroupChats = await query.ToListAsync();

                    return mentorGroupChats;
                }

                // University 
                var university = await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();

                query = query.Where(g => g.UniversityId == userId);

                var groupChats = await query.ToListAsync();

                return groupChats;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GroupChat> UpdateGroupChatAsync(int? groupChatId, GroupChat? groupChatInfo)
        {
            try
            {
                var groupChatExists = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == groupChatId);
                if (groupChatExists == null)
                {
                    throw new KeyNotFoundException("Not found group chat.");
                }

                // Update 
                groupChatExists.GroupName = groupChatInfo?.GroupName;
                groupChatExists.UpdatedAt = GetVietnamTime();

                await _context.SaveChangesAsync();

                if (groupChatExists?.MentorId != null)
                {
                    var mentor = await _context.Companies
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.CompanyId == groupChatExists.MentorId);

                    if (mentor == null)
                    {
                        throw new Exception("Not found mentor.");
                    }

                    // Notification 
                    var notificationContent = $"{mentor?.User?.Name} has updated a group chat to {groupChatExists?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = mentor?.User?.Image,
                        CompanyId = mentor?.CompanyId,
                        GroupChatId = groupChatExists?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);
                }

                if (groupChatExists?.UniversityId != null)
                {
                    var university = await _context.Users.FirstOrDefaultAsync(u => u.UserId == groupChatExists.UniversityId);

                    if (university == null)
                    {
                        throw new Exception("Not found university information.");
                    }

                    // Notification 
                    var notificationContent = $"{university?.Name} has updated a group chat to {groupChatExists?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = university?.Image,
                        UniversityId = university?.UserId,
                        GroupChatId = groupChatExists?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);
                }

                return groupChatExists;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<GroupChat> ClearMessageHistoryGroupChatAsync(int? groupChatId)
        {
            throw new NotImplementedException();
        }
        private DateTime GetVietnamTime()
        {
            return DateTime.UtcNow.AddHours(7);
        }
    }
}
