//using AutoMapper;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Moq;
//using NUnit.Framework;
//using OJTEDU.Application.ApplicationServices.Services;
//using OJTEDU.Domain.Entities;
//using OJTEDU.Domain.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
//using static OJTEDU.Application.DTOs.UserDTO;
//using OfficeOpenXml;
//using Newtonsoft.Json.Linq;
//using RichardSzalay.MockHttp;
//using System.Net;
//using System.Reflection;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class UserServiceTests
//    {
//        private Mock<IUserRepository> _userRepositoryMock;
//        private Mock<IConfiguration> _configMock;
//        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
//        private Mock<IMapper> _mapperMock;
//        private UserService _userService;

//        [SetUp]
//        public void Setup()
//        {
//            _userRepositoryMock = new Mock<IUserRepository>();
//            _configMock = new Mock<IConfiguration>();
//            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
//            _mapperMock = new Mock<IMapper>();
//            _userService = new UserService(
//                _configMock.Object,
//                _userRepositoryMock.Object,
//                _httpContextAccessorMock.Object,
//                _mapperMock.Object);
//        }

//        [Test]
//        public async Task LoginWithGoogleAsync_ShouldReturnBadRequest_WhenTokenIsNullOrEmpty()
//        {
//            // Act
//            var result = await _userService.LoginWithGoogleAsync(null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("Token cannot be empty.", result.Message);
//        }

//        [Test]
//        public async Task LoginWithGoogleAsync_ShouldReturnUnauthorized_WhenTokenIsInvalid()
//        {
//            // Arrange
//            _configMock.Setup(x => x["Google:TokenRequestUri"]).Returns("https://fake-uri.com");
//            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

//            // Act
//            var result = await _userService.LoginWithGoogleAsync("fake-token");

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(401, result.StatusCode);
//            Assert.AreEqual("Invalid Google Token.", result.Message);
//        }

//        [Test]
//        public async Task LoginWithGoogleAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
//        {
//            // Arrange
//            var token = "fake-token";
//            var fakePayload = new
//            {
//                Email = "test@example.com",
//                Picture = "http://example.com/image.jpg"
//            };

//            _configMock.Setup(x => x["Google:TokenRequestUri"]).Returns("http://fake-uri.com");
//            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(fakePayload.Email)).ReturnsAsync((User)null);

//            // Act
//            var result = await _userService.LoginWithGoogleAsync(token);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User not found.", result.Message);
//        }

//        [Test]
//        public async Task LoginWithGoogleAsync_ValidToken_ReturnsSuccessResponse()
//        {
//            // Arrange
//            string token = "valid_token";
//            var user = new User
//            {
//                UserId = 1,
//                Email = "user@example.com",
//                Name = "Test User",
//                UserCode = "U12345",
//                Image = "",
//                Role = new Role { Name = "User" },
//                Status = "Active"
//            };

//            var httpContextMock = new Mock<HttpContext>();
//            var configMock = new Mock<IConfiguration>();
//            configMock.SetupGet(x => x["Google:TokenRequestUri"]).Returns("https://oauth2.googleapis.com/token");
//            configMock.SetupGet(x => x["Google:ClientId"]).Returns("valid_client_id");
//            configMock.SetupGet(x => x["Google:ClientSecret"]).Returns("valid_client_secret");
//            configMock.SetupGet(x => x["Google:RedirectUri"]).Returns("http://localhost/callback");

//            // Correct mock setup for GetUserByEmailAsync
//            var userRepositoryMock = new Mock<IUserRepository>();
//            userRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
//                              .ReturnsAsync(user); // Ensuring it returns Task<User>

//            // Correct mock setup for UpdateUserForAdminAsync based on its return type
//            userRepositoryMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                              .ReturnsAsync(user); // Assuming UpdateUserForAdminAsync returns Task<User>

//            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
//            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

//            var googleTokenResponse = new JObject
//    {
//        { "access_token", "valid_access_token" },
//        { "id_token", "valid_id_token" }
//    };

//            var handlerMock = new MockHttpMessageHandler(googleTokenResponse.ToString(), HttpStatusCode.OK);

//            var httpClient = new HttpClient(handlerMock);
//            var userService = new UserService(configMock.Object, userRepositoryMock.Object, httpContextAccessorMock.Object, null);

//            // Inject the HttpClient manually into the private field
//            typeof(UserService).GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance)
//                               ?.SetValue(userService, httpClient);

//            // Act
//            var result = await userService.LoginWithGoogleAsync(token);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual("Login successful!", result.Message);

//            // Verify that GetUserByEmailAsync was called once
//            userRepositoryMock.Verify(x => x.GetUserByEmailAsync(It.IsAny<string>()), Times.Once);
//        }


//        [Test]
//        public async Task LogoutAsync_ShouldReturnSuccess_WhenLogoutIsCalled()
//        {
//            // Arrange
//            var httpContext = new DefaultHttpContext();

//            // Mock HttpContextAccessor
//            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

