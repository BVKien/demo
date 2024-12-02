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
