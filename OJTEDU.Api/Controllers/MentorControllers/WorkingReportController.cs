using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using static OJTEDU.Application.DTOs.WorkingReportDTO;
using System.Security.Claims;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.MentorControllers.WorkingReportController;
using static OJTEDU.Application.DTOs.InternshipDTO;

namespace OJTEDU.Api.Controllers.MentorControllers
{
    [Route("api/mentor/working-report")]
    [ApiController]
    public class WorkingReportController : ControllerBase
    {
        private readonly IWorkingReportService _workingReportService;
        public WorkingReportController(IWorkingReportService workingReportService)
        {
            _workingReportService = workingReportService;
        }

        [Authorize(Roles = "Mentor")]
        [HttpGet("list/{studentId}")]
        public async Task<IActionResult> GetAllInternshipWorkingReportsList(int? studentId)
        {
            try
            {
                var dataResponse = await _workingReportService.GetAllWorkingReportsByStudentId(studentId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<WorkingReportListForMentorDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<WorkingReportListForMentorDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<WorkingReportListForMentorDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<WorkingReportListForMentorDTO>>
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
                    Message = "An error occurred while get working report list for internship.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Mentor")]
        [HttpGet("detail/{workingReportId}")]
        public async Task<IActionResult> GetWorkingReportDetail(int? workingReportId)
        {
            try
            {
                var dataResponse = await _workingReportService.GetWorkingReportDetailForMentorAsync(workingReportId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<WorkingReportDetailForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<WorkingReportDetailForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<WorkingReportDetailForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<WorkingReportDetailForMentorDTO>
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
                    Message = "An error occurred while get working report list detail for internship.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Mentor")]
        [HttpPut("feedback")]
        public async Task<IActionResult> FeedbackWorkingReport(int? workingReportId, FeedbackWorkingReportInput? input)
        {
            try
            {
                var feedbackdto = new CreateFeedbackWorkingReportForMentorDTO
                {
                    FeedbackFromMentor = input?.FeedbackFromMentor,
                    MentorScore = input?.MentorScore,
                };

                var dataResponse = await _workingReportService.CreateMentorFeedbackAsync(workingReportId, feedbackdto);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateFeedbackWorkingReportForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateFeedbackWorkingReportForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateFeedbackWorkingReportForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<CreateFeedbackWorkingReportForMentorDTO>
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
                    Message = "An error occurred while feedback working report for internship.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}