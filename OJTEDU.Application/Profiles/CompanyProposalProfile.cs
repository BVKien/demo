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
            CreateMap<CompanyProposal, CompanyProposalDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
            .ForMember(dest => dest.CompanyProposalId, opt => opt.MapFrom(src => src.CompanyProposalId))
            .ForMember(dest => dest.ProposalTitle, opt => opt.MapFrom(src => src.ProposalTitle))
            .ForMember(dest => dest.ProposalContent, opt => opt.MapFrom(src => src.ProposalContent))
            .ForMember(dest => dest.ProposalDate, opt => opt.MapFrom(src => src.ProposalDate))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}