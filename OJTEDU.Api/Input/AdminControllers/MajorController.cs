namespace OJTEDU.Api.Input.AdminControllers
{
    public class MajorController
    {
        public class AddMajorRequestForAdminDoet
        {
            public string? MajorCode { get; set; }
            public string? MajorName { get; set; }
            public string? Description { get; set; }
            public int? DepartmentId { get; set; }
        }

        public class UpdateMajorRequestForAdminDoet
        {
            public string? MajorCode { get; set; }
            public string? MajorName { get; set; }
            public string? Description { get; set; }
            public int? DepartmentId { get; set; }
        }

        public class UpdateMajorStatusRequestForAdminDoet
        {
            public string? Status { get; set; }
        }
    }
}
