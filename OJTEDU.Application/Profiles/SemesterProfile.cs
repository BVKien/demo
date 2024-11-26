using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.SemesterDTO;

namespace OJTEDU.Application.Profiles
{
    public class SemesterProfile : Profile
    {
        public SemesterProfile()
        {
            // Admin-Doet - Semester
            CreateMap<Semester, SemesterListForAdminDoetDTO>().ReverseMap();
            CreateMap<Semester, SemesterDetailForAdminDoetDTO>().ReverseMap();
            CreateMap<Semester, AddSemesterForAdminDoetDTO>().ReverseMap();
            CreateMap<Semester, UpdateSemesterForAdminDoetDTO>().ReverseMap();
            CreateMap<Semester, UpdateSemesterStatusForAdminDoetDTO>().ReverseMap();
            CreateMap<Semester, DeleteSemesterForAdminDoetDTO>().ReverseMap();

            // Common - Semester
            CreateMap<Semester, SemesterListForCommonDTO>()
                .ForMember(dest => dest.SemesterCodeAndName, opt => opt.MapFrom(src =>
                    $"{src.SemesterCode} - {src.Name}"))
                .ReverseMap();
        }
    }
}
