using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.GroupChatDTO;

namespace OJTEDU.Application.Profiles
{
    public class GroupChatProfile : Profile
    {
        public GroupChatProfile()
        {
            // Admin, DOET, Dean, Lecturer, Mentor
            CreateMap<GroupChat, CreateGroupChatForAdminDOETDeanLecturerMentorDTO>().ReverseMap();
            CreateMap<GroupChat, GroupChatListForAdminDOETDeanLecturerMentorDTO>().ReverseMap();
            CreateMap<GroupChat, UpdateGroupChatForAdminDOETDeanLecturerMentorDTO>().ReverseMap();
        }
    }
}
