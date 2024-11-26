using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.DistrictDTO;
using static OJTEDU.Application.DTOs.WardDTO;

namespace OJTEDU.Application.DTOs
{
    public class ProvinceDTO
    {
        // Common
        public class LocationListForCommonDTO
        {
            public List<ProvinceListForCommonDTO>? ProvinceList { get; set; }
            public List<DistrictListForCommonDTO>? DistrictList { get; set; }
            public List<WardListForCommonDTO>? WardList { get; set; }
        }

        public class ProvinceListForCommonDTO
        {
            public int ProvinceId { get; set; }
            public string? Name { get; set; }
        }
    }
}
