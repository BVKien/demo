using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.CvDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/cv")]
    [ApiController]
    public class CvController : ControllerBase
    {
        private readonly ICvService _cvService;

        public CvController(ICvService cvService)
        {
            _cvService = cvService;
        }

        [Authorize(Roles = "Student")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadCv(string? fileName, string? filePath)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                if (file == null)
                    return BadRequest("No file uploaded.");

                //if (file == null || file.Length == 0)
                //    return BadRequest("No file uploaded.");

                //// Read content file to byte[]
                //byte[] fileData;
                //using (var memoryStream = new MemoryStream())
                //{
                //    await file.CopyToAsync(memoryStream);
                //    fileData = memoryStream.ToArray();
                //}

                var response = await _cvService.UploadCvAsync(userId, fileName, filePath);

                if (response.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                if (response.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                if (response.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<string>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<string>
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
                    Message = "An error occurred while upload CV.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPut("set-primary")]
        public async Task<IActionResult> SetPrimaryCv(int? cvId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var response = await _cvService.SetPrimaryCvAsync(userId, cvId);

                if (response.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = response.Message,
                        Data = false
                    });
                }

                if (response.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = response.Message,
                        Data = false
                    });
                }

                if (response.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Message = response.Message,
                        Data = false
                    });
                }

                var apiResponse = new ApiResponse<bool>
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
                    Message = "An error occurred while set primary CV.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAllStudentCv()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var response = await _cvService.GetAllCvByStudentIdAsync(userId);

                if (response.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<CvListForStudentDTO>>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                if (response.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<CvListForStudentDTO>>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                if (response.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<CvListForStudentDTO>>
                    {
                        Message = response.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<CvListForStudentDTO>>
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
                    Message = "An error occurred while get CV list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPut("delete-stored")]
        public async Task<IActionResult> DeleteAndStoredCv(int? cvId)
        {
            try
            {
                var response = await _cvService.DeleteAndStoredCvAsync(cvId);

                if (response.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = response.Message,
                        Data = false
                    });
                }

                if (response.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = response.Message,
                        Data = false
                    });
                }

                if (response.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Message = response.Message,
                        Data = false
                    });
                }

                var apiResponse = new ApiResponse<bool>
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
                    Message = "An error occurred while delete CV.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
