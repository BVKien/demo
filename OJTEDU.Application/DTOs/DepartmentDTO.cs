using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class DepartmentDTO
    {
        // Admin - DOET
        public class DepartmentListForAdminDoetDTO
        {
            public int DepartmentId { get; set; }
            public string? DepartmentCode { get; set; }
            public string? Name { get; set; }
            public string? Detail { get; set; }
            public string? Status { get; set; }
        }

        public class DepartmentDetailForAdminDoetDTO
        {
            public int DepartmentId { get; set; }
            public string? DepartmentCode { get; set; }
            public string? Name { get; set; }
            public string? Detail { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddDepartmentForAdminDoetDTO
        {
            public string? DepartmentCode { get; set; }
            public string? Name { get; set; }
            public string? Detail { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateDepartmentForAdminDoetDTO
        {
            public int DepartmentId { get; set; }
            public string? DepartmentCode { get; set; }
            public string? Name { get; set; }
            public string? Detail { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateDepartmentStatusForAdminDoetDTO
        {
            public int DepartmentId { get; set; }
            public string? DepartmentCode { get; set; }
            public string? Name { get; set; }
            public string? Detail { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteDepartmentForAdminDoetDTO
        {
            public int DepartmentId { get; set; }
            public string? DepartmentCode { get; set; }
            public string? Name { get; set; }
            public string? Detail { get; set; }
            public string? Status { get; set; }
        }

        public class StatusDepartmentListForAdminDoetDTO
        {
            public string? Status { get; set; }
        }

        // Common
        public class DepartmentListForCommonDTO
        {
            public int DepartmentId { get; set; }
            public string? DepartmentCodeAndName { get; set; }
        }
    }
}
