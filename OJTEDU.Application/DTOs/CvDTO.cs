using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class CvDTO
    {
        // Student 
        public class CvListForStudentDTO
        {
            public int CvId { get; set; }
            public string? Name { get; set; }
            public string? CvFile { get; set; }
            public string? Status { get; set; }
            public int? StudentId { get; set; }
            public string? UpdatedAt { get; set; }
        }
    }
}
