using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class NewsFaq
    {
        public NewsFaq()
        {
            NewsFaqroles = new HashSet<NewsFaqrole>();
        }

        public int NewsFaqid { get; set; }
        public int? UserId { get; set; }
        public string? Title { get; set; }
        public int? ParentId { get; set; }
        public string? Image { get; set; }
        public string? NewsFaqcontent { get; set; }
        public bool? IsNews { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<NewsFaqrole> NewsFaqroles { get; set; }
    }
}
