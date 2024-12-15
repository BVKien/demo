using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.DocumentDTO;
using static OJTEDU.Application.DTOs.ProvinceDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IProvinceService
    {
        // Common
        Task<DataResponse<LocationListForCommonDTO>> GetAllLocationsAsync(int? provinceId, int? districtId);
    }
}
