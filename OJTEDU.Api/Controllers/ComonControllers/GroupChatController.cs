using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using System.Security.Claims;
using static OJTEDU.Api.Input.CommonControllers.GroupChatController;
using static OJTEDU.Application.DTOs.GroupChatDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/message/group-chat")]
    [ApiController]
    public class GroupChatController : ControllerBase
    {
        private readonly IGroupChatService _groupChatService;

        public GroupChatController(IGroupChatService groupChatService)
        {
            _groupChatService = groupChatService;
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateGroupChat([FromBody] CreateGroupChatInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var groupChatInfoDto = new CreateGroupChatForAdminDOETDeanLecturerMentorDTO
                {
                    GroupName = input?.GroupName
                };

                var apiResponse = await _groupChatService.CreateGroupChatAsync(userId, groupChatInfoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while create group chat.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchGroupChat(string? groupName)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _groupChatService.SearchGroupChatByNameAsync(userId, groupName);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while search group chat list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor")]
        [HttpGet("list")]
        public async Task<IActionResult> GetGroupChatList()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _groupChatService.GetAllGroupChatByUserIdAsync(userId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while get group chat list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroupChat(int? groupChatId, [FromBody] UpdateGroupChatInput? input)
        {
            try
            {
                var infoDto = new UpdateGroupChatForAdminDOETDeanLecturerMentorDTO
                {
                    GroupName = input?.GroupName
                };

                var apiResponse = await _groupChatService.UpdateGroupChatAsync(groupChatId, infoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while update group chat.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
