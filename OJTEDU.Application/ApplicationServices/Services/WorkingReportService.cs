using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AppllicationDTO;
using static OJTEDU.Application.DTOs.CompanyDTO;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class WorkingReportService : IWorkingReportService
    {
        private readonly IWorkingReportRepository _workingReportRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public WorkingReportService(IWorkingReportRepository workingReportRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _workingReportRepository = workingReportRepository;
            _mapper = mapper;
        }

        // Student 
        public async Task<DataResponse<List<WorkingReportListForStudentDTO>>> GetAllByStudentIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<WorkingReportListForStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var workingReports = await _workingReportRepository.GetAllByStudentIdAsync(userId);
                var response = _mapper.Map<List<WorkingReportListForStudentDTO>>(workingReports);

                return new DataResponse<List<WorkingReportListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Working report list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<WorkingReportListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving working report list: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CreateWorkingReportForStudentDTO>> CreateWorkingReportAsync(int? userId, CreateWorkingReportForStudentDTO? workingReportInfo, string? fileName, byte[] fileData)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateWorkingReportForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                if (workingReportInfo?.ReportTitle == null)
                {
                    return new DataResponse<CreateWorkingReportForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Report title is required.",
                        Data = null
                    };
                }

                if (workingReportInfo?.ReportContent == null)
                {
                    return new DataResponse<CreateWorkingReportForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Report content is required.",
                        Data = null
                    };
                }

                var reportInfo = new WorkingReport
                {
                    ReportTitle = workingReportInfo?.ReportTitle,
                    ReportContent = workingReportInfo?.ReportContent,
                };

                var workingReport = await _workingReportRepository.CreateWorkingReportAsync(userId, reportInfo, fileName, fileData);
                var response = _mapper.Map<CreateWorkingReportForStudentDTO>(workingReport);

                return new DataResponse<CreateWorkingReportForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Create working report successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateWorkingReportForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error create working report jpb: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<UpdateWorkingReportForStudentDTO>> UpdateWorkingReportAsync(int? workingReportId, UpdateWorkingReportForStudentDTO? workingReportInfo, string? fileName, byte[] fileData)
        {
            try
            {
                if (workingReportId == null)
                {
                    return new DataResponse<UpdateWorkingReportForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found working report.",
                        Data = null
                    };
                }

                var reportInfo = new WorkingReport
                {
                    ReportContent = workingReportInfo.ReportContent,
                };

                var workingReport = await _workingReportRepository.UpdateWorkingReportAsync(workingReportId, reportInfo, fileName, fileData);
                var response = _mapper.Map<UpdateWorkingReportForStudentDTO>(workingReport);

                return new DataResponse<UpdateWorkingReportForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Update working report successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateWorkingReportForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error update working report: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<WorkingReportDetailForStudentDTO>> GetWorkingReportDetailForStudentAsync(int? workingReportId)
        {
            try
            {
                if (workingReportId == null)
                {
                    return new DataResponse<WorkingReportDetailForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found working report.",
                        Data = null
                    };
                }

                var workingReport = await _workingReportRepository.GetWorkingReportDetailAsync(workingReportId);
                var response = _mapper.Map<WorkingReportDetailForStudentDTO>(workingReport);

                return new DataResponse<WorkingReportDetailForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Working report detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<WorkingReportDetailForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieved working report detail: {ex.Message}. ",
                    Data = null
                };
            }
        }

        //For Dean
        public async Task<DataResponse<WorkingReportResponseDTO>> GetWorkingReportsByStudentIdAsync(
        int studentId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool? isDescending)
        {
            try
            {
                // Lấy thông tin userId và role từ Claims
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();

                // Lấy danh sách báo cáo làm việc từ repository
                var workingReports = await _workingReportRepository.GetWorkingReportsByStudentIdAsync(
                    studentId,
                    userId,
                    role,
                    sortBy,
                    isDescending
                );

                // Lấy thông tin sinh viên từ repository
                var student = await _workingReportRepository.GetStudentDetailsByIdAsync(studentId, userId, role);

                if (student == null)
                {
                    return new DataResponse<WorkingReportResponseDTO>
                    {
                        Data = null,
                        Message = "Student not found or access denied.",
                        StatusCode = 404
                    };
                }

                // Phân trang
                var totalReports = workingReports.Count;
                var totalPages = (int)Math.Ceiling((double)totalReports / pageSize);

                var paginatedReports = workingReports
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Ánh xạ dữ liệu sang DTO
                var workingReportDtos = _mapper.Map<List<WorkingReportDto>>(paginatedReports);

                var responseDto = new WorkingReportResponseDTO
                {
                    StudentName = student.User.Name,
                    LecturerName = student.Lecturer.Name,
                    WorkingReports = workingReportDtos
                };

                return new DataResponse<WorkingReportResponseDTO>
                {
                    Data = responseDto,
                    Message = "Working reports retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<WorkingReportResponseDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<WorkingReportResponseDTO>
                {
                    Data = null,
                    Message = $"Error retrieving working reports: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        public async Task<DataResponse<string>> UpdateWorkingReportAsync(GiveFeedbackOrScoreDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();

                var success = await _workingReportRepository.UpdateWorkingReportAsync(dto.WorkingReportId, userId, role, dto.Feedback, dto.Score);

                if (!success)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Failed to update report. Working report not found or access denied.",
                        StatusCode = 404
                    };
                }

                return new DataResponse<string>
                {
                    Data = "Report updated successfully.",
                    Message = "Report updated successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Error updating report: {ex.Message}",
                    StatusCode = 500
                };
            }
        }



        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }

            return userId;
        }

        private string GetCurrentUserRole()
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(roleClaim))
            {
                throw new UnauthorizedAccessException("User role not found.");
            }

            return roleClaim;
        }

        // Mentor 
        public async Task<DataResponse<List<WorkingReportListForMentorDTO>>> GetAllWorkingReportsByStudentId(int? studentId)
        {
            try
            {
                if (studentId == null)
                {
                    return new DataResponse<List<WorkingReportListForMentorDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var workingReport = await _workingReportRepository.GetAllWorkingReportsByStudentId(studentId);
                var response = _mapper.Map<List<WorkingReportListForMentorDTO>>(workingReport);

                return new DataResponse<List<WorkingReportListForMentorDTO>>
                {
                    StatusCode = 200,
                    Message = "Working report list for a internship retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<WorkingReportListForMentorDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieved working report list for a internship: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CreateFeedbackWorkingReportForMentorDTO>> CreateMentorFeedbackAsync(int? workingReportId, CreateFeedbackWorkingReportForMentorDTO? info)
        {
            try
            {
                if (workingReportId == null)
                {
                    return new DataResponse<CreateFeedbackWorkingReportForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found working report.",
                        Data = null
                    };
                }

                var feedback = new WorkingReport
                {
                    FeedbackFromMentor = info?.FeedbackFromMentor,
                    MentorScore = info?.MentorScore,
                };

                var workingReport = await _workingReportRepository.CreateMentorFeedbackAsync(workingReportId, feedback);
                var response = _mapper.Map<CreateFeedbackWorkingReportForMentorDTO>(workingReport);

                return new DataResponse<CreateFeedbackWorkingReportForMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Feedback for working report successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateFeedbackWorkingReportForMentorDTO>
                {
                    StatusCode = 500,
                    Message = $"Error feedback for working report: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<WorkingReportDetailForMentorDTO>> GetWorkingReportDetailForMentorAsync(int? workingReportId)
        {
            try
            {
                if (workingReportId == null)
                {
                    return new DataResponse<WorkingReportDetailForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found working report.",
                        Data = null
                    };
                }

                var workingReport = await _workingReportRepository.GetWorkingReportDetailAsync(workingReportId);
                var response = _mapper.Map<WorkingReportDetailForMentorDTO>(workingReport);

                return new DataResponse<WorkingReportDetailForMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Working report detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<WorkingReportDetailForMentorDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieved working report detail: {ex.Message}. ",
                    Data = null
                };
            }
        }
    }
}
