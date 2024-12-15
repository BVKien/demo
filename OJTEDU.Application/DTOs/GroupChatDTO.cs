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
            public int GroupChatId { get; set; }
            public string? GroupName { get; set; }
            public int? UniversityId { get; set; }
            public int? MentorId { get; set; }
            public bool? IsAdmin { get; set; }
            public string? Status { get; set; }
        }

        public class UpdateGroupChatForAdminDOETDeanLecturerMentorDTO
        {
            public string? GroupName { get; set; }
        }
    }
}
