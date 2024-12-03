using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Domain.Entities;
using static OJTEDU.Application.DTOs.StudentDTO;
using static OJTEDU.Api.Input.StudentControllers.StudentController;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.CommonControllers.AuthenticationController;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly ILogger<StudentController> _logger;
        public StudentController(IStudentService studentService, IUserService userService, ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("LoginWithGoogle-test")]
        public IActionResult LoginWithGoogle()
        {
            // Logic xử lý login
            return Ok();
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
                var dataResponse = await _userService.LoginWithGoogleAsync(request.AuthorizeCode);

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

        [Authorize(Roles = "Student")]
        [HttpGet("student-detail/{userId}")]
        public async Task<IActionResult> GetStudentDetail()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _studentService.GetStudentDetailByUserIdAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<StudentDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<StudentDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<StudentDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<StudentDetailForStudentDTO>
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
                    Message = "An error occurred while get student information.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateStudent([FromBody] UpdateStudentInput? updateInformation)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Transfer from Input to DTO
                var updateStudentDto = new UpdateStudentForStudentDTO
                {
                    Image = updateInformation?.Image,
                    AlternativeEmail = updateInformation?.AlternativeEmail,
                    Phone = updateInformation?.Phone,
                    Dob = updateInformation?.Dob,
                    Gender = updateInformation?.Gender,
                    Detail = updateInformation?.Detail,
                    WardId = updateInformation?.WardId,
                    DistrictId = updateInformation?.DistrictId,
                    ProvinceId = updateInformation?.ProvinceId
                };

                var updatedStudentResponse = await _studentService.UpdateStudentByUserIdAsync(userId, updateStudentDto);

                if (updatedStudentResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<StudentDetailForStudentDTO>
                    {
                        Message = updatedStudentResponse.Message,
                        Data = null
                    });
                }

                if (updatedStudentResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<StudentDetailForStudentDTO>
                    {
                        Message = updatedStudentResponse.Message,
                        Data = null
                    });
                }

                if (updatedStudentResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<StudentDetailForStudentDTO>
                    {
                        Message = updatedStudentResponse.Message,
                        Data = null
                    });
                }

                if (updatedStudentResponse.StatusCode == 200)
                {
                    var updatedStudentDetail = await _studentService.GetStudentDetailByUserIdAsync(userId);

                    return Ok(new ApiResponse<StudentDetailForStudentDTO>
                    {
                        Message = "Student information updated and retrieved successfully!",
                        Data = updatedStudentDetail.Data
                    });
                }

                return StatusCode(updatedStudentResponse.StatusCode, new ApiResponse<UpdateStudentForStudentDTO>
                {
                    Message = updatedStudentResponse.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while updating student information.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
