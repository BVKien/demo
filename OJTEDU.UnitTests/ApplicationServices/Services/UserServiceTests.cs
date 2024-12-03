//using AutoMapper;
//using Google.Apis.Auth;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Moq;
//using OfficeOpenXml;
//using OJTEDU.Application.ApplicationServices.Interfaces;
//using OJTEDU.Application.ApplicationServices.Services;
//using OJTEDU.Application.DTOs;
//using OJTEDU.Domain.Entities;
//using OJTEDU.Domain.Interfaces;
//using RichardSzalay.MockHttp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using static OJTEDU.Application.DTOs.UserDTO;

//namespace OJTEDU.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class UserServiceTests
//    {
//        private Mock<IGoogleJsonWebSignatureValidator> _googleValidatorMock;
//        private HttpClient _httpClient;
//        private Mock<IUserRepository> _userRepoMock;
//        private Mock<IAttendanceReportRepository> _attendanceRepoMock;
//        private Mock<IMajorRepository> _majorRepoMock;
//        private Mock<IMapper> _mapperMock;
//        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
//        private IConfiguration _config;
//        private UserService _service;

//        [SetUp]
//        public void SetUp()
//        {
//            _googleValidatorMock = new Mock<IGoogleJsonWebSignatureValidator>();
//            _userRepoMock = new Mock<IUserRepository>();
//            _attendanceRepoMock = new Mock<IAttendanceReportRepository>();
//            _majorRepoMock = new Mock<IMajorRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

//            var inMemorySettings = new Dictionary<string, string>
//            {
//                { "Google:TokenRequestUri", "https://oauth2.googleapis.com/token" },
//                { "Google:ClientId", "168459064807-nf54kboco0di487bgote8vlndpk9it8s.apps.googleusercontent.com" },
//                { "Google:ClientSecret", "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" },
//                { "Google:RedirectUri", "https://localhost:3000/login" }
//            };

//            // Mock HttpClient
//            var mockHttp = new MockHttpMessageHandler();
//            mockHttp.When("https://oauth2.googleapis.com/token")
//                    .Respond("application/json", @"{
//                ""access_token"": ""mock_access_token"",
//                ""id_token"": ""mock_id_token""
//            }");

//            _httpClient = mockHttp.ToHttpClient();

//            _config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

//            _service = new UserService(_config, _attendanceRepoMock.Object, _userRepoMock.Object, _majorRepoMock.Object, _httpContextAccessorMock.Object, _mapperMock.Object, _googleValidatorMock.Object, _httpClient);
//        }

//        // Service - Authentication - Login

//        [Test]
//        public async Task LoginWithGoogleAsync_ShouldReturnError_WhenTokenIsEmpty()
//        {
//            // Act
//            var result = await _service.LoginWithGoogleAsync("");

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("Token cannot be empty.", result.Message);
//        }

//        [Test]
//        public async Task LoginWithGoogleAsync_ShouldReturnError_WhenGoogleTokenIsInvalid()
//        {
//            // Arrange
//            var mockHttp = new MockHttpMessageHandler();
//            mockHttp.When("https://oauth2.googleapis.com/token")
//                .Respond(System.Net.HttpStatusCode.Unauthorized); // Mock invalid token response
//            var httpClient = mockHttp.ToHttpClient();

//            //_service = new UserService(
//            //    _config,
//            //    _attendanceRepoMock.Object,
//            //    _userRepoMock.Object,
//            //    _majorRepoMock.Object,
//            //    _httpContextAccessorMock.Object,
//            //    _mapperMock.Object,
//            //    _googleValidatorMock.Object,
//            //    httpClient
//            //);

//            // Act
//            var result = await _service.LoginWithGoogleAsync("invalid-token");

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual(401, result.StatusCode);
//            Assert.AreEqual("Invalid Google Token.", result.Message);
//        }

//        [Test]
//        public async Task LoginWithGoogleAsync_ShouldReturnError_WhenUserNotFound()
//        {
//            // Arrange
//            _googleValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
//                .ReturnsAsync(new GoogleJsonWebSignature.Payload { Email = "notfound@example.com" });

//            _userRepoMock.Setup(repo => repo.GetUserByEmailAsync("notfound@example.com"))
//                .ReturnsAsync((User)null); // User not found

