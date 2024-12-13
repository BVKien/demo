namespace OJTEDU.Api.Input.AdminControllers
{
    public class UserGuideController
    {
        public class AddOrUpdateUserGuideRequestForAdmin
        {
            public string? UserGuideFile { get; set; }
            public int? RoleId { get; set; }
        }

        public class UpdateUserGuideStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }
    }
}
