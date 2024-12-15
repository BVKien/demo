using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class MessageGroupRepository : IMessageGroupRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _messageFileDirectory = "wwwroot/uploads/messagesgroup/messagefiles/";
        private readonly string _imageFileDirectory = "wwwroot/uploads/messagesgroup/images/";
        private readonly INotificationRepository _notificationRepository;

        public MessageGroupRepository(OJTEDU_DB_V1Context context, INotificationRepository notificationRepository)
        {
            _context = context;

            if (!Directory.Exists(_messageFileDirectory))
            {
                Directory.CreateDirectory(_messageFileDirectory);
            }

            if (!Directory.Exists(_imageFileDirectory))
            {
                Directory.CreateDirectory(_imageFileDirectory);
            }
            _notificationRepository = notificationRepository;
        }

        // Admin, DOET, Dean, Lecturer, Mentor
        public async Task<MessageGroup> CreateMemberInMessageGroupAsync(int? userId, int? memberId, MessageGroup? messageGroupInfo)
        {
            try
            {
                var userExists = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (userExists == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var groupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == messageGroupInfo.GroupChatId);
                if (groupChat == null)
                {
                    throw new KeyNotFoundException("Not found group chat.");
                }

                // Admin
                if (userExists?.Role?.Name == "Admin")
                {
                    // Check user -> role 
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == memberId);

                    // DOET, Dean, Lecturer
                    if (user?.Role?.Name == "DOET" || user?.Role?.Name == "Dean" || user?.Role?.Name == "Lecturer")
                    {
                        var university = await _context.Users.Include(u => u.Role)
                            .FirstOrDefaultAsync(u => u.UserId == memberId
                            && (u.Role.Name == "DOET" || u.Role.Name == "Dean" || u.Role.Name == "Lecturer"));

                        var universityId = university?.UserId;

                        // Create new meber infomation
                        var uniMemberInfo = new MessageGroup
                        {
                            GroupChatId = messageGroupInfo?.GroupChatId,
                            UniversityId = universityId,
                            JoinAt = DateTime.Now,
                            IsAdmin = false,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        _context.MessageGroups.Add(uniMemberInfo);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has added {university?.Name} to the group chat {groupChat?.GroupName}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            UniversityId = universityId,
                            GroupChatId = groupChat?.GroupChatId,
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);

                        return uniMemberInfo;
                    }
                }

                // DOET
                if (userExists?.Role?.Name == "DOET")
                {
                    // Check user -> role 
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == memberId);

                    // Dean, Lecturer
                    if (user?.Role?.Name == "Dean" || user?.Role?.Name == "Lecturer")
                    {
                        var university = await _context.Users.Include(u => u.Role)
                            .FirstOrDefaultAsync(u => u.UserId == memberId
                            && (u.Role.Name == "Dean" || u.Role.Name == "Lecturer"));

                        var universityId = university?.UserId;

                        // Create new meber infomation
                        var uniMemberInfo = new MessageGroup
                        {
                            GroupChatId = messageGroupInfo?.GroupChatId,
                            UniversityId = universityId,
                            JoinAt = DateTime.Now,
                            IsAdmin = false,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        _context.MessageGroups.Add(uniMemberInfo);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has added {university?.Name} to the group chat {groupChat?.GroupName}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            UniversityId = universityId,
                            GroupChatId = groupChat?.GroupChatId,
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);

                        return uniMemberInfo;
                    }
                }

                // Dean
                if (userExists?.Role?.Name == "Dean")
                {
                    // Check user -> role 
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == memberId);

                    // Lecturer
                    if (user?.Role?.Name == "Lecturer")
                    {
                        var university = await _context.Users.Include(u => u.Role)
                            .FirstOrDefaultAsync(u => u.UserId == memberId
                            && (u.Role.Name == "Lecturer"));

                        var universityId = university?.UserId;

                        // Create new meber infomation
                        var uniMemberInfo = new MessageGroup
                        {
                            GroupChatId = messageGroupInfo?.GroupChatId,
                            UniversityId = universityId,
                            JoinAt = DateTime.Now,
                            IsAdmin = false,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        _context.MessageGroups.Add(uniMemberInfo);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has added {university?.Name} to the group chat {groupChat?.GroupName}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            UniversityId = universityId,
                            GroupChatId = groupChat?.GroupChatId,
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);

                        return uniMemberInfo;
                    }
                }

                // Lecturer
                if (userExists?.Role?.Name == "Lecturer")
                {
                    // Check user -> role 
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == memberId);

                    // Student
                    if (user?.Role?.Name == "Student")
                    {
                        var student = await _context.Students
                            .Include(s => s.User.Role)
                            .FirstOrDefaultAsync(s => s.UserId == memberId
                            && (s.User.Role.Name == "Student"));

                        var studentId = student?.StudentId;

                        // Create new meber infomation
                        var studentMemberInfo = new MessageGroup
                        {
                            GroupChatId = messageGroupInfo?.GroupChatId,
                            StudentId = studentId,
                            JoinAt = DateTime.Now,
                            IsAdmin = false,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        _context.MessageGroups.Add(studentMemberInfo);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has added {student?.User?.Name} to the group chat {groupChat?.GroupName}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            StudentId = studentId,
                            GroupChatId = groupChat?.GroupChatId,
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);

                        return studentMemberInfo;
                    }
                }

                // Mentor
                // Check user -> role 
                var studentValid = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == memberId);

                // Student
                var studentInfo = await _context.Students
                    .Include(s => s.User.Role)
                    .FirstOrDefaultAsync(s => s.UserId == memberId
                    && (s.User.Role.Name == "Student"));

                var studentValidId = studentInfo?.StudentId;

                // Create new meber infomation
                var studentMemberValidInfo = new MessageGroup
                {
                    GroupChatId = messageGroupInfo?.GroupChatId,
                    StudentId = studentValidId,
                    JoinAt = DateTime.Now,
                    IsAdmin = false,
                    IsRead = false,
                    Status = "1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _context.MessageGroups.Add(studentMemberValidInfo);
                await _context.SaveChangesAsync();

                // Notification 
                var notificationContentValid = $"{userExists?.Name} has added {studentInfo?.User?.Name} to the group chat {groupChat?.GroupName}.";
                var notiInfoValid = new Notification
                {
                    NotificationContent = notificationContentValid,
                    Image = userExists?.Image,
                    StudentId = studentValidId,
                    GroupChatId = groupChat?.GroupChatId,
                };

                await _notificationRepository.CreateNotificationAsync(notiInfoValid);

                return studentMemberValidInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MessageGroup> DeleteMemberInMessageGroupAsync(int? userId, int? groupChatId, int? memberId)
        {
            try
            {
                var userExists = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);

                if (userExists == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var groupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == groupChatId);

                if (groupChat == null)
                {
                    throw new Exception("Not found group chat.");
                }

                // Admin
                if (userExists?.Role?.Name == "Admin")
                {
                    // Check user -> role 
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == memberId);

                    // DOET, Dean, Lecturer
                    if (user?.Role?.Name == "DOET" || user?.Role?.Name == "Dean" || user?.Role?.Name == "Lecturer")
                    {
                        var memberDeleted = await _context.MessageGroups.FirstOrDefaultAsync(m => m.UniversityId == memberId && m.OutAt != null);

                        if (memberDeleted != null)
                        {
                            throw new KeyNotFoundException("Member is deleted.");
                        }

                        // Update meber infomation
                        var member = await _context.MessageGroups.FirstOrDefaultAsync(m => m.UniversityId == memberId && m.JoinAt != null);

                        member.OutAt = DateTime.Now;
                        member.Status = "0";
                        member.UpdatedAt = DateTime.Now;

                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has removed {user?.Name} from the group chat {groupChat?.GroupName}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            UniversityId = member?.UniversityId,
                            GroupChatId = groupChat?.GroupChatId,
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);

                        return member;
                    }
                }

                // DOET
                if (userExists?.Role?.Name == "DOET")
                {
                    // Check user -> role 
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == memberId);

                    // Dean, Lecturer
                    if (user?.Role?.Name == "Dean" || user?.Role?.Name == "Lecturer")
                    {
                        var memberDeleted = await _context.MessageGroups.FirstOrDefaultAsync(m => m.UniversityId == memberId && m.OutAt != null);

                        if (memberDeleted != null)
                        {
                            throw new KeyNotFoundException("Member is deleted.");
                        }

                        // Update meber infomation
                        var member = await _context.MessageGroups.FirstOrDefaultAsync(m => m.UniversityId == memberId && m.JoinAt != null);

                        member.OutAt = DateTime.Now;
                        member.Status = "0";
                        member.UpdatedAt = DateTime.Now;

                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has removed {user?.Name} from the group chat {groupChat?.GroupName}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            UniversityId = member?.UniversityId,
                            GroupChatId = groupChat?.GroupChatId,
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);

                        return member;
                    }
                }

                // Dean
                if (userExists?.Role?.Name == "Dean")
                {
                    // Check user -> role 
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == memberId);

                    // Lecturer
                    if (user?.Role?.Name == "Lecturer")
                    {
                        var memberDeleted = await _context.MessageGroups.FirstOrDefaultAsync(m => m.UniversityId == memberId && m.OutAt != null);

                        if (memberDeleted != null)
                        {
                            throw new KeyNotFoundException("Member is deleted.");
                        }

                        // Update meber infomation
                        var member = await _context.MessageGroups.FirstOrDefaultAsync(m => m.UniversityId == memberId && m.JoinAt != null);

                        member.OutAt = DateTime.Now;
                        member.Status = "0";
                        member.UpdatedAt = DateTime.Now;

                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has removed {user?.Name} from the group chat {groupChat?.GroupName}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            UniversityId = member?.UniversityId,
                            GroupChatId = groupChat?.GroupChatId,
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);

                        return member;
                    }
                }

                // Lecturer
                if (userExists?.Role?.Name == "Lecturer")
                {
                    // Check user -> role 
                    var user = await _context.Users
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserId == memberId);

                    // Lecturer
                    if (user?.Role?.Name == "Student")
                    {
                        var memberDeleted = await _context.MessageGroups.FirstOrDefaultAsync(m => m.StudentId == memberId && m.OutAt != null);

                        if (memberDeleted != null)
                        {
                            throw new KeyNotFoundException("Member is deleted.");
                        }

                        // Update meber infomation
                        var member = await _context.MessageGroups.FirstOrDefaultAsync(m => m.StudentId == memberId && m.JoinAt != null);

                        member.OutAt = DateTime.Now;
                        member.Status = "0";
                        member.UpdatedAt = DateTime.Now;

                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has removed {user?.Name} from the group chat {groupChat?.GroupName}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            UniversityId = member?.UniversityId,
                            GroupChatId = groupChat?.GroupChatId,
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);

                        return member;
                    }
                }

                // Check user -> role 
                var userValid = await _context.Students
                    .Include(u => u.User).ThenInclude(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == memberId);

                // Student
                var memberDeletedValid = await _context.MessageGroups.FirstOrDefaultAsync(m => m.StudentId == userValid.StudentId && m.OutAt != null);

                if (memberDeletedValid != null)
                {
                    throw new KeyNotFoundException("Member is deleted.");
                }

                // Update meber infomation
                var memberValid = await _context.MessageGroups.FirstOrDefaultAsync(m => m.StudentId == memberId && m.JoinAt != null);

                memberValid.OutAt = DateTime.Now;
                memberValid.Status = "0";
                memberValid.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                // Notification 
                var notificationContentValid = $"{userExists?.Name} has removed {userValid?.User?.Name} from the group chat {groupChat?.GroupName}.";
                var notiInfoValid = new Notification
                {
                    NotificationContent = notificationContentValid,
                    Image = userExists?.Image,
                    StudentId = memberDeletedValid?.StudentId,
                    GroupChatId = groupChat?.GroupChatId,
                };

                await _notificationRepository.CreateNotificationAsync(notiInfoValid);

                return memberValid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> SearchAllMemberAndGroupForUserAsync(int? userId, string? name)
        {
            try
            {
                var userExists = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);

                if (userExists == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                // Admin, DOET
                if (userExists?.Role?.Name == "Admin" || userExists?.Role?.Name == "DOET")
                {
                    // DOET, Dean, Lecturer
                    var query = _context.Users.Include(u => u.Role).Where(u => u.Role.Name == "Admin" || u.Role.Name == "DOET" || u.Role.Name == "Dean"
                        || u.Role.Name == "Lecturer" || u.Role.Name == "Company" || u.Role.Name == "Mentor");

                    if (!string.IsNullOrEmpty(name))
                    {
                        query = query.Where(u => u.Name.Contains(name) || u.UserCode.Contains(name));
                    }

                    // Handle remove duplicate results
                    var results = await query
                        .GroupBy(u => u.UserCode)
                        .Select(g => g.First())
                        .ToListAsync();

                    // Combine
                    var members = results
                        .GroupBy(u => u.UserCode)
                        .Select(g => g.First())
                        .Select(u => new User
                        {
                            UserId = u.UserId,
                            Name = u.Name,
                            Image = u.Image,
                            UserCode = u.UserCode
                        })
                        .ToList();

                    // Group chat
                    var groupQuery = _context.GroupChats
                        .Include(g => g.University).ThenInclude(u => u.Role)
                        .Include(g => g.Mentor).ThenInclude(m => m.User).ThenInclude(u => u.Role)
                        .Where(g => g.UniversityId == userExists.UserId)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(name))
                    {
                        groupQuery = groupQuery.Where(g => g.GroupName.Contains(name));
                    }

                    // Admin
                    if (userExists.Role.Name == "Admin")
                    {
                        groupQuery = groupQuery.Where(g => g.University.Role.Name == "Admin");
                    }

                    // DOET
                    if (userExists.Role.Name == "DOET")
                    {
                        groupQuery = groupQuery.Where(g => g.University.Role.Name == "DOET");
                    }

                    var groups = await groupQuery
                        .Select(g => new User
                        {
                            Name = g.GroupName
                        })
                        .ToListAsync();

                    // Combine
                    var combinedResults = members.Cast<dynamic>().Concat(groups.Cast<dynamic>()).ToList();

                    return combinedResults;
                }

                // Dean
                if (userExists?.Role?.Name == "Dean")
                {
                    var query = _context.Users.Include(u => u.Role).Where(u => u.Role.Name == "Admin" || u.Role.Name == "DOET" || u.Role.Name == "Dean"
                        || u.Role.Name == "Lecturer" || u.Role.Name == "Mentor");

                    if (!string.IsNullOrEmpty(name))
                    {
                        query = query.Where(u => u.Name.Contains(name) || u.UserCode.Contains(name));
                    }

                    // Handle remove duplicate results
                    var results = await query
                        .GroupBy(u => u.UserCode)
                        .Select(g => g.First())
                        .ToListAsync();

                    // Combine
                    var members = results
                        .GroupBy(u => u.UserCode)
                        .Select(g => g.First())
                        .Select(u => new User
                        {
                            UserId = u.UserId,
                            Name = u.Name,
                            Image = u.Image,
                            UserCode = u.UserCode
                        })
                        .ToList();

                    // Group chat
                    var groupQuery = _context.GroupChats
                        .Include(g => g.University).ThenInclude(u => u.Role)
                        .Include(g => g.Mentor).ThenInclude(m => m.User).ThenInclude(u => u.Role)
                        .Where(g => g.UniversityId == userExists.UserId)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(name))
                    {
                        groupQuery = groupQuery.Where(g => g.GroupName.Contains(name));
                    }

                    // Dean
                    if (userExists.Role.Name == "Dean")
                    {
                        groupQuery = groupQuery.Where(g => g.University.Role.Name == "Dean");
                    }

                    var groups = await groupQuery
                        .Select(g => new User
                        {
                            Name = g.GroupName
                        })
                        .ToListAsync();

                    // Combine
                    var combinedResults = members.Cast<dynamic>().Concat(groups.Cast<dynamic>()).ToList();

                    return combinedResults;
                }

                // Lecture
                if (userExists?.Role?.Name == "Lecturer")
                {
                    // Student
                    var query = _context.Students
                        .Include(s => s.User).ThenInclude(s => s.Role)
                        .Include(s => s.Lecturer).ThenInclude(s => s.Role)
                        .AsQueryable();

                    if (string.IsNullOrEmpty(name))
                    {
                        query = query.Where(s => s.LecturerId == userExists.UserId);
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        query = query.Where(s => s.LecturerId == userExists.UserId
                            && (s.User.Name.Contains(name) || s.User.UserCode.Contains(name)));
                    }

                    // Admin, Lecturer, Dean
                    var forLecQuery = _context.Users.Include(u => u.Role).AsQueryable();

                    if (string.IsNullOrEmpty(name))
                    {
                        forLecQuery = forLecQuery.Where(u => u.Role.Name == "Dean" || u.Role.Name == "Lecturer" || u.Role.Name == "Admin");
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        forLecQuery = forLecQuery.Where(u => u.Name.Contains(name) || u.UserCode.Contains(name) && (u.Role.Name == "Dean" || u.Role.Name == "Lecturer" || u.Role.Name == "Admin"));
                    }

                    // Mentor
                    var mentorInfo = _context.Internships
                        .Where(i => i.LecturerId == userExists.UserId)
                        .FirstOrDefault();

                    var mentorQuery = _context.Companies
                        .Include(c => c.User).ThenInclude(c => c.Role)
                        .Where(c => c.CompanyId == mentorInfo.CompanyId)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(name))
                    {
                        mentorQuery = mentorQuery.Where(m => m.User.Name.Contains(name) || m.User.UserCode.Contains(name));
                    }

                    // Student
                    var stdQueryValid = _context.Users
                        .Include(u => u.Role)
                        .Where(u => u.Role.Name == "Student");

                    if (!string.IsNullOrEmpty(name))
                    {
                        stdQueryValid = stdQueryValid.Where(u => u.Name.Contains(name) || u.UserCode.Contains(name));
                    }

                    // Handle remove duplicate results
                    var stdResults = await stdQueryValid
                        .GroupBy(s => s.UserCode)
                        .Select(g => g.First())
                        .ToListAsync();

                    // Combine
                    var combined = query
                        .Select(u => new User
                        {
                            UserId = u.User.UserId,
                            Name = u.User.Name,
                            Image = u.User.Image,
                            UserCode = u.User.UserCode,
                        })
                        .Concat(stdQueryValid.Select(m => new User
                        {
                            Name = m.Name,
                            Image = m.Image,
                            UserCode = m.UserCode,
                        }))
                        .Concat(forLecQuery.Select(m => new User
                        {
                            Name = m.Name,
                            Image = m.Image,
                            UserCode = m.UserCode,
                        }))
                        .Concat(mentorQuery.Select(m => new User
                        {
                            Name = m.User.Name,
                            Image = m.User.Image,
                            UserCode = m.User.UserCode,
                        }))
                        .GroupBy(u => u.UserCode)
                        .Select(g => g.First())
                        .ToList();

                    var resp = await Task.FromResult(combined);

                    // Group chat
                    var groupQuery = _context.GroupChats
                        .Include(g => g.University).ThenInclude(u => u.Role)
                        .Include(g => g.Mentor).ThenInclude(m => m.User).ThenInclude(u => u.Role)
                        .Where(g => g.UniversityId == userExists.UserId)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(name))
                    {
                        groupQuery = groupQuery.Where(g => g.GroupName.Contains(name));
                    }

                    // Lecturer
                    if (userExists.Role.Name == "Lecturer")
                    {
                        groupQuery = groupQuery.Where(g => g.University.Role.Name == "Lecturer");
                    }

                    var groups = await groupQuery
                        .Select(g => new User
                        {
                            Name = g.GroupName
                        })
                        .ToListAsync();

                    // Combine
                    var combinedResults = resp.Cast<dynamic>().Concat(groups.Cast<dynamic>()).ToList();

                    return combinedResults;
                }

                // Mentor
                if (userExists?.Role?.Name == "Mentor")
                {
                    var mentor = await _context.Companies
                        .Where(m => m.UserId == userExists.UserId)
                        .FirstOrDefaultAsync();

                    // Mentor query
                    var mentorQuery = _context.Users
                        .Where(u => u.ForCompany == mentor.User.ForCompany && u.Role.Name == "Mentor");

                    if (!string.IsNullOrEmpty(name))
                    {
                        mentorQuery = mentorQuery.Where(u => u.Name.Contains(name) || u.UserCode.Contains(name));
                    }

                    // Handle remove duplicate results
                    var mentorResults = await mentorQuery
                        .GroupBy(u => u.UserCode)
                        .Select(g => g.First())
                        .ToListAsync();

                    // Company query
                    var companyQuery = _context.Users
                        .Where(u => u.UserId == mentor.User.ForCompany && u.Role.Name == "Company");

                    if (!string.IsNullOrEmpty(name))
                    {
                        companyQuery = companyQuery.Where(u => u.Name.Contains(name) || u.UserCode.Contains(name));
                    }

                    // Handle remove duplicate results
                    var companyResults = await companyQuery
                        .GroupBy(u => u.UserCode)
                        .Select(g => g.First())
                        .ToListAsync();

                    // Lecturer query
                    var lecQuery = _context.Internships
                        .Include(i => i.Lecturer)
                            .ThenInclude(u => u.Role)
                        .Where(i => i.CompanyId == mentor.CompanyId)
                        .Select(i => i.Lecturer);

                    if (!string.IsNullOrEmpty(name))
                    {
                        lecQuery = lecQuery.Where(l => l.Name.Contains(name) || l.UserCode.Contains(name));
                    }

                    // Handle remove duplicate results
                    var lecturerResults = await lecQuery
                        .GroupBy(l => l.UserCode)
                        .Select(g => g.First())
                        .ToListAsync();

                    // Student query
                    var studentQuery = _context.Internships
                        .Include(i => i.Student)
                            .ThenInclude(s => s.User)
                            .ThenInclude(u => u.Role)
                        .Where(i => i.CompanyId == mentor.CompanyId)
                        .Select(i => i.Student.User);

                    if (!string.IsNullOrEmpty(name))
                    {
                        studentQuery = studentQuery.Where(s => s.Name.Contains(name) || s.UserCode.Contains(name));
                    }

                    // Handle remove duplicate results
                    var studentResults = await studentQuery
                        .GroupBy(s => s.UserCode)
                        .Select(g => g.First())
                        .ToListAsync();

                    // Combine
                    var resp = mentorResults
                        .Concat(companyResults)
                        .Concat(lecturerResults)
                        .Concat(studentResults)
                        .GroupBy(u => u.UserCode)
                        .Select(g => g.First())
                        .Select(u => new User
                        {
                            UserId = u.UserId,
                            Name = u.Name,
                            Image = u.Image,
                            UserCode = u.UserCode
                        })
                        .ToList();

                    // Group chat
                    var groupQuery = _context.GroupChats
                        .Include(g => g.University).ThenInclude(u => u.Role)
                        .Include(g => g.Mentor).ThenInclude(m => m.User).ThenInclude(u => u.Role)
                        .Where(g => g.MentorId == mentor.CompanyId)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(name))
                    {
                        groupQuery = groupQuery.Where(g => g.GroupName.Contains(name));
                    }

                    // Mentor
                    if (userExists.Role.Name == "Mentor")
                    {
                        groupQuery = groupQuery.Where(g => g.Mentor.User.Role.Name == "Mentor");
                    }

                    var groups = await groupQuery
                        .Select(g => new User
                        {
                            Name = g.GroupName
                        })
                        .ToListAsync();

                    // Combine
                    var combinedResults = resp.Cast<dynamic>().Concat(groups.Cast<dynamic>()).ToList();

                    return combinedResults;
                }

                // Student
                var student = await _context.Students
                    .Where(s => s.UserId == userExists.UserId)
                    .FirstOrDefaultAsync();

                var stuQueryValid = _context.Students.Include(s => s.User).ThenInclude(s => s.Role)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    stuQueryValid = stuQueryValid.Where(u => u.User.Name.Contains(name) || u.User.UserCode.Contains(name));
                }

                // Student
                var studentQueryValid = _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.Name == "Student")
                    .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    studentQueryValid = studentQueryValid.Where(u => u.Name.Contains(name) || u.UserCode.Contains(name));
                }

                var studentInfo = await _context.Students
                    .Include(s => s.User)
                    .ThenInclude(s => s.Role)
                    .Where(s => s.UserId == userExists.UserId)
                    .FirstOrDefaultAsync();

                // Lecturer
                var lecQueryValid = _context.Users.Include(u => u.Role)
                    .Where(u => u.UserId == student.LecturerId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    lecQueryValid = lecQueryValid.Where(u => u.Name.Contains(name) || u.UserCode.Contains(name));
                }

                // Mentor 
                var internshipInfo = _context.Internships
                    .Where(i => i.StudentId == student.StudentId)
                    .FirstOrDefault();

                var mentorQueryValid = _context.Companies
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .Where(c => c.CompanyId == internshipInfo.CompanyId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    mentorQueryValid = mentorQueryValid.Where(m => m.User.Name.Contains(name) || m.User.UserCode.Contains(name));
                }

                // Combine
                var combinedValid = lecQueryValid
                    .Select(u => new User
                    {
                        UserId = u.UserId,
                        Name = u.Name,
                        Image = u.Image,
                        UserCode = u.UserCode,
                    })
                    .Concat(mentorQueryValid.Select(m => new User
                    {
                        Name = m.User.Name,
                        Image = m.User.Image,
                        UserCode = m.User.UserCode,
                    }))
                    .Concat(stuQueryValid.Select(m => new User
                    {
                        UserId = m.User.UserId,
                        Name = m.User.Name,
                        Image = m.User.Image,
                        UserCode = m.User.UserCode,
                    }))
                    .Concat(studentQueryValid.Select(m => new User
                    {
                        UserId = m.UserId,
                        Name = m.Name,
                        Image = m.Image,
                        UserCode = m.UserCode,
                    }))
                    .GroupBy(u => u.UserCode)
                    .Select(g => g.First())
                    .ToList();

                var responseValid = await Task.FromResult(combinedValid);

                // Group messages
                var groupMess = await _context.MessageGroups
                    .Include(g => g.Student)
                        .ThenInclude(s => s.User)
                        .ThenInclude(u => u.Role)
                    .Where(g => g.StudentId == studentInfo.StudentId && g.JoinAt != null && g.OutAt == null && g.Status == "1")
                    .ToListAsync();

                if (!groupMess.Any())
                {
                    return responseValid;
                }

                // Handle remove duplicate group chat ids
                var groupChatIds = groupMess.Select(g => g.GroupChatId).Distinct().ToList();

                var groupQueryValid = _context.GroupChats
                    .Include(g => g.University).ThenInclude(u => u.Role)
                    .Include(g => g.Mentor).ThenInclude(m => m.User).ThenInclude(u => u.Role)
                    .Where(g => groupChatIds.Contains(g.GroupChatId))
                    .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    groupQueryValid = groupQueryValid.Where(g => g.GroupName.Contains(name));
                }

                var groupsValid = await groupQueryValid
                    .Select(g => new User
                    {
                        Name = g.GroupName
                    })
                    .ToListAsync();

                var combinedResultsValid = responseValid.Cast<dynamic>().Concat(groupsValid.Cast<dynamic>()).ToList();

                return combinedResultsValid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> SetAdminMemberInMessageGroupAsync(int? userId, int? memberId)
        {
            try
            {
                var userExists = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);

                if (userExists == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                // Member Uni side
                var memberUniSide = await _context.Users.FirstOrDefaultAsync(u => u.UserId == memberId);

                if (memberUniSide == null)
                {
                    throw new Exception("Not found member.");
                }

                // Member Company side
                var memberCompanySide = await _context.Companies
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .FirstOrDefaultAsync(c => c.UserId == memberId);

                if (memberCompanySide == null)
                {
                    throw new Exception("Not found member.");
                }

                // Member Student side
                var memberStudentSide = await _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == memberId);

                if (memberStudentSide == null)
                {
                    throw new Exception("Not found member.");
                }

                // Admin
                if (userExists?.Role?.Name == "Admin")
                {
                    bool isAdminMember = await _context.MessageGroups.AnyAsync(m => m.UniversityId == userId && m.IsAdmin == true);

                    if (!isAdminMember)
                    {
                        throw new KeyNotFoundException("Member is not an admin of message group.");
                    }

                    var member = await _context.MessageGroups
                        .Include(m => m.University)
                            .ThenInclude(m => m.Role)
                        .FirstOrDefaultAsync(m => m.UniversityId == memberId
                            && (m.University.Role.Name == "DOET" || m.University.Role.Name == "Dean" || m.University.Role.Name == "Lecturer"));

                    if (member == null)
                    {
                        throw new KeyNotFoundException("Cannot set admin permission for this member.");
                    }

                    // Set admin permission 
                    member.IsAdmin = true;
                    member.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Group chat
                    var groupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == member.GroupChatId);

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has assigned admin rights to {memberUniSide?.Name} in the group chat {groupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        UniversityId = member?.UniversityId,
                        GroupChatId = groupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                // DOET
                if (userExists?.Role?.Name == "DOET")
                {
                    bool isAdminMember = await _context.MessageGroups.AnyAsync(m => m.UniversityId == userId && m.IsAdmin == true);

                    if (!isAdminMember)
                    {
                        throw new KeyNotFoundException("Member is not an admin of message group.");
                    }

                    var member = await _context.MessageGroups
                        .Include(m => m.University)
                            .ThenInclude(m => m.Role)
                        .FirstOrDefaultAsync(m => m.UniversityId == memberId
                            && (m.University.Role.Name == "Dean" || m.University.Role.Name == "Lecturer"));

                    if (member == null)
                    {
                        throw new KeyNotFoundException("Cannot set admin permission for this member.");
                    }

                    // Set admin permission 
                    member.IsAdmin = true;
                    member.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Group chat
                    var groupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == member.GroupChatId);

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has assigned admin rights to {memberUniSide?.Name} in the group chat {groupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        UniversityId = member?.UniversityId,
                        GroupChatId = groupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                // Dean
                if (userExists?.Role?.Name == "Dean")
                {
                    bool isAdminMember = await _context.MessageGroups.AnyAsync(m => m.UniversityId == userId && m.IsAdmin == true);

                    if (!isAdminMember)
                    {
                        throw new KeyNotFoundException("Member is not an admin of message group.");
                    }

                    var member = await _context.MessageGroups
                        .Include(m => m.University)
                            .ThenInclude(m => m.Role)
                        .FirstOrDefaultAsync(m => m.UniversityId == memberId
                            && (m.University.Role.Name == "Lecturer"));

                    if (member == null)
                    {
                        throw new KeyNotFoundException("Cannot set admin permission for this member.");
                    }

                    // Set admin permission 
                    member.IsAdmin = true;
                    member.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Group chat
                    var groupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == member.GroupChatId);

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has assigned admin rights to {memberUniSide?.Name} in the group chat {groupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        UniversityId = member?.UniversityId,
                        GroupChatId = groupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                // Lecturer
                if (userExists?.Role?.Name == "Lecturer")
                {
                    bool isAdminMember = await _context.MessageGroups.AnyAsync(m => m.UniversityId == userId && m.IsAdmin == true);

                    if (!isAdminMember)
                    {
                        throw new KeyNotFoundException("Member is not an admin of message group.");
                    }

                    var student = await _context.Students.Include(s => s.User).ThenInclude(s => s.Role).FirstOrDefaultAsync(s => s.UserId == memberId);

                    var member = await _context.MessageGroups
                        .Include(m => m.Student)
                            .ThenInclude(m => m.User)
                            .ThenInclude(m => m.Role)
                        .FirstOrDefaultAsync(m => m.StudentId == student.StudentId
                            && m.Student.User.Role.Name == "Student");

                    if (member == null)
                    {
                        throw new KeyNotFoundException("Cannot set admin permission for this member.");
                    }

                    // Set admin permission 
                    member.IsAdmin = true;
                    member.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Group chat
                    var groupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == member.GroupChatId);

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has assigned admin rights to {memberStudentSide?.User?.Name} in the group chat {groupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        StudentId = member?.StudentId,
                        GroupChatId = groupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                // Mentor 
                if (userExists?.Role?.Name == "Mentor")
                {
                    var mentor = await _context.Companies.Include(c => c.User).ThenInclude(c => c.Role).FirstOrDefaultAsync(c => c.UserId == userId);

                    bool isAdminMember = await _context.MessageGroups.AnyAsync(m => m.MentorId == mentor.CompanyId && m.IsAdmin == true);

                    if (!isAdminMember)
                    {
                        throw new KeyNotFoundException("Member is not an admin of message group.");
                    }

                    var student = await _context.Students.Include(s => s.User).ThenInclude(s => s.Role).FirstOrDefaultAsync(s => s.UserId == memberId);

                    var member = await _context.MessageGroups
                        .Include(m => m.Student)
                            .ThenInclude(m => m.User)
                            .ThenInclude(m => m.Role)
                        .FirstOrDefaultAsync(m => m.StudentId == student.StudentId
                            && m.Student.User.Role.Name == "Student");

                    if (member == null)
                    {
                        throw new KeyNotFoundException("Cannot set admin permission for this member.");
                    }

                    // Set admin permission 
                    member.IsAdmin = true;
                    member.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Group chat
                    var groupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == member.GroupChatId);

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has assigned admin rights to {memberStudentSide?.User?.Name} in the group chat {groupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        StudentId = member?.StudentId,
                        GroupChatId = groupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Admin, DOET, Dean, Lecturer, Mentor, Student
        public async Task<bool> LeaveMessageGroupAsync(int? userId, int? groupChatId)
        {
            try
            {
                var userExists = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);

                if (userExists == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                // Admin
                if (userExists?.Role?.Name == "Admin")
                {
                    var goupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == groupChatId);

                    if (goupChat == null)
                    {
                        throw new KeyNotFoundException("Not found group chat.");
                    }

                    // Check if none other member is admin of group chat
                    var members = await _context.MessageGroups
                        .Where(m => m.GroupChatId == groupChatId)
                        .ToListAsync();

                    foreach (var member in members)
                    {
                        if (!members.Any(member => member.IsAdmin == true))
                        {
                            throw new KeyNotFoundException("No one member is admin of group chat.");
                        };
                    }

                    var admin = await _context.MessageGroups
                        .Where(m => m.UniversityId == userId && m.JoinAt != null)
                        .FirstOrDefaultAsync();

                    // Update leave message group 
                    admin.OutAt = DateTime.Now;
                    admin.Status = "0";
                    admin.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has left the group chat {goupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        UniversityId = admin?.UniversityId,
                        GroupChatId = goupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                // DOET
                if (userExists?.Role?.Name == "DOET")
                {
                    var goupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == groupChatId);

                    if (goupChat == null)
                    {
                        throw new KeyNotFoundException("Not found group chat.");
                    }

                    // Check if none other member is admin of group chat
                    var members = await _context.MessageGroups
                        .Where(m => m.GroupChatId == groupChatId)
                        .ToListAsync();

                    foreach (var member in members)
                    {
                        if (!members.Any(member => member.IsAdmin == true))
                        {
                            throw new KeyNotFoundException("No one member is admin of group chat.");
                        };
                    }

                    var doet = await _context.MessageGroups
                        .Where(m => m.UniversityId == userId && m.JoinAt != null)
                        .FirstOrDefaultAsync();

                    // Update leave message group 
                    doet.OutAt = DateTime.Now;
                    doet.Status = "0";
                    doet.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has left the group chat {goupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        UniversityId = doet?.UniversityId,
                        GroupChatId = goupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                // Dean
                if (userExists?.Role?.Name == "Dean")
                {
                    var goupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == groupChatId);

                    if (goupChat == null)
                    {
                        throw new KeyNotFoundException("Not found group chat.");
                    }

                    // Check if none other member is admin of group chat
                    var members = await _context.MessageGroups
                        .Where(m => m.GroupChatId == groupChatId)
                        .ToListAsync();

                    foreach (var member in members)
                    {
                        if (!members.Any(member => member.IsAdmin == true))
                        {
                            throw new KeyNotFoundException("No one member is admin of group chat.");
                        };
                    }

                    var dean = await _context.MessageGroups
                        .Where(m => m.UniversityId == userId && m.JoinAt != null)
                        .FirstOrDefaultAsync();

                    // Update leave message group 
                    dean.OutAt = DateTime.Now;
                    dean.Status = "0";
                    dean.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has left the group chat {goupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        UniversityId = dean?.UniversityId,
                        GroupChatId = goupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                // Lecturer
                if (userExists?.Role?.Name == "Lecturer")
                {
                    var goupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == groupChatId);

                    if (goupChat == null)
                    {
                        throw new KeyNotFoundException("Not found group chat.");
                    }

                    // Check if none other member is admin of group chat
                    var members = await _context.MessageGroups
                        .Where(m => m.GroupChatId == groupChatId)
                        .ToListAsync();

                    foreach (var member in members)
                    {
                        if (!members.Any(member => member.IsAdmin == true))
                        {
                            throw new KeyNotFoundException("No one member is admin of group chat.");
                        };
                    }

                    var lecturer = await _context.MessageGroups
                        .Where(m => m.UniversityId == userId && m.JoinAt != null)
                        .FirstOrDefaultAsync();

                    // Update leave message group 
                    lecturer.OutAt = DateTime.Now;
                    lecturer.Status = "0";
                    lecturer.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has left the group chat {goupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        UniversityId = lecturer?.UniversityId,
                        GroupChatId = goupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                // Mentor 
                if (userExists?.Role?.Name == "Mentor")
                {
                    var goupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == groupChatId);

                    if (goupChat == null)
                    {
                        throw new KeyNotFoundException("Not found group chat.");
                    }

                    var mentorInfo = await _context.Companies.FirstOrDefaultAsync(m => m.UserId == userId);

                    // Check if none other member is admin of group chat
                    var members = await _context.MessageGroups
                        .Where(m => m.GroupChatId == groupChatId)
                        .ToListAsync();

                    foreach (var member in members)
                    {
                        if (!members.Any(member => member.IsAdmin == true))
                        {
                            throw new KeyNotFoundException("No one member is admin of group chat.");
                        };
                    }

                    var mentor = await _context.MessageGroups
                        .Where(m => m.MentorId == mentorInfo.CompanyId && m.JoinAt != null)
                        .FirstOrDefaultAsync();

                    // Update leave message group 
                    mentor.OutAt = DateTime.Now;
                    mentor.Status = "0";
                    mentor.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification 
                    var notificationContent = $"{userExists?.Name} has left the group chat {goupChat?.GroupName}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        CompanyId = mentor?.MentorId,
                        GroupChatId = goupChat?.GroupChatId,
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                // Student
                var goupChatValid = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == groupChatId);

                if (goupChatValid == null)
                {
                    throw new KeyNotFoundException("Not found group chat.");
                }

                var studentInfo = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

                var student = await _context.MessageGroups
                    .Where(m => m.StudentId == studentInfo.StudentId && m.JoinAt != null)
                    .FirstOrDefaultAsync();

                // Update leave message group 
                student.OutAt = DateTime.Now;
                student.Status = "0";
                student.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                // Notification 
                var notificationContentValid = $"{userExists?.Name} has left the group chat {goupChatValid?.GroupName}.";
                var notiInfoValid = new Notification
                {
                    NotificationContent = notificationContentValid,
                    Image = userExists?.Image,
                    StudentId = student?.StudentId,
                    GroupChatId = goupChatValid?.GroupChatId,
                };

                await _notificationRepository.CreateNotificationAsync(notiInfoValid);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<MessageGroup>> GetAllMessagesInGroupChatAsync(int? groupChatId)
        {
            try
            {
                bool groupChatExists = await _context.GroupChats.AnyAsync(g => g.GroupChatId == groupChatId);
                if (!groupChatExists)
                {
                    throw new KeyNotFoundException("Not found group chat.");
                }

                var messages = await _context.MessageGroups
                    .Include(m => m.Student).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                    .Include(m => m.University).ThenInclude(m => m.Role)
                    .Include(m => m.Mentor).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                    .Where(m => m.GroupChatId == groupChatId)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();

                return messages;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MessageGroup> CreateMessageInMessageGroupAsync(int? userId, string? messageFileName, string? imageFileName, MessageGroup? messageGroupInfo)
        {
            try
            {
                var userExists = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (userExists == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var groupChat = await _context.GroupChats.FirstOrDefaultAsync(g => g.GroupChatId == messageGroupInfo.GroupChatId);
                if (groupChat == null)
                {
                    throw new KeyNotFoundException("Not found group chat.");
                }

                // Uni
                if (userExists?.Role?.Name == "Admin" || userExists?.Role?.Name == "DOET" || userExists?.Role?.Name == "Dean" || userExists?.Role?.Name == "Lecturer")
                {
                    //// Create file name format userId_timestamp_filename
                    //var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    //var newMessageFileName = messageFileName != null ? $"{userId}_{timestamp}_{messageFileName}" : null;
                    //var newImageFileName = imageFileName != null ? $"{userId}_{timestamp}_{imageFileName}" : null;

                    //var messageFilePath = newMessageFileName != null ? Path.Combine(_messageFileDirectory, newMessageFileName) : null;
                    //var imageFilePath = newImageFileName != null ? Path.Combine(_imageFileDirectory, newImageFileName) : null;

                    //// Save files to folders
                    //if (messageFileData != null && messageFilePath != null)
                    //{
                    //    await File.WriteAllBytesAsync(messageFilePath, messageFileData);
                    //}

                    //if (imageFileData != null && imageFilePath != null)
                    //{
                    //    await File.WriteAllBytesAsync(imageFilePath, imageFileData);
                    //}

                    //// If null 
                    //if (messageFileName == null || messageFileData == null)
                    //{
                    //    messageFilePath = null;
                    //}

                    //if (imageFileName == null || imageFileData == null)
                    //{
                    //    imageFilePath = null;
                    //}

                    // Create new message infomation
                    var uniMemberInfo = new MessageGroup
                    {
                        GroupChatId = messageGroupInfo?.GroupChatId,
                        MessageContent = messageGroupInfo?.MessageContent,
                        MessageFile = messageFileName,
                        Image = imageFileName,
                        UniversityId = userId,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    _context.MessageGroups.Add(uniMemberInfo);
                    await _context.SaveChangesAsync();

                    return uniMemberInfo;
                }

                // Mentor
                if (userExists?.Role?.Name == "Mentor")
                {
                    var mentor = await _context.Companies.Include(c => c.User).ThenInclude(c => c.Role).FirstOrDefaultAsync(c => c.UserId == userId);

                    //// Create file name format companyId_timestamp_filename
                    //var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    //var newMessageFileName = messageFileName != null ? $"{mentor.CompanyId}_{timestamp}_{messageFileName}" : null;
                    //var newImageFileName = imageFileName != null ? $"{mentor.CompanyId}_{timestamp}_{imageFileName}" : null;

                    //var messageFilePath = newMessageFileName != null ? Path.Combine(_messageFileDirectory, newMessageFileName) : null;
                    //var imageFilePath = newImageFileName != null ? Path.Combine(_imageFileDirectory, newImageFileName) : null;

                    //// Save files to folders
                    //if (messageFileData != null && messageFilePath != null)
                    //{
                    //    await File.WriteAllBytesAsync(messageFilePath, messageFileData);
                    //}

                    //if (imageFileData != null && imageFilePath != null)
                    //{
                    //    await File.WriteAllBytesAsync(imageFilePath, imageFileData);
                    //}

                    //// If null 
                    //if (messageFileName == null || messageFileData == null)
                    //{
                    //    messageFilePath = null;
                    //}

                    //if (imageFileName == null || imageFileData == null)
                    //{
                    //    imageFilePath = null;
                    //}

                    // Create new message infomation
                    var mentorMemberInfo = new MessageGroup
                    {
                        GroupChatId = messageGroupInfo?.GroupChatId,
                        MessageContent = messageGroupInfo?.MessageContent,
                        MessageFile = messageFileName,
                        Image = imageFileName,
                        MentorId = mentor.CompanyId,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    _context.MessageGroups.Add(mentorMemberInfo);
                    await _context.SaveChangesAsync();

                    return mentorMemberInfo;
                }

                // Student 
                var student = await _context.Students.Include(s => s.User).ThenInclude(s => s.Role).FirstOrDefaultAsync(s => s.UserId == userId);

                //// Create file name format studentId_timestamp_filename
                //var timestampStu = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //var newMessageFileNameStu = messageFileName != null ? $"{student.StudentId}_{timestampStu}_{messageFileName}" : null;
                //var newImageFileNameStu = imageFileName != null ? $"{student.StudentId}_{timestampStu}_{imageFileName}" : null;

                //var messageFilePathStu = newMessageFileNameStu != null ? Path.Combine(_messageFileDirectory, newMessageFileNameStu) : null;
                //var imageFilePathStu = newImageFileNameStu != null ? Path.Combine(_imageFileDirectory, newImageFileNameStu) : null;

                //// Save files to folders
                //if (messageFileData != null && messageFilePathStu != null)
                //{
                //    await File.WriteAllBytesAsync(messageFilePathStu, messageFileData);
                //}

                //if (imageFileData != null && imageFilePathStu != null)
                //{
                //    await File.WriteAllBytesAsync(imageFilePathStu, imageFileData);
                //}

                //// If null 
                //if (messageFileName == null || messageFileData == null)
                //{
                //    messageFilePathStu = null;
                //}

                //if (imageFileName == null || imageFileData == null)
                //{
                //    imageFilePathStu = null;
                //}

                // Create new message infomation
                var studentMemberInfo = new MessageGroup
                {
                    GroupChatId = messageGroupInfo?.GroupChatId,
                    MessageContent = messageGroupInfo?.MessageContent,
                    MessageFile = messageFileName,
                    Image = imageFileName,
                    StudentId = student.StudentId,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _context.MessageGroups.Add(studentMemberInfo);
                await _context.SaveChangesAsync();

                return studentMemberInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<MessageGroup>> GetAllGroupChatAsync(int? userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(g => g.UserId == userId);
                if (user == null)
                {
                    throw new KeyNotFoundException("Not found group chat.");
                }

                if (user.Role.Name == "Admin" || user.Role.Name == "DOET" || user.Role.Name == "Dean" || user.Role.Name == "Lecturer")
                {
                    var group = await _context.MessageGroups
                        .Include(m => m.Student).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                        .Include(m => m.Mentor).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                        .Include(m => m.University).ThenInclude(m => m.Role)
                        .Where(m => m.UniversityId == userId && m.Status != "0" && m.Status != null)
                        .ToListAsync();

                    if (group == null)
                    {
                        throw new KeyNotFoundException("Not found group chat.");
                    }

                    return group;
                }

                if (user.Role.Name == "Company" || user.Role.Name == "Mentor")
                {
                    var company = await _context.Companies
                        .Include(c => c.User).ThenInclude(c => c.Role)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    var group = await _context.MessageGroups
                        .Include(m => m.Student).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                        .Include(m => m.Mentor).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                        .Include(m => m.University).ThenInclude(m => m.Role)
                        .Where(m => m.MentorId == company.CompanyId && m.Status != "0" && m.Status != null)
                        .ToListAsync();

                    if (group == null)
                    {
                        throw new KeyNotFoundException("Not found group chat.");
                    }

                    return group;
                }


                if (user.Role.Name == "Student")
                {
                    var student = await _context.Students
                        .Include(c => c.User).ThenInclude(c => c.Role)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    var group = await _context.MessageGroups
                        .Include(m => m.Student).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                        .Include(m => m.Mentor).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                        .Include(m => m.University).ThenInclude(m => m.Role)
                        .Where(m => m.StudentId == student.StudentId && m.Status != "0" && m.Status != null)
                        .ToListAsync();

                    if (group == null)
                    {
                        throw new KeyNotFoundException("Not found group chat.");
                    }

                    return group;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
