using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class SemesterDTO
    {
        // Admin-Doet - Semester
        public class SemesterListForAdminDoetDTO
        {
            public int SemesterId { get; set; }
            public string? SemesterCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? Status { get; set; }
        }

        public class SemesterDetailForAdminDoetDTO
        {
            public int SemesterId { get; set; }
            public string? SemesterCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddSemesterForAdminDoetDTO
        {
            public string? SemesterCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateSemesterForAdminDoetDTO
        {
            public int SemesterId { get; set; }
            public string? SemesterCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateSemesterStatusForAdminDoetDTO
        {
            public int SemesterId { get; set; }
            public string? SemesterCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteSemesterForAdminDoetDTO
        {
            public int SemesterId { get; set; }
            public string? SemesterCode { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? Status { get; set; }
        }

        public class StatusSemesterListForAdminDoetDTO
        {
            public string? Status { get; set; }
        }

        // Common - Semester
        public class SemesterListForCommonDTO
        {
            public int SemesterId { get; set; }
            public string? SemesterCodeAndName { get; set; }
        }
    }
}
