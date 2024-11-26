using System;
using System.Collections.Generic;

namespace OJTEDU.Domain.Entities
{
    public partial class Address
    {
        public Address()
        {
            Companies = new HashSet<Company>();
            Jobs = new HashSet<Job>();
            Students = new HashSet<Student>();
        }

        public int AddressId { get; set; }
        public string? Detail { get; set; }
        public int? WardId { get; set; }
        public int? DistrictId { get; set; }
        public int? ProvinceId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual District? District { get; set; }
        public virtual Province? Province { get; set; }
        public virtual Ward? Ward { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
