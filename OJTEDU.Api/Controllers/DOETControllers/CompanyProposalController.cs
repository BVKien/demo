using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.CompanyProposalDTO;

namespace OJTEDU.Api.Controllers.DOETControllers
{
    [Route("api/doet/companyproposal")]
    [ApiController]
    public class CompanyProposalController : ControllerBase
    {
        private readonly ICompanyProposalService _compService;

        public CompanyProposalController(ICompanyProposalService compService)
        {
            _compService = compService;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAllCompanyProposalsAsync(
        int? pageNumber,
        int? pageSize)
        {
            try
            {
                // Lấy thông tin UserId từ Claims
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Gán giá trị mặc định cho pageNumber và pageSize nếu không được cung cấp
                var actualPageNumber = pageNumber ?? 1;
                var actualPageSize = pageSize ?? 15;

                // Gọi Service để lấy dữ liệu
                var response = await _compService.GetAllCompanyProposalsForDoetAsync(
                    userId, actualPageNumber, actualPageSize);

                // Kiểm tra kết quả trả về
                if (response.Data == null)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                // Trả về kết quả thành công
                return Ok(new ApiResponse<PagedResponse<List<CompanyProposalDto>>>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCompanyProposalStatus([FromBody] UpdateCompanyProposalStatusDto dto)
        {
            try
            {
                // Gọi Service để cập nhật status và response content
                var response = await _compService.UpdateCompanyProposalStatusAsync(dto);

                if (response.StatusCode != 200)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
    }
}
