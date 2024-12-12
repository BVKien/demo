using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.DOETControllers.UserController;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Api.Controllers.DOETControllers
{
    [Route("api/doet/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IJobService _jobService;

        public UserController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllUsersForDoet(string? name, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _jobService.GetAllUsersForDoetAsync(name, roleId, status, actualPageNumber, actualPageSize);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Successful retrieval of users
                var apiResponse = new ApiResponse<PagedResponse<List<UserListForDoetDTO>>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("details/{userId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetUserDetailForDoet(int? userId)
        {
            try
            {
                if (!userId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Id is required."
                    });
                }

                var dataResponse = await _jobService.GetUserDetailByIdForDoetAsync(userId.Value);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Successful retrieval of users
                var apiResponse = new ApiResponse<UserDetailForDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> AddUserForDoet([FromBody] AddUserRequestForDoet request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    errorMessages.Add("Email is required.");
                }
                else if (!IsValidEmail(request.Email)) // Hàm kiểm tra định dạng email
                {
                    errorMessages.Add("Invalid email format.");
                }
                else if (request.Email.Length > 50)
                {
                    errorMessages.Add("Email must not exceed 50 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    errorMessages.Add("Name is required.");
                }
                else if (request.Name.Length > 350)
                {
                    errorMessages.Add("Name must not exceed 350 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.UserCode))
                {
                    errorMessages.Add("UserCode is required.");
                }
                else if (request.UserCode.Length > 50)
                {
                    errorMessages.Add("UserCode cannot exceed 50 characters.");
                }

                if (request.RoleId <= 0) // Kiểm tra RoleId
                {
                    errorMessages.Add("Role is required.");
                }

                // Nếu có lỗi, trả về phản hồi lỗi
                if (errorMessages.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = $"Validation errors occurred: {string.Join(", ", errorMessages)}"
                    });
                }

                // Chuyển đổi AddUserRequest thành DTO
                var userDto = new AddUserForDoetDTO
                {
                    Email = request.Email,
                    RoleId = request.RoleId,
                    Name = request.Name,
                    UserCode = request.UserCode,
                    Information = request.Information
                };

                var dataResponse = await _jobService.AddUserForDoetAsync(userDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during user add."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Successful retrieval of students
                var apiResponse = new ApiResponse<AddUserForDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        // Hàm kiểm tra định dạng email
        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateUserForDoet(int? userId, [FromBody] UpdateUserRequestForDoet request)
        {
            try
            {
                if (!userId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Id is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    errorMessages.Add("Email is required.");
                }
                else if (!IsValidEmail(request.Email)) // Hàm kiểm tra định dạng email
                {
                    errorMessages.Add("Invalid email format.");
                }
                else if (request.Email.Length > 50)
                {
                    errorMessages.Add("Email must not exceed 50 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    errorMessages.Add("Name is required.");
                }
                else if (request.Name.Length > 350)
                {
                    errorMessages.Add("Name must not exceed 350 characters.");
                }

                if (string.IsNullOrWhiteSpace(request.UserCode))
                {
                    errorMessages.Add("UserCode is required.");
                }
                else if (request.UserCode.Length > 50)
                {
                    errorMessages.Add("UserCode cannot exceed 50 characters.");
                }

                //if (request.RoleId <= 0) // Kiểm tra RoleId
                //{
                //    errorMessages.Add("Role is required.");
                //}

                // Nếu có lỗi, trả về phản hồi lỗi
                if (errorMessages.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = $"Validation errors occurred: {string.Join(", ", errorMessages)}"
                    });
                }

                // Chuyển đổi UpdateUserRequest thành DTO
                var userDto = new UpdateUserForDoetDTO
                {
                    UserId = userId.Value,
                    Email = request.Email,
                    //RoleId = request.RoleId,
                    Name = request.Name,
                    UserCode = request.UserCode,
                    Information = request.Information
                };

                // Gọi dịch vụ để cập nhật người dùng
                var dataResponse = await _jobService.UpdateUserForDoetAsync(userDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during user update."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<UpdateUserForDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpPatch("soft-delete/{userId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> SoftDeleteUserForDoet(int? userId)
        {
            try
            {
                if (!userId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Id is required."
                    });
                }

                // Chuyển đổi DeleteUserRequest thành DTO
                var userDto = new DeleteUserForDoetDTO
                {
                    UserId = userId.Value
                };

                // Gọi dịch vụ để xóa người dùng
                var dataResponse = await _jobService.SoftDeleteUserForDoetAsync(userDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during user soft delete."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }


                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<DeleteUserForDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpPatch("{userId}/status")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> UpdateUserStatusForDoet(int? userId, UpdateUserStatusRequestForDoet request)
        {
            try
            {
                if (!userId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Id is required."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Status))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Status is required."
                    });
                }

                // Chuyển đổi UpdateUserRequest thành DTO
                var userDto = new UpdateUserStatusForDoetDTO
                {
                    UserId = userId.Value,
                    Status = request.Status
                };

                // Gọi dịch vụ để cập nhật người dùng
                var dataResponse = await _jobService.UpdateUserStatusForDoetAsync(userDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during user update status."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Tạo phản hồi thành công
                var apiResponse = new ApiResponse<UpdateUserStatusForDoetDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpPost("import-file")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> ImportUsersForDoet([FromForm] IFormFile file)
        {
            try
            {
                // Kiểm tra file đầu vào
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "The uploaded file is empty or missing. Please ensure you provide a valid Excel file. If you're unsure of the required format, download the template and follow the instructions provided in the User Guide."
                    });
                }

                // Kiểm tra định dạng file (chỉ chấp nhận .xlsx hoặc .xls)
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Invalid file format. Only Excel files (.xlsx, .xls) are accepted. Please download the template to prepare your data correctly and follow the instructions in the User Guide."
                    });
                }

                // Gọi service để thực hiện import
                var dataResponse = await _jobService.ImportUsersForDoetAsync(file);

                // Kiểm tra kết quả trả về từ service
                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during the import process."
                    });
                }

                // Trả về lỗi nếu có
                if (dataResponse.StatusCode != 200)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = dataResponse.Data,
                        Message = dataResponse.Message
                    });
                }

                // Trả về kết quả import thành công
                return Ok(new ApiResponse<object>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Error occurred while importing users: {ex.Message}"
                });
            }
        }


        [HttpGet("download-file-template")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> DownloadTemplateForDoet()
        {
            try
            {
                var dataResponse = await _jobService.GenerateUserTemplateForDoetAsync();

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during template generation."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Đặt lại vị trí của MemoryStream về đầu
                var templateStream = dataResponse.Data;
                templateStream.Position = 0;

                // Trả về file dưới dạng file download
                var fileResult = new FileStreamResult(templateStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "UserTemplateForDoet.xlsx"
                };

                return fileResult; // Trả về file Excel cho client
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Error downloading template: {ex.Message}"
                });
            }
        }

        [HttpGet("status-list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllStatusesForDoet()
        {
            try
            {
                var dataResponse = await _jobService.GetAllStatusesUserForDoetAsync();

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                // Successful retrieval of users
                var apiResponse = new ApiResponse<List<StatusUserListForDoetDTO>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("dean-list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllDeansForDOETAsync(
        string? userCode,
        string? name,
        string? departmentName,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        bool? isDescending)
        {
            try
            {
                var actualPageNumber = pageNumber ?? 1;
                var actualPageSize = pageSize ?? 15;

                var response = await _jobService.GetAllDeansAsync(
                    userCode,
                    name,
                    departmentName,
                    actualPageNumber,
                    actualPageSize,
                    sortBy,
                    isDescending);

                if (response.Data == null)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<PagedResponse<List<DeanListForAdminDOETDto>>>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("dean-details/{deanId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetDeanDetailsForDOETAsync(int deanId,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        bool? isDescending,
        string? lecturerName,
        string? studentName,
        int? studentPageNumber,
        int? studentPageSize)
        {
            try
            {
                var actualPageNumber = pageNumber ?? 1;
                var actualPageSize = pageSize ?? 15;
                var actualStudentPageNumber = studentPageNumber ?? 1;
                var actualStudentPageSize = studentPageSize ?? 15;

                var response = await _jobService.GetDeanDetailsAsync(
                    deanId,
                    actualPageNumber,
                    actualPageSize,
                    sortBy,
                    isDescending,
                    lecturerName,
                    studentName,
                    actualStudentPageNumber,
                    actualStudentPageSize);

                if (response.Data == null)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<DeanDetailsDto>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
        [HttpPost("assign-lecturers")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> AssignLecturersToDeanForAdmin([FromBody] AssignLecturersToDeanDto assignLecturersDto)
        {
            var response = await _jobService.AssignLecturersToDeanAsync(assignLecturersDto);
            if (response.Data == null)
            {
                return StatusCode(response.StatusCode, new ApiResponse<object>
                {
                    Data = null,
                    Message = response.Message
                });
            }

            return Ok(new ApiResponse<string>
            {
                Data = response.Data,
                Message = response.Message
            });
        }
        [HttpPut("assign-department")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> AssignDepartmentToDeanAsync(int deanId, int departmentId)
        {
            try
            {
                var response = await _jobService.AssignDepartmentToDeanAsync(deanId, departmentId);

                if (response.StatusCode != 200)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
        [HttpPut("assign-major")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> AssignMajorToLecturerAsync(int lecturerId, int majorId)
        {
            try
            {
                var response = await _jobService.AssignMajorToLecturerAsync(lecturerId, majorId);

                if (response.StatusCode != 200)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
        [HttpGet("lecturer-list")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetAllLecturersForDOETAsync(
         string? userCode,
         string? name,
         string? majorName,
         int? pageNumber,
         int? pageSize,
         string? sortBy,
         bool? isDescending)
        {
            try
            {
                var actualPageNumber = pageNumber ?? 1;
                var actualPageSize = pageSize ?? 15;

                var response = await _jobService.GetAllLecturersAsync(
                    userCode,
                    name,
                    majorName,
                    actualPageNumber,
                    actualPageSize,
                    sortBy,
                    isDescending);

                if (response.Data == null)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<PagedResponse<List<LecturerListDto>>>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("lecturer-detail/{lecturerId}")]
        [Authorize(Roles = "DOET")]
        public async Task<IActionResult> GetLecturerDetailsForDOETAsync(
        int lecturerId,
        string? studentName,
        string? lecturerName,
        string? semesterName,
        string? sortBy,
        bool? isDescending,
        int? pageNumber,
        int? pageSize)
        {
            try
            {
                var actualPageNumber = pageNumber ?? 1;
                var actualPageSize = pageSize ?? 15;

                var response = await _jobService.GetLecturerDetailsAsync(
                    lecturerId,
                    studentName,
                    lecturerName,
                    semesterName,
                    sortBy,
                    isDescending,
                    actualPageNumber,
                    actualPageSize);

                if (response.Data == null)
                {
                    return StatusCode(response.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = response.Message
                    });
                }

                return Ok(new ApiResponse<LecturerDetailsDto>
                {
                    Data = response.Data,
                    Message = response.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }
    }
}
