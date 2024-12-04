using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OJTEDU.Api.Configuration;
using OJTEDU.Api.Controllers.AdminControllers;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Api.Input.AdminControllers.UserController;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.UnitTests.Controllers.AdminControllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private UserController _controller;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        // Controller - User Management - User List

        [Test]
        public async Task GetAllUsersForAdmin_ShouldReturnOk_WhenDataIsAvailable()
        {
            // Arrange
            var mockResponse = new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
            {
                Data = new PagedResponse<List<UserListForAdminDTO>>
                {
                    Items = new List<UserListForAdminDTO>
                    {
                        new UserListForAdminDTO
                        {
                            UserId = 1,
                            Email = "datnthe163935@fpt.edu.vn",
                            Status = "Active",
                            Role = "Admin",
                            Name = "Nguyen Tien Dat",
                            UserCode = "ADMIN",
                            Image = "https://example.com/image1.png",
                            Information = null
                        },
                        new UserListForAdminDTO
                        {
                            UserId = 3,
                            Email = "tiendat288966@gmail.com",
                            Status = "Active",
                            Role = "Company",
                            Name = "Cong Ty FPT",
                            UserCode = "COMPANY",
                            Image = "https://example.com/image2.png",
                            Information = null
                        }
                    },
                    TotalCount = 2,
                    CurrentPage = 1,
                    PageSize = 15
                },
                Message = "User list retrieved successfully!",
                StatusCode = 200
            };

            _userServiceMock.Setup(x => x.GetAllUsersForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllUsersForAdmin(null, null, null, null, null);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<PagedResponse<List<UserListForAdminDTO>>>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User list retrieved successfully!", apiResponse.Message);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual(2, apiResponse.Data.Items.Count);
            Assert.AreEqual(1, apiResponse.Data.CurrentPage);
            Assert.AreEqual(15, apiResponse.Data.PageSize);
        }

        [Test]
        public async Task GetAllUsersForAdmin_ShouldReturnFilteredUsers_ByName()
        {
            // Arrange
            var mockResponse = new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
            {
                Data = new PagedResponse<List<UserListForAdminDTO>>
                {
                    Items = new List<UserListForAdminDTO>
                    {
                        new UserListForAdminDTO
                        {
                            UserId = 1,
                            Email = "datnthe163935@fpt.edu.vn",
                            Status = "Active",
                            Role = "Admin",
                            Name = "Nguyen Tien Dat",
                            UserCode = "ADMIN",
                            Image = "https://example.com/image1.png",
                            Information = null
                        }
                    },
                    TotalCount = 1,
                    CurrentPage = 1,
                    PageSize = 15
                },
                Message = "User list retrieved successfully!",
                StatusCode = 200
            };

            _userServiceMock.Setup(x => x.GetAllUsersForAdminAsync("Nguyen", null, null, 1, 15))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllUsersForAdmin("Nguyen", null, null, 1, 15);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<PagedResponse<List<UserListForAdminDTO>>>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User list retrieved successfully!", apiResponse.Message);
            Assert.AreEqual(1, apiResponse.Data.TotalCount);
            Assert.AreEqual("Nguyen Tien Dat", apiResponse.Data.Items.First().Name);
        }

        [Test]
        public async Task GetAllUsersForAdmin_ShouldReturnFilteredUsers_ByRoleId()
        {
            // Arrange
            var mockResponse = new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
            {
                Data = new PagedResponse<List<UserListForAdminDTO>>
                {
                    Items = new List<UserListForAdminDTO>
                    {
                        new UserListForAdminDTO
                        {
                            UserId = 2,
                            Email = "tiendat320@gmail.com",
                            Status = "Unactive",
                            Role = "DOET",
                            Name = "Nguyen Van Dat",
                            UserCode = "DOET",
                            Image = "https://example.com/image2.png",
                            Information = null
                        }
                    },
                    TotalCount = 1,
                    CurrentPage = 1,
                    PageSize = 15
                },
                Message = "User list retrieved successfully!",
                StatusCode = 200
            };

            _userServiceMock.Setup(x => x.GetAllUsersForAdminAsync(null, 2, null, 1, 15))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllUsersForAdmin(null, 2, null, 1, 15);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<PagedResponse<List<UserListForAdminDTO>>>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User list retrieved successfully!", apiResponse.Message);
            Assert.AreEqual(1, apiResponse.Data.TotalCount);
            Assert.AreEqual("Nguyen Van Dat", apiResponse.Data.Items.First().Name);
        }

        [Test]
        public async Task GetAllUsersForAdmin_ShouldReturnFilteredUsers_ByStatus()
        {
            // Arrange
            var mockResponse = new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
            {
                Data = new PagedResponse<List<UserListForAdminDTO>>
                {
                    Items = new List<UserListForAdminDTO>
                    {
                        new UserListForAdminDTO
                        {
                            UserId = 3,
                            Email = "mentor@gmail.com",
                            Status = "Active",
                            Role = "Mentor",
                            Name = "Mentor User",
                            UserCode = "MENTOR",
                            Image = "https://example.com/image3.png",
                            Information = "Mentor details"
                        }
                    },
                    TotalCount = 1,
                    CurrentPage = 1,
                    PageSize = 15
                },
                Message = "User list retrieved successfully!",
                StatusCode = 200
            };

            _userServiceMock.Setup(x => x.GetAllUsersForAdminAsync(null, null, "Active", 1, 15))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllUsersForAdmin(null, null, "Active", 1, 15);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<PagedResponse<List<UserListForAdminDTO>>>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User list retrieved successfully!", apiResponse.Message);
            Assert.AreEqual(1, apiResponse.Data.TotalCount);
            Assert.AreEqual("Mentor User", apiResponse.Data.Items.First().Name);
        }

        [Test]
        public async Task GetAllUsersForAdmin_ShouldReturnError_WhenDataResponseIsNull()
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetAllUsersForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                            .ReturnsAsync((DataResponse<PagedResponse<List<UserListForAdminDTO>>>)null);

            // Act
            var result = await _controller.GetAllUsersForAdmin(null, null, null, null, null);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Unexpected error occurred.", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        [Test]
        public async Task GetAllUsersForAdmin_ShouldReturnNotFound_WhenDataIsEmpty()
        {
            // Arrange
            var mockResponse = new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
            {
                Data = null,
                Message = "No users found.",
                StatusCode = 404
            };

            _userServiceMock.Setup(x => x.GetAllUsersForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetAllUsersForAdmin(null, null, null, null, null);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(404, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("No users found.", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        [Test]
        public async Task GetAllUsersForAdmin_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetAllUsersForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                            .ThrowsAsync(new System.Exception("Internal server error."));

            // Act
            var result = await _controller.GetAllUsersForAdmin(null, null, null, null, null);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Internal Server Error: Internal server error.", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        // Controller - User Management - User Detail

        [Test]
        public async Task GetUserDetailForAdmin_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var mockResponse = new DataResponse<UserDetailForAdminDTO>
            {
                Data = new UserDetailForAdminDTO
                {
                    UserId = 1,
                    Email = "test@example.com",
                    Name = "Test User",
                    Role = "Admin",
                    Status = "Active",
                    Information = "Some info"
                },
                Message = "User details retrieved successfully!",
                StatusCode = 200
            };

            _userServiceMock.Setup(x => x.GetUserDetailByIdForAdminAsync(1))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetUserDetailForAdmin(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<UserDetailForAdminDTO>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User details retrieved successfully!", apiResponse.Message);
            Assert.AreEqual("Test User", apiResponse.Data.Name);
        }

        [Test]
        public async Task GetUserDetailForAdmin_ShouldReturnBadRequest_WhenIdIsNull()
        {
            // Act
            var result = await _controller.GetUserDetailForAdmin(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Id is required.", apiResponse.Message);
        }

        [Test]
        public async Task GetUserDetailForAdmin_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var mockResponse = new DataResponse<UserDetailForAdminDTO>
            {
                Data = null,
                Message = "User not found.",
                StatusCode = 404
            };

            _userServiceMock.Setup(x => x.GetUserDetailByIdForAdminAsync(99))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.GetUserDetailForAdmin(99);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User not found.", apiResponse.Message);
        }

        [Test]
        public async Task GetUserDetailForAdmin_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _userServiceMock.Setup(x => x.GetUserDetailByIdForAdminAsync(It.IsAny<int>()))
                            .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetUserDetailForAdmin(1);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Assert.IsNotNull(internalServerErrorResult);
            Assert.AreEqual(500, internalServerErrorResult.StatusCode);

            var apiResponse = internalServerErrorResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Internal Server Error: Unexpected error", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        // Controller - User Management - Add User 

        [Test]
        public async Task AddUserForAdmin_ShouldReturnOk_WhenUserIsAddedSuccessfully()
        {
            // Arrange
            var request = new AddUserRequestForAdmin
            {
                Email = "test@example.com",
                Name = "Test User",
                UserCode = "USER123",
                RoleId = 1,
                Information = "Some info"
            };

            var mockResponse = new DataResponse<AddUserForAdminDTO>
            {
                Data = new AddUserForAdminDTO
                {
                    Email = "test@example.com",
                    Name = "Test User",
                    UserCode = "USER123",
                    RoleId = 1,
                    Information = "Some info",
                    CreatedAt = DateTime.Now
                },
                Message = "User added successfully!",
                StatusCode = 201
            };

            _userServiceMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<AddUserForAdminDTO>()))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.AddUserForAdmin(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<AddUserForAdminDTO>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User added successfully!", apiResponse.Message);
            Assert.AreEqual("test@example.com", apiResponse.Data.Email);
        }

        [Test]
        public async Task AddUserForAdmin_ShouldReturnBadRequest_WhenValidationFails()
        {
            // Arrange
            var request = new AddUserRequestForAdmin
            {
                Email = "", // Missing email
                Name = "",
                UserCode = "",
                RoleId = 0 // Invalid RoleId
            };

            // Act
            var result = await _controller.AddUserForAdmin(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Validation errors occurred: Email is required., Name is required., UserCode is required., Role is required.", apiResponse.Message);
        }

        [Test]
        public async Task AddUserForAdmin_ShouldReturnBadRequest_WhenEmailFormatIsInvalid()
        {
            // Arrange
            var request = new AddUserRequestForAdmin
            {
                Email = "invalid-email", // Invalid email format
                Name = "Test User",
                UserCode = "USER123",
                RoleId = 1
            };

            // Act
            var result = await _controller.AddUserForAdmin(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Validation errors occurred: Invalid email format.", apiResponse.Message);
        }

        [Test]
        public async Task AddUserForAdmin_ShouldReturnBadRequest_WhenEmailAndNameAndUserCodeExceedMaxLength()
        {
            // Arrange
            var request = new AddUserRequestForAdmin
            {
                Email = new string('a', 51) + "@example.com", // Email exceeds 50 characters
                Name = new string('N', 351), // Name exceeds 350 characters
                UserCode = new string('U', 51), // UserCode exceeds 50 characters
                RoleId = 1
            };

            // Act
            var result = await _controller.AddUserForAdmin(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("Validation errors occurred"));
            Assert.IsTrue(apiResponse.Message.Contains("Email must not exceed 50 characters."));
            Assert.IsTrue(apiResponse.Message.Contains("Name must not exceed 350 characters."));
            Assert.IsTrue(apiResponse.Message.Contains("UserCode cannot exceed 50 characters."));
        }


        [Test]
        public async Task AddUserForAdmin_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var request = new AddUserRequestForAdmin
            {
                Email = "duplicate@example.com",
                Name = "Duplicate User",
                UserCode = "USER123",
                RoleId = 1
            };

            var mockResponse = new DataResponse<AddUserForAdminDTO>
            {
                Data = null,
                Message = "A user with the same email already exists.",
                StatusCode = 400
            };

            _userServiceMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<AddUserForAdminDTO>()))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.AddUserForAdmin(request);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(400, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("A user with the same email already exists.", apiResponse.Message);
        }

        [Test]
        public async Task AddUserForAdmin_ShouldReturnBadRequest_WhenUserCodeAlreadyExists()
        {
            // Arrange
            var request = new AddUserRequestForAdmin
            {
                Email = "duplicate@example.com",
                Name = "Duplicate User",
                UserCode = "USER123",
                RoleId = 1
            };

            var mockResponse = new DataResponse<AddUserForAdminDTO>
            {
                Data = null,
                Message = "A user with the same UserCode already exists.",
                StatusCode = 400
            };

            _userServiceMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<AddUserForAdminDTO>()))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.AddUserForAdmin(request);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(400, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("A user with the same UserCode already exists.", apiResponse.Message);
        }

        [Test]
        public async Task AddUserForAdmin_ShouldReturnBadRequest_WhenUserRoleIsMentor()
        {
            // Arrange
            var request = new AddUserRequestForAdmin
            {
                Email = "mentor@example.com",
                Name = "Mentor User",
                UserCode = "Mentor123",
                RoleId = 7
            };

            var mockResponse = new DataResponse<AddUserForAdminDTO>
            {
                Data = null,
                Message = "Cannot add a user with the 'Mentor' role.",
                StatusCode = 400
            };

            _userServiceMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<AddUserForAdminDTO>()))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.AddUserForAdmin(request);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(400, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Cannot add a user with the 'Mentor' role.", apiResponse.Message);
        }

        [Test]
        public async Task AddUserForAdmin_ShouldReturnInternalServerError_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var request = new AddUserRequestForAdmin
            {
                Email = "test@example.com",
                Name = "Test User",
                UserCode = "USER123",
                RoleId = 1
            };

            _userServiceMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<AddUserForAdminDTO>()))
                            .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.AddUserForAdmin(request);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Internal Server Error: Unexpected error", apiResponse.Message);
        }

        // Controller - User Management - Update User 

        [Test]
        public async Task UpdateUserForAdmin_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var userId = 1;
            var request = new UpdateUserRequestForAdmin
            {
                Email = "updated@example.com",
                Name = "Updated User",
                UserCode = "UPDATED123",
                Information = "Updated Information"
            };

            var mockResponse = new DataResponse<UpdateUserForAdminDTO>
            {
                Data = new UpdateUserForAdminDTO
                {
                    UserId = userId,
                    Email = "updated@example.com",
                    Name = "Updated User",
                    UserCode = "UPDATED123",
                    Information = "Updated Information"
                },
                Message = "User updated successfully!",
                StatusCode = 200
            };

            _userServiceMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<UpdateUserForAdminDTO>()))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.UpdateUserForAdmin(userId, request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<UpdateUserForAdminDTO>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User updated successfully!", apiResponse.Message);
            Assert.AreEqual("updated@example.com", apiResponse.Data.Email);
        }

        [Test]
        public async Task UpdateUserForAdmin_ShouldReturnBadRequest_WhenValidationFails()
        {
            // Arrange
            var userId = 1;
            var request = new UpdateUserRequestForAdmin
            {
                Email = null, // Invalid email format
                Name = null, // Name is required
                UserCode = null // UserCode is required
            };

            // Act
            var result = await _controller.UpdateUserForAdmin(userId, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("Validation errors occurred"));
            Assert.IsTrue(apiResponse.Message.Contains("Email is required"));
            Assert.IsTrue(apiResponse.Message.Contains("Name is required"));
            Assert.IsTrue(apiResponse.Message.Contains("UserCode is required"));
        }

        [Test]
        public async Task UpdateUserForAdmin_ShouldReturnBadRequest_WhenEmailFormatIsInvalid()
        {
            // Arrange
            var userId = 1;
            var request = new UpdateUserRequestForAdmin
            {
                Email = "invalid-email-format", // Invalid email format
                Name = "Test User",
                UserCode = "USER123",
                Information = "Some information"
            };

            // Act
            var result = await _controller.UpdateUserForAdmin(userId, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Validation errors occurred: Invalid email format.", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        [Test]
        public async Task UpdateUserForAdmin_ShouldReturnBadRequest_WhenEmailAndNameAndUserCodeExceedMaxLength()
        {
            // Arrange
            var userId = 1;
            var request = new UpdateUserRequestForAdmin
            {
                Email = new string('a', 51) + "@example.com", // Email exceeds 50 characters
                Name = new string('N', 351), // Name exceeds 350 characters
                UserCode = new string('U', 51), // UserCode exceeds 50 characters
                Information = "Some information"
            };

            // Act
            var result = await _controller.UpdateUserForAdmin(userId, request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Message.Contains("Validation errors occurred"));
            Assert.IsTrue(apiResponse.Message.Contains("Email must not exceed 50 characters."));
            Assert.IsTrue(apiResponse.Message.Contains("Name must not exceed 350 characters."));
            Assert.IsTrue(apiResponse.Message.Contains("UserCode cannot exceed 50 characters."));
            Assert.IsNull(apiResponse.Data);
        }


        [Test]
        public async Task UpdateUserForAdmin_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;
            var request = new UpdateUserRequestForAdmin
            {
                Email = "notfound@example.com",
                Name = "Non-Existent User",
                UserCode = "NOTFOUND123",
                Information = "Non-existent user information"
            };

            _userServiceMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<UpdateUserForAdminDTO>()))
                            .ReturnsAsync(new DataResponse<UpdateUserForAdminDTO>
                            {
                                Data = null,
                                Message = "User not found",
                                StatusCode = 404
                            });

            // Act
            var result = await _controller.UpdateUserForAdmin(userId, request);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User not found", apiResponse.Message);
        }

        [Test]
        public async Task UpdateUserForAdmin_ShouldReturnBadRequest_WhenUserCodeAlreadyExists()
        {
            // Arrange
            var userId = 1;
            var request = new UpdateUserRequestForAdmin
            {
                Email = "test@example.com",
                Name = "Test User",
                UserCode = "EXISTINGCODE",
                Information = "Some information"
            };

            _userServiceMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<UpdateUserForAdminDTO>()))
                            .ReturnsAsync(new DataResponse<UpdateUserForAdminDTO>
                            {
                                Data = null,
                                Message = "UserCode already exists.",
                                StatusCode = 400
                            });

            // Act
            var result = await _controller.UpdateUserForAdmin(userId, request);

            // Assert
            var badRequestResult = result as ObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("UserCode already exists.", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        [Test]
        public async Task UpdateUserForAdmin_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var userId = 1;
            var request = new UpdateUserRequestForAdmin
            {
                Email = "existing@example.com",
                Name = "Test User",
                UserCode = "USER123",
                Information = "Some information"
            };

            _userServiceMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<UpdateUserForAdminDTO>()))
                            .ReturnsAsync(new DataResponse<UpdateUserForAdminDTO>
                            {
                                Data = null,
                                Message = "Email already exists.",
                                StatusCode = 400
                            });

            // Act
            var result = await _controller.UpdateUserForAdmin(userId, request);

            // Assert
            var badRequestResult = result as ObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Email already exists.", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        [Test]
        public async Task UpdateUserForAdmin_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = 1;
            var request = new UpdateUserRequestForAdmin
            {
                Email = "error@example.com",
                Name = "Error User",
                UserCode = "ERROR123",
                Information = "Causes exception"
            };

            _userServiceMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<UpdateUserForAdminDTO>()))
                            .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.UpdateUserForAdmin(userId, request);

            // Assert
            var errorResult = result as ObjectResult;
            Assert.IsNotNull(errorResult);
            Assert.AreEqual(500, errorResult.StatusCode);

            var apiResponse = errorResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Internal Server Error: Something went wrong", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        // Controller - User Management - Soft Delete User

        [Test]
        public async Task SoftDeleteUserForAdmin_ShouldReturnOk_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            var userId = 1;
            var mockResponse = new DataResponse<DeleteUserForAdminDTO>
            {
                Data = new DeleteUserForAdminDTO { UserId = userId },
                Message = "User has been marked as deleted successfully.",
                StatusCode = 200
            };

            _userServiceMock.Setup(x => x.SoftDeleteUserForAdminAsync(It.IsAny<DeleteUserForAdminDTO>()))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.SoftDeleteUserForAdmin(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<DeleteUserForAdminDTO>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User has been marked as deleted successfully.", apiResponse.Message);
            Assert.AreEqual(userId, apiResponse.Data.UserId);
        }

        [Test]
        public async Task SoftDeleteUserForAdmin_ShouldReturnBadRequest_WhenUserIdIsNull()
        {
            // Act
            var result = await _controller.SoftDeleteUserForAdmin(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Id is required.", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        [Test]
        public async Task SoftDeleteUserForAdmin_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 99;
            var mockResponse = new DataResponse<DeleteUserForAdminDTO>
            {
                Data = null,
                Message = "User not found",
                StatusCode = 404
            };

            _userServiceMock.Setup(x => x.SoftDeleteUserForAdminAsync(It.IsAny<DeleteUserForAdminDTO>()))
                            .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.SoftDeleteUserForAdmin(userId);

            // Assert
            var notFoundResult = result as ObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);

            var apiResponse = notFoundResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("User not found", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        [Test]
        public async Task SoftDeleteUserForAdmin_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var userId = 1;

            _userServiceMock.Setup(x => x.SoftDeleteUserForAdminAsync(It.IsAny<DeleteUserForAdminDTO>()))
                            .ThrowsAsync(new Exception("Internal Server Error"));

            // Act
            var result = await _controller.SoftDeleteUserForAdmin(userId);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Internal Server Error: Internal Server Error", apiResponse.Message);
            Assert.IsNull(apiResponse.Data);
        }

        // Controller - User Management - Download User File

        [Test]
        public async Task DownloadTemplateForAdmin_ShouldReturnFile_WhenTemplateGeneratedSuccessfully()
        {
            // Arrange
            var mockMemoryStream = new MemoryStream();
            var mockDataResponse = new DataResponse<MemoryStream>
            {
                Data = mockMemoryStream,
                Message = "Template generated successfully.",
                StatusCode = 200
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService
                .Setup(service => service.GenerateUserTemplateForAdminAsync())
                .ReturnsAsync(mockDataResponse);

            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.DownloadTemplateForAdmin();

            // Assert
            Assert.IsInstanceOf<FileStreamResult>(result);
            var fileResult = result as FileStreamResult;
            Assert.IsNotNull(fileResult);
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
            Assert.AreEqual("UserTemplateForAdmin.xlsx", fileResult.FileDownloadName);
        }

        [Test]
        public async Task DownloadTemplateForAdmin_ShouldReturnError_WhenTemplateGenerationFails()
        {
            // Arrange
            var mockDataResponse = new DataResponse<MemoryStream>
            {
                Data = null,
                Message = "Unexpected error occurred during template generation.",
                StatusCode = 500
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService
                .Setup(service => service.GenerateUserTemplateForAdminAsync())
                .ReturnsAsync(mockDataResponse);

            var controller = new UserController(mockUserService.Object);

            // Act
            var result = await controller.DownloadTemplateForAdmin();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            var response = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(response);
            Assert.AreEqual("Unexpected error occurred during template generation.", response.Message);
        }

        // Controller - User Management - Import User File

        [Test]
        public async Task ImportUsersForAdmin_ShouldReturnBadRequest_WhenFileIsNull()
        {
            // Act
            var result = await _controller.ImportUsersForAdmin(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("The uploaded file is empty or missing. Please ensure you provide a valid Excel file. If you're unsure of the required format, download the template and follow the instructions provided in the User Guide.", apiResponse.Message);
        }

        [Test]
        public async Task ImportUsersForAdmin_ShouldReturnBadRequest_WhenFileFormatIsInvalid()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("invalid-file.txt");
            mockFile.Setup(f => f.Length).Returns(100);

            // Act
            var result = await _controller.ImportUsersForAdmin(mockFile.Object);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);

            var apiResponse = badRequestResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Invalid file format. Only Excel files (.xlsx, .xls) are accepted. Please download the template to prepare your data correctly and follow the instructions in the User Guide.", apiResponse.Message);
        }

        [Test]
        public async Task ImportUsersForAdmin_ShouldReturnOk_WhenFileIsImportedSuccessfully()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("valid-file.xlsx");
            mockFile.Setup(f => f.Length).Returns(100);

            var mockResponse = new DataResponse<object>
            {
                Data = new { SuccessCount = 10, ErrorCount = 0, Errors = new List<string>() },
                Message = "Import completed. Successfully added 10 users.",
                StatusCode = 200
            };

            _userServiceMock.Setup(s => s.ImportUsersForAdminAsync(It.IsAny<IFormFile>())).ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.ImportUsersForAdmin(mockFile.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var apiResponse = okResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Import completed. Successfully added 10 users.", apiResponse.Message);
            Assert.AreEqual(10, ((dynamic)apiResponse.Data).SuccessCount);
        }

        [Test]
        public async Task ImportUsersForAdmin_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("valid-file.xlsx");
            mockFile.Setup(f => f.Length).Returns(100);

            _userServiceMock.Setup(s => s.ImportUsersForAdminAsync(It.IsAny<IFormFile>()))
                            .ThrowsAsync(new Exception("Unexpected error occurred."));

            // Act
            var result = await _controller.ImportUsersForAdmin(mockFile.Object);

            // Assert
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var apiResponse = objectResult.Value as ApiResponse<object>;
            Assert.IsNotNull(apiResponse);
            Assert.AreEqual("Error occurred while importing users: Unexpected error occurred.", apiResponse.Message);
        }
    }
}
