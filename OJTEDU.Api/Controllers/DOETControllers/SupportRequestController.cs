using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.SupportRequestDTO;

namespace OJTEDU.Api.Controllers.DOETControllers
{
    [Route("api/doet/support-request")]
    [ApiController]
    [Authorize (Roles="DOET")]
    public class SupportRequestController : ControllerBase
    {
        private readonly ISupportRequestService _supportRequestService;
        public SupportRequestController(ISupportRequestService supportRequestService)
        {
            _supportRequestService = supportRequestService;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAllSupportRequests(
        string? studentName,
        string? DOETName,
        string? status,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        bool? isDescending)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var response = await _supportRequestService.GetAllSupportRequestsForDOETAsync(studentName, DOETName, status, actualPageNumber, actualPageSize, sortBy, isDescending);

                if (response.Data == null)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<PagedResponse<List<SupportRequestListForDOETDto>>>
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
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }
        [HttpPut("update/{supportRequestId}")]
        public async Task<IActionResult> UpdateSupportRequest(int supportRequestId, [FromBody] UpdateSupportRequestForDOETDto dto)
        {
            try
            {
                var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int universityUserId))
                {
                    return StatusCode(401, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unauthorized. User ID not found in claims."
                    });
                }

                var response = await _supportRequestService.UpdateSupportRequestForDOETAsync(supportRequestId, dto, universityUserId);

                if (response.Data == null)
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
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }
        [HttpDelete("delete/{supportRequestId}")]
        public async Task<IActionResult> DeleteSupportRequest(int supportRequestId)
        {
            try
            {
                var response = await _supportRequestService.DeleteSupportRequestForDOETAsync(supportRequestId);

                if (response.Data == null)
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
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

    }
}
