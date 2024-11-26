using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Message
    {
        public Message()
        {
            Notifications = new HashSet<Notification>();
        }

        public int MessageId { get; set; }
        public int? ConversationId { get; set; }
        public string? MessageContent { get; set; }
        public string? MessageFile { get; set; }
        public string? Image { get; set; }
        public int? StudentId { get; set; }
        public int? UniversiryId { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsRead { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Company? Company { get; set; }
        public virtual Student? Student { get; set; }
        public virtual User? Universiry { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
