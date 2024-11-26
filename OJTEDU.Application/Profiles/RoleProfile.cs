using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.RoleDTO;

namespace OJTEDU.Application.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleListDTO>()
                .ReverseMap();

            CreateMap<Role, RoleListForAdminDTO>()
                .ReverseMap();

            CreateMap<Role, RoleListForDoetDTO>()
            .ReverseMap();

            CreateMap<Role, RoleListForCompanyDTO>()
            .ReverseMap();

            CreateMap<Role, RoleDetailForAdminDTO>()
                .ReverseMap();

            CreateMap<Role, DeleteRoleForAdminDTO>()
                .ReverseMap();

            CreateMap<Role, UpdateRoleForAdminDTO>()
                .ReverseMap();
        }
    }
}
