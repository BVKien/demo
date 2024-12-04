using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Infrastructure.Data;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.UnitTests.Repositories
{
    [TestFixture]
    public class CompanyRepositoryTests
    {
        private OJTEDU_DB_V1Context _context;
        private CompanyRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<OJTEDU_DB_V1Context>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new OJTEDU_DB_V1Context(options);
            _repository = new CompanyRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // Get All Company Infomation

        [Test]
        public async Task GetAllCompaniesForAdminDoetAsync_ShouldReturnFilteredAndSortedCompanies_WhenCompaniesExist()
        {
            // Arrange
            var provinceA = new Province { ProvinceId = 1, Name = "Province A" };
            var districtA = new District { DistrictId = 1, Name = "District A", Province = provinceA };
            var wardA = new Ward { WardId = 1, Name = "Ward A", District = districtA };

            var provinceB = new Province { ProvinceId = 2, Name = "Province B" };
            var districtB = new District { DistrictId = 2, Name = "District B", Province = provinceB };
            var wardB = new Ward { WardId = 2, Name = "Ward B", District = districtB };

            await _context.Provinces.AddRangeAsync(provinceA, provinceB);
            await _context.Districts.AddRangeAsync(districtA, districtB);
            await _context.Wards.AddRangeAsync(wardA, wardB);
            await _context.SaveChangesAsync();

            var companies = new List<Company>
            {
                new Company
                {
                    CompanyId = 1,
                    User = new User { UserId = 1, Name = "Alpha Company", UserCode = "ALPHA", Status = "Active", Role = new Role { Name = "Company" } },
                    Address = new Address { ProvinceId = provinceA.ProvinceId, DistrictId = districtA.DistrictId, WardId = wardA.WardId, Detail = "123 Main St" }
                },
                new Company
                {
                    CompanyId = 2,
                    User = new User { UserId = 2, Name = "Beta Company", UserCode = "BETA", Status = "Active", Role = new Role { Name = "Company" } },
                    Address = new Address { ProvinceId = provinceA.ProvinceId, DistrictId = districtA.DistrictId, WardId = wardB.WardId, Detail = "456 Side St" }
                },
                new Company
                {
                    CompanyId = 3,
                    User = new User { UserId = 3, Name = "Gamma Company", UserCode = "GAMMA", Status = "Unactive", Role = new Role { Name = "Company" } },
                    Address = new Address { ProvinceId = provinceB.ProvinceId, DistrictId = districtB.DistrictId, WardId = wardB.WardId, Detail = "789 Back St" }
                }
            };

            await _context.Companies.AddRangeAsync(companies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllCompaniesForAdminDoetAsync(null, null, null, null, null, null);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(3, resultList.Count);
            Assert.AreEqual("Beta Company", resultList[0].User.Name); // Active comes first
            Assert.AreEqual("Alpha Company", resultList[1].User.Name);  // Next active company
            Assert.AreEqual("Gamma Company", resultList[2].User.Name); // Unactive last
        }

        [Test]
        public async Task GetAllCompaniesForAdminDoetAsync_ShouldApplyFiltersCorrectly_WhenCompaniesExist()
        {
            // Arrange
            var provinceA = new Province { ProvinceId = 1, Name = "Province A" };
            var districtA = new District { DistrictId = 1, Name = "District A", Province = provinceA };
            var wardA = new Ward { WardId = 1, Name = "Ward A", District = districtA };

            var provinceB = new Province { ProvinceId = 2, Name = "Province B" };
            var districtB = new District { DistrictId = 2, Name = "District B", Province = provinceB };
            var wardB = new Ward { WardId = 2, Name = "Ward B", District = districtB };

            await _context.Provinces.AddRangeAsync(provinceA, provinceB);
            await _context.Districts.AddRangeAsync(districtA, districtB);
            await _context.Wards.AddRangeAsync(wardA, wardB);
            await _context.SaveChangesAsync();

            var companies = new List<Company>
            {
                new Company
                {
                    CompanyId = 1,
                    User = new User { UserId = 1, Name = "Alpha Company", UserCode = "ALPHA", Status = "Active", Role = new Role { Name = "Company" } },
                    Address = new Address { ProvinceId = provinceA.ProvinceId, DistrictId = districtA.DistrictId, WardId = wardA.WardId, Detail = "123 Main St" }
                },
                new Company
                {
                    CompanyId = 2,
                    User = new User { UserId = 2, Name = "Beta Company", UserCode = "BETA", Status = "Inactive", Role = new Role { Name = "Company" } },
                    Address = new Address { ProvinceId = provinceA.ProvinceId, DistrictId = districtA.DistrictId, WardId = wardB.WardId, Detail = "456 Side St" }
                },
                new Company
                {
                    CompanyId = 3,
                    User = new User { UserId = 3, Name = "Gamma Company", UserCode = "GAMMA", Status = "Active", Role = new Role { Name = "Company" } },
                    Address = new Address { ProvinceId = provinceB.ProvinceId, DistrictId = districtB.DistrictId, WardId = wardB.WardId, Detail = "789 Back St" }
                }
            };

            await _context.Companies.AddRangeAsync(companies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllCompaniesForAdminDoetAsync("Alpha", "ALPHA", "Active", provinceA.ProvinceId, districtA.DistrictId, wardA.WardId);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count); // Chỉ có một công ty thỏa mãn bộ lọc
            Assert.AreEqual("Alpha Company", resultList[0].User.Name);
            Assert.AreEqual("ALPHA", resultList[0].User.UserCode);
            Assert.AreEqual("Active", resultList[0].User.Status);
            Assert.AreEqual(wardA.WardId, resultList[0].Address.WardId);
        }

        // Get Company Infomation By Id

        [Test]
        public async Task GetCompanyDetailForAdminDoetAsync_ShouldReturnCompany_WhenCompanyExists()
        {
            // Arrange
            var provinceA = new Province { ProvinceId = 1, Name = "Province A" };
            var districtA = new District { DistrictId = 1, Name = "District A", Province = provinceA };
            var wardA = new Ward { WardId = 1, Name = "Ward A", District = districtA };

            var provinceB = new Province { ProvinceId = 2, Name = "Province B" };
            var districtB = new District { DistrictId = 2, Name = "District B", Province = provinceB };
            var wardB = new Ward { WardId = 2, Name = "Ward B", District = districtB };

            await _context.Provinces.AddRangeAsync(provinceA, provinceB);
            await _context.Districts.AddRangeAsync(districtA, districtB);
            await _context.Wards.AddRangeAsync(wardA, wardB);
            await _context.SaveChangesAsync();

            var company = new Company
            {
                CompanyId = 1,
                User = new User { UserId = 1, Name = "Alpha Company", UserCode = "ALPHA", Status = "Active", Role = new Role { Name = "Company" } },
                Address = new Address { ProvinceId = provinceA.ProvinceId, DistrictId = districtA.DistrictId, WardId = wardA.WardId, Detail = "123 Main St" }
            };

            await _context.Companies.AddAsync(company);  // Thêm Company cuối
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCompanyDetailForAdminDoetAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.CompanyId);
            Assert.AreEqual("Alpha Company", result.User.Name);
        }


        [Test]
        public void GetCompanyDetailForAdminDoetAsync_ShouldThrowKeyNotFoundException_WhenCompanyDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.GetCompanyDetailForAdminDoetAsync(99), "Company not found.");
        }

        // Update Company Information

        [Test]
        public async Task UpdateCompanyForAdminDoetAsync_ShouldUpdateCompanySuccessfully_WhenDataIsValid()
        {
            // Arrange
            var existingCompany = new Company
            {
                CompanyId = 1,
                TaxCode = "123456789",
                Phone = "1234567890",
                Website = "http://original.com",
                Description = "Original Description"
            };

            // Thêm thực thể ban đầu
            await _context.Companies.AddAsync(existingCompany);
            await _context.SaveChangesAsync();

            // Tách thực thể khỏi DbContext để tránh lỗi tracking
            _context.Entry(existingCompany).State = EntityState.Detached;

            var updatedCompany = new Company
            {
                CompanyId = 1, // Trùng ID
                TaxCode = "987654321",
                Phone = "0987654321",
                Website = "http://updated.com",
                Description = "Updated Description"
            };

            // Act
            await _repository.UpdateCompanyForAdminDoetAsync(updatedCompany);

            var result = await _context.Companies
                .AsNoTracking()
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CompanyId == 1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("987654321", result.TaxCode);
            Assert.AreEqual("0987654321", result.Phone);
            Assert.AreEqual("http://updated.com", result.Website);
            Assert.AreEqual("Updated Description", result.Description);
        }

    }
}
