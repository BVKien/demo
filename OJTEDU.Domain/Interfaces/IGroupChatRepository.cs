using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IGroupChatRepository
    {
        /*
         + Groupchat status: 
        0: Deleted
        1: Active
         */

        // Admin, DOET, Dean, Lecturer, Mentor
        Task<IEnumerable<GroupChat>> GetAllGroupChatByUserIdAsync(int? userId);
        Task<GroupChat> CreateGroupChatAsync(int? userId, GroupChat? groupChatInfo);
        Task<IEnumerable<GroupChat>> SearchGroupChatByNameAsync(int? userId, string? groupName);
        Task<GroupChat> ClearMessageHistoryGroupChatAsync(int? groupChatId);
        Task<GroupChat> UpdateGroupChatAsync(int? groupChatId, GroupChat? groupChatInfo);
    }
}
