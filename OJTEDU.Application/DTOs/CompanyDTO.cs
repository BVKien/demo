using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AddressDTO;
using static OJTEDU.Application.DTOs.JobDTO;

namespace OJTEDU.Application.DTOs
{
    public class CompanyDTO
    {
        // Admin - DOET
        public class CompanyListForAdminDoetDTO
        {
            public int CompanyId { get; set; }
            public string? CompanyName { get; set; }
            public string? CompanyCode { get; set; }
            public string? Address { get; set; }
            public string? Phone { get; set; }
            public string? ContactEmail { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class CompanyDetailForAdminDoetDTO
        {
            public int CompanyId { get; set; }
            public string? CompanyImage { get; set; }
            public string? CompanyName { get; set; }
            public string? CompanyCode { get; set; }
            public string? TaxCode { get; set; }
            public string? LoginEmail { get; set; }
            public string? ContactEmail { get; set; }
            public string? Phone { get; set; }
            public string? FullAddress { get; set; }
            public string? Website { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public AddressForCompanyDTO? Address { get; set; }
            public List<JobListByCompanyIdForAdminDooetDTO>? CompanyJobs { get; set; }
        }

        public class UpdateCompanyForAdminDoetDTO
        {
            public int CompanyId { get; set; }
            public string? CompanyName { get; set; }
            public string? CompanyCode { get; set; }
            public string? TaxCode { get; set; }
            public string? ContactEmail { get; set; }
            public string? Phone { get; set; }
            public int AddressId { get; set; }
            public string? Website { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        // Guest 
        public class CompanySearchListForGuestDTO
        {
            public int CompanyId { get; set; }
            public string? Name { get; set; }
            public string? Image { get; set; }
            public string? Address { get; set; }
        }

        public class CompanyDetailForGuestDTO
        {
            public int CompanyId { get; set; }
            public string? Name { get; set; }
            public string? Image { get; set; }
            public string? Email { get; set; }
            public string? AlternativeEmail { get; set; }
            public string? Phone { get; set; }
            public string? TaxCode { get; set; }
            public string? Website { get; set; }
            public string? Description { get; set; }
            public string? Address { get; set; }
        }

        // Student 
        public class CompanySearchListForStudentDTO
        {
            public int CompanyId { get; set; }
            public string? Name { get; set; }
            public string? Image { get; set; }
            public string? Address { get; set; }
            public int? JobCount { get; set; }
        }

        public class CompanyDetailForStudentDTO
        {
            public int CompanyId { get; set; }
            public string? Name { get; set; }
            public string? Image { get; set; }
            public string? Email { get; set; }
            public string? AlternativeEmail { get; set; }
            public string? Phone { get; set; }
            public string? TaxCode { get; set; }
            public string? Website { get; set; }
            public string? Description { get; set; }
            public string? Address { get; set; }
            public List<JobListByCompanyIdForStudentDTO>? JobList { get; set; }
        }

        // Company 
        public class MentorListForCompanyDTO
        {
            public int MentorId { get; set; }
            public string? MentorName { get; set; }
        }

        public class MentorsInfoListForCompanyDTO
        {
            public int CompanyId { get; set; }
            public string? AlternativeEmail { get; set; }
            public string? Phone { get; set; }
            public string? TaxCode { get; set; }
            public string? Website { get; set; }
            public string? Description { get; set; }
            public int? UserId { get; set; }
            public int? AddressId { get; set; }
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public int? RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Image { get; set; }
            public string? Information { get; set; }
            public int? ForCompany { get; set; }
            public int? AssignForId { get; set; }
            public int? DepartmentId { get; set; }
            public int? MajorId { get; set; }
        }

        public class UpdateCompanyForCompanyDTO
        {
            // User information
            public string? Image { get; set; }

            // Student information
            public string? AlternativeEmail { get; set; }
            public string? Phone { get; set; }
            public string? TaxCode { get; set; }
            public string? Website { get; set; }
            public string? Description { get; set; }
            public DateTime? Dob { get; set; }
            public bool? Gender { get; set; }

            // Address information
            public string? Detail { get; set; }
            public int? WardId { get; set; }
            public int? DistrictId { get; set; }
            public int? ProvinceId { get; set; }
        }
    }
}
