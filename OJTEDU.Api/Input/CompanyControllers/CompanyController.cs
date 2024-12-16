namespace OJTEDU.Api.Input.CompanyControllers
{
    public class CompanyController
    {
        public class UpdateCompanyInput
        {
            // User information
            public string? Image { get; set; }

            // Student information
            public string? AlternativeEmail { get; set; }
            public string? Phone { get; set; }
            public string? TaxCode { get; set; }
            public string? Website { get; set; }
            public string? Description { get; set; }
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