//            // Mock SignOutAsync
//            var authenticationServiceMock = new Mock<IAuthenticationService>();
//            authenticationServiceMock
//                .Setup(a => a.SignOutAsync(
//                    It.IsAny<HttpContext>(),
//                    CookieAuthenticationDefaults.AuthenticationScheme,
//                    It.IsAny<AuthenticationProperties>()))
//                .Returns(Task.CompletedTask);


//            httpContext.RequestServices = new ServiceCollection()
//                .AddSingleton(authenticationServiceMock.Object)
//                .BuildServiceProvider();

//            // Act
//            var result = await _userService.LogoutAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Logout successful!", result.Message);
//        }

//        [Test]
//        public async Task LogoutAsync_ShouldReturnServerError_WhenHttpContextIsNotFound()
//        {
//            // Arrange
//            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

//            // Act
//            var result = await _userService.LogoutAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("HttpContext not found.", result.Message);
//        }

//        [Test]
//        public async Task GetAllUsersForAdminAsync_ShouldReturnEmptyPagedResponse_WhenNoUsersExist()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersForAdminAsync()).ReturnsAsync(new List<User>());

//            // Act
//            var result = await _userService.GetAllUsersForAdminAsync(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(0, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//            Assert.AreEqual(10, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllUsersForAdminAsync_ShouldReturnPagedResponse_WhenUsersExistButLessThanPageSize()
//        {
//            // Arrange
//            var users = new List<User>
//            {
//                new User { UserId = 1, Email = "user1@example.com" }
//            };
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersForAdminAsync()).ReturnsAsync(users);
//            _mapperMock.Setup(mapper => mapper.Map<List<UserListForAdminDTO>>(users))
//                       .Returns(new List<UserListForAdminDTO>
//                       {
//                       new UserListForAdminDTO { UserId = 1, Email = "user1@example.com" }
//                       });

//            // Act
//            var result = await _userService.GetAllUsersForAdminAsync(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(1, result.Data.Items.Count);
//            Assert.AreEqual("user1@example.com", result.Data.Items[0].Email);
//        }

//        [Test]
//        public async Task GetAllUsersForAdminAsync_ShouldReturnPagedResponse_WhenUsersExistAndMoreThanPageSize()
//        {
//            // Arrange
//            var users = Enumerable.Range(1, 15).Select(i => new User { UserId = i, Email = $"user{i}@example.com" }).ToList();
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersForAdminAsync()).ReturnsAsync(users);
//            _mapperMock.Setup(mapper => mapper.Map<List<UserListForAdminDTO>>(It.IsAny<List<User>>()))
//                       .Returns(users.Skip(0).Take(10).Select(u => new UserListForAdminDTO { UserId = u.UserId, Email = u.Email }).ToList());

//            // Act
//            var result = await _userService.GetAllUsersForAdminAsync(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(15, result.Data.TotalCount);
//            Assert.AreEqual(2, result.Data.TotalPages);
//            Assert.AreEqual(10, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllUsersForAdminAsync_ShouldReturnNotFound_WhenKeyNotFoundExceptionIsThrown()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersForAdminAsync()).ThrowsAsync(new KeyNotFoundException("Users not found"));

//            // Act
//            var result = await _userService.GetAllUsersForAdminAsync(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Users not found", result.Message);
//        }

//        [Test]
//        public async Task GetAllUsersForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersForAdminAsync()).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _userService.GetAllUsersForAdminAsync(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get user list: Access denied", result.Message);
//        }

//        [Test]
//        public async Task GetUserDetailByIdForAdminAsync_ShouldReturnUserDetail_WhenUserExists()
//        {
//            // Arrange
//            var userId = 1;
//            var user = new User { UserId = userId, Name = "Test User", Email = "test@example.com", Status = "Active" };
//            var userDto = new UserDetailForAdminDTO { UserId = userId, Name = user.Name, Email = user.Email, Status = user.Status };

//            _userRepositoryMock.Setup(repo => repo.GetUserByIdForAdminAsync(userId)).ReturnsAsync(user);
//            _mapperMock.Setup(m => m.Map<UserDetailForAdminDTO>(user)).Returns(userDto);

//            // Act
//            var result = await _userService.GetUserDetailByIdForAdminAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(userDto, result.Data);
//            Assert.AreEqual("User details retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task GetUserDetailByIdForAdminAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
//        {
//            // Arrange
//            var userId = 1;
//            _userRepositoryMock.Setup(repo => repo.GetUserByIdForAdminAsync(userId)).ThrowsAsync(new KeyNotFoundException("User not found"));

//            // Act
//            var result = await _userService.GetUserDetailByIdForAdminAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User not found", result.Message);
//        }

//        [Test]
//        public async Task GetUserDetailByIdForAdminAsync_ShouldReturnNotFound_WhenUserIsDeleted()
//        {
//            // Arrange
//            var userId = 1;
//            var deletedUser = new User { UserId = userId, Status = "Deleted" };
//            _userRepositoryMock.Setup(repo => repo.GetUserByIdForAdminAsync(userId)).ReturnsAsync(deletedUser);

