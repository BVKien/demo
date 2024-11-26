using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.InternshipProcessDTO;

namespace OJTEDU.Application.Profiles
{
    public class InternshipProcessProfile : Profile
    {
        public InternshipProcessProfile()
        {
            // Admin - DOET
            CreateMap<InternshipProcess, InternshipProcessListForAdminDoetDTO>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByNavigation.Name))
                .ReverseMap();

            CreateMap<InternshipProcess, InternshipProcessDetailForAdminDoetDTO>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByNavigation.Name))
                .ReverseMap();

            CreateMap<InternshipProcess, AddInternshipProcessForAdminDoetDTO>()
                .ReverseMap();

            CreateMap<InternshipProcess, DeleteInternshipProcessForAdminDoetDTO>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByNavigation.Name))
                .ReverseMap();

            CreateMap<InternshipProcess, UpdateInternshipProcessForAdminDoetDTO>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByNavigation.Name))
                .ReverseMap();
        }
    }
}
