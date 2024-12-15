using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.NotificationDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/common/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IJobService _notificationService;

        public NotificationController(IJobService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Company, Student")]
        [HttpGet("list")]
        public async Task<IActionResult> GetNotificationsListForUser()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _notificationService.GetAllNotificationsByUserIdAsync(userId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<NotificationForUniCompanyStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<NotificationForUniCompanyStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<NotificationForUniCompanyStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<NotificationForUniCompanyStudentDTO>>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while retrieving all messages in conversation.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
