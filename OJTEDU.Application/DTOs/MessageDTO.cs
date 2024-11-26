using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class MessageDTO
    {
        public class CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO
        {
            public int? ConversationId { get; set; }
            public string? MessageContent { get; set; }
            public string? MessageFile { get; set; }
            public string? Image { get; set; }
            public bool? IsRead { get; set; }
        }

        public class MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO
        {
            public int MessageId { get; set; }
            public int? ConversationId { get; set; }
            public string? MessageContent { get; set; }
            public string? MessageFile { get; set; }
            public string? Image { get; set; }
            public string? StudentName { get; set; }
            public string? StudentImage { get; set; }
            public string? UniversiryName { get; set; }
            public string? UniversiryImage { get; set; }
            public string? CompanyName { get; set; }
            public string? CompanyImage { get; set; }
            public bool? IsRead { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}