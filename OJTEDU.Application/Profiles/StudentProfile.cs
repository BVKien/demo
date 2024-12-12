using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.StudentDTO;

namespace OJTEDU.Application.Profiles
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            // Student 
            CreateMap<Student, StudentDetailForStudentDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.User.UserCode))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.User.Image))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob.Value.ToString("dd-MM-yyyy")))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.Value ? "Male" : "Female"))
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester.Name))
                .ForMember(dest => dest.Major, opt => opt.MapFrom(src => src.Major.Name))
                .ForMember(dest => dest.Lecturer, opt => opt.MapFrom(src => src.Lecturer.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address.Detail + ", "
                + src.Address.Ward.Name + ", "
                + src.Address.District.Name + ", "
                + src.Address.Province.Name))
                .ReverseMap();

            CreateMap<Student, UpdateStudentForStudentDTO>().ReverseMap();

            //For Dean 
            // Mapping for StudentListDto
            CreateMap<Student, StudentListDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.User.UserCode))
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.Name))
                .ForMember(dest => dest.SemesterName, opt => opt.MapFrom(src => src.Semester.Name))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturer.Name))
                .ForMember(dest => dest.IsMajorActive, opt => opt.MapFrom(src => src.Major != null && src.Major.Status == "Active"));
             

            // Mapping for StudentDetailsDto
            CreateMap<Student, StudentDetailsDto>()
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))   // From User.Name
           .ForMember(dest => dest.AlternativeEmail, opt => opt.MapFrom(src => src.AlternativeEmail))  // Direct mapping
           .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.User.UserCode))
           .ForMember(dest => dest.Information, opt => opt.MapFrom(src => src.User.Information))
           .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.User.Image))
           .ForMember(dest => dest.SemesterName, opt => opt.MapFrom(src => src.Semester.Name)) // From Semester.Name
           .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.Name))  // From Major.Name
           .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturer.Name)) // From Lecturer.User.Name
           .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone)) // Direct mapping
           .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.Dob)) // Direct mapping for DOB
           .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
             src.Address.Detail + ", " +                                                // Address.Detail
            (src.Address.Ward != null ? src.Address.Ward.Name + ", " : "") +           // Ward.Name if not null
            (src.Address.District != null ? src.Address.District.Name + ", " : "") +   // District.Name if not null
            (src.Address.Province != null ? src.Address.Province.Name : "")            // Province.Name if not null
             ))
            .ReverseMap();
        }
    }
}
