using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.DocumentDTO;
using static OJTEDU.Application.DTOs.UserGuideDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class UserGuideService : IUserGuideService
    {
        private readonly IUserGuideRepository _userGuideRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public UserGuideService(IUserGuideRepository userGuideRepository, IRoleRepository roleRepository, IMapper mapper)
        {
            _userGuideRepository = userGuideRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse<PagedResponse<List<UserGuideListForAdminDTO>>>> GetAllUserGuidesForAdminAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var userGuides = await _userGuideRepository.GetAllUserGuidesForAdminAsync(title, roleId, status);

                var totalUserGuides = userGuides.Count();
                var totalPages = totalUserGuides == 0 ? 1 : (int)Math.Ceiling((double)totalUserGuides / pageSize);

                var userGuideDtos = totalUserGuides > 0 ? _mapper.Map<List<UserGuideListForAdminDTO>>(userGuides)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<UserGuideListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<UserGuideListForAdminDTO>>
                {
                    Items = userGuideDtos,
                    TotalCount = totalUserGuides,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<UserGuideListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "User Guide list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<UserGuideListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<UserGuideListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving user guide list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UserGuideDetailForAdminDTO>> GetUserGuideDetailByIdForAdminAsync(int userGuideId)
        {
            try
            {
                var userGuide = await _userGuideRepository.GetUserGuideByIdForAdminAsync(userGuideId);

                var userGuideDto = _mapper.Map<UserGuideDetailForAdminDTO>(userGuide);

                return new DataResponse<UserGuideDetailForAdminDTO>
                {
                    Data = userGuideDto,
                    Message = "User Guide details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UserGuideDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UserGuideDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving user guide details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddUserGuideForAdminDTO>> AddUserGuideForAdminAsync(AddUserGuideForAdminDTO addUserGuideForAdminDTO)
        {
            try
            {
                // Kiểm tra nếu User Guide cho Role đã tồn tại
                var userGuideExists = await _userGuideRepository.UserGuideExistsForRoleAsync(addUserGuideForAdminDTO.ForRoleId.Value);
                if (userGuideExists)
                {
                    return new DataResponse<AddUserGuideForAdminDTO>
                    {
                        Data = null,
                        Message = "User Guide for this role already exists.",
                        StatusCode = 400 // Bad Request
                    };
                }

                // Lấy tên của Role dựa trên RoleId (cần repository hoặc dịch vụ để lấy thông tin này)
                var role = await _roleRepository.GetRoleByIdAsync(addUserGuideForAdminDTO.ForRoleId.Value);
                if (string.IsNullOrEmpty(role.Name))
                {
                    return new DataResponse<AddUserGuideForAdminDTO>
                    {
                        Data = null,
                        Message = "Invalid Role ID.",
                        StatusCode = 400 // Bad Request
                    };
                }

                // Tự động đặt tên Title theo định dạng "UserGuide_For{RoleName}"
                addUserGuideForAdminDTO.Title = $"UserGuide_{role.Name}";

                // Tạo tài liệu User Guide để lưu vào cơ sở dữ liệu
                var userGuide = new UserGuide
                {
                    RoleId = addUserGuideForAdminDTO.ForRoleId,
                    Title = addUserGuideForAdminDTO.Title,
                    UserGuideFile = addUserGuideForAdminDTO.UserGuideFile
                };

                var addResult = await _userGuideRepository.AddUserGuideForAdminAsync(userGuide);

                var resultDto = _mapper.Map<AddUserGuideForAdminDTO>(addResult);

                return new DataResponse<AddUserGuideForAdminDTO>
                {
                    Data = resultDto,
                    Message = "User Guide added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddUserGuideForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding user guide: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateUserGuideForAdminDTO>> UpdateUserGuideForAdminAsync(UpdateUserGuideForAdminDTO updateUserGuideForAdminDTO)
        {
            try
            {
                // Kiểm tra nếu User Guide tồn tại trong cơ sở dữ liệu theo ID
                var existingUserGuide = await _userGuideRepository.GetUserGuideByIdForAdminAsync(updateUserGuideForAdminDTO.UserGuideId);
                if (existingUserGuide == null)
                {
                    return new DataResponse<UpdateUserGuideForAdminDTO>
                    {
                        Data = null,
                        Message = "User Guide not found.",
                        StatusCode = 404 // Not Found
                    };
                }

                // Kiểm tra nếu Role mới tồn tại nếu RoleId được cập nhật
                if (updateUserGuideForAdminDTO.ForRoleId.HasValue && updateUserGuideForAdminDTO.ForRoleId.Value != existingUserGuide.RoleId)
                {
                    // Kiểm tra xem Role mới có hợp lệ không
                    var newRole = await _roleRepository.GetRoleByIdAsync(updateUserGuideForAdminDTO.ForRoleId.Value);
                    if (newRole == null)
                    {
                        return new DataResponse<UpdateUserGuideForAdminDTO>
                        {
                            Data = null,
                            Message = "Invalid new Role ID.",
                            StatusCode = 400 // Bad Request
                        };
                    }

                    // Kiểm tra nếu User Guide cho Role mới đã tồn tại
                    var userGuideExistsForNewRole = await _userGuideRepository.UserGuideExistsForRoleAsync(updateUserGuideForAdminDTO.ForRoleId.Value);
                    if (userGuideExistsForNewRole)
                    {
                        return new DataResponse<UpdateUserGuideForAdminDTO>
                        {
                            Data = null,
                            Message = "User Guide for the new role already exists.",
                            StatusCode = 400 // Bad Request
                        };
                    }

                    // Cập nhật RoleId và Title theo định dạng mới "UserGuide_For{RoleName}"
                    existingUserGuide.RoleId = updateUserGuideForAdminDTO.ForRoleId.Value;
                    existingUserGuide.Title = $"UserGuide_{newRole.Name}";
                }

                // Kiểm tra nếu file User Guide mới được tải lên để thay thế file cũ
                if (!string.IsNullOrWhiteSpace(updateUserGuideForAdminDTO.UserGuideFile))
                {
                    existingUserGuide.UserGuideFile = updateUserGuideForAdminDTO.UserGuideFile;
                }

                // Thực hiện cập nhật trong cơ sở dữ liệu
                var updateResult = await _userGuideRepository.UpdateUserGuideForAdminAsync(existingUserGuide);

                // Mapping kết quả từ Entity sang DTO
                var resultDto = _mapper.Map<UpdateUserGuideForAdminDTO>(updateResult);

                return new DataResponse<UpdateUserGuideForAdminDTO>
                {
                    Data = resultDto,
                    Message = "User Guide updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateUserGuideForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating user guide: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateUserGuideStatusForAdminDTO>> UpdateUserGuideStatusForAdminAsync(UpdateUserGuideStatusForAdminDTO updateUserGuideStatusForAdminDTO)
        {
            try
            {
                var existingUserGuide = await _userGuideRepository.GetUserGuideByIdForAdminAsync(updateUserGuideStatusForAdminDTO.UserGuideId);
                if (existingUserGuide == null)
                {
                    return new DataResponse<UpdateUserGuideStatusForAdminDTO>
                    {
                        Data = null,
                        Message = "User Guide not found.",
                        StatusCode = 404 // Not Found
                    };
                }

                // Cập nhật trạng thái
                existingUserGuide.Status = updateUserGuideStatusForAdminDTO.Status;

                var updatedStatusResult = await _userGuideRepository.UpdateUserGuideForAdminAsync(existingUserGuide);

                var userGuideDto = _mapper.Map<UpdateUserGuideStatusForAdminDTO>(updatedStatusResult);

                return new DataResponse<UpdateUserGuideStatusForAdminDTO>
                {
                    Data = userGuideDto,
                    Message = "User guide updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateUserGuideStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateUserGuideStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating user guide: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteUserGuideForAdminDTO>> DeleteUserGuideForAdminAsync(DeleteUserGuideForAdminDTO deleteUserGuideForAdminDTO)
        {
            try
            {
                var deletedResult = await _userGuideRepository.DeleteUserGuideForAdminAsync(deleteUserGuideForAdminDTO.UserGuideId);

                var userGuideDto = _mapper.Map<DeleteUserGuideForAdminDTO>(deletedResult);

                return new DataResponse<DeleteUserGuideForAdminDTO>
                {
                    Data = userGuideDto,
                    Message = "User Guide has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteUserGuideForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteUserGuideForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting user guide: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<List<StatusUserGuideListForAdminDTO>>> GetAllStatusesUserGuideForAdminAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusUserGuideListForAdminDTO>
                {
                    new StatusUserGuideListForAdminDTO { Status = "Active" },
                    new StatusUserGuideListForAdminDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusUserGuideListForAdminDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusUserGuideListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusUserGuideListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        public async Task<DataResponse<UserGuideDetailForAdminDTO>> GetUserGuideByRoleNameAsync(string roleName)
        {
            try
            {
                var userGuide = await _userGuideRepository.GetUserGuideByRoleNameAsync(roleName);

                var userGuideDto = _mapper.Map<UserGuideDetailForAdminDTO>(userGuide);

                return new DataResponse<UserGuideDetailForAdminDTO>
                {
                    Data = userGuideDto,
                    Message = "User Guide details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UserGuideDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UserGuideDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving user guide details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
