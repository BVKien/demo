using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System.ComponentModel.Design;
using static OJTEDU.Application.DTOs.CompanyDTO;
using static OJTEDU.Application.DTOs.InternshipDTO;
using static OJTEDU.Application.DTOs.JobDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public CompanyService(ICompanyRepository companyRepository, IJobRepository jobRepository, IAddressRepository addressRepository, IUserRepository userRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _jobRepository = jobRepository;
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // Admin - DOET
        public async Task<DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>> GetAllCompaniesForAdminDoetAsync(string? companyName, string? companyCode, string? status, int? provinceId, int? districtId, int? wardId, int pageNumber, int pageSize)
        {
            try
            {
                var companies = await _companyRepository.GetAllCompaniesForAdminDoetAsync(companyName, companyCode, status, provinceId, districtId, wardId);

                var totalCompanies = companies.Count();
                var totalPages = totalCompanies == 0 ? 1 : (int)Math.Ceiling((double)totalCompanies / pageSize);

                var companyDtos = totalCompanies > 0 ? _mapper.Map<List<CompanyListForAdminDoetDTO>>(companies)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<CompanyListForAdminDoetDTO>();

                var pagedResponse = new PagedResponse<List<CompanyListForAdminDoetDTO>>
                {
                    Items = companyDtos,
                    TotalCount = totalCompanies,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Company list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = $"Access denied while get company list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving company list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<CompanyDetailForAdminDoetDTO>> GetCompanyDetailForAdminDoetAsync(int? companyId)
        {
            try
            {
                var company = await _companyRepository.GetCompanyDetailForAdminDoetAsync(companyId);

                if (company == null)
                {
                    return new DataResponse<CompanyDetailForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Company not found.",
                        StatusCode = 404
                    };
                }

                var companyDto = _mapper.Map<CompanyDetailForAdminDoetDTO>(company);
                companyDto.CompanyJobs = _mapper.Map<List<JobListByCompanyIdForAdminDooetDTO>>(company.Jobs);

                return new DataResponse<CompanyDetailForAdminDoetDTO>
                {
                    Data = companyDto,
                    Message = "Company details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<CompanyDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<CompanyDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Access denied while get company detail: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CompanyDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving company details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateCompanyForAdminDoetDTO>> UpdateCompanyForAdminDoetAsync(UpdateCompanyForAdminDoetDTO updateCompanyForAdminDoetDTO, int? provinceId, int? districtId, int? wardId, string? addressDetail)
        {
            try
            {
                var existingCompany = await _companyRepository.GetCompanyDetailForAdminDoetAsync(updateCompanyForAdminDoetDTO.CompanyId);
                if (existingCompany == null)
                {
                    return new DataResponse<UpdateCompanyForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Company not found.",
                        StatusCode = 404
                    };
                }

                if (!string.IsNullOrEmpty(updateCompanyForAdminDoetDTO.CompanyName))
                {
                    var userToUpdate = await _userRepository.GetUserByIdForAdminAsync(existingCompany.UserId.Value);
                    if (userToUpdate != null)
                    {
                        userToUpdate.Name = updateCompanyForAdminDoetDTO.CompanyName;
                        await _userRepository.UpdateUserForAdminAsync(userToUpdate);
                    }
                }

                // Kiểm tra và cập nhật thông tin địa chỉ nếu có
                if (provinceId.HasValue && districtId.HasValue && wardId.HasValue && !string.IsNullOrWhiteSpace(addressDetail))
                {
                    var address = existingCompany.Address ?? new Address();

                    address.ProvinceId = provinceId.Value;
                    address.DistrictId = districtId.Value;
                    address.WardId = wardId.Value;
                    address.Detail = addressDetail;
                    address.Status = "Active";
                    address.CreatedAt = existingCompany.AddressId == null ? DateTime.Now : existingCompany.Address.CreatedAt;
                    address.UpdatedAt = DateTime.Now;

                    // Lưu hoặc cập nhật Address
                    if (existingCompany.AddressId == null)
                    {
                        existingCompany.AddressId = await _addressRepository.AddAddressAsync(address);
                    }
                    else
                    {
                        existingCompany.AddressId = await _addressRepository.UpdateAddressAsync(address);
                    }
                }

                // Cập nhật các thông tin còn lại của công ty
                existingCompany.TaxCode = updateCompanyForAdminDoetDTO.TaxCode ?? existingCompany.TaxCode;
                existingCompany.AlternativeEmail = updateCompanyForAdminDoetDTO.ContactEmail;
                existingCompany.Phone = updateCompanyForAdminDoetDTO.Phone ?? existingCompany.Phone;
                existingCompany.Website = updateCompanyForAdminDoetDTO.Website;
                existingCompany.Description = updateCompanyForAdminDoetDTO.Description;
                existingCompany.UpdatedAt = DateTime.Now;

                // Gọi repository để lưu cập nhật
                await _companyRepository.UpdateCompanyForAdminDoetAsync(existingCompany);

                var responseDto = _mapper.Map<UpdateCompanyForAdminDoetDTO>(existingCompany);
                return new DataResponse<UpdateCompanyForAdminDoetDTO>
                {
                    Data = responseDto,
                    Message = "Company updated successfully.",
                    StatusCode = 200
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateCompanyForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Access denied while update company: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateCompanyForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating company: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        // Guest 
        public async Task<PagedResult<List<CompanySearchListForGuestDTO>>> SearchCompaniesAsync(string? name, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                // Fetch companies with pagination and total count
                var (companies, totalRecords) = await _companyRepository.SearchCompaniesAsync(name, provinceId, districtId, wardId, pageNumber, pageSize);
                var response = _mapper.Map<List<CompanySearchListForGuestDTO>>(companies);

                // Calculate the total number of pages
                int totalPages = pageSize.HasValue ? (int)Math.Ceiling((double)totalRecords / pageSize.Value) : 1;

                return new PagedResult<List<CompanySearchListForGuestDTO>>
                {
                    StatusCode = 200,
                    Message = "Company list retrieved successfully!",
                    TotalPages = totalPages,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<List<CompanySearchListForGuestDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving company list: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CompanyDetailForGuestDTO>> GetCompanyDetailByCompanyIdAsync(int? companyId)
        {
            try
            {
                if (companyId == null)
                {
                    return new DataResponse<CompanyDetailForGuestDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var company = await _companyRepository.GetCompanyDetailByCompanyIdAsync(companyId);
                var response = _mapper.Map<CompanyDetailForGuestDTO>(company);

                return new DataResponse<CompanyDetailForGuestDTO>
                {
                    StatusCode = 200,
                    Message = "Company detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CompanyDetailForGuestDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving company detail: {ex.Message}. ",
                    Data = null
                };
            }
        }

        // Student
        public async Task<PagedResult<List<CompanySearchListForStudentDTO>>> SearchCompaniesForStudentAsync(string? name, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                var (companies, totalRecords) = await _companyRepository.SearchCompaniesAsync(name, provinceId, districtId, wardId, pageNumber, pageSize);

                // Get all company IDs from the paginated companies
                var companyIds = companies.Select(c => c.CompanyId).ToArray();
                var jobCounts = await _jobRepository.GetJobCountsByCompanyIdsAsync(companyIds);

                // Calculate the total number of pages
                int totalPages = pageSize.HasValue ? (int)Math.Ceiling((double)totalRecords / pageSize.Value) : 1;

                // Map companies to DTO
                var response = _mapper.Map<List<CompanySearchListForStudentDTO>>(companies);

                // Assign job counts to the mapped DTOs
                foreach (var dto in response)
                {
                    dto.JobCount = jobCounts.TryGetValue(dto.CompanyId, out int count) ? count : 0;
                }

                return new PagedResult<List<CompanySearchListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Company list retrieved successfully!",
                    TotalPages = totalPages,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<List<CompanySearchListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving company list: {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CompanyDetailForStudentDTO>> GetCompanyDetailByCompanyIdForStudentAsync(int? companyId)
        {
            try
            {
                if (companyId == null)
                {
                    return new DataResponse<CompanyDetailForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var company = await _companyRepository.GetCompanyDetailByCompanyIdAsync(companyId);
                var jobList = await _jobRepository.GetAllJobsByCompanyIdAsync(companyId);

                // Map company to DTO
                var response = _mapper.Map<CompanyDetailForStudentDTO>(company);

                // Map the job list to DTOs and assign it to the company DTO
                response.JobList = _mapper.Map<List<JobListByCompanyIdForStudentDTO>>(jobList);

                return new DataResponse<CompanyDetailForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Company detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CompanyDetailForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving company detail: {ex.Message}.",
                    Data = null
                };
            }
        }

        // Company 
        public async Task<DataResponse<List<MentorListForCompanyDTO>>> GetMentorsListAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<MentorListForCompanyDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var internships = await _companyRepository.GetMentorsListAsync(userId);
                var response = _mapper.Map<List<MentorListForCompanyDTO>>(internships);

                return new DataResponse<List<MentorListForCompanyDTO>>
                {
                    StatusCode = 200,
                    Message = "Mentors list for company retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<MentorListForCompanyDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving mentors list for company: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<MentorsInfoListForCompanyDTO>>> GetAllMentorsInfoAsync()
        {
            try
            {
                var mentors = await _companyRepository.GetAllMentorsInfoAsync();
                var response = _mapper.Map<List<MentorsInfoListForCompanyDTO>>(mentors);

                return new DataResponse<List<MentorsInfoListForCompanyDTO>>
                {
                    StatusCode = 200,
                    Message = "Mentors list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<MentorsInfoListForCompanyDTO>>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
