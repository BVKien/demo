using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OJTEDU.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public CompanyRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Admin - DOET
        public async Task<IEnumerable<Company>> GetAllCompaniesForAdminDoetAsync(string? companyName, string? companyCode, string? status, int? provinceId, int? districtId, int? wardId)
        {
            IQueryable<Company> query = _context.Companies.Include(u => u.User).ThenInclude(u => u.Role)
                                                                      .Include(c => c.Address)
                                                                      .ThenInclude(p => p.Province)
                                                                      .ThenInclude(d => d.Districts)
                                                                      .ThenInclude(w => w.Wards)
                                                                      .Where(c => c.User.Role.Name == "Company" && c.User.Status != "Deleted");

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                companyName = companyName.ToLower();
                query = query.Where(n => n.User.Name.ToLower().Contains(companyName));
            }

            if (!string.IsNullOrWhiteSpace(companyCode))
            {
                companyCode = companyCode.ToLower();
                query = query.Where(n => n.User.UserCode.ToLower().Contains(companyCode));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.User.Status.ToLower().Equals(status));
            }

            // Location filters: Province, District, and Ward
            if (provinceId.HasValue)
            {
                bool provinceExists = await _context.Provinces.AnyAsync(p => p.ProvinceId == provinceId);

                if (!provinceExists)
                {
                    throw new KeyNotFoundException($"Not found province with id: {provinceId}. ");
                }

                query = query.Where(c => c.Address.ProvinceId == provinceId.Value);
            }

            if (districtId.HasValue)
            {
                bool districtExists = await _context.Districts.AnyAsync(d => d.DistrictId == districtId);

                if (!districtExists)
                {
                    throw new KeyNotFoundException($"Not found district with id: {districtId}. ");
                }

                query = query.Where(c => c.Address.DistrictId == districtId.Value);
            }

            if (wardId.HasValue)
            {
                bool wardExists = await _context.Wards.AnyAsync(w => w.WardId == wardId);

                if (!wardExists)
                {
                    throw new KeyNotFoundException($"Not found ward with id: {wardId}. ");
                }

                query = query.Where(c => c.Address.WardId == wardId.Value);
            }

            // Fetch the filtered result from the database
            var companies = await query.ToListAsync();

            if (companies == null)
            {
                throw new KeyNotFoundException("Companies not found.");
            }

            var sortedCompanies = companies.OrderByDescending(u => u.User.Status == "Active")
                                           .ThenByDescending(u => u.User.Status == "Unactive")
                                           .ThenByDescending(u => u.UpdatedAt)
                                           .ToList();

            return sortedCompanies;
        }

        public async Task<Company> GetCompanyDetailForAdminDoetAsync(int? companyId)
        {
            var companies = await GetAllCompaniesForAdminDoetAsync(null, null, null, null, null, null);

            if (companies == null)
            {
                throw new KeyNotFoundException("Companies not found.");
            }

            var company = companies.FirstOrDefault(c => c.CompanyId == companyId);

            if (company == null)
            {
                throw new KeyNotFoundException("Company not found.");
            }

            await _context.Entry(company)
                .Collection(c => c.Jobs)
                .LoadAsync();

            return company;
        }

        public async Task UpdateCompanyForAdminDoetAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        // Guest
        public async Task<(IEnumerable<Company>, int totalRecords)> SearchCompaniesAsync(string? name,
            int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                var query = _context.Companies
                    .Include(c => c.User)
                    .Include(c => c.Address)
                        .ThenInclude(a => a.Province)
                        .ThenInclude(p => p.Districts)
                        .ThenInclude(d => d.Wards)
                    .AsQueryable();

                // Name 
                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(c => c.User.Name.Contains(name));
                }

                // Province 
                if (provinceId != null)
                {
                    bool provinceExists = await _context.Provinces.AnyAsync(p => p.ProvinceId == provinceId);

                    if (!provinceExists)
                    {
                        throw new KeyNotFoundException("Not found province.");
                    }

                    query = query.Where(c => c.Address.ProvinceId == provinceId);
                }

                // Dítrict
                if (districtId != null)
                {
                    bool districtExists = await _context.Districts.AnyAsync(d => d.DistrictId == districtId);

                    if (!districtExists)
                    {
                        throw new KeyNotFoundException("Not found district.");
                    }
                }

                if (provinceId != null && districtId != null)
                {
                    query = query.Where(c => c.Address.ProvinceId == provinceId && c.Address.ProvinceId == provinceId);
                }

                // Ward
                if (wardId != null)
                {
                    bool wardExists = await _context.Wards.AnyAsync(w => w.WardId == wardId);

                    if (!wardExists)
                    {
                        throw new KeyNotFoundException("Not found ward.");
                    }
                }

                if (provinceId != null && districtId != null && wardId != null)
                {
                    query = query.Where(c => c.Address.ProvinceId == provinceId
                    && c.Address.DistrictId == districtId && c.Address.WardId == wardId);
                }

                // Calculate total count before pagination
                int totalRecords = await query.CountAsync();

                // Pagination Logic
                if (pageNumber.HasValue && pageSize.HasValue)
                {
                    int skip = (pageNumber.Value - 1) * pageSize.Value;
                    query = query.Skip(skip).Take(pageSize.Value);
                }

                var companies = await query.ToListAsync();
                return (companies, totalRecords);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Company> GetCompanyDetailByCompanyIdAsync(int? companyId)
        {
            try
            {
                bool companyExists = await _context.Companies
                    .AnyAsync(c => c.CompanyId == companyId);

                if (!companyExists)
                {
                    throw new KeyNotFoundException("Not found company.");
                }

                var company = await _context.Companies
                    .Include(c => c.User)
                    .Include(c => c.Address.Province)
                    .Include(c => c.Address.District)
                    .Include(c => c.Address.Ward)
                    .Where(c => c.CompanyId == companyId)
                    .FirstOrDefaultAsync();

                return company;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Company 
        public async Task<IEnumerable<Company>> GetMentorsListAsync(int? userId)
        {
            try
            {
                var mentors = await _context.Companies
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .Where(c => c.User.ForCompany == userId)
                    .ToListAsync();

                if (mentors == null)
                {
                    throw new KeyNotFoundException("Not found mentors list for company.");
                }

                return mentors;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Company>> GetAllMentorsInfoAsync()
        {
            try
            {
                var mentors = await _context.Companies
                    .Include(c => c.User).ThenInclude(c => c.Role)
                    .Where(c => c.User.Role.Name == "Mentor")
                    .ToListAsync();

                if (mentors == null)
                {
                    throw new KeyNotFoundException("Not found mentors list.");
                }

                return mentors;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
