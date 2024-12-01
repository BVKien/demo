using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/common/user-guide")]
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

        [HttpGet("view-file")]
        [Authorize]
        public async Task<IActionResult> GetUserGuideFilePDF()
        {
            try
            {
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                var dataResponse = await _userGuideService.GetUserGuideByRoleNameAsync(roleClaim);

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

                // Đường dẫn đến file PDF trên máy chủ
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.UserGuideFile.TrimStart('/'));

                // Kiểm tra file tồn tại
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "User Guide File not found on the server."
                    });
                }

                //// Đọc file PDF và trả về
                //var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                //return File(fileBytes, "application/pdf", Path.GetFileName(filePath));

                // Đọc file PDF và trả về
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                // Trả về file dưới dạng stream để tải xuống
                return File(fileBytes, "application/octet-stream", dataResponse.Data.UserGuideFile);
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
