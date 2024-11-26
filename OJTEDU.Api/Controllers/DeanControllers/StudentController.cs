using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.StudentDTO;

namespace OJTEDU.WebAPI.Controllers.Dean
{
    [Authorize(Roles = "Dean")]
    [ApiController]
    [Route("api/dean/student")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _stuService;

        public StudentController(IStudentService stuService)
        {
            _stuService = stuService;
        }

        // GET: api/dean/student/list
        [HttpGet("list")]
        public async Task<IActionResult> GetStudentList([FromQuery] string? studentName, [FromQuery] string? lecturerName, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;
                var dataResponse = await _stuService.GetStudentListAsync(studentName, lecturerName, actualPageNumber, actualPageSize);

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }
                return Ok(new ApiResponse<PagedResponse<List<StudentListDto>>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                });
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


        [HttpGet("ojt/list")]
        public async Task<IActionResult> GetOjtStudentList()
        {
            try
            {
                var dataResponse = await _stuService.GetOjtStudentListAsync();

                return Ok(new ApiResponse<List<StudentListDto>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }

        }

        // GET: api/dean/student/{studentId}
        [HttpGet("student-detail/{studentId}")]
        public async Task<IActionResult> GetStudentDetails(int studentId)
        {
            var dataResponse = await _stuService.GetStudentDetailsAsync(studentId);

            if (dataResponse.Data == null)
            {
                return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                {
                    Data = null,
                    Message = dataResponse.Message
                });
            }

            return Ok(new ApiResponse<StudentDetailsDto>
            {
                Data = dataResponse.Data,
                Message = dataResponse.Message
            });
        }

        // POST: api/dean/student/assign-lecturer
        [HttpPost("assign-lecturer")]
        public async Task<IActionResult> AssignLecturer([FromBody] AssignLecturerForStudentDto dto)
        {
            if (dto == null || dto.StudentIds == null || dto.StudentIds.Count == 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Data = null,
                    Message = "Invalid data."
                });
            }

            var dataResponse = await _stuService.AssignLecturerForStudentsAsync(dto);

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
