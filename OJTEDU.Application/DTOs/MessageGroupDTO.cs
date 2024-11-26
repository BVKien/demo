using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class MessageGroupDTO
    {
        public class CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO
        {
            public int? GroupChatId { get; set; }
        }

        public class GetAllMemberForUserAsyncForAdminDOETDeanLecturerMentorDTO
        {
            public string? Name { get; set; }
            public string? Image { get; set; }
            public string? UserCode { get; set; }
        }

        public class MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO
        {
            public int? GroupChatId { get; set; }
            public string? MessageContent { get; set; }
            public string? MessageFile { get; set; }
            public string? Image { get; set; }
            public string? StudentName { get; set; }
            public string? UniversityName { get; set; }
            public string? MentorName { get; set; }
            public DateTime? JoinAt { get; set; }
            public DateTime? OutAt { get; set; }
            public bool? IsAdmin { get; set; }
            public bool? IsRead { get; set; }
            public string? Status { get; set; }
        }

        public class CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO
        {
            public int? GroupChatId { get; set; }
            public string? MessageContent { get; set; }
            public string? MessageFile { get; set; }
            public string? Image { get; set; }
            public bool? IsRead { get; set; }
        }
    }
}
