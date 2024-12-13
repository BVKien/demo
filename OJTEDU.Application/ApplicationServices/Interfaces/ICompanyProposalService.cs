using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.CompanyProposalDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface ICompanyProposalService
    {
        // Student 
        Task<DataResponse<List<CompanyProposalListForStudentDTO>>> GetAllCompanyProposalByStudentIdAsync(int? userId);
        Task<DataResponse<CompanyProposalDetailForStudentDTO>> GetCompanyProposalDetailByIdAsync(int? companyProposalId);
        Task<DataResponse<CreateCompanyProposalForStudentDTO>> CreateCompanyProposalAsync(int? userId, CreateCompanyProposalForStudentDTO? companyProposalInfo, string? fileName, string? fileData);
    }
}
