//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using OJTEDU.Api.Configuration;
//using OJTEDU.Application.ApplicationServices.Interfaces;
//using static OJTEDU.Application.DTOs.EvaluationDTO;
//using System.Security.Claims;
//using static OJTEDU.Api.Input.MentorControllers.EvaluationController;

//namespace OJTEDU.Api.Controllers.MentorControllers
//{
//    [Route("api/mentor/evaluation")]
//    [ApiController]
//    public class EvaluationController : ControllerBase
//    {
//        private readonly IEvaluationService _evaluationService;
//        public EvaluationController(IEvaluationService evaluationService)
//        {
//            _evaluationService = evaluationService;
//        }

//        [Authorize(Roles = "Mentor")]
//        [HttpPut("create")]
//        public async Task<IActionResult> CreateEvaluation(CreateEvaluationInput? input)
//        {
//            try
//            {
//                var evaluationInfo = new CreateEvaluationForUniversityCompanyDTO
//                {
//                    CompanyComment = input?.CompanyFeedback,
//                    CompanyScore = input?.CompanyScore
//                };

//                var response = await _evaluationService.CreateEvaluationAsync(input.InternshipId, evaluationInfo);

//                if (response.StatusCode == 404)
//                {
//                    return BadRequest(new ApiResponse<CreateEvaluationForUniversityCompanyDTO>
//                    {
//                        Message = response.Message,
//                        Data = null
//                    });
//                }

//                if (response.StatusCode == 400)
//                {
//                    return BadRequest(new ApiResponse<CreateEvaluationForUniversityCompanyDTO>
//                    {
//                        Message = response.Message,
//                        Data = null
//                    });
//                }

//                if (response.StatusCode == 500)
//                {
//                    return StatusCode(500, new ApiResponse<CreateEvaluationForUniversityCompanyDTO>
//                    {
//                        Message = response.Message,
//                        Data = null
//                    });
//                }

//                var apiResponse = new ApiResponse<CreateEvaluationForUniversityCompanyDTO>
//                {
//                    Message = response.Message,
//                    Data = response.Data
//                };

//                return Ok(apiResponse);
//            }
//            catch (Exception ex)
//            {
//                var errorResponse = new ApiResponse<string>
//                {
//                    Message = "An error occurred while create evaluation information for student.",
//                    Data = ex.Message
//                };

//                return StatusCode(500, errorResponse);
//            }
//        }
//    }
//}
