using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJTEDU.Api.Configuration;
using OJTEDU.Api.Controllers.StudentControllers;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Domain.Entities;
using System.Security.Claims;
using static OJTEDU.Api.Input.CommonControllers.AuthenticationController;
using static OJTEDU.Application.DTOs.JobDTO;
using static OJTEDU.Application.DTOs.UserDTO;

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

        private readonly IUserService _userService;

        public JobController(IJobService jobService, ICvService cvService, IHttpClientFactory httpClientFactory, ILogger<CvController> logger, IUserService userService)
        {
            _jobService = jobService;
            _cvService = cvService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("testok")]
        public async Task<IActionResult> LoginWithGoogle([FromForm] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("LoginWithGoogle method started.");
                var dataResponse = await _userService.LoginWithGoogleAsync(request.AuthorizeCode);
                _logger.LogInformation("Login successful with Google.");
                var apiResponse = new ApiResponse<UserReadForAuthDTO>()
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in LoginWithGoogle method: {ex.Message}");
                return StatusCode(500, new ApiResponse<string>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
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

        //[Authorize(Roles = "Student")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchJobs(string? title, int? majorId, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                pageSize = pageSize ?? 15;

                //int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                int userId = 7;

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

                var suggestedJobs = await GetSuggestedJobsFromAiApiAsync("wwwroot" + cvFilePathResponse.Data);

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

        private async Task<IEnumerable<JobListSearchForStudentDTO>> GetSuggestedJobsFromAiApiAsync(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _logger.LogError("File path is null or empty.");
                return Enumerable.Empty<JobListSearchForStudentDTO>();
            }

            var client = _httpClientFactory.CreateClient();
            var request = new MultipartFormDataContent();

            try
            {
                // Open the file stream
                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    // Add the file content to the request
                    request.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

                    // Send the POST request to the Flask API
                    var response = await client.PostAsync("http://127.0.0.1:5001/api/ai/analyze", request);

                    // Ensure success status code or throw exception
                    response.EnsureSuccessStatusCode();

                    // Deserialize the JSON response into the desired DTO
                    var jobSuggestions = await response.Content.ReadFromJsonAsync<IEnumerable<JobListSearchForStudentDTO>>();

                    if (jobSuggestions == null || !jobSuggestions.Any())
                    {
                        _logger.LogWarning("No job suggestions returned from AI API.");
                        return Enumerable.Empty<JobListSearchForStudentDTO>();
                    }

                    return jobSuggestions;
                }
            }
            catch (HttpRequestException e)
            {
                // Log error and return empty collection if request fails
                _logger.LogError($"Request error: {e.Message}");
                return Enumerable.Empty<JobListSearchForStudentDTO>();
            }
            catch (Exception e)
            {
                // Log any other unexpected errors
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
