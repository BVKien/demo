using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Appllication
    {
        public Appllication()
        {
            Notifications = new HashSet<Notification>();
        }

        public int ApplicationId { get; set; }
        public int? StudentId { get; set; }
        public int? JobId { get; set; }
        public string? TestFile { get; set; }
        public string? Feedback { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string? CoverLetter { get; set; }
        public int? CvId { get; set; }
        public string? CvFile { get; set; }
        public string? StudentRejectReason { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Cv? Cv { get; set; }
        public virtual Job? Job { get; set; }
        public virtual Student? Student { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
