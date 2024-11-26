using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Security.Claims;
using static OJTEDU.Api.Input.CompanyControllers.JobController;
using static OJTEDU.Application.DTOs.JobDTO;

namespace OJTEDU.Api.Controllers.CompanyControllers
{
    [Route("api/company/job")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [Authorize(Roles = "Company")]
        [HttpGet("list")]
        public async Task<IActionResult> GetJobList()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _jobService.GetAllJobsByUserIdAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<JobListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<JobListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<JobListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<JobListForCompanyDTO>>
                {
                    Message = dataResponse.Message,
                    Data = dataResponse.Data
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while get jobs list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpGet("detail/{jobId}")]
        public async Task<IActionResult> GetJobDetail(int? jobId)
        {
            try
            {
                var dataResponse = await _jobService.GetJobDetailForCompanyAsync(jobId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<JobDetailForCompanyDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<JobDetailForCompanyDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<JobDetailForCompanyDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<JobDetailForCompanyDTO>
                {
                    Message = dataResponse.Message,
                    Data = dataResponse.Data
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while get job detail.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPost("files/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/jobs/files/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                return Ok(new
                {
                    Data = file.FileName
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>
                {
                    Message = $"An error occurred while uploading file: {ex.Message}. ",
                    Data = new { Details = ex.Message }
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var filePath = Path.Combine("wwwroot/uploads/jobs/files/", input.TestFile);

                // Initialize 
                byte[]? fileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.TestFile) && System.IO.File.Exists(filePath))
                {
                    fileData = await System.IO.File.ReadAllBytesAsync(filePath);
                }

                var jobInfoDto = new CreateJobForCompanyDTO
                {
                    Title = input?.Title,
                    Description = input?.Description,
                    SalaryRange = input?.SalaryRange,
                    Requirements = input?.Requirements,
                    SkillRequirements = input?.SkillRequirements,
                    Benefits = input?.Benefits,
                    WorkingHours = input?.WorkingHours,
                    Deadline = input?.Deadline,
                    MajorId = input?.MajorId,
                    Addressed = input?.Addressed,
                    Detail = input?.AddressDetail,
                    WardId = input?.WardId,
                    DistrictId = input?.DistrictId,
                    ProvinceId = input?.ProvinceId,
                };

                var apiResponse = await _jobService.CreateJobAsync(userId, input?.TestFile, fileData, jobInfoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateJobForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateJobForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateJobForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 200)
                {
                    if (fileData != null && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                return Ok(new ApiResponse<CreateJobForCompanyDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while create job.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPut("update/{jobId}")]
        public async Task<IActionResult> UpdateJob(int? jobId, [FromBody] UpdateJobInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var filePath = Path.Combine("wwwroot/uploads/jobs/files/", input.TestFile);

                // Initialize 
                byte[]? fileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.TestFile) && System.IO.File.Exists(filePath))
                {
                    fileData = await System.IO.File.ReadAllBytesAsync(filePath);
                }

                var jobInfoDto = new UpdateJobForCompanyDTO
                {
                    Title = input?.Title,
                    Description = input?.Description,
                    SalaryRange = input?.SalaryRange,
                    Requirements = input?.Requirements,
                    SkillRequirements = input?.SkillRequirements,
                    Benefits = input?.Benefits,
                    WorkingHours = input?.WorkingHours,
                    Deadline = input?.Deadline,
                    MajorId = input?.MajorId,
                    Detail = input?.AddressDetail,
                    WardId = input?.WardId,
                    DistrictId = input?.DistrictId,
                    ProvinceId = input?.ProvinceId,
                };

                var apiResponse = await _jobService.UpdateJobAsync(userId, jobId, input?.TestFile, fileData, jobInfoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<UpdateJobForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<UpdateJobForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<UpdateJobForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 200)
                {
                    if (fileData != null && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                return Ok(new ApiResponse<UpdateJobForCompanyDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while update job.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
