using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System.Xml.Linq;
using static OJTEDU.Application.DTOs.JobDTO;
using OJTEDU.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using AutoMapper.Configuration.Annotations;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;
        public JobService(IJobRepository jobRepository, IMapper mapper)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
        }

        // Student  
        public async Task<DataResponse<List<JobListByCompanyIdForStudentDTO>>> GetAllJobsByCompanyIdAsync(int? companyId)
        {
            try
            {
                if (companyId == null)
                {
                    return new DataResponse<List<JobListByCompanyIdForStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var jobs = await _jobRepository.GetAllJobsByCompanyIdAsync(companyId);
                var response = _mapper.Map<List<JobListByCompanyIdForStudentDTO>>(jobs);

                return new DataResponse<List<JobListByCompanyIdForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Job list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<JobListByCompanyIdForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job list {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<PagedResult<List<JobListSearchForStudentDTO>>> SearchJobsAsync(int? userId, string? title, int? majorId, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                var (jobs, totalRecords) = await _jobRepository.SearchJobsAsync(userId, title, majorId, provinceId, districtId, wardId, pageNumber, pageSize);
                var response = _mapper.Map<List<JobListSearchForStudentDTO>>(jobs);

                // Calculate the total number of pages
                int totalPages = pageSize.HasValue ? (int)Math.Ceiling((double)totalRecords / pageSize.Value) : 1;

                return new PagedResult<List<JobListSearchForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Job list retrieved successfully!",
                    TotalPages = totalPages,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<List<JobListSearchForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job list: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<JobListForStudentDTO>>> GetAllJobsAsync()
        {
            try
            {
                var jobs = await _jobRepository.GetAllJobsAsync();
                var response = _mapper.Map<List<JobListForStudentDTO>>(jobs);

                return new DataResponse<List<JobListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Job list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<JobListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job list {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<JobDetailForStudentDTO>> GetJobDetailAsync(int? jobId)
        {
            try
            {
                if (jobId == null)
                {
                    return new DataResponse<JobDetailForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found job.",
                        Data = null
                    };
                }

                var jobs = await _jobRepository.GetJobDetailAsync(jobId);
                var response = _mapper.Map<JobDetailForStudentDTO>(jobs);

                return new DataResponse<JobDetailForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Job detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<JobDetailForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job detail {ex.Message}. ",
                    Data = null
                };
            }
        }

        // Company
        public async Task<DataResponse<List<JobListForCompanyDTO>>> GetAllJobsByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<JobListForCompanyDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var jobs = await _jobRepository.GetAllJobsByUserIdAsync(userId);
                var response = _mapper.Map<List<JobListForCompanyDTO>>(jobs);

                return new DataResponse<List<JobListForCompanyDTO>>
                {
                    StatusCode = 200,
                    Message = "Job list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<JobListForCompanyDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job list {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<JobDetailForCompanyDTO>> GetJobDetailForCompanyAsync(int? jobId)
        {
            try
            {
                if (jobId == null)
                {
                    return new DataResponse<JobDetailForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found job.",
                        Data = null
                    };
                }

                var job = await _jobRepository.GetJobDetailAsync(jobId);
                var response = _mapper.Map<JobDetailForCompanyDTO>(job);

                return new DataResponse<JobDetailForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Job detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<JobDetailForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job detail {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CreateJobForCompanyDTO>> CreateJobAsync(int? userId, string? fileName, byte[] fileData, CreateJobForCompanyDTO? info)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                if (info?.MajorId == null)
                {
                    return new DataResponse<CreateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Major is required.",
                        Data = null
                    };
                }

                if (info?.Title == null)
                {
                    return new DataResponse<CreateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Title is required.",
                        Data = null
                    };
                }

                if (info?.Deadline == null)
                {
                    return new DataResponse<CreateJobForCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "Deadline is required.",
                        Data = null
                    };
                }
                else if (info.Deadline <= DateTime.Now)
                {
                    return new DataResponse<CreateJobForCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "Deadline must be a future date.",
                        Data = null
                    };
                }

                if (info?.WardId == null || info?.DistrictId == null || info?.ProvinceId == null || string.IsNullOrWhiteSpace(info?.Detail))
                {
                    return new DataResponse<CreateJobForCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "A valid address with Ward, District, Province, and detail is required.",
                        Data = null
                    };
                }

                if (info?.SalaryRange == null || string.IsNullOrWhiteSpace(info.SalaryRange))
                {
                    return new DataResponse<CreateJobForCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "Salary range is required.",
                        Data = null
                    };
                }

                // Job
                var jobInfo = new Job
                {
                    Title = info?.Title,
                    Description = info?.Description,
                    SalaryRange = info?.SalaryRange,
                    Requirements = info?.Requirements,
                    SkillRequirements = info?.SkillRequirements,
                    Benefits = info?.Benefits,
                    WorkingHours = info?.WorkingHours,
                    Deadline = info?.Deadline,
                    MajorId = info?.MajorId,
                    Addressed = info?.Addressed,
                };

                // Address
                var addressInfo = new Address
                {
                    Detail = info?.Detail,
                    WardId = info?.WardId,
                    DistrictId = info?.DistrictId,
                    ProvinceId = info?.ProvinceId,
                };

                var job = await _jobRepository.CreateJobAsync(userId, fileName, fileData, jobInfo, addressInfo);
                var response = _mapper.Map<CreateJobForCompanyDTO>(job);

                return new DataResponse<CreateJobForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Create job successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateJobForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = $"Error create job: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<UpdateJobForCompanyDTO>> UpdateJobAsync(int? userId, int? jobId, string? fileName, byte[] fileData, UpdateJobForCompanyDTO? info)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<UpdateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                if (jobId == null)
                {
                    return new DataResponse<UpdateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Job is required.",
                        Data = null
                    };
                }

                var majorId = (int)info?.MajorId;
                if (majorId == null)
                {
                    return new DataResponse<UpdateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Major is required.",
                        Data = null
                    };
                }

                // Job
                var jobInfo = new Job
                {
                    Title = info?.Title,
                    Description = info?.Description,
                    SalaryRange = info?.SalaryRange,
                    Requirements = info?.Requirements,
                    SkillRequirements = info?.SkillRequirements,
                    Benefits = info?.Benefits,
                    WorkingHours = info?.WorkingHours,
                    Deadline = info?.Deadline,
                    MajorId = info?.MajorId,
                };

                // Address
                var addressInfo = new Address
                {
                    Detail = info?.Detail,
                    WardId = info?.WardId,
                    DistrictId = info?.DistrictId,
                    ProvinceId = info?.ProvinceId,
                };

                var job = await _jobRepository.UpdateJobAsync(userId, jobId, fileName, fileData, jobInfo, addressInfo);
                var response = _mapper.Map<UpdateJobForCompanyDTO>(job);

                return new DataResponse<UpdateJobForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Update job successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateJobForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = $"Error update job: {ex.Message}. ",
                    Data = null
                };
            }
        }
    }
}
