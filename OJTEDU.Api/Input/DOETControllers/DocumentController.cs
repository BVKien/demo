namespace OJTEDU.Api.Input.DOETControllers
{
    public class DocumentController
    {
        public class AddDocumentRequestForDoet
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? DocumentFile { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateDocumentRequestForDoet
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? DocumentFile { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateDocumentStatusRequestForDoet
        {
            public string? Status { get; set; }
        }
    }
}
