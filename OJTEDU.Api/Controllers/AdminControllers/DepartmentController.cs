using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.AdminControllers.DepartmentController;
using static OJTEDU.Application.DTOs.DepartmentDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin-doet/department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("list")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetAllDepartment(string? departmentCode, string? departmentName, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _departmentService.GetAllDepartmentForAdminDoetAsync(departmentCode, departmentName, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>
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

        [HttpGet("details/{departmentId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetBannerDetail(int? departmentId)
        {
            try
            {
                if (!departmentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "departmentId is required."
                    });
                }

                var dataResponse = await _departmentService.GetDepartmentDetailByIdForAdminDoetAsync(departmentId.Value);

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

                var apiResponse = new ApiResponse<DepartmentDetailForAdminDoetDTO>
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
        public async Task<IActionResult> AddDepartment([FromForm] AddDepartmentRequestForAdminDoet request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.DepartmentCode))
                {
                    errorMessages.Add("DepartmentCode is required.");
                }
                else if (request.DepartmentCode.Length > 50)
                {
                    errorMessages.Add("DepartmentCode must not exceed 50 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.DepartmentName))
                {
                    errorMessages.Add("DepartmentName is required.");
                }
                else if (request.DepartmentName.Length > 255)
                {
                    errorMessages.Add("DepartmentName must not exceed 255 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.Detail))
                {
                    errorMessages.Add("Detail is required.");
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

                var departmentDto = new AddDepartmentForAdminDoetDTO
                {
                    DepartmentCode = request.DepartmentCode.ToUpper(),
                    Name = request.DepartmentName,
                    Detail = request.Detail
                };

                var dataResponse = await _departmentService.AddDepartmentForAdminDoetAsync(departmentDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during department add."
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

                var apiResponse = new ApiResponse<AddDepartmentForAdminDoetDTO>
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

        [HttpPut("{departmentId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> UpdateDepartment(int? departmentId, [FromForm] UpdateDepartmentRequestForAdminDoet request)
        {
            try
            {
                if (!departmentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "departmentId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.DepartmentCode))
                {
                    errorMessages.Add("DepartmentCode is required.");
                }
                else if (request.DepartmentCode.Length > 50)
                {
                    errorMessages.Add("DepartmentCode must not exceed 50 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.DepartmentName))
                {
                    errorMessages.Add("DepartmentName is required.");
                }
                else if (request.DepartmentName.Length > 255)
                {
                    errorMessages.Add("DepartmentName must not exceed 255 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.Detail))
                {
                    errorMessages.Add("Detail is required.");
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

                var updateDto = new UpdateDepartmentForAdminDoetDTO
                {
                    DepartmentId = departmentId.Value,
                    DepartmentCode = request.DepartmentCode.ToUpper(),
                    Name = request.DepartmentName,
                    Detail = request.Detail
                };

                var dataResponse = await _departmentService.UpdateDepartmentForAdminDoetAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during department update."
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
                var apiResponse = new ApiResponse<UpdateDepartmentForAdminDoetDTO>
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

        [HttpDelete("{departmentId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> DeleteDepartmentForAdmin(int? departmentId)
        {
            try
            {
                if (!departmentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "departmentId is required."
                    });
                }

                var departmentDto = new DeleteDepartmentForAdminDoetDTO
                {
                    DepartmentId = departmentId.Value
                };

                var dataResponse = await _departmentService.DeleteDepartmentForAdminDoetAsync(departmentDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during department delete."
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
                var apiResponse = new ApiResponse<DeleteDepartmentForAdminDoetDTO>
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

        [HttpPatch("{departmentId}/status")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> UpdateDepartmentStatus(int? departmentId, UpdateDepartmentStatusRequestForAdminDoet request)
        {
            try
            {
                if (!departmentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "departmentId is required."
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

                var departmentDto = new UpdateDepartmentStatusForAdminDoetDTO
                {
                    DepartmentId = departmentId.Value,
                    Status = request.Status
                };

                var dataResponse = await _departmentService.UpdateDepartmentStatusForAdminDoetAsync(departmentDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during department update status."
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
                var apiResponse = new ApiResponse<UpdateDepartmentStatusForAdminDoetDTO>
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

        [HttpPost("import-file")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> ImportDepartmentsForAdminDoet([FromForm] IFormFile file)
        {
            try
            {
                // Kiểm tra file đầu vào
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "The uploaded file is empty or missing. Please ensure you provide a valid Excel file. If you're unsure of the required format, download the template and follow the instructions provided in the User Guide."
                    });
                }

                // Kiểm tra định dạng file (chỉ chấp nhận .xlsx hoặc .xls)
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Invalid file format. Only Excel files (.xlsx, .xls) are accepted. Please download the template to prepare your data correctly and follow the instructions in the User Guide."
                    });
                }

                // Gọi service để thực hiện import
                var dataResponse = await _departmentService.ImportDepartmentsForAdminDoetAsync(file);

                // Kiểm tra kết quả trả về từ service
                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during the import process."
                    });
                }

                // Trả về lỗi nếu có
                if (dataResponse.StatusCode != 200)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = dataResponse.Data,
                        Message = dataResponse.Message
                    });
                }

                // Trả về kết quả import thành công
                return Ok(new ApiResponse<object>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Error occurred while importing departments: {ex.Message}"
                });
            }
        }


        [HttpGet("download-file-template")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> DownloadTemplateForAdminDoet()
        {
            try
            {
                var dataResponse = await _departmentService.GenerateDepartmentTemplateForAdminDoetAsync();

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during template generation."
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

                // Đặt lại vị trí của MemoryStream về đầu
                var templateStream = dataResponse.Data;
                templateStream.Position = 0;

                // Trả về file dưới dạng file download
                var fileResult = new FileStreamResult(templateStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "DepartmentTemplateForAdminDoet.xlsx"
                };

                return fileResult; // Trả về file Excel cho client
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Error downloading template: {ex.Message}"
                });
            }
        }

        [HttpGet("status-list")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetAllStatusesDepartmentForAdmin()
        {
            try
            {
                var dataResponse = await _departmentService.GetAllStatusesDepartmentForAdminDoetAsync();

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

                var apiResponse = new ApiResponse<List<StatusDepartmentListForAdminDoetDTO>>
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
