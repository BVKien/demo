using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public string? NotificationContent { get; set; }
        public string? Image { get; set; }
        public int? StudentId { get; set; }
        public int? UniversityId { get; set; }
        public int? CompanyId { get; set; }
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
        public DateTime? UpdatedAt { get; set; }

        public virtual Appllication? Application { get; set; }
        public virtual Company? Company { get; set; }
        public virtual CompanyProposal? CompanyProposal { get; set; }
        public virtual Feedback? Feedback { get; set; }
        public virtual GroupChat? GroupChat { get; set; }
        public virtual Message? Message { get; set; }
        public virtual MessageGroup? MessageGroup { get; set; }
        public virtual Student? Student { get; set; }
        public virtual SupportRequest? SupportRequest { get; set; }
        public virtual User? University { get; set; }
    }
}
