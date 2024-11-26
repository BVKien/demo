using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class User
    {
        public User()
        {
            Banners = new HashSet<Banner>();
            Companies = new HashSet<Company>();
            CompanyProposals = new HashSet<CompanyProposal>();
            DocumentUniversities = new HashSet<Document>();
            DocumentUsers = new HashSet<Document>();
            Evaluations = new HashSet<Evaluation>();
            Feedbacks = new HashSet<Feedback>();
            GroupChats = new HashSet<GroupChat>();
            InternshipProcesses = new HashSet<InternshipProcess>();
            Internships = new HashSet<Internship>();
            MessageGroups = new HashSet<MessageGroup>();
            Messages = new HashSet<Message>();
            NewsFaqs = new HashSet<NewsFaq>();
            Notifications = new HashSet<Notification>();
            Policies = new HashSet<Policy>();
            StudentLecturers = new HashSet<Student>();
            StudentUsers = new HashSet<Student>();
            SupportRequests = new HashSet<SupportRequest>();
            WorkingReports = new HashSet<WorkingReport>();
        }

        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public int? RoleId { get; set; }
        public string? Name { get; set; }
        public string? UserCode { get; set; }
        public string? Image { get; set; }
        public string? Information { get; set; }
        public int? ForCompany { get; set; }
        public int? AssignForId { get; set; }
        public int? DepartmentId { get; set; }
        public int? MajorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Major? Major { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Banner> Banners { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<CompanyProposal> CompanyProposals { get; set; }
        public virtual ICollection<Document> DocumentUniversities { get; set; }
        public virtual ICollection<Document> DocumentUsers { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<GroupChat> GroupChats { get; set; }
        public virtual ICollection<InternshipProcess> InternshipProcesses { get; set; }
        public virtual ICollection<Internship> Internships { get; set; }
        public virtual ICollection<MessageGroup> MessageGroups { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<NewsFaq> NewsFaqs { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Policy> Policies { get; set; }
        public virtual ICollection<Student> StudentLecturers { get; set; }
        public virtual ICollection<Student> StudentUsers { get; set; }
        public virtual ICollection<SupportRequest> SupportRequests { get; set; }
        public virtual ICollection<WorkingReport> WorkingReports { get; set; }
    }
}
