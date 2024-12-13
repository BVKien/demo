namespace OJTEDU.Api.Input.AdminControllers
{
    public class DocumentController
    {
        public class AddDocumentRequestForAdmin
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? DocumentFile { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateDocumentRequestForAdmin
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? DocumentFile { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateDocumentStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }
    }
}
