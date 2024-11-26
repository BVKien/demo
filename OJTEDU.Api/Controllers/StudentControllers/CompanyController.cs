using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using static OJTEDU.Application.DTOs.CompanyDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchCompanies(string? name, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                var dataResponse = await _companyService.SearchCompaniesForStudentAsync(name, provinceId, districtId, wardId, pageNumber, pageSize);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<CompanySearchListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<CompanySearchListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<CompanySearchListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponseTotalPaged<List<CompanySearchListForStudentDTO>>
                {
                    Message = dataResponse.Message,
                    TotalPageCount = dataResponse.TotalPages,
                    Data = dataResponse.Data
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while search companies.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("detail/{companyId}")]
        public async Task<IActionResult> GetCompanyDetail(int? companyId)
        {
            try
            {
                var dataResponse = await _companyService.GetCompanyDetailByCompanyIdForStudentAsync(companyId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CompanyDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CompanyDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CompanyDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<CompanyDetailForStudentDTO>
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
                    Message = "An error occurred while get company detail. ",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
