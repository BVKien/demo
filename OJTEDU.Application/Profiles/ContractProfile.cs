using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.ContractDTO;

namespace OJTEDU.Application.Profiles
{
    public class ContractProfile : Profile
    {
        public ContractProfile()
        {
            // Mentor 
            CreateMap<Contract, AssignContractInternshipForMentorDTO>().ReverseMap();
        }
    }
}
