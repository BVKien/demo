using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Evaluation
    {
        public Evaluation()
        {
            Internships = new HashSet<Internship>();
        }

        public int EvaluationId { get; set; }
        public int? MentorId { get; set; }
        public int? LecturerId { get; set; }
        public int? StudentId { get; set; }
        public string? CompanyComment { get; set; }
        public string? DeanComment { get; set; }
        public double? CompanyScore { get; set; }
        public double? DeanScore { get; set; }
        public double? EvaluationScore { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User? Lecturer { get; set; }
        public virtual Company? Mentor { get; set; }
        public virtual Student? Student { get; set; }
        public virtual ICollection<Internship> Internships { get; set; }
    }
}
