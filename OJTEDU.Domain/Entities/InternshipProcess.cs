using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class InternshipProcess
    {
        public int IntershipProcessId { get; set; }
        public string? Title { get; set; }
        public string? FilePath { get; set; }
        public int? CreatedBy { get; set; }
        public bool? IsVisible { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
    }
}
