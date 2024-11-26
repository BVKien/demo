using AutoMapper;
using Microsoft.AspNetCore.Http;
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
using static OJTEDU.Application.DTOs.AttendanceReportDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class AttendanceReportService : IAttendanceReportService
    {
        private readonly IAttendanceReportRepository _attendRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public AttendanceReportService(IAttendanceReportRepository attendReportRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _attendRepository = attendReportRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse<PagedResponse<List<AttendanceReportDto>>>> GetAttendanceReportsByStudentIdAsync(
         int studentId, int pageNumber, int pageSize)
        {
            try
            {
                var attendanceReports = await _attendRepository.GetAttendanceReportsByStudentIdAsync(studentId);

                var totalReports = attendanceReports.Count();
                var totalPages = (int)Math.Ceiling((double)totalReports / pageSize);

                var reportDtos = _mapper.Map<List<AttendanceReportDto>>(attendanceReports)
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                var pagedResponse = new PagedResponse<List<AttendanceReportDto>>
                {
                    Items = reportDtos,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<AttendanceReportDto>>>
                {
                    Data = pagedResponse,
                    Message = "Attendance reports retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<AttendanceReportDto>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<AttendanceReportDto>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        // Mentor 
        public async Task<DataResponse<SetCheckInCheckOutTimeForMentorDTO>> SetCheckInCheckOutTimeAsync(int? userId, SetCheckInCheckOutTimeForMentorDTO? info)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<SetCheckInCheckOutTimeForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found mentor.",
                        Data = null
                    };
                }

                if (info?.CheckInTime == null)
                {
                    return new DataResponse<SetCheckInCheckOutTimeForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Check in time is required.",
                        Data = null
                    };
                }

                if (info?.CheckOutTime == null)
                {
                    return new DataResponse<SetCheckInCheckOutTimeForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Check out time is required.",
                        Data = null
                    };
                }

                var setTimeInfo = new Company
                {
                    CheckInTime = info?.CheckInTime,
                    CheckOutTime = info?.CheckOutTime
                };

                var setTime = await _attendRepository.SetCheckInCheckOutTimeAsync(userId, setTimeInfo);
                var response = _mapper.Map<SetCheckInCheckOutTimeForMentorDTO>(setTime);

                return new DataResponse<SetCheckInCheckOutTimeForMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Set check in and check out time successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<SetCheckInCheckOutTimeForMentorDTO>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> CreateAutoAttendanceReportAsync(int? userId, TimeSpan? checkInTime, TimeSpan? checkOutTime)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found mentor.",
                        Data = false
                    };
                }

                if (checkInTime == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Check in time is required.",
                        Data = false
                    };
                }

                if (checkOutTime == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Check out time is required.",
                        Data = false
                    };
                }

                var response = await _attendRepository.CreateAutoAttendanceReportAsync(userId, checkInTime, checkOutTime);

                if (!response)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 400,
                        Message = "Failed to auto create attendance report.",
                        Data = false
                    };
                }

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Create auto attendance report successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = false
                };
            }
        }

        public async Task<DataResponse<UpdateAttendanceReportForMentorDTO>> UpdateAttendanceReportAsync(int? attendanceReportId, UpdateAttendanceReportForMentorDTO? info)
        {
            try
            {
                if (attendanceReportId == null)
                {
                    return new DataResponse<UpdateAttendanceReportForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found attendance report.",
                        Data = null
                    };
                }

                var arInfo = new AttendanceReport
                {
                    CheckInTime = info?.CheckInTime,
                    CheckOutTime = info?.CheckOutTime,
                    Reason = info?.Reason,
                    Status = info?.Status,
                    EarlyLeave = info?.EarlyLeave,
                    Late = info?.Late,
                };

                var ar = await _attendRepository.UpdateAttendanceReportAsync(attendanceReportId, arInfo);
                var response = _mapper.Map<UpdateAttendanceReportForMentorDTO>(ar);

                return new DataResponse<UpdateAttendanceReportForMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Update attendance report successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateAttendanceReportForMentorDTO>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> InsertAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found mentor.",
                        Data = false
                    };
                }

                var arList = await _attendRepository.InsertAttendanceReportsFromExcelAsync(userId, fileName, fileData);
                var response = _mapper.Map<bool>(arList);

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Attendance reports list inserted from attendace file successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = false
                };
            }
        }

        public async Task<DataResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>> ListAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found mentor.",
                        Data = null
                    };
                }

                var arList = await _attendRepository.ListAttendanceReportsFromExcelAsync(userId, fileName, fileData);
                var response = _mapper.Map<List<AttendanceReportListFromCsvFileForMentorDTO>>(arList);

                return new DataResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>
                {
                    StatusCode = 200,
                    Message = "Attendance reports list from csv file retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>> GetAllAttendanceReportsForMentorAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found intern.",
                        Data = null
                    };
                }

                var arList = await _attendRepository.GetAllAttendanceReportsForMentorAsync(userId);
                var response = _mapper.Map<List<AttendanceReportsListForMentorLecturerDTO>>(arList);

                return new DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                {
                    StatusCode = 200,
                    Message = "Attendance reports list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        // Mentor, Lecturer
        public async Task<DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>> GetAllAttendanceReportsByInternshipIdAsync(int? internshipId)
        {
            try
            {
                if (internshipId == null)
                {
                    return new DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found internship.",
                        Data = null
                    };
                }

                var arList = await _attendRepository.GetAllAttendanceReportsByInternshipIdAsync(internshipId);
                var response = _mapper.Map<List<AttendanceReportsListForMentorLecturerDTO>>(arList);

                return new DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                {
                    StatusCode = 200,
                    Message = "Attendance reports list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        // Lecturer
        public async Task<DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>> GetAllAttendanceReportsForLecturerAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found lecturer.",
                        Data = null
                    };
                }

                var arList = await _attendRepository.GetAllAttendanceReportsForLecturerAsync(userId);
                var response = _mapper.Map<List<AttendanceReportsListForMentorLecturerDTO>>(arList);

                return new DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                {
                    StatusCode = 200,
                    Message = "Attendance reports list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        // Student
        public async Task<DataResponse<List<AttendanceReportsListForStudentDTO>>> GetAllAttendanceReportsForStudentAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<AttendanceReportsListForStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found intern.",
                        Data = null
                    };
                }

                var arList = await _attendRepository.GetAllAttendanceReportsForStudentAsync(userId);
                var response = _mapper.Map<List<AttendanceReportsListForStudentDTO>>(arList);

                return new DataResponse<List<AttendanceReportsListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Attendance reports list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<AttendanceReportsListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
