using AutoMapper;
using AutoMapper.Execution;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.MessageGroupDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class MessageGroupService : IMessageGroupService
    {
        private readonly IMessageGroupRepository _messageGroupRepository;
        private readonly IMapper _mapper;
        public MessageGroupService(IMessageGroupRepository messageGroupRepository, IMapper mapper)
        {
            _messageGroupRepository = messageGroupRepository;
            _mapper = mapper;
        }

        // Admin, DOET, Dean, Lecturer, Mentor, Student 
        public async Task<DataResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>> CreateMemberInMessageGroupAsync(int? userId, int? memberId,
            CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO? messageGroupInfo)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                if (memberId == null)
                {
                    return new DataResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found member.",
                        Data = null
                    };
                }

                var info = new MessageGroup
                {
                    GroupChatId = messageGroupInfo?.GroupChatId,
                };

                var member = await _messageGroupRepository.CreateMemberInMessageGroupAsync(userId, memberId, info);
                var response = _mapper.Map<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>(member);

                return new DataResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Add member successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>
                {
                    StatusCode = 500,
                    Message = $"Error create member: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<string>> DeleteMemberInMessageGroupAsync(int? userId, int? groupChatId, int? memberId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<string>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                if (memberId == null)
                {
                    return new DataResponse<string>
                    {
                        StatusCode = 404,
                        Message = "Not found member.",
                        Data = null
                    };
                }

                if (groupChatId == null)
                {
                    return new DataResponse<string>
                    {
                        StatusCode = 404,
                        Message = "Not found group chat.",
                        Data = null
                    };
                }

                var member = await _messageGroupRepository.DeleteMemberInMessageGroupAsync(userId, groupChatId, memberId);
                var response = _mapper.Map<string>(member);

                return new DataResponse<string>
                {
                    StatusCode = 200,
                    Message = "Delete member successfully!",
                    Data = $"Member id: {memberId}"
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    StatusCode = 500,
                    Message = $"Error delete member: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<GetAllMemberForUserAsyncForAdminDOETDeanLecturerMentorDTO>>> SearchAllMemberAndGroupForUserAsync(int? userId, string? name)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<GetAllMemberForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                var members = await _messageGroupRepository.SearchAllMemberAndGroupForUserAsync(userId, name);

                var membersDto = members.Select(member => new GetAllMemberForUserAsyncForAdminDOETDeanLecturerMentorDTO
                {
                    Name = member.Name,
                    Image = member.Image,
                    UserCode = member.UserCode,
                }).ToList();

                return new DataResponse<List<GetAllMemberForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                {
                    StatusCode = 200,
                    Message = "Successfully retrieved members!",
                    Data = membersDto
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<GetAllMemberForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving members: {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> SetAdminMemberInMessageGroupAsync(int? userId, int? memberId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = false
                    };
                }

                if (memberId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found member.",
                        Data = false
                    };
                }

                var member = await _messageGroupRepository.SetAdminMemberInMessageGroupAsync(userId, memberId);
                var response = _mapper.Map<bool>(member);

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Set admin permission for member in message group successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"Error Set admin permission for member in message group: {ex.Message}.",
                    Data = false
                };
            }
        }

        // Admin, DOET, Dean, Lecturer, Mentor, Student
        public async Task<DataResponse<bool>> LeaveMessageGroupForUserAsync(int? userId, int? groupChatId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = false
                    };
                }

                if (groupChatId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found group chat.",
                        Data = false
                    };
                }

                var member = await _messageGroupRepository.LeaveMessageGroupAsync(userId, groupChatId);
                var response = _mapper.Map<bool>(member);

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Leave message group successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"Error leaving message group: {ex.Message}.",
                    Data = false
                };
            }
        }

        public async Task<DataResponse<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>> GetAllMessagesInGroupChatAsync(int? groupChatId)
        {
            try
            {
                if (groupChatId == null)
                {
                    return new DataResponse<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found group chat.",
                        Data = null
                    };
                }

                var messages = await _messageGroupRepository.GetAllMessagesInGroupChatAsync(groupChatId);
                var response = _mapper.Map<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>(messages);

                return new DataResponse<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Retrieving all messages in group chat successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving all messages in group chat: {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>> CreateMessageInMessageGroupAsync(int? userId, string? messageFileName, string? imageFileName,
            CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO? messageGroupInfo)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                var messageInfo = new MessageGroup
                {
                    GroupChatId = messageGroupInfo?.GroupChatId,
                    MessageContent = messageGroupInfo?.MessageContent
                };

                var message = await _messageGroupRepository.CreateMessageInMessageGroupAsync(userId, messageFileName, imageFileName, messageInfo);
                var response = _mapper.Map<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>(message);

                return new DataResponse<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Create message in group chat successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error creating message in group chat: {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>> GetAllGroupChatAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                var messages = await _messageGroupRepository.GetAllGroupChatAsync(userId);
                var response = _mapper.Map<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>(messages);

                return new DataResponse<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                {
                    StatusCode = 200,
                    Message = "Retrieving all group chat successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<GetAllGroupForUserAsyncForAdminDOETDeanLecturerMentorDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving all group chat: {ex.Message}.",
                    Data = null
                };
            }
        }
    }
}