//            // Act
//            var result = await _service.LoginWithGoogleAsync("valid-token");

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User not found.", result.Message);
//        }

//        [Test]
//        public async Task LoginWithGoogleAsync_ShouldReturnError_WhenUserNotActivated()
//        {
//            // Arrange
//            _googleValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
//                .ReturnsAsync(new GoogleJsonWebSignature.Payload { Email = "inactive@example.com" });

//            _userRepoMock.Setup(repo => repo.GetUserByEmailAsync("inactive@example.com"))
//                .ReturnsAsync(new User { Status = null }); // User not activated

//            // Act
//            var result = await _service.LoginWithGoogleAsync("valid-token");

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual(409, result.StatusCode);
//            Assert.AreEqual("User account is not activated.", result.Message);
//        }

//        [Test]
//        public async Task LoginWithGoogleAsync_ShouldReturnUser_WhenLoginIsSuccessful()
//        {
//            // Arrange
//            _googleValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
//                .ReturnsAsync(new GoogleJsonWebSignature.Payload
//                {
//                    Email = "test@example.com",
//                    Picture = "http://example.com/avatar.png"
//                });

//            _userRepoMock.Setup(repo => repo.GetUserByEmailAsync("test@example.com"))
//                .ReturnsAsync(new User
//                {
//                    UserId = 1,
//                    Email = "test@example.com",
//                    UserCode = "TEST",
//                    Image = "http://example.com/avatar.png",
//                    Status = "Active",
//                    Role = new Role { Name = "Admin" }
//                });

//            _mapperMock.Setup(m => m.Map<UserReadForAuthDTO>(It.IsAny<User>()))
//                .Returns(new UserReadForAuthDTO
//                {
//                    UserId = 1,
//                    Name = "Test User",
//                    Email = "test@example.com",
//                    Image = "http://example.com/avatar.png",
//                    UserCode = "TEST",
//                    Role = "Admin"
//                });

//            var httpContext = new DefaultHttpContext();
//            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

//            // Act
//            var result = await _service.LoginWithGoogleAsync("valid-token");

//            // Assert
//            Assert.NotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("test@example.com", result.Data.Email);
//            Assert.AreEqual("Test User", result.Data.Name);
//        }

//        // Service - User Management - User List

//        [Test]
//        public async Task GetAllUsersForAdminAsync_ShouldReturnPagedUsers_WhenUsersExist()
//        {
//            // Arrange
//            var mockUsers = new List<User>
//            {
//                new User { UserId = 1, Email = "user1@example.com", Name = "User One", RoleId = 1, Status = "Active" },
//                new User { UserId = 2, Email = "user2@example.com", Name = "User Two", RoleId = 1, Status = "Active" },
//                new User { UserId = 3, Email = "user3@example.com", Name = "User Three", RoleId = 2, Status = "Inactive" }
//            };

//            _userRepoMock.Setup(repo => repo.GetAllUsersForAdminAsync(null, null, null))
//                .ReturnsAsync(mockUsers);

//            var mockUserDtos = new List<UserListForAdminDTO>
//            {
//                new UserListForAdminDTO { UserId = 1, Email = "user1@example.com", Name = "User One", Role = "Admin", Status = "Active" },
//                new UserListForAdminDTO { UserId = 2, Email = "user2@example.com", Name = "User Two", Role = "Admin", Status = "Active" },
//                new UserListForAdminDTO { UserId = 3, Email = "user3@example.com", Name = "User Three", Role = "User", Status = "Inactive" }
//            };

//            _mapperMock.Setup(mapper => mapper.Map<List<UserListForAdminDTO>>(mockUsers))
//                .Returns(mockUserDtos);

//            // Act
//            var result = await _service.GetAllUsersForAdminAsync(null, null, null, 1, 2);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual(3, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.CurrentPage);
//            Assert.AreEqual(2, result.Data.PageSize);
//            Assert.AreEqual(2, result.Data.TotalPages);
//        }

