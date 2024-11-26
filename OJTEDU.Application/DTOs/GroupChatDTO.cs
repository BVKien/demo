using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class GroupChatDTO
    {
        public class CreateGroupChatForAdminDOETDeanLecturerMentorDTO
        {
            public string? GroupName { get; set; }
        }

        public class GroupChatListForAdminDOETDeanLecturerMentorDTO
        {
            public string? GroupName { get; set; }
        }

        public class UpdateGroupChatForAdminDOETDeanLecturerMentorDTO
        {
            public string? GroupName { get; set; }
        }
    }
}
