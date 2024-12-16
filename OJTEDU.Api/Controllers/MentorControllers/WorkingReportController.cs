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
        public async Task<IActionResult> GetAllWorkingReportsByStudentIdAsync(
    int studentId,
    string? sortBy = null,
    bool? isDescending = null,
    string? week = null,
    int? year = null)
        {
            try
            {
                // Gọi service để lấy danh sách báo cáo
                var response = await _workingReportService.GetAllWorkingReportsByStudentIdAsync(studentId, sortBy, isDescending, week, year);

                // Kiểm tra mã trạng thái từ service và trả về phản hồi phù hợp
                if (response.StatusCode != 200)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                // Trả về phản hồi thành công với DTO đã map
                return Ok(new ApiResponse<WorkingReportResponseDTO>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống và trả về phản hồi 500
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("{internshipId}/weeks")]
        public async Task<IActionResult> GetWeeksForStudentAsync(int internshipId, [FromQuery] int? year)
        {
            try
            {
                var response = await _workingReportService.GetWeeksForStudentAsync(internshipId, year);

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