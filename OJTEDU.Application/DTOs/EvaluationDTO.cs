using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class EvaluationDTO
    {
        // University, Company 
        public partial class CreateEvaluationForUniversityCompanyDTO
        {
            public string? CompanyComment { get; set; }
            public double? CompanyScore { get; set; }
            public string? DeanComment { get; set; }
            public double? DeanScore { get; set; }
        }

        // University, Company, Student
        public partial class GetEvaluationDetailForUniversityCompanyStudentDTO
        {
            public int EvaluationId { get; set; }
            public int? MentorId { get; set; }
            public int? LecturerId { get; set; }
            public int? StudentId { get; set; }
            public string? CompanyComment { get; set; }
            public string? DeanComment { get; set; }
            public double? CompanyScore { get; set; }
            public double? DeanScore { get; set; }
            public double? EvaluationScore { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public partial class GetEvaluationStudentDTO
        {
            public int EvaluationId { get; set; }
            public int? MentorId { get; set; }
            public int? LecturerId { get; set; }
            public int? StudentId { get; set; }
            public string? CompanyComment { get; set; }
            public string? DeanComment { get; set; }
            public double? CompanyScore { get; set; }
            public double? DeanScore { get; set; }
            public double? EvaluationScore { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }
    }
}
