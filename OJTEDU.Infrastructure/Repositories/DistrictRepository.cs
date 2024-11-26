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
    public class DistrictRepository : IDistrictRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public DistrictRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Common
        public async Task<IEnumerable<District>> GetAllDistrictsByProvinceIdAsync(int? provinceId)
        {
            try
            {
                bool provinceExists = await _context.Provinces.AnyAsync(p => p.ProvinceId == provinceId);

                if (!provinceExists)
                {
                    throw new KeyNotFoundException($"Not found province with id: {provinceId}. ");
                }

                var districts = await _context.Districts
                    .Where(d => d.ProvinceId == provinceId)
                    .ToListAsync();

                return districts;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving districts. " + ex.Message);
            }
        }
    }
}
