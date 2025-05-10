using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.AdminControllers.UserController;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IJobService _jobService;
        private IUserService @object;

        public UserController(IJobService jobService)
        {
            _jobService = jobService;
        }

        public UserController(IUserService @object)
        {
            this.@object = @object;
        }

        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersForAdmin(string? name, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _jobService.GetAllUsersForAdminAsync(name, roleId, status, actualPageNumber, actualPageSize);

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
                var apiResponse = new ApiResponse<PagedResponse<List<UserListForAdminDTO>>>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserDetailForAdmin(int? userId)
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

                var dataResponse = await _jobService.GetUserDetailByIdForAdminAsync(userId.Value);

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
                var apiResponse = new ApiResponse<UserDetailForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserForAdmin([FromBody] AddUserRequestForAdmin request)
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
                var userDto = new AddUserForAdminDTO
                {
                    Email = request.Email,
                    RoleId = request.RoleId,
                    Name = request.Name,
                    UserCode = request.UserCode,
                    Information = request.Information
                };

                var dataResponse = await _jobService.AddUserForAdminAsync(userDto);

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
                var apiResponse = new ApiResponse<AddUserForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserForAdmin(int? userId, [FromBody] UpdateUserRequestForAdmin request)
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
                var userDto = new UpdateUserForAdminDTO
                {
                    UserId = userId.Value,
                    Email = request.Email,
                    //RoleId = request.RoleId,
                    Name = request.Name,
                    UserCode = request.UserCode,
                    Information = request.Information
                };

                // Gọi dịch vụ để cập nhật người dùng
                var dataResponse = await _jobService.UpdateUserForAdminAsync(userDto);

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
                var apiResponse = new ApiResponse<UpdateUserForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteUserForAdmin(int? userId)
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
                var userDto = new DeleteUserForAdminDTO
                {
                    UserId = userId.Value
                };

                // Gọi dịch vụ để xóa người dùng
                var dataResponse = await _jobService.SoftDeleteUserForAdminAsync(userDto);

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
                var apiResponse = new ApiResponse<DeleteUserForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatusForAdmin(int? userId, UpdateUserStatusRequestForAdmin request)
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
                var userDto = new UpdateUserStatusForAdminDTO
                {
                    UserId = userId.Value,
                    Status = request.Status
                };

                // Gọi dịch vụ để cập nhật người dùng
                var dataResponse = await _jobService.UpdateUserStatusForAdminAsync(userDto);

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
                var apiResponse = new ApiResponse<UpdateUserStatusForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportUsersForAdmin([FromForm] IFormFile file)
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
                var dataResponse = await _jobService.ImportUsersForAdminAsync(file);

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadTemplateForAdmin()
        {
            try
            {
                var dataResponse = await _jobService.GenerateUserTemplateForAdminAsync();

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
                    FileDownloadName = "UserTemplateForAdmin.xlsx"
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStatusesForAdmin()
        {
            try
            {
                var dataResponse = await _jobService.GetAllStatusesUserForAdminAsync();

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
                var apiResponse = new ApiResponse<List<StatusUserListForAdminDTO>>
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


        [HttpGet("users-stored-list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersStoredForAdmin(string? name, int? roleId, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _jobService.GetAllUsersStoredForAdmin(name, roleId, actualPageNumber, actualPageSize);

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

                // Successful retrieval of users stored
                var apiResponse = new ApiResponse<PagedResponse<List<UserListForAdminDTO>>>
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

        [HttpGet("users-stored-detail/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserStoredDetailForAdmin(int? userId)
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

                var dataResponse = await _jobService.GetUserStoredDetailByIdForAdminAsync(userId.Value);

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
                var apiResponse = new ApiResponse<UserDetailForAdminDTO>
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


        [HttpPatch("users-stored/restore/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreUserForAdmin(int? userId)
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

                var userDto = new RestoreUserForAdminDTO
                {
                    UserId = userId.Value
                };

                // Gọi dịch vụ để xóa người dùng
                var dataResponse = await _jobService.RestoreUserForAdminAsync(userDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "An unexpected error occurred while restoring the user from the stored list."
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
                var apiResponse = new ApiResponse<RestoreUserForAdminDTO>
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

        [HttpDelete("users-stored/hard-delete/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HardDeleteUserForAdmin(int? userId)
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
                var userDto = new DeleteUserForAdminDTO
                {
                    UserId = userId.Value
                };

                // Gọi dịch vụ để xóa người dùng
                var dataResponse = await _jobService.HardDeleteUserStoredForAdminAsync(userDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during user stored hard delete."
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
                var apiResponse = new ApiResponse<DeleteUserForAdminDTO>
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDeansForAdminAsync(
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeanDetailsForAdminAsync(int deanId,
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
        [HttpPut("assign-department")]
        [Authorize(Roles = "Admin")]
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
        //[Authorize(Roles = "Admin")]
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
        [HttpPost("assign-lecturers")]
       // [Authorize(Roles = "Admin")]
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

        [HttpGet("lecturer-list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllLecturersAsync(
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLecturerDetailsForAdminAsync(
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
