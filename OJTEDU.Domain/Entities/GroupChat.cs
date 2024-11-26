using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class GroupChat
    {
        public GroupChat()
        {
            MessageGroups = new HashSet<MessageGroup>();
            Notifications = new HashSet<Notification>();
        }

        public int GroupChatId { get; set; }
        public string? GroupName { get; set; }
        public int? UniversityId { get; set; }
        public int? MentorId { get; set; }
        public bool? IsAdmin { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Company? Mentor { get; set; }
        public virtual User? University { get; set; }
        public virtual ICollection<MessageGroup> MessageGroups { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
