
//using Moq;
//using NUnit.Framework;
//using OJTEDU.Application.ApplicationServices.Services;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
//using AutoMapper;
//using OJTEDU.Domain.Entities;
//using OJTEDU.Domain.Interfaces;
//using static OJTEDU.Application.DTOs.RoleDTO;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    public class RoleServiceTests
//    {
//        private Mock<IRoleRepository> _roleRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private RoleService _roleService;

//        [SetUp]
//        public void Setup()
//        {
//            _roleRepositoryMock = new Mock<IRoleRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _roleService = new RoleService(_roleRepositoryMock.Object, _mapperMock.Object);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ReturnsPagedRoleList_WhenRolesExist()
//        {
//            // Arrange
//            var roles = new List<Role>
//            {
//                new Role { RoleId = 1, Name = "Admin", Description = "Administrator Role", Status = "Active" },
//                new Role { RoleId = 2, Name = "User", Description = "User Role", Status = "Active" }
//            };
//            var roleDtos = new List<RoleListForAdminDTO>
//            {
//                new RoleListForAdminDTO { RoleId = 1, Name = "Admin", Description = "Administrator Role", Status = "Active" },
//                new RoleListForAdminDTO { RoleId = 2, Name = "User", Description = "User Role", Status = "Active" }
//            };

//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).ReturnsAsync(roles);
//            _mapperMock.Setup(m => m.Map<List<RoleListForAdminDTO>>(roles)).Returns(roleDtos);

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 2);

//            // Assert
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual("Role list retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ReturnsEmptyPagedResponse_WhenNoRolesExist()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).ReturnsAsync(new List<Role>());
//            _mapperMock.Setup(m => m.Map<List<RoleListForAdminDTO>>(It.IsAny<List<Role>>())).Returns(new List<RoleListForAdminDTO>());

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(0, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//            Assert.AreEqual(10, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ThrowsNotFoundException_WhenRolesAreNull()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).Throws(new KeyNotFoundException("Roles not found"));

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 2);

//            // Assert
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Roles not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ReturnsForbidden_WhenUnauthorizedAccess()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).Throws(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 2);

//            // Assert
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get role list: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ReturnsServerError_WhenUnhandledExceptionOccurs()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).Throws(new Exception("Unexpected error"));

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 2);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving role list: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ShouldReturnPagedResponse_WhenRolesExistAndMoreThanPageSize()
//        {
//            // Arrange
//            var roles = Enumerable.Range(1, 5).Select(i => new Role { RoleId = i, Name = $"Role {i}", Description = $"Description {i}", Status = "Active" }).ToList();
//            var roleDtos = roles.Select(r => new RoleListForAdminDTO { RoleId = r.RoleId, Name = r.Name, Description = r.Description, Status = r.Status }).ToList();

//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).ReturnsAsync(roles);
//            _mapperMock.Setup(m => m.Map<List<RoleListForAdminDTO>>(roles)).Returns(roleDtos);

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 3);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(5, result.Data.TotalCount);
//            Assert.AreEqual(2, result.Data.TotalPages);
//            Assert.AreEqual(3, result.Data.Items.Count); // Should only return 3 items due to page size
//            Assert.AreEqual(3, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ShouldReturnPagedResponse_WhenRolesExistButLessThanPageSize()
//        {
//            // Arrange
//            var roles = Enumerable.Range(1, 2).Select(i => new Role { RoleId = i, Name = $"Role {i}", Description = $"Description {i}", Status = "Active" }).ToList();
//            var roleDtos = roles.Select(r => new RoleListForAdminDTO { RoleId = r.RoleId, Name = r.Name, Description = r.Description, Status = r.Status }).ToList();

//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).ReturnsAsync(roles);
//            _mapperMock.Setup(m => m.Map<List<RoleListForAdminDTO>>(roles)).Returns(roleDtos);

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(2, result.Data.Items.Count); // Should return all items since count < page size
//            Assert.AreEqual(5, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ShouldReturnPagedResponse_WhenRolesExistAndExactlyPageSize()
//        {
//            // Arrange
//            var roles = Enumerable.Range(1, 3).Select(i => new Role { RoleId = i, Name = $"Role {i}", Description = $"Description {i}", Status = "Active" }).ToList();
//            var roleDtos = roles.Select(r => new RoleListForAdminDTO { RoleId = r.RoleId, Name = r.Name, Description = r.Description, Status = r.Status }).ToList();

