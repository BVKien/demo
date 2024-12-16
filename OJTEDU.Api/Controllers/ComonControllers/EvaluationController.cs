using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using System.Security.Claims;
using static OJTEDU.Api.Input.CommonControllers.EvaluationController;
using static OJTEDU.Application.DTOs.EvaluationDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/common/evaluation")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IJobService _evaluationService;
        public EvaluationController(IJobService evaluationService)
        {
            _evaluationService = evaluationService;
        }

        [Authorize(Roles = "Dean, Lecturer, Mentor")]
        [HttpPut("create")]
        public async Task<IActionResult> CreateEvaluation(CreateEvaluationInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var evaluationInfo = new CreateEvaluationForUniversityCompanyDTO
                {
                    CompanyComment = input?.CompanyComment,
                    CompanyScore = input?.CompanyScore,
                    DeanComment = input?.DeanComment,
                    DeanScore = input?.DeanScore,
                };

                var response = await _evaluationService.CreateEvaluationAsync(userId, input.InternshipId, evaluationInfo);

                if (response.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateEvaluationForUniversityCompanyDTO>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                if (response.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateEvaluationForUniversityCompanyDTO>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                if (response.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateEvaluationForUniversityCompanyDTO>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<CreateEvaluationForUniversityCompanyDTO>
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
                    Message = "An error occurred while create evaluation information for student.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Dean, Lecturer")]
        [HttpGet("detail/{internshipId}")]
        public async Task<IActionResult> GetEvaluationDetail(int? internshipId)
        {
            try
            {
                var response = await _evaluationService.GetEvaluationDetailByInternshipId(internshipId);

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
