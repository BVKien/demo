namespace OJTEDU.Api.Input.CompanyControllers
{
    public class UserController
    {
        public class AddUserRequestForCompany
        {
            public string? Email { get; set; }
            public string? Name { get; set; }
            public string? Information { get; set; }
        }

        public class UpdateUserRequestForCompany
        {
            public string? Email { get; set; }
            public string? Name { get; set; }
            public string? Information { get; set; }
        }

        public class UpdateUserStatusRequestForCompany
        {
            public string? Status { get; set; }
        }
    }
}
