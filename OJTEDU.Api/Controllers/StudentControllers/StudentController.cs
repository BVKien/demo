using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Domain.Entities;
using static OJTEDU.Application.DTOs.StudentDTO;
using static OJTEDU.Api.Input.StudentControllers.StudentController;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OJTEDU.Application.DTOs;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("student-detail")]
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
        [HttpPut("update")]
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
