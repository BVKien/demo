using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AttendanceReportDTO;

namespace OJTEDU.WebAPI.Controllers.Dean
{
    [Authorize(Roles = "Dean")]
    [ApiController]
    [Route("api/dean/attendance-report")]
    public class AttendanceReportController : ControllerBase
    {
        private readonly IAttendanceReportService _attendService;

        public AttendanceReportController(IAttendanceReportService attendService)
        {
            _attendService = attendService;
        }

        // GET: api/dean/attendance-report/{studentId}
        [HttpGet("detai/{studentId}")]
        public async Task<IActionResult> GetAttendanceReports(int studentId, int? pageNumber , int? pageSize )
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;
                var dataResponse = await _attendService.GetAttendanceReportsByStudentIdAsync(studentId, actualPageNumber, actualPageSize);

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
    }
}
