using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.MajorDTO;
using static OJTEDU.Application.DTOs.MessageGroupDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IMessageGroupService
    {
        // Admin, DOET, Dean, Lecturer, Mentor
        Task<DataResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>> CreateMemberInMessageGroupAsync(int? userId, int? memberId,
            CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO? messageGroupInfo);
        Task<DataResponse<string>> DeleteMemberInMessageGroupAsync(int? userId, int? groupChatId, int? memberId);
        Task<DataResponse<List<GetAllMemberForUserAsyncForAdminDOETDeanLecturerMentorDTO>>> SearchAllMemberAndGroupForUserAsync(int? userId, string? name);
        Task<DataResponse<bool>> SetAdminMemberInMessageGroupAsync(int? userId, int? memberId);

        // Admin, DOET, Dean, Lecturer, Mentor, Student
        Task<DataResponse<bool>> LeaveMessageGroupForUserAsync(int? userId, int? groupChatId);
        Task<DataResponse<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>> GetAllMessagesInGroupChatAsync(int? groupChatId);
        Task<DataResponse<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>> CreateMessageInMessageGroupAsync(int? userId, string? messageFileName, string? imageFileName,
            CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO? messageGroupInfo);
        Task<DataResponse<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>> GetAllGroupChatAsync(int? userId);
    }
}
