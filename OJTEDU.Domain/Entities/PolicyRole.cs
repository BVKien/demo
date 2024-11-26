using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class PolicyRole
    {
        public int PolicyRoleId { get; set; }
        public int? PolicyId { get; set; }
        public int? RoleId { get; set; }

        public virtual Policy? Policy { get; set; }
        public virtual Role? Role { get; set; }
    }
}
