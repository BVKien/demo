using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static OJTEDU.Api.Input.AdminControllers.DocumentController;
using static OJTEDU.Application.DTOs.DocumentDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin/documents")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _contractService;
        //private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<DocumentController> _logger;
        private readonly IJobService _jobService;

        public DocumentController(IDocumentService contractService,
            ILogger<DocumentController> logger, IJobService jobService)
        {
            _contractService = contractService;
            _logger = logger;
            _jobService = jobService;
        }

        //[HttpGet("list")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> GetAllDocumentsForAdmin(string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        //{
        //    try
        //    {
        //        int actualPageNumber = pageNumber ?? 1;
        //        int actualPageSize = pageSize ?? 15;

        //        var dataResponse = await _contractService.GetAllDocumentsForAdminAsync(title, roleId, status, actualPageNumber, actualPageSize);

        //        if (dataResponse == null)
        //        {
        //            return StatusCode(500, new ApiResponse<object>
        //            {
        //                Data = null,
        //                Message = "Unexpected error occurred."
        //            });
        //        }

        //        if (dataResponse.Data == null)
        //        {
        //            return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
        //            {
        //                Data = null,
        //                Message = dataResponse.Message
        //            });
        //        }

        //        var apiResponse = new ApiResponse<PagedResponse<List<DocumentListForAdminDTO>>>
        //        {
        //            Data = dataResponse.Data,
        //            Message = dataResponse.Message
        //        };

        //        return Ok(apiResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<object>
        //        {
        //            Data = null,
        //            Message = $"Internal Server Error: {ex.Message}"
        //        });
        //    }
        //}

        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDocumentsForAdmin(string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            _logger.LogInformation("GetAllDocumentsForAdmin called with parameters: Title={Title}, RoleId={RoleId}, Status={Status}, PageNumber={PageNumber}, PageSize={PageSize}",
                title, roleId, status, pageNumber, pageSize);

            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                _logger.LogDebug("Defaulting actualPageNumber to {PageNumber} and actualPageSize to {PageSize}", actualPageNumber, actualPageSize);

                var dataResponse = await _jobService.GetAllDocumentsForAdminAsync(title, roleId, status, actualPageNumber, actualPageSize);

                if (dataResponse == null)
                {
                    _logger.LogError("Unexpected error: dataResponse is null");
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    _logger.LogWarning("No data found: StatusCode={StatusCode}, Message={Message}", dataResponse.StatusCode, dataResponse.Message);
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                _logger.LogInformation("Successfully retrieved documents: TotalCount={TotalCount}", dataResponse.Data.TotalCount);

                var apiResponse = new ApiResponse<PagedResponse<List<DocumentListForAdminDTO>>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred in GetAllDocumentsForAdmin");
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("details/{documentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDocumentDetailForAdmin(int? documentId)
        {
            try
            {
                if (!documentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Id is required."
                    });
                }

                var dataResponse = await _contractService.GetDocumentDetailByIdForAdminAsync(documentId.Value);

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
                var apiResponse = new ApiResponse<DocumentDetailForAdminDTO>
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
        public async Task<IActionResult> AddDocumentForAdmin([FromForm] AddDocumentRequestForAdmin request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
                }
                else if (request.Title.Length > 100)
                {
                    errorMessages.Add("Title must not exceed 100 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    errorMessages.Add("Description is required.");
                }

                if (request.DocumentFile == null)
                {
                    errorMessages.Add("DocumentFile is required.");
                }
                //else
                //{
                //    // Giới hạn dung lượng file (tối đa 10MB)
                //    long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                //    if (request.DocumentFile.Length > maxFileSizeInBytes)
                //    {
                //        errorMessages.Add("File size must not exceed 10MB.");
                //    }
                //}

                //if (request.ForRoleIds == null || !request.ForRoleIds.Any())
                //{
                //    errorMessages.Add("At least one role is required.");
                //}

                if (string.IsNullOrWhiteSpace(request.ForRoleIds))
                {
                    errorMessages.Add("At least one role is required.");
                }
                else
                {
                    // Kiểm tra xem `ForRoleIds` có chỉ chứa số hay không
                    if (!Regex.IsMatch(request.ForRoleIds, @"^(\d+\s*,\s*)*\d+$"))
                    {
                        errorMessages.Add("ForRoleIds must only contain numbers separated by commas.");
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

                string createdByUniversityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //// Tạo tên file duy nhất
                //string fileName = request.DocumentFile.FileName;
                //string uniqueFileName = $"{createdByUniversityId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{fileName}";

                //// Lấy đường dẫn đến thư mục wwwroot/documents
                //string documentsPath = Path.Combine(_webHostEnvironment.WebRootPath, "documents");

                //// Kiểm tra xem thư mục tồn tại chưa, nếu không có thì tạo mới
                //if (!Directory.Exists(documentsPath))
                //{
                //    Directory.CreateDirectory(documentsPath);
                //}

                //// Tạo đường dẫn đầy đủ đến tệp tin
                //string filePath = Path.Combine(documentsPath, uniqueFileName);

                //// Lưu tệp tin vào thư mục
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await request.DocumentFile.CopyToAsync(fileStream);
                //}

                //var relativeDocumentPath = $"/documents/{uniqueFileName}";

                var forRoleIdsList = request.ForRoleIds
            .Split(',')
            .Select(id =>
            {
                int? parsedId = int.TryParse(id.Trim(), out int result) ? (int?)result : null;
                return parsedId;
            })
            .ToList();

                // Tạo tài liệu để lưu vào cơ sở dữ liệu
                var documentDto = new AddDocumentForAdminDTO
                {
                    UniversityId = int.Parse(createdByUniversityId),
                    Title = request.Title,
                    Description = request.Description,
                    DocumentFile = request.DocumentFile, // Lưu tên file vào cơ sở dữ liệu
                    ForRoleIds = forRoleIdsList
                };

                var dataResponse = await _contractService.AddDocumentForAdminAsync(documentDto);

                if (dataResponse == null)
                {
                    // Xóa file nếu có lỗi
                    //System.IO.File.Delete(filePath);

                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during document add."
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
                var apiResponse = new ApiResponse<AddDocumentForAdminDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                //// Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                //if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "documents", request.DocumentFile.FileName)))
                //{
                //    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "documents", request.DocumentFile.FileName));
                //}
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }


        [HttpPut("{documentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDocumentForAdmin(int? documentId, [FromForm] UpdateDocumentRequestForAdmin request)
        {
            try
            {
                if (!documentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "documentId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
                }
                else if (request.Title.Length > 100)
                {
                    errorMessages.Add("Title must not exceed 100 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    errorMessages.Add("Description is required.");
                }

                //if (request.ForRoleIds == null || !request.ForRoleIds.Any())
                //{
                //    errorMessages.Add("At least one role is required.");
                //}

                if (string.IsNullOrWhiteSpace(request.ForRoleIds))
                {
                    errorMessages.Add("At least one role is required.");
                }
                else
                {
                    // Kiểm tra xem `ForRoleIds` có chỉ chứa số hay không
                    if (!Regex.IsMatch(request.ForRoleIds, @"^(\d+\s*,\s*)*\d+$"))
                    {
                        errorMessages.Add("ForRoleIds must only contain numbers separated by commas.");
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

                var existingDocuments = await _contractService.GetDocumentDetailByIdForAdminAsync(documentId.Value);
                if (existingDocuments == null || existingDocuments.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Document not found."
                    });
                }

                var forRoleIdsList = request.ForRoleIds
            .Split(',')
            .Select(id =>
            {
                int? parsedId = int.TryParse(id.Trim(), out int result) ? (int?)result : null;
                return parsedId;
            })
            .ToList();

                var documentDto = new UpdateDocumentForAdminDTO
                {
                    DocumentId = documentId.Value,
                    Title = request.Title,
                    Description = request.Description,
                    DocumentFile = existingDocuments.Data.DocumentFile,
                    ForRoleIds = forRoleIdsList
                };

                string createdByUniversityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                string filePath = null;

                if (request.DocumentFile != null)
                {

                    //// Giới hạn dung lượng file (tối đa 10MB)
                    //long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                    //if (request.DocumentFile.Length > maxFileSizeInBytes)
                    //{
                    //    errorMessages.Add("File size must not exceed 10MB.");
                    //}

                    //// Nếu có lỗi, trả về phản hồi lỗi
                    //if (errorMessages.Any())
                    //{
                    //    return BadRequest(new ApiResponse<object>
                    //    {
                    //        Data = null,
                    //        Message = $"Validation errors occurred: {string.Join(", ", errorMessages)}"
                    //    });
                    //}

                    //string uniqueFileName = $"{createdByUniversityId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{request.DocumentFile.FileName}";
                    //string documentsPath = Path.Combine(_webHostEnvironment.WebRootPath, "documents");


                    //if (!Directory.Exists(documentsPath))
                    //{
                    //    Directory.CreateDirectory(documentsPath);
                    //}

                    //filePath = Path.Combine(documentsPath, uniqueFileName);
                    //using (var fileStream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    await request.DocumentFile.CopyToAsync(fileStream);
                    //}

                    //string oldDocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, documentDto.DocumentFile.TrimStart('/'));
                    //if (System.IO.File.Exists(oldDocumentPath))
                    //{
                    //    System.IO.File.Delete(oldDocumentPath);
                    //}

                    //var relativeDocumentPath = $"/documents/{uniqueFileName}";

                    // Cập nhật tên ảnh mới trong DTO
                    documentDto.DocumentFile = request.DocumentFile;
                }

                var dataResponse = await _contractService.UpdateDocumentForAdminAsync(documentDto);

                if (dataResponse == null)
                {
                    // Xóa file nếu có lỗi
                    if (filePath != null && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during user update."
                    });
                }

                if (dataResponse.Data == null)
                {
                    // Xóa file nếu có lỗi
                    if (filePath != null && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<UpdateDocumentForAdminDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                //// Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                //if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "documents", request.DocumentFile.FileName)))
                //{
                //    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "documents", request.DocumentFile.FileName));
                //}
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpDelete("{documentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDocumentForAdmin(int? documentId)
        {
            try
            {
                if (!documentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "documentId is required."
                    });
                }

                var documentDto = new DeleteDocumentForAdminDTO
                {
                    DocumentId = documentId.Value
                };

                // Gọi dịch vụ để xóa người dùng
                var dataResponse = await _contractService.DeleteDocumentForAdminAsync(documentDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during document delete."
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

                //// Xóa tệp tin vật lý khỏi thư mục nếu xóa trong cơ sở dữ liệu thành công
                //string filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.DocumentFile.TrimStart('/'));
                //if (System.IO.File.Exists(filePath))
                //{
                //    System.IO.File.Delete(filePath);
                //}

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<DeleteDocumentForAdminDTO>
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

        [HttpPatch("{documentId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDocumentStatusForAdmin(int? documentId, UpdateDocumentStatusRequestForAdmin request)
        {
            try
            {
                if (!documentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "documentId is required."
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

                var documentDto = new UpdateDocumentStatusForAdminDTO
                {
                    DocumentId = documentId.Value,
                    Status = request.Status
                };

                var dataResponse = await _contractService.UpdateDocumentStatusForAdminAsync(documentDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during document update status."
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
                var apiResponse = new ApiResponse<UpdateDocumentStatusForAdminDTO>
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
                var dataResponse = await _contractService.GetAllStatusesDocumentForAdminAsync();

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

                var apiResponse = new ApiResponse<List<StatusDocumentListForAdminDTO>>
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

        [HttpGet("download-file/{documentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadDocumentForAdmin(int? documentId)
        {
            try
            {
                if (!documentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "documentId is required."
                    });
                }

                var dataResponse = await _contractService.GetDocumentDetailByIdForAdminAsync(documentId.Value);

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

                //// Xây dựng đường dẫn đến file
                //var filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.DocumentFile.TrimStart('/'));

                //// Kiểm tra nếu file không tồn tại trên server
                //if (!System.IO.File.Exists(filePath))
                //{
                //    return NotFound(new ApiResponse<object>
                //    {
                //        Data = null,
                //        Message = "File not found on the server."
                //    });
                //}

                // Đọc nội dung file
                // var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Trả về file dưới dạng stream để tải xuống
                //return File(fileBytes, "application/octet-stream", dataResponse.Data.DocumentFile);
                return Ok(dataResponse.Data.DocumentFile);
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
