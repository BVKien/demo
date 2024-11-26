using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using System.Security.Claims;
using static OJTEDU.Api.Input.StudentControllers.FeedbackController;
using static OJTEDU.Application.DTOs.FeedbackDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("list")]
        public async Task<IActionResult> GetFeedbackList()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _feedbackService.GetAllFeedbacksByStudentIdAsync(userId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<FeedbackListForStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<FeedbackListForStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<FeedbackListForStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<FeedbackListForStudentDTO>>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while retrieving feedbacks list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("detail/{feedbackId}")]
        public async Task<IActionResult> GetFeedbackDetail(int? feedbackId)
        {
            try
            {
                var apiResponse = await _feedbackService.GetFeedbackByFeedbackIdAsync(feedbackId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<FeedbackDetailForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<FeedbackDetailForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<FeedbackDetailForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<FeedbackDetailForStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while retrieving feedback detail.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateFeedback(CreateFeedbackInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var feedbackInfo = new CreateFeedbackForStudentDTO
                {
                    FeedbackContent = input?.FeedbackContent,
                };

                var apiResponse = await _feedbackService.CreateFeedbackAsync(userId, feedbackInfo);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateFeedbackForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateFeedbackForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateFeedbackForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<CreateFeedbackForStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while create feedback.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPut("delete-stored")]
        public async Task<IActionResult> DeleteForStoredFeedback(int? feedbackId)
        {
            try
            {
                var apiResponse = await _feedbackService.DeleteForStoredFeedbackAsync(feedbackId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while delete feedback.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
