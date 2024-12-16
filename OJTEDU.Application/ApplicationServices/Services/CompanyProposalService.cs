using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<DataResponse<CreateCompanyProposalForStudentDTO>> CreateCompanyProposalAsync(int? userId, CreateCompanyProposalForStudentDTO? companyProposalInfo, string? fileName)
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
                    ProposalTitle = companyProposalInfo?.ProposalTitle,
                    ProposalContent = companyProposalInfo?.ProposalContent,
                };

                var newCompanyProposal = await _companyProposalRepository.CreateCompanyProposalAsync(userId, companyProposal, fileName);
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

        public async Task<DataResponse<PagedResponse<List<CompanyProposalDto>>>> GetAllCompanyProposalsForDoetAsync(
               int userId, int pageNumber, int pageSize)
        {
            try
            {
                // Kiểm tra người dùng có phải DOET hay không
                var isDoet = await _companyProposalRepository.IsUserDoetAsync(userId);
                if (!isDoet)
                {
                    return new DataResponse<PagedResponse<List<CompanyProposalDto>>>
                    {
                        Data = null,
                        Message = "Access denied. Only DOET role can view company proposals.",
                        StatusCode = 403
                    };
                }

                // Lấy danh sách từ repository
                var companyProposals = await _companyProposalRepository.GetAllCompanyProposalsForDoetAsync();

                if (companyProposals == null || !companyProposals.Any())
                {
                    return new DataResponse<PagedResponse<List<CompanyProposalDto>>>
                    {
                        Data = null,
                        Message = "No company proposals found.",
                        StatusCode = 204
                    };
                }

                // Phân trang dữ liệu
                var totalProposals = companyProposals.Count;
                var totalPages = (int)Math.Ceiling((double)totalProposals / pageSize);

                var paginatedProposals = companyProposals
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Map sang DTO
                var proposalDtos = _mapper.Map<List<CompanyProposalDto>>(paginatedProposals);

                var pagedResponse = new PagedResponse<List<CompanyProposalDto>>
                {
                    Items = proposalDtos,
                    TotalCount = totalProposals,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<CompanyProposalDto>>>
                {
                    Data = pagedResponse,
                    Message = "Company proposals retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<CompanyProposalDto>>>
                {
                    Data = null,
                    Message = $"Error retrieving company proposals: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<string>> UpdateCompanyProposalStatusAsync(UpdateCompanyProposalStatusDto dto)
        {
            try
            {
                // Lấy CompanyProposal từ Repository
                var proposal = await _companyProposalRepository.GetCompanyProposalByIdAsync(dto.CompanyProposalId);

                if (proposal == null)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Company proposal not found.",
                        StatusCode = 404
                    };
                }

                // Cập nhật Status và ResponseContent
                proposal.Status = dto.Status;
                proposal.ResponseContent = dto.ResponseContent;
                proposal.UpdatedAt = DateTime.Now;

                await _companyProposalRepository.UpdateCompanyProposalAsync(proposal);

                return new DataResponse<string>
                {
                    Data = "Success",
                    Message = $"Company proposal updated successfully. New status: {dto.Status}",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Error updating proposal status: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
