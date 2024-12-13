namespace OJTEDU.Api.Input.AdminControllers
{
    public class InternshipProcessController
    {
        public class AddOrUpdateInternshipProcessRequestForAdmin
        {
            public string? Title { get; set; }
            public string? FilePath { get; set; }
        }

        public class UpdateInternshipProcessVisibleRequestForAdmin
        {
            public bool? IsVisible { get; set; }
        }
    }
}
