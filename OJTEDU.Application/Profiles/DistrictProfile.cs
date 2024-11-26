using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.DistrictDTO;

namespace OJTEDU.Application.Profiles
{
    public class DistrictProfile : Profile
    {
        public DistrictProfile()
        {
            // Common 
            CreateMap<District, DistrictListForCommonDTO>().ReverseMap();
        }
    }
}