//            // Act
//            var result = await _userService.GetUserDetailByIdForAdminAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User is deleted.", result.Message);
//        }

//        [Test]
//        public async Task GetUserDetailByIdForAdminAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var userId = 1;
//            _userRepositoryMock.Setup(repo => repo.GetUserByIdForAdminAsync(userId)).ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _userService.GetUserDetailByIdForAdminAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving user details: Database error", result.Message);
//        }

//        [Test]
//        public async Task GetUserDetailByIdForAdminAsync_ShouldReturnUnauthorized_WhenAccessDenied()
//        {
//            // Arrange
//            var userId = 1;
//            _userRepositoryMock.Setup(repo => repo.GetUserByIdForAdminAsync(userId)).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _userService.GetUserDetailByIdForAdminAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get user detail: Access denied", result.Message);
//        }

//        [Test]
//        public async Task AddUserForAdminAsync_ShouldReturnSuccess_WhenUserIsAddedSuccessfully()
//        {
//            // Arrange
//            var addUserDto = new AddUserForAdminDTO
//            {
//                Email = "new@example.com",
//                RoleId = 1,
//                Name = "New User",
//                UserCode = "TU001",
//                Information = "Test information"
//            };

//            var user = new User
//            {
//                Email = addUserDto.Email,
//                RoleId = addUserDto.RoleId,
//                Name = addUserDto.Name,
//                UserCode = addUserDto.UserCode,
//                Information = addUserDto.Information,
//                CreatedAt = DateTime.Now
//            };

//            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(addUserDto.Email)).ReturnsAsync((User)null); // Simulate no existing user
//            _userRepositoryMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<User>())).ReturnsAsync(user);

//            // Act
//            var result = await _userService.AddUserForAdminAsync(addUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("User added successfully!", result.Message);
//            Assert.AreEqual(addUserDto.Email, result.Data.Email);
//        }

//        [Test]
//        public async Task AddUserForAdminAsync_ShouldReturnBadRequest_WhenEmailAlreadyExists()
//        {
//            // Arrange
//            var addUserDto = new AddUserForAdminDTO
//            {
//                Email = "existinguser@example.com",
//                RoleId = 1,
//                Name = "Existing User",
//                UserCode = "EU001",
//                Information = "Information about the existing user"
//            };

//            _userRepositoryMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<User>()))
//                .Throws(new InvalidOperationException("A user with the same email already exists."));

//            // Act
//            var result = await _userService.AddUserForAdminAsync(addUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("A user with the same email already exists.", result.Message);
//        }

//        [Test]
//        public async Task AddUserForAdminAsync_ShouldReturnBadRequest_WhenUserCodeAlreadyExists()
//        {
//            // Arrange
//            var addUserDto = new AddUserForAdminDTO
//            {
//                Email = "newuser@example.com",
//                RoleId = 1,
//                Name = "New User",
//                UserCode = "EXISTING_USER_CODE", // Giả định user code này đã tồn tại
//                Information = "Information about the new user"
//            };

//            _userRepositoryMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<User>()))
//                .Throws(new InvalidOperationException("A user with the same UserCode already exists."));

//            // Act
//            var result = await _userService.AddUserForAdminAsync(addUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("A user with the same UserCode already exists.", result.Message);
//        }

//        [Test]
//        public async Task AddUserForAdminAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var addUserDto = new AddUserForAdminDTO
//            {
//                Email = "newuser@example.com",
//                RoleId = 1,
//                Name = "New User",
//                UserCode = "NU001",
//                Information = "Information about the new user"
//            };

//            _userRepositoryMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<User>()))
//                .Throws(new Exception("Database error."));

//            // Act
//            var result = await _userService.AddUserForAdminAsync(addUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding user: Database error.", result.Message);
//        }

//        [Test]
//        public async Task AddUserForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            var addUserDto = new AddUserForAdminDTO
//            {
//                Email = "newuser@example.com",
//                RoleId = 1,
//                Name = "New User",
//                UserCode = "NU001",
//                Information = "Information about the new user"
//            };

//            _userRepositoryMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<User>()))
//                .Throws(new UnauthorizedAccessException("Access denied."));

//            // Act
//            var result = await _userService.AddUserForAdminAsync(addUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add user: Access denied.", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserForAdminAsync_ShouldReturnSuccess_WhenUserIsUpdatedSuccessfully()
//        {
//            // Arrange
//            var updateUserDto = new UpdateUserForAdminDTO
//            {
//                UserId = 1,
//                Email = "updated@example.com",
//                RoleId = 2,
//                Name = "Updated User",
//                UserCode = "TU002",
//                Information = "Updated information"
//            };

//            var user = new User
//            {
//                UserId = updateUserDto.UserId,
//                Email = updateUserDto.Email,
//                RoleId = updateUserDto.RoleId,
//                Name = updateUserDto.Name,
//                UserCode = updateUserDto.UserCode,
//                Information = updateUserDto.Information,
//                UpdatedAt = DateTime.Now
//            };

