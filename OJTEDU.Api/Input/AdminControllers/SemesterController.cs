namespace OJTEDU.Api.Input.AdminControllers
{
    public class SemesterController
    {
        // Admin-Doet - Semester
        public class AddSemesterRequestForAdminDoet
        {
            public string? SemesterCode { get; set; }
            public string? SemesterName { get; set; }
            public string? Description { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class UpdateSemesterRequestForAdminDoet
        {
            public string? SemesterCode { get; set; }
            public string? SemesterName { get; set; }
            public string? Description { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class UpdateSemesterStatusRequestForAdminDoet
        {
            public string? Status { get; set; }
        }
    }
}
