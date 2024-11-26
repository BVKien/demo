using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.WebAPI.Controllers.Dean
{
    [Authorize(Roles = "Dean")]
    [ApiController]
    [Route("api/dean/")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/dean/user/profile
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

        // PUT: api/dean/user/profile
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

        [HttpGet("lecturer-list")]
        public async Task<IActionResult> GetLecturerListForDeanAsync(
        string? name,
        string? userCode,
        string? majorName,
        string? sortBy,
        bool isDescending = false,
        int pageNumber = 1,
        int pageSize = 10)
        {
            var response = await _userService.GetLecturerListForDeanAsync(name, userCode, majorName, sortBy, isDescending, pageNumber, pageSize);

            if (response.Data == null)
            {
                return StatusCode(response.StatusCode, new ApiResponse<object>
                {
                    Data = null,
                    Message = response.Message
                });
            }

            return Ok(new ApiResponse<PagedResponse<List<LecturerListDto>>>
            {
                Data = response.Data,
                Message = response.Message
            });
        }

        [HttpGet("lecturer-detail/{lecturerId}")]
        public async Task<IActionResult> GetLecturerDetailsForDeanAsync(
        int lecturerId,
        string? studentName,
        string? lecturerName,
        string? semesterName,
        string? sortBy,
        bool? isDescending,
        int? pageNumber,
        int? pageSize)
        {
            try
            {
                var actualPageNumber = pageNumber ?? 1;
                var actualPageSize = pageSize ?? 15;

                var response = await _userService.GetLecturerDetailsAsync(
                    lecturerId,
                    studentName,
                    lecturerName,
                    semesterName,
                    sortBy,
                    isDescending,
                    actualPageNumber,
                    actualPageSize);

                if (response.Data == null)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<LecturerDetailsDto>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }


        [HttpPost("create-lecturer")]
        public async Task<IActionResult> CreateLecturer([FromBody] CreateLecturerDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Data = null,
                    Message = "Invalid data."
                });
            }

            var dataResponse = await _userService.CreateLecturerAsync(dto);

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
