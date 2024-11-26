using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class AppllicationDTO
    {
        // Student 
        public class ApplyJobForStudentDTO
        {
            public int? StudentId { get; set; }
            public int? JobId { get; set; }
            public string? CoverLetter { get; set; }
            public int? CvId { get; set; }
        }

        public class AppllicationDetailForStudentDTO
        {
            public int? StudentId { get; set; }
            public int? JobId { get; set; }
            public string? TestFile { get; set; }
            public string? CoverLetter { get; set; }
            public int? CvId { get; set; }
            public string? CvFile { get; set; }
            public string? Status { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
        }

        public class AppllicationListForStudentDTO
        {
            public int ApplicationId { get; set; }
            public int? StudentId { get; set; }
            public string? StudentEmail { get; set; }
            public string? StudentCode { get; set; }
            public string? StudentName { get; set; }
            public string? JobTitle { get; set; }
            public string? InterviewDate { get; set; }
            public string? Status { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
        }

        // Company 
        public class AppllicationListForCompanyDTO
        {
            public int ApplicationId { get; set; }
            public int? StudentId { get; set; }
            public string? StudentEmail { get; set; }
            public string? StudentCode { get; set; }
            public string? StudentName { get; set; }
            public string? JobTitle { get; set; }
            public string? InterviewDate { get; set; }
            public string? Status { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
            //public DateTime? DeletedAt { get; set; } // Cân nhắc chức năng
        }

        public class AppllicationDetailForCompanyDTO
        {
            public int ApplicationId { get; set; }
            public int? StudentId { get; set; }
            public string? StudentEmail { get; set; }
            public string? StudentCode { get; set; }
            public string? StudentName { get; set; }
            public string? JobTitle { get; set; }
            public string? TestFile { get; set; }
            public string? Feedback { get; set; }
            public string? InterviewDate { get; set; }
            public string? CoverLetter { get; set; }
            public string? CvName { get; set; }
            public string? CvFile { get; set; }
            public string? Status { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
            //public DateTime? DeletedAt { get; set; }
        }
    }
}
