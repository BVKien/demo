using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Ward
    {
        public Ward()
        {
            Addresses = new HashSet<Address>();
        }

        public int WardId { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public int? DistrictId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual District? District { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}
