namespace OJTEDU.Api.Input.AdminControllers
{
    public class AddressController
    {
        // Admin - Province
        public class AddProvinceRequestForAdmin
        {
            public string? ProvinceName { get; set; }
        }

        public class UpdateProvinceRequestForAdmin
        {
            public string? ProvinceName { get; set; }
        }

        public class UpdateProvinceStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }

        // Admin - District
        public class AddDistrictRequestForAdmin
        {
            public string? DistrictName { get; set; }
            public int? ProvinceId { get; set; }
        }

        public class UpdateDistrictRequestForAdmin
        {
            public string? DistrictName { get; set; }
        }

        public class UpdateDistrictStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }

        // Admin - Ward
        public class AddWardRequestForAdmin
        {
            public string? WardName { get; set; }
            public int? DistrictId { get; set; }
        }

        public class UpdateWardRequestForAdmin
        {
            public string? WardName { get; set; }
        }

        public class UpdateWardStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }
    }
}
