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
using static OJTEDU.Application.DTOs.JobDTO;
using static OJTEDU.Application.DTOs.MajorDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        public MajorService(IMajorRepository majorRepository, IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _majorRepository = majorRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        // Admin - DOET
        public async Task<DataResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>> GetAllMajorForAdminDoetAsync(string? majorCode, string? majorName, string? status, int? departmentId, int pageNumber, int pageSize)
        {
            try
            {
                var majors = await _majorRepository.GetAllMajorForAdminDoetAsync(majorCode, majorName, status, departmentId);

                var totalMajors = majors.Count();
                var totalPages = totalMajors == 0 ? 1 : (int)Math.Ceiling((double)totalMajors / pageSize);

                var majorDtos = totalMajors > 0 ? _mapper.Map<List<MajorListForAdminDoetDTO>>(majors)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<MajorListForAdminDoetDTO>();

                var pagedResponse = new PagedResponse<List<MajorListForAdminDoetDTO>>
                {
                    Items = majorDtos,
                    TotalCount = totalMajors,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Major list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving major list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<MajorDetailForAdminDoetDTO>> GetMajorIdDetailByIdForAdminDoetAsync(int majorId)
        {
            try
            {
                var major = await _majorRepository.GetMajorByIdAsync(majorId);

                if (major == null)
                {
                    return new DataResponse<MajorDetailForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major not found!",
                        StatusCode = 404
                    };
                }

                var majorDto = _mapper.Map<MajorDetailForAdminDoetDTO>(major);

                return new DataResponse<MajorDetailForAdminDoetDTO>
                {
                    Data = majorDto,
                    Message = "Major details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<MajorDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving major details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddMajorForAdminDoetDTO>> AddMajorForAdminDoetAsync(AddMajorForAdminDoetDTO addMajorForAdminDoetDTO)
        {
            try
            {
                // Kiểm tra department có tồn tại và đang active không
                var department = await _departmentRepository.GetDepartmentByIdAsync(addMajorForAdminDoetDTO.DepartmentId.Value);

                if (department == null)
                {
                    return new DataResponse<AddMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404 // Not Found
                    };
                }

                if (!department.Status.Equals("Active"))
                {
                    return new DataResponse<AddMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Cannot add major because the department is not active.",
                        StatusCode = 400 // Bad Request
                    };
                }

                var existingMajor = await _majorRepository.GetMajorByCodeAsync(addMajorForAdminDoetDTO.MajorCode);

                if (existingMajor != null)
                {
                    return new DataResponse<AddMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major code already exists!",
                        StatusCode = 400 // Bad Request
                    };
                }

                var major = _mapper.Map<Major>(addMajorForAdminDoetDTO);
                major.CreatedAt = GetVietnamTime();
                major.UpdatedAt = GetVietnamTime();
                major.Status = "Active";
                await _majorRepository.AddMajorAsync(major);

                var addedMajorDto = _mapper.Map<AddMajorForAdminDoetDTO>(major);

                return new DataResponse<AddMajorForAdminDoetDTO>
                {
                    Data = addedMajorDto,
                    Message = "Major added successfully!",
                    StatusCode = 201 // Created
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddMajorForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding major: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateMajorForAdminDoetDTO>> UpdateMajorForAdminDoetAsync(UpdateMajorForAdminDoetDTO updateMajorForAdminDoetDTO)
        {
            try
            {
                var major = await _majorRepository.GetMajorByIdAsync(updateMajorForAdminDoetDTO.MajorId);

                if (major == null)
                {
                    return new DataResponse<UpdateMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major not found!",
                        StatusCode = 404
                    };
                }

                if (updateMajorForAdminDoetDTO.DepartmentId.HasValue)
                {
                    var department = await _departmentRepository.GetDepartmentByIdAsync(updateMajorForAdminDoetDTO.DepartmentId.Value);

                    if (department == null)
                    {
                        return new DataResponse<UpdateMajorForAdminDoetDTO>
                        {
                            Data = null,
                            Message = "Department not found!",
                            StatusCode = 404 // Not Found
                        };
                    }

                    if (!department.Status.Equals("Active"))
                    {
                        return new DataResponse<UpdateMajorForAdminDoetDTO>
                        {
                            Data = null,
                            Message = "Cannot update major because the department is not active.",
                            StatusCode = 400 // Bad Request
                        };
                    }
                }

                var existingMajorWithCode = await _majorRepository.GetMajorByCodeAsync(updateMajorForAdminDoetDTO.MajorCode);
                if (existingMajorWithCode != null && existingMajorWithCode.MajorId != updateMajorForAdminDoetDTO.MajorId)
                {
                    return new DataResponse<UpdateMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major code already exists!",
                        StatusCode = 400
                    };
                }

                major.MajorCode = updateMajorForAdminDoetDTO.MajorCode ?? major.MajorCode;
                major.Name = updateMajorForAdminDoetDTO.Name ?? major.Name;
                major.Description = updateMajorForAdminDoetDTO.Description ?? major.Description;
                major.DepartmentId = updateMajorForAdminDoetDTO.DepartmentId ?? major.DepartmentId;
                major.UpdatedAt = GetVietnamTime();

                await _majorRepository.UpdateMajorAsync(major);

                var updatedMajorDto = _mapper.Map<UpdateMajorForAdminDoetDTO>(major);

                return new DataResponse<UpdateMajorForAdminDoetDTO>
                {
                    Data = updatedMajorDto,
                    Message = "Major updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateMajorForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating major: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateMajorStatusForAdminDoetDTO>> UpdateMajorStatusForAdminDoetAsync(UpdateMajorStatusForAdminDoetDTO updateMajorStatusForAdminDoetDTO)
        {
            try
            {
                var major = await _majorRepository.GetMajorByIdAsync(updateMajorStatusForAdminDoetDTO.MajorId);

                if (major == null)
                {
                    return new DataResponse<UpdateMajorStatusForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major not found!",
                        StatusCode = 404
                    };
                }

                major.Status = updateMajorStatusForAdminDoetDTO.Status ?? major.Status;
                major.UpdatedAt = GetVietnamTime();
                await _majorRepository.UpdateMajorAsync(major);

                var updatedMajorStatusDto = _mapper.Map<UpdateMajorStatusForAdminDoetDTO>(major);

                return new DataResponse<UpdateMajorStatusForAdminDoetDTO>
                {
                    Data = updatedMajorStatusDto,
                    Message = "Major status updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateMajorStatusForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating major status: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteMajorForAdminDoetDTO>> DeleteMajorForAdminDoetAsync(DeleteMajorForAdminDoetDTO deleteMajorForAdminDoetDTO)
        {
            try
            {
                var major = await _majorRepository.GetMajorByIdAsync(deleteMajorForAdminDoetDTO.MajorId);

                if (major == null)
                {
                    return new DataResponse<DeleteMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra ràng buộc
                bool hasDependencies = await _majorRepository.CheckMajorDependenciesAsync(major.MajorId);

                if (hasDependencies)
                {
                    // Đổi trạng thái thành 'Unactive' thay vì xóa
                    major.Status = "Unactive";
                    major.UpdatedAt = GetVietnamTime();
                    await _majorRepository.UpdateMajorAsync(major);

                    return new DataResponse<DeleteMajorForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteMajorForAdminDoetDTO>(major),
                        Message = "Major is in use and cannot be deleted. The status has been updated to 'Unactive' to disable it while keeping associated data intact.",
                        StatusCode = 200
                    };
                }
                else
                {
                    // Xóa nếu không có ràng buộc
                    await _majorRepository.DeleteMajorAsync(major);

                    return new DataResponse<DeleteMajorForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteMajorForAdminDoetDTO>(major),
                        Message = "Major deleted successfully.",
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteMajorForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error deleting major: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<List<StatusMajorListForAdminDoetDTO>>> GetAllStatusesMajorForAdminDoetAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusMajorListForAdminDoetDTO>
                {
                    new StatusMajorListForAdminDoetDTO { Status = "Active" },
                    new StatusMajorListForAdminDoetDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusMajorListForAdminDoetDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusMajorListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusMajorListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Common

        public async Task<DataResponse<List<MajorListForCommonDTO>>> GetAllMajorForCommonAsync()
        {
            try
            {
                var majors = await _majorRepository.GetAllMajorForCommonAsync();

                var majorsDtos = _mapper.Map<List<MajorListForCommonDTO>>(majors);

                return new DataResponse<List<MajorListForCommonDTO>>
                {
                    Data = majorsDtos,
                    Message = "Major list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<MajorListForCommonDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<MajorListForCommonDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving major list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // Student 
        public async Task<DataResponse<List<MajorListForStudentDTO>>> GetAllMajorsAsync()
        {
            try
            {
                var majors = await _majorRepository.GetAllMajorsAsync();
                var response = _mapper.Map<List<MajorListForStudentDTO>>(majors);

                return new DataResponse<List<MajorListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Major list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<MajorListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving major list {ex.Message}. ",
                    Data = null
                };
            }
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
