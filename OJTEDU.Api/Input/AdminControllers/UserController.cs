namespace OJTEDU.Api.Input.AdminControllers
{
    public class UserController
    {
        public class AddUserRequestForAdmin
        {
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
        }

        public class UpdateUserRequestForAdmin
        {
            public string? Email { get; set; }
            //public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
        }

        public class UpdateUserStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }
    }
}
