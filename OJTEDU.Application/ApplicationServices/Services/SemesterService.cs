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
using static OJTEDU.Application.DTOs.SemesterDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMapper _mapper;
        public SemesterService(ISemesterRepository semesterRepository, IMapper mapper)
        {
            _semesterRepository = semesterRepository;
            _mapper = mapper;
        }

        // Admin-Doet - Semester Management

        public async Task<DataResponse<PagedResponse<List<SemesterListForAdminDoetDTO>>>> GetAllSemesterForAdminDoetAsync(string? semesterCode, string? name, string? status, DateTime? startEventDate, DateTime? endEventDate, int pageNumber, int pageSize)
        {
            try
            {
                var semesters = await _semesterRepository.GetAllSemesterForAdminDoetAsync(semesterCode, name, status, startEventDate, endEventDate);

                var totalSemesters = semesters.Count();
                var totalPages = totalSemesters == 0 ? 1 : (int)Math.Ceiling((double)totalSemesters / pageSize);

                var semesterDtos = totalSemesters > 0 ? _mapper.Map<List<SemesterListForAdminDoetDTO>>(semesters)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<SemesterListForAdminDoetDTO>();

                var pagedResponse = new PagedResponse<List<SemesterListForAdminDoetDTO>>
                {
                    Items = semesterDtos,
                    TotalCount = totalSemesters,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<SemesterListForAdminDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Semester list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<SemesterListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<SemesterListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving semester list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<SemesterDetailForAdminDoetDTO>> GetSemesterDetailByIdForAdminDoetAsync(int semesterId)
        {
            try
            {
                var semester = await _semesterRepository.GetSemesterByIdAsync(semesterId);

                if (semester == null)
                {
                    return new DataResponse<SemesterDetailForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Semester not found!",
                        StatusCode = 404
                    };
                }

                var semesterDto = _mapper.Map<SemesterDetailForAdminDoetDTO>(semester);

                return new DataResponse<SemesterDetailForAdminDoetDTO>
                {
                    Data = semesterDto,
                    Message = "Semester details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<SemesterDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving semester details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddSemesterForAdminDoetDTO>> AddSemesterForAdminDoetAsync(AddSemesterForAdminDoetDTO addSemesterForAdminDoetDTO)
        {
            try
            {
                var existingSemester = await _semesterRepository.GetSemesterByCodeAsync(addSemesterForAdminDoetDTO.SemesterCode);

                if (existingSemester != null)
                {
                    return new DataResponse<AddSemesterForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Semester code already exists!",
                        StatusCode = 400 // Bad Request
                    };
                }

                var semester = _mapper.Map<Semester>(addSemesterForAdminDoetDTO);
                semester.CreatedAt = GetVietnamTime();
                semester.UpdatedAt = GetVietnamTime();
                semester.Status = "Active";
                await _semesterRepository.AddSemesterAsync(semester);

                var addedDto = _mapper.Map<AddSemesterForAdminDoetDTO>(semester);

                return new DataResponse<AddSemesterForAdminDoetDTO>
                {
                    Data = addedDto,
                    Message = "Semester added successfully!",
                    StatusCode = 201 // Created
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddSemesterForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding semester: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateSemesterForAdminDoetDTO>> UpdateSemesterForAdminDoetAsync(UpdateSemesterForAdminDoetDTO updateSemesterForAdminDoetDTO)
        {
            try
            {
                var semester = await _semesterRepository.GetSemesterByIdAsync(updateSemesterForAdminDoetDTO.SemesterId);

                if (semester == null)
                {
                    return new DataResponse<UpdateSemesterForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Semester not found!",
                        StatusCode = 404
                    };
                }

                var existingSemesterWithCode = await _semesterRepository.GetSemesterByCodeAsync(updateSemesterForAdminDoetDTO.SemesterCode);
                if (existingSemesterWithCode != null && existingSemesterWithCode.SemesterId != updateSemesterForAdminDoetDTO.SemesterId)
                {
                    return new DataResponse<UpdateSemesterForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Semester code already exists!",
                        StatusCode = 400
                    };
                }

                semester.SemesterCode = updateSemesterForAdminDoetDTO.SemesterCode ?? semester.SemesterCode;
                semester.Name = updateSemesterForAdminDoetDTO.Name ?? semester.Name;
                semester.StartDate = updateSemesterForAdminDoetDTO.StartDate ?? semester.StartDate;
                semester.EndDate = updateSemesterForAdminDoetDTO.EndDate ?? semester.EndDate;
                semester.Description = updateSemesterForAdminDoetDTO.Description;
                semester.UpdatedAt = GetVietnamTime();

                await _semesterRepository.UpdateSemesterAsync(semester);

                var updatedDto = _mapper.Map<UpdateSemesterForAdminDoetDTO>(semester);

                return new DataResponse<UpdateSemesterForAdminDoetDTO>
                {
                    Data = updatedDto,
                    Message = "Semester updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateSemesterForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating semester: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateSemesterStatusForAdminDoetDTO>> UpdateSemesterStatusForAdminDoetAsync(UpdateSemesterStatusForAdminDoetDTO updateSemesterStatusForAdminDoetDTO)
        {
            try
            {
                var semester = await _semesterRepository.GetSemesterByIdAsync(updateSemesterStatusForAdminDoetDTO.SemesterId);

                if (semester == null)
                {
                    return new DataResponse<UpdateSemesterStatusForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Semester not found!",
                        StatusCode = 404
                    };
                }

                semester.Status = updateSemesterStatusForAdminDoetDTO.Status ?? semester.Status;
                semester.UpdatedAt = GetVietnamTime();
                await _semesterRepository.UpdateSemesterAsync(semester);

                var updatedDto = _mapper.Map<UpdateSemesterStatusForAdminDoetDTO>(semester);

                return new DataResponse<UpdateSemesterStatusForAdminDoetDTO>
                {
                    Data = updatedDto,
                    Message = "Semester status updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateSemesterStatusForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating semester status: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteSemesterForAdminDoetDTO>> DeleteSemesterForAdminDoetAsync(DeleteSemesterForAdminDoetDTO deleteSemesterForAdminDoetDTO)
        {
            try
            {
                var semester = await _semesterRepository.GetSemesterByIdAsync(deleteSemesterForAdminDoetDTO.SemesterId);

                if (semester == null)
                {
                    return new DataResponse<DeleteSemesterForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Semester not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra ràng buộc
                bool hasDependencies = await _semesterRepository.CheckSemesterDependenciesAsync(semester.SemesterId);

                if (hasDependencies)
                {
                    // Đổi trạng thái thành 'Unactive' thay vì xóa
                    semester.Status = "Unactive";
                    semester.UpdatedAt = GetVietnamTime();
                    await _semesterRepository.UpdateSemesterAsync(semester);

                    return new DataResponse<DeleteSemesterForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteSemesterForAdminDoetDTO>(semester),
                        Message = "Semester is in use and cannot be deleted. The status has been updated to 'Unactive' to disable it while keeping associated data intact.",
                        StatusCode = 200
                    };
                }
                else
                {
                    // Xóa nếu không có ràng buộc
                    await _semesterRepository.DeleteSemesterAsync(semester);

                    return new DataResponse<DeleteSemesterForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteSemesterForAdminDoetDTO>(semester),
                        Message = "Semester deleted successfully.",
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteSemesterForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error deleting semester: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // Admin-Doet - Status List

        public async Task<DataResponse<List<StatusSemesterListForAdminDoetDTO>>> GetAllStatusesSemesterForAdminDoetAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusSemesterListForAdminDoetDTO>
                {
                    new StatusSemesterListForAdminDoetDTO { Status = "Active" },
                    new StatusSemesterListForAdminDoetDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusSemesterListForAdminDoetDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusSemesterListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusSemesterListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Common - Semester 

        public async Task<DataResponse<List<SemesterListForCommonDTO>>> GetAllSemesterForCommonAsync()
        {
            try
            {
                var semesters = await _semesterRepository.GetAllSemesterForCommonAsync();

                var semesterDtos = _mapper.Map<List<SemesterListForCommonDTO>>(semesters);

                return new DataResponse<List<SemesterListForCommonDTO>>
                {
                    Data = semesterDtos,
                    Message = "Semester list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<SemesterListForCommonDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<SemesterListForCommonDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving semester list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        private DateTime GetVietnamTime()
        {
            return DateTime.UtcNow.AddHours(7);
        }
    }
}
