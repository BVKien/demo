using OJTEDU.Domain.Entities;

namespace OJTEDU.Api.Input.StudentControllers
{
    public class CompanyProposalController
    {
        public class CreateCompanyProposalInput
        {
            public string? ProposalContent { get; set; }
            public string? Contract { get; set; }
        }
    }
}