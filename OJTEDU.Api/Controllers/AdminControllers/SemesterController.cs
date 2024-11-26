using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.AdminControllers.SemesterController;
using static OJTEDU.Application.DTOs.SemesterDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin-doet/semester")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        public SemesterController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        // Admin-Doet - Semester
        [HttpGet("list")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetAllSemester(string? semesterCode, string? name, string? status, DateTime? startEventDate, DateTime? endEventDate, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _semesterService.GetAllSemesterForAdminDoetAsync(semesterCode, name, status, startEventDate, endEventDate, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<SemesterListForAdminDoetDTO>>>
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

        [HttpGet("details/{semesterId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetSemesterIdDetail(int? semesterId)
        {
            try
            {
                if (!semesterId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "semesterId is required."
                    });
                }

                var dataResponse = await _semesterService.GetSemesterDetailByIdForAdminDoetAsync(semesterId.Value);

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

                var apiResponse = new ApiResponse<SemesterDetailForAdminDoetDTO>
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

        [HttpPost]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> AddSemester([FromForm] AddSemesterRequestForAdminDoet request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.SemesterCode))
                {
                    errorMessages.Add("ProvinceName is required.");
                }
                else if (request.SemesterCode.Length > 50)
                {
                    errorMessages.Add("ProvinceName must not exceed 50 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.SemesterName))
                {
                    errorMessages.Add("SemesterName is required.");
                }
                else if (request.SemesterName.Length > 255)
                {
                    errorMessages.Add("SemesterName must not exceed 50 characters.");
                }

                // Kiểm tra StartDate và EndDate
                if (!request.StartDate.HasValue)
                {
                    errorMessages.Add("StartDate is required.");
                }
                if (!request.EndDate.HasValue)
                {
                    errorMessages.Add("EndDate is required.");
                }
                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    if (request.EndDate < request.StartDate)
                    {
                        errorMessages.Add("EndDate cannot be earlier than StartDate.");
                    }
                }

                // Nếu có lỗi, trả về phản hồi lỗi
                if (errorMessages.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = $"Validation errors occurred: {string.Join(", ", errorMessages)}"
                    });
                }

                var addDto = new AddSemesterForAdminDoetDTO
                {
                    SemesterCode = request.SemesterCode.ToUpper(),
                    Name = request.SemesterName,
                    Description = request.Description,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                };

                var dataResponse = await _semesterService.AddSemesterForAdminDoetAsync(addDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during semester add."
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

                var apiResponse = new ApiResponse<AddSemesterForAdminDoetDTO>
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

        [HttpPut("{semesterId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> UpdateSemester(int? semesterId, [FromForm] UpdateSemesterRequestForAdminDoet request)
        {
            try
            {
                if (!semesterId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "semesterId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.SemesterCode))
                {
                    errorMessages.Add("ProvinceName is required.");
                }
                else if (request.SemesterCode.Length > 50)
                {
                    errorMessages.Add("ProvinceName must not exceed 50 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.SemesterName))
                {
                    errorMessages.Add("SemesterName is required.");
                }
                else if (request.SemesterName.Length > 255)
                {
                    errorMessages.Add("SemesterName must not exceed 50 characters.");
                }

                // Kiểm tra StartDate và EndDate
                if (!request.StartDate.HasValue)
                {
                    errorMessages.Add("StartDate is required.");
                }
                if (!request.EndDate.HasValue)
                {
                    errorMessages.Add("EndDate is required.");
                }
                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    if (request.EndDate < request.StartDate)
                    {
                        errorMessages.Add("EndDate cannot be earlier than StartDate.");
                    }
                }

                // Nếu có lỗi, trả về phản hồi lỗi
                if (errorMessages.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = $"Validation errors occurred: {string.Join(", ", errorMessages)}"
                    });
                }

                var updateDto = new UpdateSemesterForAdminDoetDTO
                {
                    SemesterId = semesterId.Value,
                    SemesterCode = request.SemesterCode.ToUpper(),
                    Name = request.SemesterName,
                    Description = request.Description,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                };

                var dataResponse = await _semesterService.UpdateSemesterForAdminDoetAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during semester update."
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

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<UpdateSemesterForAdminDoetDTO>
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

        [HttpPatch("{semesterId}/status")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> UpdateSemesterStatus(int? semesterId, UpdateSemesterStatusRequestForAdminDoet request)
        {
            try
            {
                if (!semesterId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "semesterId is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Status))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Status is required."
                    });
                }

                var updateDto = new UpdateSemesterStatusForAdminDoetDTO
                {
                    SemesterId = semesterId.Value,
                    Status = request.Status
                };

                var dataResponse = await _semesterService.UpdateSemesterStatusForAdminDoetAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during semester update status."
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

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<UpdateSemesterStatusForAdminDoetDTO>
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

        [HttpDelete("{semesterId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSemester(int? semesterId)
        {
            try
            {
                if (!semesterId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "semesterId is required."
                    });
                }

                var deleteDto = new DeleteSemesterForAdminDoetDTO
                {
                    SemesterId = semesterId.Value
                };

                var dataResponse = await _semesterService.DeleteSemesterForAdminDoetAsync(deleteDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during semester delete."
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

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<DeleteSemesterForAdminDoetDTO>
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

        // Admin-DOET - Status List
        [HttpGet("status-list")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetAllStatusesSemesterForAdmin()
        {
            try
            {
                var dataResponse = await _semesterService.GetAllStatusesSemesterForAdminDoetAsync();

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

                var apiResponse = new ApiResponse<List<StatusSemesterListForAdminDoetDTO>>
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
