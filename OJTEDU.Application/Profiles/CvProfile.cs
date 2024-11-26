using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.CvDTO;

namespace OJTEDU.Application.Profiles
{
    public class CvProfile : Profile
    {
        public CvProfile()
        {
            // Student
            CreateMap<Cv, CvListForStudentDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    src.Status == "1" ? "Primary" :
                    src.Status == "0" ? "Normal" :
                    src.Status == "2" ? "Stored" : "Unknown"))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy")))
                .ReverseMap();
        }
    }
}
