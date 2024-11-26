using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class JobDTO
    {
        // Admin - DOET
        public class JobListByCompanyIdForAdminDooetDTO
        {
            public int JobId { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? SalaryRange { get; set; }
        }

        // Student  
        public class JobListByCompanyIdForStudentDTO
        {
            public int JobId { get; set; }
            public string? Title { get; set; }
            public string? SalaryRange { get; set; }
            public string? Deadline { get; set; }
            public string? Address { get; set; }
        }

        public class JobListSearchForStudentDTO
        {
            public int JobId { get; set; }
            public string? CompanyImage { get; set; }
            public string? CompanyName { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? SalaryRange { get; set; }
            public string? Deadline { get; set; }
            public string? Major { get; set; }
            public string? Address { get; set; }
        }

        public class JobListForStudentDTO
        {
            public int JobId { get; set; }
            public string? CompanyName { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? TestFile { get; set; }
            public string? SalaryRange { get; set; }
            public string? Requirements { get; set; }
            public string? SkillRequirements { get; set; }
            public string? Benefits { get; set; }
            public string? WorkingHours { get; set; }
            public string? Deadline { get; set; }
            public string? Major { get; set; }
            public string? Address { get; set; }
        }

        public class JobDetailForStudentDTO
        {
            public int JobId { get; set; }
            public string? CompanyImage { get; set; }
            public string? CompanyName { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? TestFile { get; set; }
            public string? SalaryRange { get; set; }
            public string? Requirements { get; set; }
            public string? SkillRequirements { get; set; }
            public string? Benefits { get; set; }
            public string? WorkingHours { get; set; }
            public string? Deadline { get; set; }
            public string? Major { get; set; }
            public string? Address { get; set; }
        }

        // Company 
        public class JobListForCompanyDTO
        {
            public int JobId { get; set; }
            public string? Title { get; set; }
            public string? SalaryRange { get; set; }
            public string? Requirements { get; set; }
            public string? SkillRequirements { get; set; }
            public string? WorkingHours { get; set; }
            public string? Deadline { get; set; }
            public string? Status { get; set; }
            public string? MajorName { get; set; }
            public string? Address { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
            public string? DeletedAt { get; set; }
        }

        public class JobDetailForCompanyDTO
        {
            public int JobId { get; set; }
            public string? CompanyName { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? TestFile { get; set; }
            public string? SalaryRange { get; set; }
            public string? Requirements { get; set; }
            public string? SkillRequirements { get; set; }
            public string? Benefits { get; set; }
            public string? WorkingHours { get; set; }
            public string? Deadline { get; set; }
            public string? Status { get; set; }
            public string? MajorName { get; set; }
            public string? Address { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
            public string? DeletedAt { get; set; }
        }

        public class CreateJobForCompanyDTO
        {
            public int JobId { get; set; }
            public int? CompanyId { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? TestFile { get; set; }
            public string? SalaryRange { get; set; }
            public string? Requirements { get; set; }
            public string? SkillRequirements { get; set; }
            public string? Benefits { get; set; }
            public string? WorkingHours { get; set; }
            public DateTime? Deadline { get; set; }
            public int? MajorId { get; set; }
            public int? Addressed { get; set; }
            public string? Detail { get; set; }
            public int? WardId { get; set; }
            public int? DistrictId { get; set; }
            public int? ProvinceId { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateJobForCompanyDTO
        {
            public int JobId { get; set; }
            public int? CompanyId { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? TestFile { get; set; }
            public string? SalaryRange { get; set; }
            public string? Requirements { get; set; }
            public string? SkillRequirements { get; set; }
            public string? Benefits { get; set; }
            public string? WorkingHours { get; set; }
            public DateTime? Deadline { get; set; }
            public int? MajorId { get; set; }
            public int? Addressed { get; set; }
            public string? Detail { get; set; }
            public int? WardId { get; set; }
            public int? DistrictId { get; set; }
            public int? ProvinceId { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
