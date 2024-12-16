using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.CompanyDTO;
using static OJTEDU.Application.DTOs.JobDTO;
using static OJTEDU.Application.DTOs.StudentDTO;

namespace OJTEDU.Application.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            // Admin - DOET
            CreateMap<Company, CompanyListForAdminDoetDTO>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.User.UserCode))
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.AlternativeEmail))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User.Status))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                    $"{src.Address.Detail}, {src.Address.Ward.Name}, {src.Address.District.Name}, {src.Address.Province.Name}"))
                .ReverseMap();

            CreateMap<Company, CompanyDetailForAdminDoetDTO>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.User.UserCode))
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.AlternativeEmail))
                .ForMember(dest => dest.LoginEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User.Status))
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src =>
                    $"{src.Address.Detail}, {src.Address.Ward.Name}, {src.Address.District.Name}, {src.Address.Province.Name}"))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.CompanyJobs, opt => opt.MapFrom(src => src.Jobs))
                .ReverseMap();

            CreateMap<Company, UpdateCompanyForAdminDoetDTO>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : null))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.User != null ? src.User.UserCode : null))
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.AlternativeEmail))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User != null ? src.User.Status : null))
                .ReverseMap();

            // Guest 
            CreateMap<Company, CompanySearchListForGuestDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.User.Image))
                .ForMember(dest => dest.Address, opt => opt
                .MapFrom(src => $"{src.Address.Province.Name}"))
                .ReverseMap();

            CreateMap<Company, CompanyDetailForGuestDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.User.Image))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Address, opt => opt
                .MapFrom(src => src.Address.Province.Name))
                .ReverseMap();

            // Student 
            CreateMap<Company, CompanySearchListForStudentDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.User.Image))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                    $"{src.Address.Detail}, {src.Address.Ward.Name}, {src.Address.District.Name}, {src.Address.Province.Name}"))
                .ReverseMap();

            CreateMap<Company, CompanyDetailForStudentDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.User.Image))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                    $"{src.Address.Detail}, {src.Address.Ward.Name}, {src.Address.District.Name}, {src.Address.Province.Name}"))
                .ReverseMap();

            // Company
            CreateMap<Company, MentorListForCompanyDTO>()
                .ForMember(dest => dest.MentorId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.User.Name))
                .ReverseMap();

            CreateMap<Company, MentorsInfoListForCompanyDTO>().ReverseMap();
            CreateMap<Company, UpdateCompanyForCompanyDTO>().ReverseMap();

            // Company
            CreateMap<Student, CompanyDetailForCompanyDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.User.UserCode))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.User.Image))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address.Detail + ", "
                + src.Address.Ward.Name + ", "
                + src.Address.District.Name + ", "
                + src.Address.Province.Name))
                .ReverseMap();
        }
    }
}
