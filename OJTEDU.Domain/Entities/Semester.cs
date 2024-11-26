using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Semester
    {
        public Semester()
        {
            Internships = new HashSet<Internship>();
            Students = new HashSet<Student>();
        }

        public int SemesterId { get; set; }
        public string? SemesterCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Internship> Internships { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
