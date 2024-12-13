using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Api.Input.AdminControllers.InternshipProcessController;
using static OJTEDU.Application.DTOs.InternshipProcessDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin-doet/internship-process")]
    [ApiController]
    public class InternshipProcessController : ControllerBase
    {
        private readonly IInternshipProcessService _internshipProcessService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InternshipProcessController(IInternshipProcessService internshipProcessService, IWebHostEnvironment webHostEnvironment)
        {
            _internshipProcessService = internshipProcessService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("list")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetAllInternshipProcessForAdminDoet(string? title, bool? isVisible, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _internshipProcessService.GetAllInternshipProcessForAdminDoetAsync(title, isVisible, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<InternshipProcessListForAdminDoetDTO>>>
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

        [HttpGet("details/{internshipProcessId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> GetInternshipProcessDetailForAdminDoet(int? internshipProcessId)
        {
            try
            {
                if (!internshipProcessId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "InternshipProcessId is required."
                    });
                }

                var dataResponse = await _internshipProcessService.GetInternshipProcessDetailByIdForAdminDoetAsync(internshipProcessId.Value);

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
                var apiResponse = new ApiResponse<InternshipProcessDetailForAdminDoetDTO>
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
        public async Task<IActionResult> AddInternshipProcessForAdminDoet([FromForm] AddOrUpdateInternshipProcessRequestForAdmin request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (string.IsNullOrEmpty(request.Title))
                {
                    errorMessages.Add("Title is required.");
                }

                // Kiểm tra file
                if (request.FilePath == null || request.FilePath.Length == 0)
                {
                    errorMessages.Add("File is required.");
                }
                //else
                //{
                //    // Lấy phần mở rộng của file
                //    string fileExtension = Path.GetExtension(request.FilePath.FileName).ToLower();

                //    // Chỉ cho phép file .pdf
                //    if (fileExtension != ".pdf")
                //    {
                //        errorMessages.Add("Only .pdf files are allowed.");
                //    }

                //    // Giới hạn dung lượng file (tối đa 10MB)
                //    long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                //    if (request.FilePath.Length > maxFileSizeInBytes)
                //    {
                //        errorMessages.Add("File size must not exceed 10MB.");
                //    }
                //}

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

                //// Tạo tên file duy nhất
                //string fileName = request.FilePath.FileName;
                //string uniqueFileName = $"{createdByUniversityId}_{DateTime.Now:yyyyMMddHHmmssfff}_{Path.GetFileNameWithoutExtension(fileName)}.pdf";

                //// Lấy đường dẫn đến thư mục wwwroot/documents
                //string internshipprocessPath = Path.Combine(_webHostEnvironment.WebRootPath, "internshipprocess");

                //// Kiểm tra xem thư mục tồn tại chưa, nếu không có thì tạo mới
                //if (!Directory.Exists(internshipprocessPath))
                //{
                //    Directory.CreateDirectory(internshipprocessPath);
                //}

                //// Tạo đường dẫn đầy đủ đến tệp tin
                //string filePath = Path.Combine(internshipprocessPath, uniqueFileName);

                //// Lưu tệp tin vào thư mục
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await request.FilePath.CopyToAsync(fileStream);
                //}

                //var relativeDocumentPath = $"/internshipprocess/{uniqueFileName}";

                // Tạo tài liệu để lưu vào cơ sở dữ liệu
                var userGuideDto = new AddInternshipProcessForAdminDoetDTO
                {
                    Title = request.Title,
                    CreatedBy = int.Parse(createdByUniversityId),
                    FilePath = request.FilePath  // Lưu tên file vào cơ sở dữ liệu
                };

                var dataResponse = await _internshipProcessService.AddInternshipProcessForAdminDoetAsync(userGuideDto);

                if (dataResponse == null)
                {
                    // Xóa file nếu có lỗi
                    //System.IO.File.Delete(filePath);

                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during internship process add."
                    });
                }

                if (dataResponse.Data == null)
                {
                    // Xóa file nếu có lỗi
                    //System.IO.File.Delete(filePath);

                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Successful retrieval of students
                var apiResponse = new ApiResponse<AddInternshipProcessForAdminDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                //// Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                //if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "internshipprocess", request.FilePath.FileName)))
                //{
                //    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "internshipprocess", request.FilePath.FileName));
                //}
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }


        [HttpPut("{internshipProcessId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> UpdateInternshipProcessForAdminDoet(int? internshipProcessId, [FromForm] AddOrUpdateInternshipProcessRequestForAdmin request)
        {
            try
            {
                if (!internshipProcessId.HasValue)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "InternshipProcessId is required."
                    });
                }

                var errorMessages = new List<string>();

                if (string.IsNullOrEmpty(request.Title))
                {
                    errorMessages.Add("Title is required.");
                }

                //// Nếu file được gửi lên, kiểm tra định dạng
                //if (request.FilePath != null && request.FilePath.Length > 0)
                //{
                //    string fileExtension = Path.GetExtension(request.FilePath.FileName).ToLower();
                //    if (fileExtension != ".pdf")
                //    {
                //        errorMessages.Add("Only .pdf files are allowed.");
                //    }

                //    // Giới hạn dung lượng file (tối đa 10MB)
                //    long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                //    if (request.FilePath.Length > maxFileSizeInBytes)
                //    {
                //        errorMessages.Add("File size must not exceed 10MB.");
                //    }
                //}

                if (errorMessages.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = $"Validation errors occurred: {string.Join(", ", errorMessages)}"
                    });
                }

                var existingInternProcess = await _internshipProcessService.GetInternshipProcessDetailByIdForAdminDoetAsync(internshipProcessId.Value);
                if (existingInternProcess == null || existingInternProcess.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Intership Process not found."
                    });
                }

                var internProcessDto = new UpdateInternshipProcessForAdminDoetDTO
                {
                    IntershipProcessId = internshipProcessId.Value,
                    FilePath = existingInternProcess.Data.FilePath,
                    Title = request.Title
                };

                string createdByUniversityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Nếu có file được gửi lên
                if (request.FilePath != null && request.FilePath.Length > 0)
                {
                    //// Tạo file mới
                    //string uniqueFileName = $"{createdByUniversityId}_{DateTime.Now:yyyyMMddHHmmssfff}_{Path.GetFileNameWithoutExtension(request.FilePath.FileName)}.pdf";
                    //string internProcessPath = Path.Combine(_webHostEnvironment.WebRootPath, "internshipprocess");

                    //if (!Directory.Exists(internProcessPath))
                    //{
                    //    Directory.CreateDirectory(internProcessPath);
                    //}

                    //string newFilePath = Path.Combine(internProcessPath, uniqueFileName);
                    //using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                    //{
                    //    await request.FilePath.CopyToAsync(fileStream);
                    //}

                    //// Xóa file cũ nếu tồn tại
                    //string oldInternProcessPath = Path.Combine(_webHostEnvironment.WebRootPath, internProcessDto.FilePath.TrimStart('/'));
                    //if (System.IO.File.Exists(oldInternProcessPath))
                    //{
                    //    System.IO.File.Delete(oldInternProcessPath);
                    //}

                    //// Cập nhật đường dẫn file mới
                    //var relativeDocumentPath = $"/internshipprocess/{uniqueFileName}";
                    internProcessDto.FilePath = request.FilePath;
                }

                var dataResponse = await _internshipProcessService.UpdateInternshipProcessForAdminDoetAsync(internProcessDto);

                if (dataResponse == null || dataResponse.Data == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse?.Message ?? "Unexpected error occurred during internship process update."
                    });
                }

                return Ok(new ApiResponse<UpdateInternshipProcessForAdminDoetDTO>
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


        [HttpDelete("{internshipProcessId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> DeleteInternshipProcessForAdminDoet(int? internshipProcessId)
        {
            try
            {
                if (!internshipProcessId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "InternshipProcessId is required."
                    });
                }

                var internshipProcessDto = new DeleteInternshipProcessForAdminDoetDTO
                {
                    IntershipProcessId = internshipProcessId.Value
                };

                // Gọi dịch vụ để xóa người dùng
                var dataResponse = await _internshipProcessService.DeleteInternshipProcessForAdminDoetAsync(internshipProcessDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during internship process delete."
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
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<DeleteInternshipProcessForAdminDoetDTO>
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

        [HttpPatch("{internshipProcessId}/visible")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> UpdateInternshipProcessVisibleForAdminDoet(int? internshipProcessId, UpdateInternshipProcessVisibleRequestForAdmin request)
        {
            try
            {
                if (!internshipProcessId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "InternshipProcessId is required."
                    });
                }

                if (!request.IsVisible.HasValue)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Visibility status is required."
                    });
                }

                var userGuideDto = new UpdateInternshipProcessForAdminDoetDTO
                {
                    IntershipProcessId = internshipProcessId.Value,
                    IsVisible = request.IsVisible
                };

                var dataResponse = await _internshipProcessService.UpdateInternshipProcessVisibleForAdminDoetAsync(userGuideDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during internship process update visible status."
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
                var apiResponse = new ApiResponse<UpdateInternshipProcessForAdminDoetDTO>
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

        [HttpGet("download-file/{internshipProcessId}")]
        [Authorize(Roles = "Admin,DOET")]
        public async Task<IActionResult> DownloadInternshipProcessForAdminDoet(int? internshipProcessId)
        {
            try
            {
                if (!internshipProcessId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "InternshipProcessId is required."
                    });
                }

                var dataResponse = await _internshipProcessService.GetInternshipProcessDetailByIdForAdminDoetAsync(internshipProcessId.Value);

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
                var pdfFilePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.FilePath.TrimStart('/'));

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
