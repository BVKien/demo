using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.AttendanceReportDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/attendance-report")]
    [ApiController]
    public class AttendanceReportController : ControllerBase
    {
        private readonly IAttendanceReportService _attendanceReportService;
        private readonly IHttpClientFactory _httpClientFactory;
        public AttendanceReportController(IAttendanceReportService attendanceReportService, IHttpClientFactory httpClientFactory)
        {
            _attendanceReportService = attendanceReportService;
            _httpClientFactory = httpClientFactory;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAttendanceReportsListForInternship()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _attendanceReportService.GetAllAttendanceReportsForStudentAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<AttendanceReportsListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<AttendanceReportsListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<AttendanceReportsListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<AttendanceReportsListForStudentDTO>>
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
                    Message = "An error occurred while get attendance report list for internship.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
