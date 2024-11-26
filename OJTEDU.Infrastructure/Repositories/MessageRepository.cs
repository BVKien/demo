using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OJTEDU.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _messageFileDirectory = "wwwroot/uploads/messages/messagefiles/";
        private readonly string _imageFileDirectory = "wwwroot/uploads/messages/images/";
        private readonly INotificationRepository _notificationRepository;
        public MessageRepository(OJTEDU_DB_V1Context context, INotificationRepository notificationRepository)
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

        // First message from sender -> create conversation
        // Admin, DOET, Dean, Lecturer, Mentor, Company, Student
        public async Task<Message> CreateFirstMessageConversationAsync(int? userId, int? receiverId, string? messageFileName, byte[]? messageFileData, string? imageFileName, byte[]? imageFileData, Message? messageInfo)
        {
            try
            {
                var userExists = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);

                if (userExists == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var receiverExists = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == receiverId);

                if (receiverExists == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                // get all messages
                var messages = await _context.Messages
                    .Include(m => m.Universiry).ThenInclude(m => m.Role)
                    .Include(m => m.Company).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                    .Include(m => m.Student).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                    .ToListAsync();

                // Uni side
                if (userExists?.Role.Name == "Admin" || userExists?.Role.Name == "DOET" || userExists?.Role.Name == "Dean" || userExists?.Role.Name == "Lecturer")
                {
                    // Create file name format userId_timestamp_filename
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    var newMessageFileName = messageFileName != null ? $"{userId}_{timestamp}_{messageFileName}" : null;
                    var newImageFileName = imageFileName != null ? $"{userId}_{timestamp}_{imageFileName}" : null;

                    var messageFilePath = newMessageFileName != null ? Path.Combine(_messageFileDirectory, newMessageFileName) : null;
                    var imageFilePath = newImageFileName != null ? Path.Combine(_imageFileDirectory, newImageFileName) : null;

                    // Save files to folders
                    if (messageFileData != null && messageFilePath != null)
                    {
                        await File.WriteAllBytesAsync(messageFilePath, messageFileData);
                    }

                    if (imageFileData != null && imageFilePath != null)
                    {
                        await File.WriteAllBytesAsync(imageFilePath, imageFileData);
                    }

                    // If null 
                    if (messageFileName == null || messageFileData == null)
                    {
                        messageFilePath = null;
                    }

                    if (imageFileName == null || imageFileData == null)
                    {
                        imageFilePath = null;
                    }

                    int conversationId;

                    // check all conversation id in messages list
                    if (!messages.Any(m => m.ConversationId != null))
                    {
                        conversationId = 1;
                    }
                    else
                    {
                        conversationId = messages.Where(m => m.ConversationId != null).Max(m => m.ConversationId.Value) + 1;
                    }

                    var message = new Message
                    {
                        MessageContent = messageInfo?.MessageContent,
                        ConversationId = conversationId,
                        MessageFile = messageFilePath,
                        Image = imageFilePath,
                        UniversiryId = userId,
                        IsRead = false,
                        Status = "1",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Messages.AddAsync(message);
                    await _context.SaveChangesAsync();

                    // Receiver Uni side
                    if (receiverExists?.Role.Name == "Admin" || receiverExists?.Role.Name == "DOET" || receiverExists?.Role.Name == "Dean" || receiverExists?.Role.Name == "Lecturer")
                    {
                        // check reciver exists 
                        var receiver = await _context.Messages.Include(m => m.Universiry).ThenInclude(m => m.Role).Where(m => m.UniversiryId == receiverId).FirstOrDefaultAsync();

                        var messageUniReceiver = new Message
                        {
                            ConversationId = conversationId,
                            UniversiryId = receiverId,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        await _context.Messages.AddAsync(messageUniReceiver);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has sent a message to {receiverExists?.Name}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            UniversityId = receiverExists?.UserId,
                            MessageId = message?.MessageId
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);
                    }

                    // Receiver Company side
                    if (receiverExists?.Role.Name == "Company" || receiverExists?.Role.Name == "Mentor")
                    {
                        var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == receiverExists.UserId);

                        // check reciver exists 
                        var receiver = await _context.Messages
                            .Where(m => m.CompanyId == company.CompanyId)
                            .FirstOrDefaultAsync();

                        var messageUniReceiver = new Message
                        {
                            ConversationId = conversationId,
                            CompanyId = company.CompanyId,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        await _context.Messages.AddAsync(messageUniReceiver);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has sent a message to {company?.User?.Name}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            CompanyId = company?.CompanyId,
                            MessageId = message?.MessageId
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);
                    }

                    // Receiver Student side
                    if (receiverExists?.Role.Name == "Student")
                    {
                        var studentInfo = await _context.Students.FirstOrDefaultAsync(c => c.UserId == receiverExists.UserId);

                        // check reciver exists 
                        var receiver = await _context.Messages
                            .Where(m => m.StudentId == studentInfo.StudentId)
                            .FirstOrDefaultAsync();

                        var messageUniReceiver = new Message
                        {
                            ConversationId = conversationId,
                            StudentId = studentInfo.StudentId,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        await _context.Messages.AddAsync(messageUniReceiver);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has sent a message to {studentInfo?.User?.Name}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            StudentId = studentInfo?.StudentId,
                            MessageId = message?.MessageId
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);
                    }

                    return message;
                }

                // Company side
                if (userExists?.Role?.Name == "Company" || userExists?.Role?.Name == "Mentor")
                {
                    var company = await _context.Companies
                        .Include(c => c.User).ThenInclude(c => c.Role)
                        .FirstOrDefaultAsync();

                    // Create file name format companyId_timestamp_filename
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    var newMessageFileName = messageFileName != null ? $"{company.CompanyId}_{timestamp}_{messageFileName}" : null;
                    var newImageFileName = imageFileName != null ? $"{company.CompanyId}_{timestamp}_{imageFileName}" : null;

                    var messageFilePath = newMessageFileName != null ? Path.Combine(_messageFileDirectory, newMessageFileName) : null;
                    var imageFilePath = newImageFileName != null ? Path.Combine(_imageFileDirectory, newImageFileName) : null;

                    // Save files to folders
                    if (messageFileData != null && messageFilePath != null)
                    {
                        await File.WriteAllBytesAsync(messageFilePath, messageFileData);
                    }

                    if (imageFileData != null && imageFilePath != null)
                    {
                        await File.WriteAllBytesAsync(imageFilePath, imageFileData);
                    }

                    // If null 
                    if (messageFileName == null || messageFileData == null)
                    {
                        messageFilePath = null;
                    }

                    if (imageFileName == null || imageFileData == null)
                    {
                        imageFilePath = null;
                    }

                    int conversationId;

                    // check all conversation id in messages list
                    if (!messages.Any(m => m.ConversationId != null))
                    {
                        conversationId = 1;
                    }
                    else
                    {
                        conversationId = messages.Where(m => m.ConversationId != null).Max(m => m.ConversationId.Value) + 1;
                    }

                    var message = new Message
                    {
                        MessageContent = messageInfo?.MessageContent,
                        ConversationId = conversationId,
                        MessageFile = messageFilePath,
                        Image = imageFilePath,
                        CompanyId = company.CompanyId,
                        IsRead = false,
                        Status = "1",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Messages.AddAsync(message);
                    await _context.SaveChangesAsync();

                    // Receiver Uni side
                    if (receiverExists?.Role.Name == "Admin" || receiverExists?.Role.Name == "DOET" || receiverExists?.Role.Name == "Dean" || receiverExists?.Role.Name == "Lecturer")
                    {
                        // check reciver exists 
                        var receiver = await _context.Messages.Include(m => m.Universiry).ThenInclude(m => m.Role).Where(m => m.UniversiryId == receiverId).FirstOrDefaultAsync();

                        var messageCompanyReceiver = new Message
                        {
                            ConversationId = conversationId,
                            UniversiryId = receiverId,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        await _context.Messages.AddAsync(messageCompanyReceiver);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has sent a message to {receiverExists?.Name}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            UniversityId = receiverExists?.UserId,
                            MessageId = message?.MessageId
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);
                    }

                    // Receiver Company side
                    if (receiverExists?.Role.Name == "Company" || receiverExists?.Role.Name == "Mentor")
                    {
                        var companyReceiver = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == receiverExists.UserId);

                        // check reciver exists 
                        var receiver = await _context.Messages
                            .Where(m => m.CompanyId == companyReceiver.CompanyId)
                            .FirstOrDefaultAsync();

                        var messageCompanyReceiver = new Message
                        {
                            ConversationId = conversationId,
                            CompanyId = companyReceiver.CompanyId,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        await _context.Messages.AddAsync(messageCompanyReceiver);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has sent a message to {companyReceiver?.User?.Name}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            CompanyId = companyReceiver?.CompanyId,
                            MessageId = message?.MessageId
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);
                    }

                    // Receiver Student side
                    if (receiverExists?.Role.Name == "Student")
                    {
                        var studentInfo = await _context.Students.FirstOrDefaultAsync(c => c.UserId == receiverExists.UserId);

                        // check reciver exists 
                        var receiver = await _context.Messages
                            .Where(m => m.StudentId == studentInfo.StudentId)
                            .FirstOrDefaultAsync();

                        var messageCompanyReceiver = new Message
                        {
                            ConversationId = conversationId,
                            StudentId = studentInfo.StudentId,
                            IsRead = false,
                            Status = "1",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };

                        await _context.Messages.AddAsync(messageCompanyReceiver);
                        await _context.SaveChangesAsync();

                        // Notification 
                        var notificationContent = $"{userExists?.Name} has sent a message to {studentInfo?.User?.Name}.";
                        var notiInfo = new Notification
                        {
                            NotificationContent = notificationContent,
                            Image = userExists?.Image,
                            StudentId = studentInfo?.StudentId,
                            MessageId = message?.MessageId
                        };

                        await _notificationRepository.CreateNotificationAsync(notiInfo);
                    }

                    return message;
                }

                // Student 
                var student = await _context.Students.Include(c => c.User)
                    .ThenInclude(c => c.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                // Create file name format studentId_timestamp_filename
                var timestampValid = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var newMessageFileNameValid = messageFileName != null ? $"{student.StudentId}_{timestampValid}_{messageFileName}" : null;
                var newImageFileNameValid = imageFileName != null ? $"{student.StudentId}_{timestampValid}_{imageFileName}" : null;

                var messageFilePathValid = newMessageFileNameValid != null ? Path.Combine(_messageFileDirectory, newMessageFileNameValid) : null;
                var imageFilePathValid = newImageFileNameValid != null ? Path.Combine(_imageFileDirectory, newImageFileNameValid) : null;

                // Save files to folders
                if (messageFileData != null && messageFilePathValid != null)
                {
                    await File.WriteAllBytesAsync(messageFilePathValid, messageFileData);
                }

                if (imageFileData != null && imageFilePathValid != null)
                {
                    await File.WriteAllBytesAsync(imageFilePathValid, imageFileData);
                }

                // If null 
                if (messageFileName == null || messageFileData == null)
                {
                    messageFilePathValid = null;
                }

                if (imageFileName == null || imageFileData == null)
                {
                    imageFilePathValid = null;
                }

                int conversationIdValid;

                // check all conversation id in messages list
                if (!messages.Any(m => m.ConversationId != null))
                {
                    conversationIdValid = 1;
                }
                else
                {
                    conversationIdValid = messages.Where(m => m.ConversationId != null).Max(m => m.ConversationId.Value) + 1;
                }

                var messageValid = new Message
                {
                    MessageContent = messageInfo?.MessageContent,
                    ConversationId = conversationIdValid,
                    MessageFile = messageFilePathValid?.Replace("wwwroot", ""),
                    Image = imageFilePathValid?.Replace("wwwroot", ""),
                    StudentId = student.StudentId,
                    IsRead = false,
                    Status = "1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                await _context.Messages.AddAsync(messageValid);
                await _context.SaveChangesAsync();

                // Receiver Uni side
                if (receiverExists?.Role.Name == "Admin" || receiverExists?.Role.Name == "DOET" || receiverExists?.Role.Name == "Dean" || receiverExists?.Role.Name == "Lecturer")
                {
                    // check reciver exists 
                    var receiver = await _context.Messages.Include(m => m.Universiry).ThenInclude(m => m.Role).Where(m => m.UniversiryId == receiverId).FirstOrDefaultAsync();

                    var messageStudentReceiver = new Message
                    {
                        ConversationId = conversationIdValid,
                        UniversiryId = receiverId,
                        IsRead = false,
                        Status = "1",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Messages.AddAsync(messageStudentReceiver);
                    await _context.SaveChangesAsync();

                    // Notification 
                    var notificationContent = $"{student?.User?.Name} has sent a message to {receiverExists?.Name}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        UniversityId = receiverExists?.UserId,
                        MessageId = messageValid?.MessageId
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);
                }

                // Receiver Company side
                if (receiverExists?.Role.Name == "Company" || receiverExists?.Role.Name == "Mentor")
                {
                    var companyReceiver = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == receiverExists.UserId);

                    // check reciver exists 
                    var receiver = await _context.Messages
                        .Where(m => m.CompanyId == companyReceiver.CompanyId)
                        .FirstOrDefaultAsync();

                    var messageStudentReceiver = new Message
                    {
                        ConversationId = conversationIdValid,
                        CompanyId = companyReceiver.CompanyId,
                        IsRead = false,
                        Status = "1",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Messages.AddAsync(messageStudentReceiver);
                    await _context.SaveChangesAsync();

                    // Notification 
                    var notificationContent = $"{student?.User?.Name} has sent a message to {companyReceiver?.User?.Name}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        CompanyId = companyReceiver?.CompanyId,
                        MessageId = messageValid?.MessageId
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);
                }

                // Receiver Student side
                if (receiverExists?.Role.Name == "Student")
                {
                    var studentInfo = await _context.Students.FirstOrDefaultAsync(c => c.UserId == receiverExists.UserId);

                    // check reciver exists 
                    var receiver = await _context.Messages
                        .Where(m => m.StudentId == studentInfo.StudentId)
                        .FirstOrDefaultAsync();

                    var messageStudentReceiver = new Message
                    {
                        ConversationId = conversationIdValid,
                        StudentId = studentInfo.StudentId,
                        IsRead = false,
                        Status = "1",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Messages.AddAsync(messageStudentReceiver);
                    await _context.SaveChangesAsync();

                    // Notification 
                    var notificationContent = $"{student?.User?.Name} has sent a message to {studentInfo?.User?.Name}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = userExists?.Image,
                        StudentId = studentInfo?.StudentId,
                        MessageId = messageValid?.MessageId
                    };

                    await _notificationRepository.CreateNotificationAsync(notiInfo);
                }

                return messageValid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Message> CreateMessageAsync(int? userId, string? messageFileName, byte[]? messageFileData, string? imageFileName, byte[]? imageFileData, Message? messageInfo)
        {
            try
            {
                var userExists = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);

                if (userExists == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var converation = await _context.Messages.FirstOrDefaultAsync(m => m.ConversationId == messageInfo.ConversationId);

                if (converation == null)
                {
                    throw new KeyNotFoundException("Not found conversation.");
                }

                // Uni side
                if (userExists?.Role.Name == "Admin" || userExists?.Role.Name == "DOET" || userExists?.Role.Name == "Dean" || userExists?.Role.Name == "Lecturer")
                {
                    // Create file name format userId_timestamp_filename
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    var newMessageFileName = messageFileName != null ? $"{userId}_{timestamp}_{messageFileName}" : null;
                    var newImageFileName = imageFileName != null ? $"{userId}_{timestamp}_{imageFileName}" : null;

                    var messageFilePath = newMessageFileName != null ? Path.Combine(_messageFileDirectory, newMessageFileName) : null;
                    var imageFilePath = newImageFileName != null ? Path.Combine(_imageFileDirectory, newImageFileName) : null;

                    // Save files to folders
                    if (messageFileData != null && messageFilePath != null)
                    {
                        await File.WriteAllBytesAsync(messageFilePath, messageFileData);
                    }

                    if (imageFileData != null && imageFilePath != null)
                    {
                        await File.WriteAllBytesAsync(imageFilePath, imageFileData);
                    }

                    // If null 
                    if (messageFileName == null || messageFileData == null)
                    {
                        messageFilePath = null;
                    }

                    if (imageFileName == null || imageFileData == null)
                    {
                        imageFilePath = null;
                    }

                    var message = new Message
                    {
                        MessageContent = messageInfo?.MessageContent,
                        ConversationId = messageInfo?.ConversationId,
                        MessageFile = messageFilePath?.Replace("wwwroot", ""),
                        Image = imageFilePath?.Replace("wwwroot", ""),
                        UniversiryId = userId,
                        IsRead = false,
                        Status = "1",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Messages.AddAsync(message);
                    await _context.SaveChangesAsync();

                    return message;
                }

                // Company side
                if (userExists?.Role.Name == "Company" || userExists?.Role.Name == "Mentor")
                {
                    var company = await _context.Companies
                        .Include(c => c.User).ThenInclude(c => c.Role)
                        .FirstOrDefaultAsync();

                    // Create file name format companyId_timestamp_filename
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    var newMessageFileName = messageFileName != null ? $"{company.CompanyId}_{timestamp}_{messageFileName}" : null;
                    var newImageFileName = imageFileName != null ? $"{company.CompanyId}_{timestamp}_{imageFileName}" : null;

                    var messageFilePath = newMessageFileName != null ? Path.Combine(_messageFileDirectory, newMessageFileName) : null;
                    var imageFilePath = newImageFileName != null ? Path.Combine(_imageFileDirectory, newImageFileName) : null;

                    // Save files to folders
                    if (messageFileData != null && messageFilePath != null)
                    {
                        await File.WriteAllBytesAsync(messageFilePath, messageFileData);
                    }

                    if (imageFileData != null && imageFilePath != null)
                    {
                        await File.WriteAllBytesAsync(imageFilePath, imageFileData);
                    }

                    // If null 
                    if (messageFileName == null || messageFileData == null)
                    {
                        messageFilePath = null;
                    }

                    if (imageFileName == null || imageFileData == null)
                    {
                        imageFilePath = null;
                    }

                    var message = new Message
                    {
                        MessageContent = messageInfo?.MessageContent,
                        ConversationId = messageInfo?.ConversationId,
                        MessageFile = messageFilePath?.Replace("wwwroot", ""),
                        Image = imageFilePath?.Replace("wwwroot", ""),
                        CompanyId = company.CompanyId,
                        IsRead = false,
                        Status = "1",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Messages.AddAsync(message);
                    await _context.SaveChangesAsync();

                    return message;
                }

                // Student 
                var student = await _context.Students.Include(c => c.User)
                    .ThenInclude(c => c.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                // Create file name format studentId_timestamp_filename
                var timestampValid = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var newMessageFileNameValid = messageFileName != null ? $"{student.StudentId}_{timestampValid}_{messageFileName}" : null;
                var newImageFileNameValid = imageFileName != null ? $"{student.StudentId}_{timestampValid}_{imageFileName}" : null;

                var messageFilePathValid = newMessageFileNameValid != null ? Path.Combine(_messageFileDirectory, newMessageFileNameValid) : null;
                var imageFilePathValid = newImageFileNameValid != null ? Path.Combine(_imageFileDirectory, newImageFileNameValid) : null;

                // Save files to folders
                if (messageFileData != null && messageFilePathValid != null)
                {
                    await File.WriteAllBytesAsync(messageFilePathValid, messageFileData);
                }

                if (imageFileData != null && imageFilePathValid != null)
                {
                    await File.WriteAllBytesAsync(imageFilePathValid, imageFileData);
                }

                // If null 
                if (messageFileName == null || messageFileData == null)
                {
                    messageFilePathValid = null;
                }

                if (imageFileName == null || imageFileData == null)
                {
                    imageFilePathValid = null;
                }

                var messageValid = new Message
                {
                    MessageContent = messageInfo?.MessageContent,
                    ConversationId = messageInfo?.ConversationId,
                    MessageFile = messageFilePathValid?.Replace("wwwroot", ""),
                    Image = imageFilePathValid?.Replace("wwwroot", ""),
                    StudentId = student.StudentId,
                    IsRead = false,
                    Status = "1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                await _context.Messages.AddAsync(messageValid);
                await _context.SaveChangesAsync();

                return messageValid;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Message>> GetAllMessageInConversationAsync(int? conversationId)
        {
            try
            {
                bool conversationExists = await _context.Messages.AnyAsync(m => m.ConversationId == conversationId);

                if (!conversationExists)
                {
                    throw new KeyNotFoundException("Not found conversation.");
                }

                var conversation = await _context.Messages
                    .Include(m => m.Universiry).ThenInclude(m => m.Role)
                    .Include(m => m.Company).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                    .Include(m => m.Student).ThenInclude(m => m.User).ThenInclude(m => m.Role)
                    .Where(m => m.ConversationId == conversationId)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();

                if (conversation == null)
                {
                    throw new KeyNotFoundException("Not found conversation.");
                }

                return conversation;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
