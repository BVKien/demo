using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class FeedbackDTO
    {
        // Student 
        public class CreateFeedbackForStudentDTO
        {
            public int FeedbackId { get; set; }
            public int? StudentId { get; set; }
            public int? CompanyId { get; set; }
            public int? UniversityId { get; set; }
            public string? FeedbackContent { get; set; }
        }

        public class FeedbackDetailForStudentDTO
        {
            public int FeedbackId { get; set; }
            public string? StudentName { get; set; }
            public string? StudentCode { get; set; }
            public string? CompanyName { get; set; }
            public string? DOETName { get; set; }
            public string? FeedbackContent { get; set; }
            public string? Status { get; set; }
            public string? CreatedAt { get; set; }
        }

        public class FeedbackListForStudentDTO
        {
            public int FeedbackId { get; set; }
            public int? CompanyName { get; set; }
            public string? FeedbackContent { get; set; }
            public string? Status { get; set; }
            public string? CreatedAt { get; set; }
        }
    }
}
