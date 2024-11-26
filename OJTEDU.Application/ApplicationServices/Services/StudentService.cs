using AutoMapper;
using Microsoft.AspNetCore.Http;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System.Drawing.Printing;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.ProvinceDTO;
using static OJTEDU.Application.DTOs.StudentDTO;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class StudentService : IStudentService
    {
        private readonly IAttendanceReportRepository _attendRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public StudentService(IStudentRepository studentRepository, IMapper mapper, IAttendanceReportRepository attendRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _attendRepository = attendRepository;
            _httpContextAccessor = httpContextAccessor;
            _studentRepository = studentRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // Student
        public async Task<DataResponse<StudentDetailForStudentDTO>> GetStudentDetailByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<StudentDetailForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var student = await _studentRepository.GetStudentDetailByUserIdAsync(userId);
                var response = _mapper.Map<StudentDetailForStudentDTO>(student);

                return new DataResponse<StudentDetailForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Student information retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<StudentDetailForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving student information {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<UpdateStudentForStudentDTO>> UpdateStudentByUserIdAsync(int? userId, UpdateStudentForStudentDTO? updateInformation)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<UpdateStudentForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                // Create updated entities based on input data
                var updatedUser = new User
                {
                    Image = updateInformation.Image
                };

                var updatedStudent = new Student
                {
                    AlternativeEmail = updateInformation.AlternativeEmail,
                    Phone = updateInformation.Phone,
                    Dob = updateInformation.Dob,
                    Gender = updateInformation.Gender,
                };

                var updatedAddress = new Address
                {
                    Detail = updateInformation.Detail,
                    WardId = updateInformation.WardId,
                    DistrictId = updateInformation.DistrictId,
                    ProvinceId = updateInformation.ProvinceId
                };

                var updateStudentInfo = await _studentRepository.UpdateStudentByUserIdAsync(userId, updatedUser, updatedStudent, updatedAddress);
                var response = _mapper.Map<UpdateStudentForStudentDTO>(updateStudentInfo);

                return new DataResponse<UpdateStudentForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Student information retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateStudentForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while updating student information for user id {userId}: {ex.Message}.",
                    Data = null
                };
            }
        }
        //For Dean
        public async Task<DataResponse<string>> AssignLecturerForStudentsAsync(AssignLecturerForStudentDto dto)
        {
            try
            {
                // Lấy danh sách sinh viên để kiểm tra vai trò
                var studentsToUpdate = await _studentRepository.GetStudentsByIdsAsync(dto.StudentIds);
                if (studentsToUpdate == null || studentsToUpdate.Count == 0)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "StudentIds not found.",
                        StatusCode = 404
                    };
                }

                // Lấy thông tin Lecturer từ User dựa trên dto.LecturerId
                var lecturer = await _userRepository.GetUserByIdAsync(dto.LecturerId);
                if (lecturer == null)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Lecturer not found.",
                        StatusCode = 404
                    };
                }

                // Kiểm tra vai trò của Lecturer (LecturerId phải có Role là Lecturer hoặc Dean)
                if (lecturer.Role == null || (lecturer.Role.Name != "Lecturer" && lecturer.Role.Name != "Dean"))
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Invalid Lecturer Role. Lecturer must be Dean or Lecturer.",
                        StatusCode = 400
                    };
                }

                // Cập nhật LecturerId cho từng sinh viên
                foreach (var student in studentsToUpdate)
                {
                    student.LecturerId = dto.LecturerId;
                    student.UpdatedAt = DateTime.Now;
                }

                // Lưu thay đổi vào repository
                await _studentRepository.UpdateStudentsAsync(studentsToUpdate);

                // Trả về thành công
                return new DataResponse<string>
                {
                    Data = "Success",
                    Message = "LecturerId was updated successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                // Log lỗi tại đây nếu cần thiết
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Error: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        // 2. GetStudentListAsync (for Dean and Lecturer)
        public async Task<DataResponse<PagedResponse<List<StudentListDto>>>> GetStudentListAsync(
            string? studentName,
            string? lecturerName,
            int pageNumber,
            int pageSize)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();

                var students = await _studentRepository.GetStudentListAsync(userId, role, studentName, lecturerName);

                if (students == null || !students.Any())
                {
                    return new DataResponse<PagedResponse<List<StudentListDto>>>
                    {
                        Data = null,
                        Message = "No students found.",
                        StatusCode = 404
                    };
                }

                var totalStudents = students.Count();
                var totalPages = (int)Math.Ceiling((double)totalStudents / pageSize);

                var studentDtos = _mapper.Map<List<StudentListDto>>(students)
                                          .Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToList();

                // Calculate 'Attended' status for each student
               

                var pagedResponse = new PagedResponse<List<StudentListDto>>
                {
                    Items = studentDtos,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<StudentListDto>>>
                {
                    Data = pagedResponse,
                    Message = "Student list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<StudentListDto>>>
                {
                    Data = null,
                    Message = $"Error retrieving student list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // KienBV - Fix
        public async Task<DataResponse<List<StudentListDto>>> GetOjtStudentListAsync()
        {
            try
            {
                var userId = GetCurrentUserId();

                var students = await _studentRepository.GetOjtStudentListAsync(userId);
                var response = _mapper.Map<List<StudentListDto>>(students);

                return new DataResponse<List<StudentListDto>>
                {
                    StatusCode = 200,
                    Message = "Student list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<StudentListDto>>
                {
                    Data = null,
                    Message = $"Error retrieving student list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // 3. GetStudentDetailsAsync (for Dean and Lecturer)
        public async Task<DataResponse<StudentDetailsDto>> GetStudentDetailsAsync(int studentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();

                var student = await _studentRepository.GetStudentDetailsByIdAsync(studentId, userId, role);

                if (student == null)
                {
                    return new DataResponse<StudentDetailsDto>
                    {
                        Data = null,
                        Message = "Student not found or access denied.",
                        StatusCode = 404
                    };
                }

                var studentDetailsDto = _mapper.Map<StudentDetailsDto>(student);

                return new DataResponse<StudentDetailsDto>
                {
                    Data = studentDetailsDto,
                    Message = "Student details retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<StudentDetailsDto>
                {
                    Data = null,
                    Message = $"Error retrieving student details: {ex.Message}",
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
    }
}
