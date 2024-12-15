using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using static OJTEDU.Application.DTOs.StudentDTO;

namespace OJTEDU.Api.Controllers.DOETControllers
{
    [Authorize(Roles = "DOET")]
    [ApiController]
    [Route("api/doet/student")]
    public class StudentController : ControllerBase
    {
        private readonly IJobService _jobService;

        public StudentController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetStudentListForDeanAsync(
        string? code,
        string? studentName,
        string? lecturerName,
        string? majorName,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        bool? isDescending)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var response = await _jobService.GetStudentListAsync(
                    code,
                    studentName,
                    lecturerName,
                    majorName,
                    actualPageNumber,
                    actualPageSize,
                    sortBy,
                    isDescending
                );

                if (response.Data == null)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<PagedResponse<List<StudentListDto>>>
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
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("detail/{studentId}")]
        public async Task<IActionResult> GetStudentDetails(int studentId)
        {
            var dataResponse = await _jobService.GetStudentDetailsAsync(studentId);

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
        [HttpPut("update/{studentId}")]
        public async Task<IActionResult> UpdateStudentAsync(int studentId, [FromBody] UpdateStudentDto dto)
        {
            try
            {
                var response = await _jobService.UpdateStudentAsync(studentId, dto);

                if (response.StatusCode != 200)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<string>
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
    }
}
