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
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public ProvinceRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Common 
        public async Task<IEnumerable<Province>> GetAllProvincesAsync()
        {
            try
            {
                var provinces = await _context.Provinces.ToListAsync();
                return provinces;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving provinces. " + ex.Message);
            }
        }
    }
}
