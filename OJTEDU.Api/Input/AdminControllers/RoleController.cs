namespace OJTEDU.Api.Input.AdminControllers
{
    public class RoleController
    {
        public class AddRoleRequestForAdmin
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
        }

        public class UpdateRoleRequestForAdmin
        {
            //public string? Name { get; set; }
            public string? Description { get; set; }
        }
    }
}
