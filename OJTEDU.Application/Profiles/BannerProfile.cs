using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.BannerDTO;

namespace OJTEDU.Application.Profiles
{
    public class BannerProfile : Profile
    {
        public BannerProfile()
        {
            // Admin 
            CreateMap<Banner, BannerListForAdminDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ReverseMap();

            CreateMap<Banner, BannerDetailForAdminDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ReverseMap();

            CreateMap<Banner, DeleteBannerForAdminDTO>()
                .ReverseMap();

            CreateMap<Banner, UpdateBannerForAdminDTO>()
                .ReverseMap();

            CreateMap<Banner, UpdateBannerStatusForAdminDTO>()
                .ReverseMap();

            // Common
            CreateMap<Banner, BannerListForCommonDTO>()
                .ReverseMap();
        }
    }
}
