using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class StudentDTO
    {
        public class StudentDetailForStudentDTO
        {
            public int StudentId { get; set; }
            public string? Email { get; set; }
            public string? AlternativeEmail { get; set; }
            public string? Name { get; set; }
            public string? StudentCode { get; set; }
            public string? Image { get; set; }
            public string? Phone { get; set; }
            public string? Dob { get; set; }
            public string? Gender { get; set; }
            public string? Semester { get; set; }
            public string? Major { get; set; }
            public string? Lecturer { get; set; }
            public string? Address { get; set; }
        }

        public class UpdateStudentForStudentDTO
        {
            // User information
            public string? Image { get; set; }

            // Student information
            public string? AlternativeEmail { get; set; }
            public string? Phone { get; set; }
            public DateTime? Dob { get; set; }
            public bool? Gender { get; set; }

            // Address information
            public string? Detail { get; set; }
            public int? WardId { get; set; }
            public int? DistrictId { get; set; }
            public int? ProvinceId { get; set; }
        }
        //For Dean
        public class AssignLecturerForStudentDto
        {
            public int LecturerId { get; set; }
            public List<int> StudentIds { get; set; }
        }
        public class StudentListDto
        {
            public string Name { get; set; }
            public string UserCode { get; set; }
            public string MajorName { get; set; }
            public string SemesterName { get; set; }
            public string LecturerName { get; set; }
        }

        public class StudentDetailsDto
        {
            public string Email { get; set; }
            public string AlternativeEmail { get; set; }
            public string Name { get; set; }
            public string UserCode { get; set; }
            public string Information { get; set; }
            public string Image { get; set; }
            public string SemesterName { get; set; }
            public string MajorName { get; set; }
            public string LecturerName { get; set; }
            public string Phone { get; set; }
            public DateTime? DOB { get; set; }
            public string Address { get; set; }
        }

    }
}
