using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class CompanyProposalDTO
    {
        public class CompanyProposalListForStudentDTO
        {
            public int CompanyProposalId { get; set; }
            public string? ProposalTitle { get; set; }
            public string? ProposalContent { get; set; }
            public DateTime? ProposalDate { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class CompanyProposalDetailForStudentDTO
        {
            public int CompanyProposalId { get; set; }
            public string? Student { get; set; }
            public string? University { get; set; }
            public string? ProposalTitle { get; set; }
            public string? ProposalContent { get; set; }
            public string? ResponseContent { get; set; }
            public DateTime? ProposalDate { get; set; }
            public DateTime? ResponseDate { get; set; }
            public string? Contract { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class CreateCompanyProposalForStudentDTO
        {
            public int? StudentId { get; set; }
            public int? UniversityId { get; set; }
            public string? ProposalTitle { get; set; }
            public string? ProposalContent { get; set; }
            public string? Contract { get; set; }
        }

        public class CompanyProposalDto
        {
            public int CompanyProposalId { get; set; }
            public string? StudentName { get; set; }
            public string? ProposalTitle { get; set; }
            public string? ProposalContent { get; set; }
            public DateTime? ProposalDate { get; set; }
            public string? Contract { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }
        public class UpdateCompanyProposalStatusDto
        {
            public int CompanyProposalId { get; set; }
            public string Status { get; set; } // 0: Rejected, 1: Reviewing, 2: Accepted
            public string? ResponseContent { get; set; } // Nội dung phản hồi
        }


    }
}
