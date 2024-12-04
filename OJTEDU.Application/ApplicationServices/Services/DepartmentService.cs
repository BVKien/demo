using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.DepartmentDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        // Admin - Doet
        public async Task<DataResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>> GetAllDepartmentForAdminDoetAsync(string? departmentCode, string? departmentName, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var departments = await _departmentRepository.GetAllDepartmentForAdminDoetAsync(departmentCode, departmentName, status);

                var totalDepartments = departments.Count();
                var totalPages = totalDepartments == 0 ? 1 : (int)Math.Ceiling((double)totalDepartments / pageSize);

                var departmentDtos = totalDepartments > 0 ? _mapper.Map<List<DepartmentListForAdminDoetDTO>>(departments)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<DepartmentListForAdminDoetDTO>();

                var pagedResponse = new PagedResponse<List<DepartmentListForAdminDoetDTO>>
                {
                    Items = departmentDtos,
                    TotalCount = totalDepartments,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Department list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving department list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DepartmentDetailForAdminDoetDTO>> GetDepartmentDetailByIdForAdminDoetAsync(int departmentId)
        {
            try
            {
                var department = await _departmentRepository.GetDepartmentByIdAsync(departmentId);

                if (department == null)
                {
                    return new DataResponse<DepartmentDetailForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404
                    };
                }

                var departmentDto = _mapper.Map<DepartmentDetailForAdminDoetDTO>(department);

                return new DataResponse<DepartmentDetailForAdminDoetDTO>
                {
                    Data = departmentDto,
                    Message = "Company details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DepartmentDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving department details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddDepartmentForAdminDoetDTO>> AddDepartmentForAdminDoetAsync(AddDepartmentForAdminDoetDTO addDepartmentForAdminDoetDTO)
        {
            try
            {
                // Kiểm tra xem departmentCode đã tồn tại hay chưa
                var existingDepartment = await _departmentRepository.GetDepartmentByCodeAsync(addDepartmentForAdminDoetDTO.DepartmentCode);

                if (existingDepartment != null)
                {
                    return new DataResponse<AddDepartmentForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department code already exists!",
                        StatusCode = 400 // Bad Request
                    };
                }

                // Thêm mới department
                var department = _mapper.Map<Department>(addDepartmentForAdminDoetDTO);
                department.CreatedAt = GetVietnamTime();
                department.UpdatedAt = GetVietnamTime();
                department.Status = "Active";
                await _departmentRepository.AddDepartmentAsync(department);

                var addedDepartmentDto = _mapper.Map<AddDepartmentForAdminDoetDTO>(department);

                return new DataResponse<AddDepartmentForAdminDoetDTO>
                {
                    Data = addedDepartmentDto,
                    Message = "Department added successfully!",
                    StatusCode = 201 // Created
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddDepartmentForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding department: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateDepartmentForAdminDoetDTO>> UpdateDepartmentForAdminDoetAsync(UpdateDepartmentForAdminDoetDTO updateDepartmentForAdminDoetDTO)
        {
            try
            {
                // Tìm Department theo Id
                var department = await _departmentRepository.GetDepartmentByIdAsync(updateDepartmentForAdminDoetDTO.DepartmentId);

                if (department == null)
                {
                    return new DataResponse<UpdateDepartmentForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra trùng mã departmentCode với Department khác
                var existingDepartmentWithCode = await _departmentRepository.GetDepartmentByCodeAsync(updateDepartmentForAdminDoetDTO.DepartmentCode);
                if (existingDepartmentWithCode != null && existingDepartmentWithCode.DepartmentId != updateDepartmentForAdminDoetDTO.DepartmentId)
                {
                    return new DataResponse<UpdateDepartmentForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department code already exists!",
                        StatusCode = 400
                    };
                }

                // Cập nhật thông tin Department
                department.DepartmentCode = updateDepartmentForAdminDoetDTO.DepartmentCode ?? department.DepartmentCode;
                department.Name = updateDepartmentForAdminDoetDTO.Name ?? department.Name;
                department.Detail = updateDepartmentForAdminDoetDTO.Detail ?? department.Detail;
                department.UpdatedAt = GetVietnamTime();

                await _departmentRepository.UpdateDepartmentAsync(department);

                var updatedDepartmentDto = _mapper.Map<UpdateDepartmentForAdminDoetDTO>(department);

                return new DataResponse<UpdateDepartmentForAdminDoetDTO>
                {
                    Data = updatedDepartmentDto,
                    Message = "Department updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDepartmentForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating department: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateDepartmentStatusForAdminDoetDTO>> UpdateDepartmentStatusForAdminDoetAsync(UpdateDepartmentStatusForAdminDoetDTO updateDepartmentStatusForAdminDoetDTO)
        {
            try
            {
                // Tìm Department theo Id
                var department = await _departmentRepository.GetDepartmentByIdAsync(updateDepartmentStatusForAdminDoetDTO.DepartmentId);

                if (department == null)
                {
                    return new DataResponse<UpdateDepartmentStatusForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404
                    };
                }

                // Cập nhật trạng thái của Department
                department.Status = updateDepartmentStatusForAdminDoetDTO.Status ?? department.Status;
                department.UpdatedAt = GetVietnamTime();
                await _departmentRepository.UpdateDepartmentAsync(department);

                var updatedDepartmentStatusDto = _mapper.Map<UpdateDepartmentStatusForAdminDoetDTO>(department);

                return new DataResponse<UpdateDepartmentStatusForAdminDoetDTO>
                {
                    Data = updatedDepartmentStatusDto,
                    Message = "Department status updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDepartmentStatusForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating department status: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteDepartmentForAdminDoetDTO>> DeleteDepartmentForAdminDoetAsync(DeleteDepartmentForAdminDoetDTO deleteDepartmentForAdminDoetDTO)
        {
            try
            {
                var department = await _departmentRepository.GetDepartmentByIdAsync(deleteDepartmentForAdminDoetDTO.DepartmentId);

                if (department == null)
                {
                    return new DataResponse<DeleteDepartmentForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra ràng buộc
                bool hasDependencies = await _departmentRepository.CheckDepartmentDependenciesAsync(department.DepartmentId);

                if (hasDependencies)
                {
                    // Đổi trạng thái thành 'Unactive' thay vì xóa
                    department.Status = "Unactive";
                    department.UpdatedAt = GetVietnamTime();
                    await _departmentRepository.UpdateDepartmentAsync(department);

                    return new DataResponse<DeleteDepartmentForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteDepartmentForAdminDoetDTO>(department),
                        Message = "Department is in use and cannot be deleted. The status has been updated to 'Unactive' to disable it while keeping associated data intact.",
                        StatusCode = 200
                    };
                }
                else
                {
                    // Xóa nếu không có ràng buộc
                    await _departmentRepository.DeleteDepartmentAsync(department);

                    return new DataResponse<DeleteDepartmentForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteDepartmentForAdminDoetDTO>(department),
                        Message = "Department deleted successfully.",
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteDepartmentForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error deleting department: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<List<StatusDepartmentListForAdminDoetDTO>>> GetAllStatusesDepartmentForAdminDoetAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusDepartmentListForAdminDoetDTO>
                {
                    new StatusDepartmentListForAdminDoetDTO { Status = "Active" },
                    new StatusDepartmentListForAdminDoetDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusDepartmentListForAdminDoetDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusDepartmentListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusDepartmentListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Common
        public async Task<DataResponse<List<DepartmentListForCommonDTO>>> GetAllDepartmentForCommonAsync()
        {
            try
            {
                var departments = await _departmentRepository.GetAllDepartmentForCommonAsync();

                var departmentDtos = _mapper.Map<List<DepartmentListForCommonDTO>>(departments);

                return new DataResponse<List<DepartmentListForCommonDTO>>
                {
                    Data = departmentDtos,
                    Message = "Department list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<DepartmentListForCommonDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<DepartmentListForCommonDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving department list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
