using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class DocumentRole
    {
        public int DocumentRoleId { get; set; }
        public int? DocumentId { get; set; }
        public int? RoleId { get; set; }

        public virtual Document? Document { get; set; }
        public virtual Role? Role { get; set; }
    }
}
