using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IMessageGroupRepository
    {
        /*
         + Message group status: Đây là status của các người dùng trong group nhắn tin, không phải status của 1 tin nhắn
        0: Deleted 
        1: Active
         */

        // Company check?

        // Admin, DOET, Dean, Lecturer, Mentor
        Task<MessageGroup> CreateMemberInMessageGroupAsync(int? userId, int? memberId, MessageGroup? messageGroupInfo);
        Task<IEnumerable<dynamic>> SearchAllMemberAndGroupForUserAsync(int? userId, string? name); // + Student
        Task<MessageGroup> DeleteMemberInMessageGroupAsync(int? userId, int? groupChatId, int? memberId);
        Task<bool> SetAdminMemberInMessageGroupAsync(int? userId, int? memberId);

        // Admin, DOET, Dean, Lecturer, Mentor, Student
        Task<bool> LeaveMessageGroupAsync(int? userId, int? groupChatId);
        Task<List<MessageGroup>> GetAllMessagesInGroupChatAsync(int? groupChatId);
        Task<MessageGroup> CreateMessageInMessageGroupAsync(int? userId, string? messageFileName, byte[]? messageFileData, string? imageFileName, byte[]? imageFileData, MessageGroup? messageGroupInfo);

        // off noti 
        // is read -> is seen 
    }
}
