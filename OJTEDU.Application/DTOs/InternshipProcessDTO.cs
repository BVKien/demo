using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class InternshipProcessDTO
    {
        // Admin - DOET
        public class InternshipProcessListForAdminDoetDTO
        {
            public int IntershipProcessId { get; set; }
            public string? Title { get; set; }
            public string? FilePath { get; set; }
            public string? CreatedBy { get; set; }
            public bool? IsVisible { get; set; }
        }

        public class InternshipProcessDetailForAdminDoetDTO
        {
            public int IntershipProcessId { get; set; }
            public string? Title { get; set; }
            public string? FilePath { get; set; }
            public string? CreatedBy { get; set; }
            public bool? IsVisible { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddInternshipProcessForAdminDoetDTO
        {
            public string? Title { get; set; }
            public string? FilePath { get; set; }
            public int? CreatedBy { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateInternshipProcessForAdminDoetDTO
        {
            public int IntershipProcessId { get; set; }
            public string? Title { get; set; }
            public string? FilePath { get; set; }
            public string? CreatedBy { get; set; }
            public bool? IsVisible { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteInternshipProcessForAdminDoetDTO
        {
            public int IntershipProcessId { get; set; }
            public string? Title { get; set; }
            public string? FilePath { get; set; }
            public string? CreatedBy { get; set; }
            public bool? IsVisible { get; set; }
        }
    }
}
