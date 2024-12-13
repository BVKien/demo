using OJTEDU.Domain.Entities;

namespace OJTEDU.Api.Input.StudentControllers
{
    public class CompanyProposalController
    {
        public class CreateCompanyProposalInput
        {
            public string? ProposalTitle { get; set; }
            public string? ProposalContentFileName { get; set; }
            public string? ProposalContentFilePath { get; set; }
            public string? Contract { get; set; }
        }
    }
}