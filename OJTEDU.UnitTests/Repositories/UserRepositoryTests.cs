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
    public class UserRepositoryTests
    {
        private OJTEDU_DB_V1Context _context;
        private UserRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<OJTEDU_DB_V1Context>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new OJTEDU_DB_V1Context(options);
            _repository = new UserRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // Get All User

        [Test]
        public async Task GetAllUsersForAdminAsync_ShouldReturnFilteredAndSortedUsers_WhenUsersExists()
        {
            // Arrange
            var adminRole = new Role { RoleId = 1, Name = "Admin" };
            var userRole = new Role { RoleId = 2, Name = "Student" };

            var users = new List<User>
            {
                new User { UserId = 1, Name = "Admin User", Email = "admin@example.com", Role = adminRole, RoleId = adminRole.RoleId, Status = "Active" },
                new User { UserId = 2, Name = "Student One", Email = "user1@example.com", Role = userRole, RoleId = userRole.RoleId, Status = "Active" },
                new User { UserId = 3, Name = "Student Two", Email = "user2@example.com", Role = userRole, RoleId = userRole.RoleId, Status = "Unactive" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUsersForAdminAsync(null, null, null);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(3, resultList.Count);
            Assert.AreEqual("Admin User", resultList[0].Name); // Admin comes first
            Assert.AreEqual("Student One", resultList[1].Name);  // Active user next
            Assert.AreEqual("Student Two", resultList[2].Name);  // Unactive user last
        }

        [Test]
        public async Task GetAllUsersForAdminAsync_ShouldApplyFiltersCorrectly_WhenUsersExists()
        {
            // Arrange
            var adminRole = new Role { RoleId = 1, Name = "Admin" };
            var userRole = new Role { RoleId = 2, Name = "Student" };

            var users = new List<User>
            {
                new User { UserId = 1, Name = "Alice Admin", Email = "admin@example.com", Role = adminRole, RoleId = adminRole.RoleId, Status = "Active" },
                new User { UserId = 2, Name = "Bob Student", Email = "user1@example.com", Role = userRole, RoleId = userRole.RoleId, Status = "Active" },
                new User { UserId = 3, Name = "Charlie Student", Email = "user2@example.com", Role = userRole, RoleId = userRole.RoleId, Status = "Unactive" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUsersForAdminAsync("Alice", 1, "Active");

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count); // Only one user matches the filters
            Assert.AreEqual("Alice Admin", resultList[0].Name);
            Assert.AreEqual("admin@example.com", resultList[0].Email);
        }

        // Get User By Id

        [Test]
        public async Task GetUserByIdForAdminAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = new Role { Name = "Admin" },
                Status = "Active"
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByIdForAdminAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Test User", result.Name);
        }

        [Test]
        public void GetUserByIdForAdminAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.GetUserByIdForAdminAsync(99));
        }

        // Get User By Email

        [Test]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                Name = "Test User",
                Role = new Role { Name = "Admin" }
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByEmailAsync("test@example.com");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test User", result.Name);
            Assert.AreEqual("Admin", result.Role.Name);
        }

        [Test]
        public void GetUserByEmailAsync_ShouldThrowException_WhenDatabaseFails()
        {
            // Arrange
            var invalidRepo = new UserRepository(null);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await invalidRepo.GetUserByEmailAsync("test@example.com"));
        }

        // Add User

        [Test]
        public async Task AddUserForAdminAsync_ShouldAddUserSuccessfully()
        {
            // Arrange
            var mentorRole = new Role
            {
                RoleId = 1,
                Name = "Admin"
            };
            await _context.Roles.AddAsync(mentorRole);
            await _context.SaveChangesAsync();

            var user = new User
            {
                Email = "test@example.com",
                Name = "Test User",
                UserCode = "USER123",
                RoleId = 1
            };

            // Act
            var result = await _repository.AddUserForAdminAsync(user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test@example.com", result.Email);
        }

        [Test]
        public async Task AddUserForAdminAsync_ShouldThrowInvalidOperationException_WhenEmailAlreadyExists()
        {
            // Arrange
            var user1 = new User
            {
                Email = "duplicate@example.com",
                Name = "User One",
                UserCode = "USER123",
                RoleId = 1
            };

            var user2 = new User
            {
                Email = "duplicate@example.com",
                Name = "User Two",
                UserCode = "USER124",
                RoleId = 1
            };

            await _context.Users.AddAsync(user1);
            await _context.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _repository.AddUserForAdminAsync(user2));
        }

        [Test]
        public async Task AddUserForAdminAsync_ShouldThrowInvalidOperationException_WhenUserCodeAlreadyExists()
        {
            // Arrange
            var user1 = new User
            {
                Email = "user1@example.com",
                Name = "User One",
                UserCode = "USER123",
                RoleId = 1
            };

            var user2 = new User
            {
                Email = "user2@example.com",
                Name = "User Two",
                UserCode = "USER123",
                RoleId = 1
            };

            await _context.Users.AddAsync(user1);
            await _context.SaveChangesAsync();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _repository.AddUserForAdminAsync(user2));
        }

        [Test]
        public async Task AddUserForAdminAsync_ShouldThrowInvalidOperationException_WhenRoleIsMentor()
        {
            // Arrange
            var mentorRole = new Role
            {
                RoleId = 7,
                Name = "Mentor"
            };
            await _context.Roles.AddAsync(mentorRole);
            await _context.SaveChangesAsync();

            var user = new User
            {
                Email = "mentor@example.com",
                Name = "Mentor User",
                UserCode = "MENTOR123",
                RoleId = 7 // Mentor Role
            };

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _repository.AddUserForAdminAsync(user));
            Assert.AreEqual("Cannot add a user with the 'Mentor' role.", exception.Message);
        }

        // Update User 

        [Test]
        public async Task UpdateUserForAdminAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var user = new User
            {
                UserId = 999, // UserId không tồn tại
                Email = "newemail@example.com",
                Name = "New Name"
            };

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.UpdateUserForAdminAsync(user));
        }

        [Test]
        public async Task UpdateUserForAdminAsync_ShouldThrowInvalidOperationException_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser1 = new User { UserId = 1, Email = "existing1@example.com", Name = "User 1" };
            var existingUser2 = new User { UserId = 2, Email = "existing2@example.com", Name = "User 2" };
            await _context.Users.AddRangeAsync(existingUser1, existingUser2);
            await _context.SaveChangesAsync();

            var userToUpdate = new User
            {
                UserId = 1,
                Email = "existing2@example.com", // Email bị trùng với UserId 2
                Name = "Updated Name"
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _repository.UpdateUserForAdminAsync(userToUpdate));
        }

        [Test]
        public async Task UpdateUserForAdminAsync_ShouldThrowInvalidOperationException_WhenUserCodeAlreadyExists()
        {
            // Arrange
            var existingUser1 = new User { UserId = 1, UserCode = "USER1", Name = "User 1" };
            var existingUser2 = new User { UserId = 2, UserCode = "USER2", Name = "User 2" };
            await _context.Users.AddRangeAsync(existingUser1, existingUser2);
            await _context.SaveChangesAsync();

            var userToUpdate = new User
            {
                UserId = 1,
                UserCode = "USER2", // UserCode bị trùng với UserId 2
                Name = "Updated Name"
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _repository.UpdateUserForAdminAsync(userToUpdate));
        }

        [Test]
        public async Task UpdateUserForAdminAsync_ShouldUpdateUserSuccessfully_WhenDataIsValid()
        {
            // Arrange
            var existingUser = new User
            {
                UserId = 1,
                Email = "original@example.com",
                Name = "Original Name",
                UserCode = "ORIGINAL_CODE",
                Status = "Active",
                Information = "Original Info"
            };
            await _context.Users.AddAsync(existingUser);
            await _context.SaveChangesAsync();

            var updatedUser = new User
            {
                UserId = 1,
                Email = "updated@example.com",
                Name = "Updated Name",
                UserCode = "UPDATED_CODE",
                Status = "Inactive",
                Information = "Updated Info"
            };

            // Act
            var result = await _repository.UpdateUserForAdminAsync(updatedUser);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("updated@example.com", result.Email);
            Assert.AreEqual("Updated Name", result.Name);
            Assert.AreEqual("UPDATED_CODE", result.UserCode);
            Assert.AreEqual("Inactive", result.Status);
            Assert.AreEqual("Updated Info", result.Information);
        }

        [Test]
        public async Task UpdateUserForAdminAsync_ShouldNotUpdateNullFields()
        {
            // Arrange
            var existingUser = new User
            {
                UserId = 1,
                Email = "original@example.com",
                Name = "Original Name",
                RoleId = 2,
                UserCode = "ORIGINAL_CODE",
                Status = "Active",
                Information = "Original Info"
            };
            await _context.Users.AddAsync(existingUser);
            await _context.SaveChangesAsync();

            var updatedUser = new User
            {
                UserId = 1,
                Email = null, // Không cập nhật
                Name = "Updated Name", // Cập nhật
                RoleId = null,
                UserCode = null, // Không cập nhật
                Status = null // Không cập nhật
            };

            // Act
            var result = await _repository.UpdateUserForAdminAsync(updatedUser);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("original@example.com", result.Email); // Không bị thay đổi
            Assert.AreEqual("Updated Name", result.Name); // Đã cập nhật
            Assert.AreEqual(2, result.RoleId); // Không bị thay đổi
            Assert.AreEqual("ORIGINAL_CODE", result.UserCode); // Không bị thay đổi
            Assert.AreEqual("Active", result.Status); // Không bị thay đổi
        }

        // Soft Delete User

        [Test]
        public async Task SoftDeleteUserForAdminAsync_ShouldMarkUserAsDeleted()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Name = "Test User",
                Status = "Active"
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SoftDeleteUserForAdminAsync(user.UserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Deleted", result.Status);
            Assert.IsNotNull(result.DeletedAt);
        }

        [Test]
        public async Task SoftDeleteUserForAdminAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.SoftDeleteUserForAdminAsync(99));
        }

        // Add Users For Import 

        [Test]
        public async Task AddUsersForAdminAsync_ShouldAddUsersSuccessfully()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Email = "test@example.com", Name = "Test User", UserCode = "USER123", RoleId = 2 }
            };

            // Act
            await _repository.AddUsersForAdminAsync(users);

            // Assert
            Assert.AreEqual(1, _context.Users.Count());
        }

        [Test]
        public async Task IsUserCodeExistsAsync_ShouldReturnTrue_WhenUserCodeExists()
        {
            // Arrange
            var user = new User { Email = "test@example.com", UserCode = "USER123" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsUserCodeExistsAsync("USER123");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsEmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
        {
            // Arrange
            var user = new User { Email = "test@example.com", UserCode = "USER123" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsEmailExistsAsync("test@example.com");

            // Assert
            Assert.IsTrue(result);
        }

        // User Stored List

        [Test]
        public async Task GetAllUsersStoredAsync_ShouldReturnFilteredAndSortedUsers_WhenUsersExists()
        {
            // Arrange
            var doetRole = new Role { RoleId = 2, Name = "DOET" };
            var userRole = new Role { RoleId = 3, Name = "Student" };

            var users = new List<User>
            {
                new User { UserId = 1, Name = "DOET User", Email = "doet@example.com", Role = doetRole, RoleId = doetRole.RoleId, Status = "Deleted" },
                new User { UserId = 2, Name = "Student One", Email = "user1@example.com", Role = userRole, RoleId = userRole.RoleId, Status = "Deleted" },
                new User { UserId = 3, Name = "Student Two", Email = "user2@example.com", Role = userRole, RoleId = userRole.RoleId, Status = "Deleted" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUsersStoredAsync(null, null);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(3, resultList.Count);
            Assert.AreEqual("DOET User", resultList[0].Name); // Admin comes first
            Assert.AreEqual("Student One", resultList[1].Name);  // Active user next
            Assert.AreEqual("Student Two", resultList[2].Name);  // Unactive user last
        }

        [Test]
        public async Task GetAllUsersStoredAsync_ShouldApplyFiltersCorrectly_WhenUsersExists()
        {
            // Arrange
            var doetRole = new Role { RoleId = 2, Name = "DOET" };
            var userRole = new Role { RoleId = 3, Name = "Student" };

            var users = new List<User>
            {
                new User { UserId = 1, Name = "Alice Doet", Email = "doet@example.com", Role = doetRole, RoleId = doetRole.RoleId, Status = "Deleted" },
                new User { UserId = 2, Name = "Bob Student", Email = "user1@example.com", Role = userRole, RoleId = userRole.RoleId, Status = "Deleted" },
                new User { UserId = 3, Name = "Charlie Student", Email = "user2@example.com", Role = userRole, RoleId = userRole.RoleId, Status = "Deleted" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUsersStoredAsync("Alice", 2);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.AreEqual(1, resultList.Count); // Only one user matches the filters
            Assert.AreEqual("Alice Doet", resultList[0].Name);
            Assert.AreEqual("doet@example.com", resultList[0].Email);
        }

        // User Stored Detail

        [Test]
        public async Task GetUserStoredByIdForAdminAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Name = "Test User",
                Email = "test@example.com",
                Role = new Role { Name = "DOET" },
                Status = "Deleted"
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserStoredByIdForAdminAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Test User", result.Name);
            Assert.AreEqual("Deleted", result.Status);
        }

        [Test]
        public void GetUserStoredByIdForAdminAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.GetUserStoredByIdForAdminAsync(99));
        }

        // Restore User

        [Test]
        public async Task RestoreUserStoredAsync_ShouldRestoreUserSuccessfully_WhenUserExistsAndDeleted()
        {
            // Arrange
            var deletedUser = new User
            {
                UserId = 1,
                Email = "deleteduser@example.com",
                Name = "Deleted User",
                UserCode = "DELETED_CODE",
                Status = "Deleted",
                DeletedAt = DateTime.Now,
                Information = "Deleted Info"
            };

            await _context.Users.AddAsync(deletedUser);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.RestoreUserStoredAsync(deletedUser.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Active", result.Status);
            Assert.IsNull(result.DeletedAt);
            Assert.AreEqual("deleteduser@example.com", result.Email);
            Assert.AreEqual("Deleted User", result.Name);
            Assert.AreEqual("DELETED_CODE", result.UserCode);
            Assert.AreEqual("Deleted Info", result.Information);
        }

        [Test]
        public async Task RestoreUserStoredAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 99; // Non-existing user ID

            // Act & Assert
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.RestoreUserStoredAsync(userId));

            Assert.AreEqual("User not found", exception.Message);
        }


        // Hard Delete User

        [Test]
        public async Task HardDeleteUserStoredAsync_ShouldDeleteUser_WhenUserExistsAndIsDeleted()
        {
            // Arrange
            var user = new User { UserId = 1, Status = "Deleted" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.HardDeleteUserStoredAsync(user.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Deleted", result.Status); // Ensure the status was "Deleted" before removal
            var userInDb = await _context.Users.FindAsync(user.UserId);
            Assert.IsNull(userInDb); // User should no longer exist
        }

        [Test]
        public async Task HardDeleteUserStoredAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Act & Assert
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.HardDeleteUserStoredAsync(99)); // Non-existent user ID
            Assert.AreEqual("User not found in the stored user list.", exception.Message);
        }

    }
}
