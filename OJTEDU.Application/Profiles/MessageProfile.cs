using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.MessageDTO;

namespace OJTEDU.Application.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            // Admin, DOET, Dean, Lecturer, Company, Mentor, Student
            CreateMap<Message, CreateMessageForAdminDOETDeanLecturerMentorCompanyStudentDTO>().ReverseMap();
            CreateMap<Message, MessageListForAdminDOETDeanLecturerMentorCompanyStudentDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.StudentImage, opt => opt.MapFrom(src => src.Student.User.Image))
                .ForMember(dest => dest.UniversiryName, opt => opt.MapFrom(src => src.Universiry.Name))
                .ForMember(dest => dest.UniversiryImage, opt => opt.MapFrom(src => src.Universiry.Image))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.User.Name))
                .ForMember(dest => dest.CompanyImage, opt => opt.MapFrom(src => src.Company.User.Image))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == "1" ? "Active" : src.Status == "0" ? "Deleted" : "Unknown"))
                .ReverseMap();
        }
    }
}
