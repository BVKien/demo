using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static OJTEDU.Api.Input.DOETControllers.NewsFaqController;
using static OJTEDU.Application.DTOs.NewsFaqDTO;

namespace OJTEDU.Api.Controllers.DOETControllers
{
    [Route("api/doet/news-faq")]
    [ApiController]
    public class NewsFaqController : ControllerBase
    {
        private readonly INewsFaqService _newsFaqService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NewsFaqController(INewsFaqService newsFaqService, IWebHostEnvironment webHostEnvironment)
        {
            _newsFaqService = newsFaqService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("parent-news/list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllParentNewsForDoet(string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _newsFaqService.GetAllParentNewsForDoetAsync(title, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ParentNewsListForDoetDTO>>>
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

        [HttpGet("parent-news/details/{newsId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetParentNewsDetailForDoet(int? newsId)
        {
            try
            {
                if (!newsId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "newsId is required."
                    });
                }

                var dataResponse = await _newsFaqService.GetParentNewsDetailByIdForDoetAsync(newsId.Value);

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

                var apiResponse = new ApiResponse<ParentNewsDetailForDoetDTO>
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

        [HttpPost("parent-news")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> AddParentNewsForDoet([FromForm] AddParentNewsRequestForDoet request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
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

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var forRoleIdsList = request.ForRoleIds
            .Split(',')
            .Select(id =>
            {
                int? parsedId = int.TryParse(id.Trim(), out int result) ? (int?)result : null;
                return parsedId;
            })
            .ToList();

                // Tạo tài liệu để lưu vào cơ sở dữ liệu
                var parentNewsDto = new AddParentNewsForDoetDTO
                {
                    UserId = int.Parse(createdByUserId),
                    Title = request.Title,
                    ForRoleIds = forRoleIdsList
                };

                var dataResponse = await _newsFaqService.AddParentNewsForDoetAsync(parentNewsDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent news add."
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

                var apiResponse = new ApiResponse<AddParentNewsForDoetDTO>
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

        [HttpPut("parent-news/{newsId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateParentNewsForDoet(int? newsId, [FromForm] UpdateParentNewsRequestForDoet request)
        {
            try
            {
                if (!newsId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "newsId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
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

                // Tìm bản tin cũ để lấy thông tin
                var existingParentNews = await _newsFaqService.GetParentNewsDetailByIdForDoetAsync(newsId.Value);
                if (existingParentNews == null || existingParentNews.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Parent News not found."
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

                var updateDto = new UpdateParentNewsForDoetDTO
                {
                    ParentNewsId = newsId.Value,
                    Title = request.Title,
                    ForRoleIds = forRoleIdsList
                };

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var dataResponse = await _newsFaqService.UpdateParentNewsForDoetAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent news update."
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
                var apiResponse = new ApiResponse<UpdateParentNewsForDoetDTO>
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

        [HttpDelete("parent-news/{newsId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> DeleteParentNewsForDoet(int? newsId)
        {
            try
            {
                if (!newsId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "newsId is required."
                    });
                }

                var parentNewsDto = new DeleteParentNewsForDoetDTO
                {
                    ParentNewsId = newsId.Value
                };

                var dataResponse = await _newsFaqService.DeleteParentNewsForDoetAsync(parentNewsDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent news delete."
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
                var apiResponse = new ApiResponse<DeleteParentNewsForDoetDTO>
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

        [HttpPatch("parent-news/{newsId}/status")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateParentNewsStatusForDoet(int? newsId, UpdateParentNewsStatusRequestForDoet request)
        {
            try
            {
                if (!newsId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "newsId is required."
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

                var parentNewsDto = new UpdateParentNewsStatusForDoetDTO
                {
                    ParentNewsId = newsId.Value,
                    Status = request.Status
                };

                var dataResponse = await _newsFaqService.UpdateParentNewsStatusForDoetAsync(parentNewsDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent news update status."
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
                var apiResponse = new ApiResponse<UpdateParentNewsStatusForDoetDTO>
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

        [HttpGet("parent-news/status-list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllStatusesNewsForDoet()
        {
            try
            {
                var dataResponse = await _newsFaqService.GetAllStatusesNewsForDoetAsync();

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

                var apiResponse = new ApiResponse<List<StatusNewsListForDoetDTO>>
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

        [HttpGet("child-news/list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllChildNewsForDoet(int? parentId, string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                //if (!parentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                //{
                //    return BadRequest(new ApiResponse<object>
                //    {
                //        Data = null,
                //        Message = "parentId is required."
                //    });
                //}

                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _newsFaqService.GetAllChildNewsForDoetAsync(parentId.Value, title, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ChildNewsListForDoetDTO>>>
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

        [HttpGet("child-news/details/{newsId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetChildNewsDetailForDoet(int? newsId)
        {
            try
            {
                if (!newsId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "newsId is required."
                    });
                }

                var dataResponse = await _newsFaqService.GetChildNewsDetailByIdForDoetAsync(newsId.Value);

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

                var apiResponse = new ApiResponse<ChildNewsDetailForDoetDTO>
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

        [HttpPost("child-news")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> AddChildNewsForDoet([FromForm] AddChildNewsRequestForDoet request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
                }

                if (string.IsNullOrWhiteSpace(request.ChildNewscontent))
                {
                    errorMessages.Add("ChildNewscontent is required.");
                }

                if (request.Image == null || request.Image.Length == 0)
                {
                    errorMessages.Add("Image is required.");
                }
                else
                {
                    // Kiểm tra phần mở rộng file
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    string fileExtension = Path.GetExtension(request.Image.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        errorMessages.Add("Only image files with extensions .jpg, .jpeg, .png, .gif, .bmp, .webp are allowed.");
                    }

                    // Giới hạn dung lượng file (tối đa 10MB)
                    long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                    if (request.Image.Length > maxFileSizeInBytes)
                    {
                        errorMessages.Add("Image size must not exceed 10MB.");
                    }
                }

                if (request.ParentNewsId <= 0)
                {
                    errorMessages.Add("ParentNewsId is required and must be a valid number.");
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
                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Tạo tên file duy nhất
                string fileName = request.Image.FileName;
                string uniqueFileName = $"{createdByUserId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{fileName}";

                string newsPath = Path.Combine(_webHostEnvironment.WebRootPath, "news");

                // Kiểm tra xem thư mục tồn tại chưa, nếu không có thì tạo mới
                if (!Directory.Exists(newsPath))
                {
                    Directory.CreateDirectory(newsPath);
                }

                // Tạo đường dẫn đầy đủ đến tệp tin
                string filePath = Path.Combine(newsPath, uniqueFileName);

                // Lưu tệp tin vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(fileStream);
                }

                var relativeImagePath = $"/news/{uniqueFileName}";

                // Tạo tài liệu để lưu vào cơ sở dữ liệu
                var childNewsDto = new AddChildNewsForDoetDTO
                {
                    UserId = int.Parse(createdByUserId),
                    Title = request.Title,
                    ChildNewscontent = request.ChildNewscontent,
                    Image = relativeImagePath, // Lưu tên file vào cơ sở dữ liệu
                    ParentId = request.ParentNewsId
                };

                var dataResponse = await _newsFaqService.AddChildNewsForDoetAsync(childNewsDto);

                if (dataResponse == null)
                {
                    // Xóa file nếu có lỗi
                    System.IO.File.Delete(filePath);

                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child news add."
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

                var apiResponse = new ApiResponse<AddChildNewsForDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                // Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "news", request.Image.FileName)))
                {
                    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "news", request.Image.FileName));
                }
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpPut("child-news/{newsId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateChildNewsForDoet(int? newsId, [FromForm] UpdateChildNewsRequestForDoet request)
        {
            try
            {
                if (!newsId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "newsId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
                }

                if (string.IsNullOrWhiteSpace(request.ChildNewscontent))
                {
                    errorMessages.Add("ChildNewscontent is required.");
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

                // Tìm bản tin cũ để lấy thông tin
                var existingChildNews = await _newsFaqService.GetChildNewsDetailByIdForDoetAsync(newsId.Value);
                if (existingChildNews == null || existingChildNews.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Child News not found."
                    });
                }

                // Cập nhật thông tin cho DTO mới
                var updateDto = new UpdateChildNewsForDoetDTO
                {
                    ChildNewsId = newsId.Value,
                    Title = request.Title,
                    ChildNewscontent = request.ChildNewscontent,
                    Image = existingChildNews.Data.Image // Giữ lại ảnh cũ để có thể xóa sau
                };

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                string filePath = null;

                // Xử lý ảnh mới nếu có
                if (request.Image != null && request.Image.Length > 0)
                {
                    // Kiểm tra phần mở rộng file (chỉ chấp nhận các định dạng ảnh)
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    string fileExtension = Path.GetExtension(request.Image.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        errorMessages.Add("Only image files with extensions .jpg, .jpeg, .png, .gif, .bmp, .webp are allowed.");
                    }

                    // Giới hạn dung lượng file (tối đa 10MB)
                    long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                    if (request.Image.Length > maxFileSizeInBytes)
                    {
                        errorMessages.Add("Image size must not exceed 10MB.");
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

                    string uniqueFileName = $"{createdByUserId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{request.Image.FileName}";
                    string newsPath = Path.Combine(_webHostEnvironment.WebRootPath, "news");

                    if (!Directory.Exists(newsPath))
                    {
                        Directory.CreateDirectory(newsPath);
                    }

                    filePath = Path.Combine(newsPath, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Image.CopyToAsync(fileStream);
                    }

                    // Xóa ảnh cũ nếu có
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, updateDto.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    var relativeImagePath = $"/news/{uniqueFileName}";

                    // Cập nhật tên ảnh mới trong DTO
                    updateDto.Image = relativeImagePath;
                }

                var dataResponse = await _newsFaqService.UpdateChildNewsForDoetAsync(updateDto);

                if (dataResponse == null)
                {
                    // Kiểm tra và xóa file nếu nó tồn tại
                    if (filePath != null && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child news update."
                    });
                }

                if (dataResponse.Data == null)
                {
                    // Kiểm tra và xóa file nếu nó tồn tại
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
                var apiResponse = new ApiResponse<UpdateChildNewsForDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                // Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "news", request.Image.FileName)))
                {
                    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "news", request.Image.FileName));
                }
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpDelete("child-news/{newsId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> DeleteChildNewsForDoet(int? newsId)
        {
            try
            {
                if (!newsId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "newsId is required."
                    });
                }

                var childNewsDto = new DeleteChildNewsForDoetDTO
                {
                    ChildNewsId = newsId.Value
                };

                var dataResponse = await _newsFaqService.DeleteChildNewsForDoetAsync(childNewsDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child news delete."
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
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.Image.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<DeleteChildNewsForDoetDTO>
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

        [HttpPatch("child-news/{newsId}/status")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateChildNewsStatusForAdmin(int? newsId, UpdateChildNewsStatusRequestForDoet request)
        {
            try
            {
                if (!newsId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "newsId is required."
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

                var childNewsDto = new UpdateChildNewsStatusForDoetDTO
                {
                    ChildNewsId = newsId.Value,
                    Status = request.Status
                };

                var dataResponse = await _newsFaqService.UpdateChildNewsStatusForDoetAsync(childNewsDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child news update status."
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
                var apiResponse = new ApiResponse<UpdateChildNewsStatusForDoetDTO>
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


        [HttpGet("parent-faq/list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllParentFaqForDoet(string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _newsFaqService.GetAllParentFaqForDoetAsync(title, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ParentFaqListForDoetDTO>>>
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

        [HttpGet("parent-faq/details/{faqId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetParentFaqDetailForDoet(int? faqId)
        {
            try
            {
                if (!faqId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "faqId is required."
                    });
                }

                var dataResponse = await _newsFaqService.GetParentFaqDetailByIdForDoetAsync(faqId.Value);

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

                var apiResponse = new ApiResponse<ParentFaqDetailForDoetDTO>
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

        [HttpPost("parent-faq")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> AddParentFaqForDoet([FromForm] AddParentFaqRequestForDoet request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
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

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var forRoleIdsList = request.ForRoleIds
           .Split(',')
           .Select(id =>
           {
               int? parsedId = int.TryParse(id.Trim(), out int result) ? (int?)result : null;
               return parsedId;
           })
           .ToList();

                // Tạo tài liệu để lưu vào cơ sở dữ liệu
                var parentFaqDto = new AddParentFaqForDoetDTO
                {
                    UserId = int.Parse(createdByUserId),
                    Title = request.Title,
                    ForRoleIds = forRoleIdsList
                };

                var dataResponse = await _newsFaqService.AddParentFaqForDoetAsync(parentFaqDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent faq add."
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

                var apiResponse = new ApiResponse<AddParentFaqForDoetDTO>
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

        [HttpPut("parent-faq/{faqId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateParentFaqForDoet(int? faqId, [FromForm] UpdateParentFaqRequestForDoet request)
        {
            try
            {
                if (!faqId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "faqId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
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

                var existingParentFaq = await _newsFaqService.GetParentFaqDetailByIdForDoetAsync(faqId.Value);
                if (existingParentFaq == null || existingParentFaq.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Parent Faq not found."
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

                var updateDto = new UpdateParentFaqForDoetDTO
                {
                    ParentFaqId = faqId.Value,
                    Title = request.Title,
                    ForRoleIds = forRoleIdsList
                };

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var dataResponse = await _newsFaqService.UpdateParentFaqForDoetAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent faq update."
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
                var apiResponse = new ApiResponse<UpdateParentFaqForDoetDTO>
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

        [HttpDelete("parent-faq/{faqId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> DeleteParentFaqForDoet(int? faqId)
        {
            try
            {
                if (!faqId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "faqId is required."
                    });
                }

                var parentFaqDto = new DeleteParentFaqForDoetDTO
                {
                    ParentFaqId = faqId.Value
                };

                var dataResponse = await _newsFaqService.DeleteParentFaqForDoetAsync(parentFaqDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent faq delete."
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
                var apiResponse = new ApiResponse<DeleteParentFaqForDoetDTO>
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

        [HttpPatch("parent-faq/{faqId}/status")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateParentFaqStatusForDoet(int? faqId, UpdateParentFaqStatusRequestForDoet request)
        {
            try
            {
                if (!faqId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "faqId is required."
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

                var parentFaqDto = new UpdateParentFaqStatusForDoetDTO
                {
                    ParentFaqId = faqId.Value,
                    Status = request.Status
                };

                var dataResponse = await _newsFaqService.UpdateParentFaqStatusForDoetAsync(parentFaqDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent faq update status."
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
                var apiResponse = new ApiResponse<UpdateParentFaqStatusForDoetDTO>
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

        [HttpGet("parent-faq/status-list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllStatusesFaqForDoet()
        {
            try
            {
                var dataResponse = await _newsFaqService.GetAllStatusesFaqForDoetAsync();

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

                var apiResponse = new ApiResponse<List<StatusFaqListForDoetDTO>>
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

        [HttpGet("child-faq/list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllChildFaqForDoet(int? parentId, string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                //if (!parentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                //{
                //    return BadRequest(new ApiResponse<object>
                //    {
                //        Data = null,
                //        Message = "parentId is required."
                //    });
                //}

                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _newsFaqService.GetAllChildFaqForDoetAsync(parentId.Value, title, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ChildFaqListForDoetDTO>>>
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

        [HttpGet("child-faq/details/{faqId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetChildFaqDetailForDoet(int? faqId)
        {
            try
            {
                if (!faqId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "faqId is required."
                    });
                }

                var dataResponse = await _newsFaqService.GetChildFaqDetailByIdForDoetAsync(faqId.Value);

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

                var apiResponse = new ApiResponse<ChildFaqDetailForDoetDTO>
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

        [HttpPost("child-faq")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> AddChildFaqForDoet([FromForm] AddChildFaqRequestForDoet request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
                }

                if (string.IsNullOrWhiteSpace(request.ChildFaqcontent))
                {
                    errorMessages.Add("ChildFaqcontent is required.");
                }

                if (request.Image == null || request.Image.Length == 0)
                {
                    errorMessages.Add("Image is required.");
                }
                else
                {
                    // Kiểm tra phần mở rộng file
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    string fileExtension = Path.GetExtension(request.Image.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        errorMessages.Add("Only image files with extensions .jpg, .jpeg, .png, .gif, .bmp, .webp are allowed.");
                    }

                    // Giới hạn dung lượng file (tối đa 10MB)
                    long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                    if (request.Image.Length > maxFileSizeInBytes)
                    {
                        errorMessages.Add("Image size must not exceed 10MB.");
                    }
                }

                if (request.ParentFaqId <= 0)
                {
                    errorMessages.Add("ParentFaqId is required and must be a valid number.");
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

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Tạo tên file duy nhất
                string fileName = request.Image.FileName;
                string uniqueFileName = $"{createdByUserId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{fileName}";

                string faqPath = Path.Combine(_webHostEnvironment.WebRootPath, "faqs");

                // Kiểm tra xem thư mục tồn tại chưa, nếu không có thì tạo mới
                if (!Directory.Exists(faqPath))
                {
                    Directory.CreateDirectory(faqPath);
                }

                // Tạo đường dẫn đầy đủ đến tệp tin
                string filePath = Path.Combine(faqPath, uniqueFileName);

                // Lưu tệp tin vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(fileStream);
                }

                var relativeImagePath = $"/faqs/{uniqueFileName}";

                var childFaqDto = new AddChildFaqForDoetDTO
                {
                    UserId = int.Parse(createdByUserId),
                    Title = request.Title,
                    ChildFaqcontent = request.ChildFaqcontent,
                    Image = relativeImagePath, // Lưu tên file vào cơ sở dữ liệu
                    ParentId = request.ParentFaqId
                };

                var dataResponse = await _newsFaqService.AddChildFaqForDoetAsync(childFaqDto);

                if (dataResponse == null)
                {
                    // Xóa file nếu có lỗi
                    System.IO.File.Delete(filePath);

                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child faq add."
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

                var apiResponse = new ApiResponse<AddChildFaqForDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                // Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "faqs", request.Image.FileName)))
                {
                    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "faqs", request.Image.FileName));
                }
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpPut("child-faq/{faqId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateChildFaqForDoet(int? faqId, [FromForm] UpdateChildFaqRequestForDoet request)
        {
            try
            {
                if (!faqId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "faqId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    errorMessages.Add("Title is required.");
                }

                if (string.IsNullOrWhiteSpace(request.ChildFaqcontent))
                {
                    errorMessages.Add("ChildNewscontent is required.");
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

                // Tìm bản tin cũ để lấy thông tin
                var existingChildFaq = await _newsFaqService.GetChildFaqDetailByIdForDoetAsync(faqId.Value);
                if (existingChildFaq == null || existingChildFaq.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Child Faq not found."
                    });
                }

                // Cập nhật thông tin cho DTO mới
                var updateDto = new UpdateChildFaqForDoetDTO
                {
                    ChildFaqId = faqId.Value,
                    Title = request.Title,
                    ChildFaqcontent = request.ChildFaqcontent,
                    Image = existingChildFaq.Data.Image // Giữ lại ảnh cũ để có thể xóa sau
                };

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                string filePath = null;

                // Xử lý ảnh mới nếu có
                if (request.Image != null && request.Image.Length > 0)
                {
                    // Kiểm tra phần mở rộng file (chỉ chấp nhận các định dạng ảnh)
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    string fileExtension = Path.GetExtension(request.Image.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        errorMessages.Add("Only image files with extensions .jpg, .jpeg, .png, .gif, .bmp, .webp are allowed.");
                    }

                    // Giới hạn dung lượng file (tối đa 10MB)
                    long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                    if (request.Image.Length > maxFileSizeInBytes)
                    {
                        errorMessages.Add("Image size must not exceed 10MB.");
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

                    string uniqueFileName = $"{createdByUserId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{request.Image.FileName}";
                    string faqPath = Path.Combine(_webHostEnvironment.WebRootPath, "faqs");

                    if (!Directory.Exists(faqPath))
                    {
                        Directory.CreateDirectory(faqPath);
                    }

                    filePath = Path.Combine(faqPath, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Image.CopyToAsync(fileStream);
                    }

                    // Xóa ảnh cũ nếu có
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, updateDto.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    var relativeImagePath = $"/faqs/{uniqueFileName}";

                    // Cập nhật tên ảnh mới trong DTO
                    updateDto.Image = relativeImagePath;
                }

                var dataResponse = await _newsFaqService.UpdateChildFaqForDoetAsync(updateDto);

                if (dataResponse == null)
                {
                    // Kiểm tra và xóa file nếu nó tồn tại
                    if (filePath != null && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child faq update."
                    });
                }

                if (dataResponse.Data == null)
                {
                    // Kiểm tra và xóa file nếu nó tồn tại
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
                var apiResponse = new ApiResponse<UpdateChildFaqForDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                // Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "faqs", request.Image.FileName)))
                {
                    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "faqs", request.Image.FileName));
                }
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpDelete("child-faq/{faqId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> DeleteChildFaqForDoet(int? faqId)
        {
            try
            {
                if (!faqId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "faqId is required."
                    });
                }

                var childFaqDto = new DeleteChildFaqForDoetDTO
                {
                    ChildFaqId = faqId.Value
                };

                var dataResponse = await _newsFaqService.DeleteChildFaqForDoetAsync(childFaqDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child faq delete."
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
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.Image.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<DeleteChildFaqForDoetDTO>
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

        [HttpPatch("child-faq/{faqId}/status")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateChildFaqStatusForDoet(int? faqId, UpdateChildNewsStatusRequestForDoet request)
        {
            try
            {
                if (!faqId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "faqId is required."
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

                var childFaqDto = new UpdateChildFaqStatusForDoetDTO
                {
                    ChildFaqId = faqId.Value,
                    Status = request.Status
                };

                var dataResponse = await _newsFaqService.UpdateChildFaqStatusForDoetAsync(childFaqDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child faq update status."
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
                var apiResponse = new ApiResponse<UpdateChildFaqStatusForDoetDTO>
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
