using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;
using static OJTEDU.Application.DTOs.GroupChatDTO;
using static OJTEDU.Application.DTOs.InternshipDTO;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class InternshipService : IInternshipService
    {
        private readonly IInternshipRepository _internshipRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public InternshipService(IInternshipRepository internshipRepository, IMapper mapper, IUserRepository userRepository)
        {
            _internshipRepository = internshipRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // Mentor 
        public async Task<DataResponse<List<InternshipListForMentorDTO>>> GetAllInternshipsByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<InternshipListForMentorDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found mentor.",
                        Data = null
                    };
                }

                var internships = await _internshipRepository.GetAllInternshipsByUserIdAsync(userId);
                var response = _mapper.Map<List<InternshipListForMentorDTO>>(internships);

                return new DataResponse<List<InternshipListForMentorDTO>>
                {
                    StatusCode = 200,
                    Message = "Internships list for mentor retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<InternshipListForMentorDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving internship list for mentor: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<InternshipDetailForMentorDTO>> GetInternshipDetailAsync(int? internshipId)
        {
            try
            {
                if (internshipId == null)
                {
                    return new DataResponse<InternshipDetailForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found internship.",
                        Data = null
                    };
                }

                var internship = await _internshipRepository.GetInternshipDetailAsync(internshipId);
                var response = _mapper.Map<InternshipDetailForMentorDTO>(internship);

                return new DataResponse<InternshipDetailForMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Internship detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<InternshipDetailForMentorDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving internship detail: {ex.Message}. ",
                    Data = null
                };
            }
        }

        // Company 
        public async Task<DataResponse<List<InternshipListForCompanyDTO>>> GetAllInternshipsByUserIdForCompanyAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<InternshipListForCompanyDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var internships = await _internshipRepository.GetAllInternshipsByUserIdForCompanyAsync(userId);
                var response = _mapper.Map<List<InternshipListForCompanyDTO>>(internships);

                return new DataResponse<List<InternshipListForCompanyDTO>>
                {
                    StatusCode = 200,
                    Message = "Internships list for company retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<InternshipListForCompanyDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving internship list for company: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> AssignInternshipsForMentorAsync(int? userId, int? mentorId, int[]? internshipIds)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = false
                    };
                }

                if (mentorId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Mentor is required.",
                        Data = false
                    };
                }

                if (internshipIds == null || !internshipIds.Any())
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Internships is required.",
                        Data = false
                    };
                }

                var response = await _internshipRepository.AssignInternshipsForMentorAsync(userId, mentorId, internshipIds);

                if (!response)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 400,
                        Message = "Failed to assign internships to the mentor.",
                        Data = false
                    };
                }

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Internships successfully assigned to the mentor.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"Error assigning internships to mentor: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<DataResponse<CreateInternshipForCompanyDTO>> CreateInternshipAsync(int? studentId)
        {
            try
            {
                if (studentId == null)
                {
                    return new DataResponse<CreateInternshipForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var internship = await _internshipRepository.CreateInternshipAsync(studentId);
                var response = _mapper.Map<CreateInternshipForCompanyDTO>(internship);

                return new DataResponse<CreateInternshipForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "An internship created successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateInternshipForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = $"Error creating an internship.",
                    Data = null
                };
            }
        }
        //Admin DOET Dean Lecturer 
        public async Task<DataResponse<PagedResponse<List<InternshipDto>>>> GetAllInternshipsAsync(
    int userId,
    string role,
    string? searchTerm,
    DateTime? startDate,
    DateTime? endDate,
    string? statusFilter,
    string? sortBy,
    bool isDescending,
    int pageNumber,
    int pageSize)
        {
            try
            {
                // Validate startDate and endDate
                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    return new DataResponse<PagedResponse<List<InternshipDto>>>
                    {
                        Data = null,
                        Message = "Start date cannot be later than end date.",
                        StatusCode = 400
                    };
                }

                // Lấy danh sách internships từ repository
                var internships = await _internshipRepository.GetAllInternshipsAsync(
                    userId, role, searchTerm, startDate, endDate, statusFilter, sortBy, isDescending);

                if (internships == null || !internships.Any())
                {
                    return new DataResponse<PagedResponse<List<InternshipDto>>>
                    {
                        Data = null,
                        Message = "No internships found.",
                        StatusCode = 204
                    };
                }

                // Phân trang
                var totalInternships = internships.Count();
                var totalPages = (int)Math.Ceiling((double)totalInternships / pageSize);

                // Áp dụng phân trang
                var paginatedInternships = internships
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Mapping dữ liệu sang DTO
                var internshipDtos = _mapper.Map<List<InternshipDto>>(paginatedInternships);

                // Chuẩn bị response
                var pagedResponse = new PagedResponse<List<InternshipDto>>
                {
                    Items = internshipDtos,
                    TotalCount = totalInternships,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<InternshipDto>>>
                {
                    Data = pagedResponse,
                    Message = "Internships retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<InternshipDto>>>
                {
                    Data = null,
                    Message = $"Error retrieving internships: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
        public async Task<DataResponse<InternshipDetailWithReportsDTO>> GetInternshipDetailsAsync(
            int internshipId,
            string? sortBy,
            bool? isDescending,
            string? week,
            int userId,
            string role,
            int? year = null)
        {
            try
            {
                // Lấy thông tin Internship và danh sách WorkingReports từ repository
                var (internship, workingReports) = await _internshipRepository.GetInternshipDetailsWithWorkingReportsAsync(internshipId, userId, role);

                if (internship == null)
                {
                    return new DataResponse<InternshipDetailWithReportsDTO>
                    {
                        Data = null,
                        Message = "Internship not found.",
                        StatusCode = 204
                    };
                }

                // Lấy múi giờ Việt Nam
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime currentVietnamTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, vietnamTimeZone);

                // Nếu không có năm, mặc định lấy năm hiện tại
                year ??= currentVietnamTime.Year;

                // Mặc định lấy tuần hiện tại nếu không có tuần nào được chọn
                string selectedWeek = week;
                if (string.IsNullOrEmpty(week))
                {
                    DateTime currentWeekStart = currentVietnamTime.AddDays(-(int)currentVietnamTime.DayOfWeek + (int)DayOfWeek.Monday);
                    DateTime currentWeekEnd = currentWeekStart.AddDays(6);

                    selectedWeek = $"{currentWeekStart:dd/MM} to {currentWeekEnd:dd/MM}";
                }

                // Lọc WorkingReports theo tuần được chọn
                var weekDates = selectedWeek.Split(" to ");
                if (weekDates.Length == 2)
                {
                    DateTime weekStart = DateTime.ParseExact($"{weekDates[0]}/{year}", "dd/MM/yyyy", null);
                    DateTime weekEnd = DateTime.ParseExact($"{weekDates[1]}/{year}", "dd/MM/yyyy", null).AddDays(1).AddSeconds(-1);

                    workingReports = workingReports
                        .Where(w => w.CreatedAt >= weekStart && w.CreatedAt <= weekEnd)
                        .ToList();
                }
                else
                {
                    throw new ArgumentException("Invalid week format. Expected format: 'dd/MM to dd/MM'.");
                }

                // Sắp xếp danh sách WorkingReports
                switch (sortBy?.ToLower())
                {
                    case "updatedat":
                        workingReports = isDescending.GetValueOrDefault()
                            ? workingReports.OrderByDescending(w => w.UpdatedAt).ToList()
                            : workingReports.OrderBy(w => w.UpdatedAt).ToList();
                        break;
                    case "createdat":
                    default:
                        workingReports = isDescending.GetValueOrDefault()
                            ? workingReports.OrderByDescending(w => w.CreatedAt).ToList()
                            : workingReports.OrderBy(w => w.CreatedAt).ToList();
                        break;
                }

                // Mapping dữ liệu
                var internshipDto = _mapper.Map<InternshipDetailForMentorDTO>(internship);
                var workingReportsDto = _mapper.Map<List<WorkingReportDto>>(workingReports);

                // Tạo đối tượng DTO chứa thông tin Internship và WorkingReports
                var responseDto = new InternshipDetailWithReportsDTO
                {
                    Internship = internshipDto,
                    Week = selectedWeek, // Gán tuần được chọn
                    WorkingReports = workingReportsDto
                };

                return new DataResponse<InternshipDetailWithReportsDTO>
                {
                    Data = responseDto,
                    Message = "Internship details retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<InternshipDetailWithReportsDTO>
                {
                    Data = null,
                    Message = $"Error retrieving internship details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
        public async Task<DataResponse<string>> AssignLecturerForInternshipsAsync(string role, AssignLecturerForInternshipDto dto)
        {
            try
            {
                // Kiểm tra role phải là "Dean" hoặc "Lecturer"
                if (role != "Dean" && role != "Lecturer")
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Only Dean or Lecturer roles are allowed to perform this action.",
                        StatusCode = 403
                    };
                }

                // Lấy danh sách internships từ repository
                var internshipsToUpdate = await _internshipRepository.GetInternshipsByIdsAsync(dto.InternshipIds);
                if (internshipsToUpdate == null || internshipsToUpdate.Count == 0)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "No internships found with the provided IDs.",
                        StatusCode = 204
                    };
                }

                // Cập nhật LecturerId cho từng Internship
                foreach (var internship in internshipsToUpdate)
                {
                    internship.LecturerId = dto.LecturerId;
                    internship.UpdatedAt = DateTime.Now;
                }

                // Lưu thay đổi vào repository
                await _internshipRepository.UpdateInternshipsAsync(internshipsToUpdate);

                return new DataResponse<string>
                {
                    Data = "Success",
                    Message = "Internships updated successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Error: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


    }
}
