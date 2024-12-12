using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Api.Input.StudentControllers.WorkingReportController;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/working-report")]
    [ApiController]
    public class WorkingReportController : ControllerBase
    {
        private readonly IWorkingReportService _workingReportService;
        public WorkingReportController(IWorkingReportService workingReportService)
        {
            _workingReportService = workingReportService;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("list")]
        public async Task<IActionResult> GetWorkingReportList()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _workingReportService.GetAllByStudentIdAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<WorkingReportListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<WorkingReportListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<WorkingReportListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<WorkingReportListForStudentDTO>>
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
                    Message = "An error occurred while get working report list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("detail/{workingReportId}")]
        public async Task<IActionResult> GetWorkingReportDetail(int? workingReportId)
        {
            try
            {
                var dataResponse = await _workingReportService.GetWorkingReportDetailForStudentAsync(workingReportId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<WorkingReportListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<WorkingReportListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<WorkingReportListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<WorkingReportDetailForStudentDTO>
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
                    Message = "An error occurred while get working report detail.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPost("files/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/workingreports/files/");
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
        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkingReport([FromBody] CreateWorkingReportInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var filePath = Path.Combine("wwwroot/uploads/workingreports/files/", input.FileAttachment);

                // Initialize 
                byte[]? fileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.FileAttachment) && System.IO.File.Exists(filePath))
                {
                    fileData = await System.IO.File.ReadAllBytesAsync(filePath);
                }

                var workingReportInfo = new CreateWorkingReportForStudentDTO
                {
                    ReportTitle = input.ReportTitle,
                    ReportContent = input.ReportContent,
                };

                var apiResponse = await _workingReportService.CreateWorkingReportAsync(userId, workingReportInfo, input.FileAttachment, fileData);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateWorkingReportForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateWorkingReportForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateWorkingReportForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 200)
                {
                    if (fileData != null && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                return Ok(new ApiResponse<CreateWorkingReportForStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while create working report.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpPut("update/{workingReportId}")]
        public async Task<IActionResult> UpdateWorkingReport(int? workingReportId, [FromBody] UpdateWorkingReportInput? input)
        {
            try
            {
                var filePath = Path.Combine("wwwroot/uploads/workingreports/files/", input.FileAttachment);

                // Initialize 
                byte[]? fileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.FileAttachment) && System.IO.File.Exists(filePath))
                {
                    fileData = await System.IO.File.ReadAllBytesAsync(filePath);
                }

                var workingReportInfo = new UpdateWorkingReportForStudentDTO
                {
                    ReportTitle = input.ReportTitle,
                    ReportContent = input.ReportContent,
                };

                var apiResponse = await _workingReportService.UpdateWorkingReportAsync(workingReportId, workingReportInfo, input.FileAttachment, fileData);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<UpdateWorkingReportForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<UpdateWorkingReportForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<UpdateWorkingReportForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 200)
                {
                    if (fileData != null && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                return Ok(new ApiResponse<UpdateWorkingReportForStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while update working report.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
