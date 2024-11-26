namespace OJTEDU.Api.Input.MentorControllers
{
    public class WorkingReportController
    {
        public class FeedbackWorkingReportInput
        {
            public string? FeedbackFromMentor { get; set; }
            public double? MentorScore { get; set; }
        }
    }
}
