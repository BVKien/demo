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
using static OJTEDU.Application.DTOs.AppllicationDTO;
using static OJTEDU.Application.DTOs.GroupChatDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class GroupChatService : IGroupChatService
    {
        private readonly IGroupChatRepository _groupChatRepository;
        private readonly IMapper _mapper;
        public GroupChatService(IGroupChatRepository groupChatRepository, IMapper mapper)
        {
            _groupChatRepository = groupChatRepository;
            _mapper = mapper;
        }

        // Admin, DOET, Dean, Lecturer, Mentor
        public async Task<DataResponse<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>> CreateGroupChatAsync(int? userId,
            CreateGroupChatForAdminDOETDeanLecturerMentorDTO? groupChatInfo)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                var group = new GroupChat
                {
                    GroupName = groupChatInfo?.GroupName
                };

                var groupChat = await _groupChatRepository.CreateGroupChatAsync(userId, group);
                var response = _mapper.Map<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>(groupChat);

                return new DataResponse<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Create group chat successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateGroupChatForAdminDOETDeanLecturerMentorDTO>
                {
                    StatusCode = 500,
                    Message = $"Error create group chat: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>> SearchGroupChatByNameAsync(int? userId, string? groupName)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                var groupChats = await _groupChatRepository.SearchGroupChatByNameAsync(userId, groupName);
                var response = _mapper.Map<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>(groupChats);

                return new DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                {
                    StatusCode = 200,
                    Message = "Group chat list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error search group chat list: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>> GetAllGroupChatByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                var groupChats = await _groupChatRepository.GetAllGroupChatByUserIdAsync(userId);
                var response = _mapper.Map<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>(groupChats);

                return new DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                {
                    StatusCode = 200,
                    Message = "Group chat list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<GroupChatListForAdminDOETDeanLecturerMentorDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error get group chat list: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>> UpdateGroupChatAsync(int? groupChatId, 
            UpdateGroupChatForAdminDOETDeanLecturerMentorDTO? groupChatInfo)
        {
            try
            {
                if (groupChatId == null)
                {
                    return new DataResponse<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found group chat.",
                        Data = null
                    };
                }

                var info = new GroupChat
                {
                    GroupName = groupChatInfo?.GroupName
                };

                var groupChat = await _groupChatRepository.UpdateGroupChatAsync(groupChatId, info);
                var response = _mapper.Map<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>(groupChat);

                return new DataResponse<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Group chat update successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>
                {
                    StatusCode = 500,
                    Message = $"Error update group chat: {ex.Message}. ",
                    Data = null
                };
            }
        }
    }
}
