using static OJTEDU.Application.DTOs.StudentDTO;

namespace OJTEDU.Api.Input.StudentControllers
{
    public class StudentController
    {
        public class UpdateStudentInput
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
    }
}
