using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.EvaluationDTO;

namespace OJTEDU.Application.Profiles
{
    public class EvaluationProfile : Profile
    {
        public EvaluationProfile()
        {
            // University, Company, Student
            CreateMap<Evaluation, CreateEvaluationForUniversityCompanyDTO>().ReverseMap();
            CreateMap<Evaluation, GetEvaluationDetailForUniversityCompanyStudentDTO>().ReverseMap();
            CreateMap<Evaluation, GetEvaluationStudentDTO>().ReverseMap();
        }
    }
}
