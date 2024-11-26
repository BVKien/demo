using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AttendanceReportDTO;

namespace OJTEDU.WebAPI.Controllers.Lecturer
{
    [ApiController]
    [Route("api/lecturer/attendance-report")]
    public class AttendanceReportController : ControllerBase
    {
        private readonly IAttendanceReportService _attendanceReportService;
        private readonly IHttpClientFactory _httpClientFactory;
        public AttendanceReportController(IAttendanceReportService attendanceReportService, IHttpClientFactory httpClientFactory)
        {
            _attendanceReportService = attendanceReportService;
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/lecturer/attendance-report/{studentId}
        [Authorize(Roles = "Lecturer")]
        [HttpGet("detail/{studentId}")]
        public async Task<IActionResult> GetAttendanceReports(int studentId, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;
                var dataResponse = await _attendanceReportService.GetAttendanceReportsByStudentIdAsync(studentId, actualPageNumber, actualPageSize);

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                return Ok(new ApiResponse<PagedResponse<List<AttendanceReportDto>>>
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

        [Authorize(Roles = "Lecturer")]
        [HttpGet("list/{internshipId}")]
        public async Task<IActionResult> GetAttendanceReportsListForInternship(int? internshipId)
        {
            try
            {
                var dataResponse = await _attendanceReportService.GetAllAttendanceReportsByInternshipIdAsync(internshipId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
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

        [Authorize(Roles = "Lecturer")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAttendanceReportsList()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _attendanceReportService.GetAllAttendanceReportsForLecturerAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
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
                    Message = "An error occurred while get attendance report list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
