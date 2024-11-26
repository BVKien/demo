using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AddressDTO;

namespace OJTEDU.Application.Profiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            // Admin - Province
            CreateMap<Province, ProvinceListForAdminDTO>()
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<Province, ProvinceDetailForAdminDTO>()
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<Province, AddProvinceForAdminDTO>()
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<Province, UpdateProvinceForAdminDTO>()
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<Province, UpdateProvinceStatusForAdminDTO>()
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<Province, DeleteProvinceForAdminDTO>()
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            // Admin - District
            CreateMap<District, DistrictListForAdminDTO>()
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ParentProvinceName, opt => opt.MapFrom(src => src.Province.Name))
                .ReverseMap();
            CreateMap<District, DistrictDetailForAdminDTO>()
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ParentProvinceName, opt => opt.MapFrom(src => src.Province.Name))
                .ReverseMap();
            CreateMap<District, AddDistrictForAdminDTO>()
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<District, UpdateDistrictForAdminDTO>()
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<District, UpdateDistrictStatusForAdminDTO>()
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<District, DeleteDistrictForAdminDTO>()
                .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            // Admin - Ward
            CreateMap<Ward, WardListForAdminDTO>()
                .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ParentDistrictName, opt => opt.MapFrom(src => src.District.Name))
                .ReverseMap();
            CreateMap<Ward, WardDetailForAdminDTO>()
                .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ParentDistrictName, opt => opt.MapFrom(src => src.District.Name))
                .ReverseMap();
            CreateMap<Ward, AddWardForAdminDTO>()
                .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<Ward, UpdateWardForAdminDTO>()
                .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<Ward, UpdateWardStatusForAdminDTO>()
                .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
            CreateMap<Ward, DeleteWardForAdminDTO>()
                .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<Address, AddressForCompanyDTO>()
                .ReverseMap();
        }
    }
}
