using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class SupportRequest
    {
        public SupportRequest()
        {
            Notifications = new HashSet<Notification>();
        }

        public int SupportRequestId { get; set; }
        public int? StudentId { get; set; }
        public int? UniversityId { get; set; }
        public string? RequestContent { get; set; }
        public string? FeedbackContent { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Student? Student { get; set; }
        public virtual User? University { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
