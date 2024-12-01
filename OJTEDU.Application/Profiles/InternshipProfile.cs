using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.InternshipDTO;

namespace OJTEDU.Application.Profiles
{
    public class InternshipProfile : Profile
    {
        public InternshipProfile()
        {
            // Mentor
            CreateMap<Internship, InternshipListForMentorDTO>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Internship, InternshipDetailForMentorDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.Student.User.UserCode))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.User.Email))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.User.Name))
                .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Company.User.UserCode))
                .ForMember(dest => dest.CompanyEmail, opt => opt.MapFrom(src => src.Company.User.Email))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturer.Name))
                .ForMember(dest => dest.LecturerEmail, opt => opt.MapFrom(src => src.Lecturer.Email))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.Value.ToString("dd-MM-yyyy")))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.Value.ToString("dd-MM-yyyy")))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == "1" ? "In progress" : src.Status == "0" ? "Failed" : src.Status == "2" ? "Passed" : "Unknown"))
                .ForMember(dest => dest.ContractName, opt => opt.MapFrom(src => src.Contract.Name))
                .ForMember(dest => dest.ContractFile, opt => opt.MapFrom(src => src.Contract.ContractFile))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.Contract.ContractType.Name))
                .ForMember(dest => dest.SemesterName, opt => opt.MapFrom(src => src.Semester.Name))
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.Name))
                .ForMember(dest => dest.MajorCode, opt => opt.MapFrom(src => src.Major.MajorCode))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            // Company 
            CreateMap<Internship, InternshipListForCompanyDTO>()
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Company.User.Name))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturer.Name))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == "1" ? "In progress" : src.Status == "0" ? "Failed" : src.Status == "2" ? "Passed" : "Unknown"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Internship, CreateInternshipForCompanyDTO>().ReverseMap();

            CreateMap<Internship, InternshipDto>()
           .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
           .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.User.Name))
           .ForMember(dest => dest.JobName, opt => opt.MapFrom(src => src.Job.Title))
           .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Student.Lecturer.Name))
           .ForMember(dest => dest.SemesterName, opt => opt.MapFrom(src => src.Semester.Name))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
           .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
           .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
           .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));

           
        }
    }
}
