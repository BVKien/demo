namespace OJTEDU.Api.Input.AdminControllers
{
    public class DepartmentController
    {
        public class AddDepartmentRequestForAdminDoet
        {
            public string? DepartmentCode { get; set; }
            public string? DepartmentName { get; set; }
            public string? Detail { get; set; }
        }

        public class UpdateDepartmentRequestForAdminDoet
        {
            public string? DepartmentCode { get; set; }
            public string? DepartmentName { get; set; }
            public string? Detail { get; set; }
        }

        public class UpdateDepartmentStatusRequestForAdminDoet
        {
            public string? Status { get; set; }
        }
    }
}
