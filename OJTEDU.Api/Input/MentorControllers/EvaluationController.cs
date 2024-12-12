namespace OJTEDU.Api.Input.MentorControllers
{
    public class EvaluationController
    {
        public class CreateEvaluationInput
        {
            public int? InternshipId { get; set; }
            public string? CompanyFeedback { get; set; }
            public double? CompanyScore { get; set; }
        }
    }
}
