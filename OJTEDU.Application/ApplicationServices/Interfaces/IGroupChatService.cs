using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.GroupChatDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IGroupChatService
    {
        // Admin, DOET, Dean, Lecturer, Mentor
        Task<DataResponse<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>> CreateGroupChatAsync(int? userId,
            CreateGroupChatForAdminDOETDeanLecturerMentorDTO? groupChatInfo);
        Task<DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>> SearchGroupChatByNameAsync(int? userId, string? groupName);
        Task<DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>> GetAllGroupChatByUserIdAsync(int? userId);
        Task<DataResponse<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>> UpdateGroupChatAsync(int? groupChatId, UpdateGroupChatForAdminDOETDeanLecturerMentorDTO? groupChatInfo);
    }
}
