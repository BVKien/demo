//using AutoMapper;
//using Moq;
//using NUnit.Framework;
//using OJTEDU.Application.ApplicationServices.Services;
//using OJTEDU.Application.DTOs;
//using OJTEDU.Domain.Entities;
//using OJTEDU.Domain.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
//using static OJTEDU.Application.DTOs.CompanyDTO;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class CompanyServiceTests
//    {
//        private Mock<ICompanyRepository> _companyRepositoryMock;
//        private Mock<IJobRepository> _jobRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private CompanyService _companyService;

//        [SetUp]
//        public void Setup()
//        {
//            _companyRepositoryMock = new Mock<ICompanyRepository>();
//            _jobRepositoryMock = new Mock<IJobRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _companyService = new CompanyService(_companyRepositoryMock.Object, _jobRepositoryMock.Object, _mapperMock.Object);
//        }
//        #region Guest

//        #region SearchCompaniesAsync

//        [Test]
//        public async Task SearchCompaniesAsync_ShouldReturnCompanyList_WhenCompaniesExist()
//        {
//            // Arrange
//            var companyList = new List<Company>
//            {
//                new Company { CompanyId = 1, User = new User { Name = "Company 1" } },
//                new Company { CompanyId = 2, User = new User { Name = "Company 2" } }
//            };
//            var companyDtoList = new List<CompanySearchListForGuestDTO>
//            {
//                new CompanySearchListForGuestDTO { CompanyId = 1, Name = "Company 1" },
//                new CompanySearchListForGuestDTO { CompanyId = 2, Name = "Company 2" }
//            };

//            _companyRepositoryMock.Setup(repo => repo.SearchCompaniesAsync(null, null, null, null, 1, 10))
//                                  .ReturnsAsync((companyList, 2));
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanySearchListForGuestDTO>>(companyList))
//                       .Returns(companyDtoList);

//            // Act
//            var result = await _companyService.SearchCompaniesAsync(null, null, null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("Company list retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task SearchCompaniesAsync_ShouldReturnEmptyList_WhenNoCompaniesFound()
//        {
//            // Arrange
//            _companyRepositoryMock.Setup(repo => repo.SearchCompaniesAsync("Nonexistent", null, null, null, 1, 10))
//                                  .ReturnsAsync((new List<Company>(), 0));
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanySearchListForGuestDTO>>(It.IsAny<List<Company>>()))
//                       .Returns(new List<CompanySearchListForGuestDTO>());

//            // Act
//            var result = await _companyService.SearchCompaniesAsync("Nonexistent", null, null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(0, result.Data.Count);
//            Assert.AreEqual("Company list retrieved successfully!", result.Message);
//        }


//        [Test]
//        public async Task SearchCompaniesAsync_ShouldReturnPagedResults_WhenPageSizeIsSet()
//        {
//            // Arrange
//            var companyList = new List<Company>
//    {
//        new Company { CompanyId = 1, User = new User { Name = "Company 1" } },
//        new Company { CompanyId = 2, User = new User { Name = "Company 2" } }
//    };
//            var companyDtoList = new List<CompanySearchListForGuestDTO>
//    {
//        new CompanySearchListForGuestDTO { CompanyId = 1, Name = "Company 1" },
//        new CompanySearchListForGuestDTO { CompanyId = 2, Name = "Company 2" }
//    };

//            _companyRepositoryMock.Setup(repo => repo.SearchCompaniesAsync(null, null, null, null, 1, 1))
//                                  .ReturnsAsync((companyList.Take(1).ToList(), 2)); // Mock as if there's pagination
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanySearchListForGuestDTO>>(companyList.Take(1).ToList()))
//                       .Returns(companyDtoList.Take(1).ToList());

//            // Act
//            var result = await _companyService.SearchCompaniesAsync(null, null, null, null, 1, 1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count); // Only one result as per page size
//            Assert.AreEqual("Company list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.TotalPages);
//        }

