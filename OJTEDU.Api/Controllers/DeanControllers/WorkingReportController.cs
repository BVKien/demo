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
        [HttpGet("detail/{studentId}")]
        public async Task<IActionResult> GetWorkingReportsByStudentIdForDean(
        int studentId,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        bool? isDescending)
        {
            try
            {
                var dataResponse = await _workService.GetWorkingReportsByStudentIdAsync(
                    studentId,
                    pageNumber ?? 1,
                    pageSize ?? 15,
                    sortBy,
                    isDescending
                );

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                return Ok(new ApiResponse<WorkingReportResponseDTO>
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
    }
}