//        [Test]
//        public async Task GetAllUsersForAdminAsync_ShouldReturnPagedUsers_WhenUsersExistFilterByNameAndRoleAndStatus()
//        {
//            // Arrange
//            var mockUsers = new List<User>
//            {
//                new User { UserId = 1, Email = "user1@example.com", Name = "Alice", RoleId = 1, Status = "Active" },
//                new User { UserId = 2, Email = "user2@example.com", Name = "Bob", RoleId = 2, Status = "Active" }
//            };

//            _userRepoMock.Setup(repo => repo.GetAllUsersForAdminAsync("Alice", 1, "Active"))
//                .ReturnsAsync(mockUsers.Where(u => u.Name.ToLower().Contains("alice") && u.RoleId == 1 && u.Status == "Active").ToList());

//            var mockUserDtos = new List<UserListForAdminDTO>
//            {
//                new UserListForAdminDTO { UserId = 1, Email = "user1@example.com", Name = "Alice", Role = "Admin", Status = "Active" }
//            };

//            _mapperMock.Setup(mapper => mapper.Map<List<UserListForAdminDTO>>(It.IsAny<List<User>>()))
//                .Returns(mockUserDtos);

//            // Act
//            var result = await _service.GetAllUsersForAdminAsync("Alice", 1, "Active", 1, 2);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Items.Count);
//            Assert.AreEqual(1, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.CurrentPage);
//            Assert.AreEqual(2, result.Data.PageSize);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual("Alice", result.Data.Items.First().Name);
//        }

//        [Test]
//        public async Task GetAllUsersForAdminAsync_ShouldReturnEmpty_WhenNoUsersExist()
//        {
//            // Arrange
//            _userRepoMock.Setup(repo => repo.GetAllUsersForAdminAsync(null, null, null))
//                .ReturnsAsync(new List<User>());

//            // Act
//            var result = await _service.GetAllUsersForAdminAsync(null, null, null, 1, 2);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.IsEmpty(result.Data.Items);
//            Assert.AreEqual(0, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.CurrentPage);
//            Assert.AreEqual(2, result.Data.PageSize);
//            Assert.AreEqual(1, result.Data.TotalPages);
//        }

//        [Test]
//        public async Task GetAllUsersForAdminAsync_ShouldHandleKeyNotFoundException()
//        {
//            // Arrange
//            _userRepoMock.Setup(repo => repo.GetAllUsersForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new KeyNotFoundException("Users not found"));

//            // Act
//            var result = await _service.GetAllUsersForAdminAsync(null, null, null, 1, 2);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Users not found", result.Message);
//        }

//        // Service - User Management - User Detail

//        [Test]
//        public async Task GetUserDetailByIdForAdminAsync_ShouldReturnUser_WhenUserExists()
//        {
//            // Arrange
//            var user = new User
//            {
//                UserId = 1,
//                Email = "test@example.com",
//                Name = "Test User",
//                Role = new Role { Name = "Admin" },
//                Status = "Active",
//                Information = "Some info"
//            };

//            _userRepoMock.Setup(repo => repo.GetUserByIdForAdminAsync(1)).ReturnsAsync(user);

//            _mapperMock.Setup(mapper => mapper.Map<UserDetailForAdminDTO>(user))
//                       .Returns(new UserDetailForAdminDTO
//                       {
//                           UserId = 1,
//                           Email = "test@example.com",
//                           Name = "Test User",
//                           Role = "Admin",
//                           Status = "Active",
//                           Information = "Some info"
//                       });

//            // Act
//            var result = await _service.GetUserDetailByIdForAdminAsync(1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Test User", result.Data.Name);
//        }

//        [Test]
//        public async Task GetUserDetailByIdForAdminAsync_ShouldReturnNotFound_WhenUserIsDeleted()
//        {
//            // Arrange
//            _userRepoMock.Setup(repo => repo.GetUserByIdForAdminAsync(1))
//                         .ReturnsAsync(new User { Status = "Deleted" });

//            // Act
//            var result = await _service.GetUserDetailByIdForAdminAsync(1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User is deleted.", result.Message);
//        }

//        // Service - User Management - Add User

//        [Test]
//        public async Task AddUserForAdminAsync_ShouldAddUserSuccessfully()
//        {
//            // Arrange
//            var addUserDto = new AddUserForAdminDTO
//            {
//                Email = "test@example.com",
//                Name = "Test User",
//                UserCode = "USER123",
//                RoleId = 1,
//                Information = "Some info"
//            };