//        [Test]
//        public async Task SearchCompaniesAsync_ShouldFilterResultsByProvinceId()
//        {
//            // Arrange
//            var companyList = new List<Company>
//    {
//        new Company { CompanyId = 1, User = new User { Name = "Company 1" }, Address = new Address { ProvinceId = 5 } },
//        new Company { CompanyId = 2, User = new User { Name = "Company 2" }, Address = new Address { ProvinceId = 5 } }
//    };
//            var companyDtoList = new List<CompanySearchListForGuestDTO>
//    {
//        new CompanySearchListForGuestDTO { CompanyId = 1, Name = "Company 1" },
//        new CompanySearchListForGuestDTO { CompanyId = 2, Name = "Company 2" }
//    };

//            _companyRepositoryMock.Setup(repo => repo.SearchCompaniesAsync(null, 5, null, null, 1, 10))
//                                  .ReturnsAsync((companyList, 2));
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanySearchListForGuestDTO>>(companyList))
//                       .Returns(companyDtoList);

//            // Act
//            var result = await _companyService.SearchCompaniesAsync(null, 5, null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("Company list retrieved successfully!", result.Message);
//            Assert.AreEqual(1, result.Data[0].CompanyId);
//            Assert.AreEqual(2, result.Data[1].CompanyId);
//        }


//        [Test]
//        public async Task SearchCompaniesAsync_ShouldFilterByName_WhenNameIsProvided()
//        {
//            // Arrange
//            var companyList = new List<Company>
//    {
//        new Company { CompanyId = 1, User = new User { Name = "Matching Company 1" } },
//        new Company { CompanyId = 2, User = new User { Name = "Another Company" } }
//    };
//            var filteredCompanyList = new List<Company> { companyList[0] };
//            var companyDtoList = new List<CompanySearchListForGuestDTO>
//    {
//        new CompanySearchListForGuestDTO { CompanyId = 1, Name = "Matching Company 1" }
//    };

//            _companyRepositoryMock.Setup(repo => repo.SearchCompaniesAsync("Matching", null, null, null, 1, 10))
//                                  .ReturnsAsync((filteredCompanyList, 1));
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanySearchListForGuestDTO>>(filteredCompanyList))
//                       .Returns(companyDtoList);

//            // Act
//            var result = await _companyService.SearchCompaniesAsync("Matching", null, null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Matching Company 1", result.Data[0].Name);
//        }


//        [Test]
//        public async Task SearchCompaniesAsync_ShouldFilterByMultipleLocationCriteria()
//        {
//            // Arrange
//            var companyList = new List<Company>
//    {
//        new Company { CompanyId = 1, User = new User { Name = "Location-Based Company" }, Address = new Address { ProvinceId = 1, DistrictId = 1, WardId = 1 } }
//    };
//            var companyDtoList = new List<CompanySearchListForGuestDTO>
//    {
//        new CompanySearchListForGuestDTO { CompanyId = 1, Name = "Location-Based Company" }
//    };

//            _companyRepositoryMock.Setup(repo => repo.SearchCompaniesAsync(null, 1, 1, 1, 1, 10))
//                                  .ReturnsAsync((companyList, 1));
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanySearchListForGuestDTO>>(companyList))
//                       .Returns(companyDtoList);

//            // Act
//            var result = await _companyService.SearchCompaniesAsync(null, 1, 1, 1, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Location-Based Company", result.Data[0].Name);
//        }



//        [Test]
//        public async Task SearchCompaniesAsync_ShouldReturnFirstPage_WhenPageNumberIsNull()
//        {
//            // Arrange
//            var companyList = new List<Company>
//    {
//        new Company { CompanyId = 1, User = new User { Name = "Company with Null Page" } }
//    };
//            var companyDtoList = new List<CompanySearchListForGuestDTO>
//    {
//        new CompanySearchListForGuestDTO { CompanyId = 1, Name = "Company with Null Page" }
//    };