//            _userRepositoryMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>())).ReturnsAsync(user);
//            _mapperMock.Setup(m => m.Map<UpdateUserForAdminDTO>(user)).Returns(updateUserDto);

//            // Act
//            var result = await _userService.UpdateUserForAdminAsync(updateUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("User updated successfully!", result.Message);
//            Assert.AreEqual(updateUserDto.Email, result.Data.Email);
//        }

//        [Test]
//        public async Task UpdateUserForAdminAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
//        {
//            // Arrange
//            var updateUserDto = new UpdateUserForAdminDTO
//            {
//                UserId = 999, // User không tồn tại
//                Email = "nonexistent@example.com"
//            };

//            _userRepositoryMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                .Throws(new KeyNotFoundException("User not found"));

//            // Act
//            var result = await _userService.UpdateUserForAdminAsync(updateUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User not found", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserForAdminAsync_ShouldReturnBadRequest_WhenEmailAlreadyExists()
//        {
//            // Arrange
//            var updateUserDto = new UpdateUserForAdminDTO
//            {
//                UserId = 1,
//                Email = "existinguser@example.com" // Giả định email này đã tồn tại
//            };

//            _userRepositoryMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                .Throws(new InvalidOperationException("A user with the same email already exists."));

//            // Act
//            var result = await _userService.UpdateUserForAdminAsync(updateUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("A user with the same email already exists.", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserForAdminAsync_ShouldReturnBadRequest_WhenUserCodeAlreadyExists()
//        {
//            // Arrange
//            var updateUserDto = new UpdateUserForAdminDTO
//            {
//                UserId = 1,
//                UserCode = "EXISTING_USER_CODE" // Giả định user code này đã tồn tại
//            };

//            _userRepositoryMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                .Throws(new InvalidOperationException("A user with the same UserCode already exists."));

//            // Act
//            var result = await _userService.UpdateUserForAdminAsync(updateUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("A user with the same UserCode already exists.", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserForAdminAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var updateUserDto = new UpdateUserForAdminDTO
//            {
//                UserId = 1,
//                Email = "newuser@example.com"
//            };

//            _userRepositoryMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                .Throws(new Exception("Database error."));

//            // Act
//            var result = await _userService.UpdateUserForAdminAsync(updateUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating user: Database error.", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            var updateUserDto = new UpdateUserForAdminDTO
//            {
//                UserId = 1,
//                Email = "updated@example.com",
//                RoleId = 2,
//                Name = "Updated User",
//                UserCode = "TU002",
//                Information = "Updated information"
//            };

//            _userRepositoryMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                .Throws(new UnauthorizedAccessException("Access denied."));

//            // Act
//            var result = await _userService.UpdateUserForAdminAsync(updateUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update user: Access denied.", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserStatusForAdminAsync_ShouldReturnSuccess_WhenUserStatusIsUpdatedSuccessfully()
//        {
//            // Arrange
//            var updateUserStatusDto = new UpdateUserStatusForAdminDTO
//            {
//                UserId = 1,
//                Status = "Inactive"
//            };

//            var existingUser = new User
//            {
//                UserId = updateUserStatusDto.UserId,
//                Email = "user@example.com",
//                Role = new Role { Name = "User" }, // Người dùng không phải Admin
//                Status = "Active"
//            };

//            // Thiết lập mock để trả về người dùng hiện tại
//            _userRepositoryMock.Setup(x => x.GetUserByIdForAdminAsync(updateUserStatusDto.UserId))
//                .ReturnsAsync(existingUser);

//            // Thiết lập mock để cập nhật trạng thái
//            _userRepositoryMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                .ReturnsAsync((User user) =>
//                {
//                    existingUser.Status = user.Status; // Cập nhật trạng thái
//                    return existingUser; // Trả về người dùng đã được cập nhật
//                });

//            // Act
//            var result = await _userService.UpdateUserStatusForAdminAsync(updateUserStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("User updated successfully!", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserStatusForAdminAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
//        {
//            // Arrange
//            var updateUserStatusDto = new UpdateUserStatusForAdminDTO
//            {
//                UserId = 999, // User không tồn tại
//                Status = "Inactive"
//            };

//            _userRepositoryMock.Setup(x => x.GetUserByIdForAdminAsync(updateUserStatusDto.UserId))
//                .Throws(new KeyNotFoundException("User not found"));

//            // Act
//            var result = await _userService.UpdateUserStatusForAdminAsync(updateUserStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User not found", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserStatusForAdminAsync_ShouldReturnBadRequest_WhenAdminRole()
//        {
//            // Arrange
//            var updateUserStatusDto = new UpdateUserStatusForAdminDTO
//            {
//                UserId = 1,
//                Status = "Inactive"
//            };

//            var adminUser = new User
//            {
//                UserId = 1,
//                Role = new Role { Name = "Admin" } // User là admin
//            };

