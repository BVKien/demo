using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class MessageGroup
    {
        public MessageGroup()
        {
            Notifications = new HashSet<Notification>();
        }

        public int MessageGroupId { get; set; }
        public int? GroupChatId { get; set; }
        public string? MessageContent { get; set; }
        public string? MessageFile { get; set; }
        public string? Image { get; set; }
        public int? StudentId { get; set; }
        public int? UniversityId { get; set; }
        public int? MentorId { get; set; }
        public DateTime? JoinAt { get; set; }
        public DateTime? OutAt { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsRead { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual GroupChat? GroupChat { get; set; }
        public virtual Company? Mentor { get; set; }
        public virtual Student? Student { get; set; }
        public virtual User? University { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