//            _companyRepositoryMock.Setup(repo => repo.SearchCompaniesAsync(null, null, null, null, null, 10))
//                                  .ReturnsAsync((companyList, 1));
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanySearchListForGuestDTO>>(companyList))
//                       .Returns(companyDtoList);

//            // Act
//            var result = await _companyService.SearchCompaniesAsync(null, null, null, null, null, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Company with Null Page", result.Data[0].Name);
//        }



//        #endregion

//        #region GetCompanyDetailByCompanyIdAsync

//        [Test]
//        public async Task GetCompanyDetailByCompanyIdAsync_ShouldReturnCompanyDetail_WhenCompanyExists()
//        {
//            // Arrange
//            var company = new Company
//            {
//                CompanyId = 1,
//                User = new User { Name = "Sample Company" },
//                Address = new Address { Detail = "123 Main St" }
//            };
//            var companyDto = new CompanyDetailForGuestDTO { CompanyId = 1, Name = "Sample Company", Address = "123 Main St" };

//            _companyRepositoryMock.Setup(repo => repo.GetCompanyDetailByCompanyIdAsync(1)).ReturnsAsync(company);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForGuestDTO>(company)).Returns(companyDto);

//            // Act
//            var result = await _companyService.GetCompanyDetailByCompanyIdAsync(1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company detail retrieved successfully!", result.Message);
//            Assert.AreEqual(companyDto, result.Data);
//        }


//        [Test]
//        public async Task GetCompanyDetailByCompanyIdAsync_ShouldHandleCompanyWithIncompleteAddress()
//        {
//            // Arrange
//            var companyId = 1;
//            var company = new Company
//            {
//                CompanyId = companyId,
//                User = new User { Name = "Company with Incomplete Address", Email = "company@incomplete.com", Image = "image.jpg" },
//                Address = new Address
//                {
//                    Detail = "123 Main St",
//                    Province = new Province { Name = "Province" },
//                    District = null, // No district information
//                    Ward = new Ward { Name = "Ward" }
//                }
//            };

//            var companyDto = new CompanyDetailForGuestDTO
//            {
//                CompanyId = companyId,
//                Name = "Company with Incomplete Address",
//                Email = "company@incomplete.com",
//                Image = "image.jpg",
//                Address = "123 Main St, Ward, Province" // Expected missing district handling
//            };

//            _companyRepositoryMock.Setup(repo => repo.GetCompanyDetailByCompanyIdAsync(companyId)).ReturnsAsync(company);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForGuestDTO>(company)).Returns(companyDto);

//            // Act
//            var result = await _companyService.GetCompanyDetailByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company detail retrieved successfully!", result.Message);
//            Assert.AreEqual(companyDto.Address, result.Data.Address);
//        }


//        [Test]
//        public async Task GetCompanyDetailByCompanyIdAsync_ShouldHandleCompanyWithMissingUserInfo()
//        {
//            // Arrange
//            var companyId = 1;
//            var company = new Company
//            {
//                CompanyId = companyId,
//                User = null, // Missing user information
//                Address = new Address
//                {
//                    Detail = "123 Main St",
//                    Province = new Province { Name = "Province" },
//                    District = new District { Name = "District" },
//                    Ward = new Ward { Name = "Ward" }
//                }
//            };

//            var companyDto = new CompanyDetailForGuestDTO
//            {
//                CompanyId = companyId,
//                Name = null, // Expecting null name due to missing user info
//                Address = "123 Main St, Ward, District, Province"
//            };

//            _companyRepositoryMock.Setup(repo => repo.GetCompanyDetailByCompanyIdAsync(companyId)).ReturnsAsync(company);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForGuestDTO>(company)).Returns(companyDto);

//            // Act
//            var result = await _companyService.GetCompanyDetailByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company detail retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data.Name); // Expecting null Name field
//            Assert.AreEqual(companyDto.Address, result.Data.Address);
//        }

