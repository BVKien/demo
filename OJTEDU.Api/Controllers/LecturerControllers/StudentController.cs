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
        public async Task<IActionResult> GetStudentList([FromQuery] string? studentName,int? pageNumber,int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;            
                var dataResponse = await _stuService.GetStudentListAsync(studentName, null, actualPageNumber, actualPageSize);

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
