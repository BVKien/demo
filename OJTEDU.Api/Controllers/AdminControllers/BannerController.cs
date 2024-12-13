using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Api.Input.AdminControllers.BannerController;
using static OJTEDU.Application.DTOs.BannerDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin/banner")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BannerController(IBannerService bannerService, IWebHostEnvironment webHostEnvironment)
        {
            _bannerService = bannerService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBannerForAdmin(DateTime? startEventDate, DateTime? endEventDate, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _bannerService.GetAllBannerForAdminAsync(startEventDate, endEventDate, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<BannerListForAdminDTO>>>
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

        [HttpGet("details/{bannerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBannerDetailForAdmin(int? bannerId)
        {
            try
            {
                if (!bannerId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "bannerId is required."
                    });
                }

                var dataResponse = await _bannerService.GetBannerDetailByIdForAdminAsync(bannerId.Value);

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

                var apiResponse = new ApiResponse<BannerDetailForAdminDTO>
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
        public async Task<IActionResult> AddBannerForAdmin([FromForm] AddBannerRequestForAdmin request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (request.Image == null)
                {
                    errorMessages.Add("Image is required.");
                }
                //else
                //{
                //    // Kiểm tra phần mở rộng file
                //    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                //    string fileExtension = Path.GetExtension(request.Image.FileName).ToLower();

                //    if (!allowedExtensions.Contains(fileExtension))
                //    {
                //        errorMessages.Add("Only image files with extensions .jpg, .jpeg, .png, .gif, .bmp, .webp are allowed.");
                //    }

                //    // Giới hạn dung lượng file (tối đa 10MB)
                //    long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                //    if (request.Image.Length > maxFileSizeInBytes)
                //    {
                //        errorMessages.Add("Image size must not exceed 10MB.");
                //    }
                //}

                if (!request.EventDate.HasValue)
                {
                    errorMessages.Add("EventDate is required.");
                }
                else if (request.EventDate.Value < DateTime.Now)
                {
                    errorMessages.Add("EventDate cannot be in the past.");
                }

                // Kiểm tra liên kết (Link)
                if (string.IsNullOrWhiteSpace(request.Link))
                {
                    errorMessages.Add("Link is required.");
                }
                else
                {
                    Uri uriResult;
                    bool isValidUrl = Uri.TryCreate(request.Link, UriKind.Absolute, out uriResult) &&
                                      (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (!isValidUrl)
                    {
                        errorMessages.Add("Link is not a valid URL.");
                    }
                    else
                    {
                        // Lấy path từ URL (bỏ phần domain)
                        request.Link = uriResult.PathAndQuery;
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

                //// Tạo tên file duy nhất
                //string fileName = request.Image.FileName;
                //string uniqueFileName = $"{createdByUserId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{fileName}";

                //string bannerPath = Path.Combine(_webHostEnvironment.WebRootPath, "banners");

                //// Kiểm tra xem thư mục tồn tại chưa, nếu không có thì tạo mới
                //if (!Directory.Exists(bannerPath))
                //{
                //    Directory.CreateDirectory(bannerPath);
                //}

                //// Tạo đường dẫn đầy đủ đến tệp tin
                //string filePath = Path.Combine(bannerPath, uniqueFileName);

                //// Lưu tệp tin vào thư mục
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await request.Image.CopyToAsync(fileStream);
                //}

                //var relativeImagePath = $"/banners/{uniqueFileName}";

                var bannerDto = new AddBannerForAdminDTO
                {
                    UserId = int.Parse(createdByUserId),
                    Link = request.Link,
                    EventDate = request.EventDate,
                    Image = request.Image
                };

                var dataResponse = await _bannerService.AddBannerForAdminAsync(bannerDto);

                if (dataResponse == null)
                {
                    // Xóa file nếu có lỗi
                    // System.IO.File.Delete(filePath);

                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during banner add."
                    });
                }

                if (dataResponse.Data == null)
                {
                    // Xóa file nếu có lỗi
                    // System.IO.File.Delete(filePath);

                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                var apiResponse = new ApiResponse<AddBannerForAdminDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                //// Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                //if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "banners", request.Image.FileName)))
                //{
                //    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "banners", request.Image.FileName));
                //}
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpPut("{bannerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBannerForAdmin(int? bannerId, [FromForm] UpdateBannerRequestForAdmin request)
        {
            try
            {
                if (!bannerId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "bannerId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (!request.EventDate.HasValue)
                {
                    errorMessages.Add("EventDate is required.");
                }
                else if (request.EventDate.Value < DateTime.Now)
                {
                    errorMessages.Add("EventDate cannot be in the past.");
                }

                if (string.IsNullOrWhiteSpace(request.Link))
                {
                    errorMessages.Add("Link is required.");
                }
                else
                {
                    Uri uriResult;
                    bool isValidUrl = Uri.TryCreate(request.Link, UriKind.Absolute, out uriResult) &&
                                      (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (!isValidUrl)
                    {
                        errorMessages.Add("Link is not a valid URL.");
                    }
                }

                // Kiểm tra liên kết (Link)
                if (!string.IsNullOrEmpty(request.Link))
                {
                    Uri uriResult;
                    bool isValidUrl = Uri.TryCreate(request.Link, UriKind.Absolute, out uriResult) &&
                                      (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (!isValidUrl)
                    {
                        errorMessages.Add("Link is not a valid URL.");
                    }
                    else
                    {
                        // Lấy path từ URL (bỏ phần domain)
                        request.Link = uriResult.PathAndQuery;
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

                var existingBanner = await _bannerService.GetBannerDetailByIdForAdminAsync(bannerId.Value);
                if (existingBanner == null || existingBanner.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Banner not found."
                    });
                }

                var updateDto = new UpdateBannerForAdminDTO
                {
                    BannerId = bannerId.Value,
                    Link = request.Link,
                    EventDate = request.EventDate,
                    Image = existingBanner.Data.Image
                };

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                string filePath = null;

                // Xử lý ảnh mới nếu có
                if (request.Image != null)
                {
                    //// Kiểm tra phần mở rộng file (chỉ chấp nhận các định dạng ảnh)
                    //string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    //string fileExtension = Path.GetExtension(request.Image.FileName).ToLower();

                    //if (!allowedExtensions.Contains(fileExtension))
                    //{
                    //    errorMessages.Add("Only image files with extensions .jpg, .jpeg, .png, .gif, .bmp, .webp are allowed.");
                    //}

                    //// Giới hạn dung lượng file (tối đa 10MB)
                    //long maxFileSizeInBytes = 10 * 1024 * 1024; // 10MB
                    //if (request.Image.Length > maxFileSizeInBytes)
                    //{
                    //    errorMessages.Add("Image size must not exceed 10MB.");
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

                    //string uniqueFileName = $"{createdByUserId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{request.Image.FileName}";
                    //string bannerPath = Path.Combine(_webHostEnvironment.WebRootPath, "banners");

                    //if (!Directory.Exists(bannerPath))
                    //{
                    //    Directory.CreateDirectory(bannerPath);
                    //}

                    //filePath = Path.Combine(bannerPath, uniqueFileName);
                    //using (var fileStream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    await request.Image.CopyToAsync(fileStream);
                    //}

                    //// Xóa ảnh cũ nếu có
                    //string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, updateDto.Image.TrimStart('/'));
                    //if (System.IO.File.Exists(oldImagePath))
                    //{
                    //    System.IO.File.Delete(oldImagePath);
                    //}

                    //var relativeImagePath = $"/banners/{uniqueFileName}";

                    // Cập nhật tên ảnh mới trong DTO
                    updateDto.Image = request.Image;
                }

                var dataResponse = await _bannerService.UpdateBannerForAdminAsync(updateDto);

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
                        Message = "Unexpected error occurred during banner update."
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
                var apiResponse = new ApiResponse<UpdateBannerForAdminDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                //// Xóa file nếu đã được tạo nhưng có lỗi xảy ra
                //if (System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "banners", request.Image.FileName)))
                //{
                //    System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, "banners", request.Image.FileName));
                //}
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpDelete("{bannerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBannerForAdmin(int? bannerId)
        {
            try
            {
                if (!bannerId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "bannerId is required."
                    });
                }

                var bannerDto = new DeleteBannerForAdminDTO
                {
                    BannerId = bannerId.Value
                };

                var dataResponse = await _bannerService.DeleteBannerForAdminAsync(bannerDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during banner delete."
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
                var apiResponse = new ApiResponse<DeleteBannerForAdminDTO>
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

        [HttpPatch("{bannerId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBannerStatusForAdmin(int? bannerId, UpdateBannerStatusRequestForAdmin request)
        {
            try
            {
                if (!bannerId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "bannerId is required."
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

                var bannerDto = new UpdateBannerStatusForAdminDTO
                {
                    BannerId = bannerId.Value,
                    Status = request.Status
                };

                var dataResponse = await _bannerService.UpdateBannerStatusForAdminAsync(bannerDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during banner update status."
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
                var apiResponse = new ApiResponse<UpdateBannerStatusForAdminDTO>
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
        public async Task<IActionResult> GetAllStatusesBannerForAdmin()
        {
            try
            {
                var dataResponse = await _bannerService.GetAllStatusesBannerForAdminAsync();

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

                var apiResponse = new ApiResponse<List<StatusBannerListForAdminDTO>>
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
