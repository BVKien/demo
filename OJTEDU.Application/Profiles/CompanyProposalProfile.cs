using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.CompanyProposalDTO;

namespace OJTEDU.Application.Profiles
{
    public class CompanyProposalProfile : Profile
    {
        public CompanyProposalProfile()
        {
            // Student 
            CreateMap<CompanyProposal, CompanyProposalListForStudentDTO>().ReverseMap();
            CreateMap<CompanyProposal, CompanyProposalDetailForStudentDTO>()
                .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University.Name))
                .ReverseMap();

            CreateMap<CompanyProposal, CreateCompanyProposalForStudentDTO>().ReverseMap();
        }
    }
}