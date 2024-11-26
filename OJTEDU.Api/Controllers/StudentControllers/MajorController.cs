using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using static OJTEDU.Application.DTOs.MajorDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/major")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        private readonly IMajorService _majorService;
        public MajorController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("list")]
        public async Task<IActionResult> GetMajorList()
        {
            try
            {
                var dataResponse = await _majorService.GetAllMajorsAsync();

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<MajorListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<MajorListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<MajorListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<MajorListForStudentDTO>>
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
                    Message = "An error occurred while get majors list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
