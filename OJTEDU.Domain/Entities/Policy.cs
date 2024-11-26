using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Policy
    {
        public Policy()
        {
            PolicyRoles = new HashSet<PolicyRole>();
        }

        public int PolicyId { get; set; }
        public int? ParentId { get; set; }
        public string? PolicyContent { get; set; }
        public int? UserId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<PolicyRole> PolicyRoles { get; set; }
    }
}
