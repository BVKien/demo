using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using OJTEDU.Application.Profiles;
using OJTEDU.Domain.Entities;
using OJTEDU.Infrastructure.Data;
using OJTEDU.Infrastructure.Repositories;
using static OJTEDU.Application.DTOs.RoleDTO;
using OJTEDU.Domain.Interfaces;
using Assert = Xunit.Assert;

namespace OJTEDU.Application.IntegrationTests
{
    public class RoleServiceIntegrationTests : IAsyncLifetime
    {
        private readonly IRoleService _roleService;
        private readonly OJTEDU_DB_V1Context _context;

        public RoleServiceIntegrationTests()
        {
            var services = new ServiceCollection();

            // Add DbContext with in-memory provider for isolation
            services.AddDbContext<OJTEDU_DB_V1Context>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            // Configure AutoMapper with RoleProfile
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new RoleProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Add repositories and services
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();

            var provider = services.BuildServiceProvider();

            _context = provider.GetRequiredService<OJTEDU_DB_V1Context>();
            _roleService = provider.GetRequiredService<IRoleService>();

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Add test data to the in-memory database
            _context.Roles.AddRange(new List<Role>
        {
            new Role { RoleId = 1, Name = "Admin", Description = "Administrator Role", Status = "Active" },
            new Role { RoleId = 2, Name = "User", Description = "Standard User Role", Status = "Active" }
        });
            _context.SaveChanges();
        }



        // 2. Test for Non-Existent Role Detail Retrieval
        [Fact]
        public async Task GetRoleDetailByIdForAdminAsync_ReturnsNull_WhenRoleDoesNotExist()
        {
            // Act
            var result = await _roleService.GetRoleDetailByIdForAdminAsync(999);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Role not found", result.Message);
        }

        // 3. Test Adding a Role with Duplicate Name
        [Fact]
        public async Task AddRoleForAdminAsync_ReturnsError_WhenRoleNameAlreadyExists()
        {
            // Arrange
            var newRole = new AddRoleForAdminDTO
            {
                Name = "Admin", // Existing role name
                Description = "Duplicate Role",
                Status = "Active"
            };

            // Act
            var result = await _roleService.AddRoleForAdminAsync(newRole);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("A role with the same name already exists.", result.Message);
        }

        // 4. Test Update Role with Non-Existent Role ID
        [Fact]
        public async Task UpdateRoleForAdminAsync_ReturnsError_WhenRoleDoesNotExist()
        {
            // Arrange
            var updateRole = new UpdateRoleForAdminDTO
            {
                RoleId = 999, // Non-existent RoleId
                Name = "Updated Name",
                Description = "Updated Description",
                Status = "Inactive"
            };

            // Act
            var result = await _roleService.UpdateRoleForAdminAsync(updateRole);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Role not found", result.Message);
        }

        // 5. Test Updating Role with Duplicate Name
        [Fact]
        public async Task UpdateRoleForAdminAsync_ReturnsError_WhenRoleNameAlreadyExists()
        {
            // Arrange
            var updateRole = new UpdateRoleForAdminDTO
            {
                RoleId = 2, // Existing role with RoleId = 2
                Name = "Admin", // Name that already exists in RoleId = 1
                Description = "Updated Description",
                Status = "Inactive"
            };

            // Act
            var result = await _roleService.UpdateRoleForAdminAsync(updateRole);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("A role with the same name already exists.", result.Message);
        }






        // 4. Add Role with Duplicate Description
        [Fact]
        public async Task AddRoleForAdminAsync_ShouldAllowDuplicateDescription_WhenRoleDescriptionAlreadyExists()
        {
            // Arrange
            var newRole = new AddRoleForAdminDTO
            {
                Name = "UniqueRoleName",
                Description = "Administrator Role", // Duplicate description, unique name
                Status = "Active"
            };

            // Act
            var result = await _roleService.AddRoleForAdminAsync(newRole);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(201, result.StatusCode);
        }

        // 5. Test Sorting Roles by Name
        [Fact]
        public async Task GetAllRolesForAdminAsync_ShouldReturnRolesSortedByName()
        {
            // Arrange
            _context.Roles.Add(new Role { Name = "ZRole", Description = "Test", Status = "Active" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _roleService.GetAllRolesForAdminAsync(1, 10);

            // Assert
            var roles = result.Data.Items.Select(r => r.Name).ToList();
            var sortedRoles = roles.OrderBy(r => r).ToList();
            Assert.Equal(sortedRoles, roles);
        }

        // 6. Concurrent Updates to the Same Role
        [Fact]
        public async Task UpdateRoleForAdminAsync_ShouldHandleConcurrentUpdates()
        {
            // Arrange
            var role1Update = new UpdateRoleForAdminDTO
            {
                RoleId = 1,
                Name = "ConcurrentUpdate1",
                Description = "First Update",
                Status = "Active"
            };

            var role2Update = new UpdateRoleForAdminDTO
            {
                RoleId = 1,
                Name = "ConcurrentUpdate2",
                Description = "Second Update",
                Status = "Active"
            };

            // Act
            var task1 = _roleService.UpdateRoleForAdminAsync(role1Update);
            var task2 = _roleService.UpdateRoleForAdminAsync(role2Update);
            await Task.WhenAll(task1, task2);

            // Assert
            var updatedRole = await _context.Roles.FindAsync(1);
            Assert.True(updatedRole.Name == "ConcurrentUpdate1" || updatedRole.Name == "ConcurrentUpdate2");
        }

        public async Task DisposeAsync()
        {
            // Clean up the database after each test
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        public Task InitializeAsync() => Task.CompletedTask;
    }
}