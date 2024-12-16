namespace OJTEDU.Api.Input.CommonControllers
{
    public class EvaluationController
    {
        public class CreateEvaluationInput
        {
            public int? InternshipId { get; set; }
            public string? CompanyComment { get; set; }
            public double? CompanyScore { get; set; }
            public string? DeanComment { get; set; }
            public double? DeanScore { get; set; }
        }
    }
}
