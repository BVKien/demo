using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.InternshipDTO;


namespace OJTEDU.Api.Controllers.DeanControllers
{
    [Authorize(Roles = "Dean")]
    [Route("api/dean/internships")]
    [ApiController]
    public class InternshipController : ControllerBase
    {
        private readonly IInternshipService _internshipService;

        public InternshipController(IInternshipService internshipService)
        {
            _internshipService = internshipService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllInternshipsAsync(
        string? searchTerm,
        DateTime? startDate,
        DateTime? endDate,
        string? statusFilter,
        string? sortBy,
        bool? isDescending,
        int? pageNumber,
        int? pageSize)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                var actualPageNumber = pageNumber ?? 1;
                var actualPageSize = pageSize ?? 15;

                var response = await _internshipService.GetAllInternshipsAsync(
                    userId,
                    role,
                    searchTerm,
                    startDate,
                    endDate,
                    statusFilter,
                    sortBy,
                    isDescending ?? false,
                    actualPageNumber,
                    actualPageSize);

                if (response.Data == null)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<PagedResponse<List<InternshipDto>>>
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
