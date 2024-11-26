using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class WardRepository : IWardRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public WardRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Common
        public async Task<IEnumerable<Ward>> GetAllWardsByDistrictIdAsync(int? districtId)
        {
            try
            {
                bool districtExists = await _context.Districts.AnyAsync(d => d.DistrictId == districtId);

                if (!districtExists)
                {
                    throw new KeyNotFoundException($"Not found ward with id: {districtId}. ");
                }

                var wards = await _context.Wards
                    .Where(w => w.DistrictId == districtId)
                    .ToListAsync();

                return wards;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving ward. " + ex.Message);
            }
        }
    }
}