//            _userRepositoryMock.Setup(x => x.GetUserByIdForAdminAsync(updateUserStatusDto.UserId))
//                .ReturnsAsync(adminUser);

//            // Act
//            var result = await _userService.UpdateUserStatusForAdminAsync(updateUserStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("Cannot update the status of a user with the 'Admin' role.", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserStatusForAdminAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var updateUserStatusDto = new UpdateUserStatusForAdminDTO
//            {
//                UserId = 1,
//                Status = "Inactive"
//            };

//            _userRepositoryMock.Setup(x => x.GetUserByIdForAdminAsync(updateUserStatusDto.UserId))
//                .Throws(new Exception("Database error."));

//            // Act
//            var result = await _userService.UpdateUserStatusForAdminAsync(updateUserStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating user: Database error.", result.Message);
//        }

//        [Test]
//        public async Task UpdateUserStatusForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            var updateUserStatusDto = new UpdateUserStatusForAdminDTO
//            {
//                UserId = 1,
//                Status = "Inactive"
//            };

//            _userRepositoryMock.Setup(x => x.GetUserByIdForAdminAsync(updateUserStatusDto.UserId))
//                .Throws(new UnauthorizedAccessException("Access denied."));

//            // Act
//            var result = await _userService.UpdateUserStatusForAdminAsync(updateUserStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update user status: Access denied.", result.Message);
//        }

//        [Test]
//        public async Task SoftDeleteUserForAdminAsync_ShouldReturnSuccess_WhenUserIsDeletedSuccessfully()
//        {
//            // Arrange
//            var deleteUserForAdminDto = new DeleteUserForAdminDTO
//            {
//                UserId = 1
//            };

//            var user = new User
//            {
//                UserId = deleteUserForAdminDto.UserId,
//                Status = "Active",
//                DeletedAt = null // Chưa xóa
//            };

//            _userRepositoryMock.Setup(x => x.SoftDeleteUserForAdminAsync(deleteUserForAdminDto.UserId))
//                .ReturnsAsync(user);

//            // Act
//            var result = await _userService.SoftDeleteUserForAdminAsync(deleteUserForAdminDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("User has been marked as deleted successfully.", result.Message);
//        }

//        [Test]
//        public async Task SoftDeleteUserForAdminAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
//        {
//            // Arrange
//            var deleteUserForAdminDto = new DeleteUserForAdminDTO
//            {
//                UserId = 999 // User không tồn tại
//            };

//            _userRepositoryMock.Setup(x => x.SoftDeleteUserForAdminAsync(deleteUserForAdminDto.UserId))
//                .Throws(new KeyNotFoundException("User not found"));

//            // Act
//            var result = await _userService.SoftDeleteUserForAdminAsync(deleteUserForAdminDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User not found", result.Message);
//        }

//        [Test]
//        public async Task SoftDeleteUserForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            var deleteUserForAdminDto = new DeleteUserForAdminDTO
//            {
//                UserId = 1
//            };

//            _userRepositoryMock.Setup(x => x.SoftDeleteUserForAdminAsync(deleteUserForAdminDto.UserId))
//                .Throws(new UnauthorizedAccessException("Access denied."));

//            // Act
//            var result = await _userService.SoftDeleteUserForAdminAsync(deleteUserForAdminDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while soft delete user: Access denied.", result.Message);
//        }

//        [Test]
//        public async Task GenerateUserTemplateForAdminAsync_ShouldReturnSuccess_WhenCalled()
//        {
//            // Act
//            var result = await _userService.GenerateUserTemplateForAdminAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.IsNotNull(result.Data);
//            Assert.IsInstanceOfType<MemoryStream>(result.Data);

//            // Kiểm tra nội dung của MemoryStream
//            using (var memoryStream = result.Data)
//            {
//                memoryStream.Position = 0; // Đặt vị trí về đầu
//                using (var package = new ExcelPackage(memoryStream))
//                {
//                    var worksheet = package.Workbook.Worksheets[0];
//                    Assert.AreEqual("User Template For Admin", worksheet.Name);
//                    Assert.AreEqual("Email(*)", worksheet.Cells[1, 1].Value);
//                    Assert.AreEqual("FullName(*)", worksheet.Cells[1, 2].Value);
//                    // Thêm các kiểm tra khác về nội dung worksheet nếu cần
//                }
//            }
//        }

//        // Test trường hợp không có tệp
//        [Test]
//        public async Task ImportUsersForAdminAsync_ShouldReturnError_WhenFileIsNull()
//        {
//            var result = await _userService.ImportUsersForAdminAsync(null);

//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("File is empty or not provided.", result.Message);
//        }

//        // Test trường hợp tệp rỗng
//        [Test]
//        public async Task ImportUsersForAdminAsync_ShouldReturnError_WhenFileIsEmpty()
//        {
//            var fileMock = new Mock<IFormFile>();
//            fileMock.Setup(f => f.Length).Returns(0);

//            var result = await _userService.ImportUsersForAdminAsync(fileMock.Object);

