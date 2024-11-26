using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.RoleDTO;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        // Admin
        public async Task<DataResponse<List<RoleListForAdminDTO>>> GetAllRolesToAddUpdateForAdminAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesToAddUpdateForAdminAsync();

                var roleDtos = _mapper.Map<List<RoleListForAdminDTO>>(roles);

                return new DataResponse<List<RoleListForAdminDTO>>
                {
                    Data = roleDtos,
                    Message = "Role list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<RoleListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<RoleListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving role list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<PagedResponse<List<RoleListForAdminDTO>>>> GetAllRolesForAdminAsync(int pageNumber, int pageSize)
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesForAdminAsync();

                var totalRoles = roles.Count();
                var totalPages = totalRoles == 0 ? 1 : (int)Math.Ceiling((double)totalRoles / pageSize);

                var roleDtos = totalRoles > 0 ? _mapper.Map<List<RoleListForAdminDTO>>(roles)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<RoleListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<RoleListForAdminDTO>>
                {
                    Items = roleDtos,
                    TotalCount = totalRoles,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Role list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Access denied while get role list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving role list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<RoleDetailForAdminDTO>> GetRoleDetailByIdForAdminAsync(int roleId)
        {
            try
            {
                var role = await _roleRepository.GetRoleByIdAsync(roleId);

                var roleDto = _mapper.Map<RoleDetailForAdminDTO>(role);

                return new DataResponse<RoleDetailForAdminDTO>
                {
                    Data = roleDto,
                    Message = "Role details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<RoleDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<RoleDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while get role detail: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<RoleDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving role details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddRoleForAdminDTO>> AddRoleForAdminAsync(AddRoleForAdminDTO addRoleForAdminDTO)
        {
            try
            {
                var role = new Role
                {
                    Name = addRoleForAdminDTO.Name,
                    Description = addRoleForAdminDTO.Description
                };

                var addRoleResult = await _roleRepository.AddRoleAsync(role);

                // Cập nhật thời gian tạo vào DTO trả về
                addRoleForAdminDTO.Status = addRoleResult.Status;
                addRoleForAdminDTO.CreatedAt = addRoleResult.CreatedAt;

                return new DataResponse<AddRoleForAdminDTO>
                {
                    Data = addRoleForAdminDTO,
                    Message = "Role added successfully!",
                    StatusCode = 201
                };
            }
            catch (InvalidOperationException ex)
            {
                return new DataResponse<AddRoleForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<AddRoleForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while add user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddRoleForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateRoleForAdminDTO>> UpdateRoleForAdminAsync(UpdateRoleForAdminDTO updateRoleForAdminDTO)
        {
            try
            {
                var role = new Role
                {
                    RoleId = updateRoleForAdminDTO.RoleId,
                    Name = updateRoleForAdminDTO.Name,
                    Description = updateRoleForAdminDTO.Description,
                    Status = updateRoleForAdminDTO.Status
                };

                var updatedRoleResult = await _roleRepository.UpdateRoleAsync(role);

                var roleDto = _mapper.Map<UpdateRoleForAdminDTO>(updatedRoleResult);

                return new DataResponse<UpdateRoleForAdminDTO>
                {
                    Data = roleDto,
                    Message = "Role updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp role ko tồn tại
                return new DataResponse<UpdateRoleForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                return new DataResponse<UpdateRoleForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateRoleForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while update user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateRoleForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteRoleForAdminDTO>> DeleteRoleForAdminAsync(DeleteRoleForAdminDTO deleteRoleForAdminDTO)
        {
            try
            {
                // Kiểm tra ràng buộc
                bool hasDependencies = await _roleRepository.CheckRoleDependenciesAsync(deleteRoleForAdminDTO.RoleId);

                if (hasDependencies)
                {
                    return new DataResponse<DeleteRoleForAdminDTO>
                    {
                        Data = null,
                        Message = "Role is in use and cannot be deleted.",
                        StatusCode = 200
                    };
                }
                else
                {
                    // Xóa nếu không có ràng buộc
                    var deletedRoleResult = await _roleRepository.DeleteRoleAsync(deleteRoleForAdminDTO.RoleId);

                    return new DataResponse<DeleteRoleForAdminDTO>
                    {
                        Data = _mapper.Map<DeleteRoleForAdminDTO>(deletedRoleResult),
                        Message = "Role has been permanently deleted successfully.",
                        StatusCode = 200
                    };
                }
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<DeleteRoleForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteRoleForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting role: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        // Doet

        public async Task<DataResponse<PagedResponse<List<RoleListForAdminDTO>>>> GetAllRolesForDoetAsync(int pageNumber, int pageSize)
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesForDoetAsync();

                var totalRoles = roles.Count();
                var totalPages = totalRoles == 0 ? 1 : (int)Math.Ceiling((double)totalRoles / pageSize);

                var roleDtos = totalRoles > 0 ? _mapper.Map<List<RoleListForAdminDTO>>(roles)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<RoleListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<RoleListForAdminDTO>>
                {
                    Items = roleDtos,
                    TotalCount = totalRoles,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Role list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Access denied while get role list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving role list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<List<RoleListForAdminDTO>>> GetAllRolesToAddUpdateForDoetAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesToAddUpdateForDoetAsync();

                var roleDtos = _mapper.Map<List<RoleListForAdminDTO>>(roles);

                return new DataResponse<List<RoleListForAdminDTO>>
                {
                    Data = roleDtos,
                    Message = "Role list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<RoleListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<RoleListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving role list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // Company

        public async Task<DataResponse<PagedResponse<List<RoleListForAdminDTO>>>> GetAllRolesForCompanyAsync(int pageNumber, int pageSize)
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesForCompanyAsync();

                var totalRoles = roles.Count();
                var totalPages = totalRoles == 0 ? 1 : (int)Math.Ceiling((double)totalRoles / pageSize);

                var roleDtos = totalRoles > 0 ? _mapper.Map<List<RoleListForAdminDTO>>(roles)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<RoleListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<RoleListForAdminDTO>>
                {
                    Items = roleDtos,
                    TotalCount = totalRoles,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Role list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Access denied while get role list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<RoleListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving role list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
