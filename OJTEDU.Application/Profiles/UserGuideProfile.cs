using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.UserGuideDTO;

namespace OJTEDU.Application.Profiles
{
    public class UserGuideProfile : Profile
    {
        public UserGuideProfile()
        {
            // Admin 
            CreateMap<UserGuide, UserGuideListForAdminDTO>()
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src => src.Role.Name))
                .ReverseMap();

            CreateMap<UserGuide, UserGuideDetailForAdminDTO>()
                .ForMember(dest => dest.ForRoleId, opt => opt.MapFrom(src => src.RoleId))
                .ReverseMap();

            CreateMap<UserGuide, AddUserGuideForAdminDTO>()
                .ForMember(dest => dest.ForRoleId, opt => opt.MapFrom(src => src.RoleId))
                .ReverseMap();

            CreateMap<UserGuide, DeleteUserGuideForAdminDTO>()
                .ForMember(dest => dest.ForRoleId, opt => opt.MapFrom(src => src.RoleId))
                .ReverseMap();

            CreateMap<UserGuide, UpdateUserGuideForAdminDTO>()
                .ForMember(dest => dest.ForRoleId, opt => opt.MapFrom(src => src.RoleId))
                .ReverseMap();

            CreateMap<UserGuide, UpdateUserGuideStatusForAdminDTO>()
                .ForMember(dest => dest.ForRoleId, opt => opt.MapFrom(src => src.RoleId))
                .ReverseMap();
        }
    }
}
