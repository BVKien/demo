using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.MessageDTO;
using static OJTEDU.Application.DTOs.MessageGroupDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessageService(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>> CreateFirstMessageConversationAsync(int? userId, int? receiverId,
            string? messageFileName, string? imageFileName,
            CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO? messageGroupInfo)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                if (receiverId == null)
                {
                    return new DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found receiver.",
                        Data = null
                    };
                }

                var messageInfo = new Message
                {
                    MessageContent = messageGroupInfo?.MessageContent
                };

                var message = await _messageRepository.CreateFirstMessageConversationAsync(userId, receiverId, messageFileName, imageFileName, messageInfo);
                var response = _mapper.Map<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>(message);

                return new DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Create message in group chat successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error creating message in group chat: {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>> CreateMessageAsync(int? userId,
            string? messageFileName, string? imageFileName,
            CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO? messageGroupInfo)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                var messageInfo = new Message
                {
                    ConversationId = messageGroupInfo?.ConversationId,
                    MessageContent = messageGroupInfo?.MessageContent
                };

                var message = await _messageRepository.CreateMessageAsync(userId, messageFileName, imageFileName, messageInfo);
                var response = _mapper.Map<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>(message);

                return new DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Create message in group chat successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error creating message in group chat: {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>> GetAllMessageInConversationAsync(int? conversationId)
        {
            try
            {
                if (conversationId == null)
                {
                    return new DataResponse<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found conversation.",
                        Data = null
                    };
                }

                var messages = await _messageRepository.GetAllMessageInConversationAsync(conversationId);
                var response = _mapper.Map<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>(messages);

                return new DataResponse<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Retrieving all messages in conversation successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving all messages in conversation: {ex.Message}.",
                    Data = null
                };
            }
        }
    }
}
