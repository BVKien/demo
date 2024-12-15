using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.MessageGroupDTO;

namespace OJTEDU.Application.Profiles
{
    public class MessageGroupProfile : Profile
    {
        public MessageGroupProfile()
        {
            // Admin, DOET, Dean, Lecturer, Mentor
            CreateMap<MessageGroup, CreateMemberMessageGroupForAdminDOETDeanLecturerMentorDTO>().ReverseMap();
            CreateMap<MessageGroup, GetAllMemberForUserAsyncForAdminDOETDeanLecturerMentorDTO>().ReverseMap();

        CreateMap<MessageGroup, MessagesInGroupChatListForForAdminDOETDeanLecturerMentorStudentDTO>()
                .ForMember(dest => dest.StudentAvatar, opt => opt.MapFrom(src => src.Student.User.Image))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.UniversityAvatar, opt => opt.MapFrom(src => src.University.Image))
                .ForMember(dest => dest.UniversityName, opt => opt.MapFrom(src => src.University.Name))
                .ForMember(dest => dest.MentorAvatar, opt => opt.MapFrom(src => src.Mentor.User.Image))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == "1" ? "Active" : src.Status == "0" ? "Deleted" : "Unknown"))
                .ReverseMap();

            // Admin, DOET, Dean, Lecturer, Mentor, Student
            CreateMap<MessageGroup, CreateMessageGroupForAdminDOETDeanLecturerMentorStudentDTO>().ReverseMap();
        }
    }
}
