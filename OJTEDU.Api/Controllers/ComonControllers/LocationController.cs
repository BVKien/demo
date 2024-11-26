using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using static OJTEDU.Application.DTOs.ProvinceDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public LocationController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllLocations(int? provinceId, int? districtId)
        {
            try
            {
                var dataResponse = await _provinceService.GetAllLocationsAsync(provinceId, districtId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<LocationListForCommonDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<LocationListForCommonDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<LocationListForCommonDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<LocationListForCommonDTO>
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
                    Message = "An error occurred while get locations.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