//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("File is empty or not provided.", result.Message);
//        }

//        [Test]
//        public async Task GetAllStatusesUserForAdminAsync_ShouldReturnSuccess_WhenStatusesAreRetrieved()
//        {
//            // Act
//            var result = await _userService.GetAllStatusesUserForAdminAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Status List retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("Active", result.Data[0].Status);
//            Assert.AreEqual("Inactive", result.Data[1].Status);
//        }

//        [Test]
//        public async Task SearchUsersForAdminAsync_ShouldReturnPagedResponse_WhenUsersAreFound()
//        {
//            // Arrange
//            var users = new List<User>
//        {
//            new User { Name = "John Doe", RoleId = 1, Status = "Active" },
//            new User { Name = "Jane Doe", RoleId = 2, Status = "Inactive" }
//        };

//            var userDtos = new List<UserListForAdminDTO>
//        {
//            new UserListForAdminDTO { Name = "John Doe", Status = "Active" },
//            new UserListForAdminDTO { Name = "Jane Doe", Status = "Inactive" }
//        };

//            _userRepositoryMock.Setup(repo => repo.SearchUsersForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>())).ReturnsAsync(users);
//            _mapperMock.Setup(mapper => mapper.Map<List<UserListForAdminDTO>>(users)).Returns(userDtos);

//            // Act
//            var result = await _userService.SearchUsersForAdminAsync(null, null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual("Users retrieved successfully.", result.Message);
//        }

//        // Trường hợp không tìm thấy người dùng, ném KeyNotFoundException
//        [Test]
//        public async Task SearchUsersForAdminAsync_ShouldReturnNotFound_WhenNoUsersFound()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.SearchUsersForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                               .ThrowsAsync(new KeyNotFoundException("No users found matching the search criteria."));

//            // Act
//            var result = await _userService.SearchUsersForAdminAsync(null, null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("No users found matching the search criteria.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        // Trường hợp quyền truy cập bị từ chối
//        [Test]
//        public async Task SearchUsersForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.SearchUsersForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                               .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _userService.SearchUsersForAdminAsync(null, null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while search users: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        // Trường hợp ngoại lệ chung
//        [Test]
//        public async Task SearchUsersForAdminAsync_ShouldReturnServerError_WhenGeneralExceptionIsThrown()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.SearchUsersForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                               .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _userService.SearchUsersForAdminAsync(null, null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error occurred during search: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllUsersStoredForAdmin_ShouldReturnEmptyPagedResponse_WhenNoUsersExist()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User>());

//            // Act
//            var result = await _userService.GetAllUsersStoredForAdmin(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(0, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//            Assert.AreEqual(10, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllUsersStoredForAdmin_ShouldReturnPagedResponse_WhenUsersExistButLessThanPageSize()
//        {
//            // Arrange
//            var users = new List<User>
//        {
//            new User { UserId = 1, Email = "user1@example.com", Status = "Deleted" }
//        };
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(users);
//            _mapperMock.Setup(mapper => mapper.Map<List<UserListForAdminDTO>>(users))
//                       .Returns(new List<UserListForAdminDTO>
//                       {
//                       new UserListForAdminDTO { UserId = 1, Email = "user1@example.com" }
//                       });

//            // Act
//            var result = await _userService.GetAllUsersStoredForAdmin(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(1, result.Data.Items.Count);
//            Assert.AreEqual("user1@example.com", result.Data.Items[0].Email);
//        }

//        [Test]
//        public async Task GetAllUsersStoredForAdmin_ShouldReturnPagedResponse_WhenUsersExistAndMoreThanPageSize()
//        {
//            // Arrange
//            var users = Enumerable.Range(1, 15).Select(i => new User { UserId = i, Email = $"user{i}@example.com", Status = "Deleted" }).ToList();
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(users);
//            _mapperMock.Setup(mapper => mapper.Map<List<UserListForAdminDTO>>(It.IsAny<List<User>>()))
//                       .Returns(users.Skip(0).Take(10).Select(u => new UserListForAdminDTO { UserId = u.UserId, Email = u.Email }).ToList());

//            // Act
//            var result = await _userService.GetAllUsersStoredForAdmin(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(15, result.Data.TotalCount);
//            Assert.AreEqual(2, result.Data.TotalPages);
//            Assert.AreEqual(10, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllUsersStoredForAdmin_ShouldReturnNotFound_WhenKeyNotFoundExceptionIsThrown()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ThrowsAsync(new KeyNotFoundException("Users not found"));

//            // Act
//            var result = await _userService.GetAllUsersStoredForAdmin(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Users not found", result.Message);
//        }

//        [Test]
//        public async Task GetAllUsersStoredForAdmin_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _userService.GetAllUsersStoredForAdmin(1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get user stored list: Access denied", result.Message);
//        }

