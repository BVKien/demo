using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface ICompanyProposalRepository
    {
        /*
         + Company proposal status:
        0: Rejected
        1: Reviewing
        2: Accepted
         */

        // Student 
        Task<IEnumerable<CompanyProposal>> GetAllCompanyProposalByStudentIdAsync(int? userId);
        Task<CompanyProposal> GetCompanyProposalDetailByIdAsync(int? companyProposalId);
        Task<CompanyProposal> CreateCompanyProposalAsync(int? userId, CompanyProposal? companyProposalInfo, string? fileName);

        // University side - missing
        Task<List<CompanyProposal>> GetAllCompanyProposalsForDoetAsync();
        Task<bool> IsUserDoetAsync(int userId);
        Task<CompanyProposal> GetCompanyProposalByIdAsync(int proposalId);
        Task UpdateCompanyProposalAsync(CompanyProposal proposal);
    }
}