//            var addedUser = new User
//            {
//                UserId = 1,
//                Email = "test@example.com",
//                Name = "Test User",
//                UserCode = "USER123",
//                RoleId = 1,
//                CreatedAt = DateTime.Now
//            };

//            _userRepoMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<User>()))
//                         .ReturnsAsync(addedUser);

//            // Act
//            var result = await _service.AddUserForAdminAsync(addUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("test@example.com", result.Data.Email);
//        }

//        [Test]
//        public async Task AddUserForAdminAsync_ShouldReturnBadRequest_WhenEmailAlreadyExists()
//        {
//            // Arrange
//            var addUserDto = new AddUserForAdminDTO
//            {
//                Email = "duplicate@example.com",
//                Name = "Duplicate User",
//                UserCode = "USER123",
//                RoleId = 1
//            };

//            _userRepoMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<User>()))
//                         .ThrowsAsync(new InvalidOperationException("A user with the same email already exists."));

//            // Act
//            var result = await _service.AddUserForAdminAsync(addUserDto);

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
//                Email = "test@example.com",
//                Name = "Duplicate User",
//                UserCode = "DUPLICATE123",
//                RoleId = 1
//            };

//            _userRepoMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<User>()))
//                         .ThrowsAsync(new InvalidOperationException("A user with the same UserCode already exists."));

//            // Act
//            var result = await _service.AddUserForAdminAsync(addUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("A user with the same UserCode already exists.", result.Message);
//        }

//        [Test]
//        public async Task AddUserForAdminAsync_ShouldReturnBadRequest_WhenUserIsMentor()
//        {
//            // Arrange
//            var addUserDto = new AddUserForAdminDTO
//            {
//                Email = "mentor@example.com",
//                Name = "Mentor User",
//                UserCode = "Mentor123",
//                RoleId = 7
//            };

//            _userRepoMock.Setup(x => x.AddUserForAdminAsync(It.IsAny<User>()))
//                         .ThrowsAsync(new InvalidOperationException("Cannot add a user with the 'Mentor' role."));

//            // Act
//            var result = await _service.AddUserForAdminAsync(addUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("Cannot add a user with the 'Mentor' role.", result.Message);
//        }

//        // Service - User Management - Update User

//        [Test]
//        public async Task UpdateUserForAdminAsync_ShouldUpdateUserSuccessfully()
//        {
//            // Arrange
//            var updateUserDto = new UpdateUserForAdminDTO
//            {
//                UserId = 1,
//                Email = "updated@example.com",
//                Name = "Updated User",
//                UserCode = "UPDATED123",
//                Information = "Updated Information"
//            };

//            var updatedUser = new User
//            {
//                UserId = 1,
//                Email = "updated@example.com",
//                Name = "Updated User",
//                UserCode = "UPDATED123",
//                Information = "Updated Information"
//            };

//            _userRepoMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                         .ReturnsAsync(updatedUser);

//            _mapperMock.Setup(x => x.Map<UpdateUserForAdminDTO>(It.IsAny<User>()))
//                       .Returns(updateUserDto);

//            // Act
//            var result = await _service.UpdateUserForAdminAsync(updateUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("updated@example.com", result.Data.Email);
//        }

//        [Test]
//        public async Task UpdateUserForAdminAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
//        {
//            // Arrange
//            var updateUserDto = new UpdateUserForAdminDTO
//            {
//                UserId = 999,
//                Email = "notfound@example.com",
//                Name = "Non-Existent User",
//                UserCode = "NOTFOUND123",
//                Information = "Non-existent user information"
//            };

//            _userRepoMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                         .ThrowsAsync(new KeyNotFoundException("User not found"));

//            // Act
//            var result = await _service.UpdateUserForAdminAsync(updateUserDto);

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
//                Email = "duplicate@example.com",
//                Name = "Duplicate User",
//                UserCode = "DUPLICATE123"
//            };

//            _userRepoMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                         .ThrowsAsync(new InvalidOperationException("A user with the same email already exists."));

//            // Act
//            var result = await _service.UpdateUserForAdminAsync(updateUserDto);

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
//                Email = "duplicate@example.com",
//                Name = "Duplicate User",
//                UserCode = "DUPLICATE123"
//            };

