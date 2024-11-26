using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using static OJTEDU.Application.DTOs.WorkingReportDTO;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.InternshipDTO;

namespace OJTEDU.Api.Controllers.MentorControllers
{
    [Route("api/mentor/internship")]
    [ApiController]
    public class InternshipController : ControllerBase
    {
        private readonly IInternshipService _internshipService;
        public InternshipController(IInternshipService internshipService)
        {
            _internshipService = internshipService;
        }

        [Authorize(Roles = "Mentor")]
        [HttpGet("list")]
        public async Task<IActionResult> GetInternshipsList()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _internshipService.GetAllInternshipsByUserIdAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<InternshipListForMentorDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<InternshipListForMentorDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<InternshipListForMentorDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<InternshipListForMentorDTO>>
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
                    Message = "An error occurred while get internship list for mentor.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Mentor")]
        [HttpGet("detail/{internshipId}")]
        public async Task<IActionResult> GetInternshipsDetail(int? internshipId)
        {
            try
            {
                var dataResponse = await _internshipService.GetInternshipDetailAsync(internshipId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<InternshipDetailForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<InternshipDetailForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<InternshipDetailForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<InternshipDetailForMentorDTO>
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
                    Message = "An error occurred while get internship detail for mentor.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
