using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Api.Input.AdminControllers.UserGuideController;
using static OJTEDU.Application.DTOs.UserGuideDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin/user-guide")]
    [ApiController]
    public class UserGuideController : ControllerBase
    {
        private readonly IUserGuideService _userGuideService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserGuideController(IUserGuideService userGuideService, IWebHostEnvironment webHostEnvironment)
        {
            _userGuideService = userGuideService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUserGuidesForAdmin(string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _userGuideService.GetAllUserGuidesForAdminAsync(title, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<UserGuideListForAdminDTO>>>
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

        [HttpGet("details/{userGuideId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserGuideDetailForAdmin(int? userGuideId)
        {
            try
            {
                if (!userGuideId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "userGuideId is required."
                    });
                }

                var dataResponse = await _userGuideService.GetUserGuideDetailByIdForAdminAsync(userGuideId.Value);

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

                // Successful retrieval of users
                var apiResponse = new ApiResponse<UserGuideDetailForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDocumentForAdmin([FromForm] AddOrUpdateUserGuideRequestForAdmin request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra file
                if (request.UserGuideFile == null || request.UserGuideFile.Length == 0)
                {
                    errorMessages.Add("UserGuide File is required.");
                }
                else
                {
                    // Lấy phần mở rộng của file
                    string fileExtension = Path.GetExtension(request.UserGuideFile.FileName).ToLower();

                    // Chỉ cho phép file .pdf
                    if (fileExtension != ".pdf")
                    {
                        errorMessages.Add("Only .pdf files are allowed.");
                    }
                }

                if (!request.RoleId.HasValue)
                {
                    errorMessages.Add("Role is required.");
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

                string createdByUniversityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Tạo tên file duy nhất
                string fileName = request.UserGuideFile.FileName;
                string uniqueFileName = $"{createdByUniversityId}_{DateTime.Now:yyyyMMddHHmmssfff}_{Path.GetFileNameWithoutExtension(fileName)}.pdf";

                // Lấy đường dẫn đến thư mục wwwroot/documents
                string userguidesPath = Path.Combine(_webHostEnvironment.WebRootPath, "userguides");

                // Kiểm tra xem thư mục tồn tại chưa, nếu không có thì tạo mới
                if (!Directory.Exists(userguidesPath))
                {
                    Directory.CreateDirectory(userguidesPath);
                }

                // Tạo đường dẫn đầy đủ đến tệp tin
                string filePath = Path.Combine(userguidesPath, uniqueFileName);

                // Lưu tệp tin vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.UserGuideFile.CopyToAsync(fileStream);
                }

                var relativeDocumentPath = $"/userguides/{uniqueFileName}";

                // Tạo tài liệu để lưu vào cơ sở dữ liệu
                var userGuideDto = new AddUserGuideForAdminDTO
                {
                    UserGuideFile = relativeDocumentPath, // Lưu tên file vào cơ sở dữ liệu
                    ForRoleId = request.RoleId.Value
                };

                var dataResponse = await _userGuideService.AddUserGuideForAdminAsync(userGuideDto);

                if (dataResponse == null)
                {
                    // Xóa file nếu có lỗi
                    System.IO.File.Delete(filePath);

                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during user guide add."
                    });
                }

                if (dataResponse.Data == null)
                {
                    // Xóa file nếu có lỗi
                    System.IO.File.Delete(filePath);

                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Successful retrieval of students
                var apiResponse = new ApiResponse<AddUserGuideForAdminDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                // Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "userguides", request.UserGuideFile.FileName)))
                {
                    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "userguides", request.UserGuideFile.FileName));
                }
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }


        [HttpPut("{userGuideId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserGuideForAdmin(int? userGuideId, [FromForm] AddOrUpdateUserGuideRequestForAdmin request)
        {
            try
            {
                if (!userGuideId.HasValue)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "UserGuideId is required."
                    });
                }

                var errorMessages = new List<string>();

                // Nếu file được gửi lên, kiểm tra định dạng
                if (request.UserGuideFile != null && request.UserGuideFile.Length > 0)
                {
                    string fileExtension = Path.GetExtension(request.UserGuideFile.FileName).ToLower();
                    if (fileExtension != ".pdf")
                    {
                        errorMessages.Add("Only .pdf files are allowed.");
                    }
                }

                if (!request.RoleId.HasValue)
                {
                    errorMessages.Add("Role is required.");
                }

                if (errorMessages.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = $"Validation errors occurred: {string.Join(", ", errorMessages)}"
                    });
                }

                var existingUserGuides = await _userGuideService.GetUserGuideDetailByIdForAdminAsync(userGuideId.Value);
                if (existingUserGuides == null || existingUserGuides.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "User Guide not found."
                    });
                }

                var userGuideDto = new UpdateUserGuideForAdminDTO
                {
                    UserGuideId = userGuideId.Value,
                    UserGuideFile = existingUserGuides.Data.UserGuideFile,
                    ForRoleId = request.RoleId.Value
                };

                string createdByUniversityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Nếu có file được gửi lên
                if (request.UserGuideFile != null && request.UserGuideFile.Length > 0)
                {
                    // Tạo file mới
                    string uniqueFileName = $"{createdByUniversityId}_{DateTime.Now:yyyyMMddHHmmssfff}_{Path.GetFileNameWithoutExtension(request.UserGuideFile.FileName)}.pdf";
                    string userGuidesPath = Path.Combine(_webHostEnvironment.WebRootPath, "userguides");

                    if (!Directory.Exists(userGuidesPath))
                    {
                        Directory.CreateDirectory(userGuidesPath);
                    }

                    string newFilePath = Path.Combine(userGuidesPath, uniqueFileName);
                    using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await request.UserGuideFile.CopyToAsync(fileStream);
                    }

                    // Xóa file cũ nếu tồn tại
                    string oldUserGuidePath = Path.Combine(_webHostEnvironment.WebRootPath, userGuideDto.UserGuideFile.TrimStart('/'));
                    if (System.IO.File.Exists(oldUserGuidePath))
                    {
                        System.IO.File.Delete(oldUserGuidePath);
                    }

                    // Cập nhật đường dẫn file mới
                    var relativeDocumentPath = $"/userguides/{uniqueFileName}";
                    userGuideDto.UserGuideFile = relativeDocumentPath;
                }

                var dataResponse = await _userGuideService.UpdateUserGuideForAdminAsync(userGuideDto);

                if (dataResponse == null || dataResponse.Data == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse?.Message ?? "Unexpected error occurred during user guide update."
                    });
                }

                return Ok(new ApiResponse<UpdateUserGuideForAdminDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                });
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


        [HttpDelete("{userGuideId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserGuideForAdmin(int? userGuideId)
        {
            try
            {
                if (!userGuideId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "UserGuideId is required."
                    });
                }

                var userGuideDto = new DeleteUserGuideForAdminDTO
                {
                    UserGuideId = userGuideId.Value
                };

                // Gọi dịch vụ để xóa người dùng
                var dataResponse = await _userGuideService.DeleteUserGuideForAdminAsync(userGuideDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during user guide delete."
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

                // Xóa tệp tin vật lý khỏi thư mục nếu xóa trong cơ sở dữ liệu thành công
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.UserGuideFile.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<DeleteUserGuideForAdminDTO>
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

        [HttpPatch("{userGuideId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDocumentStatusForAdmin(int? userGuideId, UpdateUserGuideStatusRequestForAdmin request)
        {
            try
            {
                if (!userGuideId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "UserGuideId is required."
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

                var userGuideDto = new UpdateUserGuideStatusForAdminDTO
                {
                    UserGuideId = userGuideId.Value,
                    Status = request.Status
                };

                var dataResponse = await _userGuideService.UpdateUserGuideStatusForAdminAsync(userGuideDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during user guide update status."
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
                var apiResponse = new ApiResponse<UpdateUserGuideStatusForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStatusesForAdmin()
        {
            try
            {
                var dataResponse = await _userGuideService.GetAllStatusesUserGuideForAdminAsync();

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

                var apiResponse = new ApiResponse<List<StatusUserGuideListForAdminDTO>>
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

        [HttpGet("download-file/{userGuideId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadUserGuideForAdmin(int? userGuideId)
        {
            try
            {
                if (!userGuideId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "UserGuideId is required."
                    });
                }

                var dataResponse = await _userGuideService.GetUserGuideDetailByIdForAdminAsync(userGuideId.Value);

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

                // Đường dẫn file PDF
                var pdfFilePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.UserGuideFile.TrimStart('/'));

                if (!System.IO.File.Exists(pdfFilePath))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "File not found on the server."
                    });
                }

                // Tên file Word khi tải về
                var wordFileName = Path.GetFileNameWithoutExtension(pdfFilePath) + ".docx";
                var tempWordFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "temp", wordFileName);

                try
                {
                    // Đảm bảo thư mục tạm tồn tại
                    var tempDir = Path.Combine(_webHostEnvironment.WebRootPath, "temp");
                    if (!Directory.Exists(tempDir))
                    {
                        Directory.CreateDirectory(tempDir);
                    }

                    // Chuyển đổi PDF sang Word
                    using (var pdfDocument = new Aspose.Pdf.Document(pdfFilePath))
                    {
                        var saveOptions = new Aspose.Pdf.DocSaveOptions
                        {
                            Format = Aspose.Pdf.DocSaveOptions.DocFormat.DocX,
                            Mode = Aspose.Pdf.DocSaveOptions.RecognitionMode.Flow // Giữ định dạng dòng
                        };

                        pdfDocument.Save(tempWordFilePath, saveOptions);
                    }

                    // Đọc file Word thành byte array
                    var wordFileBytes = await System.IO.File.ReadAllBytesAsync(tempWordFilePath);

                    // Trả về file Word
                    return File(wordFileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", wordFileName);
                }
                finally
                {
                    // Xóa file tạm sau khi sử dụng
                    if (System.IO.File.Exists(tempWordFilePath))
                    {
                        System.IO.File.Delete(tempWordFilePath);
                    }
                }
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
