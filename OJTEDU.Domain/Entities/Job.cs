using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Job
    {
        public Job()
        {
            Appllications = new HashSet<Appllication>();
            Internships = new HashSet<Internship>();
        }

        public int JobId { get; set; }
        public int? CompanyId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TestFile { get; set; }
        public string? SalaryRange { get; set; }
        public string? Requirements { get; set; }
        public string? SkillRequirements { get; set; }
        public string? Benefits { get; set; }
        public string? WorkingHours { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Status { get; set; }
        public int? MajorId { get; set; }
        public int? Addressed { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Address? AddressedNavigation { get; set; }
        public virtual Company? Company { get; set; }
        public virtual Major? Major { get; set; }
        public virtual ICollection<Appllication> Appllications { get; set; }
        public virtual ICollection<Internship> Internships { get; set; }
    }
}
