namespace OJTEDU.Api.Input.AdminControllers
{
    public class CompanyController
    {
        public class UpdateCompanyRequestForAdminDoet
        {
            public string? CompanyName { get; set; }
            public string? TaxCode { get; set; }
            public string? ContactEmail { get; set; }
            public string? Phone { get; set; }
            public string? Website { get; set; }
            public string? Description { get; set; }
            public int? ProvinceId { get; set; }
            public int? DistrictId { get; set; }
            public int? WardId { get; set; }
            public string? AddressDetail { get; set; }
        }
    }
}
