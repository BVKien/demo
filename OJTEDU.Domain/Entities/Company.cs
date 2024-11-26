using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Company
    {
        public Company()
        {
            AttendanceReports = new HashSet<AttendanceReport>();
            Contracts = new HashSet<Contract>();
            Evaluations = new HashSet<Evaluation>();
            Feedbacks = new HashSet<Feedback>();
            GroupChats = new HashSet<GroupChat>();
            Internships = new HashSet<Internship>();
            Jobs = new HashSet<Job>();
            MessageGroups = new HashSet<MessageGroup>();
            Messages = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
            WorkingReports = new HashSet<WorkingReport>();
        }

        public int CompanyId { get; set; }
        public string? AlternativeEmail { get; set; }
        public string? Phone { get; set; }
        public string? TaxCode { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public int? UserId { get; set; }
        public int? AddressId { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Address? Address { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<AttendanceReport> AttendanceReports { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<GroupChat> GroupChats { get; set; }
        public virtual ICollection<Internship> Internships { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<MessageGroup> MessageGroups { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<WorkingReport> WorkingReports { get; set; }
    }
}
