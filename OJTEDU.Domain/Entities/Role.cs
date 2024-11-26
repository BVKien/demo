using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Role
    {
        public Role()
        {
            DocumentRoles = new HashSet<DocumentRole>();
            NewsFaqroles = new HashSet<NewsFaqrole>();
            PolicyRoles = new HashSet<PolicyRole>();
            UserGuides = new HashSet<UserGuide>();
            Users = new HashSet<User>();
        }

        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<DocumentRole> DocumentRoles { get; set; }
        public virtual ICollection<NewsFaqrole> NewsFaqroles { get; set; }
        public virtual ICollection<PolicyRole> PolicyRoles { get; set; }
        public virtual ICollection<UserGuide> UserGuides { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
