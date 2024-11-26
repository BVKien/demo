using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.MajorDTO;

namespace OJTEDU.Application.Profiles
{
    public class MajorProfile : Profile
    {
        public MajorProfile()
        {
            // Admin - DOET
            CreateMap<Major, MajorListForAdminDoetDTO>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src =>
                    $"{src.Department.DepartmentCode} - {src.Department.Name}"))
                .ReverseMap();
            CreateMap<Major, MajorDetailForAdminDoetDTO>().ReverseMap();
            CreateMap<Major, AddMajorForAdminDoetDTO>().ReverseMap();
            CreateMap<Major, UpdateMajorForAdminDoetDTO>().ReverseMap();
            CreateMap<Major, UpdateMajorStatusForAdminDoetDTO>().ReverseMap();
            CreateMap<Major, DeleteMajorForAdminDoetDTO>().ReverseMap();

            // Common
            CreateMap<Major, MajorListForCommonDTO>()
                .ForMember(dest => dest.MajorCodeAndName, opt => opt.MapFrom(src =>
                    $"{src.MajorCode} - {src.Name}"))
                .ReverseMap();

            // Student 
            CreateMap<Major, MajorListForStudentDTO>().ReverseMap();
        }
    }
}
