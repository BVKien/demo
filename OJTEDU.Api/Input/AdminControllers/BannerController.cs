namespace OJTEDU.Api.Input.AdminControllers
{
    public class BannerController
    {
        public class AddBannerRequestForAdmin
        {
            public IFormFile? Image { get; set; }
            public DateTime? EventDate { get; set; }
            public string? Link { get; set; }
        }

        public class UpdateBannerRequestForAdmin
        {
            public IFormFile? Image { get; set; }
            public DateTime? EventDate { get; set; }
            public string? Link { get; set; }
        }

        public class UpdateBannerStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }
    }
}
