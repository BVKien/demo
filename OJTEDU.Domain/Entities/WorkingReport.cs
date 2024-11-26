using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class WorkingReport
    {
        public int WorkingReportId { get; set; }
        public int? MentorId { get; set; }
        public int? LecturerId { get; set; }
        public int? StudentId { get; set; }
        public string? ReportTitle { get; set; }
        public string? ReportContent { get; set; }
        public DateTime? ReportDate { get; set; }
        public string? FeedbackFromLecturer { get; set; }
        public string? FeedbackFromMentor { get; set; }
        public string? FileAttachment { get; set; }
        public double? LecturerScore { get; set; }
        public double? MentorScore { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User? Lecturer { get; set; }
        public virtual Company? Mentor { get; set; }
        public virtual Student? Student { get; set; }
    }
}
