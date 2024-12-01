using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class CompanyProposal
    {
        public CompanyProposal()
        {
            Notifications = new HashSet<Notification>();
        }

        public int CompanyProposalId { get; set; }
        public int? StudentId { get; set; }
        public int? UniversityId { get; set; }
        public int? ResponseForProposalId { get; set; }
        public string? ProposalTitle { get; set; }
        public string? ProposalContent { get; set; }
        public string? ResponseContent { get; set; }
        public DateTime? ProposalDate { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string? Contract { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Student? Student { get; set; }
        public virtual User? University { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
