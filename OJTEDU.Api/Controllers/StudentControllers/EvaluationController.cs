using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.EvaluationDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/evaluation")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;
        public EvaluationController(IEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("detail")]
        public async Task<IActionResult> GetEvaluationDetail()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var response = await _evaluationService.GetEvaluationDetailByUserId(userId);

                if (response.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                if (response.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                if (response.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                {
                    Message = response.Message,
                    Data = response.Data
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while get evaluation information for student.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
