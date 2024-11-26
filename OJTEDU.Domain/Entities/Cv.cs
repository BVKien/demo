using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Cv
    {
        public Cv()
        {
            Appllications = new HashSet<Appllication>();
        }

        public int CvId { get; set; }
        public string? Name { get; set; }
        public string? CvFile { get; set; }
        public string? PersonalInformation { get; set; }
        public string? CareerObjective { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public string? Certification { get; set; }
        public string? ExtracurricularActivities { get; set; }
        public string? Project { get; set; }
        public string? PersonalInterest { get; set; }
        public string? Skill { get; set; }
        public string? Status { get; set; }
        public int? StudentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Student? Student { get; set; }
        public virtual ICollection<Appllication> Appllications { get; set; }
    }
}
