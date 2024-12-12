using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class NotificationDTO
    {
        // Uni, Company, Student
        public partial class NotificationForUniCompanyStudentDTO
        {
            public int NotificationId { get; set; }
            public string? NotificationContent { get; set; }
            public string? Image { get; set; }
            public int? StudentId { get; set; }
            public string? StudentName { get; set; }
            public int? UniversityId { get; set; }
            public string? UniversityName { get; set; }
            public int? CompanyId { get; set; }
            public string? CompanyName { get; set; }
            public bool? IsRead { get; set; }
            public string? Status { get; set; }
            public int? ApplicationId { get; set; }
            public int? SupportRequestId { get; set; }
            public int? CompanyProposalId { get; set; }
            public int? FeedbackId { get; set; }
            public int? MessageId { get; set; }
            public int? GroupChatId { get; set; }
            public int? MessageGroupId { get; set; }
            public DateTime? CreatedAt { get; set; }
        }
    }
}