//        [Test]
//        public async Task GetCompanyDetailByCompanyIdAsync_ShouldReturnPartialAddress_WhenWardIsNull()
//        {
//            // Arrange
//            var companyId = 1;
//            var company = new Company
//            {
//                CompanyId = companyId,
//                User = new User { Name = "Company Name", Email = "email@company.com", Image = "image.jpg" },
//                Address = new Address
//                {
//                    Detail = "123 Main St",
//                    Province = new Province { Name = "Province" },
//                    District = new District { Name = "District" },
//                    Ward = null // Missing ward information
//                }
//            };

//            var companyDto = new CompanyDetailForGuestDTO
//            {
//                CompanyId = companyId,
//                Name = "Company Name",
//                Email = "email@company.com",
//                Image = "image.jpg",
//                Address = "123 Main St, District, Province" // Expected output with missing ward
//            };

//            _companyRepositoryMock.Setup(repo => repo.GetCompanyDetailByCompanyIdAsync(companyId)).ReturnsAsync(company);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForGuestDTO>(company)).Returns(companyDto);

//            // Act
//            var result = await _companyService.GetCompanyDetailByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company detail retrieved successfully!", result.Message);
//            Assert.AreEqual("123 Main St, District, Province", result.Data.Address);
//        }

//        [Test]
//        public async Task GetCompanyDetailByCompanyIdAsync_ShouldHandleEmptyAddressComponents()
//        {
//            // Arrange
//            var companyId = 1;
//            var company = new Company
//            {
//                CompanyId = companyId,
//                User = new User { Name = "Company with No Address", Email = "email@company.com", Image = "image.jpg" },
//                Address = new Address
//                {
//                    Detail = string.Empty,
//                    Province = null,
//                    District = null,
//                    Ward = null
//                }
//            };

//            var companyDto = new CompanyDetailForGuestDTO
//            {
//                CompanyId = companyId,
//                Name = "Company with No Address",
//                Email = "email@company.com",
//                Image = "image.jpg",
//                Address = null // Expected null or empty address field
//            };

//            _companyRepositoryMock.Setup(repo => repo.GetCompanyDetailByCompanyIdAsync(companyId)).ReturnsAsync(company);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForGuestDTO>(company)).Returns(companyDto);

//            // Act
//            var result = await _companyService.GetCompanyDetailByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company detail retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data.Address); // Expecting null Address field
//        }

//        [Test]
//        public async Task GetCompanyDetailByCompanyIdAsync_ShouldHandleSpecialCharactersInCompanyName()
//        {
//            // Arrange
//            var companyId = 1;
//            var company = new Company
//            {
//                CompanyId = companyId,
//                User = new User { Name = "Company & Co.", Email = "info@companyandco.com", Image = "company_logo.jpg" },
//                Address = new Address
//                {
//                    Detail = "456 Market St",
//                    Province = new Province { Name = "Capital Region" },
//                    District = new District { Name = "Central District" },
//                    Ward = new Ward { Name = "Downtown" }
//                }
//            };

//            var companyDto = new CompanyDetailForGuestDTO
//            {
//                CompanyId = companyId,
//                Name = "Company & Co.",
//                Email = "info@companyandco.com",
//                Image = "company_logo.jpg",
//                Address = "456 Market St, Downtown, Central District, Capital Region"
//            };

//            _companyRepositoryMock.Setup(repo => repo.GetCompanyDetailByCompanyIdAsync(companyId)).ReturnsAsync(company);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForGuestDTO>(company)).Returns(companyDto);

//            // Act
//            var result = await _companyService.GetCompanyDetailByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company detail retrieved successfully!", result.Message);
//            Assert.AreEqual("Company & Co.", result.Data.Name);
//            Assert.AreEqual(companyDto.Address, result.Data.Address);
//        }

