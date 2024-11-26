using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Api.Input.CompanyControllers.UserController;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Api.Controllers.Company
{
    [Route("api/company/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("list")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> GetAllUsersForCompany(string? name, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var companyId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var dataResponse = await _userService.GetAllUsersForCompanyAsync(int.Parse(companyId), name, roleId, status, actualPageNumber, actualPageSize);

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
                var apiResponse = new ApiResponse<PagedResponse<List<UserListForCompanyDTO>>>
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
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> GetUserDetailForCompany(int? userId)
        {
            try
            {
                if (!userId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "userId is required."
                    });
                }

                var companyId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var dataResponse = await _userService.GetUserDetailByIdForCompanyAsync(int.Parse(companyId), userId.Value);

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
                var apiResponse = new ApiResponse<UserDetailForCompanyDTO>
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
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> AddUserForCompany([FromBody] AddUserRequestForCompany request)
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

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    errorMessages.Add("Name is required.");
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
                var userDto = new AddUserForCompanyDTO
                {
                    Email = request.Email,
                    Name = request.Name,
                    Information = request.Information
                };

                var companyId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var dataResponse = await _userService.AddUserForCompanyAsync(int.Parse(companyId), userDto);

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
                var apiResponse = new ApiResponse<AddUserForCompanyDTO>
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
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateUserForCompany(int? userId, [FromBody] UpdateUserRequestForCompany request)
        {
            try
            {
                if (!userId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "userId is required."
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

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    errorMessages.Add("Name is required.");
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

                // Chuyển đổi UpdateUserRequest thành DTO
                var userDto = new UpdateUserForCompanyDTO
                {
                    UserId = userId.Value,
                    Email = request.Email,
                    Name = request.Name,
                    Information = request.Information
                };

                var companyId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Gọi dịch vụ để cập nhật người dùng
                var dataResponse = await _userService.UpdateUserForCompanyAsync(int.Parse(companyId), userDto);

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
                var apiResponse = new ApiResponse<UpdateUserForCompanyDTO>
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
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> SoftDeleteUserForCompany(int? userId)
        {
            try
            {
                if (!userId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "userId is required."
                    });
                }

                // Chuyển đổi DeleteUserRequest thành DTO
                var userDto = new DeleteUserForCompanyDTO
                {
                    UserId = userId.Value
                };

                var companyId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Gọi dịch vụ để xóa người dùng
                var dataResponse = await _userService.SoftDeleteUserForCompanyAsync(int.Parse(companyId), userDto);

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
                var apiResponse = new ApiResponse<DeleteUserForCompanyDTO>
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
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> UpdateUserStatusForCompany(int? userId, UpdateUserStatusRequestForCompany request)
        {
            try
            {
                if (!userId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "userId is required."
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
                var userDto = new UpdateUserStatusForCompanyDTO
                {
                    UserId = userId.Value,
                    Status = request.Status
                };

                var companyId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Gọi dịch vụ để cập nhật người dùng
                var dataResponse = await _userService.UpdateUserStatusForCompanyAsync(int.Parse(companyId), userDto);

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
                var apiResponse = new ApiResponse<UpdateUserStatusForCompanyDTO>
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


        [HttpGet("status-list")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> GetAllStatusesForCompany()
        {
            try
            {
                var dataResponse = await _userService.GetAllStatusesUserForCompanyAsync();

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
                var apiResponse = new ApiResponse<List<StatusUserListForCompanyDTO>>
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
    }
}
