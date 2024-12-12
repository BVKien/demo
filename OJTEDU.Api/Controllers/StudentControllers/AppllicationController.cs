using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System.Security.Claims;
using static OJTEDU.Api.Input.StudentControllers.AppllicationController;
using static OJTEDU.Application.DTOs.AppllicationDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/application")]
    [ApiController]
    public class AppllicationController : ControllerBase
    {
        private readonly IAppllicationService _appllicationService;

        public AppllicationController(IAppllicationService appllicationService)
        {
            _appllicationService = appllicationService;
        }

        [Authorize(Roles = "Student")]
        [HttpPost("files/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/applications/files/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                return Ok(new
                {
                    Data = file.FileName
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while uploading file.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyJob([FromBody] ApplyJobInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var testFilePath = Path.Combine("wwwroot/uploads/applications/files/", input.TestFile);
                var cvFilePath = input.CvFilePath; // Path.Combine("wwwroot/uploads/applications/files/", input.CvFile);

                // Initialize 
                byte[]? testFileData = null;
                byte[]? cvFileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.TestFile) && System.IO.File.Exists(testFilePath))
                {
                    testFileData = await System.IO.File.ReadAllBytesAsync(testFilePath);
                }
                //if (!string.IsNullOrEmpty(input.CvFile) && System.IO.File.Exists(cvFilePath))
                //{
                //    cvFileData = await System.IO.File.ReadAllBytesAsync(cvFilePath);
                //}

                var applyInfoDto = new ApplyJobForStudentDTO
                {
                    JobId = input.JobId,
                    CoverLetter = input.CoverLetter,
                    CvId = input.CvId
                };

                var apiResponse = await _appllicationService.ApplyJobAsync(userId, applyInfoDto, input.TestFile, testFileData, input.CvFilePath, cvFileData);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<ApplyJobForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<ApplyJobForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<ApplyJobForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 200)
                {
                    if (testFileData != null && System.IO.File.Exists(testFilePath))
                    {
                        System.IO.File.Delete(testFilePath);
                    }

                    if (cvFileData != null && System.IO.File.Exists(cvFilePath))
                    {
                        System.IO.File.Delete(cvFilePath);
                    }
                }

                return Ok(new ApiResponse<ApplyJobForStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while applying for job.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAllApplicationsByUserId()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _appllicationService.GetAllApplicationsByUserIdAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<AppllicationListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<AppllicationListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<AppllicationListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<AppllicationListForStudentDTO>>
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
                    Message = "An error occurred while get applications list for student. ",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPut("offer/action/{applicationId}")]
        public async Task<IActionResult> CompanyOffersActions(int? applicationId, string? studentRejectReason, string? status)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _appllicationService.CompanyOffersActionsAsync(userId, applicationId, studentRejectReason, status);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = dataResponse.Message,
                        Data = false
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = dataResponse.Message,
                        Data = false
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Message = dataResponse.Message,
                        Data = false
                    });
                }

                var apiResponse = new ApiResponse<bool>
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
                    Message = "An error occurred while actions to company offers for student. ",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
