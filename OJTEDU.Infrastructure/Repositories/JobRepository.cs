using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        // === Document ===
        // Admin
        public async Task<IEnumerable<Document>> GetAllDocumentsForAdminAsync(string? title, int? roleId, string? status)
        {
            IQueryable<Document> query = _context.Documents.Include(d => d.DocumentRoles).ThenInclude(dr => dr.Role).Include(u => u.University)
                                                       .Where(u => (u.University.Role.Name.Equals("Admin") || u.University.Role.Name.Equals("DOET")));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var documents = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (documents == null)
            {
                throw new KeyNotFoundException("Documents not found.");
            }

            var sortedDocuments = documents.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.DocumentId)
                                           .ToList();

            return sortedDocuments;
        }

        public async Task<Document> GetDocumentByIdForAdminAsync(int documentId)
        {
            var document = await _context.Documents.Include(u => u.DocumentRoles).ThenInclude(dr => dr.Role).Include(u => u.University)
                                                   .FirstOrDefaultAsync(u => (u.University.Role.Name.Equals("Admin") || u.University.Role.Name.Equals("DOET")) && u.DocumentId == documentId);
            if (document == null)
            {
                throw new KeyNotFoundException("Document not found");
            }
            return document;
        }

        public async Task<Document> AddDocumentForAdminAsync(Document document, List<int?> roleIds)
        {
            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Thêm tài liệu vào bảng Document
                document.CreatedAt = DateTime.Now;
                document.UpdatedAt = DateTime.Now;
                document.Status = "Active"; // Mặc định trạng thái là Active
                await _context.Documents.AddAsync(document);
                await _context.SaveChangesAsync();

                // Thêm các bản ghi vào bảng DocumentRoles
                foreach (var roleId in roleIds)
                {
                    var documentRole = new DocumentRole
                    {
                        DocumentId = document.DocumentId,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.DocumentRoles.AddAsync(documentRole);
                }

                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return document;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding document with roles: {ex.Message}");
            }
        }

        public async Task UpdateDocumentRolesAsync(int documentId, List<int?> newRoleIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Xóa các DocumentRoles hiện tại
                var existingRoles = _context.DocumentRoles.Where(dr => dr.DocumentId == documentId).ToList();
                _context.DocumentRoles.RemoveRange(existingRoles);
                await _context.SaveChangesAsync();

                // Thêm mới DocumentRoles
                foreach (var roleId in newRoleIds)
                {
                    var documentRole = new DocumentRole
                    {
                        DocumentId = documentId,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.DocumentRoles.AddAsync(documentRole);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error updating roles for document: {ex.Message}");
            }
        }

        public async Task<Document> UpdateDocumentForAdminAsync(Document document)
        {
            var existingDocument = await GetDocumentByIdForAdminAsync(document.DocumentId);
            if (existingDocument == null)
            {
                throw new KeyNotFoundException("Document not found");
            }

            existingDocument.Title = document.Title ?? existingDocument.Title;
            existingDocument.DocumentFile = document.DocumentFile ?? existingDocument.DocumentFile;
            existingDocument.Description = document.Description ?? existingDocument.Description;
            existingDocument.UniversityId = document.UniversityId ?? existingDocument.UniversityId;
            existingDocument.Status = document.Status ?? existingDocument.Status;
            existingDocument.UpdatedAt = DateTime.Now;

            _context.Documents.Update(existingDocument);
            await _context.SaveChangesAsync();
            return existingDocument;
        }

        public async Task<Document> DeleteDocumentForAdminAsync(int documentId)
        {
            var document = await GetDocumentByIdForAdminAsync(documentId);
            if (document == null)
            {
                throw new KeyNotFoundException("Document not found in the list.");
            }

            var documentRoles = _context.DocumentRoles.Where(dr => dr.DocumentId == documentId).ToList();
            if (documentRoles.Any())
            {
                _context.DocumentRoles.RemoveRange(documentRoles);
            }

            document.DeletedAt = DateTime.Now; // Cập nhật thời gian xóa

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return document;
        }

        // Doet

        public async Task<IEnumerable<Document>> GetAllDocumentsForDoetAsync(string? title, int? roleId, string? status)
        {
            IQueryable<Document> query = _context.Documents.Include(d => d.DocumentRoles).ThenInclude(dr => dr.Role).Include(u => u.University)
                                                       .Where(u => u.University.Role.Name.Equals("DOET"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var documents = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (documents == null)
            {
                throw new KeyNotFoundException("Documents not found.");
            }

            var sortedDocuments = documents.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.DocumentId)
                                           .ToList();

            return sortedDocuments;
        }

        public async Task<Document> GetDocumentByIdForDoetAsync(int documentId)
        {
            var document = await _context.Documents.Include(u => u.DocumentRoles).ThenInclude(dr => dr.Role).Include(u => u.University)
                                                   .FirstOrDefaultAsync(u => u.University.Role.Name.Equals("DOET") && u.DocumentId == documentId);
            if (document == null)
            {
                throw new KeyNotFoundException("Document not found");
            }
            return document;
        }

        public async Task<Document> AddDocumentForDoetAsync(Document document, List<int?> roleIds)
        {
            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Thêm tài liệu vào bảng Document
                document.CreatedAt = DateTime.Now;
                document.UpdatedAt = DateTime.Now;
                document.Status = "Active"; // Mặc định trạng thái là Active
                await _context.Documents.AddAsync(document);
                await _context.SaveChangesAsync();

                // Thêm các bản ghi vào bảng DocumentRoles
                foreach (var roleId in roleIds)
                {
                    var documentRole = new DocumentRole
                    {
                        DocumentId = document.DocumentId,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.DocumentRoles.AddAsync(documentRole);
                }

                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return document;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding document with roles: {ex.Message}");
            }
        }

        public async Task<Document> UpdateDocumentForDoetAsync(Document document)
        {
            var existingDocument = await GetDocumentByIdForDoetAsync(document.DocumentId);
            if (existingDocument == null)
            {
                throw new KeyNotFoundException("Document not found");
            }

            existingDocument.Title = document.Title ?? existingDocument.Title;
            existingDocument.DocumentFile = document.DocumentFile ?? existingDocument.DocumentFile;
            existingDocument.Description = document.Description ?? existingDocument.Description;
            existingDocument.UniversityId = document.UniversityId ?? existingDocument.UniversityId;
            existingDocument.Status = document.Status ?? existingDocument.Status;
            existingDocument.UpdatedAt = DateTime.Now;

            _context.Documents.Update(existingDocument);
            await _context.SaveChangesAsync();
            return existingDocument;

        }

        public async Task<Document> DeleteDocumentForDoetAsync(int documentId)
        {
            var document = await GetDocumentByIdForDoetAsync(documentId);
            if (document == null)
            {
                throw new KeyNotFoundException("Document not found in the list.");
            }

            var documentRoles = _context.DocumentRoles.Where(dr => dr.DocumentId == documentId).ToList();
            if (documentRoles.Any())
            {
                _context.DocumentRoles.RemoveRange(documentRoles);
            }

            document.DeletedAt = DateTime.Now; // Cập nhật thời gian xóa

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return document;
        }

        //// Common 
        public async Task<IEnumerable<Document>> GetAllDocumentsAsync(string role, string? title)
        {
            var documentsQuery = _context.Documents
                                 .Include(d => d.DocumentRoles).ThenInclude(dr => dr.Role)
                                 .Include(d => d.University)
                                 .Where(d => d.Status == "Active");

            if (role == "guest")
            {
                // Nếu role là "guest", chỉ lấy tài liệu có RoleId là null và sắp xếp theo DocumentId giảm dần
                documentsQuery = documentsQuery.Where(d => d.DocumentRoles.All(dr => dr.RoleId == null));
            }
            else
            {
                // Nếu role không phải là "guest", lấy cả tài liệu dành cho vai trò của người dùng và tài liệu dành cho guest
                documentsQuery = documentsQuery.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            if (!string.IsNullOrEmpty(title))
            {
                title = title.ToLower();
                documentsQuery = documentsQuery.Where(n => n.Title.ToLower().Contains(title));
            }

            // Sắp xếp: Tài liệu của vai trò đăng nhập lên đầu, sau đó mới đến tài liệu dành cho guest
            documentsQuery = documentsQuery
                .OrderByDescending(d => d.DocumentRoles.Any(dr => dr.Role != null && dr.Role.Name.Equals(role))) // Vai trò đăng nhập lên đầu
                .ThenBy(d => d.DocumentRoles.Any(dr => dr.RoleId == null)) // Sau đó là guest
                .ThenByDescending(d => d.DocumentId); // Sắp xếp theo DocumentId giảm dần

            var documentsList = await documentsQuery.ToListAsync();

            if (documentsList == null)
            {
                throw new KeyNotFoundException("No documents found for the specified role.");
            }

            return documentsList;
        }

        public async Task<Document> GetDocumentDetailAsync(int? documentId, string role)
        {
            var allDocuments = await GetAllDocumentsAsync(role, null);

            var documentDetail = allDocuments.FirstOrDefault(n => n.DocumentId == documentId);

            if (documentDetail == null)
            {
                throw new KeyNotFoundException("Document detail not found.");
            }

            return documentDetail;
        }

        //// Guest
        //public async Task<Document> GetInternshipProcessDocumentAsync()
        //{
        //    try
        //    {
        //        var document = await _context.Documents
        //            .Include(d => d.University)
        //            .Where(d => d.RoleId == null)
        //            .FirstOrDefaultAsync();

        //        return document;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"An error occurred while get internship process document. " + ex.Message);
        //    }
        //}

        // Company
        public async Task<Document> CreateDocumentsByUserIdAsync(int? userId, string? fileName, string? fileData, Document? info)
        {
            try
            {
                var company = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Company");
                if (company == null)
                {
                    throw new KeyNotFoundException("Not found company.");
                }

                //// Create file name format userId_timestamp_filename
                //var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //var newFileName = fileName != null ? $"{userId}_{timestamp}_{fileName}" : null;

                //var filePath = newFileName != null ? Path.Combine(_documentDirectory, newFileName) : null;

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

                var document = new Document
                {
                    UserId = userId,
                    Title = info?.Title,
                    DocumentFile = fileData,
                    Description = info?.Description,
                    Status = "Active",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _context.Documents.AddAsync(document);
                await _context.SaveChangesAsync();

                return document;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> StoredDocumentsByUserIdAsync(int? documentId)
        {
            try
            {
                var document = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId);
                if (document == null)
                {
                    throw new KeyNotFoundException("Not found document.");
                }

                document.Status = "Deleted";
                document.UpdatedAt = DateTime.Now;
                document.DeletedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsByUserIdAsync(int? userId)
        {
            try
            {
                var documents = await _context.Documents
                    .Where(d => d.UserId == userId && d.Status == "Active")
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync();
                if (documents == null)
                {
                    throw new KeyNotFoundException("Not found test files list.");
                }

                return documents;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Document> UpdateDocumentAsync(int? documentId, string? fileName, byte[] fileData, Document? info)
        {
            try
            {
                var document = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId);
                if (document == null)
                {
                    throw new KeyNotFoundException("Not found document.");
                }

                //// Create file name format documentId_timestamp_filename
                //var timestampValid = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //var newFileNameValid = fileName != null ? $"{document.DocumentId}_{timestampValid}_{fileName}" : null;

                //var filePathValid = newFileNameValid != null ? Path.Combine(_documentDirectory, newFileNameValid) : null;

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

                //var filePath = filePathValid?.Replace("wwwroot", "");

                if (fileName == null)
                {
                    document.Title = info?.Title;
                    document.Description = info?.Description;
                    document.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return document;
                }

                document.Title = info?.Title;
                document.DocumentFile = fileName;
                document.DocumentFile = info?.DocumentFile;
                document.Description = info?.Description;
                document.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return document;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // === Student ===
        // Student 
        public async Task<Student> GetStudentDetailByUserIdAsync(int? userId)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var student = await _context.Students
                    .Include(s => s.User)
                    .Include(s => s.Lecturer)
                    .Include(s => s.Semester)
                    .Include(s => s.Major)
                    .Include(s => s.Address)
                        .ThenInclude(a => a.Ward)
                        .ThenInclude(a => a.District)
                        .ThenInclude(a => a.Province)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                return student;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Student> UpdateStudentByUserIdAsync(int? userId, User? updateUser, Student? updateInformation, Address? updateAddress)
        {
            if (userId == null || updateInformation == null)
            {
                throw new ArgumentNullException("User id or update information cannot be null.");
            }

            try
            {
                // Check if user exists
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                // Find the student by userId
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                // Update Student information
                // TBL User
                user.Image = updateUser.Image ?? user.Image;
                user.UpdatedAt = DateTime.Now; // Update the timestamp

                // TBL Student
                student.AlternativeEmail = updateInformation.AlternativeEmail ?? student.AlternativeEmail;
                student.Phone = updateInformation.Phone ?? student.Phone;
                student.Dob = updateInformation.Dob ?? student.Dob;
                student.Gender = updateInformation.Gender ?? student.Gender;

                // TBL Address
                if (updateAddress != null && student.AddressId.HasValue)
                {
                    var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == student.AddressId.Value);

                    if (address == null)
                    {
                        throw new KeyNotFoundException("Not found address.");
                    }

                    // Update address details
                    address.Detail = updateAddress.Detail ?? address.Detail;
                    address.WardId = updateAddress.WardId ?? address.WardId;
                    address.DistrictId = updateAddress.DistrictId ?? address.DistrictId;
                    address.ProvinceId = updateAddress.ProvinceId ?? address.ProvinceId;
                    address.UpdatedAt = DateTime.Now; // Update the timestamp
                }

                student.UpdatedAt = DateTime.Now; // Update the timestamp

                // Save all changes to the database
                await _context.SaveChangesAsync();

                return student;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //For Dean
        public async Task<User> GetDeanByUserIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Dean");
        }
        public async Task<IEnumerable<Student>> GetStudentListAsync(
        int userId,
        string role,
        string? code,
        string? studentName,
        string? lecturerName,
        string? majorName,
        string? sortBy,
        bool? isDescending)
        {
            // Khởi tạo truy vấn
            IQueryable<Student> query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .Include(s => s.Lecturer)
                .Where(s => s.User.Status != "Deleted");

            if (role == "Dean")
            {
                // Lấy thông tin Dean và kiểm tra hợp lệ
                var dean = await _context.Users
                    .Include(u => u.Department) // Kết nối với bảng Department
                    .FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Dean");

                if (dean == null || !dean.DepartmentId.HasValue)
                {
                    throw new KeyNotFoundException("Dean not found or department not assigned.");
                }

                // Lấy danh sách MajorId thuộc Department của Dean
                var majorIdsInDepartment = await _context.Majors
                    .Where(m => m.DepartmentId == dean.DepartmentId && m.Status == "Active")
                    .Select(m => m.MajorId)
                    .ToListAsync();

                if (!majorIdsInDepartment.Any())
                {
                    throw new KeyNotFoundException("Dean does not manage any majors.");
                }

                // Lọc danh sách sinh viên theo MajorId trong Department của Dean
                query = query.Where(s => s.MajorId.HasValue && majorIdsInDepartment.Contains(s.MajorId.Value));

            }
            else if (role == "Lecturer")
            {
                // Lọc theo LecturerId
                query = query.Where(s => s.LecturerId == userId);
            }
            else if (role == "Admin" || role == "DOET")
            {
                // Admin và DOET có quyền truy cập toàn bộ sinh viên, không cần thêm điều kiện
            }
            else
            {
                throw new UnauthorizedAccessException("Role not authorized to view student list.");
            }
            // Tìm kiếm theo Student Name
            if (!string.IsNullOrWhiteSpace(studentName))
            {
                studentName = studentName.ToLower();
                query = query.Where(s => s.User.Name.ToLower().Contains(studentName));
            }
            if (!string.IsNullOrWhiteSpace(code))
            {
                code = code.ToLower();
                query = query.Where(s => s.User.UserCode.ToLower().Contains(code));
            }

            // Tìm kiếm theo Lecturer Name (chỉ áp dụng cho Dean)
            if ((role == "Dean" || role == "Lecturer" || role == "Admin") && !string.IsNullOrWhiteSpace(lecturerName))
            {
                lecturerName = lecturerName.ToLower();
                query = query.Where(s => s.Lecturer != null && s.Lecturer.Name.ToLower().Contains(lecturerName));
            }

            // Tìm kiếm theo Major Name
            if (!string.IsNullOrWhiteSpace(majorName))
            {
                majorName = majorName.ToLower();
                query = query.Where(s => s.Major != null && s.Major.Name.ToLower().Contains(majorName));
            }

            // Sắp xếp
            switch (sortBy?.ToLower())
            {
                case "studentname":
                    query = isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(s => s.User.Name)
                        : query.OrderBy(s => s.User.Name);
                    break;

                case "lecturername":
                    query = isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(s => s.Lecturer.Name)
                        : query.OrderBy(s => s.Lecturer.Name);
                    break;

                case "majorname":
                    query = isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(s => s.Major.Name)
                        : query.OrderBy(s => s.Major.Name);
                    break;

                default:
                    query = query.OrderBy(s => s.User.Name); // Mặc định sắp xếp theo Student Name tăng dần
                    break;
            }

            return await query.ToListAsync();
        }

        // KienBV - fixed 
        public async Task<IEnumerable<Student>> GetOjtStudentListAsync(int userId)
        {
            var dean = await _context.Users.Include(u => u.Role)
                .Where(u => u.Role.Name == "Dean" && u.UserId == userId)
                .FirstOrDefaultAsync();

            if (dean == null)
            {
                throw new KeyNotFoundException("Not found dean.");
            }

            var majors = await _context.Majors
                .Include(m => m.Department)
                .Where(m => m.DepartmentId == dean.DepartmentId)
                .ToListAsync();

            if (majors == null)
            {
                throw new KeyNotFoundException("Not majors list of department.");
            }

            var majorIds = majors.Select(m => m.MajorId).ToList();

            var students = await _context.Students
                .Include(s => s.User)
                .ThenInclude(u => u.Role)
                .Where(s => majorIds.Contains((int)s.MajorId))
                .ToListAsync();

            if (students == null)
            {
                throw new KeyNotFoundException("Not students have major of department.");
            }

            return students;
        }

        // Get students by IDs
        public async Task<List<Student>> GetStudentsByIdsAsync(List<int> studentIds)
        {
            return await _context.Students
                .Where(s => studentIds.Contains(s.StudentId))
                .ToListAsync();
        }

        // Update students
        public async Task UpdateStudentsAsync(List<Student> students)
        {
            _context.Students.UpdateRange(students);
            await _context.SaveChangesAsync();
        }

        // Get student details for Dean or Lecturer
        public async Task<Student> GetStudentDetailsByIdAsync(int studentId, int userId, string role)
        {
            // Khởi tạo query lấy thông tin sinh viên
            var query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .Include(s => s.Lecturer)
                .Include(s => s.Address)
                    .ThenInclude(a => a.Ward)
                .Include(s => s.Address.District)
                .Include(s => s.Address.Province)
                .Where(s => s.StudentId == studentId && s.User.Status != "Deleted");

            // Logic cho Lecturer
            if (role == "Lecturer")
            {
                query = query.Where(s => s.LecturerId == userId);
            }
            // Logic cho Dean
            else if (role == "Dean")
            {
                // Lấy thông tin Dean
                var dean = await GetDeanByUserIdAsync(userId);
                if (dean == null || !dean.DepartmentId.HasValue)
                {
                    throw new KeyNotFoundException("Dean not found or doesn't manage any department.");
                }

                // Lấy danh sách MajorId thuộc Department mà Dean quản lý
                var majorIdsInDepartment = await _context.Majors
                    .Where(m => m.DepartmentId == dean.DepartmentId)
                    .Select(m => m.MajorId)
                    .ToListAsync();

                if (!majorIdsInDepartment.Any())
                {
                    throw new KeyNotFoundException("Dean does not manage any majors.");
                }

                // Kiểm tra MajorId của sinh viên có thuộc MajorId trong Department không
                query = query.Where(s => s.MajorId.HasValue && majorIdsInDepartment.Contains(s.MajorId.Value));
            }
            else if (role == "Admin" || role == "DOET")
            {

            }
            // Lấy sinh viên đầu tiên phù hợp
            var student = await query.FirstOrDefaultAsync();

            // Nếu không tìm thấy sinh viên
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found or access denied.");
            }

            return student;
        }

        public async Task<Major> GetMajorByIdAsync(int majorId)
        {
            return await _context.Majors.FirstOrDefaultAsync(m => m.MajorId == majorId);
        }

        public async Task<Semester> GetSemesterByIdAsync(int semesterId)
        {
            return await _context.Semesters.FirstOrDefaultAsync(s => s.SemesterId == semesterId);
        }

        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        //End

        // === Notification ===
        // Student, University, Company
        public async Task<Notification> CreateNotificationAsync(Notification? info)
        {
            try
            {
                if (info?.NotificationContent == null)
                {
                    throw new Exception("Notification content is required.");
                }

                var noti = new Notification
                {
                    NotificationContent = info?.NotificationContent,
                    Image = info?.Image,
                    StudentId = info?.StudentId,
                    UniversityId = info?.UniversityId,
                    CompanyId = info?.CompanyId,
                    IsRead = info?.IsRead,
                    Status = info?.Status,
                    ApplicationId = info?.ApplicationId,
                    SupportRequestId = info?.SupportRequestId,
                    CompanyProposalId = info?.CompanyProposalId,
                    FeedbackId = info?.FeedbackId,
                    MessageId = info?.MessageId,
                    GroupChatId = info?.GroupChatId,
                    MessageGroupId = info?.MessageGroupId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _context.Notifications.AddAsync(noti);
                await _context.SaveChangesAsync();

                return noti;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsByUserIdAsync(int? userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    throw new Exception("Not found user.");
                }

                // Uni 
                if (user?.Role?.Name == "Admin" || user?.Role?.Name == "DOET"
                    || user?.Role?.Name == "Dean" || user?.Role?.Name == "Lecturer")
                {
                    var uniNotis = await _context.Notifications
                        .Include(n => n.Student).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.Company).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.University).ThenInclude(n => n.Role)
                        .Include(n => n.Application)
                        .Include(n => n.SupportRequest)
                        .Include(n => n.Feedback)
                        .Include(n => n.Feedback)
                        .Include(n => n.Message)
                        .Include(n => n.MessageGroup)
                        .Include(n => n.GroupChat)
                        .Where(n => n.UniversityId == userId)
                        .ToListAsync();

                    return uniNotis;
                }

                // Company 
                if (user?.Role?.Name == "Company" || user?.Role?.Name == "Mentor")
                {
                    var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

                    var companyNotis = await _context.Notifications
                        .Include(n => n.Student).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.Company).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.University).ThenInclude(n => n.Role)
                        .Include(n => n.Application)
                        .Include(n => n.SupportRequest)
                        .Include(n => n.Feedback)
                        .Include(n => n.Feedback)
                        .Include(n => n.Message)
                        .Include(n => n.MessageGroup)
                        .Include(n => n.GroupChat)
                        .Where(n => n.CompanyId == company.CompanyId)
                        .ToListAsync();

                    return companyNotis;
                }

                // Student 
                if (user?.Role?.Name == "Student")
                {
                    var student = await _context.Students.FirstOrDefaultAsync(c => c.UserId == userId);

                    var studentNotis = await _context.Notifications
                        .Include(n => n.Student).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.Company).ThenInclude(n => n.User).ThenInclude(n => n.Role)
                        .Include(n => n.University).ThenInclude(n => n.Role)
                        .Include(n => n.Application)
                        .Include(n => n.SupportRequest)
                        .Include(n => n.Feedback)
                        .Include(n => n.Feedback)
                        .Include(n => n.Message)
                        .Include(n => n.MessageGroup)
                        .Include(n => n.GroupChat)
                        .Where(n => n.StudentId == student.StudentId)
                        .ToListAsync();

                    return studentNotis;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // === Attendance ===
        public async Task<List<AttendanceReport>> GetAttendanceReportsByStudentIdAsync(int studentId)
        {
            var attendanceReports = await _context.AttendanceReports
                //.Where(ar => ar.StudentId == studentId)
                .Where(ar => ar.InternshipId == studentId)
                //.Include(ar => ar.Mentor)
                //    .ThenInclude(m => m.User)
                //.Include(ar => ar.Student)
                //    .ThenInclude(s => s.User)
                .ToListAsync();

            return attendanceReports;
        }

        // Mentor 
        public async Task<Company> SetCheckInCheckOutTimeAsync(int? userId, Company? info)
        {
            try
            {
                var mentor = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

                if (mentor == null)
                {
                    throw new Exception("Not found mentor.");
                }

                mentor.CheckInTime = info?.CheckInTime;
                mentor.CheckOutTime = info?.CheckOutTime;
                mentor.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return mentor;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> CreateAttendanceReportFileAsync(int? userId, int[]? internshipIds, string? fileName, byte[] fileData)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateAutoAttendanceReportAsync(int? userId, TimeSpan? checkInTime, TimeSpan? checkOutTime)
        {
            try
            {
                var mentor = await _context.Companies
                    .Include(c => c.User)
                    .ThenInclude(u => u.Role)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (mentor == null)
                {
                    throw new KeyNotFoundException("Mentor not found.");
                }

                var internships = await _context.Internships
                    .Include(i => i.Student)
                        .ThenInclude(s => s.User)
                    .Include(i => i.Company)
                    .Include(i => i.Semester)
                    .Where(i => i.CompanyId == mentor.CompanyId)
                    .ToListAsync();

                var newAttendanceReports = new List<AttendanceReport>();

                foreach (var internship in internships)
                {
                    var latestAttendance = await _context.AttendanceReports
                        .Where(ar => ar.InternshipId == internship.IntershipId)
                        .OrderByDescending(ar => ar.Date)
                        .FirstOrDefaultAsync();

                    DateTime currentDate = DateTime.Now.Date;

                    // Determine new day
                    DateTime newDate = (latestAttendance == null)
                        ? (internship.StartDate?.Date ?? currentDate)
                        : (latestAttendance.Date.Value.Date < currentDate ? currentDate : DateTime.MinValue);

                    if (newDate == DateTime.MinValue || newDate > internship.EndDate)
                    {
                        continue; // No need to create if out of date
                    }

                    // Create
                    var newAttendanceReport = new AttendanceReport
                    {
                        MentorId = mentor.CompanyId,
                        InternshipId = internship.IntershipId,
                        Date = newDate,
                        CheckInTime = checkInTime,
                        CheckOutTime = checkOutTime,
                        Status = "1",
                        EarlyLeave = false,
                        Late = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    newAttendanceReports.Add(newAttendanceReport);
                }

                // Save db changes
                if (newAttendanceReports.Any())
                {
                    await _context.AttendanceReports.AddRangeAsync(newAttendanceReports);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in create auto attendance report async: {ex.Message}");
                throw;
            }
        }

        public async Task<AttendanceReport> UpdateAttendanceReportAsync(int? attendanceReportId, AttendanceReport? info)
        {
            try
            {
                var attendanceReport = await _context.AttendanceReports
                    .FirstOrDefaultAsync(a => a.AttendanceReportId == attendanceReportId);

                if (attendanceReport == null)
                {
                    throw new Exception("Not found attendance report.");
                }

                attendanceReport.CheckInTime = info?.CheckInTime;
                attendanceReport.CheckOutTime = info?.CheckOutTime;
                attendanceReport.Reason = info?.Reason;
                attendanceReport.Status = info?.Status;
                attendanceReport.EarlyLeave = info?.EarlyLeave;
                attendanceReport.Late = info?.Late;
                attendanceReport.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return attendanceReport;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> InsertAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData)
        {
            try
            {
                var mentor = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

                if (mentor == null)
                {
                    throw new Exception("Mentor not found.");
                }

                //// Create new file name with format mentorId_timestamp_filename
                //var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //var newFileName = $"{mentor.CompanyId}_{timestamp}_{fileName}";
                //var filePath = Path.Combine(_attendanceReportDirectory, newFileName);

                var reports = new List<AttendanceReport>();

                // Read fiel by using EPPlus
                using (var package = new ExcelPackage(new MemoryStream(fileData)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Read the first sheet
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Start from line 2 if line 1 is header
                    {
                        try
                        {
                            var attendanceReport = new AttendanceReport
                            {
                                MentorId = mentor.CompanyId,
                                Date = worksheet.Cells[row, 3].GetValue<DateTime>(),
                                CheckInTime = ParseTimeFromExcel(worksheet.Cells[row, 4].Value),
                                CheckOutTime = ParseTimeFromExcel(worksheet.Cells[row, 5].Value),
                                Reason = worksheet.Cells[row, 6].GetValue<string>(),
                                Status = worksheet.Cells[row, 7].GetValue<string>(),
                                Late = worksheet.Cells[row, 8].GetValue<bool>(),
                                EarlyLeave = worksheet.Cells[row, 9].GetValue<bool>(),
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            // Find internship based on code
                            var internshipCode = worksheet.Cells[row, 1].GetValue<string>();
                            var internship = await _context.Internships
                                .FirstOrDefaultAsync(i => i.Code == internshipCode);

                            if (internship != null)
                            {
                                attendanceReport.InternshipId = internship.IntershipId;
                                reports.Add(attendanceReport);
                            }
                            else
                            {
                                Console.WriteLine($"Internship with code {internshipCode} not found.");
                            }
                        }
                        catch (Exception rowEx)
                        {
                            Console.WriteLine($"Error processing row at line {row}: {rowEx.Message}");
                        }
                    }
                }

                // Insert all reports into database
                if (reports.Count > 0)
                {
                    await _context.AttendanceReports.AddRangeAsync(reports);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Excel file: {ex.Message}");
                throw new Exception("An error occurred while inserting attendance reports.");
            }
        }

        private TimeSpan? ParseTimeFromExcel(object cellValue)
        {
            if (cellValue == null)
                return null;

            // Trường hợp 1: Nếu là kiểu DateTime (Excel có thể lưu giờ như một phần ngày)
            if (cellValue is DateTime dateTime)
                return dateTime.TimeOfDay;

            // Trường hợp 2: Nếu là kiểu số thập phân (Excel lưu giờ theo tỷ lệ ngày)
            if (double.TryParse(cellValue.ToString(), out var numericValue))
                return TimeSpan.FromDays(numericValue);

            // Trường hợp 3: Nếu là chuỗi và có thể parse sang TimeSpan
            if (TimeSpan.TryParse(cellValue.ToString(), out var parsedTime))
                return parsedTime;

            return null;
        }

        //public async Task<IEnumerable<AttendanceReport>> ListAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData)
        //{
        //    try
        //    {
        //        var mentor = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);

        //        if (mentor == null)
        //        {
        //            throw new Exception("Mentor not found.");
        //        }

        //        // Create new file name with format mentorId_timestamp_filename
        //        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        //        var newFileName = $"{mentor.CompanyId}_{timestamp}_{fileName}";
        //        var filePath = Path.Combine(_attendanceReportDirectory, newFileName);

        //        var attendanceReports = new List<AttendanceReport>();

        //        using (var package = new ExcelPackage(new MemoryStream(fileData)))
        //        {
        //            var worksheet = package.Workbook.Worksheets[0];
        //            var rowCount = worksheet.Dimension.Rows;

        //            for (int row = 2; row <= rowCount; row++)
        //            {
        //                try
        //                {
        //                    var attendanceReport = new AttendanceReport
        //                    {
        //                        Date = worksheet.Cells[row, 3].GetValue<DateTime>(),
        //                        CheckInTime = TimeSpan.TryParse(worksheet.Cells[row, 4].GetValue<string>(), out var checkIn) ? checkIn : (TimeSpan?)null,
        //                        CheckOutTime = TimeSpan.TryParse(worksheet.Cells[row, 5].GetValue<string>(), out var checkOut) ? checkOut : (TimeSpan?)null,
        //                        Reason = worksheet.Cells[row, 6].GetValue<string>(),
        //                        Status = worksheet.Cells[row, 7].GetValue<string>(),
        //                        Late = worksheet.Cells[row, 8].GetValue<bool>(),
        //                        EarlyLeave = worksheet.Cells[row, 9].GetValue<bool>()
        //                    };

        //                    attendanceReports.Add(attendanceReport);
        //                }
        //                catch (Exception rowEx)
        //                {
        //                    Console.WriteLine($"Error processing row at line {row}: {rowEx.Message}");
        //                }
        //            }
        //        }

        //        return attendanceReports;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error processing Excel file: {ex.Message}");
        //        return new List<AttendanceReport>();
        //    }
        //}

        // Mentor, Lecturer
        public async Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsByInternshipIdAsync(int? internshipId)
        {
            try
            {
                var internship = await _context.Internships
                    .Include(i => i.Student)
                    .FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                var attendanceReports = await _context.AttendanceReports
                    .Include(a => a.Mentor).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Internship).ThenInclude(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Where(a => a.InternshipId == internship.IntershipId)
                    .ToListAsync();

                return attendanceReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForMentorAsync(int? userId)
        {
            try
            {
                var mentor = await _context.Companies
                    .Include(i => i.User).ThenInclude(i => i.Role)
                    .FirstOrDefaultAsync(i => i.UserId == userId);

                if (mentor == null)
                {
                    throw new Exception("Not found mentor.");
                }

                var attendanceReports = await _context.AttendanceReports
                    .Include(a => a.Mentor).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Internship).ThenInclude(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Where(a => a.MentorId == mentor.CompanyId)
                    .ToListAsync();

                return attendanceReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Lecturer
        public async Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForLecturerAsync(int? userId)
        {
            try
            {
                var internship = await _context.Internships
                    .Include(i => i.Student)
                    .FirstOrDefaultAsync(i => i.Student.LecturerId == userId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                var attendanceReports = await _context.AttendanceReports
                    .Include(a => a.Mentor).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Internship).ThenInclude(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Where(a => a.InternshipId == internship.IntershipId)
                    .ToListAsync();

                return attendanceReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Student
        public async Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForStudentAsync(int? userId)
        {
            try
            {
                var internship = await _context.Internships
                    .Include(i => i.Student)
                    .FirstOrDefaultAsync(i => i.Student.UserId == userId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                var attendanceReports = await _context.AttendanceReports
                    .Include(a => a.Mentor).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Internship).ThenInclude(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Where(a => a.InternshipId == internship.IntershipId)
                    .ToListAsync();

                return attendanceReports;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // === Eva === 
        // University, Company
        public async Task<Evaluation> CreateEvaluationAsync(int? userId, int? internshipId, Evaluation? info)
        {
            try
            {
                var user = await _context.Users.Include(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (user == null)
                {
                    throw new Exception("Not found user.");
                }

                // Internship
                var internship = await _context.Internships.FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                // Check evaluation exist
                var evaluationExist = await _context.Evaluations.FirstOrDefaultAsync(e => e.StudentId == internship.StudentId);

                // Create 
                if (evaluationExist != null)
                {
                    if (user.Role.Name == "Mentor")
                    {
                        var newinfo = new Evaluation
                        {
                            CompanyComment = info.CompanyComment,
                            CompanyScore = info.CompanyScore,
                        };

                        UpdateEvaluationAsync(userId, internshipId, newinfo);
                        return newinfo;
                    }

                    if (user.Role.Name == "Dean" || user.Role.Name == "Lecturer")
                    {
                        var newinfo = new Evaluation
                        {
                            DeanComment = info.DeanComment,
                            DeanScore = info.DeanScore,
                        };

                        UpdateEvaluationAsync(userId, internshipId, newinfo);
                        return newinfo;
                    }
                }

                if (user.Role.Name == "Mentor")
                {
                    var newinfo = new Evaluation
                    {
                        CompanyComment = info.CompanyComment,
                        CompanyScore = info.CompanyScore,
                    };

                    await _context.AddAsync(newinfo);
                    _context.SaveChangesAsync();

                    return newinfo;
                }

                if (user.Role.Name == "Dean" || user.Role.Name == "Lecturer")
                {
                    var newinfo = new Evaluation
                    {
                        DeanComment = info.DeanComment,
                        DeanScore = info.DeanScore,
                    };

                    await _context.AddAsync(newinfo);
                    _context.SaveChangesAsync();
                    return newinfo;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Evaluation> UpdateEvaluationAsync(int? userId, int? internshipId, Evaluation? info)
        {
            try
            {
                var user = await _context.Users.Include(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (user == null)
                {
                    throw new Exception("Not found user.");
                }

                // Internship 
                var internship = await _context.Internships.FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                // Check evaluation exist
                var evaluationExist = await _context.Evaluations.FirstOrDefaultAsync(e => e.StudentId == internship.StudentId);

                if (evaluationExist == null)
                {
                    throw new Exception("Not found working report list for this evaluation.");
                }

                if (user.Role.Name == "Mentor")
                {
                    // Update 
                    evaluationExist.CompanyComment = info.CompanyComment;
                    evaluationExist.CompanyScore = info.CompanyScore;
                    evaluationExist.UpdatedAt = DateTime.Now;
                }

                if (user.Role.Name == "Dean" || user.Role.Name == "Lecturer")
                {
                    // Update 
                    evaluationExist.DeanComment = info.DeanComment;
                    evaluationExist.DeanScore = info.DeanScore;
                    evaluationExist.UpdatedAt = DateTime.Now;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Evaluation> GetEvaluationScoreAsync(int? userId)
        {
            try
            {
                // Validate User
                var user = await _context.Users.Include(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                // Validate Internship
                var internship = await _context.Internships.FirstOrDefaultAsync(i => i.Student.UserId == userId);
                if (internship == null)
                {
                    throw new Exception("Internship not found.");
                }

                // Check for Existing Evaluation
                var evaluationExist = await _context.Evaluations
                    .FirstOrDefaultAsync(e => e.StudentId == internship.StudentId);

                // Retrieve Working Reports
                var workingReports = await _context.WorkingReports
                    .Where(wk => wk.StudentId == internship.StudentId)
                    .ToListAsync();

                if (workingReports == null || !workingReports.Any())
                {
                    throw new Exception("No working reports found for this internship.");
                }

                // Define Evaluation Object
                Evaluation evaluation;

                // Case: Internship status = "pass" (status == "2")
                if (internship.Status == "2")
                {
                    // Calculate Process Scores (70%)
                    var mentorProcessScore = (workingReports.Sum(wk => wk.MentorScore ?? 0)) / workingReports.Count();
                    var uniProcessScore = (workingReports.Sum(wk => wk.LecturerScore ?? 0)) / workingReports.Count();
                    var processScore = (((mentorProcessScore + uniProcessScore) / 2) * 70) / 100;

                    // Final Score (30%)
                    var finalScore = (((evaluationExist.DeanScore + evaluationExist.CompanyScore) / 2) * 30) / 100;

                    // Total Evaluation Score
                    var evaluationScore = processScore + finalScore;

                    evaluationExist.EvaluationScore = evaluationScore;
                    evaluationExist.UpdatedAt = DateTime.Now;
                }
                // Case: Internship status = "fail" (status == "0")
                if (internship.Status == "0")
                {
                    evaluationExist.EvaluationScore = 0;
                    evaluationExist.UpdatedAt = DateTime.Now;
                }

                // Save Evaluation to Database
                await _context.SaveChangesAsync();

                return evaluationExist;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating evaluation: {ex.Message}");
            }
        }

        // University, Company, Student
        public async Task<Evaluation> GetEvaluationDetailByUserId(int? userId)
        {
            {
                try
                {
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

                    if (student == null)
                    {
                        throw new Exception("Not found student.");
                    }

                    var evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.StudentId == student.StudentId);

                    if (evaluation == null)
                    {
                        throw new Exception("Not found evaluation information.");
                    }

                    return evaluation;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<Evaluation> GetEvaluationDetailByInternshipId(int? internshipId)
        {
            {
                try
                {
                    var internship = await _context.Internships.FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                    if (internship == null)
                    {
                        throw new Exception("Not found internship.");
                    }

                    var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == internship.StudentId);

                    if (student == null)
                    {
                        throw new Exception("Not found student.");
                    }

                    var evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.StudentId == student.StudentId);

                    if (evaluation == null)
                    {
                        throw new Exception("Not found evaluation information.");
                    }

                    return evaluation;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
