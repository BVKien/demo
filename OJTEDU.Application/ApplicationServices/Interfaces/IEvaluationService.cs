using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.EvaluationDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IEvaluationService
    {
        // University, Company
        Task<DataResponse<CreateEvaluationForUniversityCompanyDTO>> CreateEvaluationAsync(int? internshipId, CreateEvaluationForUniversityCompanyDTO? info);

        // University, Company, Student
        Task<DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>> GetEvaluationDetailByUserId(int? userId);
        Task<DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>> GetEvaluationDetailByInternshipId(int? internshipId);
    }
}
