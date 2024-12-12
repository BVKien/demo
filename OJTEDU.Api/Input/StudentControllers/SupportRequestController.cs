namespace OJTEDU.Api.Input.StudentControllers
{
    public class SupportRequestController
    {
        public class CreateSupportRequestInput
        {
            public string? RequestTitle { get; set; }
            public string? RequestContent { get; set; }
        }
    }
}
