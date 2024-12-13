namespace OJTEDU.Api.Input.CompanyControllers
{
    public class DocumentController
    {
        public class CreateTestFileDocumentInput
        {
            public string? Title { get; set; }
            public string? DocumentFileName { get; set; }
            public string? DocumentFilePath { get; set; }
            public string? Description { get; set; }
        }

        public class UpdateTestFileDocumentInput 
        {
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
        }
    }
}
