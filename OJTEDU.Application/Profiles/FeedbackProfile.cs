using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.FeedbackDTO;

namespace OJTEDU.Application.Profiles
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            // Student 
            CreateMap<Feedback, CreateFeedbackForStudentDTO>().ReverseMap();
            CreateMap<Feedback, FeedbackListForStudentDTO>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();
            
            CreateMap<Feedback, FeedbackDetailForStudentDTO>()
                .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.Student.User.UserCode))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.User.Name))
                .ForMember(dest => dest.DOETName, opt => opt.MapFrom(src => src.University.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    src.Status == "1" ? "Successfully" :
                    src.Status == "0" ? "Deleted" : "Unknown"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();
        }
    }
}