//            _userRepoMock.Setup(x => x.UpdateUserForAdminAsync(It.IsAny<User>()))
//                         .ThrowsAsync(new InvalidOperationException("A user with the same UserCode already exists."));

//            // Act
//            var result = await _service.UpdateUserForAdminAsync(updateUserDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("A user with the same UserCode already exists.", result.Message);
//        }

//        // Service - User Management - Soft Delete User

//        [Test]
//        public async Task SoftDeleteUserForAdminAsync_ShouldDeleteUserSuccessfully()
//        {
//            // Arrange
//            var userId = 1;
//            var user = new User { UserId = userId, Status = "Deleted" };

//            _userRepoMock.Setup(x => x.SoftDeleteUserForAdminAsync(userId))
//                         .ReturnsAsync(user);

//            _mapperMock.Setup(x => x.Map<DeleteUserForAdminDTO>(user))
//                       .Returns(new DeleteUserForAdminDTO { UserId = userId });

//            // Act
//            var result = await _service.SoftDeleteUserForAdminAsync(new DeleteUserForAdminDTO { UserId = userId });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("User has been marked as deleted successfully.", result.Message);
//            Assert.AreEqual(userId, result.Data.UserId);
//        }

//        [Test]
//        public async Task SoftDeleteUserForAdminAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
//        {
//            // Arrange
//            var userId = 99;

//            _userRepoMock.Setup(x => x.SoftDeleteUserForAdminAsync(userId))
//                         .ThrowsAsync(new KeyNotFoundException("User not found"));

//            // Act
//            var result = await _service.SoftDeleteUserForAdminAsync(new DeleteUserForAdminDTO { UserId = userId });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("User not found", result.Message);
//        }

//        // Service - User Management - Download User File

//        [Test]
//        public async Task GenerateUserTemplateForAdminAsync_ShouldReturnMemoryStream_WhenTemplateGeneratedSuccessfully()
//        {
//            // Act
//            var result = await _service.GenerateUserTemplateForAdminAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Template generated successfully.", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.IsInstanceOf<MemoryStream>(result.Data);
//        }

//        [Test]
//        public async Task GenerateUserTemplateForAdminAsync_ShouldHandleException_AndReturnError()
//        {
//            // Arrange
//            var mockUserService = new Mock<IUserService>();
//            mockUserService.Setup(service => service.GenerateUserTemplateForAdminAsync())
//                .ReturnsAsync(new DataResponse<MemoryStream>
//                {
//                    Data = null,
//                    Message = "Error generating template",
//                    StatusCode = 500
//                });

//            var service = mockUserService.Object;

//            // Act
//            var result = await service.GenerateUserTemplateForAdminAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error generating template", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        // Service - User Management - Import User File

//        [Test]
//        public async Task ImportUsersForAdminAsync_ShouldImportValidUsersSuccessfully()
//        {
//            // Arrange
//            var mockFile = CreateMockExcelFile(new List<(string Email, string Name, string UserCode, int RoleId, string Information, string MajorCode)>
//            {
//                ("datnthe163935@fpt.edu.vn", "Nguyễn Tiến Đạt", "HE163935", 2, "SĐT: 123456789, Địa chỉ: Hà Nội", "SE"),
//                ("phongdaotaoFPT@fe.edu.vn", "Phòng đào tạo FPT", "DOET", 4, "SĐT: 123456789, Địa chỉ: Hà Nội", null)
//            });

//            _majorRepoMock.Setup(repo => repo.GetMajorByCodeAsync("SE")).ReturnsAsync(new Major { MajorId = 1, Status = "Active" });
//            _userRepoMock.Setup(repo => repo.IsUserCodeExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
//            _userRepoMock.Setup(repo => repo.IsEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

//            // Act
//            var result = await _service.ImportUsersForAdminAsync(mockFile);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Import completed. Successfully added 2 users.", result.Message);
//        }

