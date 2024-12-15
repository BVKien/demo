using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Api.Input.MentorControllers.AttendanceReportController;
using static OJTEDU.Application.DTOs.AttendanceReportDTO;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace OJTEDU.Api.Controllers.MentorControllers
{
    [Route("api/mentor/attendance-report")]
    [ApiController]
    public class AttendanceReportController : ControllerBase
    {
        private readonly IJobService _jobService;
        public AttendanceReportController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [Authorize(Roles = "Mentor")]
        [HttpPut("set-time")]
        public async Task<IActionResult> SetCheckInCheckOutTime(SetCheckInCheckOutTimeInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var setTimeDto = new SetCheckInCheckOutTimeForMentorDTO
                {
                    CheckInTime = input?.CheckInTime,
                    CheckOutTime = input?.CheckOutTime
                };

                var dataResponse = await _jobService.SetCheckInCheckOutTimeAsync(userId, setTimeDto);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<SetCheckInCheckOutTimeForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<SetCheckInCheckOutTimeForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<SetCheckInCheckOutTimeForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<SetCheckInCheckOutTimeForMentorDTO>
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
                    Message = "An error occurred while setting check in check out time.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        //[Authorize(Roles = "Mentor")]
        //[HttpPut("update")]
        //public async Task<IActionResult> UpdateAttendanceReport(int? attendanceReportId, UpdateAttendanceReportInput? input)
        //{
        //    try
        //    {
        //        var arDto = new UpdateAttendanceReportForMentorDTO
        //        {
        //            CheckInTime = input?.CheckInTime,
        //            CheckOutTime = input?.CheckOutTime,
        //            Reason = input?.Reason,
        //            Status = input?.Status,
        //            EarlyLeave = input?.EarlyLeave,
        //            Late = input?.Late,
        //        };

        //        var dataResponse = await _jobService.UpdateAttendanceReportAsync(attendanceReportId, arDto);

        //        if (dataResponse.StatusCode == 404)
        //        {
        //            return BadRequest(new ApiResponse<UpdateAttendanceReportForMentorDTO>
        //            {
        //                Message = dataResponse.Message,
        //                Data = null
        //            });
        //        }

        //        if (dataResponse.StatusCode == 400)
        //        {
        //            return BadRequest(new ApiResponse<UpdateAttendanceReportForMentorDTO>
        //            {
        //                Message = dataResponse.Message,
        //                Data = null
        //            });
        //        }

        //        if (dataResponse.StatusCode == 500)
        //        {
        //            return StatusCode(500, new ApiResponse<UpdateAttendanceReportForMentorDTO>
        //            {
        //                Message = dataResponse.Message,
        //                Data = null
        //            });
        //        }

        //        var apiResponse = new ApiResponse<UpdateAttendanceReportForMentorDTO>
        //        {
        //            Message = dataResponse.Message,
        //            Data = dataResponse.Data
        //        };

        //        return Ok(apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new ApiResponse<string>
        //        {
        //            Message = "An error occurred while updating attendance report for internship.",
        //            Data = ex.Message
        //        };

        //        return StatusCode(500, errorResponse);
        //    }
        //}

        //[Authorize(Roles = "Mentor")]
        //[HttpPost("auto-create")]
        //public async Task<IActionResult> CreateAutoAttendanceReport(CreateAutoAttendanceReportInput? input)
        //{
        //    try
        //    {
        //        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        //        var dataResponse = await _jobService.CreateAutoAttendanceReportAsync(userId, input?.CheckInTime, input?.CheckOutTime);

        //        if (dataResponse.StatusCode == 404)
        //        {
        //            return BadRequest(new ApiResponse<bool>
        //            {
        //                Message = dataResponse.Message,
        //                Data = false
        //            });
        //        }

        //        if (dataResponse.StatusCode == 400)
        //        {
        //            return BadRequest(new ApiResponse<bool>
        //            {
        //                Message = dataResponse.Message,
        //                Data = false
        //            });
        //        }

        //        if (dataResponse.StatusCode == 500)
        //        {
        //            return StatusCode(500, new ApiResponse<bool>
        //            {
        //                Message = dataResponse.Message,
        //                Data = false
        //            });
        //        }

        //        var apiResponse = new ApiResponse<bool>
        //        {
        //            Message = dataResponse.Message,
        //            Data = dataResponse.Data
        //        };

        //        return Ok(apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new ApiResponse<string>
        //        {
        //            Message = "An error occurred while creating auto attendance report for internship.",
        //            Data = ex.Message
        //        };

        //        return StatusCode(500, errorResponse);
        //    }
        //}

        //[Authorize(Roles = "Mentor")]
        //[HttpPost("files/upload")]
        //public async Task<IActionResult> UploadFile(IFormFile file)
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //            return BadRequest("No file uploaded.");

        //        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/attendancereports/files/");
        //        if (!Directory.Exists(uploadPath))
        //        {
        //            Directory.CreateDirectory(uploadPath);
        //        }

        //        var filePath = Path.Combine(uploadPath, file.FileName);
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        byte[] fileData;
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(memoryStream);
        //            fileData = memoryStream.ToArray();
        //        }

        //        return Ok(new
        //        {
        //            Data = file.FileName
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new ApiResponse<string>
        //        {
        //            Message = $"An error occurred while uploading file.",
        //            Data = ex.Message
        //        };

        //        return StatusCode(500, errorResponse);
        //    }
        //}

        //[Authorize(Roles = "Mentor")]
        //[HttpGet("attendance-file/list/{fileName}")]
        //public async Task<IActionResult> ListAttendanceReports(string? fileName)
        //{
        //    try
        //    {
        //        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        //        var filePath = Path.Combine("wwwroot/uploads/attendancereports/files/", fileName);

        //        // Initialize 
        //        byte[]? fileData = null;

        //        // Read content file if it is not null
        //        if (!string.IsNullOrEmpty(fileName) && System.IO.File.Exists(filePath))
        //        {
        //            fileData = await System.IO.File.ReadAllBytesAsync(filePath);
        //        }

        //        var apiResponse = await _jobService.ListAttendanceReportsFromExcelAsync(userId, fileName, fileData);

        //        if (apiResponse.StatusCode == 404)
        //        {
        //            return BadRequest(new ApiResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>
        //            {
        //                Message = apiResponse.Message,
        //                Data = null
        //            });
        //        }

        //        if (apiResponse.StatusCode == 400)
        //        {
        //            return BadRequest(new ApiResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>
        //            {
        //                Message = apiResponse.Message,
        //                Data = null
        //            });
        //        }

        //        if (apiResponse.StatusCode == 500)
        //        {
        //            return StatusCode(500, new ApiResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>
        //            {
        //                Message = apiResponse.Message,
        //                Data = null
        //            });
        //        }

        //        if (apiResponse.StatusCode == 200)
        //        {
        //            if (fileData != null && System.IO.File.Exists(filePath))
        //            {
        //                System.IO.File.Delete(filePath);
        //            }
        //        }

        //        return Ok(new ApiResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>
        //        {
        //            Message = apiResponse.Message,
        //            Data = apiResponse.Data
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new ApiResponse<string>
        //        {
        //            Message = "An error occurred while uploading attendace file.",
        //            Data = ex.Message
        //        };

        //        return StatusCode(500, errorResponse);
        //    }
        //}

        //[Authorize(Roles = "Mentor")]
        //[HttpPost("attendance-file/import")]
        //public async Task<IActionResult> InsertAttendanceReports(InsertAttendanceReportInput? input)
        //{
        //    try
        //    {
        //        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        //        var filePath = Path.Combine("wwwroot/uploads/attendancereports/files/", input?.FileName);

        //        // Initialize 
        //        byte[]? fileData = null;

        //        // Read content file if it is not null
        //        if (!string.IsNullOrEmpty(input?.FileName) && System.IO.File.Exists(filePath))
        //        {
        //            fileData = await System.IO.File.ReadAllBytesAsync(filePath);
        //        }

        //        var apiResponse = await _jobService.InsertAttendanceReportsFromExcelAsync(userId, input?.FileName, fileData);

        //        if (apiResponse.StatusCode == 404)
        //        {
        //            return BadRequest(new ApiResponse<bool>
        //            {
        //                Message = apiResponse.Message,
        //                Data = false
        //            });
        //        }

        //        if (apiResponse.StatusCode == 400)
        //        {
        //            return BadRequest(new ApiResponse<bool>
        //            {
        //                Message = apiResponse.Message,
        //                Data = false
        //            });
        //        }

        //        if (apiResponse.StatusCode == 500)
        //        {
        //            return StatusCode(500, new ApiResponse<bool>
        //            {
        //                Message = apiResponse.Message,
        //                Data = false
        //            });
        //        }

        //        if (apiResponse.StatusCode == 200)
        //        {
        //            if (fileData != null && System.IO.File.Exists(filePath))
        //            {
        //                System.IO.File.Delete(filePath);
        //            }
        //        }

        //        return Ok(new ApiResponse<bool>
        //        {
        //            Message = apiResponse.Message,
        //            Data = apiResponse.Data
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new ApiResponse<string>
        //        {
        //            Message = "An error occurred while inserting attendace file.",
        //            Data = ex.Message
        //        };

        //        return StatusCode(500, errorResponse);
        //    }
        //}

        //[Authorize(Roles = "Mentor")]
        //[HttpPost("attendance-file/download")]
        //public async Task<IActionResult> DownloadAttendanceReports()
        //{
        //    try
        //    {
        //        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "attendancereports", "attendanceimportfile", "Attendance_Import_File.xlsx");

        //        if (!System.IO.File.Exists(filePath))
        //        {
        //            return NotFound(new
        //            {
        //                Success = false,
        //                Message = "The requested file does not exist."
        //            });
        //        }

        //        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        //        string fileName = "Attendance_Import_File.xlsx";

        //        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Success = false,
        //            Message = "An error occurred while processing your request.",
        //            Error = ex.Message
        //        });
        //    }
        //}

        //[Authorize(Roles = "Mentor")]
        //[HttpGet("list/{internshipId}")]
        //public async Task<IActionResult> GetAttendanceReportsListForInternship(int? internshipId)
        //{
        //    try
        //    {
        //        var dataResponse = await _jobService.GetAllAttendanceReportsByInternshipIdAsync(internshipId);

        //        if (dataResponse.StatusCode == 404)
        //        {
        //            return BadRequest(new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
        //            {
        //                Message = dataResponse.Message,
        //                Data = null
        //            });
        //        }

        //        if (dataResponse.StatusCode == 400)
        //        {
        //            return BadRequest(new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
        //            {
        //                Message = dataResponse.Message,
        //                Data = null
        //            });
        //        }

        //        if (dataResponse.StatusCode == 500)
        //        {
        //            return StatusCode(500, new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
        //            {
        //                Message = dataResponse.Message,
        //                Data = null
        //            });
        //        }

        //        var apiResponse = new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
        //        {
        //            Message = dataResponse.Message,
        //            Data = dataResponse.Data
        //        };

        //        return Ok(apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new ApiResponse<string>
        //        {
        //            Message = "An error occurred while get attendance report list for internship.",
        //            Data = ex.Message
        //        };

        //        return StatusCode(500, errorResponse);
        //    }
        //}

        //[Authorize(Roles = "Mentor")]
        //[HttpGet("list")]
        //public async Task<IActionResult> GetAttendanceReportsList()
        //{
        //    try
        //    {
        //        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        //        var dataResponse = await _jobService.GetAllAttendanceReportsForMentorAsync(userId);

        //        if (dataResponse.StatusCode == 404)
        //        {
        //            return BadRequest(new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
        //            {
        //                Message = dataResponse.Message,
        //                Data = null
        //            });
        //        }

        //        if (dataResponse.StatusCode == 400)
        //        {
        //            return BadRequest(new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
        //            {
        //                Message = dataResponse.Message,
        //                Data = null
        //            });
        //        }

        //        if (dataResponse.StatusCode == 500)
        //        {
        //            return StatusCode(500, new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
        //            {
        //                Message = dataResponse.Message,
        //                Data = null
        //            });
        //        }

        //        var apiResponse = new ApiResponse<List<AttendanceReportsListForMentorLecturerDTO>>
        //        {
        //            Message = dataResponse.Message,
        //            Data = dataResponse.Data
        //        };

        //        return Ok(apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new ApiResponse<string>
        //        {
        //            Message = "An error occurred while get attendance report list.",
        //            Data = ex.Message
        //        };

        //        return StatusCode(500, errorResponse);
        //    }
        // }
    }
}
