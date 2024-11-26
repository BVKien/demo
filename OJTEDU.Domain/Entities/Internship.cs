using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Internship
    {
        public Internship()
        {
            AttendanceReports = new HashSet<AttendanceReport>();
        }

        public int IntershipId { get; set; }
        public int? StudentId { get; set; }
        public int? CompanyId { get; set; }
        public int? JobId { get; set; }
        public int? LecturerId { get; set; }
        public string? Code { get; set; }
        public string? InformationDetail { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int? ContractId { get; set; }
        public int? SemesterId { get; set; }
        public int? MajorId { get; set; }
        public int? EvaluationId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Company? Company { get; set; }
        public virtual Contract? Contract { get; set; }
        public virtual Evaluation? Evaluation { get; set; }
        public virtual Job? Job { get; set; }
        public virtual User? Lecturer { get; set; }
        public virtual Major? Major { get; set; }
        public virtual Semester? Semester { get; set; }
        public virtual Student? Student { get; set; }
        public virtual ICollection<AttendanceReport> AttendanceReports { get; set; }
    }
}
