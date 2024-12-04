using AutoMapper;
using Moq;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.CompanyDTO;

namespace OJTEDU.UnitTests.ApplicationServices.Services
{
    [TestFixture]
    public class CompanyServiceTests
    {
        private Mock<ICompanyRepository> _companyRepoMock;
        private Mock<IJobRepository> _jobRepoMock;
        private Mock<IAddressRepository> _addressRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IMapper> _mapperMock;
        private CompanyService _service;

        [SetUp]
        public void SetUp()
        {
            _companyRepoMock = new Mock<ICompanyRepository>();
            _jobRepoMock = new Mock<IJobRepository>();
            _addressRepoMock = new Mock<IAddressRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new CompanyService(_companyRepoMock.Object, _jobRepoMock.Object, _addressRepoMock.Object, _userRepoMock.Object, _mapperMock.Object);
        }

        // Service - Company Infomation Management - Company Infomation List

        [Test]
        public async Task GetAllCompanies_ShouldReturnPagedCompanies_WhenCompaniesExist()
        {
            // Arrange
            var mockCompanies = new List<Company>
            {
                new Company { CompanyId = 1, User = new User { UserId = 1, Name = "Company One", Status = "Active" }, Address = new Address { ProvinceId = 1, DistrictId = 1, WardId = 1 }},
                new Company { CompanyId = 2, User = new User { UserId = 2, Name = "Company Two", Status = "Active" }, Address = new Address { ProvinceId = 2, DistrictId = 2, WardId = 2 }},
                new Company { CompanyId = 3, User = new User { UserId = 3, Name = "Company Three", Status = "Inactive" }, Address = new Address { ProvinceId = 1, DistrictId = 1, WardId = 3 }}
            };

            _companyRepoMock.Setup(repo => repo.GetAllCompaniesForAdminDoetAsync(null, null, null, null, null, null))
                .ReturnsAsync(mockCompanies);

            var mockCompanyDtos = new List<CompanyListForAdminDoetDTO>
            {
                new CompanyListForAdminDoetDTO { CompanyId = 1, CompanyName = "Company One", Status = "Active" },
                new CompanyListForAdminDoetDTO { CompanyId = 2, CompanyName = "Company Two", Status = "Active" },
                new CompanyListForAdminDoetDTO { CompanyId = 3, CompanyName = "Company Three", Status = "Inactive" }
            };

            _mapperMock.Setup(mapper => mapper.Map<List<CompanyListForAdminDoetDTO>>(mockCompanies))
                .Returns(mockCompanyDtos);

            // Act
            var result = await _service.GetAllCompaniesForAdminDoetAsync(null, null, null, null, null, null, 1, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(2, result.Data.Items.Count);
            Assert.AreEqual(3, result.Data.TotalCount);
            Assert.AreEqual(1, result.Data.CurrentPage);
            Assert.AreEqual(2, result.Data.PageSize);
            Assert.AreEqual(2, result.Data.TotalPages);
        }


        [Test]
        public async Task GetAllCompanies_ShouldFilterByAllParameters()
        {
            // Arrange
            var mockCompanies = new List<Company>
            {
                new Company
                {
                    CompanyId = 1,
                    User = new User { UserId = 1, Name = "Alpha Company", UserCode = "ALPHA", Status = "Active" },
                    Address = new Address { ProvinceId = 1, DistrictId = 1, WardId = 1 }
                }
            };

            // Mock repository trả về danh sách công ty đã lọc
            _companyRepoMock.Setup(repo => repo.GetAllCompaniesForAdminDoetAsync("Alpha", "ALPHA", "Active", 1, 1, 1))
                .ReturnsAsync(mockCompanies);

            var mockCompanyDtos = new List<CompanyListForAdminDoetDTO>
            {
                new CompanyListForAdminDoetDTO { CompanyId = 1, CompanyName = "Alpha Company", CompanyCode = "ALPHA", Status = "Active"}
            };

            _mapperMock.Setup(mapper => mapper.Map<List<CompanyListForAdminDoetDTO>>(It.IsAny<List<Company>>()))
                .Returns(mockCompanyDtos);

            // Act
            var result = await _service.GetAllCompaniesForAdminDoetAsync("Alpha", "ALPHA", "Active", 1, 1, 1, 1, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(1, result.Data.Items.Count);
            Assert.AreEqual(1, result.Data.TotalCount);
            Assert.AreEqual(1, result.Data.CurrentPage);
            Assert.AreEqual(2, result.Data.PageSize);
            Assert.AreEqual(1, result.Data.TotalPages);
            Assert.AreEqual("Alpha Company", result.Data.Items.First().CompanyName);
            Assert.AreEqual("ALPHA", result.Data.Items.First().CompanyCode);
        }

        [Test]
        public async Task GetAllCompanies_ShouldReturnEmpty_WhenNoCompaniesExist()
        {
            // Arrange
            _companyRepoMock.Setup(repo => repo.GetAllCompaniesForAdminDoetAsync(null, null, null, null, null, null))
                .ReturnsAsync(new List<Company>());

            // Act
            var result = await _service.GetAllCompaniesForAdminDoetAsync(null, null, null, null, null, null, 1, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsEmpty(result.Data.Items);
            Assert.AreEqual(0, result.Data.TotalCount);
            Assert.AreEqual(1, result.Data.CurrentPage);
            Assert.AreEqual(2, result.Data.PageSize);
            Assert.AreEqual(1, result.Data.TotalPages);
        }


        [Test]
        public async Task GetAllCompanies_ShouldHandleKeyNotFoundException()
        {
            // Arrange
            _companyRepoMock.Setup(repo => repo.GetAllCompaniesForAdminDoetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ThrowsAsync(new KeyNotFoundException("No companies found."));

            // Act
            var result = await _service.GetAllCompaniesForAdminDoetAsync(null, null, null, null, null, null, 1, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("No companies found.", result.Message);
        }

        // Service - Company Infomation Management - Company Infomation Detail

        [Test]
        public async Task GetCompanyDetailForAdminDoetAsync_ShouldReturnCompany_WhenCompanyExists()
        {
            // Arrange
            var company = new Company
            {
                CompanyId = 1,
                User = new User
                {
                    UserId = 1,
                    Name = "Test Company",
                    UserCode = "TEST123",
                    Status = "Active"
                },
                Address = new Address
                {
                    Detail = "123 Main St",
                    Ward = new Ward { Name = "Ward A" },
                    District = new District { Name = "District A" },
                    Province = new Province { Name = "Province A" }
                },
                AlternativeEmail = "test@example.com",
                CreatedAt = DateTime.UtcNow
            };

            _companyRepoMock.Setup(repo => repo.GetCompanyDetailForAdminDoetAsync(1)).ReturnsAsync(company);

            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForAdminDoetDTO>(company))
                       .Returns(new CompanyDetailForAdminDoetDTO
                       {
                           CompanyId = 1,
                           CompanyName = "Test Company",
                           CompanyCode = "TEST123",
                           FullAddress = "123 Main St, Ward A, District A, Province A",
                           ContactEmail = "test@example.com",
                           Status = "Active",
                           CreatedAt = company.CreatedAt
                       });

            // Act
            var result = await _service.GetCompanyDetailForAdminDoetAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Test Company", result.Data.CompanyName);
            Assert.AreEqual("123 Main St, Ward A, District A, Province A", result.Data.FullAddress);
        }

        [Test]
        public async Task GetCompanyDetailForAdminDoetAsync_ShouldReturnNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company
                {
                    CompanyId = 2, // Different ID
                    User = new User { Name = "Beta Company", UserCode = "BETA", Status = "Active" }
                }
            };

            _companyRepoMock.Setup(repo => repo.GetAllCompaniesForAdminDoetAsync(null, null, null, null, null, null))
                            .ReturnsAsync(companies);

            // Act
            var result = await _service.GetCompanyDetailForAdminDoetAsync(1); // ID = 1, which does not exist

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Company not found.", result.Message);
        }

        // Service - Company Infomation Management - Update Company Infomation

        [Test]
        public async Task UpdateCompanyForAdminDoetAsync_ShouldUpdateCompanyNameAndAddressSuccessfully()
        {
            // Arrange
            var updateCompanyDto = new UpdateCompanyForAdminDoetDTO
            {
                CompanyId = 1,
                CompanyName = "Updated Company Name",
                TaxCode = "123456789",
                ContactEmail = "updated@example.com",
                Phone = "1234567890",
                Website = "http://updated-website.com",
                Description = "Updated Description"
            };

            var existingCompany = new Company
            {
                CompanyId = 1,
                UserId = 1,
                TaxCode = "111111111",
                Phone = "0987654321",
                Website = "http://old-website.com",
                Description = "Old Description",
                Address = null // Company currently has no address
            };

            var user = new User
            {
                UserId = 1,
                Name = "Old User"
            };

            _companyRepoMock.Setup(x => x.GetCompanyDetailForAdminDoetAsync(1))
                                  .ReturnsAsync(existingCompany);

            _userRepoMock.Setup(x => x.GetUserByIdForAdminAsync(1))
                               .ReturnsAsync(user);

            _addressRepoMock.Setup(x => x.AddAddressAsync(It.IsAny<Address>()))
                                  .ReturnsAsync(1);

            _mapperMock.Setup(x => x.Map<UpdateCompanyForAdminDoetDTO>(It.IsAny<Company>()))
                       .Returns(updateCompanyDto);

            // Act
            var result = await _service.UpdateCompanyForAdminDoetAsync(updateCompanyDto, 1, 2, 3, "Updated Address");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Updated Company Name", result.Data.CompanyName);
            Assert.AreEqual("Updated Description", result.Data.Description);
            Assert.AreEqual("http://updated-website.com", result.Data.Website);

            // Verify that user name was updated
            _userRepoMock.Verify(x => x.UpdateUserForAdminAsync(It.Is<User>(u => u.Name == "Updated Company Name")), Times.Once);

            // Verify that address was added
            _addressRepoMock.Verify(x => x.AddAddressAsync(It.Is<Address>(a =>
                a.ProvinceId == 1 &&
                a.DistrictId == 2 &&
                a.WardId == 3 &&
                a.Detail == "Updated Address" &&
                a.Status == "Active"
            )), Times.Once);

            // Verify that company details were updated
            _companyRepoMock.Verify(x => x.UpdateCompanyForAdminDoetAsync(It.Is<Company>(c =>
                c.TaxCode == "123456789" &&
                c.Phone == "1234567890" &&
                c.Website == "http://updated-website.com" &&
                c.Description == "Updated Description"
            )), Times.Once);
        }


        [Test]
        public async Task UpdateCompanyForAdminDoetAsync_ShouldReturnNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            _companyRepoMock.Setup(x => x.GetCompanyDetailForAdminDoetAsync(It.IsAny<int>()))
                                  .ReturnsAsync((Company)null);

            var updateCompanyDto = new UpdateCompanyForAdminDoetDTO
            {
                CompanyId = 999,
                CompanyName = "Non-Existent Company"
            };

            // Act
            var result = await _service.UpdateCompanyForAdminDoetAsync(updateCompanyDto, null, null, null, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Company not found.", result.Message);
        }

    }
}
