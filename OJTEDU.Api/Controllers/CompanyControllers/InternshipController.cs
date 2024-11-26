using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using static OJTEDU.Application.DTOs.InternshipDTO;
using System.Security.Claims;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.CompanyControllers.InternshipController;

namespace OJTEDU.Api.Controllers.CompanyControllers
{
    [Route("api/company/internship")]
    [ApiController]
    public class InternshipController : ControllerBase
    {
        private readonly IInternshipService _internshipService;
        public InternshipController(IInternshipService internshipService)
        {
            _internshipService = internshipService;
        }

        [Authorize(Roles = "Company")]
        [HttpGet("list")]
        public async Task<IActionResult> GetInternshipsList()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _internshipService.GetAllInternshipsByUserIdForCompanyAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<InternshipListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<InternshipListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<InternshipListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<InternshipListForCompanyDTO>>
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
                    Message = "An error occurred while get internship list for company.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPut("assign-to-mentor")]
        public async Task<IActionResult> AssignInternshipsToTheMentor(int? mentorId, [FromBody] AssignInternshipsToTheMentorInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _internshipService.AssignInternshipsForMentorAsync(userId, mentorId, input?.InternshipIds);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = dataResponse.Message,
                        Data = false
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = dataResponse.Message,
                        Data = false
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Message = dataResponse.Message,
                        Data = false
                    });
                }

                var apiResponse = new ApiResponse<bool>
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
                    Message = "An error occurred while assignning internships list to the mentor.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
