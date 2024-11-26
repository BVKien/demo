using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Student
    {
        public Student()
        {
            Appllications = new HashSet<Appllication>();
            CompanyProposals = new HashSet<CompanyProposal>();
            Cvs = new HashSet<Cv>();
            Evaluations = new HashSet<Evaluation>();
            Feedbacks = new HashSet<Feedback>();
            Internships = new HashSet<Internship>();
            MessageGroups = new HashSet<MessageGroup>();
            Messages = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
            SupportRequests = new HashSet<SupportRequest>();
            WorkingReports = new HashSet<WorkingReport>();
        }

        public int StudentId { get; set; }
        public string? AlternativeEmail { get; set; }
        public string? Phone { get; set; }
        public DateTime? Dob { get; set; }
        public bool? Gender { get; set; }
        public int? UserId { get; set; }
        public int? SemesterId { get; set; }
        public int? MajorId { get; set; }
        public int? LecturerId { get; set; }
        public int? AddressId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Address? Address { get; set; }
        public virtual User? Lecturer { get; set; }
        public virtual Major? Major { get; set; }
        public virtual Semester? Semester { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Appllication> Appllications { get; set; }
        public virtual ICollection<CompanyProposal> CompanyProposals { get; set; }
        public virtual ICollection<Cv> Cvs { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Internship> Internships { get; set; }
        public virtual ICollection<MessageGroup> MessageGroups { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<SupportRequest> SupportRequests { get; set; }
        public virtual ICollection<WorkingReport> WorkingReports { get; set; }
    }
}
