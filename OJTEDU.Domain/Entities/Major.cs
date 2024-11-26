using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Major
    {
        public Major()
        {
            Internships = new HashSet<Internship>();
            Jobs = new HashSet<Job>();
            Students = new HashSet<Student>();
            Users = new HashSet<User>();
        }

        public int MajorId { get; set; }
        public string? MajorCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? DepartmentId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Department? Department { get; set; }
        public virtual ICollection<Internship> Internships { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
