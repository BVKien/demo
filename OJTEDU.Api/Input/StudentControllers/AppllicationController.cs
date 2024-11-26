namespace OJTEDU.Api.Input.StudentControllers
{
    public class AppllicationController
    {
        public class ApplyJobInput
        {
            public int? JobId { get; set; }
            public string? TestFile { get; set; }
            public string? CoverLetter { get; set; }
            public int? CvId { get; set; }
            public string? CvFile { get; set; }
        }
    }
}
