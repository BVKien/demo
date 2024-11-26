using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.DepartmentDTO;

namespace OJTEDU.Application.Profiles
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            // Admin - DOET
            CreateMap<Department, DepartmentListForAdminDoetDTO>().ReverseMap();
            CreateMap<Department, DepartmentDetailForAdminDoetDTO>().ReverseMap();
            CreateMap<Department, AddDepartmentForAdminDoetDTO>().ReverseMap();
            CreateMap<Department, UpdateDepartmentForAdminDoetDTO>().ReverseMap();
            CreateMap<Department, UpdateDepartmentStatusForAdminDoetDTO>().ReverseMap();
            CreateMap<Department, DeleteDepartmentForAdminDoetDTO>().ReverseMap();

            // Common
            CreateMap<Department, DepartmentListForCommonDTO>()
                .ForMember(dest => dest.DepartmentCodeAndName, opt => opt.MapFrom(src =>
                    $"{src.DepartmentCode} - {src.Name}"))
                .ReverseMap();
        }
    }
}
