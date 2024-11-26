using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class District
    {
        public District()
        {
            Addresses = new HashSet<Address>();
            Wards = new HashSet<Ward>();
        }

        public int DistrictId { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public int? ProvinceId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Province? Province { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Ward> Wards { get; set; }
    }
}
