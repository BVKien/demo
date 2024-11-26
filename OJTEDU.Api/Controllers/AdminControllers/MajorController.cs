using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.AdminControllers.MajorController;
using static OJTEDU.Application.DTOs.MajorDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin-doet/major")]
    public class MajorController : ControllerBase
    {
        private readonly IMajorService _majorService;
        public MajorController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        [HttpGet("list")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetAllMajor(string? majorCode, string? majorName, string? status, int? departmentId, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _majorService.GetAllMajorForAdminDoetAsync(majorCode, majorName, status, departmentId, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>
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

        [HttpGet("details/{majorId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetMajorDetail(int? majorId)
        {
            try
            {
                if (!majorId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "majorId is required."
                    });
                }

                var dataResponse = await _majorService.GetMajorIdDetailByIdForAdminDoetAsync(majorId.Value);

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

                var apiResponse = new ApiResponse<MajorDetailForAdminDoetDTO>
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
        public async Task<IActionResult> AddMajor([FromForm] AddMajorRequestForAdminDoet request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.MajorCode))
                {
                    errorMessages.Add("MajorCode is required.");
                }
                else if (request.MajorCode.Length > 50)
                {
                    errorMessages.Add("DepartmentCode must not exceed 50 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.MajorName))
                {
                    errorMessages.Add("MajorName is required.");
                }
                else if (request.MajorName.Length > 255)
                {
                    errorMessages.Add("MajorName must not exceed 255 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    errorMessages.Add("Description is required.");
                }

                if (!request.DepartmentId.HasValue && request.DepartmentId <= 0)
                {
                    errorMessages.Add("Department is required.");
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

                var majorDTO = new AddMajorForAdminDoetDTO
                {
                    MajorCode = request.MajorCode.ToUpper(),
                    Name = request.MajorName,
                    Description = request.Description,
                    DepartmentId = request.DepartmentId
                };

                var dataResponse = await _majorService.AddMajorForAdminDoetAsync(majorDTO);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during major add."
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

                var apiResponse = new ApiResponse<AddMajorForAdminDoetDTO>
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

        [HttpPut("{majorId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> UpdateMajor(int? majorId, [FromForm] UpdateMajorRequestForAdminDoet request)
        {
            try
            {
                if (!majorId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "majorId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.MajorCode))
                {
                    errorMessages.Add("MajorCode is required.");
                }
                else if (request.MajorCode.Length > 50)
                {
                    errorMessages.Add("DepartmentCode must not exceed 50 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.MajorName))
                {
                    errorMessages.Add("MajorName is required.");
                }
                else if (request.MajorName.Length > 255)
                {
                    errorMessages.Add("MajorName must not exceed 255 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    errorMessages.Add("Description is required.");
                }

                if (!request.DepartmentId.HasValue && request.DepartmentId <= 0)
                {
                    errorMessages.Add("Department is required.");
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

                var updateDto = new UpdateMajorForAdminDoetDTO
                {
                    MajorId = majorId.Value,
                    MajorCode = request.MajorCode.ToUpper(),
                    Name = request.MajorName,
                    Description = request.Description,
                    DepartmentId = request.DepartmentId
                };

                var dataResponse = await _majorService.UpdateMajorForAdminDoetAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during major update."
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
                var apiResponse = new ApiResponse<UpdateMajorForAdminDoetDTO>
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

        [HttpDelete("{majorId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> DeleteMajorForAdmin(int? majorId)
        {
            try
            {
                if (!majorId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "majorId is required."
                    });
                }

                var majorDto = new DeleteMajorForAdminDoetDTO
                {
                    MajorId = majorId.Value
                };

                var dataResponse = await _majorService.DeleteMajorForAdminDoetAsync(majorDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during major delete."
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
                var apiResponse = new ApiResponse<DeleteMajorForAdminDoetDTO>
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

        [HttpPatch("{majorId}/status")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> UpdateDepartmentStatus(int? majorId, UpdateMajorStatusRequestForAdminDoet request)
        {
            try
            {
                if (!majorId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "majorId is required."
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

                var majorDto = new UpdateMajorStatusForAdminDoetDTO
                {
                    MajorId = majorId.Value,
                    Status = request.Status
                };

                var dataResponse = await _majorService.UpdateMajorStatusForAdminDoetAsync(majorDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during major update status."
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
                var apiResponse = new ApiResponse<UpdateMajorStatusForAdminDoetDTO>
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

        [HttpGet("status-list")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetAllStatusesMajorForAdmin()
        {
            try
            {
                var dataResponse = await _majorService.GetAllStatusesMajorForAdminDoetAsync();

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

                var apiResponse = new ApiResponse<List<StatusMajorListForAdminDoetDTO>>
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
