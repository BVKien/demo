using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.WebAPI.Controllers.Lecturer
{
    [Authorize(Roles = "Lecturer")]
    [ApiController]
    [Route("api/lecturer/")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/lecturer/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> ViewProfile()
        {
            var dataResponse = await _userService.ViewProfileAsync();

            if (dataResponse.Data == null)
            {
                return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                {
                    Data = null,
                    Message = dataResponse.Message
                });
            }

            return Ok(new ApiResponse<UserProfileDto>
            {
                Data = dataResponse.Data,
                Message = dataResponse.Message
            });
        }

        // PUT: api/lecturer/user/profile
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Data = null,
                    Message = "Invalid data."
                });
            }

            var dataResponse = await _userService.UpdateProfileAsync(dto);

            if (dataResponse.Data == null)
            {
                return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                {
                    Data = null,
                    Message = dataResponse.Message
                });
            }

            return Ok(new ApiResponse<string>
            {
                Data = dataResponse.Data,
                Message = dataResponse.Message
            });
        }
    }
}
