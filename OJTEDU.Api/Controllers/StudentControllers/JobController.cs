using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJTEDU.Api.Configuration;
using OJTEDU.Api.Controllers.StudentControllers;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Domain.Entities;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.JobDTO;

namespace OJTEDU.Api.Controllers.GuestControllers
{
    [Route("api/student/job")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly ICvService _cvService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CvController> _logger;

        public JobController(IJobService jobService, ICvService cvService, IHttpClientFactory httpClientFactory, ILogger<CvController> logger)
        {
            _jobService = jobService;
            _cvService = cvService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("list/{companyId}")]
        public async Task<IActionResult> GetJobListByCompanyId(int? companyId)
        {
            try
            {
                var dataResponse = await _jobService.GetAllJobsByCompanyIdAsync(companyId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<JobListByCompanyIdForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<JobListByCompanyIdForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<JobListByCompanyIdForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<JobListByCompanyIdForStudentDTO>>
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
                    Message = $"An error occurred while get job list by company.",
                    Data = ex.Message 
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchJobs(string? title, int? majorId, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                pageSize = pageSize ?? 15;

                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _jobService.SearchJobsAsync(userId, title, majorId, provinceId, districtId, wardId, pageNumber, pageSize);
                var dbJobs = dataResponse.Data;

                var cvFilePathResponse = await _cvService.GetPrimaryCvFilePathAsync(userId);

                if (cvFilePathResponse.StatusCode != 200)
                {
                    var apiResponseNoneAi = new ApiResponseTotalPaged<List<JobListSearchForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        TotalPageCount = dataResponse.TotalPages,
                        Data = dataResponse.Data
                    };

                    return Ok(apiResponseNoneAi);
                }

                var suggestedJobs = await GetSuggestedJobsFromAiApiAsync(cvFilePathResponse.Data);

                var combinedJobs = dbJobs.Concat(suggestedJobs)
                    .DistinctBy(j => j.JobId) // Remove duplicate work
                    .ToList();

                var apiResponse = new ApiResponseTotalPaged<List<JobListSearchForStudentDTO>>
                {
                    Message = dataResponse.Message,
                    TotalPageCount = dataResponse.TotalPages,
                    Data = combinedJobs
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while search jobs.",
                    Data = ex.Message,
                };

                return StatusCode(500, errorResponse);
            }
        }

        private async Task<IEnumerable<JobListSearchForStudentDTO>> GetSuggestedJobsFromAiApiAsync(string? filePath)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new MultipartFormDataContent();

            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                request.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

                try
                {
                    var response = await client.PostAsync("http://127.0.0.1:5001/api/ai/analyze", request);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadFromJsonAsync<IEnumerable<JobListSearchForStudentDTO>>();
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError($"Request error: {e.Message}");
                    return Enumerable.Empty<JobListSearchForStudentDTO>(); // Return null or handle error 
                }
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetJobList()
        {
            try
            {
                var dataResponse = await _jobService.GetAllJobsAsync();

                var apiResponse = new ApiResponse<List<JobListForStudentDTO>>
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
                    Message = "An error occurred while get job list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("detail/{jobId}")]
        public async Task<IActionResult> GetJobDetail(int? jobId)
        {
            try
            {
                var dataResponse = await _jobService.GetJobDetailAsync(jobId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<JobDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<JobDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<JobDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<JobDetailForStudentDTO>
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
    }
}
