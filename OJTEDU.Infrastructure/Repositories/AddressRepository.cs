using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;

namespace OJTEDU.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly OJTEDU_DB_V1Context _context;

        public AddressRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Admin - Province Management
        public async Task<IEnumerable<Province>> GetAllProvinceAsync(string? name, string? status)
        {
            IQueryable<Province> query = _context.Provinces;

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var provinces = await query.ToListAsync();

            if (provinces == null)
            {
                throw new KeyNotFoundException("Provinces not found.");
            }

            var sortedProvinces = provinces.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.UpdatedAt)
                                           .ToList();

            return sortedProvinces;
        }

        public async Task<Province> GetProvinceByIdAsync(int provinceId)
        {
            return await _context.Provinces.FirstOrDefaultAsync(c => c.ProvinceId == provinceId);
        }

        public async Task<Province> GetProvinceByNameAsync(string? name)
        {
            return await _context.Provinces.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task AddProvinceAsync(Province province)
        {
            await _context.Provinces.AddAsync(province);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProvinceAsync(Province province)
        {
            _context.Provinces.Update(province);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProvinceAsync(Province province)
        {
            _context.Provinces.Remove(province);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckProvinceDependenciesAsync(int provinceId)
        {
            bool hasDistricts = await _context.Districts.AnyAsync(e => e.ProvinceId == provinceId);
            bool hasAddresses = await _context.Addresses.AnyAsync(p => p.ProvinceId == provinceId);

            return hasDistricts || hasAddresses;
        }

        // Admin - District Management
        public async Task<IEnumerable<District>> GetAllDistrictByProvinceIdAsync(int provinceId, string? name)
        {
            IQueryable<District> query = _context.Districts.Include(d => d.Province).Where(d => d.ProvinceId == provinceId);

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(name));
            }

            // Fetch the filtered result from the database
            var districts = await query.ToListAsync();

            if (districts == null)
            {
                throw new KeyNotFoundException("Districts not found.");
            }

            var sortedDistricts = districts.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.UpdatedAt)
                                           .ToList();

            return sortedDistricts;
        }

        public async Task<District> GetDistrictByIdAsync(int districtId)
        {
            return await _context.Districts.Include(d => d.Province).FirstOrDefaultAsync(c => c.DistrictId == districtId);
        }

        public async Task<District> GetDistrictByNameAsync(string? name)
        {
            return await _context.Districts.Include(d => d.Province).FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task AddDistrictAsync(District district)
        {
            await _context.Districts.AddAsync(district);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDistrictAsync(District district)
        {
            _context.Districts.Update(district);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDistrictAsync(District district)
        {
            _context.Districts.Remove(district);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckDistrictDependenciesAsync(int districtId)
        {
            bool hasWards = await _context.Wards.AnyAsync(e => e.DistrictId == districtId);
            bool hasAddresses = await _context.Addresses.AnyAsync(p => p.DistrictId == districtId);

            return hasWards || hasAddresses;
        }

        // Admin - Ward Management
        public async Task<IEnumerable<Ward>> GetAllWardByDistrictIdAsync(int districtId, string? name)
        {
            IQueryable<Ward> query = _context.Wards.Include(d => d.District).Where(d => d.DistrictId == districtId);

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(name));
            }

            // Fetch the filtered result from the database
            var wards = await query.ToListAsync();

            if (wards == null)
            {
                throw new KeyNotFoundException("Wards not found.");
            }

            var sortedWards = wards.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.UpdatedAt)
                                           .ToList();

            return sortedWards;
        }

        public async Task<Ward> GetWardByIdAsync(int wardId)
        {
            return await _context.Wards.Include(d => d.District).FirstOrDefaultAsync(c => c.WardId == wardId);
        }

        public async Task<Ward> GetWardByNameAsync(string? name)
        {
            return await _context.Wards.Include(d => d.District).FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task AddWardAsync(Ward ward)
        {
            await _context.Wards.AddAsync(ward);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWardAsync(Ward ward)
        {
            _context.Wards.Update(ward);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWardAsync(Ward ward)
        {
            _context.Wards.Remove(ward);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckWardDependenciesAsync(int wardId)
        {
            bool hasAddresses = await _context.Addresses.AnyAsync(p => p.WardId == wardId);

            return hasAddresses;
        }


        //

        public async Task<int> AddAddressAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return address.AddressId;
        }

        public async Task<int> UpdateAddressAsync(Address address)
        {
            var existingAddress = await _context.Addresses.FindAsync(address.AddressId);
            if (existingAddress == null)
            {
                throw new KeyNotFoundException("Address For Company not found");
            }
            existingAddress.ProvinceId = address.ProvinceId;
            existingAddress.DistrictId = address.DistrictId;
            existingAddress.WardId = address.WardId;
            existingAddress.Detail = address.Detail;
            existingAddress.UpdatedAt = DateTime.Now;

            _context.Addresses.Update(existingAddress);
            await _context.SaveChangesAsync();
            return existingAddress.AddressId;
        }

        // Common
        public async Task<bool> IsValidAddressAsync(int? wardId, int? districtId, int? provinceId)
        {
            if (wardId == null || districtId == null || provinceId == null)
            {
                return false;
            }

            var ward = await _context.Wards.FirstOrDefaultAsync(w => w.WardId == wardId && w.DistrictId == districtId);

            if (ward == null)
            {
                return false;
            }

            var district = await _context.Districts.FirstOrDefaultAsync(d => d.DistrictId == districtId && d.ProvinceId == provinceId);

            if (district == null)
            {
                return false;
            }

            return true;
        }
    }
}
