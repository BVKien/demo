using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.SupportRequestDTO;
using OJTEDU.Domain.Entities;
using static OJTEDU.Api.Input.StudentControllers.SupportRequestController;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/support-request")]
    [ApiController]
    public class SupportRequestController : ControllerBase
    {
        private readonly ISupportRequestService _supportRequestService;
        public SupportRequestController(ISupportRequestService supportRequestService)
        {
            _supportRequestService = supportRequestService;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("list")]
        public async Task<IActionResult> GetSupportRequestList()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _supportRequestService.GetAllSupportRequestByUserIdAsync(userId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<SupportRequestListForStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<SupportRequestListForStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<SupportRequestListForStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<SupportRequestListForStudentDTO>>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while retrieving support request list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("detail/{supportRequestId}")]
        public async Task<IActionResult> GetSupportRequestDetail(int? supportRequestId)
        {
            try
            {
                var apiResponse = await _supportRequestService.GetSupportRequestDetailAsync(supportRequestId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<SupportRequestDetailForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<SupportRequestDetailForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<SupportRequestDetailForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<SupportRequestDetailForStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while retrieving support request detail.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateSupportRequest(CreateSupportRequestInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var supportRequestInfo = new CreateSupportRequestForStudentDTO
                {
                    RequestContent = input?.RequestContent,
                };

                var apiResponse = await _supportRequestService.CreateSupportRequestAsync(userId, supportRequestInfo);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateSupportRequestForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateSupportRequestForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateSupportRequestForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<CreateSupportRequestForStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while create support request.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPut("delete-stored")]
        public async Task<IActionResult> DeleteForStoredSupportRequest(int? supportRequestId)
        {
            try
            {
                var apiResponse = await _supportRequestService.DeleteForStoredSupportRequestAsync(supportRequestId);

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
                    Message = "An error occurred while delete support request.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
