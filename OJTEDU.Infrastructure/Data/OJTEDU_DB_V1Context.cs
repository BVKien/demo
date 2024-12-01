using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OJTEDU.Domain.Entities;

namespace OJTEDU.Infrastructure.Data
{
    public partial class OJTEDU_DB_V1Context : DbContext
    {
        public OJTEDU_DB_V1Context()
        {
        }

        public OJTEDU_DB_V1Context(DbContextOptions<OJTEDU_DB_V1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Appllication> Appllications { get; set; } = null!;
        public virtual DbSet<AttendanceReport> AttendanceReports { get; set; } = null!;
        public virtual DbSet<Banner> Banners { get; set; } = null!;
        public virtual DbSet<Company> Companies { get; set; } = null!;
        public virtual DbSet<CompanyProposal> CompanyProposals { get; set; } = null!;
        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<ContractType> ContractTypes { get; set; } = null!;
        public virtual DbSet<Cv> Cvs { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<DocumentRole> DocumentRoles { get; set; } = null!;
        public virtual DbSet<Evaluation> Evaluations { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<GroupChat> GroupChats { get; set; } = null!;
        public virtual DbSet<Internship> Internships { get; set; } = null!;
        public virtual DbSet<InternshipProcess> InternshipProcesses { get; set; } = null!;
        public virtual DbSet<Job> Jobs { get; set; } = null!;
        public virtual DbSet<Major> Majors { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<MessageGroup> MessageGroups { get; set; } = null!;
        public virtual DbSet<NewsFaq> NewsFaqs { get; set; } = null!;
        public virtual DbSet<NewsFaqrole> NewsFaqroles { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Policy> Policies { get; set; } = null!;
        public virtual DbSet<PolicyRole> PolicyRoles { get; set; } = null!;
        public virtual DbSet<Province> Provinces { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Semester> Semesters { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<SupportRequest> SupportRequests { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserGuide> UserGuides { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;
        public virtual DbSet<WorkingReport> WorkingReports { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Address_District");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Address_Province");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_Address_Ward");
            });

            modelBuilder.Entity<Appllication>(entity =>
            {
                entity.HasKey(e => e.ApplicationId)
                    .HasName("PK_Application");

                entity.ToTable("Appllication");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.InterviewDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Cv)
                    .WithMany(p => p.Appllications)
                    .HasForeignKey(d => d.CvId)
                    .HasConstraintName("FK_Application_Cv");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Appllications)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_Application_Job");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Appllications)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_Application_Student");
            });

            modelBuilder.Entity<AttendanceReport>(entity =>
            {
                entity.ToTable("AttendanceReport");

                entity.Property(e => e.CheckInTime).HasColumnType("time(0)");

                entity.Property(e => e.CheckOutTime).HasColumnType("time(0)");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Internship)
                    .WithMany(p => p.AttendanceReports)
                    .HasForeignKey(d => d.InternshipId)
                    .HasConstraintName("FK_AttendanceReport_Internship");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.AttendanceReports)
                    .HasForeignKey(d => d.MentorId)
                    .HasConstraintName("FK_AttendanceReport_Company");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.ToTable("Banner");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EventDate).HasColumnType("datetime");

                entity.Property(e => e.Link).HasColumnType("text");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Banners)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Layout_User");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Company");

                entity.Property(e => e.AlternativeEmail).HasMaxLength(50);

                entity.Property(e => e.CheckInTime).HasColumnType("time(0)");

                entity.Property(e => e.CheckOutTime).HasColumnType("time(0)");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Website).HasMaxLength(100);

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_Company_Address");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Company_User");
            });

            modelBuilder.Entity<CompanyProposal>(entity =>
            {
                entity.ToTable("CompanyProposal");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ProposalDate).HasColumnType("datetime");

                entity.Property(e => e.ProposalTitle).HasMaxLength(250);

                entity.Property(e => e.ResponseDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.CompanyProposals)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_CompanyProposal_Student");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.CompanyProposals)
                    .HasForeignKey(d => d.UniversityId)
                    .HasConstraintName("FK_CompanyProposal_User");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contract");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Contract_Company");

                entity.HasOne(d => d.ContractType)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.ContractTypeId)
                    .HasConstraintName("FK_Contract_ContractType");
            });

            modelBuilder.Entity<ContractType>(entity =>
            {
                entity.ToTable("ContractType");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Cv>(entity =>
            {
                entity.ToTable("Cv");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(300);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Cvs)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_Cv_Student");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DepartmentCode).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("District");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_District_Province");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.DocumentUniversities)
                    .HasForeignKey(d => d.UniversityId)
                    .HasConstraintName("FK_Document_User");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DocumentUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Document_User1");
            });

            modelBuilder.Entity<DocumentRole>(entity =>
            {
                entity.HasOne(d => d.Document)
                    .WithMany(p => p.DocumentRoles)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("FK_DocumentRoles_Document");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.DocumentRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_DocumentRoles_Role");
            });

            modelBuilder.Entity<Evaluation>(entity =>
            {
                entity.ToTable("Evaluation");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Lecturer)
                    .WithMany(p => p.Evaluations)
                    .HasForeignKey(d => d.LecturerId)
                    .HasConstraintName("FK_Evaluation_User");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.Evaluations)
                    .HasForeignKey(d => d.MentorId)
                    .HasConstraintName("FK_Evaluation_Company");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Evaluations)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_Evaluation_Student");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Feedback_Company");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_Feedback_Student");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.UniversityId)
                    .HasConstraintName("FK_Feedback_User");
            });

            modelBuilder.Entity<GroupChat>(entity =>
            {
                entity.ToTable("GroupChat");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.GroupName).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.GroupChats)
                    .HasForeignKey(d => d.MentorId)
                    .HasConstraintName("FK_GroupChat_Company");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.GroupChats)
                    .HasForeignKey(d => d.UniversityId)
                    .HasConstraintName("FK_GroupChat_User");
            });

            modelBuilder.Entity<Internship>(entity =>
            {
                entity.HasKey(e => e.IntershipId)
                    .HasName("PK_Intership");

                entity.ToTable("Internship");

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Internships)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Intership_Company");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Internships)
                    .HasForeignKey(d => d.ContractId)
                    .HasConstraintName("FK_Intership_Contract");

                entity.HasOne(d => d.Evaluation)
                    .WithMany(p => p.Internships)
                    .HasForeignKey(d => d.EvaluationId)
                    .HasConstraintName("FK_Intership_Evaluation");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Internships)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_Internship_Job");

                entity.HasOne(d => d.Lecturer)
                    .WithMany(p => p.Internships)
                    .HasForeignKey(d => d.LecturerId)
                    .HasConstraintName("FK_Intership_User");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.Internships)
                    .HasForeignKey(d => d.MajorId)
                    .HasConstraintName("FK_Intership_Major");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Internships)
                    .HasForeignKey(d => d.SemesterId)
                    .HasConstraintName("FK_Intership_Semester");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Internships)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_Intership_Student");
            });

            modelBuilder.Entity<InternshipProcess>(entity =>
            {
                entity.HasKey(e => e.IntershipProcessId);

                entity.ToTable("InternshipProcess");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.InternshipProcesses)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_InternshipProcess_User");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job");

                entity.Property(e => e.Benefits).HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Deadline).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Requirements).HasMaxLength(255);

                entity.Property(e => e.SalaryRange).HasMaxLength(50);

                entity.Property(e => e.SkillRequirements).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.TestFile).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.WorkingHours).HasMaxLength(50);

                entity.HasOne(d => d.AddressedNavigation)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.Addressed)
                    .HasConstraintName("FK_Job_Address");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Job_Company");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.MajorId)
                    .HasConstraintName("FK_Job_Major");
            });

            modelBuilder.Entity<Major>(entity =>
            {
                entity.ToTable("Major");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.MajorCode).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Majors)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Major_Department");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Message_Company");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_Message_Student");

                entity.HasOne(d => d.Universiry)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.UniversiryId)
                    .HasConstraintName("FK_Message_User");
            });

            modelBuilder.Entity<MessageGroup>(entity =>
            {
                entity.ToTable("MessageGroup");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.JoinAt).HasColumnType("datetime");

                entity.Property(e => e.OutAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.GroupChat)
                    .WithMany(p => p.MessageGroups)
                    .HasForeignKey(d => d.GroupChatId)
                    .HasConstraintName("FK_GroupMessage_GroupChat");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.MessageGroups)
                    .HasForeignKey(d => d.MentorId)
                    .HasConstraintName("FK_MessageGroup_Company");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.MessageGroups)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_MessageGroup_Student");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.MessageGroups)
                    .HasForeignKey(d => d.UniversityId)
                    .HasConstraintName("FK_MessageGroup_User");
            });

            modelBuilder.Entity<NewsFaq>(entity =>
            {
                entity.ToTable("NewsFAQ");

                entity.Property(e => e.NewsFaqid).HasColumnName("NewsFAQId");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Image).HasColumnType("text");

                entity.Property(e => e.NewsFaqcontent).HasColumnName("NewsFAQContent");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NewsFaqs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_NewsFAQ_User");
            });

            modelBuilder.Entity<NewsFaqrole>(entity =>
            {
                entity.ToTable("NewsFAQRoles");

                entity.Property(e => e.NewsFaqroleId).HasColumnName("NewsFAQRoleId");

                entity.Property(e => e.NewsFaqid).HasColumnName("NewsFAQId");

                entity.HasOne(d => d.NewsFaq)
                    .WithMany(p => p.NewsFaqroles)
                    .HasForeignKey(d => d.NewsFaqid)
                    .HasConstraintName("FK_NewsFAQRoles_NewsFAQ");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.NewsFaqroles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_NewsFAQRoles_Role");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Image).HasColumnType("text");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("FK_Notification_Application");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Notification_Company");

                entity.HasOne(d => d.CompanyProposal)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.CompanyProposalId)
                    .HasConstraintName("FK_Notification_CompanyProposal");

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.FeedbackId)
                    .HasConstraintName("FK_Notification_Feedback");

                entity.HasOne(d => d.GroupChat)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.GroupChatId)
                    .HasConstraintName("FK_Notification_GroupChat");

                entity.HasOne(d => d.MessageGroup)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.MessageGroupId)
                    .HasConstraintName("FK_Notification_MessageGroup");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.MessageId)
                    .HasConstraintName("FK_Notification_Message");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_Notification_Student");

                entity.HasOne(d => d.SupportRequest)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.SupportRequestId)
                    .HasConstraintName("FK_Notification_SupportRequest");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UniversityId)
                    .HasConstraintName("FK_Notification_User");
            });

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.ToTable("Policy");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Policies)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Policy_User");
            });

            modelBuilder.Entity<PolicyRole>(entity =>
            {
                entity.HasOne(d => d.Policy)
                    .WithMany(p => p.PolicyRoles)
                    .HasForeignKey(d => d.PolicyId)
                    .HasConstraintName("FK_PolicyRoles_Policy");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.PolicyRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_PolicyRoles_Role");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.ToTable("Province");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.ToTable("Semester");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.SemesterCode).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.AlternativeEmail).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_Student_Address");

                entity.HasOne(d => d.Lecturer)
                    .WithMany(p => p.StudentLecturers)
                    .HasForeignKey(d => d.LecturerId)
                    .HasConstraintName("FK_Student_User");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.MajorId)
                    .HasConstraintName("FK_Student_Major");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.SemesterId)
                    .HasConstraintName("FK_Student_Semester");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StudentUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Student_User1");
            });

            modelBuilder.Entity<SupportRequest>(entity =>
            {
                entity.ToTable("SupportRequest");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.RequestTitle).HasMaxLength(250);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.SupportRequests)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_SupportRequest_Student");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.SupportRequests)
                    .HasForeignKey(d => d.UniversityId)
                    .HasConstraintName("FK_SupportRequest_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Image).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(350);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserCode).HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_User_Department");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.MajorId)
                    .HasConstraintName("FK_User_Major");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_Role");
            });

            modelBuilder.Entity<UserGuide>(entity =>
            {
                entity.ToTable("UserGuide");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserGuides)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_UserGuide_Role");
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.ToTable("Ward");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Ward_District");
            });

            modelBuilder.Entity<WorkingReport>(entity =>
            {
                entity.ToTable("WorkingReport");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

                entity.Property(e => e.ReportDate).HasColumnType("datetime");

                entity.Property(e => e.ReportTitle).HasMaxLength(250);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Lecturer)
                    .WithMany(p => p.WorkingReports)
                    .HasForeignKey(d => d.LecturerId)
                    .HasConstraintName("FK_WorkingReport_User");

                entity.HasOne(d => d.Mentor)
                    .WithMany(p => p.WorkingReports)
                    .HasForeignKey(d => d.MentorId)
                    .HasConstraintName("FK_WorkingReport_Company");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.WorkingReports)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_WorkingReport_Student");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