//        [Test]
//        public async Task GetUserStoredDetailByIdForAdminAsync_ShouldReturnUserDetail_WhenUserExistsAndIsDeleted()
//        {
//            // Arrange
//            var userId = 1;
//            var deletedUser = new User { UserId = userId, Name = "Test User", Email = "test@example.com", Status = "Deleted" };
//            var userDto = new UserDetailForAdminDTO { UserId = userId, Name = deletedUser.Name, Email = deletedUser.Email };

//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User> { deletedUser });
//            _userRepositoryMock.Setup(repo => repo.GetUserByIdForAdminAsync(userId)).ReturnsAsync(deletedUser);
//            _mapperMock.Setup(mapper => mapper.Map<UserDetailForAdminDTO>(deletedUser)).Returns(userDto);

//            // Act
//            var result = await _userService.GetUserStoredDetailByIdForAdminAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(userDto, result.Data);
//            Assert.AreEqual("User Stored details retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task GetUserStoredDetailByIdForAdminAsync_ShouldReturnNotFound_WhenUserNotInStoredList()
//        {
//            // Arrange
//            var userId = 1;
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User>());
//            _userRepositoryMock.Setup(repo => repo.GetUserByIdForAdminAsync(userId)).ThrowsAsync(new KeyNotFoundException("User is not in the stored list."));

//            // Act
//            var result = await _userService.GetUserStoredDetailByIdForAdminAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User is not in the stored list.", result.Message);
//        }

//        [Test]
//        public async Task GetUserStoredDetailByIdForAdminAsync_ShouldReturnUnauthorized_WhenAccessDenied()
//        {
//            // Arrange
//            var userId = 1;
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _userService.GetUserStoredDetailByIdForAdminAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get user stored detail: Access denied", result.Message);
//        }

//        [Test]
//        public async Task GetUserStoredDetailByIdForAdminAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var userId = 1;
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _userService.GetUserStoredDetailByIdForAdminAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving user stored details: Database error", result.Message);
//        }

//        [Test]
//        public async Task SearchUsersStoredForAdminAsync_ShouldReturnPagedResponse_WhenUsersExist()
//        {
//            // Arrange
//            var users = new List<User>
//        {
//            new User { UserId = 1, Name = "Deleted User 1", Status = "Deleted" },
//            new User { UserId = 2, Name = "Deleted User 2", Status = "Deleted" }
//        };

//            var userDtos = new List<UserListForAdminDTO>
//        {
//            new UserListForAdminDTO { UserId = 1, Name = "Deleted User 1" },
//            new UserListForAdminDTO { UserId = 2, Name = "Deleted User 2" }
//        };

//            _userRepositoryMock.Setup(repo => repo.SearchUsersStoredAsync(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(users);
//            _mapperMock.Setup(mapper => mapper.Map<List<UserListForAdminDTO>>(users)).Returns(userDtos);

//            // Act
//            var result = await _userService.SearchUsersStoredForAdminAsync(null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(2, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task SearchUsersStoredForAdminAsync_ShouldReturnNotFound_WhenNoUsersMatchSearchCriteria()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.SearchUsersStoredAsync(It.IsAny<string>(), It.IsAny<int?>()))
//                               .ThrowsAsync(new KeyNotFoundException("No users stored found matching the search criteria."));

//            // Act
//            var result = await _userService.SearchUsersStoredForAdminAsync(null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("No users stored found matching the search criteria.", result.Message);
//        }

//        [Test]
//        public async Task SearchUsersStoredForAdminAsync_ShouldReturnUnauthorized_WhenAccessDenied()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.SearchUsersStoredAsync(It.IsAny<string>(), It.IsAny<int?>()))
//                               .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _userService.SearchUsersStoredForAdminAsync(null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while search users stored: Access denied", result.Message);
//        }

//        [Test]
//        public async Task SearchUsersStoredForAdminAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            _userRepositoryMock.Setup(repo => repo.SearchUsersStoredAsync(It.IsAny<string>(), It.IsAny<int?>()))
//                               .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _userService.SearchUsersStoredForAdminAsync(null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error occurred during search: Database error", result.Message);
//        }

//        [Test]
//        public async Task HardDeleteUserStoredForAdminAsync_ShouldReturnSuccess_WhenUserIsPermanentlyDeleted()
//        {
//            // Arrange
//            var deleteUserDto = new DeleteUserForAdminDTO { UserId = 1 };
//            var user = new User { UserId = 1, Status = "Deleted" };

//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User> { user });
//            _userRepositoryMock.Setup(repo => repo.HardDeleteUserStoredAsync(1)).ReturnsAsync(user);

//            var deleteUserResultDto = new DeleteUserForAdminDTO { UserId = 1 };
//            _mapperMock.Setup(mapper => mapper.Map<DeleteUserForAdminDTO>(user)).Returns(deleteUserResultDto);

//            // Act
//            var result = await _userService.HardDeleteUserStoredForAdminAsync(deleteUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("User Stored has been permanently deleted successfully.", result.Message);
//        }

