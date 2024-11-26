using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class AttendanceReport
    {
        public int AttendanceReportId { get; set; }
        public int? MentorId { get; set; }
        public int? InternshipId { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public bool? Late { get; set; }
        public bool? EarlyLeave { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Internship? Internship { get; set; }
        public virtual Company? Mentor { get; set; }
    }
}
