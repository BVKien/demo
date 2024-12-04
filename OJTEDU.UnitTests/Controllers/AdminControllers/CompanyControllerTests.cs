using Microsoft.AspNetCore.Mvc;
using Moq;
using OJTEDU.Api.Configuration;
using OJTEDU.Api.Controllers.AdminControllers;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.AdminControllers.CompanyController;
using static OJTEDU.Api.Input.AdminControllers.UserController;
using static OJTEDU.Application.DTOs.CompanyDTO;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.UnitTests.Controllers.AdminControllers
{
    [TestFixture]
    public class CompanyControllerTests
    {
        private Mock<ICompanyService> _companyServiceMock;
        private CompanyController _controller;

        [SetUp]
        public void SetUp()
        {
            _companyServiceMock = new Mock<ICompanyService>();
            _controller = new CompanyController(_companyServiceMock.Object);
        }

        // Controller - Company Infomation Management - Company Infomation List

        [Test]
        public async Task GetAllCompanies_ShouldReturnOk_WhenDataIsAvailable()
        {
            // Arrange
            var mockResponse = new DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>
            {
                Data = new PagedResponse<List<CompanyListForAdminDoetDTO>>
                {
                    Items = new List<CompanyListForAdminDoetDTO>
                {
                    new CompanyListForAdminDoetDTO
                    {
                        CompanyId = 1,
                        CompanyName = "Alpha Company",
                        CompanyCode = "ALPHA",
                        Address = "123 Main St, Ward A, District B, Hanoi",
                        Phone = "123456789",
                        ContactEmail = "contact@alpha.com"
                    },
                    new CompanyListForAdminDoetDTO
                    {
                        CompanyId = 2,
                        CompanyName = "Beta Company",
                        CompanyCode = "BETA",
                        Address = "456 Side St, Ward X, District Y, HCM",
                        Phone = "987654321",
                        ContactEmail = "contact@beta.com"
                    }
                },
                    TotalCount = 2,
                    CurrentPage = 1,
                    PageSize = 15
                },
                Message = "Company list retrieved successfully!",
                StatusCode = 200
            };

            _companyServiceMock.Setup(x => x.GetAllCompaniesForAdminDoetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>()))
                               .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllCompanies(null, null, null, null, null, null, null, null);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Company list retrieved successfully!", apiResponse.Message);
            Assert.AreEqual(2, apiResponse.Data.Items.Count);
            Assert.AreEqual(1, apiResponse.Data.CurrentPage);
            Assert.AreEqual(15, apiResponse.Data.PageSize);
            Assert.AreEqual("Alpha Company", apiResponse.Data.Items.First().CompanyName);
        }

        [Test]
        public async Task GetAllCompanies_ShouldReturnFilteredData_WhenFilterApplied()
        {
            // Arrange
            var mockResponse = new DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>
            {
                Data = new PagedResponse<List<CompanyListForAdminDoetDTO>>
                {
                    Items = new List<CompanyListForAdminDoetDTO>
                {
                    new CompanyListForAdminDoetDTO
                    {
                        CompanyId = 1,
                        CompanyName = "Alpha Company",
                        CompanyCode = "ALPHA",
                        Address = "123 Main St, Ward A, District B, Hanoi",
                        Phone = "123456789",
                        ContactEmail = "contact@alpha.com",
                        Status = "Active",
                        CreatedAt = DateTime.Now
                    }
                },
                    TotalCount = 1,
                    CurrentPage = 1,
                    PageSize = 15
                },
                Message = "Filtered company list retrieved successfully!",
                StatusCode = 200
            };

            _companyServiceMock.Setup(x => x.GetAllCompaniesForAdminDoetAsync("Alpha", "ALPHA", "Active", 1, 1, 1, 1, 15))
                               .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllCompanies("Alpha", "ALPHA", "Active", 1, 1, 1, 1, 15);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Filtered company list retrieved successfully!", apiResponse.Message);
            Assert.AreEqual(1, apiResponse.Data.TotalCount);
            Assert.AreEqual("Alpha Company", apiResponse.Data.Items.First().CompanyName);
        }


        [Test]
        public async Task GetAllCompanies_ShouldReturnInternalServerError_WhenDataResponseIsNull()
        {
            // Arrange
            _companyServiceMock.Setup(x => x.GetAllCompaniesForAdminDoetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>()))
                               .ReturnsAsync((DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>)null);

            // Act
            var result = await _controller.GetAllCompanies(null, null, null, null, null, null, 1, 15);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Unexpected error occurred.", apiResponse.Message);
        }


        [Test]
        public async Task GetAllCompanies_ShouldReturnNotFound_WhenNoCompaniesExist()
        {
            // Arrange
            var mockResponse = new DataResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>
            {
                Data = null,
                Message = "No companies found.",
                StatusCode = 404
            };

            _companyServiceMock.Setup(x => x.GetAllCompaniesForAdminDoetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>()))
                               .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllCompanies(null, null, null, null, null, null, 1, 15);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("No companies found.", apiResponse.Message);
        }

        // Controller - Company Infomation Management - Company Infomation Detail

        [Test]
        public async Task GetCompanyDetail_ShouldReturnOk_WhenCompanyExists()
        {
            // Arrange
            var mockResponse = new DataResponse<CompanyDetailForAdminDoetDTO>
            {
                Data = new CompanyDetailForAdminDoetDTO
                {
                    CompanyId = 1,
                    CompanyName = "Test Company",
                    CompanyCode = "TEST123",
                    FullAddress = "123 Main St, City, State",
                    Phone = "123456789",
                    ContactEmail = "test@example.com",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                },
                Message = "Company details retrieved successfully!",
                StatusCode = 200
            };

            _companyServiceMock.Setup(x => x.GetCompanyDetailForAdminDoetAsync(1))
                               .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetCompanyDetail(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<CompanyDetailForAdminDoetDTO>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Company details retrieved successfully!", apiResponse.Message);
            Assert.AreEqual("Test Company", apiResponse.Data.CompanyName);
        }

        [Test]
        public async Task GetCompanyDetail_ShouldReturnBadRequest_WhenIdIsNull()
        {
            // Act
            var result = await _controller.GetCompanyDetail(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("companyId is required.", apiResponse.Message);
        }

        [Test]
        public async Task GetCompanyDetail_ShouldReturnNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            var mockResponse = new DataResponse<CompanyDetailForAdminDoetDTO>
            {
                Data = null,
                Message = "Company not found.",
                StatusCode = 404
            };

            _companyServiceMock.Setup(x => x.GetCompanyDetailForAdminDoetAsync(99))
                               .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetCompanyDetail(99);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Company not found.", apiResponse.Message);
        }

        [Test]
        public async Task GetCompanyDetail_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _companyServiceMock.Setup(x => x.GetCompanyDetailForAdminDoetAsync(It.IsAny<int>()))
                               .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetCompanyDetail(1);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Assert.IsNotNull(internalServerErrorResult);
            Assert.AreEqual(500, internalServerErrorResult.StatusCode);

            var apiResponse = internalServerErrorResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Internal Server Error: Unexpected error", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        // Controller - Company Infomation Management - Update Company Infomation

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var mockResponse = new DataResponse<UpdateCompanyForAdminDoetDTO>
            {
                Data = new UpdateCompanyForAdminDoetDTO { CompanyId = 1, CompanyName = "Updated Company" },
                Message = "Company updated successfully.",
                StatusCode = 200
            };

            var request = new UpdateCompanyRequestForAdminDoet
            {
                CompanyName = "Updated Company",
                TaxCode = "123456",
                Phone = "0987654321",
                ProvinceId = 1,
                DistrictId = 1,
                WardId = 1,
                AddressDetail = "123 Main St"
            };

            _companyServiceMock.Setup(x => x.UpdateCompanyForAdminDoetAsync(It.IsAny<UpdateCompanyForAdminDoetDTO>(), 1, 1, 1, "123 Main St"))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<UpdateCompanyForAdminDoetDTO>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Company updated successfully.", apiResponse.Message);
            Assert.AreEqual("Updated Company", apiResponse.Data.CompanyName);
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenCompanyIdIsMissing()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet();

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(null, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("companyId is required.", apiResponse.Message);
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenCompanyNameIsMissing()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                CompanyName = "" // Missing CompanyName
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("CompanyName is required."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenTaxCodeIsMissing()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                TaxCode = "" // Missing TaxCode
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("TaxCode is required."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenTaxCodeExceedsMaxLength()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                TaxCode = new string('1', 51) // Too long TaxCode
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("TaxCode must not exceed 50 characters."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnValidationError_WhenRequestIsInvalid()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                TaxCode = "InvalidTaxCode!" // Invalid TaxCode
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("TaxCode must contain only digits."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenContactEmailIsInvalid()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                ContactEmail = "invalid-email.com" // Invalid email format
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("Invalid ContactEmail format."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenContactEmailExceedsMaxLength()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                ContactEmail = new string('a', 51) + "@example.com" // Email exceeds 50 characters
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("ContactEmail must not exceed 50 characters."));
        }


        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenPhoneIsMissing()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                Phone = "" // Missing Phone
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("Phone is required."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenPhoneContainsNonDigits()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                Phone = "123ABC" // Invalid Phone (contains letters)
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("Phone must contain only digits."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenPhoneExceedsMaxLength()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                Phone = new string('1', 21) // Phone exceeds 20 digits
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("Phone must not exceed 20 digits."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenWebsiteExceedsMaxLength()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                Website = new string('a', 101) // Website exceeds 100 characters
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("Website must not exceed 100 characters."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenWebsiteIsInvalid()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                Website = "invalid-website" // Invalid website format
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("Website is not a valid URL."));
        }



        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnBadRequest_WhenProvinceIdIsMissing()
        {
            // Arrange
            var request = new UpdateCompanyRequestForAdminDoet
            {
                ProvinceId = null,
                DistrictId = null,
                WardId = null,
                AddressDetail = ""
            };

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(1, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("ProvinceId is required."));
            Assert.IsTrue(apiResponse.Message.Contains("DistrictId is required."));
            Assert.IsTrue(apiResponse.Message.Contains("WardId is required."));
            Assert.IsTrue(apiResponse.Message.Contains("AddressDetail is required."));
        }

        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnNotFound_WhenCompanyDoesNotExist()
        {
            // Arrange
            var companyId = 999;
            var request = new UpdateCompanyRequestForAdminDoet
            {
                CompanyName = "Updated Company",
                TaxCode = "123456",
                Phone = "0987654321",
                ProvinceId = 1,
                DistrictId = 1,
                WardId = 1,
                AddressDetail = "123 Main St"
            };

            _companyServiceMock.Setup(x => x.UpdateCompanyForAdminDoetAsync(It.IsAny<UpdateCompanyForAdminDoetDTO>(), 1, 1, 1, "123 Main St"))
                            .ReturnsAsync(new DataResponse<UpdateCompanyForAdminDoetDTO>
                            {
                                Data = null,
                                Message = "Company not found.",
                                StatusCode = 404
                            });

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(companyId, request);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Company not found.", apiResponse.Message);
        }


        [Test]
        public async Task UpdateCompanyForAdminDoet_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var companyId = 1;
            var request = new UpdateCompanyRequestForAdminDoet
            {
                CompanyName = "Updated Company",
                TaxCode = "123456",
                Phone = "0987654321",
                ProvinceId = 1,
                DistrictId = 1,
                WardId = 1,
                AddressDetail = "123 Main St"
            };

            _companyServiceMock.Setup(x => x.UpdateCompanyForAdminDoetAsync(It.IsAny<UpdateCompanyForAdminDoetDTO>(), 1, 1, 1, "123 Main St"))
                            .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.UpdateCompanyForAdminDoet(companyId, request);

            // Assert
            var errorResult = result as ObjectResult;
            Assert.IsNotNull(errorResult);
            Assert.AreEqual(500, errorResult.StatusCode);

            var apiResponse = errorResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Internal Server Error: Something went wrong", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }
    }
}
