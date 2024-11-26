namespace OJTEDU.Api.Input.CompanyControllers
{
    public class JobController
    {
        public class CreateJobInput
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? TestFile { get; set; }
            public string? SalaryRange { get; set; } // double
            public string? Requirements { get; set; }
            public string? SkillRequirements { get; set; }
            public string? Benefits { get; set; }
            public string? WorkingHours { get; set; } // double
            public DateTime? Deadline { get; set; }
            public int? MajorId { get; set; }
            public int? Addressed { get; set; }
            public string? AddressDetail { get; set; }
            public int? WardId { get; set; }
            public int? DistrictId { get; set; }
            public int? ProvinceId { get; set; }
        }

        public class UpdateJobInput
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? TestFile { get; set; }
            public string? SalaryRange { get; set; } // double
            public string? Requirements { get; set; }
            public string? SkillRequirements { get; set; }
            public string? Benefits { get; set; }
            public string? WorkingHours { get; set; } // double
            public DateTime? Deadline { get; set; }
            public int? MajorId { get; set; }
            public string? AddressDetail { get; set; }
            public int? WardId { get; set; }
            public int? DistrictId { get; set; }
            public int? ProvinceId { get; set; }
        }
    }
}
