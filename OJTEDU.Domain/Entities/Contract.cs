using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Contract
    {
        public Contract()
        {
            Internships = new HashSet<Internship>();
        }

        public int ContractId { get; set; }
        public int? ContractTypeId { get; set; }
        public int? CompanyId { get; set; }
        public string? Name { get; set; }
        public string? ContractFile { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Company? Company { get; set; }
        public virtual ContractType? ContractType { get; set; }
        public virtual ICollection<Internship> Internships { get; set; }
    }
}
