namespace OJTEDU.Api.Input.DOETControllers
{
    public class UserController
    {
        public class AddUserRequestForDoet
        {
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
        }

        public class UpdateUserRequestForDoet
        {
            public string? Email { get; set; }
            //public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
        }

        public class UpdateUserStatusRequestForDoet
        {
            public string? Status { get; set; }
        }
    }
}
