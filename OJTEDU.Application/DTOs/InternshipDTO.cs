using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OJTEDU.Application.DTOs
{
    public class InternshipDTO
    {
        // Mentor 
        public class InternshipListForMentorDTO
        {
            public int IntershipId { get; set; }
            public int? StudentId { get; set; }
            public int? CompanyId { get; set; }
            public int? JobId { get; set; }
            public int? LecturerId { get; set; }
            public string? Code { get; set; }
            public string? InformationDetail { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? Status { get; set; }
            public int? ContractId { get; set; }
            public int? SemesterId { get; set; }
            public int? MajorId { get; set; }
            public int? EvaluationId { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
        }

        public class InternshipDetailForMentorDTO
        {
            public int IntershipId { get; set; }
            public string? Code { get; set; }
            public string? StudentName { get; set; }
            public string? StudentCode { get; set; }
            public string? StudentEmail { get; set; }
            public string? CompanyName { get; set; }
            public string? CompanyCode { get; set; }
            public string? CompanyEmail { get; set; }
            public string? JobTitle { get; set; }
            public string? LecturerName { get; set; }
            public string? LecturerEmail { get; set; }
            public string? InformationDetail { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? Status { get; set; }
            public string? ContractName { get; set; }
            public string? ContractFile { get; set; }
            public string? ContractType { get; set; }
            public string? SemesterName { get; set; }
            public string? MajorName { get; set; }
            public string? MajorCode { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
        }

        // Company 
        public class InternshipListForCompanyDTO
        {
            public int IntershipId { get; set; }
            public string? Code { get; set; }
            public int? StudentId { get; set; }
            public string? MentorName { get; set; }
            public string? JobTitle { get; set; }
            public string? LecturerName { get; set; }
            public string? InformationDetail { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? Status { get; set; }
            public int? ContractId { get; set; }
            public int? SemesterId { get; set; }
            public int? MajorId { get; set; }
            public int? EvaluationId { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
            public string? DeletedAt { get; set; }
        }

        public class CreateInternshipForCompanyDTO
        {
            public int IntershipId { get; set; }
            public string? Code { get; set; }
            public int? StudentId { get; set; }
            public int? JobId { get; set; }
            public int? LecturerId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? Status { get; set; }
            public int? SemesterId { get; set; }
            public int? MajorId { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
