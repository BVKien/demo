using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.WebAPI.Controllers.Dean
{
    [Authorize(Roles = "Dean")]
    [ApiController]
    [Route("api/dean/working-report")]
    public class WorkingReportController : ControllerBase
    {
        private readonly IWorkingReportService _workService;

        public WorkingReportController(IWorkingReportService workService)
        {
            _workService = workService;
        }

        // GET: api/dean/working-report/{studentId}
        [HttpGet("list/{internshipId}")]
        public async Task<IActionResult> GetWorkingReportsByStudentIdAsync(
        int studentId,
        string? sortBy,
        bool? isDescending,
        string? week,
        int? year)
        {
            try
            {
                var response = await _workService.GetWorkingReportsByStudentIdAsync(studentId, sortBy, isDescending, week, year);

                if (response.StatusCode != 200)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<WorkingReportResponseDTO>
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

        // POST: api/dean/working-report/{studentId}/score
        [HttpPost("update-working-report")]
        public async Task<IActionResult> UpdateWorkingReport([FromBody] GiveFeedbackOrScoreDto dto)
        {
            var result = await _workService.UpdateWorkingReportAsync(dto);

            if (result.Data == null)
            {
                return StatusCode(result.StatusCode, new ApiResponse<object>
                {
                    Data = null,
                    Message = result.Message
                });
            }

            return Ok(new ApiResponse<string>
            {
                Data = result.Data,
                Message = result.Message
            });
        }

        [HttpGet("{internshipId}/weeks")]
        public async Task<IActionResult> GetWeeksForStudentAsync(int studentId, [FromQuery] int? year)
        {
            try
            {
                var response = await _workService.GetWeeksForStudentAsync(studentId, year);

                if (response.StatusCode != 200)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<List<string>>
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
