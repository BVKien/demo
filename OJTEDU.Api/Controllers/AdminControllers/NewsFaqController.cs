using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static OJTEDU.Api.Input.AdminControllers.NewsFaqController;
using static OJTEDU.Application.DTOs.NewsFaqDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin/news-faq")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllParentNewsForAdmin(string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _newsFaqService.GetAllParentNewsForAdminAsync(title, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ParentNewsListForAdminDTO>>>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetParentNewsDetailForAdmin(int? newsId)
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

                var dataResponse = await _newsFaqService.GetParentNewsDetailByIdForAdminAsync(newsId.Value);

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

                var apiResponse = new ApiResponse<ParentNewsDetailForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddParentNewsForAdmin([FromForm] AddParentNewsRequestForAdmin request)
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

                if (string.IsNullOrWhiteSpace(request.ParentNewscontent))
                {
                    errorMessages.Add("ParentNewscontent is required.");
                }

                if (request.Image == null || request.Image.Length == 0)
                {
                    errorMessages.Add("Image is required.");
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

                var forRoleIdsList = request.ForRoleIds
           .Split(',')
           .Select(id =>
           {
               int? parsedId = int.TryParse(id.Trim(), out int result) ? (int?)result : null;
               return parsedId;
           })
           .ToList();

                // Tạo tài liệu để lưu vào cơ sở dữ liệu
                var parentNewsDto = new AddParentNewsForAdminDTO
                {
                    UserId = int.Parse(createdByUserId),
                    Title = request.Title,
                    ParentNewscontent = request.ParentNewscontent,
                    Image = relativeImagePath, // Lưu tên file vào cơ sở dữ liệu
                    ForRoleIds = forRoleIdsList
                };

                var dataResponse = await _newsFaqService.AddParentNewsForAdminAsync(parentNewsDto);

                if (dataResponse == null)
                {
                    // Xóa file nếu có lỗi
                    System.IO.File.Delete(filePath);

                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent news add."
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

                var apiResponse = new ApiResponse<AddParentNewsForAdminDTO>
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

        [HttpPut("parent-news/{newsId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateParentNewsForAdmin(int? newsId, [FromForm] UpdateParentNewsRequestForAdmin request)
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

                if (string.IsNullOrWhiteSpace(request.ParentNewscontent))
                {
                    errorMessages.Add("ParentNewscontent is required.");
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
                var existingParentNews = await _newsFaqService.GetParentNewsDetailByIdForAdminAsync(newsId.Value);
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

                var updateDto = new UpdateParentNewsForAdminDTO
                {
                    ParentNewsId = newsId.Value,
                    Title = request.Title,
                    ParentNewscontent = request.ParentNewscontent,
                    Image = existingParentNews.Data.Image,
                    ForRoleIds = forRoleIdsList
                };

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                string filePath = null;

                // Xử lý ảnh mới nếu có
                if (request.Image != null && request.Image.Length > 0)
                {
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

                var dataResponse = await _newsFaqService.UpdateParentNewsForAdminAsync(updateDto);

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
                        Message = "Unexpected error occurred during parent news update."
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
                var apiResponse = new ApiResponse<UpdateParentNewsForAdminDTO>
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

        [HttpDelete("parent-news/{newsId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteParentNewsForAdmin(int? newsId)
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

                var parentNewsDto = new DeleteParentNewsForAdminDTO
                {
                    ParentNewsId = newsId.Value
                };

                var dataResponse = await _newsFaqService.DeleteParentNewsForAdminAsync(parentNewsDto);

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

                // Xóa tệp tin vật lý khỏi thư mục nếu xóa trong cơ sở dữ liệu thành công
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.Image.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Xóa tệp tin ảnh của ChildNews
                foreach (var child in dataResponse.Data.DeletedChildNews)
                {
                    string childFilePath = Path.Combine(_webHostEnvironment.WebRootPath, child.Image.TrimStart('/'));
                    if (System.IO.File.Exists(childFilePath))
                    {
                        System.IO.File.Delete(childFilePath);
                    }
                }

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<DeleteParentNewsForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateParentNewsStatusForAdmin(int? newsId, UpdateParentNewsStatusRequestForAdmin request)
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

                var parentNewsDto = new UpdateParentNewsStatusForAdminDTO
                {
                    ParentNewsId = newsId.Value,
                    Status = request.Status
                };

                var dataResponse = await _newsFaqService.UpdateParentNewsStatusForAdminAsync(parentNewsDto);

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
                var apiResponse = new ApiResponse<UpdateParentNewsStatusForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStatusesNewsForAdmin()
        {
            try
            {
                var dataResponse = await _newsFaqService.GetAllStatusesNewsForAdminAsync();

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

                var apiResponse = new ApiResponse<List<StatusNewsListForAdminDTO>>
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

        [HttpGet("child-news/list/{parentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllChildNewsForAdmin(int? parentId, string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                if (!parentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "parentId is required."
                    });
                }

                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _newsFaqService.GetAllChildNewsForAdminAsync(parentId.Value, title, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ChildNewsListForAdminDTO>>>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetChildNewsDetailForAdmin(int? newsId)
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

                var dataResponse = await _newsFaqService.GetChildNewsDetailByIdForAdminAsync(newsId.Value);

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

                var apiResponse = new ApiResponse<ChildNewsDetailForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddChildNewsForAdmin([FromForm] AddChildNewsRequestForAdmin request)
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
                var childNewsDto = new AddChildNewsForAdminDTO
                {
                    UserId = int.Parse(createdByUserId),
                    Title = request.Title,
                    ChildNewscontent = request.ChildNewscontent,
                    Image = relativeImagePath, // Lưu tên file vào cơ sở dữ liệu
                    ParentId = request.ParentNewsId
                };

                var dataResponse = await _newsFaqService.AddChildNewsForAdminAsync(childNewsDto);

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

                var apiResponse = new ApiResponse<AddChildNewsForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateChildNewsForAdmin(int? newsId, [FromForm] UpdateChildNewsRequestForAdmin request)
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
                var existingChildNews = await _newsFaqService.GetChildNewsDetailByIdForAdminAsync(newsId.Value);
                if (existingChildNews == null || existingChildNews.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Child News not found."
                    });
                }

                // Cập nhật thông tin cho DTO mới
                var updateDto = new UpdateChildNewsForAdminDTO
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

                var dataResponse = await _newsFaqService.UpdateChildNewsForAdminAsync(updateDto);

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
                var apiResponse = new ApiResponse<UpdateChildNewsForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChildNewsForAdmin(int? newsId)
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

                var childNewsDto = new DeleteChildNewsForAdminDTO
                {
                    ChildNewsId = newsId.Value
                };

                var dataResponse = await _newsFaqService.DeleteChildNewsForAdminAsync(childNewsDto);

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
                var apiResponse = new ApiResponse<DeleteChildNewsForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateChildNewsStatusForAdmin(int? newsId, UpdateChildNewsStatusRequestForAdmin request)
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

                var childNewsDto = new UpdateChildNewsStatusForAdminDTO
                {
                    ChildNewsId = newsId.Value,
                    Status = request.Status
                };

                var dataResponse = await _newsFaqService.UpdateChildNewsStatusForAdminAsync(childNewsDto);

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
                var apiResponse = new ApiResponse<UpdateChildNewsStatusForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllParentFaqForAdmin(string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _newsFaqService.GetAllParentFaqForAdminAsync(title, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ParentFaqListForAdminDTO>>>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetParentFaqDetailForAdmin(int? faqId)
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

                var dataResponse = await _newsFaqService.GetParentFaqDetailByIdForAdminAsync(faqId.Value);

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

                var apiResponse = new ApiResponse<ParentFaqDetailForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddParentFaqForAdmin([FromForm] AddParentFaqRequestForAdmin request)
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

                if (string.IsNullOrWhiteSpace(request.ParentFaqcontent))
                {
                    errorMessages.Add("ParentFaqcontent is required.");
                }

                if (request.Image == null || request.Image.Length == 0)
                {
                    errorMessages.Add("Image is required.");
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

                var forRoleIdsList = request.ForRoleIds
            .Split(',')
            .Select(id =>
            {
                int? parsedId = int.TryParse(id.Trim(), out int result) ? (int?)result : null;
                return parsedId;
            })
            .ToList();

                // Tạo tài liệu để lưu vào cơ sở dữ liệu
                var parentFaqDto = new AddParentFaqForAdminDTO
                {
                    UserId = int.Parse(createdByUserId),
                    Title = request.Title,
                    ParentFaqcontent = request.ParentFaqcontent,
                    Image = relativeImagePath, // Lưu tên file vào cơ sở dữ liệu
                    ForRoleIds = forRoleIdsList
                };

                var dataResponse = await _newsFaqService.AddParentFaqForAdminAsync(parentFaqDto);

                if (dataResponse == null)
                {
                    // Xóa file nếu có lỗi
                    System.IO.File.Delete(filePath);

                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent faq add."
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

                var apiResponse = new ApiResponse<AddParentFaqForAdminDTO>
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

        [HttpPut("parent-faq/{faqId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateParentFaqForAdmin(int? faqId, [FromForm] UpdateParentFaqRequestForAdmin request)
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

                if (string.IsNullOrWhiteSpace(request.ParentFaqcontent))
                {
                    errorMessages.Add("ParentFaqcontent is required.");
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

                var existingParentFaq = await _newsFaqService.GetParentFaqDetailByIdForAdminAsync(faqId.Value);
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

                var updateDto = new UpdateParentFaqForAdminDTO
                {
                    ParentFaqId = faqId.Value,
                    Title = request.Title,
                    ParentFaqcontent = request.ParentFaqcontent,
                    Image = existingParentFaq.Data.Image,
                    ForRoleIds = forRoleIdsList
                };

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                string filePath = null;

                // Xử lý ảnh mới nếu có
                if (request.Image != null && request.Image.Length > 0)
                {
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

                var dataResponse = await _newsFaqService.UpdateParentFaqForAdminAsync(updateDto);

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
                        Message = "Unexpected error occurred during parent faq update."
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
                var apiResponse = new ApiResponse<UpdateParentFaqForAdminDTO>
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

        [HttpDelete("parent-faq/{faqId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteParentFaqForAdmin(int? faqId)
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

                var parentFaqDto = new DeleteParentFaqForAdminDTO
                {
                    ParentFaqId = faqId.Value
                };

                var dataResponse = await _newsFaqService.DeleteParentFaqForAdminAsync(parentFaqDto);

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

                // Xóa tệp tin vật lý khỏi thư mục nếu xóa trong cơ sở dữ liệu thành công
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.Image.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                foreach (var child in dataResponse.Data.DeletedChildFaq)
                {
                    string childFilePath = Path.Combine(_webHostEnvironment.WebRootPath, child.Image.TrimStart('/'));
                    if (System.IO.File.Exists(childFilePath))
                    {
                        System.IO.File.Delete(childFilePath);
                    }
                }

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<DeleteParentFaqForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateParentFaqStatusForAdmin(int? faqId, UpdateParentFaqStatusRequestForAdmin request)
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

                var parentFaqDto = new UpdateParentFaqStatusForAdminDTO
                {
                    ParentFaqId = faqId.Value,
                    Status = request.Status
                };

                var dataResponse = await _newsFaqService.UpdateParentFaqStatusForAdminAsync(parentFaqDto);

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
                var apiResponse = new ApiResponse<UpdateParentFaqStatusForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStatusesFaqForAdmin()
        {
            try
            {
                var dataResponse = await _newsFaqService.GetAllStatusesFaqForAdminAsync();

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

                var apiResponse = new ApiResponse<List<StatusFaqListForAdminDTO>>
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

        [HttpGet("child-faq/list/{parentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllChildFaqForAdmin(int? parentId, string? title, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                if (!parentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "parentId is required."
                    });
                }

                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId.Value, title, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ChildFaqListForAdminDTO>>>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetChildFaqDetailForAdmin(int? faqId)
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

                var dataResponse = await _newsFaqService.GetChildFaqDetailByIdForAdminAsync(faqId.Value);

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

                var apiResponse = new ApiResponse<ChildFaqDetailForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddChildFaqForAdmin([FromForm] AddChildFaqRequestForAdmin request)
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

                var childFaqDto = new AddChildFaqForAdminDTO
                {
                    UserId = int.Parse(createdByUserId),
                    Title = request.Title,
                    ChildFaqcontent = request.ChildFaqcontent,
                    Image = relativeImagePath, // Lưu tên file vào cơ sở dữ liệu
                    ParentId = request.ParentFaqId
                };

                var dataResponse = await _newsFaqService.AddChildFaqForAdminAsync(childFaqDto);

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

                var apiResponse = new ApiResponse<AddChildFaqForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateChildFaqForAdmin(int? faqId, [FromForm] UpdateChildFaqRequestForAdmin request)
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
                var existingChildFaq = await _newsFaqService.GetChildFaqDetailByIdForAdminAsync(faqId.Value);
                if (existingChildFaq == null || existingChildFaq.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Child Faq not found."
                    });
                }

                // Cập nhật thông tin cho DTO mới
                var updateDto = new UpdateChildFaqForAdminDTO
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

                var dataResponse = await _newsFaqService.UpdateChildFaqForAdminAsync(updateDto);

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
                var apiResponse = new ApiResponse<UpdateChildFaqForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChildFaqForAdmin(int? faqId)
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

                var childFaqDto = new DeleteChildFaqForAdminDTO
                {
                    ChildFaqId = faqId.Value
                };

                var dataResponse = await _newsFaqService.DeleteChildFaqForAdminAsync(childFaqDto);

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
                var apiResponse = new ApiResponse<DeleteChildFaqForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateChildFaqStatusForAdmin(int? faqId, UpdateChildNewsStatusRequestForAdmin request)
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

                var childFaqDto = new UpdateChildFaqStatusForAdminDTO
                {
                    ChildFaqId = faqId.Value,
                    Status = request.Status
                };

                var dataResponse = await _newsFaqService.UpdateChildFaqStatusForAdminAsync(childFaqDto);

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
                var apiResponse = new ApiResponse<UpdateChildFaqStatusForAdminDTO>
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