//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).ReturnsAsync(roles);
//            _mapperMock.Setup(m => m.Map<List<RoleListForAdminDTO>>(roles)).Returns(roleDtos);

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 3);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(3, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(3, result.Data.Items.Count); // Should return exactly page size items
//            Assert.AreEqual(3, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ShouldReturnNotFound_WhenKeyNotFoundExceptionIsThrown()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).Throws(new KeyNotFoundException("Roles not found"));

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Roles not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllRolesForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForAdminAsync()).Throws(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _roleService.GetAllRolesForAdminAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get role list: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        //Doet
        

//        [Test]
//        public async Task GetAllRolesForDoetAsync_ShouldReturnNotFound_WhenKeyNotFoundExceptionIsThrown()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForDoetAsync()).Throws(new KeyNotFoundException("Roles not found"));

//            // Act
//            var result = await _roleService.GetAllRolesForDoetAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Roles not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllRolesForDoetAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForDoetAsync()).Throws(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _roleService.GetAllRolesForDoetAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get role list: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllRolesForDoetAsync_ShouldReturnServerError_WhenUnhandledExceptionIsThrown()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForDoetAsync()).Throws(new Exception("Unexpected error"));

//            // Act
//            var result = await _roleService.GetAllRolesForDoetAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving role list: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        //Company

//        [Test]
//        public async Task GetAllRolesForCompanyAsync_ShouldReturnPagedResponse_WhenRolesExistAndMoreThanPageSize()
//        {
//            // Arrange
//            var roles = Enumerable.Range(1, 5).Select(i => new Role { RoleId = i, Name = "Company", Description = $"Description {i}", Status = "Active" }).ToList();
//            var roleDtos = roles.Select(r => new RoleListForAdminDTO { RoleId = r.RoleId, Name = r.Name, Description = r.Description, Status = r.Status }).ToList();

//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForCompanyAsync()).ReturnsAsync(roles);
//            _mapperMock.Setup(m => m.Map<List<RoleListForAdminDTO>>(roles)).Returns(roleDtos);

//            // Act
//            var result = await _roleService.GetAllRolesForCompanyAsync(pageNumber: 1, pageSize: 3);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(5, result.Data.TotalCount);
//            Assert.AreEqual(2, result.Data.TotalPages);
//            Assert.AreEqual(3, result.Data.Items.Count); // Should only return 3 items due to page size
//            Assert.AreEqual(3, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllRolesForCompanyAsync_ShouldReturnPagedResponse_WhenRolesExistButLessThanPageSize()
//        {
//            // Arrange
//            var roles = new List<Role>
//            {
//                new Role { RoleId = 1, Name = "Company", Description = "Description 1", Status = "Active" },
//                new Role { RoleId = 2, Name = "Mentor", Description = "Description 2", Status = "Active" }
//            };
//            var roleDtos = roles.Select(r => new RoleListForAdminDTO { RoleId = r.RoleId, Name = r.Name, Description = r.Description, Status = r.Status }).ToList();

//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForCompanyAsync()).ReturnsAsync(roles);
//            _mapperMock.Setup(m => m.Map<List<RoleListForAdminDTO>>(roles)).Returns(roleDtos);

//            // Act
//            var result = await _roleService.GetAllRolesForCompanyAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(2, result.Data.Items.Count); // All items should be returned since count < page size
//            Assert.AreEqual(5, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllRolesForCompanyAsync_ShouldReturnPagedResponse_WhenRolesExistAndExactlyPageSize()
//        {
//            // Arrange
//            var roles = Enumerable.Range(1, 3).Select(i => new Role { RoleId = i, Name = "Company", Description = $"Description {i}", Status = "Active" }).ToList();
//            var roleDtos = roles.Select(r => new RoleListForAdminDTO { RoleId = r.RoleId, Name = r.Name, Description = r.Description, Status = r.Status }).ToList();

//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForCompanyAsync()).ReturnsAsync(roles);
//            _mapperMock.Setup(m => m.Map<List<RoleListForAdminDTO>>(roles)).Returns(roleDtos);

//            // Act
//            var result = await _roleService.GetAllRolesForCompanyAsync(pageNumber: 1, pageSize: 3);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(3, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(3, result.Data.Items.Count); // Should return exactly page size items
//            Assert.AreEqual(3, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllRolesForCompanyAsync_ShouldReturnNotFound_WhenKeyNotFoundExceptionIsThrown()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForCompanyAsync()).Throws(new KeyNotFoundException("Roles not found"));

//            // Act
//            var result = await _roleService.GetAllRolesForCompanyAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Roles not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllRolesForCompanyAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForCompanyAsync()).Throws(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _roleService.GetAllRolesForCompanyAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get role list: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllRolesForCompanyAsync_ShouldReturnServerError_WhenUnhandledExceptionIsThrown()
//        {
//            // Arrange
//            _roleRepositoryMock.Setup(repo => repo.GetAllRolesForCompanyAsync()).Throws(new Exception("Unexpected error"));

//            // Act
//            var result = await _roleService.GetAllRolesForCompanyAsync(pageNumber: 1, pageSize: 5);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving role list: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        //GetRoleDetail
//        [Test]
//        public async Task GetRoleDetailByIdForAdminAsync_ShouldReturnRoleDetail_WhenRoleExists()
//        {
//            // Arrange
//            int roleId = 1;
//            var role = new Role { RoleId = roleId, Name = "Admin", Description = "Admin Role", Status = "Active", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
//            var roleDto = new RoleDetailForAdminDTO { RoleId = roleId, Name = "Admin", Description = "Admin Role", Status = "Active", CreatedAt = role.CreatedAt, UpdatedAt = role.UpdatedAt };

//            _roleRepositoryMock.Setup(repo => repo.GetRoleByIdAsync(roleId)).ReturnsAsync(role);
//            _mapperMock.Setup(m => m.Map<RoleDetailForAdminDTO>(role)).Returns(roleDto);

//            // Act
//            var result = await _roleService.GetRoleDetailByIdForAdminAsync(roleId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(roleDto, result.Data);
//            Assert.AreEqual("Role details retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task GetRoleDetailByIdForAdminAsync_ShouldReturnNotFound_WhenRoleDoesNotExist()
//        {
//            // Arrange
//            int roleId = 99;

//            _roleRepositoryMock.Setup(repo => repo.GetRoleByIdAsync(roleId)).Throws(new KeyNotFoundException("Role not found"));

//            // Act
//            var result = await _roleService.GetRoleDetailByIdForAdminAsync(roleId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Role not found", result.Message);
//        }

//        [Test]
//        public async Task GetRoleDetailByIdForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            int roleId = 1;

//            _roleRepositoryMock.Setup(repo => repo.GetRoleByIdAsync(roleId)).Throws(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _roleService.GetRoleDetailByIdForAdminAsync(roleId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Access denied while get role detail: Access denied", result.Message);
//        }

//        [Test]
//        public async Task GetRoleDetailByIdForAdminAsync_ShouldReturnServerError_WhenUnhandledExceptionIsThrown()
//        {
//            // Arrange
//            int roleId = 1;

//            _roleRepositoryMock.Setup(repo => repo.GetRoleByIdAsync(roleId)).Throws(new Exception("Unexpected error"));

//            // Act
//            var result = await _roleService.GetRoleDetailByIdForAdminAsync(roleId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Error retrieving role details: Unexpected error", result.Message);
//        }

//        //AddRole
//        [Test]
//        public async Task AddRoleForAdminAsync_ShouldReturnCreatedResponse_WhenRoleIsAddedSuccessfully()
//        {
//            // Arrange
//            var addRoleDto = new AddRoleForAdminDTO
//            {
//                Name = "New Role",
//                Description = "New Role Description"
//            };
//            var createdRole = new Role
//            {
//                RoleId = 1,
//                Name = addRoleDto.Name,
//                Description = addRoleDto.Description,
//                CreatedAt = DateTime.Now,
//                Status = "Active"
//            };

//            _roleRepositoryMock.Setup(repo => repo.AddRoleAsync(It.IsAny<Role>())).ReturnsAsync(createdRole);

//            // Act
//            var result = await _roleService.AddRoleForAdminAsync(addRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Role added successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(createdRole.CreatedAt, result.Data.CreatedAt);
//            Assert.AreEqual(createdRole.Status, result.Data.Status);
//        }

//        [Test]
//        public async Task AddRoleForAdminAsync_ShouldReturnBadRequest_WhenRoleNameAlreadyExists()
//        {
//            // Arrange
//            var addRoleDto = new AddRoleForAdminDTO
//            {
//                Name = "Existing Role",
//                Description = "Description for Existing Role"
//            };

//            _roleRepositoryMock.Setup(repo => repo.AddRoleAsync(It.IsAny<Role>()))
//                .ThrowsAsync(new InvalidOperationException("A role with the same name already exists."));

//            // Act
//            var result = await _roleService.AddRoleForAdminAsync(addRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("A role with the same name already exists.", result.Message);
//        }

//        [Test]
//        public async Task AddRoleForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            var addRoleDto = new AddRoleForAdminDTO
//            {
//                Name = "New Role",
//                Description = "New Role Description"
//            };

//            _roleRepositoryMock.Setup(repo => repo.AddRoleAsync(It.IsAny<Role>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _roleService.AddRoleForAdminAsync(addRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Access denied while add user: Access denied", result.Message);
//        }

//        [Test]
//        public async Task AddRoleForAdminAsync_ShouldReturnServerError_WhenUnhandledExceptionIsThrown()
//        {
//            // Arrange
//            var addRoleDto = new AddRoleForAdminDTO
//            {
//                Name = "New Role",
//                Description = "New Role Description"
//            };

//            _roleRepositoryMock.Setup(repo => repo.AddRoleAsync(It.IsAny<Role>()))
//                .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _roleService.AddRoleForAdminAsync(addRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Error adding user: Unexpected error", result.Message);
//        }

//        //UpdateRole
//        [Test]
//        public async Task UpdateRoleForAdminAsync_ShouldReturnUpdatedRole_WhenRoleIsUpdatedSuccessfully()
//        {
//            // Arrange
//            var updateRoleDto = new UpdateRoleForAdminDTO
//            {
//                RoleId = 1,
//                Name = "Updated Role",
//                Description = "Updated Description",
//                Status = "Active",
//                UpdatedAt = DateTime.Now
//            };
//            var updatedRole = new Role
//            {
//                RoleId = updateRoleDto.RoleId,
//                Name = updateRoleDto.Name,
//                Description = updateRoleDto.Description,
//                Status = updateRoleDto.Status,
//                UpdatedAt = updateRoleDto.UpdatedAt
//            };

//            _roleRepositoryMock.Setup(repo => repo.UpdateRoleAsync(It.IsAny<Role>())).ReturnsAsync(updatedRole);
//            _mapperMock.Setup(m => m.Map<UpdateRoleForAdminDTO>(updatedRole)).Returns(updateRoleDto);

//            // Act
//            var result = await _roleService.UpdateRoleForAdminAsync(updateRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Role updated successfully!", result.Message);
//            Assert.AreEqual(updateRoleDto, result.Data);
//        }

//        [Test]
//        public async Task UpdateRoleForAdminAsync_ShouldReturnNotFound_WhenRoleDoesNotExist()
//        {
//            // Arrange
//            var updateRoleDto = new UpdateRoleForAdminDTO
//            {
//                RoleId = 99, // non-existent roleId
//                Name = "Nonexistent Role",
//                Description = "Nonexistent Description",
//                Status = "Inactive"
//            };

//            _roleRepositoryMock.Setup(repo => repo.UpdateRoleAsync(It.IsAny<Role>()))
//                .ThrowsAsync(new KeyNotFoundException("Role not found"));

//            // Act
//            var result = await _roleService.UpdateRoleForAdminAsync(updateRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Role not found", result.Message);
//        }

//        [Test]
//        public async Task UpdateRoleForAdminAsync_ShouldReturnBadRequest_WhenRoleNameAlreadyExists()
//        {
//            // Arrange
//            var updateRoleDto = new UpdateRoleForAdminDTO
//            {
//                RoleId = 1,
//                Name = "Duplicate Name",
//                Description = "Description",
//                Status = "Active"
//            };

//            _roleRepositoryMock.Setup(repo => repo.UpdateRoleAsync(It.IsAny<Role>()))
//                .ThrowsAsync(new InvalidOperationException("A role with the same name already exists."));

//            // Act
//            var result = await _roleService.UpdateRoleForAdminAsync(updateRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(400, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("A role with the same name already exists.", result.Message);
//        }

//        [Test]
//        public async Task UpdateRoleForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            var updateRoleDto = new UpdateRoleForAdminDTO
//            {
//                RoleId = 1,
//                Name = "Test Role",
//                Description = "Test Description",
//                Status = "Active"
//            };

//            _roleRepositoryMock.Setup(repo => repo.UpdateRoleAsync(It.IsAny<Role>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _roleService.UpdateRoleForAdminAsync(updateRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Access denied while update user: Access denied", result.Message);
//        }

//        [Test]
//        public async Task UpdateRoleForAdminAsync_ShouldReturnServerError_WhenUnhandledExceptionIsThrown()
//        {
//            // Arrange
//            var updateRoleDto = new UpdateRoleForAdminDTO
//            {
//                RoleId = 1,
//                Name = "Test Role",
//                Description = "Test Description",
//                Status = "Active"
//            };

//            _roleRepositoryMock.Setup(repo => repo.UpdateRoleAsync(It.IsAny<Role>()))
//                .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _roleService.UpdateRoleForAdminAsync(updateRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Error updating user: Unexpected error", result.Message);
//        }

//        //DeleteRole
//        [Test]
//        public async Task DeleteRoleForAdminAsync_ShouldReturnDeletedRole_WhenRoleIsDeletedSuccessfully()
//        {
//            // Arrange
//            var deleteRoleDto = new DeleteRoleForAdminDTO { RoleId = 1, Name = "Role to Delete", Description = "Description", Status = "Inactive" };
//            var deletedRole = new Role { RoleId = deleteRoleDto.RoleId, Name = deleteRoleDto.Name, Description = deleteRoleDto.Description, Status = deleteRoleDto.Status };

//            _roleRepositoryMock.Setup(repo => repo.DeleteRoleAsync(deleteRoleDto.RoleId)).ReturnsAsync(deletedRole);
//            _mapperMock.Setup(m => m.Map<DeleteRoleForAdminDTO>(deletedRole)).Returns(deleteRoleDto);

//            // Act
//            var result = await _roleService.DeleteRoleForAdminAsync(deleteRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Role has been permanently deleted successfully.", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(deleteRoleDto, result.Data);
//        }

//        [Test]
//        public async Task DeleteRoleForAdminAsync_ShouldReturnNotFound_WhenRoleDoesNotExist()
//        {
//            // Arrange
//            var deleteRoleDto = new DeleteRoleForAdminDTO { RoleId = 99 }; // Non-existent Role ID

//            _roleRepositoryMock.Setup(repo => repo.DeleteRoleAsync(deleteRoleDto.RoleId))
//                .ThrowsAsync(new KeyNotFoundException("Role not found in the role list."));

//            // Act
//            var result = await _roleService.DeleteRoleForAdminAsync(deleteRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Role not found in the role list.", result.Message);
//        }

//        [Test]
//        public async Task DeleteRoleForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            var deleteRoleDto = new DeleteRoleForAdminDTO { RoleId = 1 };

//            _roleRepositoryMock.Setup(repo => repo.DeleteRoleAsync(deleteRoleDto.RoleId))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _roleService.DeleteRoleForAdminAsync(deleteRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Access denied while hard delete role: Access denied", result.Message);
//        }

//        [Test]
//        public async Task DeleteRoleForAdminAsync_ShouldReturnServerError_WhenUnhandledExceptionIsThrown()
//        {
//            // Arrange
//            var deleteRoleDto = new DeleteRoleForAdminDTO { RoleId = 1 };

//            _roleRepositoryMock.Setup(repo => repo.DeleteRoleAsync(deleteRoleDto.RoleId))
//                .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _roleService.DeleteRoleForAdminAsync(deleteRoleDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsNull(result.Data);
//            Assert.AreEqual("Error permanently deleting role: Unexpected error", result.Message);
//        }
//    }
//}
