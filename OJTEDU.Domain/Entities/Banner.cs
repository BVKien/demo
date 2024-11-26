using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Banner
    {
        public int BannerId { get; set; }
        public string? Image { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Link { get; set; }
        public int? UserId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User? User { get; set; }
    }
}
