using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using static OJTEDU.Application.DTOs.AppllicationDTO;

namespace OJTEDU.Api.Controllers.CompanyControllers
{
    [Route("api/company/application")]
    [ApiController]
    public class AppllicationController : ControllerBase
    {
        private readonly IAppllicationService _appllicationService;

        public AppllicationController(IAppllicationService appllicationService)
        {
            _appllicationService = appllicationService;
        }

        [Authorize(Roles = "Company")]
        [HttpGet("list/{jobId}")]
        public async Task<IActionResult> GetAllApplicationsByJobId(int? jobId)
        {
            try
            {
                var dataResponse = await _appllicationService.GetAllApplicationsByJobIdAsync(jobId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<AppllicationListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<AppllicationListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<AppllicationListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<AppllicationListForCompanyDTO>>
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
                    Message = "An error occurred while get applications list for job.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpGet("detail/{applicationId}")]
        public async Task<IActionResult> GetApplicationDetail(int? applicationId)
        {
            try
            {
                var dataResponse = await _appllicationService.GetApplicationDetailForCompanyAsync(applicationId);

                var apiResponse = new ApiResponse<AppllicationDetailForCompanyDTO>
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
                    Message = "An error occurred while get application detail.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPut("action/{applicationId}")]
        public async Task<IActionResult> StudentApplicationsActions(int? applicationId, string? feedback, DateTime? interviewDate, string? status)
        {
            try
            {
                var dataResponse = await _appllicationService.StudentApplicationsActionsAsync(applicationId, feedback, interviewDate, status);

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
                    Message = "An error occurred while actions to student applications for company.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
