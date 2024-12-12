namespace OJTEDU.Api.Input.CommonControllers
{
    public class EvaluationController
    {
        public class CreateEvaluationInput
        {
            public int? InternshipId { get; set; }
            public string? UniversityFeedback { get; set; }
            public double? UniversityScore { get; set; }
        }
    }
}
