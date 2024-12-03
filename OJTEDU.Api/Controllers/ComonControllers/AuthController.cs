using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OJTEDU.Api.Configuration;
using OJTEDU.Api.Controllers.StudentControllers;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System.Security.Claims;
using static OJTEDU.Api.Input.CommonControllers.AuthenticationController;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/common/auth")]
    [ApiController]
    //[EnableCors("AllowSpecificOrigin")]
    public class AuthController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly ILogger<CvController> _logger;

        public AuthController(IJobService jobService, ILogger<CvController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginWithGoogle([FromForm] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("LoginWithGoogle method started.");
                var dataResponse = await _jobService.LoginWithGoogleAsync(request.AuthorizeCode);
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

        [HttpGet("check")]
        public async Task<IActionResult> CheckAuthentication()
        {
            try
            {
                _logger.LogInformation("CheckAuthentication method started.");

                if (User.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInformation("User is authenticated.");

                    var dataResponse = await _jobService.GetAuthenticatedUserInfoAsync(User);

                    if (dataResponse.StatusCode == 200 && dataResponse.Data != null)
                    {
                        _logger.LogInformation("User details fetched successfully.");
                        return Ok(new ApiResponse<UserReadForAuthDTO>
                        {
                            Data = dataResponse.Data,
                            Message = "User is authenticated."
                        });
                    }

                    _logger.LogWarning("User authentication failed. Returning Unauthorized.");
                    return Unauthorized(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "User is not authenticated."
                    });
                }
                else
                {
                    _logger.LogWarning("User is not authenticated. Returning Unauthorized.");
                    return Unauthorized(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "User is not authenticated."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in CheckAuthentication method: {ex.Message}");
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                _logger.LogInformation("Logout method started.");
                var response = await _jobService.LogoutAsync();
                _logger.LogInformation("Logout successful.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in Logout method: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("list-test")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersForAdmin(string? name, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _jobService.GetAllUsersForAdminAsync(name, roleId, status, actualPageNumber, actualPageSize);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Successful retrieval of users
                var apiResponse = new ApiResponse<PagedResponse<List<UserListForAdminDTO>>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }
    }
}
