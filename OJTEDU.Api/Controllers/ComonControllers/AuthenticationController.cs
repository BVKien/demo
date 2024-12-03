using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Api.Input;
using OJTEDU.Application.ApplicationServices.Interfaces;
using static OJTEDU.Api.Input.CommonControllers.AuthenticationController;
using static OJTEDU.Application.DTOs.UserDTO;
using Microsoft.Extensions.Logging;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [ApiController]
    [Route("api/common/authentication")]
    [EnableCors]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticationController> _logger;
        private IUserService @object;
        private readonly IJobService _jobService;

        public AuthenticationController(IUserService userService, ILogger<AuthenticationController> logger, IJobService jobService)
        {
            _userService = userService;
            _logger = logger;
            _jobService = jobService;
        }

        public AuthenticationController(IUserService @object)
        {
            this.@object = @object;
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromForm] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("LoginWithGoogle method started.");

                // Kiểm tra xem model có hợp lệ hay không
                if (string.IsNullOrWhiteSpace(request.AuthorizeCode))
                {
                    _logger.LogWarning("AuthorizeCode is missing or empty.");
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "AuthorizeCode is required."
                    });
                }

                _logger.LogInformation("Processing Google login with the provided authorize code.");
                var dataResponse = await _jobService.LoginWithGoogleAsync(request.AuthorizeCode);

                if (dataResponse == null)
                {
                    _logger.LogError("Unexpected error occurred while logging in with Google.");
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    _logger.LogWarning($"Login failed: {dataResponse.Message}");
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

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
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("check-auth")]
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
    }
}



/*
 using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Api.Input;
using OJTEDU.Application.ApplicationServices.Interfaces;
using static OJTEDU.Api.Input.CommonControllers.AuthenticationController;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("AllowSpecificOrigin")]
    [EnableCors]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromForm] LoginRequest request)
        {
            try
            {
                // Kiểm tra xem model có hợp lệ hay không
                if (string.IsNullOrWhiteSpace(request.AuthorizeCode))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "AuthorizeCode is required."
                    });
                }

                var dataResponse = await _userService.LoginWithGoogleAsync(request.AuthorizeCode);


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

                var apiResponse = new ApiResponse<UserReadForAuthDTO>()
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

        [HttpGet("check-auth")]
        public async Task<IActionResult> CheckAuthentication()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var dataResponse = await _userService.GetAuthenticatedUserInfoAsync(User);

                if (dataResponse.StatusCode == 200 && dataResponse.Data != null)
                {
                    // Trả về thông tin chi tiết nếu tài khoản tồn tại và hợp lệ
                    return Ok(new ApiResponse<UserReadForAuthDTO>
                    {
                        Data = dataResponse.Data,
                        Message = "User is authenticated."
                    });
                }

                return Unauthorized(new ApiResponse<object>
                {
                    Data = null,
                    Message = "User is not authenticated."
                });
            }
            else
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Data = null,
                    Message = "User is not authenticated."
                });
            }
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var response = await _userService.LogoutAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
 */