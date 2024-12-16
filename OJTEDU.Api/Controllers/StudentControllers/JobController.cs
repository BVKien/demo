using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJTEDU.Api.Configuration;
using OJTEDU.Api.Controllers.StudentControllers;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Domain.Entities;
using System.Net.Http.Headers;
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

                var combinedJobs = suggestedJobs
                    .Concat(dbJobs)
                    .DistinctBy(j => j.JobId)
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

        private async Task<IEnumerable<JobListSearchForStudentDTO>> GetSuggestedJobsFromAiApiAsync(string? fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                _logger.LogError("File URL is null or empty.");
                return Enumerable.Empty<JobListSearchForStudentDTO>();
            }

            var client = _httpClientFactory.CreateClient();
            var request = new MultipartFormDataContent();

            try
            {
                // Download the file from the provided URL
                var fileBytes = await client.GetByteArrayAsync(fileUrl);
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    _logger.LogError("Failed to download file or file is empty.");
                    return Enumerable.Empty<JobListSearchForStudentDTO>();
                }

                // Create a file stream content from the downloaded file
                var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath) ?? "uploaded_file";
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                request.Add(fileContent, "file", fileName);

                _logger.LogInformation($"File downloaded and prepared for upload: {fileName}");

                // Send POST request to the Flask API
                var response = await client.PostAsync("http://127.0.0.1:5001/api/ai/analyze", request); //https://sep490-g62-ojtedu-be-ai-7.onrender.com/

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Deserialize the JSON response from the Flask API
                var jobSuggestions = await response.Content.ReadFromJsonAsync<IEnumerable<JobListSearchForStudentDTO>>();

                if (jobSuggestions == null || !jobSuggestions.Any())
                {
                    _logger.LogWarning("No job suggestions returned from AI API.");
                    return Enumerable.Empty<JobListSearchForStudentDTO>();
                }

                return jobSuggestions;
            }
            catch (HttpRequestException e)
            {
                // Log the HTTP request error
                _logger.LogError($"Request error: {e.Message}");
                return Enumerable.Empty<JobListSearchForStudentDTO>();
            }
            catch (Exception e)
            {
                // Log other errors
                _logger.LogError($"An unexpected error occurred: {e.Message}");
                return Enumerable.Empty<JobListSearchForStudentDTO>();
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
