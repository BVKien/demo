using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class SupportRequestDTO
    {
        // Student 
        public class CreateSupportRequestForStudentDTO
        {
            public int SupportRequestId { get; set; }
            public int? StudentId { get; set; }
            public int? UniversityId { get; set; }
            public string? RequestTitle { get; set; }
            public string? RequestContent { get; set; }
        }

        public class SupportRequestDetailForStudentDTO
        {
            public int SupportRequestId { get; set; }
            public string? StudentName { get; set; }
            public string? StudentCode { get; set; }
            public string? DOETName { get; set; }
            public string? RequestTitle { get; set; }
            public string? RequestContent { get; set; }
            public string? FeedbackContent { get; set; }
            public string? Status { get; set; }
            public string? CreatedAt { get; set; }
        }

        public class SupportRequestListForStudentDTO
        {
            public int SupportRequestId { get; set; }
            public string? DOETName { get; set; }
            public string? RequestTitle { get; set; }
            public string? RequestContent { get; set; }
            public string? Status { get; set; }
            public string? CreatedAt { get; set; }
        }
        public class SupportRequestListForDOETDto
        {
            public string StudentName { get; set; }
            public string DOETName { get; set; }
            public string? RequestTitle { get; set; }
            public string RequestContent { get; set; }
            public string FeedbackContent { get; set; }
            public string Status { get; set; }
            public string CreatedAt { get; set; }
        }
        public class UpdateSupportRequestForDOETDto
        {
            public string FeedbackContent { get; set; }
            public int Status { get; set; } // 1: Approved, 2: Rejected
        }

    }
}
