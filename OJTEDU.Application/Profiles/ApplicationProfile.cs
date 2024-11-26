using AutoMapper;
using System;
using System.Linq;
using System.Text;
using OJTEDU.Domain.Entities;
using static OJTEDU.Application.DTOs.AppllicationDTO;

namespace OJTEDU.Application.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            // Student 
            CreateMap<Appllication, ApplyJobForStudentDTO>();
            CreateMap<Appllication, AppllicationListForStudentDTO>()
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.User.Email))
                .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.Student.User.UserCode))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
                .ForMember(dest => dest.InterviewDate, opt => opt.MapFrom(src => src.InterviewDate.Value.ToString("dd-MM-yyyy")))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                src.Status == "1" ? "Reviewing"
                : src.Status == "0" ? "Rejected"
                : src.Status == "2" ? "Offered"
                : src.Status == "3" ? "Accept Offer"
                : src.Status == "4" ? "Accepted Internship"
                : src.Status == "5" ? "Internship Comfirmed"
                : "Unknown"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            // Comapny 
            CreateMap<Appllication, AppllicationListForCompanyDTO>()
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.User.Email))
                .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.Student.User.UserCode))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
                .ForMember(dest => dest.InterviewDate, opt => opt.MapFrom(src => src.InterviewDate.Value.ToString("dd-MM-yyyy")))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                src.Status == "1" ? "Reviewing"
                : src.Status == "0" ? "Rejected"
                : src.Status == "2" ? "Offered"
                : src.Status == "3" ? "Accept Offer"
                : src.Status == "4" ? "Accepted Internship"
                : src.Status == "5" ? "Internship Comfirmed"
                : "Unknown"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Appllication, AppllicationDetailForCompanyDTO>()
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.User.Email))
                .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.Student.User.UserCode))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
                .ForMember(dest => dest.InterviewDate, opt => opt.MapFrom(src => src.InterviewDate.Value.ToString("dd-MM-yyyy")))
                .ForMember(dest => dest.CvName, opt => opt.MapFrom(src => src.Cv.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                src.Status == "1" ? "Reviewing"
                : src.Status == "0" ? "Rejected"
                : src.Status == "2" ? "Offered"
                : src.Status == "3" ? "Accept Offer"
                : src.Status == "4" ? "Accepted Internship"
                : src.Status == "5" ? "Internship Comfirmed"
                : "Unknown"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();
        }
    }
}
