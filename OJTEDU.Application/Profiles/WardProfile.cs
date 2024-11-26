using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.WardDTO;

namespace OJTEDU.Application.Profiles
{
    public class WardProfile : Profile
    {
        public WardProfile() 
        {
            // Common 
            CreateMap<Ward, WardListForCommonDTO>().ReverseMap();
        }
    }
}
