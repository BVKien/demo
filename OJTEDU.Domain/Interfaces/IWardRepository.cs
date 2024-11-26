using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IWardRepository
    {
        // Common 
        Task<IEnumerable<Ward>> GetAllWardsByDistrictIdAsync(int? districtId);
    }
}
