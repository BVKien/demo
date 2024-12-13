using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.MessageDTO;
using static OJTEDU.Application.DTOs.MessageGroupDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IMessageService
    {
        // Admin, DOET, Dean, Lecturer, Mentor, Company, Student
        Task<DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>> CreateFirstMessageConversationAsync(int? userId, int? receiverId,
            string? messageFileName, string? imageFileName,
            CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO? messageGroupInfo);

        Task<DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>> CreateMessageAsync(int? userId,
            string? messageFileName, string? imageFileName,
            CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO? messageGroupInfo);
        Task<DataResponse<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>> GetAllMessageInConversationAsync(int? conversationId);
    }
}
