using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.StudentDTO;

namespace OJTEDU.WebAPI.Controllers.Lecturer
{
    [Authorize(Roles = "Lecturer")]
    [ApiController]
    [Route("api/lecturer/student")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _stuService;

        public StudentController(IStudentService stuService)
        {
            _stuService = stuService;
        }

        // GET: api/lecturer/student/list
        [HttpGet("list")]
        public async Task<IActionResult> GetStudentListForLecturerAsync(
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

                var response = await _stuService.GetStudentListAsync(
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

        // GET: api/lecturer/student/{studentId}
        [HttpGet("detail/{studentId}")]
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


    }
}
