using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.CompanyDTO;
using static OJTEDU.Application.DTOs.CompanyProposalDTO;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class CompanyProposalService : ICompanyProposalService
    {
        private readonly ICompanyProposalRepository _companyProposalRepository;
        private readonly IMapper _mapper;
        public CompanyProposalService(ICompanyProposalRepository companyProposalRepository, IMapper mapper)
        {
            _companyProposalRepository = companyProposalRepository;
            _mapper = mapper;
        }

        // Student 
        public async Task<DataResponse<List<CompanyProposalListForStudentDTO>>> GetAllCompanyProposalByStudentIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<CompanyProposalListForStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var companyProposal = await _companyProposalRepository.GetAllCompanyProposalByStudentIdAsync(userId);
                var response = _mapper.Map<List<CompanyProposalListForStudentDTO>>(companyProposal);

                return new DataResponse<List<CompanyProposalListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Company proposal list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<CompanyProposalListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving company proposal list: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CompanyProposalDetailForStudentDTO>> GetCompanyProposalDetailByIdAsync(int? companyProposalId)
        {
            try
            {
                if (companyProposalId == null)
                {
                    return new DataResponse<CompanyProposalDetailForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found company proposal.",
                        Data = null
                    };
                }

                var companyProposal = await _companyProposalRepository.GetCompanyProposalDetailByIdAsync(companyProposalId);
                var response = _mapper.Map<CompanyProposalDetailForStudentDTO>(companyProposal);

                return new DataResponse<CompanyProposalDetailForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Company proposal detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CompanyProposalDetailForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving company proposal detail: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CreateCompanyProposalForStudentDTO>> CreateCompanyProposalAsync(int? userId, CreateCompanyProposalForStudentDTO? companyProposalInfo, string? fileName, byte[] fileData)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateCompanyProposalForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var companyProposal = new CompanyProposal
                {
                    ProposalContent = companyProposalInfo?.ProposalContent,
                };

                var newCompanyProposal = await _companyProposalRepository.CreateCompanyProposalAsync(userId, companyProposal, fileName, fileData);
                var response = _mapper.Map<CreateCompanyProposalForStudentDTO>(newCompanyProposal);

                return new DataResponse<CreateCompanyProposalForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Create company proposal successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateCompanyProposalForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error create company proposal jpb: {ex.Message}. ",
                    Data = null
                };
            }
        }
    }
}
