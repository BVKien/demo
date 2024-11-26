using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class UserGuide
    {
        public int UserGuideId { get; set; }
        public string? Title { get; set; }
        public string? UserGuideFile { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
    }
}
