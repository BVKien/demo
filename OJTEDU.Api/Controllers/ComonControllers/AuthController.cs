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
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/common/auth")]
    [ApiController]
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
    }
}
