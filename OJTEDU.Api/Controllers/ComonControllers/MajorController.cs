using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using static OJTEDU.Application.DTOs.MajorDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/common/major")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        private readonly IMajorService _majorService;
        public MajorController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllMajor()
        {
            try
            {
                var dataResponse = await _majorService.GetAllMajorForCommonAsync();

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                var apiResponse = new ApiResponse<List<MajorListForCommonDTO>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }
    }
}
