using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Department
    {
        public Department()
        {
            Majors = new HashSet<Major>();
            Users = new HashSet<User>();
        }

        public int DepartmentId { get; set; }
        public string? DepartmentCode { get; set; }
        public string? Name { get; set; }
        public string? Detail { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Major> Majors { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
