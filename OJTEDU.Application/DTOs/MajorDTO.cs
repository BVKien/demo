using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class MajorDTO
    {
        // Admin - DOET
        public class MajorListForAdminDoetDTO
        {
            public int MajorId { get; set; }
            public string? MajorCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Department { get; set; }
            public string? Status { get; set; }
        }

        public class MajorDetailForAdminDoetDTO
        {
            public int MajorId { get; set; }
            public string? MajorCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int? DepartmentId { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddMajorForAdminDoetDTO
        {
            public string? MajorCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int? DepartmentId { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateMajorForAdminDoetDTO
        {
            public int MajorId { get; set; }
            public string? MajorCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int? DepartmentId { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateMajorStatusForAdminDoetDTO
        {
            public int MajorId { get; set; }
            public string? MajorCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int? DepartmentId { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteMajorForAdminDoetDTO
        {
            public int MajorId { get; set; }
            public string? MajorCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int? DepartmentId { get; set; }
            public string? Status { get; set; }
        }

        public class StatusMajorListForAdminDoetDTO
        {
            public string? Status { get; set; }
        }

        // Common
        public class MajorListForCommonDTO
        {
            public int MajorId { get; set; }
            public string? MajorCodeAndName { get; set; }
        }

        // Student
        public class MajorListForStudentDTO 
        {
            public int MajorId { get; set; }
            public string? Name { get; set; }
        }
    }
}
