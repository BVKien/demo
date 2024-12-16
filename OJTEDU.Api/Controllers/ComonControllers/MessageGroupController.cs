using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Domain.Entities;
using System.Security.Claims;
using static OJTEDU.Api.Input.CommonControllers.MessageGroupController;
using static OJTEDU.Application.DTOs.MessageGroupDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/message-group")]
    [ApiController]
    public class MessageGroupController : ControllerBase
    {
        private readonly IMessageGroupService _messageGroupService;

        public MessageGroupController(IMessageGroupService messageGroupService)
        {
            _messageGroupService = messageGroupService;
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        [HttpPost("member/create")]
        public async Task<IActionResult> CreateMemberGroupMessage(int? memberId, [FromBody] CreateMemberGroupMessageInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var memberInfoDto = new CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO
                {
                    GroupChatId = input?.GroupChatId
                };

                var apiResponse = await _messageGroupService.CreateMemberInMessageGroupAsync(userId, memberId, memberInfoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while add member in group message.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        [HttpPut("member/delete")]
        public async Task<IActionResult> DeleteMemberGroupMessage(int? groupChatId, int? memberId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _messageGroupService.DeleteMemberInMessageGroupAsync(userId, groupChatId, memberId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<string>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while delete member in group message.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        [HttpGet("member-group/search")]
        public async Task<IActionResult> SearchAllMembersAndGroupsForUser(string? name)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _messageGroupService.SearchAllMemberAndGroupForUserAsync(userId, name);

                return Ok(new ApiResponse<List<GetAllMemberForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>
                {
                    Message = $"An error occurred while searc members: {ex.Message}.",
                    Data = new { Details = ex.Message }
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        [HttpPut("member/leave")]
        public async Task<IActionResult> LeaveGroupMessage(int? groupChatId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _messageGroupService.LeaveMessageGroupForUserAsync(userId, groupChatId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while leaving group message.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        [HttpPut("member/admin-permission")]
        public async Task<IActionResult> SetAdminGroupMessage(int? memberId)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _messageGroupService.SetAdminMemberInMessageGroupAsync(userId, memberId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<bool>
                    {
                        Message = apiResponse.Message,
                        Data = false
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while setting admin permission in group message.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        [HttpGet("message/list/{groupChatId}")]
        public async Task<IActionResult> GetAllMessagesInGroup(int? groupChatId)
        {
            try
            {
                var apiResponse = await _messageGroupService.GetAllMessagesInGroupChatAsync(groupChatId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while retrieving all messages in group chat.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        //[Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        //[HttpPost("files/upload")]
        //public async Task<IActionResult> UploadFile()
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //            return BadRequest("No file uploaded.");

        //        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/messagesgroup/files/");
        //        if (!Directory.Exists(uploadPath))
        //        {
        //            Directory.CreateDirectory(uploadPath);
        //        }

        //        var filePath = Path.Combine(uploadPath, file.FileName);
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        byte[] fileData;
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(memoryStream);
        //            fileData = memoryStream.ToArray();
        //        }

        //        return Ok(new
        //        {
        //            Data = file.FileName
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new ApiResponse<string>
        //        {
        //            Message = $"An error occurred while uploading file.",
        //            Data = ex.Message
        //        };

        //        return StatusCode(500, errorResponse);
        //    }
        //}

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        [HttpPost("message/create")]
        public async Task<IActionResult> CreateMessagesInGroup(CreateMessagesInGroupInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                //var messageFilePath = Path.Combine("wwwroot/uploads/messagesgroup/files/", input.MessageFile);
                //var imageFilePath = Path.Combine("wwwroot/uploads/messagesgroup/files/", input.Image);

                //// Initialize 
                //byte[]? messageFileData = null;
                //byte[]? imageFileData = null;

                //// Read content file if it is not null
                //if (!string.IsNullOrEmpty(input.MessageFile) && System.IO.File.Exists(messageFilePath))
                //{
                //    messageFileData = await System.IO.File.ReadAllBytesAsync(messageFilePath);
                //}
                //if (!string.IsNullOrEmpty(input.Image) && System.IO.File.Exists(imageFilePath))
                //{
                //    imageFileData = await System.IO.File.ReadAllBytesAsync(imageFilePath);
                //}

                var messageInfoDto = new CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO
                {
                    GroupChatId = input?.GroupChatId,
                    MessageContent = input?.MessageContent
                };

                var apiResponse = await _messageGroupService.CreateMessageInMessageGroupAsync(userId, input?.MessageFile, input?.Image, messageInfoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                //if (apiResponse.StatusCode == 200)
                //{
                //    if (messageFileData != null && System.IO.File.Exists(messageFilePath))
                //    {
                //        System.IO.File.Delete(messageFilePath);
                //    }

                //    if (imageFileData != null && System.IO.File.Exists(imageFilePath))
                //    {
                //        System.IO.File.Delete(imageFilePath);
                //    }
                //}

                return Ok(new ApiResponse<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while create messages in group chat.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        [HttpGet("group-chat/list")]
        public async Task<IActionResult> GetAllGroupChat()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var apiResponse = await _messageGroupService.GetAllGroupChatAsync(userId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while retrieving all group chat.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
