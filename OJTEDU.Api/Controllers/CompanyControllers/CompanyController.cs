using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using static OJTEDU.Application.DTOs.InternshipDTO;
using System.Security.Claims;
using OJTEDU.Application.ApplicationServices.Interfaces;
using static OJTEDU.Application.DTOs.CompanyDTO;
using OJTEDU.Application.ApplicationServices.Services;
using static OJTEDU.Api.Input.StudentControllers.StudentController;
using static OJTEDU.Application.DTOs.StudentDTO;
using static OJTEDU.Api.Input.CompanyControllers.CompanyController;

namespace OJTEDU.Api.Controllers.CompanyControllers
{
    [Route("api/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [Authorize(Roles = "Company")]
        [HttpGet("mentor/list")]
        public async Task<IActionResult> GetInternshipsList()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _companyService.GetMentorsListAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<MentorListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<MentorListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<MentorListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<MentorListForCompanyDTO>>
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
                    Message = "An error occurred while get mentors list for company.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyInput? updateInformation)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Transfer from Input to DTO
                var updateStudentDto = new UpdateCompanyForCompanyDTO
                {
                    Image = updateInformation?.Image,
                    AlternativeEmail = updateInformation?.AlternativeEmail,
                    Phone = updateInformation?.Phone,
                    TaxCode = updateInformation?.TaxCode,
                    Website = updateInformation?.Website,
                    Description = updateInformation?.Description,
                    Dob = updateInformation?.Dob,
                    Gender = updateInformation?.Gender,
                    Detail = updateInformation?.Detail,
                    WardId = updateInformation?.WardId,
                    DistrictId = updateInformation?.DistrictId,
                    ProvinceId = updateInformation?.ProvinceId
                };

                var updatedStudentResponse = await _companyService.UpdateCompanyByUserIdAsync(userId, updateStudentDto);

                if (updatedStudentResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<UpdateCompanyForCompanyDTO>
                    {
                        Message = updatedStudentResponse.Message,
                        Data = null
                    });
                }

                if (updatedStudentResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<UpdateCompanyForCompanyDTO>
                    {
                        Message = updatedStudentResponse.Message,
                        Data = null
                    });
                }

                if (updatedStudentResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<UpdateCompanyForCompanyDTO>
                    {
                        Message = updatedStudentResponse.Message,
                        Data = null
                    });
                }

                if (updatedStudentResponse.StatusCode == 200)
                {

                    return Ok(new ApiResponse<UpdateCompanyForCompanyDTO>
                    {
                        Message = "Company information updated and retrieved successfully!",
                        Data = updatedStudentResponse.Data
                    });
                }

                return StatusCode(updatedStudentResponse.StatusCode, new ApiResponse<UpdateCompanyForCompanyDTO>
                {
                    Message = updatedStudentResponse.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while updating company information.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpGet("company-detail")]
        public async Task<IActionResult> GetStudentCompany()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _companyService.GetCompanyDetailByUserIdAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CompanyDetailForCompanyDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CompanyDetailForCompanyDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CompanyDetailForCompanyDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<CompanyDetailForCompanyDTO>
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
                    Message = "An error occurred while get student information.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
