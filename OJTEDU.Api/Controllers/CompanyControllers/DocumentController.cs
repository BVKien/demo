using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Api.Input.CompanyControllers.DocumentController;
using static OJTEDU.Application.DTOs.DocumentDTO;

namespace OJTEDU.Api.Controllers.CompanyControllers
{
    [Route("api/company/document")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [Authorize(Roles = "Company")]
        [HttpGet("test-file/list")]
        public async Task<IActionResult> GetAllTestfilesDocument()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _documentService.GetAllDocumentsByUserIdAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<DocumentTestFilesListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<DocumentTestFilesListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<DocumentTestFilesListForCompanyDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<DocumentTestFilesListForCompanyDTO>>
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
                    Message = "An error occurred while get test files list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPost("files/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/documents/files/");
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

        [Authorize(Roles = "Company")]
        [HttpPost("test-file/create")]
        public async Task<IActionResult> CreateTestFileDocument([FromBody] CreateTestFileDocumentInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var filePath = Path.Combine("wwwroot/uploads/documents/files/", input.DocumentFile);

                // Initialize 
                byte[]? fileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.DocumentFile) && System.IO.File.Exists(filePath))
                {
                    fileData = await System.IO.File.ReadAllBytesAsync(filePath);
                }

                var testDocInfoDto = new CreateDocumentTestFilesForCompanyDTO
                {
                    Title = input?.Title,
                    Description = input?.Description,
                };

                var apiResponse = await _documentService.CreateDocumentsByUserIdAsync(userId, input?.DocumentFile, fileData, testDocInfoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateDocumentTestFilesForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateDocumentTestFilesForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateDocumentTestFilesForCompanyDTO>
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

                return Ok(new ApiResponse<CreateDocumentTestFilesForCompanyDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while create test file.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPut("test-file/update/{documentId}")]
        public async Task<IActionResult> UpdateTestFileDocument(int? documentId, [FromBody] UpdateTestFileDocumentInput? input)
        {
            try
            {
                var filePath = Path.Combine("wwwroot/uploads/documents/files/", input.DocumentFile);

                // Initialize 
                byte[]? fileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.DocumentFile) && System.IO.File.Exists(filePath))
                {
                    fileData = await System.IO.File.ReadAllBytesAsync(filePath);
                }

                var testDocInfoDto = new UpdateDocumentTestFilesForCompanyDTO
                {
                    Title = input?.Title,
                    Description = input?.Description,
                };

                var apiResponse = await _documentService.UpdateDocumentAsync(documentId, input?.DocumentFile, fileData, testDocInfoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<UpdateDocumentTestFilesForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<UpdateDocumentTestFilesForCompanyDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<UpdateDocumentTestFilesForCompanyDTO>
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

                return Ok(new ApiResponse<UpdateDocumentTestFilesForCompanyDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while update test file.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Company")]
        [HttpPut("test-file/delete-stored/{documentId}")]
        public async Task<IActionResult> DeletedForStoredTestFileDocument(int? documentId)
        {
            try
            {
                var apiResponse = await _documentService.StoredDocumentsByUserIdAsync(documentId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while delete test file.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}