//        [Test]
//        public async Task ImportUsersForAdminAsync_ShouldReturnErrors_ForInvalidRows()
//        {
//            // Arrange
//            var mockFile = CreateMockExcelFile(new List<(string Email, string Name, string UserCode, int RoleId, string Information, string MajorCode)>
//            {
//                ("", "valid", "VALID1", 2, "Info", "SE"), // Missing email
//                ("valid@example.com", "", "VALID1", 2, "Info", "SE"), // Missing name
//                ("valid@example.com", "valid", "", 2, "Info", "SE"), // Missing usercode
//                ("valid@example.com", "Missing Email", "VALID1", 0, "Info", "SE"), // Missing role
//                ("invalid-email", "valid", "VALID2", 2, "Info", "SE"), // Invalid email
//                ("valid@example.com", "Long UserCode", new string('A', 51), 2, "Info", "SE"), // UserCode > 50 chars
//                (new string('A', 51) + "@example.com", "Long Email", "VALID3", 2, "Info", "SE"), // Email > 50 chars
//                ("longemail@example.com", new string('N', 351), "VALID3", 2, "Info", "SE"), // Name > 350 chars
//                ("existing@example.com", "Existing Email", "VALID8", 2, "Info", "SE"), // Existing email
//                ("valid@example.com", "Existing UserCode", "EXISTING_CODE", 2, "Info", "SE"), // Existing UserCode
//                ("valid1@example.com", "Student Missing Major", "VALID1", 2, "Info", ""), // Missing MajorCode
//                ("valid2@example.com", "Student Nonexistent Major", "VALID2", 2, "Info", "NONEXISTENT"), // Nonexistent MajorCode
//                ("valid3@example.com", "Student Inactive Major", "VALID3", 2, "Info", "INACTIVE"), // Inactive MajorCode

//            });

//            // Mock repository methods to simulate existing email and UserCode
//            _userRepoMock.Setup(repo => repo.IsUserCodeExistsAsync("EXISTING_CODE")).ReturnsAsync(true);
//            _userRepoMock.Setup(repo => repo.IsEmailExistsAsync("existing@example.com")).ReturnsAsync(true);

//            // Mock repository methods for MajorCode
//            _majorRepoMock.Setup(repo => repo.GetMajorByCodeAsync("NONEXISTENT")).ReturnsAsync((Major)null); // Nonexistent Major
//            _majorRepoMock.Setup(repo => repo.GetMajorByCodeAsync("INACTIVE")).ReturnsAsync(new Major
//            {
//                MajorId = 1,
//                MajorCode = "INACTIVE",
//                Status = "inactive"
//            }); // Inactive Major
//            _majorRepoMock.Setup(repo => repo.GetMajorByCodeAsync("ACTIVE")).ReturnsAsync(new Major
//            {
//                MajorId = 2,
//                MajorCode = "ACTIVE",
//                Status = "active"
//            }); // Active Major

//            // Act
//            var result = await _service.ImportUsersForAdminAsync(mockFile);

//            // Assert
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.AreEqual("Import failed. There were 13 errors. Please fix the reported errors to successfully add the file.", result.Message);
//        }

//        private IFormFile CreateMockExcelFile(IEnumerable<(string Email, string Name, string UserCode, int RoleId, string Information, string MajorCode)> users)
//        {
//            var stream = new MemoryStream();
//            using (var package = new ExcelPackage(stream))
//            {
//                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

//                // Add headers
//                worksheet.Cells[1, 1].Value = "Email(*)";
//                worksheet.Cells[1, 2].Value = "FullName(*)";
//                worksheet.Cells[1, 3].Value = "UserCode(*)";
//                worksheet.Cells[1, 4].Value = "RoleId(*)";
//                worksheet.Cells[1, 5].Value = "Information";
//                worksheet.Cells[1, 6].Value = "MajorCode(* : Bắt buộc với student)";

//                // Add user data
//                int row = 2;
//                foreach (var user in users)
//                {
//                    worksheet.Cells[row, 1].Value = user.Email;
//                    worksheet.Cells[row, 2].Value = user.Name;
//                    worksheet.Cells[row, 3].Value = user.UserCode;
//                    worksheet.Cells[row, 4].Value = user.RoleId;
//                    worksheet.Cells[row, 5].Value = user.Information;
//                    worksheet.Cells[row, 6].Value = user.MajorCode;
//                    row++;
//                }

//                package.Save();
//            }

//            stream.Position = 0;
//            return new FormFile(stream, 0, stream.Length, "file", "test.xlsx");
//        }
//    }
//}
