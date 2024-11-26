using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class NewsFaqrole
    {
        public int NewsFaqroleId { get; set; }
        public int? NewsFaqid { get; set; }
        public int? RoleId { get; set; }

        public virtual NewsFaq? NewsFaq { get; set; }
        public virtual Role? Role { get; set; }
    }
}
