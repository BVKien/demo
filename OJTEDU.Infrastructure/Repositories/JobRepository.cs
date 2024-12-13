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
    public class JobRepository : IJobRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _testFileDirectory = "wwwroot/uploads/jobs/testfiles/";
        private readonly IAddressRepository _addressRepository;
        public JobRepository(OJTEDU_DB_V1Context context, IAddressRepository addressRepository)
        {
            _context = context;
            _addressRepository = addressRepository;

            if (!Directory.Exists(_testFileDirectory))
            {
                Directory.CreateDirectory(_testFileDirectory);
            }
        }

        // Student
        public async Task<Dictionary<int?, int>> GetJobCountsByCompanyIdsAsync(int[] companyIds)
        {
            try
            {
                var jobs = await _context.Jobs
                    .Where(j => j.CompanyId.HasValue && companyIds.Contains(j.CompanyId.Value))
                    .ToListAsync();

                var jobCounts = jobs?
                    .GroupBy(j => j.CompanyId)?
                    .ToDictionary(g => g.Key, g => g.Count());

                return jobCounts;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while counting jobs for the given company IDs.", ex);
            }
        }

        public async Task<IEnumerable<Job>> GetAllJobsByCompanyIdAsync(int? companyId)
        {
            try
            {
                bool companyExists = await _context.Companies
                    .AnyAsync(c => c.CompanyId == companyId);

                if (!companyExists)
                {
                    throw new KeyNotFoundException("Not found company.");
                }

                var jobs = await _context.Jobs
                    .Include(j => j.AddressedNavigation.Province)
                    .Where(j => j.CompanyId == companyId)
                    .ToListAsync();

                return jobs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(IEnumerable<Job>, int totalRecords)> SearchJobsAsync(int? userId, string? title, int? majorId,
            int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                // Student   
                bool studentExists = await _context.Users.AnyAsync(s => s.UserId == userId);

                if (!studentExists)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var student = await _context.Students.Include(s => s.User).ThenInclude(s => s.Role).FirstOrDefaultAsync();

                var query = _context.Jobs
                    .Include(j => j.Company)
                        .ThenInclude(j => j.User)
                    .Include(j => j.Major)
                    .Include(j => j.AddressedNavigation)
                        .ThenInclude(j => j.Province)
                        .ThenInclude(j => j.Districts)
                        .ThenInclude(j => j.Wards)
                    .AsQueryable();

                // Title 
                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(j => j.Title.Contains(title));
                }

                // Major 
                if (majorId != null)
                {
                    bool majorExists = await _context.Majors.AnyAsync(m => m.MajorId == majorId);

                    if (!majorExists)
                    {
                        throw new KeyNotFoundException("Not found major.");
                    }

                    query = query.Where(j => j.MajorId == majorId);
                }

                if (!string.IsNullOrEmpty(title) && majorId != null)
                {
                    query = query.Where(j => j.Title.Contains(title) && j.MajorId == majorId);
                }

                // Province 
                if (provinceId != null)
                {
                    bool provinceExists = await _context.Provinces.AnyAsync(p => p.ProvinceId == provinceId);

                    if (!provinceExists)
                    {
                        throw new KeyNotFoundException("Not found province.");
                    }

                    query = query.Where(j => j.AddressedNavigation.ProvinceId == provinceId);
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
                    query = query.Where(j => j.AddressedNavigation.ProvinceId == provinceId && j.AddressedNavigation.DistrictId == districtId);
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
                    query = query.Where(j => j.AddressedNavigation.ProvinceId == provinceId
                    && j.AddressedNavigation.DistrictId == districtId && j.AddressedNavigation.WardId == wardId);
                }

                // Calculate total count before pagination
                int totalRecords = await query.CountAsync();

                query = query.OrderByDescending(j => j.CreatedAt);

                // Pagination Logic
                if (pageNumber.HasValue && pageSize.HasValue)
                {
                    int skip = (pageNumber.Value - 1) * pageSize.Value;
                    query = query.Skip(skip).Take(pageSize.Value);
                }

                var jobs = await query.ToListAsync();

                return (jobs, totalRecords);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            try
            {
                var jobs = await _context.Jobs
                    .Include(j => j.Company)
                        .ThenInclude(j => j.User)
                    .Include(j => j.Major)
                    .Include(j => j.AddressedNavigation)
                        .ThenInclude(j => j.Province)
                        .ThenInclude(j => j.Districts)
                        .ThenInclude(j => j.Wards)
                    .ToListAsync();

                return jobs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // + Company 
        public async Task<Job> GetJobDetailAsync(int? jobId)
        {
            try
            {
                var jobs = await _context.Jobs
                    .Include(j => j.Company)
                        .ThenInclude(j => j.User)
                    .Include(j => j.Major)
                    .Include(j => j.AddressedNavigation)
                        .ThenInclude(j => j.Province)
                        .ThenInclude(j => j.Districts)
                        .ThenInclude(j => j.Wards)
                    .Where(j => j.JobId == jobId)
                    .FirstOrDefaultAsync();

                if (jobs == null)
                {
                    throw new KeyNotFoundException("Not found job.");
                }

                return jobs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Company 
        public async Task<IEnumerable<Job>> GetAllJobsByUserIdAsync(int? userId)
        {
            try
            {
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

                if (company == null)
                {
                    throw new KeyNotFoundException("Not found company.");
                }

                var jobs = await _context.Jobs
                    .Include(j => j.Company)
                        .ThenInclude(j => j.User)
                    .Include(j => j.Major)
                    .Include(j => j.AddressedNavigation)
                        .ThenInclude(j => j.Province)
                        .ThenInclude(j => j.Districts)
                        .ThenInclude(j => j.Wards)
                    .Where(j => j.CompanyId == company.CompanyId)
                    .ToListAsync();

                return jobs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Job> CreateJobAsync(int? userId, string? fileName, Job? info, Address? addressInfo)
        {
            try
            {
                // Comapny 
                var company = await _context.Companies.Include(c => c.User).ThenInclude(c => c.Role).FirstOrDefaultAsync(c => c.UserId == userId);

                if (company == null)
                {
                    throw new KeyNotFoundException("Not found company.");
                }

                if (info?.Addressed == null)
                {
                    bool isValidAddress = await _addressRepository.IsValidAddressAsync(addressInfo?.WardId, addressInfo?.DistrictId, addressInfo?.ProvinceId);

                    if (!isValidAddress)
                    {
                        throw new Exception($"Not valid address with Ward id {addressInfo?.WardId}; District id: {addressInfo?.DistrictId}; Province id: {addressInfo?.ProvinceId}.");
                    }

                    // Address
                    var address = new Address
                    {
                        Detail = addressInfo?.Detail,
                        WardId = addressInfo?.WardId,
                        DistrictId = addressInfo?.DistrictId,
                        ProvinceId = addressInfo?.ProvinceId,
                        Status = "1",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Addresses.AddAsync(address);
                    await _context.SaveChangesAsync();

                    var jobValid = new Job
                    {
                        CompanyId = company.CompanyId,
                        Title = info?.Title,
                        Description = info?.Description,
                        SalaryRange = info?.SalaryRange,
                        Requirements = info?.Requirements,
                        SkillRequirements = info?.SkillRequirements,
                        Benefits = info?.Benefits,
                        WorkingHours = info?.WorkingHours,
                        Deadline = info?.Deadline,
                        Status = "1",
                        MajorId = info?.MajorId,
                        Addressed = address.AddressId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Jobs.AddAsync(jobValid);
                    await _context.SaveChangesAsync();

                    // Test file 
                    //// Create file name format jobId_timestamp_filename
                    //var timestampValid = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    //var newFileNameValid = fileName != null ? $"{jobValid.JobId}_{timestampValid}_{fileName}" : null;

                    //var filePathValid = newFileNameValid != null ? Path.Combine(_testFileDirectory, newFileNameValid) : null;

                    //// Save files to folders
                    //if (fileData != null && filePathValid != null)
                    //{
                    //    await File.WriteAllBytesAsync(filePathValid, fileData);
                    //}

                    //// If null 
                    //if (fileName == null || fileData == null)
                    //{
                    //    filePathValid = null;
                    //}

                    jobValid.TestFile = fileName;

                    _context.Jobs.Update(jobValid);
                    await _context.SaveChangesAsync();

                    return jobValid;
                }

                // Company address 
                var addressId = (int)info?.Addressed;
                var companyAddress = await _context.Companies.FirstOrDefaultAsync(c => c.AddressId == addressId && c.UserId == userId);

                if (companyAddress == null)
                {
                    throw new KeyNotFoundException("Not found company address.");
                }

                var job = new Job
                {
                    CompanyId = company.CompanyId,
                    Title = info?.Title,
                    Description = info?.Description,
                    SalaryRange = info?.SalaryRange,
                    Requirements = info?.Requirements,
                    SkillRequirements = info?.SkillRequirements,
                    Benefits = info?.Benefits,
                    WorkingHours = info?.WorkingHours,
                    Deadline = info?.Deadline,
                    Status = "1",
                    MajorId = info?.MajorId,
                    Addressed = info?.Addressed,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                await _context.Jobs.AddAsync(job);
                await _context.SaveChangesAsync();

                // Test file 
                //// Create file name format jobId_timestamp_filename
                //var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //var newFileName = fileName != null ? $"{job.JobId}_{timestamp}_{fileName}" : null;

                //var filePath = newFileName != null ? Path.Combine(_testFileDirectory, newFileName) : null;

                //// Save files to folders
                //if (fileData != null && filePath != null)
                //{
                //    await File.WriteAllBytesAsync(filePath, fileData);
                //}

                //// If null 
                //if (fileName == null || fileData == null)
                //{
                //    filePath = null;
                //}

                job.TestFile = fileName;

                _context.Jobs.Update(job);
                await _context.SaveChangesAsync();

                return job;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Job> UpdateJobAsync(int? userId, int? jobId, string? fileName, Job? info, Address? addressInfo)
        {
            try
            {
                // Job 
                var job = await _context.Jobs
                    .Include(j => j.Company).ThenInclude(c => c.User).ThenInclude(c => c.Role)
                    .Include(j => j.Major)
                    .Include(j => j.AddressedNavigation)
                        .ThenInclude(j => j.Province)
                        .ThenInclude(j => j.Districts)
                        .ThenInclude(j => j.Wards)
                    .FirstOrDefaultAsync(j => j.JobId == jobId);

                if (job == null)
                {
                    throw new KeyNotFoundException("Not found job.");
                }

                // Major
                var majorId = (int)info?.MajorId;
                bool majorExists = await _context.Majors.AnyAsync(m => m.MajorId == majorId);

                if (!majorExists)
                {
                    throw new KeyNotFoundException("Not found major.");
                }

                // Check address
                var address = await _context.Companies.Include(c => c.User).ThenInclude(c => c.Role)
                    .FirstOrDefaultAsync(c => c.AddressId == job.AddressedNavigation.AddressId && c.UserId == userId && c.User.Role.Name == "Company");

                // Test file 
                //// Create file name format jobId_timestamp_filename
                //var timestampValid = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //var newFileNameValid = fileName != null ? $"{job.JobId}_{timestampValid}_{fileName}" : null;

                //var filePathValid = newFileNameValid != null ? Path.Combine(_testFileDirectory, newFileNameValid) : null;

                //// Save files to folders
                //if (fileData != null && filePathValid != null)
                //{
                //    await File.WriteAllBytesAsync(filePathValid, fileData);
                //}

                //// If null 
                //if (fileName == null || fileData == null)
                //{
                //    filePathValid = null;
                //}

                var filePath = fileName;

                // If address company -> add new update address
                if (address != null)
                {
                    bool isValidNewAddress = await _addressRepository.IsValidAddressAsync(addressInfo?.WardId, addressInfo?.DistrictId, addressInfo?.ProvinceId);

                    if (!isValidNewAddress)
                    {
                        throw new Exception($"Not valid address with Ward id {addressInfo?.WardId}; District id: {addressInfo?.DistrictId}; Province id: {addressInfo?.ProvinceId}.");
                    }

                    // Address
                    var newAddress = new Address
                    {
                        Detail = addressInfo?.Detail,
                        WardId = addressInfo?.WardId,
                        DistrictId = addressInfo?.DistrictId,
                        ProvinceId = addressInfo?.ProvinceId,
                        Status = "1",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };

                    await _context.Addresses.AddAsync(newAddress);
                    await _context.SaveChangesAsync();

                    if (filePath == null)
                    {
                        // Update 
                        job.Title = info?.Title;
                        job.Description = info?.Description;
                        job.SalaryRange = info?.SalaryRange;
                        job.Requirements = info?.Requirements;
                        job.SkillRequirements = info?.SkillRequirements;
                        job.Benefits = info?.Benefits;
                        job.WorkingHours = info?.WorkingHours;
                        job.Deadline = info?.Deadline;
                        job.MajorId = info?.MajorId;
                        job.Addressed = newAddress.AddressId;
                        job.UpdatedAt = DateTime.Now;

                        await _context.SaveChangesAsync();

                        return job;
                    }

                    // Update 
                    job.Title = info?.Title;
                    job.Description = info?.Description;
                    job.TestFile = filePath;
                    job.SalaryRange = info?.SalaryRange;
                    job.Requirements = info?.Requirements;
                    job.SkillRequirements = info?.SkillRequirements;
                    job.Benefits = info?.Benefits;
                    job.WorkingHours = info?.WorkingHours;
                    job.Deadline = info?.Deadline;
                    job.MajorId = info?.MajorId;
                    job.Addressed = newAddress.AddressId;
                    job.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return job;
                }

                // If != address company -> add new update address
                var updateAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == job.AddressedNavigation.AddressId);

                if (updateAddress == null)
                {
                    throw new KeyNotFoundException("Not found address.");
                }

                // Check update address valid 
                bool isValidAddress = await _addressRepository.IsValidAddressAsync(addressInfo?.WardId, addressInfo?.DistrictId, addressInfo?.ProvinceId);

                if (!isValidAddress)
                {
                    throw new Exception($"Not valid address with Ward id {addressInfo?.WardId}; District id: {addressInfo?.DistrictId}; Province id: {addressInfo?.ProvinceId}.");
                }

                // Update address infomation
                updateAddress.Detail = addressInfo?.Detail;
                updateAddress.WardId = addressInfo?.WardId;
                updateAddress.DistrictId = addressInfo?.DistrictId;
                updateAddress.ProvinceId = addressInfo?.ProvinceId;
                updateAddress.Status = "1";
                updateAddress.UpdatedAt = DateTime.Now;

                if (filePath == null)
                {
                    // Update job infomation
                    job.Title = info?.Title;
                    job.Description = info?.Description;
                    job.SalaryRange = info?.SalaryRange;
                    job.Requirements = info?.Requirements;
                    job.SkillRequirements = info?.SkillRequirements;
                    job.Benefits = info?.Benefits;
                    job.WorkingHours = info?.WorkingHours;
                    job.Deadline = info?.Deadline;
                    job.MajorId = info?.MajorId;
                    job.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return job;
                }

                // Update job infomation
                job.Title = info?.Title;
                job.Description = info?.Description;
                job.TestFile = filePath;
                job.SalaryRange = info?.SalaryRange;
                job.Requirements = info?.Requirements;
                job.SkillRequirements = info?.SkillRequirements;
                job.Benefits = info?.Benefits;
                job.WorkingHours = info?.WorkingHours;
                job.Deadline = info?.Deadline;
                job.MajorId = info?.MajorId;
                job.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return job;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