//        [Test]
//        public async Task HardDeleteUserStoredForAdminAsync_ShouldReturnNotFound_WhenUserNotInStoredList()
//        {
//            // Arrange
//            var deleteUserDto = new DeleteUserForAdminDTO { UserId = 1 };
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User>());

//            // Act
//            var result = await _userService.HardDeleteUserStoredForAdminAsync(deleteUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User is not in the stored list.", result.Message);
//        }

//        [Test]
//        public async Task HardDeleteUserStoredForAdminAsync_ShouldReturnBadRequest_WhenUserIsNotDeleted()
//        {
//            // Arrange
//            var deleteUserDto = new DeleteUserForAdminDTO { UserId = 1 };
//            var user = new User { UserId = 1, Status = "Deleted" };

//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User> { user });
//            _userRepositoryMock.Setup(repo => repo.HardDeleteUserStoredAsync(1)).ThrowsAsync(new InvalidOperationException("Cannot permanently delete the user because it does not exist in the stored user list."));

//            // Act
//            var result = await _userService.HardDeleteUserStoredForAdminAsync(deleteUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("Cannot permanently delete the user because it does not exist in the stored user list.", result.Message);
//        }

//        [Test]
//        public async Task HardDeleteUserStoredForAdminAsync_ShouldReturnUnauthorized_WhenAccessDenied()
//        {
//            // Arrange
//            var deleteUserDto = new DeleteUserForAdminDTO { UserId = 1 };
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _userService.HardDeleteUserStoredForAdminAsync(deleteUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while hard delete user stored: Access denied", result.Message);
//        }

//        [Test]
//        public async Task HardDeleteUserStoredForAdminAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var deleteUserDto = new DeleteUserForAdminDTO { UserId = 1 };
//            var user = new User { UserId = 1, Status = "Deleted" };

//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User> { user });
//            _userRepositoryMock.Setup(repo => repo.HardDeleteUserStoredAsync(1))
//                               .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _userService.HardDeleteUserStoredForAdminAsync(deleteUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);  // Kiểm tra mã lỗi 500
//            Assert.AreEqual("Error permanently deleting user stored: Database error", result.Message);  // Kiểm tra thông báo lỗi
//        }

//        [Test]
//        public async Task RestoreUserForAdminAsync_ShouldReturnSuccess_WhenUserIsRestored()
//        {
//            // Arrange
//            var restoreUserDto = new RestoreUserForAdminDTO { UserId = 1 };
//            var user = new User { UserId = 1, Status = "Deleted" };

//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User> { user });
//            _userRepositoryMock.Setup(repo => repo.RestoreUserStoredAsync(1)).ReturnsAsync(user);

//            var restoreUserResultDto = new RestoreUserForAdminDTO { UserId = 1 };
//            _mapperMock.Setup(mapper => mapper.Map<RestoreUserForAdminDTO>(user)).Returns(restoreUserResultDto);

//            // Act
//            var result = await _userService.RestoreUserForAdminAsync(restoreUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("User Stored has been restored successfully.", result.Message);
//        }

//        [Test]
//        public async Task RestoreUserForAdminAsync_ShouldReturnNotFound_WhenUserNotInStoredList()
//        {
//            // Arrange
//            var restoreUserDto = new RestoreUserForAdminDTO { UserId = 1 };
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User>());

//            // Act
//            var result = await _userService.RestoreUserForAdminAsync(restoreUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User is not in the stored list.", result.Message);
//        }

//        [Test]
//        public async Task RestoreUserForAdminAsync_ShouldReturnBadRequest_WhenUserIsNotDeleted()
//        {
//            // Arrange
//            var restoreUserDto = new RestoreUserForAdminDTO { UserId = 1 };
//            var user = new User { UserId = 1, Status = "Deleted" };

//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User> { user });
//            _userRepositoryMock.Setup(repo => repo.RestoreUserStoredAsync(1)).ThrowsAsync(new InvalidOperationException("Cannot restore the user because it does not exist in the stored user list."));

//            // Act
//            var result = await _userService.RestoreUserForAdminAsync(restoreUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("Cannot restore the user because it does not exist in the stored user list.", result.Message);
//        }

//        [Test]
//        public async Task RestoreUserForAdminAsync_ShouldReturnUnauthorized_WhenAccessDenied()
//        {
//            // Arrange
//            var restoreUserDto = new RestoreUserForAdminDTO { UserId = 1 };
//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _userService.RestoreUserForAdminAsync(restoreUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while restore user stored: Access denied", result.Message);
//        }

//        [Test]
//        public async Task RestoreUserForAdminAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var restoreUserDto = new RestoreUserForAdminDTO { UserId = 1 };
//            var user = new User { UserId = 1, Status = "Deleted" };

//            _userRepositoryMock.Setup(repo => repo.GetAllUsersStoredAsync()).ReturnsAsync(new List<User> { user });
//            _userRepositoryMock.Setup(repo => repo.RestoreUserStoredAsync(1)).ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _userService.RestoreUserForAdminAsync(restoreUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error restoring user stored: Database error", result.Message);
//        }
//    }
//}
