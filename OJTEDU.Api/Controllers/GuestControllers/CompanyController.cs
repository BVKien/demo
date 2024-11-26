using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Domain.Entities;
using static OJTEDU.Application.DTOs.CompanyDTO;

namespace OJTEDU.Api.Controllers.GuestControllers
{
    [Route("api/guest/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCompanies(string? name, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                var dataResponse = await _companyService.SearchCompaniesAsync(name, provinceId, districtId, wardId, pageNumber, pageSize);

                var apiResponse = new ApiResponseTotalPaged<List<CompanySearchListForGuestDTO>>
                {
                    Message = dataResponse.Message,
                    TotalPageCount = dataResponse.TotalPages,
                    Data = dataResponse.Data
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>
                {
                    Message = "An error occurred while search companies. ",
                    Data = new { Details = ex.Message }
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("detail/{companyId}")]
        public async Task<IActionResult> GetCompanyDetail(int? companyId)
        {
            try
            {
                var dataResponse = await _companyService.GetCompanyDetailByCompanyIdAsync(companyId);

                var apiResponse = new ApiResponse<CompanyDetailForGuestDTO>
                {
                    Message = dataResponse.Message,
                    Data = dataResponse.Data
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>
                {
                    Message = $"An error occurred while get company detail with company id {companyId}. ",
                    Data = new { Details = ex.Message }
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
