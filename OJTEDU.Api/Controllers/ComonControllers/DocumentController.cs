using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.DocumentDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/common/documents")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _contractService;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentController(IDocumentService contractService, IWebHostEnvironment webHostEnvironment)
        {
            _contractService = contractService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllDocuments(string? title, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                string role = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.Role)?.Value : "guest";

                var dataResponse = await _contractService.GetAllDocumentsAsync(role, title, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<DocumentListForCommonDTO>>>
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

        [HttpGet("details/{documentId}")]
        public async Task<IActionResult> GetDocumentDetail(int? documentId)
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

                string role = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.Role)?.Value : "guest";

                var dataResponse = await _contractService.GetDocumentDetailAsync(documentId.Value, role);

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

                var apiResponse = new ApiResponse<DocumentDetailForCommonDTO>
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
        public async Task<IActionResult> DownloadDocument(int? documentId)
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

                string role = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.Role)?.Value : "guest";

                var dataResponse = await _contractService.GetDocumentDetailAsync(documentId.Value, role);

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

                // Xây dựng đường dẫn đến file
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.DocumentFile.TrimStart('/'));

                // Kiểm tra nếu file không tồn tại trên server
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "File not found on the server."
                    });
                }

                // Đọc nội dung file
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Trả về file dưới dạng stream để tải xuống
                return File(fileBytes, "application/octet-stream", dataResponse.Data.DocumentFile);
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
