using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class ContractType
    {
        public ContractType()
        {
            Contracts = new HashSet<Contract>();
        }

        public int ContractTypeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
