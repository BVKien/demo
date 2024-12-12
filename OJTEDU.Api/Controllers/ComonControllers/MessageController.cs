using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.MessageDTO;
using static OJTEDU.Api.Input.CommonControllers.MessageController;
using static OJTEDU.Application.DTOs.MessageGroupDTO;
using OJTEDU.Application.DTOs;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Student")]
        [HttpPost("files/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/messages/files/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                return Ok(new
                {
                    Data = file.FileName
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while uploading file.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Company, Student")]
        [HttpPost("conversation/create")]
        public async Task<IActionResult> CreateFirstMessageConversation(int? userId, int? receiverId, CreateFirstMessageConversationInput? input)
        {
            try
            {
                //int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var messageFilePath = Path.Combine("wwwroot/uploads/messages/files/", input.MessageFile);
                var imageFilePath = Path.Combine("wwwroot/uploads/messages/files/", input.Image);

                // Initialize 
                byte[]? messageFileData = null;
                byte[]? imageFileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.MessageFile) && System.IO.File.Exists(messageFilePath))
                {
                    messageFileData = await System.IO.File.ReadAllBytesAsync(messageFilePath);
                }
                if (!string.IsNullOrEmpty(input.Image) && System.IO.File.Exists(imageFilePath))
                {
                    imageFileData = await System.IO.File.ReadAllBytesAsync(imageFilePath);
                }

                var messageInfoDto = new CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO
                {
                    MessageContent = input?.MessageContent
                };

                var apiResponse = await _messageService.CreateFirstMessageConversationAsync(userId, receiverId, input?.MessageFile, messageFileData, input?.Image, imageFileData, messageInfoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 200)
                {
                    if (messageFileData != null && System.IO.File.Exists(messageFilePath))
                    {
                        System.IO.File.Delete(messageFilePath);
                    }

                    if (imageFileData != null && System.IO.File.Exists(imageFilePath))
                    {
                        System.IO.File.Delete(imageFilePath);
                    }
                }

                return Ok(new ApiResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while create message.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Company, Student")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateMessage(int? userId, CreateMessageInput? input)
        {
            try
            {
                //int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var messageFilePath = Path.Combine("wwwroot/uploads/messages/files/", input.MessageFile);
                var imageFilePath = Path.Combine("wwwroot/uploads/messages/files/", input.Image);

                // Initialize 
                byte[]? messageFileData = null;
                byte[]? imageFileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.MessageFile) && System.IO.File.Exists(messageFilePath))
                {
                    messageFileData = await System.IO.File.ReadAllBytesAsync(messageFilePath);
                }
                if (!string.IsNullOrEmpty(input.Image) && System.IO.File.Exists(imageFilePath))
                {
                    imageFileData = await System.IO.File.ReadAllBytesAsync(imageFilePath);
                }

                var messageInfoDto = new CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO
                {
                    ConversationId = input?.ConversationId,
                    MessageContent = input?.MessageContent
                };

                var apiResponse = await _messageService.CreateMessageAsync(userId, input?.MessageFile, messageFileData, input?.Image, imageFileData, messageInfoDto);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 200)
                {
                    if (messageFileData != null && System.IO.File.Exists(messageFilePath))
                    {
                        System.IO.File.Delete(messageFilePath);
                    }

                    if (imageFileData != null && System.IO.File.Exists(imageFilePath))
                    {
                        System.IO.File.Delete(imageFilePath);
                    }
                }

                return Ok(new ApiResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while create message.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Admin, DOET, Dean, Lecturer, Mentor, Company, Student")]
        [HttpGet("list/{conversationId}")]
        public async Task<IActionResult> GetAllMessageInConversationAsync(int? conversationId)
        {
            try
            {
                var apiResponse = await _messageService.GetAllMessageInConversationAsync(conversationId);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>
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
