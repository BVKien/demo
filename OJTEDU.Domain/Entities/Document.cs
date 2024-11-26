using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Document
    {
        public Document()
        {
            DocumentRoles = new HashSet<DocumentRole>();
        }

        public int DocumentId { get; set; }
        public int? UniversityId { get; set; }
        public int? UserId { get; set; }
        public string? Title { get; set; }
        public string? DocumentFile { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User? University { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<DocumentRole> DocumentRoles { get; set; }
    }
}
