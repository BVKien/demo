//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using OJTEDU.Api.Configuration;
//using OJTEDU.Api.Input;
//using OJTEDU.Application.ApplicationServices.Interfaces;
//using static OJTEDU.Api.Input.CommonControllers.AuthenticationController;
//using static OJTEDU.Application.DTOs.UserDTO;
//using Microsoft.Extensions.Logging;

//namespace OJTEDU.Api.Controllers.ComonControllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [EnableCors("AllowSpecificOrigin")]
//    public class AuthenticationController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        private readonly ILogger<AuthenticationController> _logger;
//        public AuthenticationController(IUserService userService, ILogger<AuthenticationController> logger)
//        {
//            _userService = userService;
//            _logger = logger;
//        }

//        [HttpPost("login-google")]
//        public async Task<IActionResult> LoginWithGoogle([FromForm] LoginRequest request)
//        {
//            try
//            {
//                // Kiểm tra xem model có hợp lệ hay không
//                if (string.IsNullOrWhiteSpace(request.AuthorizeCode))
//                {
//                    return BadRequest(new ApiResponse<object>
//                    {
//                        Data = null,
//                        Message = "AuthorizeCode is required."
//                    });
//                }

//                _logger.LogInformation("Starting Google login process for AuthorizeCode: {AuthorizeCode}", request.AuthorizeCode);

//                var dataResponse = await _userService.LoginWithGoogleAsync(request.AuthorizeCode);


//                if (dataResponse == null)
//                {
//                    return StatusCode(500, new ApiResponse<object>
//                    {
//                        Data = null,
//                        Message = "Unexpected error occurred."
//                    });
//                }

//                if (dataResponse.Data == null)
//                {
//                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
//                    {
//                        Data = null,
//                        Message = dataResponse.Message
//                    });
//                }

//                var apiResponse = new ApiResponse<UserReadForAuthDTO>()
//                {
//                    Data = dataResponse.Data,
//                    Message = dataResponse.Message
//                };

//                return Ok(apiResponse);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<object>
//                {
//                    Data = null,
//                    Message = $"Internal Server Error: {ex.Message}"
//                });
//            }
//        }

//        [HttpGet("check-auth")]
//        public async Task<IActionResult> CheckAuthentication()
//        {
//            if (User.Identity?.IsAuthenticated == true)
//            {
//                var dataResponse = await _userService.GetAuthenticatedUserInfoAsync(User);

//                if (dataResponse.StatusCode == 200 && dataResponse.Data != null)
//                {
//                    // Trả về thông tin chi tiết nếu tài khoản tồn tại và hợp lệ
//                    return Ok(new ApiResponse<UserReadForAuthDTO>
//                    {
//                        Data = dataResponse.Data,
//                        Message = "User is authenticated."
//                    });
//                }

//                return Unauthorized(new ApiResponse<object>
//                {
//                    Data = null,
//                    Message = "User is not authenticated."
//                });
//            }
//            else
//            {
//                return Unauthorized(new ApiResponse<object>
//                {
//                    Data = null,
//                    Message = "User is not authenticated."
//                });
//            }
//        }


//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout()
//        {
//            try
//            {
//                var response = await _userService.LogoutAsync();
//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//    }
//}

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
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUserService userService, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromForm] LoginRequest request)
        {
            _logger.LogInformation("Received request to login with Google. AuthorizeCode: {AuthorizeCode}", request.AuthorizeCode);

            try
            {
                if (string.IsNullOrWhiteSpace(request.AuthorizeCode))
                {
                    _logger.LogWarning("Login failed: AuthorizeCode is missing.");
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "AuthorizeCode is required."
                    });
                }

                var dataResponse = await _userService.LoginWithGoogleAsync(request.AuthorizeCode);

                if (dataResponse == null)
                {
                    _logger.LogError("Login failed: Unexpected error occurred during Google login process.");
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    _logger.LogWarning("Login failed: {Message}", dataResponse.Message);
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                _logger.LogInformation("Login successful for AuthorizeCode: {AuthorizeCode}", request.AuthorizeCode);

                return Ok(new ApiResponse<UserReadForAuthDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred during Google login.");
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
            _logger.LogInformation("Received request to check authentication.");

            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInformation("User is authenticated. Fetching user info.");
                    var dataResponse = await _userService.GetAuthenticatedUserInfoAsync(User);

                    if (dataResponse.StatusCode == 200 && dataResponse.Data != null)
                    {
                        _logger.LogInformation("User authenticated successfully.");
                        return Ok(new ApiResponse<UserReadForAuthDTO>
                        {
                            Data = dataResponse.Data,
                            Message = "User is authenticated."
                        });
                    }

                    _logger.LogWarning("User is authenticated but user info could not be retrieved.");
                    return Unauthorized(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "User is not authenticated."
                    });
                }
                else
                {
                    _logger.LogWarning("User is not authenticated.");
                    return Unauthorized(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "User is not authenticated."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred during authentication check.");
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
            _logger.LogInformation("Received request to log out.");

            try
            {
                var response = await _userService.LogoutAsync();
                _logger.LogInformation("Logout successful.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred during logout.");
                return BadRequest(new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Logout failed: {ex.Message}"
                });
            }
        }
    }
}

