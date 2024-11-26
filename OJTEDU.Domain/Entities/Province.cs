using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Province
    {
        public Province()
        {
            Addresses = new HashSet<Address>();
            Districts = new HashSet<District>();
        }

        public int ProvinceId { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<District> Districts { get; set; }
    }
}
