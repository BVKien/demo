using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.ProvinceDTO;

namespace OJTEDU.Application.Profiles
{
    public class ProvinceProfile : Profile
    {
        public ProvinceProfile() 
        {
            // Common 
            CreateMap<Province, ProvinceListForCommonDTO>().ReverseMap();
        }
    }
}
