using AutoMapper;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.SupportRequestDTO;

namespace OJTEDU.Application.Profiles
{
    public class SupportRequestProfile : Profile
    {
        public SupportRequestProfile()
        {
            // Student 
            CreateMap<SupportRequest, CreateSupportRequestForStudentDTO>().ReverseMap();
            CreateMap<SupportRequest, SupportRequestListForStudentDTO>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<SupportRequest, SupportRequestDetailForStudentDTO>()
                .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.Student.User.UserCode))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.DOETName, opt => opt.MapFrom(src => src.University.Name))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
            .ReverseMap();
            //DOET
            CreateMap<SupportRequest, SupportRequestListForDOETDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.DOETName, opt => opt.MapFrom(src => src.University.Name))
                .ForMember(dest => dest.RequestContent, opt => opt.MapFrom(src => src.RequestContent))
                .ForMember(dest => dest.FeedbackContent, opt => opt.MapFrom(src => src.FeedbackContent))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.HasValue ? src.CreatedAt.Value.ToString("dd-MM-yyyy") : null));
                
        }
    }
}