//        [Test]
//        public async Task GetCompanyDetailByCompanyIdAsync_ShouldHandleLongCompanyName()
//        {
//            // Arrange
//            var companyId = 1;
//            var company = new Company
//            {
//                CompanyId = companyId,
//                User = new User
//                {
//                    Name = "The Long Name Company with Numerous Departments and Affiliates Ltd.",
//                    Email = "contact@longnamecompany.com",
//                    Image = "long_logo.jpg"
//                },
//                Address = new Address
//                {
//                    Detail = "789 Broad Ave",
//                    Province = new Province { Name = "Northern Province" },
//                    District = new District { Name = "Upper District" },
//                    Ward = new Ward { Name = "North Ward" }
//                }
//            };

//            var companyDto = new CompanyDetailForGuestDTO
//            {
//                CompanyId = companyId,
//                Name = "The Long Name Company with Numerous Departments and Affiliates Ltd.",
//                Email = "contact@longnamecompany.com",
//                Image = "long_logo.jpg",
//                Address = "789 Broad Ave, North Ward, Upper District, Northern Province"
//            };

//            _companyRepositoryMock.Setup(repo => repo.GetCompanyDetailByCompanyIdAsync(companyId)).ReturnsAsync(company);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForGuestDTO>(company)).Returns(companyDto);

//            // Act
//            var result = await _companyService.GetCompanyDetailByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company detail retrieved successfully!", result.Message);
//            Assert.AreEqual(companyDto.Name, result.Data.Name);
//            Assert.AreEqual(companyDto.Address, result.Data.Address);
//        }

//        [Test]
//        public async Task GetCompanyDetailByCompanyIdAsync_ShouldHandleWhitespaceOnlyFields()
//        {
//            // Arrange
//            var companyId = 1;
//            var company = new Company
//            {
//                CompanyId = companyId,
//                User = new User { Name = " ", Email = " ", Image = " " },
//                Address = new Address
//                {
//                    Detail = " ",
//                    Province = new Province { Name = " " },
//                    District = new District { Name = " " },
//                    Ward = new Ward { Name = " " }
//                }
//            };

//            var companyDto = new CompanyDetailForGuestDTO
//            {
//                CompanyId = companyId,
//                Name = " ",
//                Email = " ",
//                Image = " ",
//                Address = " "
//            };

//            _companyRepositoryMock.Setup(repo => repo.GetCompanyDetailByCompanyIdAsync(companyId)).ReturnsAsync(company);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForGuestDTO>(company)).Returns(companyDto);

//            // Act
//            var result = await _companyService.GetCompanyDetailByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company detail retrieved successfully!", result.Message);
//            Assert.AreEqual(" ", result.Data.Name);
//            Assert.AreEqual(" ", result.Data.Address);
//        }

//        [Test]
//        public async Task GetCompanyDetailByCompanyIdAsync_ShouldHandleEmptyAddressAndContactInfo()
//        {
//            // Arrange
//            var companyId = 1;
//            var company = new Company
//            {
//                CompanyId = companyId,
//                User = new User { Name = "Company with Empty Fields", Email = string.Empty, Image = string.Empty },
//                Address = null
//            };

//            var companyDto = new CompanyDetailForGuestDTO
//            {
//                CompanyId = companyId,
//                Name = "Company with Empty Fields",
//                Email = string.Empty,
//                Image = string.Empty,
//                Address = null
//            };

//            _companyRepositoryMock.Setup(repo => repo.GetCompanyDetailByCompanyIdAsync(companyId)).ReturnsAsync(company);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyDetailForGuestDTO>(company)).Returns(companyDto);

//            // Act
//            var result = await _companyService.GetCompanyDetailByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company detail retrieved successfully!", result.Message);
//            Assert.AreEqual("Company with Empty Fields", result.Data.Name);
//            Assert.IsNull(result.Data.Address);
//        }


//        #endregion

//        #endregion
//    }
//}
