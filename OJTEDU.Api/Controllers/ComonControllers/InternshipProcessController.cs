using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using System.Security.Claims;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/common/internship-process")]
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

        [HttpGet("view-file")]
        public async Task<IActionResult> GetIntershipProcessFilePDF()
        {
            try
            {
                var dataResponse = await _internshipProcessService.GetInternshipProcessByVisibleAsync();

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
                // var filePath = Path.Combine(_webHostEnvironment.WebRootPath, dataResponse.Data.FilePath.TrimStart('/'));

                //// Kiểm tra file tồn tại
                //if (!System.IO.File.Exists(filePath))
                //{
                //    return NotFound(new ApiResponse<object>
                //    {
                //        Data = null,
                //        Message = "Internship Process File not found on the server."
                //    });
                //}

                //// Đọc file PDF và trả về
                //var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                //return File(fileBytes, "application/pdf", Path.GetFileName(filePath));

                // Đọc nội dung file
                // var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                // Trả về file dưới dạng stream để tải xuống
                return Ok(dataResponse.Data.FilePath);
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
