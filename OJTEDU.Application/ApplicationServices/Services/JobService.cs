using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System.Xml.Linq;
using static OJTEDU.Application.DTOs.JobDTO;
using OJTEDU.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using AutoMapper.Configuration.Annotations;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using static OJTEDU.Application.DTOs.UserDTO;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using static OJTEDU.Application.DTOs.StudentDTO;
using System.Text;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IGoogleJsonWebSignatureValidator _googleValidator;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMajorRepository _majorRepository;
        public JobService(IJobRepository jobRepository, IMapper mapper, HttpClient httpClient, IConfiguration config, IGoogleJsonWebSignatureValidator googleValidator, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IMajorRepository majorRepository)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _googleValidator = googleValidator;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _majorRepository = majorRepository;
        }

        // Student  
        public async Task<DataResponse<List<JobListByCompanyIdForStudentDTO>>> GetAllJobsByCompanyIdAsync(int? companyId)
        {
            try
            {
                if (companyId == null)
                {
                    return new DataResponse<List<JobListByCompanyIdForStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var jobs = await _jobRepository.GetAllJobsByCompanyIdAsync(companyId);
                var response = _mapper.Map<List<JobListByCompanyIdForStudentDTO>>(jobs);

                return new DataResponse<List<JobListByCompanyIdForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Job list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<JobListByCompanyIdForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job list {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<PagedResult<List<JobListSearchForStudentDTO>>> SearchJobsAsync(int? userId, string? title, int? majorId, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                var (jobs, totalRecords) = await _jobRepository.SearchJobsAsync(userId, title, majorId, provinceId, districtId, wardId, pageNumber, pageSize);
                var response = _mapper.Map<List<JobListSearchForStudentDTO>>(jobs);

                // Calculate the total number of pages
                int totalPages = pageSize.HasValue ? (int)Math.Ceiling((double)totalRecords / pageSize.Value) : 1;

                return new PagedResult<List<JobListSearchForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Job list retrieved successfully!",
                    TotalPages = totalPages,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new PagedResult<List<JobListSearchForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job list: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<JobListForStudentDTO>>> GetAllJobsAsync()
        {
            try
            {
                var jobs = await _jobRepository.GetAllJobsAsync();
                var response = _mapper.Map<List<JobListForStudentDTO>>(jobs);

                return new DataResponse<List<JobListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Job list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<JobListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job list {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<JobDetailForStudentDTO>> GetJobDetailAsync(int? jobId)
        {
            try
            {
                if (jobId == null)
                {
                    return new DataResponse<JobDetailForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found job.",
                        Data = null
                    };
                }

                var jobs = await _jobRepository.GetJobDetailAsync(jobId);
                var response = _mapper.Map<JobDetailForStudentDTO>(jobs);

                return new DataResponse<JobDetailForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Job detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<JobDetailForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job detail {ex.Message}. ",
                    Data = null
                };
            }
        }

        // Company
        public async Task<DataResponse<List<JobListForCompanyDTO>>> GetAllJobsByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<JobListForCompanyDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var jobs = await _jobRepository.GetAllJobsByUserIdAsync(userId);
                var response = _mapper.Map<List<JobListForCompanyDTO>>(jobs);

                return new DataResponse<List<JobListForCompanyDTO>>
                {
                    StatusCode = 200,
                    Message = "Job list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<JobListForCompanyDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job list {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<JobDetailForCompanyDTO>> GetJobDetailForCompanyAsync(int? jobId)
        {
            try
            {
                if (jobId == null)
                {
                    return new DataResponse<JobDetailForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found job.",
                        Data = null
                    };
                }

                var job = await _jobRepository.GetJobDetailAsync(jobId);
                var response = _mapper.Map<JobDetailForCompanyDTO>(job);

                return new DataResponse<JobDetailForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Job detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<JobDetailForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving job detail {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CreateJobForCompanyDTO>> CreateJobAsync(int? userId, string? fileName, byte[] fileData, CreateJobForCompanyDTO? info)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var majorId = (int)info?.MajorId;
                if (majorId == null)
                {
                    return new DataResponse<CreateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Major is required.",
                        Data = null
                    };
                }

                // Job
                var jobInfo = new Job
                {
                    Title = info?.Title,
                    Description = info?.Description,
                    SalaryRange = info?.SalaryRange,
                    Requirements = info?.Requirements,
                    SkillRequirements = info?.SkillRequirements,
                    Benefits = info?.Benefits,
                    WorkingHours = info?.WorkingHours,
                    Deadline = info?.Deadline,
                    MajorId = info?.MajorId,
                    Addressed = info?.Addressed,
                };

                // Address
                var addressInfo = new Address
                {
                    Detail = info?.Detail,
                    WardId = info?.WardId,
                    DistrictId = info?.DistrictId,
                    ProvinceId = info?.ProvinceId,
                };

                var job = await _jobRepository.CreateJobAsync(userId, fileName, fileData, jobInfo, addressInfo);
                var response = _mapper.Map<CreateJobForCompanyDTO>(job);

                return new DataResponse<CreateJobForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Create job successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateJobForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = $"Error create job: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<UpdateJobForCompanyDTO>> UpdateJobAsync(int? userId, int? jobId, string? fileName, byte[] fileData, UpdateJobForCompanyDTO? info)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<UpdateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                if (jobId == null)
                {
                    return new DataResponse<UpdateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Job is required.",
                        Data = null
                    };
                }

                var majorId = (int)info?.MajorId;
                if (majorId == null)
                {
                    return new DataResponse<UpdateJobForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Major is required.",
                        Data = null
                    };
                }

                // Job
                var jobInfo = new Job
                {
                    Title = info?.Title,
                    Description = info?.Description,
                    SalaryRange = info?.SalaryRange,
                    Requirements = info?.Requirements,
                    SkillRequirements = info?.SkillRequirements,
                    Benefits = info?.Benefits,
                    WorkingHours = info?.WorkingHours,
                    Deadline = info?.Deadline,
                    MajorId = info?.MajorId,
                };

                // Address
                var addressInfo = new Address
                {
                    Detail = info?.Detail,
                    WardId = info?.WardId,
                    DistrictId = info?.DistrictId,
                    ProvinceId = info?.ProvinceId,
                };

                var job = await _jobRepository.UpdateJobAsync(userId, jobId, fileName, fileData, jobInfo, addressInfo);
                var response = _mapper.Map<UpdateJobForCompanyDTO>(job);

                return new DataResponse<UpdateJobForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Update job successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateJobForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = $"Error update job: {ex.Message}. ",
                    Data = null
                };
            }
        }

        // User service 
        // Common - Authentication
        public async Task<DataResponse<UserReadForAuthDTO>> LoginWithGoogleAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return new DataResponse<UserReadForAuthDTO>
                    {
                        Data = null,
                        Message = "Token cannot be empty.",
                        StatusCode = 400 // Lỗi yêu cầu không hợp lệ
                    };
                }

                using (var client = _httpClient)
                {

                    string decodedCode = Uri.UnescapeDataString(token);

                    var tokenRequestUri = _config["Google:TokenRequestUri"];
                    var googleClientId = _config["Google:ClientId"];
                    var googleClientSecret = _config["Google:ClientSecret"];
                    var redirectUri = _config["Google:RedirectUri"];

                    var requestContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("code", decodedCode),
                        new KeyValuePair<string, string>("client_id", googleClientId),
                        new KeyValuePair<string, string>("client_secret", googleClientSecret),
                        new KeyValuePair<string, string>("redirect_uri", redirectUri),
                        new KeyValuePair<string, string>("grant_type", "authorization_code")
                    });

                    var tokenResponse = await client.PostAsync(tokenRequestUri, requestContent);

                    if (!tokenResponse.IsSuccessStatusCode)
                    {
                        return new DataResponse<UserReadForAuthDTO>
                        {
                            Data = null,
                            Message = "Invalid Google Token.",
                            StatusCode = 401 // Lỗi xác thực
                        };
                    }

                    var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
                    var tokenResponseContentJson = JObject.Parse(tokenResponseContent);

                    string accessToken = tokenResponseContentJson["access_token"].ToString();
                    string idToken = tokenResponseContentJson["id_token"].ToString();

                    //var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                    //{
                    //    Audience = new[] { googleClientId }
                    //});

                    var payload = await _googleValidator.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { googleClientId }
                    });

                    if (payload == null)
                    {
                        return new DataResponse<UserReadForAuthDTO>
                        {
                            Data = null,
                            Message = "Invalid Google Token.",
                            StatusCode = 401 // Lỗi xác thực
                        };
                    }

                    var user = await _userRepository.GetUserByEmailAsync(payload.Email);

                    if (user == null)
                    {
                        return new DataResponse<UserReadForAuthDTO>
                        {
                            Data = null,
                            Message = "User not found.",
                            StatusCode = 404 // Không tìm thấy tài khoản
                        };
                    }

                    if (user.Status == null)
                    {
                        return new DataResponse<UserReadForAuthDTO>
                        {
                            Data = null,
                            Message = "User account is not activated.",
                            StatusCode = 409 // Xung đột tài nguyên
                        };
                    }

                    // Kiểm tra nếu Avatar chưa có, lấy từ Google và cập nhật vào DB
                    if (string.IsNullOrEmpty(user.Image))
                    {
                        user.Image = payload.Picture; // Lấy avatar từ Google
                        await _userRepository.UpdateUserForAdminAsync(user); // Lưu cập nhật vào DB
                    }

                    if (user.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, user.Role.Name)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var httpContext = _httpContextAccessor.HttpContext;

                        if (httpContext == null)
                        {
                            return new DataResponse<UserReadForAuthDTO>
                            {
                                Data = null,
                                Message = "HttpContext not found.",
                                StatusCode = 500 // Lỗi phía máy chủ
                            };
                        }

                        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
                        {
                            IsPersistent = true,
                        });

                        var userDto = _mapper.Map<UserReadForAuthDTO>(user);

                        return new DataResponse<UserReadForAuthDTO>
                        {
                            Data = userDto,
                            Message = "Login successful!",
                            StatusCode = 200 // Thành công
                        };
                    }
                    else
                    {
                        return new DataResponse<UserReadForAuthDTO>
                        {
                            Data = null,
                            Message = "User account is not activated.",
                            StatusCode = 403 // Không được phép truy cập
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<UserReadForAuthDTO>
                {
                    Data = null,
                    Message = $"Login failed: {ex.Message}",
                    StatusCode = 500 // Lỗi phía máy chủ
                };
            }
        }

        public async Task<DataResponse<object>> LogoutAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                {
                    return new DataResponse<object>
                    {
                        Data = null,
                        Message = "HttpContext not found.",
                        StatusCode = 500 // Lỗi phía máy chủ
                    };
                }

                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return new DataResponse<object>
                {
                    Data = null,
                    Message = "Logout successful!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = $"Logout failed: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UserReadForAuthDTO>> GetAuthenticatedUserInfoAsync(ClaimsPrincipal userClaims)
        {
            var email = userClaims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                await LogoutAsync();
                return new DataResponse<UserReadForAuthDTO>
                {
                    Data = null,
                    Message = "Email claim not found.",
                    StatusCode = 404
                };
            }

            try
            {
                // Gọi phương thức không đồng bộ để lấy thông tin người dùng
                var user = await _userRepository.GetUserByEmailAsync(email);

                // Kiểm tra nếu tài khoản không còn hoạt động
                if (user.Status != "Active")
                {
                    await LogoutAsync();
                    return new DataResponse<UserReadForAuthDTO>
                    {
                        Data = null,
                        Message = "User account is no longer active.",
                        StatusCode = 404
                    };
                }

                var userDto = _mapper.Map<UserReadForAuthDTO>(user);

                return new DataResponse<UserReadForAuthDTO>
                {
                    Data = userDto,
                    Message = "User details retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex.InnerException is KeyNotFoundException)
            {
                await LogoutAsync();
                return new DataResponse<UserReadForAuthDTO>
                {
                    Data = null,
                    Message = "User account has been deleted or does not exist.",
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                await LogoutAsync();
                // Xử lý các lỗi khác
                return new DataResponse<UserReadForAuthDTO>
                {
                    Data = null,
                    Message = $"An unexpected error occurred: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // Admin - UserManagement
        public async Task<DataResponse<PagedResponse<List<UserListForAdminDTO>>>> GetAllUsersForAdminAsync(string? name, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var users = await _userRepository.GetAllUsersForAdminAsync(name, roleId, status);

                var totalUsers = users.Count();
                var totalPages = totalUsers == 0 ? 1 : (int)Math.Ceiling((double)totalUsers / pageSize);

                var userDtos = totalUsers > 0 ? _mapper.Map<List<UserListForAdminDTO>>(users).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                                                : new List<UserListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<UserListForAdminDTO>>
                {
                    Items = userDtos,
                    TotalCount = totalUsers,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "User list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Access denied while get user list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving user list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UserDetailForAdminDTO>> GetUserDetailByIdForAdminAsync(int userId)
        {
            try
            {
                // Lấy danh sách người dùng đã xóa
                var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);

                // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
                if (deletedUsers.Any(u => u.UserId == userId))
                {
                    throw new KeyNotFoundException("User is not in the list.");
                }

                var user = await _userRepository.GetUserByIdForAdminAsync(userId);

                if (user.Status == "Deleted")
                {
                    throw new KeyNotFoundException("User is deleted.");
                }

                var userDto = _mapper.Map<UserDetailForAdminDTO>(user);

                return new DataResponse<UserDetailForAdminDTO>
                {
                    Data = userDto,
                    Message = "User details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UserDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UserDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while get user detail: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UserDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving user details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddUserForAdminDTO>> AddUserForAdminAsync(AddUserForAdminDTO addUserForAdminDTO)
        {
            try
            {
                // Tạo người dùng mới
                var user = new User
                {
                    Email = addUserForAdminDTO.Email,
                    RoleId = addUserForAdminDTO.RoleId,
                    Name = addUserForAdminDTO.Name,
                    UserCode = addUserForAdminDTO.UserCode,
                    Information = addUserForAdminDTO.Information
                };

                // Thêm người dùng vào cơ sở dữ liệu
                var addUserResult = await _userRepository.AddUserForAdminAsync(user);

                // Cập nhật thời gian tạo vào DTO trả về
                addUserForAdminDTO.CreatedAt = addUserResult.CreatedAt;

                return new DataResponse<AddUserForAdminDTO>
                {
                    Data = addUserForAdminDTO,
                    Message = "User added successfully!",
                    StatusCode = 201
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp email đã tồn tại
                return new DataResponse<AddUserForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<AddUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while add user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateUserForAdminDTO>> UpdateUserForAdminAsync(UpdateUserForAdminDTO updateUserForAdminDTO)
        {
            try
            {
                // Tạo đối tượng người dùng mới từ DTO
                var user = new User
                {
                    UserId = updateUserForAdminDTO.UserId,
                    Email = updateUserForAdminDTO.Email,
                    //RoleId = updateUserForAdminDTO.RoleId,
                    Name = updateUserForAdminDTO.Name,
                    UserCode = updateUserForAdminDTO.UserCode,
                    Information = updateUserForAdminDTO.Information
                };

                // Cập nhật thông tin người dùng trong cơ sở dữ liệu
                var updatedUserResult = await _userRepository.UpdateUserForAdminAsync(user);

                var userDto = _mapper.Map<UpdateUserForAdminDTO>(updatedUserResult);

                return new DataResponse<UpdateUserForAdminDTO>
                {
                    Data = userDto,
                    Message = "User updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<UpdateUserForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp email hoặc UserCode đã tồn tại
                return new DataResponse<UpdateUserForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while update user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateUserStatusForAdminDTO>> UpdateUserStatusForAdminAsync(UpdateUserStatusForAdminDTO updateUserStatusForAdminDTO)
        {
            try
            {

                // Lấy thông tin người dùng từ cơ sở dữ liệu để kiểm tra vai trò
                var existingUser = await _userRepository.GetUserByIdForAdminAsync(updateUserStatusForAdminDTO.UserId);

                // Kiểm tra xem vai trò của người dùng có phải là "Admin" hay không
                if (existingUser != null && existingUser.Role != null && existingUser.Role.Name == "Admin")
                {
                    throw new InvalidOperationException("Cannot update the status of a user with the 'Admin' role.");
                }

                // Tạo đối tượng người dùng mới từ DTO
                var user = new User
                {
                    UserId = updateUserStatusForAdminDTO.UserId,
                    Status = updateUserStatusForAdminDTO.Status
                };

                // Cập nhật thông tin người dùng trong cơ sở dữ liệu
                var updatedUserStatusResult = await _userRepository.UpdateUserForAdminAsync(user);

                var userDto = _mapper.Map<UpdateUserStatusForAdminDTO>(updatedUserStatusResult);


                return new DataResponse<UpdateUserStatusForAdminDTO>
                {
                    Data = userDto,
                    Message = "User updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<UpdateUserStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp email hoặc UserCode đã tồn tại
                return new DataResponse<UpdateUserStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateUserStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while update user status: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateUserStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteUserForAdminDTO>> SoftDeleteUserForAdminAsync(DeleteUserForAdminDTO deleteUserForAdminDTO)
        {
            try
            {
                // Xóa người dùng trong cơ sở dữ liệu
                var deletedUserResult = await _userRepository.SoftDeleteUserForAdminAsync(deleteUserForAdminDTO.UserId);

                var userDto = _mapper.Map<DeleteUserForAdminDTO>(deletedUserResult);

                return new DataResponse<DeleteUserForAdminDTO>
                {
                    Data = userDto,
                    Message = "User has been marked as deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<DeleteUserForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<DeleteUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while soft delete user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Error marking user as deleted: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<MemoryStream>> GenerateUserTemplateForAdminAsync()
        {
            try
            {
                // Tạo một MemoryStream mới để lưu trữ template
                var memoryStream = new MemoryStream();

                // Sử dụng một thư viện như EPPlus, ClosedXML, hoặc NPOI để tạo file Excel
                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("User Template For Admin");

                    // Thêm tiêu đề cột
                    worksheet.Cells[1, 1].Value = "Email(*)";
                    worksheet.Cells[1, 2].Value = "FullName(*)";
                    worksheet.Cells[1, 3].Value = "UserCode(*)";
                    worksheet.Cells[1, 4].Value = "RoleId(*)";
                    worksheet.Cells[1, 5].Value = "Information";
                    worksheet.Cells[1, 6].Value = "MajorCode(* : Bắt buộc với student)";

                    // Định dạng tiêu đề cột
                    for (int col = 1; col <= 6; col++)
                    {
                        worksheet.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, col].Style.Font.Bold = true; // Đặt chữ in đậm
                    }

                    // Thêm dữ liệu mẫu cho student
                    worksheet.Cells[2, 1].Value = "datnthe163935@fpt.edu.vn"; // Email
                    worksheet.Cells[2, 2].Value = "Nguyễn Tiến Đạt"; // Full Name
                    worksheet.Cells[2, 3].Value = "HE163935"; // User Code
                    worksheet.Cells[2, 4].Value = 2; // Role ID
                    worksheet.Cells[2, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information
                    worksheet.Cells[2, 6].Value = "SE"; // MajorCode

                    // Thêm dữ liệu mẫu cho doet
                    worksheet.Cells[3, 1].Value = "phongdaotaoFPT@fe.edu.vn"; // Email
                    worksheet.Cells[3, 2].Value = "Phòng đào tạo FPT"; // Full Name
                    worksheet.Cells[3, 3].Value = "DOET"; // User Code
                    worksheet.Cells[3, 4].Value = 4; // Role ID
                    worksheet.Cells[3, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

                    // Thêm dữ liệu mẫu cho dean
                    worksheet.Cells[4, 1].Value = "nguyenvananh123@fe.edu.vn"; // Email
                    worksheet.Cells[4, 2].Value = "Nguyễn Văn Anh"; // Full Name
                    worksheet.Cells[4, 3].Value = "AnhVN123"; // User Code
                    worksheet.Cells[4, 4].Value = 5; // Role ID
                    worksheet.Cells[4, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

                    // Thêm dữ liệu mẫu cho lecturer
                    worksheet.Cells[5, 1].Value = "nguyenvanchien456@fe.edu.vn"; // Email
                    worksheet.Cells[5, 2].Value = "Nguyễn Văn Chiến"; // Full Name
                    worksheet.Cells[5, 3].Value = "chienVN456"; // User Code
                    worksheet.Cells[5, 4].Value = 6; // Role ID
                    worksheet.Cells[5, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

                    // Thêm dữ liệu mẫu cho company
                    worksheet.Cells[6, 1].Value = "congtyFPT@gmail.com"; // Email
                    worksheet.Cells[6, 2].Value = "Công ty FPT"; // Full Name
                    worksheet.Cells[6, 3].Value = "FPT (Viết tắt tên công ty)"; // User Code
                    worksheet.Cells[6, 4].Value = 3; // Role ID
                    worksheet.Cells[6, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

                    // Thêm tiêu đề "Hướng dẫn điền Role" chiếm 2 cột
                    worksheet.Cells[4, 7].Value = "Hướng dẫn điền Role";
                    worksheet.Cells[4, 7, 4, 8].Merge = true; // Ghép ô từ F4 đến G4
                    worksheet.Cells[4, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[4, 7].Style.Font.Bold = true; // Đặt chữ in đậm

                    // Thêm tiêu đề cho bảng hướng dẫn
                    worksheet.Cells[5, 7].Value = "Role ID";
                    worksheet.Cells[5, 8].Value = "Role Name";

                    // Định dạng tiêu đề Role ID và Role Name
                    worksheet.Cells[5, 7, 5, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[5, 7, 5, 8].Style.Font.Bold = true; // Đặt chữ in đậm

                    // Thêm dữ liệu mẫu cho các role
                    worksheet.Cells[6, 7].Value = 2; // Role ID
                    worksheet.Cells[6, 8].Value = "Student"; // Role Name

                    worksheet.Cells[7, 7].Value = 3; // Role ID
                    worksheet.Cells[7, 8].Value = "Company"; // Role Name

                    worksheet.Cells[8, 7].Value = 4; // Role ID
                    worksheet.Cells[8, 8].Value = "DOET"; // Role Name

                    worksheet.Cells[9, 7].Value = 5; // Role ID
                    worksheet.Cells[9, 8].Value = "Dean"; // Role Name

                    worksheet.Cells[10, 7].Value = 6; // Role ID
                    worksheet.Cells[10, 8].Value = "Lecturer"; // Role Name

                    // Định dạng bảng hướng dẫn
                    worksheet.Cells[4, 7, 10, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[5, 7, 10, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    // Căn giữa cho tất cả các ô
                    for (int row = 2; row <= 10; row++)
                    {
                        for (int col = 1; col <= 6; col++)
                        {
                            worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }

                    // Kéo dãn cột
                    worksheet.Column(1).Width = 30; // Email
                    worksheet.Column(2).Width = 25; // Full Name
                    worksheet.Column(3).Width = 27; // User Code
                    worksheet.Column(4).Width = 10; // Role ID
                    worksheet.Column(5).Width = 60; // Information
                    worksheet.Column(6).Width = 40; // MajorCode
                    worksheet.Column(7).Width = 15; // Role ID (guidance)
                    worksheet.Column(8).Width = 20; // Role Name (guidance)

                    // Thêm ghi chú cho các trường
                    worksheet.Cells[13, 7].Value = "Ghi chú:"; // Cột 6
                    worksheet.Cells[14, 7].Value = "(*) : Bắt buộc điền."; // Cột 6
                    worksheet.Cells[15, 7].Value = "Giới hạn UserCode <= 50 ký tự."; // Cột 6
                    worksheet.Cells[16, 7].Value = "Email cần đúng định dạng (vd: example@mail.com)."; // Cột 6
                    worksheet.Cells[17, 7].Value = "Role: Phải là số theo mẫu và dựa vào template."; // Cột 6
                    worksheet.Cells[18, 7].Value = "MajorCode: Bắt buộc với role Student (2), là mã của ngành học."; // Ghi chú cho cột Major

                    // Định dạng ghi chú
                    for (int row = 13; row <= 18; row++)
                    {
                        worksheet.Cells[row, 7].Style.Font.Bold = true;
                        worksheet.Cells[row, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }

                    // Lưu file vào MemoryStream
                    package.Save();
                }

                // Đặt lại vị trí của MemoryStream về đầu
                memoryStream.Position = 0;

                return new DataResponse<MemoryStream>
                {
                    Data = memoryStream,
                    Message = "Template generated successfully.",
                    StatusCode = 200
                };
            }
            catch (IOException ioEx)
            {
                // Xử lý lỗi liên quan đến I/O
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"I/O error while generating template: {ioEx.Message}",
                    StatusCode = 500
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"Access denied while generating template: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (OutOfMemoryException memEx)
            {
                // Xử lý lỗi không đủ bộ nhớ
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"Out of memory while generating template: {memEx.Message}",
                    StatusCode = 500
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"Error generating template: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<object>> ImportUsersForAdminAsync(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (file == null || file.Length == 0)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = "File is empty or not provided.",
                    StatusCode = 400
                };
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        var users = new List<User>();
                        var errorMessages = new List<string>();

                        // Xác định số hàng cuối cùng trong phạm vi từ cột A đến cột E
                        int lastRow = 1;
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            bool hasDataInRange = false;
                            for (int col = 1; col <= 6; col++) // Chỉ từ cột A đến E (1 đến 6)
                            {
                                if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col]?.Text))
                                {
                                    hasDataInRange = true;
                                    break;
                                }
                            }
                            if (hasDataInRange)
                            {
                                lastRow = row;
                            }
                        }

                        // Tạo dictionary để lưu mối quan hệ giữa email và majorId
                        var majorMapping = new Dictionary<string, int?>(); // Lưu email và MajorId tương ứng

                        for (int row = 2; row <= lastRow; row++)
                        {
                            var email = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var fullName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            var userCode = worksheet.Cells[row, 3].Value?.ToString().Trim();
                            var roleIdValue = worksheet.Cells[row, 4].Value?.ToString().Trim();
                            var information = worksheet.Cells[row, 5].Value?.ToString().Trim();
                            var majorCode = worksheet.Cells[row, 6].Value?.ToString().Trim();

                            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(roleIdValue))
                            {
                                errorMessages.Add($"Row {row}: Missing required data.");
                                continue;
                            }

                            // Kiểm tra email hợp lệ
                            if (!IsValidEmail(email))
                            {
                                errorMessages.Add($"Row {row}: Invalid email format.");
                                continue;
                            }

                            // Kiểm tra độ dài của UserCode
                            if (userCode.Length > 50)
                            {
                                errorMessages.Add($"Row {row}: UserCode must not exceed 50 characters.");
                                continue;
                            }

                            if (email.Length > 50)
                            {
                                errorMessages.Add($"Row {row}: Email must not exceed 50 characters.");
                                continue;
                            }

                            if (fullName.Length > 350)
                            {
                                errorMessages.Add($"Row {row}: FullName must not exceed 350 characters.");
                                continue;
                            }

                            // Kiểm tra và parse RoleId
                            if (!int.TryParse(roleIdValue, out int roleId) || !(new[] { 2, 3, 4, 5, 6 }.Contains(roleId)))
                            {
                                errorMessages.Add($"Row {row}: RoleId must follow the template and cannot be a letter. It must be one of the following values: 2, 3, 4, 5, 6.");
                                continue;
                            }

                            // Nếu RoleId là Student, kiểm tra Major
                            int? majorId = null;
                            if (roleId == 2)
                            {
                                if (string.IsNullOrWhiteSpace(majorCode))
                                {
                                    errorMessages.Add($"Row {row}: Major is required for RoleId = 2 (Student).");
                                    continue;
                                }

                                // Kiểm tra Major Code có tồn tại trong bảng Major không
                                var major = await _majorRepository.GetMajorByCodeAsync(majorCode);
                                if (major == null)
                                {
                                    errorMessages.Add($"Row {row}: Major code '{majorCode}' does not exist.");
                                    continue;
                                }

                                // Kiểm tra trạng thái Active của Major
                                if (major.Status.ToLower() != "active")
                                {
                                    errorMessages.Add($"Row {row}: Major code '{majorCode}' is not active and cannot be assigned to students.");
                                    continue;
                                }

                                majorId = major.MajorId; // Lấy MajorId từ bảng Major
                            }

                            // Lưu MajorId vào dictionary
                            majorMapping[email] = majorId;

                            // Kiểm tra nếu UserCode hoặc Email đã tồn tại
                            var isUserCodeExists = await _userRepository.IsUserCodeExistsAsync(userCode);
                            var isEmailExists = await _userRepository.IsEmailExistsAsync(email);

                            if (isUserCodeExists)
                            {
                                errorMessages.Add($"Row {row}: UserCode '{userCode}' already exists.");
                                continue;
                            }

                            if (isEmailExists)
                            {
                                errorMessages.Add($"Row {row}: Email '{email}' already exists.");
                                continue;
                            }

                            var user = new User
                            {
                                Email = email,
                                Name = fullName,
                                UserCode = userCode,
                                RoleId = roleId,
                                Information = information,
                                Status = "Active",
                                CreatedAt = GetVietnamTime(),
                                UpdatedAt = GetVietnamTime()
                            };

                            users.Add(user);
                        }

                        // Nếu có bất kỳ lỗi nào, trả về lỗi và không thêm vào DB
                        if (errorMessages.Any())
                        {
                            return new DataResponse<object>
                            {
                                Data = new
                                {
                                    SuccessCount = 0,
                                    ErrorCount = errorMessages.Count,
                                    Errors = errorMessages
                                },
                                Message = $"Import failed. There were {errorMessages.Count} errors. Please fix the reported errors to successfully add the file.",
                                StatusCode = 400
                            };
                        }


                        await _userRepository.AddUsersForAdminAsync(users);

                        var studentUsers = new List<Student>();
                        var companyUsers = new List<Company>();

                        foreach (var user in users)
                        {
                            // Lấy lại UserId sau khi lưu
                            var addedUser = await _userRepository.GetUserByEmailAsync(user.Email);

                            if (addedUser != null)
                            {
                                if (addedUser.RoleId == 2) // RoleId = 2 là Student
                                {
                                    var majorId = majorMapping[user.Email]; // Lấy MajorId từ dictionary
                                    studentUsers.Add(new Student { UserId = addedUser.UserId, MajorId = majorId, CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() });
                                }
                                else if (addedUser.RoleId == 3) // RoleId = 3 là Company
                                {
                                    companyUsers.Add(new Company { UserId = addedUser.UserId, CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() });
                                }
                            }
                        }

                        // Thực hiện thêm các bản ghi Student và Company nếu có
                        await _userRepository.AddStudentsAndCompaniesForAdminAsync(studentUsers, companyUsers);

                        var successCount = users.Count;

                        var resultMessage = $"Import completed. Successfully added {successCount} users.";

                        return new DataResponse<object>
                        {
                            Data = new
                            {
                                SuccessCount = successCount,
                                ErrorCount = 0,
                                Errors = errorMessages
                            },
                            Message = resultMessage,
                            StatusCode = 200
                        };
                    }
                }
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<object>
                {
                    Data = null,
                    Message = $"Access denied while importing users: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = $"Error importing users: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
        // Hàm kiểm tra định dạng email
        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DataResponse<List<StatusUserListForAdminDTO>>> GetAllStatusesUserForAdminAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusUserListForAdminDTO>
                {
                    new StatusUserListForAdminDTO { Status = "Active" },
                    new StatusUserListForAdminDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusUserListForAdminDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusUserListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<List<StatusUserListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Access denied while get status list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusUserListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        public async Task<DataResponse<PagedResponse<List<UserListForAdminDTO>>>> GetAllUsersStoredForAdmin(string? name, int? roleId, int pageNumber, int pageSize)
        {
            try
            {
                var users = await _userRepository.GetAllUsersStoredAsync(name, roleId);

                var totalUsers = users.Count();
                var totalPages = totalUsers == 0 ? 1 : (int)Math.Ceiling((double)totalUsers / pageSize);

                var userDtos = totalUsers > 0 ? _mapper.Map<List<UserListForAdminDTO>>(users).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                                                : new List<UserListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<UserListForAdminDTO>>
                {
                    Items = userDtos,
                    TotalCount = totalUsers,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "User stored list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Access denied while get user stored list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving user stored list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UserDetailForAdminDTO>> GetUserStoredDetailByIdForAdminAsync(int userId)
        {
            try
            {
                // Lấy danh sách người dùng đã xóa
                var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);
                // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
                if (!deletedUsers.Any(u => u.UserId == userId))
                {
                    throw new KeyNotFoundException("User is not in the stored list.");
                }

                var user = await _userRepository.GetUserStoredByIdForAdminAsync(userId);

                // Kiểm tra trạng thái người dùng
                if (user.Status != "Deleted")
                {
                    throw new KeyNotFoundException("User is not deleted.");
                }

                var userDto = _mapper.Map<UserDetailForAdminDTO>(user);

                return new DataResponse<UserDetailForAdminDTO>
                {
                    Data = userDto,
                    Message = "User Stored details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UserDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UserDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while get user stored detail: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UserDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving user stored details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteUserForAdminDTO>> HardDeleteUserStoredForAdminAsync(DeleteUserForAdminDTO deleteUserForAdminDTO)
        {
            try
            {
                // Lấy danh sách người dùng đã xóa
                var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);
                // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
                if (!deletedUsers.Any(u => u.UserId == deleteUserForAdminDTO.UserId))
                {
                    throw new KeyNotFoundException("User is not in the stored list.");
                }

                // Xóa người dùng trong cơ sở dữ liệu
                var deletedUserResult = await _userRepository.HardDeleteUserStoredAsync(deleteUserForAdminDTO.UserId);

                var userDto = _mapper.Map<DeleteUserForAdminDTO>(deletedUserResult);

                return new DataResponse<DeleteUserForAdminDTO>
                {
                    Data = userDto,
                    Message = "User Stored has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<DeleteUserForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp user không ở trạng thái Deleted
                return new DataResponse<DeleteUserForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<DeleteUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while hard delete user stored: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting user stored: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<RestoreUserForAdminDTO>> RestoreUserForAdminAsync(RestoreUserForAdminDTO restoreUserForAdminDTO)
        {
            try
            {
                // Lấy danh sách người dùng đã xóa
                var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);
                // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
                if (!deletedUsers.Any(u => u.UserId == restoreUserForAdminDTO.UserId))
                {
                    throw new KeyNotFoundException("User is not in the stored list.");
                }

                // Xóa người dùng trong cơ sở dữ liệu
                var deletedUserResult = await _userRepository.RestoreUserStoredAsync(restoreUserForAdminDTO.UserId);

                var userDto = _mapper.Map<RestoreUserForAdminDTO>(deletedUserResult);

                return new DataResponse<RestoreUserForAdminDTO>
                {
                    Data = userDto,
                    Message = "User Stored has been restored successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<RestoreUserForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp user không ở trạng thái Deleted
                return new DataResponse<RestoreUserForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<RestoreUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while restore user stored: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<RestoreUserForAdminDTO>
                {
                    Data = null,
                    Message = $"Error restoring user stored: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        // Doet - UserManagement
        public async Task<DataResponse<PagedResponse<List<UserListForDoetDTO>>>> GetAllUsersForDoetAsync(string? name, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var users = await _userRepository.GetAllUsersForDoetAsync(name, roleId, status);

                var totalUsers = users.Count();
                var totalPages = totalUsers == 0 ? 1 : (int)Math.Ceiling((double)totalUsers / pageSize);

                var userDtos = totalUsers > 0 ? _mapper.Map<List<UserListForDoetDTO>>(users).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                                                : new List<UserListForDoetDTO>();

                var pagedResponse = new PagedResponse<List<UserListForDoetDTO>>
                {
                    Items = userDtos,
                    TotalCount = totalUsers,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };


                return new DataResponse<PagedResponse<List<UserListForDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "User list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<UserListForDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<PagedResponse<List<UserListForDoetDTO>>>
                {
                    Data = null,
                    Message = $"Access denied while get user list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<UserListForDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving user list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UserDetailForDoetDTO>> GetUserDetailByIdForDoetAsync(int userId)
        {
            try
            {
                // Lấy danh sách người dùng đã xóa
                var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);

                // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
                if (deletedUsers.Any(u => u.UserId == userId))
                {
                    throw new KeyNotFoundException("User is not in the list.");
                }

                var user = await _userRepository.GetUserByIdForDoetAsync(userId);

                if (user.Status == "Deleted")
                {
                    throw new KeyNotFoundException("User is deleted.");
                }

                var userDto = _mapper.Map<UserDetailForDoetDTO>(user);

                return new DataResponse<UserDetailForDoetDTO>
                {
                    Data = userDto,
                    Message = "User details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UserDetailForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UserDetailForDoetDTO>
                {
                    Data = null,
                    Message = $"Access denied while get user detail: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UserDetailForDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving user details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddUserForDoetDTO>> AddUserForDoetAsync(AddUserForDoetDTO addUserForDoetDTO)
        {
            try
            {
                // Tạo người dùng mới
                var user = new User
                {
                    Email = addUserForDoetDTO.Email,
                    RoleId = addUserForDoetDTO.RoleId,
                    Name = addUserForDoetDTO.Name,
                    UserCode = addUserForDoetDTO.UserCode,
                    Information = addUserForDoetDTO.Information
                };

                // Thêm người dùng vào cơ sở dữ liệu
                var addUserResult = await _userRepository.AddUserForDoetAsync(user);

                // Cập nhật thời gian tạo vào DTO trả về
                addUserForDoetDTO.CreatedAt = addUserResult.CreatedAt;

                return new DataResponse<AddUserForDoetDTO>
                {
                    Data = addUserForDoetDTO,
                    Message = "User added successfully!",
                    StatusCode = 201
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp email đã tồn tại
                return new DataResponse<AddUserForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<AddUserForDoetDTO>
                {
                    Data = null,
                    Message = $"Access denied while add user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddUserForDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateUserForDoetDTO>> UpdateUserForDoetAsync(UpdateUserForDoetDTO updateUserForDoetDTO)
        {
            try
            {
                // Tạo đối tượng người dùng mới từ DTO
                var user = new User
                {
                    UserId = updateUserForDoetDTO.UserId,
                    Email = updateUserForDoetDTO.Email,
                    //RoleId = updateUserForDoetDTO.RoleId,
                    Name = updateUserForDoetDTO.Name,
                    UserCode = updateUserForDoetDTO.UserCode,
                    Information = updateUserForDoetDTO.Information
                };

                // Cập nhật thông tin người dùng trong cơ sở dữ liệu
                var updatedUserResult = await _userRepository.UpdateUserForDoetAsync(user);

                var userDto = _mapper.Map<UpdateUserForDoetDTO>(updatedUserResult);

                return new DataResponse<UpdateUserForDoetDTO>
                {
                    Data = userDto,
                    Message = "User updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<UpdateUserForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp email hoặc UserCode đã tồn tại
                return new DataResponse<UpdateUserForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateUserForDoetDTO>
                {
                    Data = null,
                    Message = $"Access denied while update user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateUserForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateUserStatusForDoetDTO>> UpdateUserStatusForDoetAsync(UpdateUserStatusForDoetDTO updateUserStatusForDoetDTO)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdForDoetAsync(updateUserStatusForDoetDTO.UserId);

                if (existingUser.Role != null && existingUser.Role.Name == "DOET")
                {
                    throw new InvalidOperationException("Cannot update the status of a user with the 'DOET' role.");
                }

                // Tạo đối tượng người dùng mới từ DTO
                var user = new User
                {
                    UserId = updateUserStatusForDoetDTO.UserId,
                    Status = updateUserStatusForDoetDTO.Status
                };

                // Cập nhật thông tin người dùng trong cơ sở dữ liệu
                var updatedUserStatusResult = await _userRepository.UpdateUserForDoetAsync(user);

                var userDto = _mapper.Map<UpdateUserStatusForDoetDTO>(updatedUserStatusResult);

                return new DataResponse<UpdateUserStatusForDoetDTO>
                {
                    Data = userDto,
                    Message = "User updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<UpdateUserStatusForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp email hoặc UserCode đã tồn tại
                return new DataResponse<UpdateUserStatusForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateUserStatusForDoetDTO>
                {
                    Data = null,
                    Message = $"Access denied while update user status: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateUserStatusForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteUserForDoetDTO>> SoftDeleteUserForDoetAsync(DeleteUserForDoetDTO deleteUserForDoetDTO)
        {
            try
            {
                // Xóa người dùng trong cơ sở dữ liệu
                var deletedUserResult = await _userRepository.SoftDeleteUserForDoetAsync(deleteUserForDoetDTO.UserId);

                var userDto = _mapper.Map<DeleteUserForDoetDTO>(deletedUserResult);

                return new DataResponse<DeleteUserForDoetDTO>
                {
                    Data = userDto,
                    Message = "User has been marked as deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<DeleteUserForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<DeleteUserForDoetDTO>
                {
                    Data = null,
                    Message = $"Access denied while soft delete user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteUserForDoetDTO>
                {
                    Data = null,
                    Message = $"Error marking user as deleted: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<MemoryStream>> GenerateUserTemplateForDoetAsync()
        {
            try
            {
                // Tạo một MemoryStream mới để lưu trữ template
                var memoryStream = new MemoryStream();

                // Sử dụng một thư viện như EPPlus, ClosedXML, hoặc NPOI để tạo file Excel
                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("User Template For Doet");

                    // Thêm tiêu đề cột
                    worksheet.Cells[1, 1].Value = "Email(*)";
                    worksheet.Cells[1, 2].Value = "FullName(*)";
                    worksheet.Cells[1, 3].Value = "UserCode(*)";
                    worksheet.Cells[1, 4].Value = "RoleId(*)";
                    worksheet.Cells[1, 5].Value = "Information";
                    worksheet.Cells[1, 6].Value = "MajorCode(* : Bắt buộc với student)";

                    // Định dạng tiêu đề cột
                    for (int col = 1; col <= 6; col++)
                    {
                        worksheet.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, col].Style.Font.Bold = true; // Đặt chữ in đậm
                    }

                    // Thêm dữ liệu mẫu student
                    worksheet.Cells[2, 1].Value = "datnthe163935@fpt.edu.vn"; // Email
                    worksheet.Cells[2, 2].Value = "Nguyễn Tiến Đạt"; // Full Name
                    worksheet.Cells[2, 3].Value = "HE163935"; // User Code
                    worksheet.Cells[2, 4].Value = 2; // Role ID
                    worksheet.Cells[2, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information
                    worksheet.Cells[2, 6].Value = "SE"; // MajorCode

                    // Thêm dữ liệu mẫu cho dean
                    worksheet.Cells[3, 1].Value = "nguyenvananh123@fe.edu.vn"; // Email
                    worksheet.Cells[3, 2].Value = "Nguyễn Văn Anh"; // Full Name
                    worksheet.Cells[3, 3].Value = "AnhVN123"; // User Code
                    worksheet.Cells[3, 4].Value = 5; // Role ID
                    worksheet.Cells[3, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

                    // Thêm dữ liệu mẫu cho lecturer
                    worksheet.Cells[4, 1].Value = "nguyenvanchien456@fe.edu.vn"; // Email
                    worksheet.Cells[4, 2].Value = "Nguyễn Văn Chiến"; // Full Name
                    worksheet.Cells[4, 3].Value = "chienVN456"; // User Code
                    worksheet.Cells[4, 4].Value = 6; // Role ID
                    worksheet.Cells[4, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

                    // Thêm dữ liệu mẫu cho company
                    worksheet.Cells[5, 1].Value = "congtyFPT@gmail.com"; // Email
                    worksheet.Cells[5, 2].Value = "Công ty FPT"; // Full Name
                    worksheet.Cells[5, 3].Value = "FPT (Viết tắt tên công ty)"; // User Code
                    worksheet.Cells[5, 4].Value = 3; // Role ID
                    worksheet.Cells[5, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

                    // Thêm tiêu đề "Hướng dẫn điền Role" chiếm 2 cột
                    worksheet.Cells[4, 7].Value = "Hướng dẫn điền Role";
                    worksheet.Cells[4, 7, 4, 8].Merge = true; // Ghép ô từ F4 đến G4
                    worksheet.Cells[4, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[4, 7].Style.Font.Bold = true; // Đặt chữ in đậm

                    // Thêm tiêu đề cho bảng hướng dẫn
                    worksheet.Cells[5, 7].Value = "Role ID";
                    worksheet.Cells[5, 8].Value = "Role Name";

                    // Định dạng tiêu đề Role ID và Role Name
                    worksheet.Cells[5, 7, 5, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[5, 7, 5, 8].Style.Font.Bold = true; // Đặt chữ in đậm

                    // Thêm dữ liệu mẫu cho các role
                    worksheet.Cells[6, 7].Value = 2; // Role ID
                    worksheet.Cells[6, 8].Value = "Student"; // Role Name

                    worksheet.Cells[7, 7].Value = 3; // Role ID
                    worksheet.Cells[7, 8].Value = "Company"; // Role Name

                    worksheet.Cells[8, 7].Value = 5; // Role ID
                    worksheet.Cells[8, 8].Value = "Dean"; // Role Name

                    worksheet.Cells[9, 7].Value = 6; // Role ID
                    worksheet.Cells[9, 8].Value = "Lecturer"; // Role Name

                    // Định dạng bảng hướng dẫn
                    worksheet.Cells[4, 7, 9, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[5, 7, 9, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    // Căn giữa cho tất cả các ô
                    for (int row = 2; row <= 9; row++)
                    {
                        for (int col = 1; col <= 6; col++)
                        {
                            worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }

                    // Kéo dãn cột
                    worksheet.Column(1).Width = 30; // Email
                    worksheet.Column(2).Width = 25; // Full Name
                    worksheet.Column(3).Width = 27; // User Code
                    worksheet.Column(4).Width = 10; // Role ID
                    worksheet.Column(5).Width = 60; // Information
                    worksheet.Column(6).Width = 40; // MajorCode
                    worksheet.Column(7).Width = 15; // Role ID (guidance)
                    worksheet.Column(8).Width = 20; // Role Name (guidance)

                    // Thêm ghi chú cho các trường
                    worksheet.Cells[13, 7].Value = "Ghi chú:"; // Cột 6
                    worksheet.Cells[14, 7].Value = "(*) : Bắt buộc điền."; // Cột 6
                    worksheet.Cells[15, 7].Value = "Giới hạn UserCode <= 50 ký tự."; // Cột 6
                    worksheet.Cells[16, 7].Value = "Email cần đúng định dạng (vd: example@mail.com)."; // Cột 6
                    worksheet.Cells[17, 7].Value = "Role: Phải là số theo mẫu và dựa vào template."; // Cột 6
                    worksheet.Cells[18, 7].Value = "MajorCode: Bắt buộc với role Student (2), là mã của ngành học."; // Ghi chú cho cột Major

                    // Định dạng ghi chú
                    for (int row = 13; row <= 18; row++)
                    {
                        worksheet.Cells[row, 7].Style.Font.Bold = true;
                        worksheet.Cells[row, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }

                    // Lưu file vào MemoryStream
                    package.Save();
                }

                // Đặt lại vị trí của MemoryStream về đầu
                memoryStream.Position = 0;

                return new DataResponse<MemoryStream>
                {
                    Data = memoryStream,
                    Message = "Template generated successfully.",
                    StatusCode = 200
                };
            }
            catch (IOException ioEx)
            {
                // Xử lý lỗi liên quan đến I/O
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"I/O error while generating template: {ioEx.Message}",
                    StatusCode = 500
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"Access denied while generating template: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (OutOfMemoryException memEx)
            {
                // Xử lý lỗi không đủ bộ nhớ
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"Out of memory while generating template: {memEx.Message}",
                    StatusCode = 500
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"Error generating template: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<object>> ImportUsersForDoetAsync(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (file == null || file.Length == 0)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = "File is empty or not provided.",
                    StatusCode = 400
                };
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        var users = new List<User>();
                        var errorMessages = new List<string>();

                        // Xác định số hàng cuối cùng trong phạm vi từ cột A đến cột E
                        int lastRow = 1;
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            bool hasDataInRange = false;
                            for (int col = 1; col <= 5; col++) // Chỉ từ cột A đến E (1 đến 5)
                            {
                                if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col]?.Text))
                                {
                                    hasDataInRange = true;
                                    break;
                                }
                            }
                            if (hasDataInRange)
                            {
                                lastRow = row;
                            }
                        }

                        // Tạo dictionary để lưu mối quan hệ giữa email và majorId
                        var majorMapping = new Dictionary<string, int?>(); // Lưu email và MajorId tương ứng

                        for (int row = 2; row <= lastRow; row++)
                        {
                            var email = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var fullName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            var userCode = worksheet.Cells[row, 3].Value?.ToString().Trim();
                            var roleIdValue = worksheet.Cells[row, 4].Value?.ToString().Trim();
                            var information = worksheet.Cells[row, 5].Value?.ToString().Trim();
                            var majorCode = worksheet.Cells[row, 6].Value?.ToString().Trim();

                            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(roleIdValue))
                            {
                                errorMessages.Add($"Row {row}: Missing required data.");
                                continue;
                            }

                            // Kiểm tra email hợp lệ
                            if (!IsValidEmail(email))
                            {
                                errorMessages.Add($"Row {row}: Invalid email format.");
                                continue;
                            }

                            // Kiểm tra độ dài của UserCode
                            if (userCode.Length > 50)
                            {
                                errorMessages.Add($"Row {row}: UserCode must not exceed 50 characters.");
                                continue;
                            }

                            // Kiểm tra và parse RoleId
                            if (!int.TryParse(roleIdValue, out int roleId) || !(new[] { 2, 3, 5, 6, 7 }.Contains(roleId)))
                            {
                                errorMessages.Add($"Row {row}: RoleId must follow the template and cannot be a letter. It must be one of the following values: 2, 3, 5, 6.");
                                continue;
                            }

                            // Nếu RoleId là Student, kiểm tra Major
                            int? majorId = null;
                            if (roleId == 2)
                            {
                                if (string.IsNullOrWhiteSpace(majorCode))
                                {
                                    errorMessages.Add($"Row {row}: Major is required for RoleId = 2 (Student).");
                                    continue;
                                }

                                // Kiểm tra Major Code có tồn tại trong bảng Major không
                                var major = await _majorRepository.GetMajorByCodeAsync(majorCode);
                                if (major == null)
                                {
                                    errorMessages.Add($"Row {row}: Major code '{majorCode}' does not exist.");
                                    continue;
                                }

                                // Kiểm tra trạng thái Active của Major
                                if (major.Status.ToLower() != "active")
                                {
                                    errorMessages.Add($"Row {row}: Major code '{majorCode}' is not active and cannot be assigned to students.");
                                    continue;
                                }

                                majorId = major.MajorId; // Lấy MajorId từ bảng Major
                            }

                            // Lưu MajorId vào dictionary
                            majorMapping[email] = majorId;

                            // Kiểm tra nếu UserCode hoặc Email đã tồn tại
                            var isUserCodeExists = await _userRepository.IsUserCodeExistsAsync(userCode);
                            var isEmailExists = await _userRepository.IsEmailExistsAsync(email);

                            if (isUserCodeExists)
                            {
                                errorMessages.Add($"Row {row}: UserCode '{userCode}' already exists.");
                                continue;
                            }

                            if (isEmailExists)
                            {
                                errorMessages.Add($"Row {row}: Email '{email}' already exists.");
                                continue;
                            }

                            var user = new User
                            {
                                Email = email,
                                Name = fullName,
                                UserCode = userCode,
                                RoleId = roleId,
                                Information = information,
                                Status = "Active",
                                CreatedAt = GetVietnamTime(),
                                UpdatedAt = GetVietnamTime()
                            };

                            users.Add(user);
                        }

                        // Nếu có bất kỳ lỗi nào, trả về lỗi và không thêm vào DB
                        if (errorMessages.Any())
                        {
                            return new DataResponse<object>
                            {
                                Data = new
                                {
                                    SuccessCount = 0,
                                    ErrorCount = errorMessages.Count,
                                    Errors = errorMessages
                                },
                                Message = $"Import failed. There were {errorMessages.Count} errors. Please fix the reported errors to successfully add the file.",
                                StatusCode = 400
                            };
                        }

                        // Thực hiện thêm các bản ghi hợp lệ
                        await _userRepository.AddUsersForDoetAsync(users);

                        var studentUsers = new List<Student>();
                        var companyUsers = new List<Company>();

                        foreach (var user in users)
                        {
                            // Lấy lại UserId sau khi lưu
                            var addedUser = await _userRepository.GetUserByEmailAsync(user.Email);

                            if (addedUser != null)
                            {
                                if (addedUser.RoleId == 2) // RoleId = 2 là Student
                                {
                                    var majorId = majorMapping[user.Email]; // Lấy MajorId từ dictionary
                                    studentUsers.Add(new Student { UserId = addedUser.UserId, MajorId = majorId, CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() });
                                }
                                else if (addedUser.RoleId == 3) // RoleId = 3 là Company
                                {
                                    companyUsers.Add(new Company { UserId = addedUser.UserId, CreatedAt = GetVietnamTime(), UpdatedAt = GetVietnamTime() });
                                }
                            }
                        }

                        // Thực hiện thêm các bản ghi Student và Company nếu có
                        await _userRepository.AddStudentsAndCompaniesForDoetAsync(studentUsers, companyUsers);

                        var successCount = users.Count;

                        var resultMessage = $"Import completed. Successfully added {successCount} users.";

                        return new DataResponse<object>
                        {
                            Data = new
                            {
                                SuccessCount = successCount,
                                ErrorCount = 0,
                                Errors = errorMessages
                            },
                            Message = resultMessage,
                            StatusCode = 200
                        };
                    }
                }
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<object>
                {
                    Data = null,
                    Message = $"Access denied while importing users: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = $"Error importing users: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        public async Task<DataResponse<List<StatusUserListForDoetDTO>>> GetAllStatusesUserForDoetAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusUserListForDoetDTO>
                {
                    new StatusUserListForDoetDTO { Status = "Active" },
                    new StatusUserListForDoetDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusUserListForDoetDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusUserListForDoetDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<List<StatusUserListForDoetDTO>>
                {
                    Data = null,
                    Message = $"Access denied while get status list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusUserListForDoetDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Doet - UserManagement
        public async Task<DataResponse<PagedResponse<List<UserListForCompanyDTO>>>> GetAllUsersForCompanyAsync(int companyId, string? name, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var users = await _userRepository.GetAllUsersForCompanyAsync(companyId, name, roleId, status);

                var totalUsers = users.Count();
                var totalPages = totalUsers == 0 ? 1 : (int)Math.Ceiling((double)totalUsers / pageSize);

                var userDtos = totalUsers > 0 ? _mapper.Map<List<UserListForCompanyDTO>>(users).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                                                : new List<UserListForCompanyDTO>();

                var pagedResponse = new PagedResponse<List<UserListForCompanyDTO>>
                {
                    Items = userDtos,
                    TotalCount = totalUsers,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };


                return new DataResponse<PagedResponse<List<UserListForCompanyDTO>>>
                {
                    Data = pagedResponse,
                    Message = "User list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<UserListForCompanyDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<PagedResponse<List<UserListForCompanyDTO>>>
                {
                    Data = null,
                    Message = $"Access denied while get user list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<UserListForCompanyDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving user list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UserDetailForCompanyDTO>> GetUserDetailByIdForCompanyAsync(int companyId, int userId)
        {
            try
            {
                // Lấy danh sách người dùng đã xóa
                var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);

                // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
                if (deletedUsers.Any(u => u.UserId == userId))
                {
                    throw new KeyNotFoundException("User is not in the list.");
                }

                var user = await _userRepository.GetUserByIdForCompanyAsync(companyId, userId);

                if (user.Status == "Deleted")
                {
                    throw new KeyNotFoundException("User is deleted.");
                }

                var userDto = _mapper.Map<UserDetailForCompanyDTO>(user);

                return new DataResponse<UserDetailForCompanyDTO>
                {
                    Data = userDto,
                    Message = "User details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UserDetailForCompanyDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UserDetailForCompanyDTO>
                {
                    Data = null,
                    Message = $"Access denied while get user detail: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UserDetailForCompanyDTO>
                {
                    Data = null,
                    Message = $"Error retrieving user details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddUserForCompanyDTO>> AddUserForCompanyAsync(int companyId, AddUserForCompanyDTO addUserForCompanyDTO)
        {
            try
            {
                // Tạo người dùng mới
                var user = new User
                {
                    Email = addUserForCompanyDTO.Email,
                    Name = addUserForCompanyDTO.Name,
                    Information = addUserForCompanyDTO.Information,
                    ForCompany = companyId,
                    RoleId = 7
                };

                // Thêm người dùng vào cơ sở dữ liệu
                var addUserResult = await _userRepository.AddUserForCompanyAsync(companyId, user);

                // Cập nhật thời gian tạo vào DTO trả về
                addUserForCompanyDTO.CreatedAt = addUserResult.CreatedAt;
                addUserForCompanyDTO.RoleId = addUserResult.RoleId.Value;
                addUserForCompanyDTO.ForCompany = addUserResult.ForCompany;
                addUserForCompanyDTO.UserCode = addUserResult.UserCode;

                return new DataResponse<AddUserForCompanyDTO>
                {
                    Data = addUserForCompanyDTO,
                    Message = "User added successfully!",
                    StatusCode = 201
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp email đã tồn tại
                return new DataResponse<AddUserForCompanyDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<AddUserForCompanyDTO>
                {
                    Data = null,
                    Message = $"Access denied while add user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddUserForCompanyDTO>
                {
                    Data = null,
                    Message = $"Error adding user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateUserForCompanyDTO>> UpdateUserForCompanyAsync(int companyId, UpdateUserForCompanyDTO updateUserForCompanyDTO)
        {
            try
            {
                // Tạo đối tượng người dùng mới từ DTO
                var user = new User
                {
                    UserId = updateUserForCompanyDTO.UserId,
                    Email = updateUserForCompanyDTO.Email,
                    Name = updateUserForCompanyDTO.Name,
                    Information = updateUserForCompanyDTO.Information
                };

                // Cập nhật thông tin người dùng trong cơ sở dữ liệu
                var updatedUserResult = await _userRepository.UpdateUserForCompanyAsync(companyId, user);

                var userDto = _mapper.Map<UpdateUserForCompanyDTO>(updatedUserResult);

                return new DataResponse<UpdateUserForCompanyDTO>
                {
                    Data = userDto,
                    Message = "User updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<UpdateUserForCompanyDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp email hoặc UserCode đã tồn tại
                return new DataResponse<UpdateUserForCompanyDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateUserForCompanyDTO>
                {
                    Data = null,
                    Message = $"Access denied while update user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateUserForCompanyDTO>
                {
                    Data = null,
                    Message = $"Error updating user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateUserStatusForCompanyDTO>> UpdateUserStatusForCompanyAsync(int companyId, UpdateUserStatusForCompanyDTO updateUserStatusForCompanyDTO)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdForCompanyAsync(companyId, updateUserStatusForCompanyDTO.UserId);

                if (existingUser.Role != null && existingUser.Role.Name == "Company")
                {
                    throw new InvalidOperationException("Cannot update the status of a user with the 'Company' role.");
                }

                // Tạo đối tượng người dùng mới từ DTO
                var user = new User
                {
                    UserId = updateUserStatusForCompanyDTO.UserId,
                    Status = updateUserStatusForCompanyDTO.Status
                };

                // Cập nhật thông tin người dùng trong cơ sở dữ liệu
                var updatedUserStatusResult = await _userRepository.UpdateUserForCompanyAsync(companyId, user);

                var userDto = _mapper.Map<UpdateUserStatusForCompanyDTO>(updatedUserStatusResult);

                return new DataResponse<UpdateUserStatusForCompanyDTO>
                {
                    Data = userDto,
                    Message = "User updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<UpdateUserStatusForCompanyDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                // Trường hợp email hoặc UserCode đã tồn tại
                return new DataResponse<UpdateUserStatusForCompanyDTO>
                {
                    Data = null,
                    Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
                    StatusCode = 400 // Bad Request
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateUserStatusForCompanyDTO>
                {
                    Data = null,
                    Message = $"Access denied while update user status: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateUserStatusForCompanyDTO>
                {
                    Data = null,
                    Message = $"Error updating user: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteUserForCompanyDTO>> SoftDeleteUserForCompanyAsync(int companyId, DeleteUserForCompanyDTO deleteUserForCompanyDTO)
        {
            try
            {
                // Xóa người dùng trong cơ sở dữ liệu
                var deletedUserResult = await _userRepository.SoftDeleteUserForCompanyAsync(companyId, deleteUserForCompanyDTO.UserId);

                var userDto = _mapper.Map<DeleteUserForCompanyDTO>(deletedUserResult);

                return new DataResponse<DeleteUserForCompanyDTO>
                {
                    Data = userDto,
                    Message = "User has been marked as deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                // Trường hợp user ko tồn tại
                return new DataResponse<DeleteUserForCompanyDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<DeleteUserForCompanyDTO>
                {
                    Data = null,
                    Message = $"Access denied while soft delete user: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteUserForCompanyDTO>
                {
                    Data = null,
                    Message = $"Error marking user as deleted: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }


        public async Task<DataResponse<List<StatusUserListForCompanyDTO>>> GetAllStatusesUserForCompanyAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusUserListForCompanyDTO>
                {
                    new StatusUserListForCompanyDTO { Status = "Active" },
                    new StatusUserListForCompanyDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusUserListForCompanyDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusUserListForCompanyDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<List<StatusUserListForCompanyDTO>>
                {
                    Data = null,
                    Message = $"Access denied while get status list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusUserListForCompanyDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        //For Dean
        public async Task<DataResponse<UserProfileDto>> ViewProfileAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();

                User user = null;
                if (role == "Dean")
                {
                    user = await _userRepository.GetDeanByUserIdAsync(userId);
                }
                else if (role == "Lecturer")
                {
                    user = await _userRepository.GetLecturerByUserIdAsync(userId);
                }

                if (user == null)
                {
                    return new DataResponse<UserProfileDto>
                    {
                        Data = null,
                        Message = "User not found.",
                        StatusCode = 404
                    };
                }

                var userProfileDto = _mapper.Map<UserProfileDto>(user);

                return new DataResponse<UserProfileDto>
                {
                    Data = userProfileDto,
                    Message = "Profile retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UserProfileDto>
                {
                    Data = null,
                    Message = $"Error retrieving profile: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // UpdateProfileAsync (for Dean and Lecturer)
        public async Task<DataResponse<string>> UpdateProfileAsync(UpdateProfileDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();

                User user = null;
                if (role == "Dean")
                {
                    user = await _userRepository.GetDeanByUserIdAsync(userId);
                }
                else if (role == "Lecturer")
                {
                    user = await _userRepository.GetLecturerByUserIdAsync(userId);
                }

                if (user == null)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "User not found.",
                        StatusCode = 404
                    };
                }

                user.Name = dto.Name;
                user.Information = dto.Information;
                user.UpdatedAt = GetVietnamTime();

                if (role == "Dean")
                {
                    await _userRepository.UpdateDeanAsync(user);
                }
                else if (role == "Lecturer")
                {
                    await _userRepository.UpdateLecturerAsync(user);
                }

                return new DataResponse<string>
                {
                    Data = "Success",
                    Message = "Profile updated successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Error updating profile: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // CreateLecturerAsync
        public async Task<DataResponse<string>> CreateLecturerAsync(CreateLecturerDto dto)
        {
            try
            {
                var assignForId = GetCurrentUserId();

                var existingUser = await _userRepository.GetLecturerByEmailForDeanAsync(dto.Email);
                if (existingUser != null)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Email already in use.",
                        StatusCode = 400
                    };
                }

                var roleId = await _userRepository.GetRoleIdByNameAsync("Lecturer");
                if (roleId == 0)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Role 'Lecturer' not found.",
                        StatusCode = 404
                    };
                }

                var newLecturer = new User
                {
                    Email = dto.Email,
                    Name = dto.Name,
                    RoleId = roleId,
                    Status = "Active",
                    AssignForId = assignForId,
                    CreatedAt = GetVietnamTime(),
                    UpdatedAt = GetVietnamTime()
                };

                await _userRepository.CreateLecturerForDeanAsync(newLecturer);

                return new DataResponse<string>
                {
                    Data = "Lecturer created successfully.",
                    Message = "Lecturer created successfully.",
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

        // GetLecturerListForDeanAsync
        public async Task<DataResponse<PagedResponse<List<LecturerListDto>>>> GetLecturerListForDeanAsync(
        string? name,
        string? userCode,
        string? majorName,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize)
        {
            try
            {
                var assignForId = GetCurrentUserId();

                // Lấy danh sách lecturers từ repository
                var lecturers = await _userRepository.GetLecturerListForDeanAsync(assignForId, name, userCode, majorName, sortBy, isDescending);

                if (lecturers == null || !lecturers.Any())
                {
                    return new DataResponse<PagedResponse<List<LecturerListDto>>>
                    {
                        Data = null,
                        Message = "No lecturers found.",
                        StatusCode = 404
                    };
                }

                // Phân trang
                var totalLecturers = lecturers.Count();
                var totalPages = (int)Math.Ceiling((double)totalLecturers / pageSize);

                var paginatedLecturers = lecturers
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var lecturerDtos = _mapper.Map<List<LecturerListDto>>(paginatedLecturers);

                var pagedResponse = new PagedResponse<List<LecturerListDto>>
                {
                    Items = lecturerDtos,
                    TotalCount = totalLecturers,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<LecturerListDto>>>
                {
                    Data = pagedResponse,
                    Message = "Lecturer list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<LecturerListDto>>>
                {
                    Data = null,
                    Message = $"Error retrieving lecturer list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        // GetLecturerDetailsAsync
        public async Task<DataResponse<LecturerDetailsDto>> GetLecturerDetailsAsync(
        int lecturerId,
        string? studentName,
        string? lecturerName,
        string? semesterName,
        string? sortBy,
        bool? isDescending,
        int pageNumber,
        int pageSize)
        {
            try
            {
                // Lấy thông tin Lecturer, Dean, và danh sách sinh viên
                var (lecturer, dean, students) = await _userRepository.GetLecturerDetailsWithDeanAndStudentsAsync(lecturerId);

                if (lecturer == null)
                {
                    return new DataResponse<LecturerDetailsDto>
                    {
                        Data = null,
                        Message = "Lecturer not found.",
                        StatusCode = 404
                    };
                }

                // Áp dụng tìm kiếm
                if (!string.IsNullOrWhiteSpace(studentName))
                {
                    studentName = studentName.ToLower();
                    students = students.Where(s => s.User.Name.ToLower().Contains(studentName)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(lecturerName))
                {
                    lecturerName = lecturerName.ToLower();
                    students = students.Where(s => s.Lecturer.Name.ToLower().Contains(lecturerName)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(semesterName))
                {
                    semesterName = semesterName.ToLower();
                    students = students.Where(s => s.Semester.Name.ToLower().Contains(semesterName)).ToList();
                }

                // Áp dụng sắp xếp
                switch (sortBy?.ToLower())
                {
                    case "name":
                        students = isDescending.HasValue && isDescending.Value
                            ? students.OrderByDescending(s => s.User.Name).ToList()
                            : students.OrderBy(s => s.User.Name).ToList();
                        break;
                    case "lecturername":
                        students = isDescending.HasValue && isDescending.Value
                            ? students.OrderByDescending(s => s.Lecturer.Name).ToList()
                            : students.OrderBy(s => s.Lecturer.Name).ToList();
                        break;
                    case "semestername":
                        students = isDescending.HasValue && isDescending.Value
                            ? students.OrderByDescending(s => s.Semester.Name).ToList()
                            : students.OrderBy(s => s.Semester.Name).ToList();
                        break;
                    default:
                        students = students.OrderBy(s => s.User.Name).ToList();
                        break;
                }

                // Phân trang
                var totalStudents = students.Count;
                var totalPages = (int)Math.Ceiling((double)totalStudents / pageSize);

                var paginatedStudents = students
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Mapping dữ liệu
                var studentDtos = _mapper.Map<List<StudentListDto>>(paginatedStudents);

                var lecturerDetails = _mapper.Map<LecturerDetailsDto>(lecturer);
                lecturerDetails.DeanName = dean?.Name; // Gán Dean Name
                lecturerDetails.Students = new PagedResponse<List<StudentListDto>>
                {
                    Items = studentDtos,
                    TotalCount = totalStudents,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<LecturerDetailsDto>
                {
                    Data = lecturerDetails,
                    Message = "Lecturer details retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<LecturerDetailsDto>
                {
                    Data = null,
                    Message = $"Error retrieving lecturer details: {ex.Message}",
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

        public async Task<DataResponse<PagedResponse<List<DeanListForAdminDOETDto>>>> GetAllDeansAsync(
        string? userCode,
        string? name,
        string? departmentName,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool? isDescending)
        {
            try
            {
                // Gọi repository
                var deans = await _userRepository.GetAllDeansAsync(userCode, name, departmentName, sortBy, isDescending);

                if (deans == null || !deans.Any())
                {
                    return new DataResponse<PagedResponse<List<DeanListForAdminDOETDto>>>
                    {
                        Data = null,
                        Message = "No deans found.",
                        StatusCode = 404
                    };
                }

                // Tính toán phân trang
                var totalDeans = deans.Count;
                var totalPages = (int)Math.Ceiling((double)totalDeans / pageSize);

                // Lấy dữ liệu theo phân trang
                var paginatedDeans = deans
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Mapping dữ liệu
                var deanDtos = _mapper.Map<List<DeanListForAdminDOETDto>>(paginatedDeans);

                var pagedResponse = new PagedResponse<List<DeanListForAdminDOETDto>>
                {
                    Items = deanDtos,
                    TotalCount = totalDeans,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<DeanListForAdminDOETDto>>>
                {
                    Data = pagedResponse,
                    Message = "Dean list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<DeanListForAdminDOETDto>>>
                {
                    Data = null,
                    Message = $"Error retrieving dean list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeanDetailsDto>> GetDeanDetailsAsync(
        int deanId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool? isDescending,
        string? lecturerName,
        string? studentName,
        int studentPageNumber,
        int studentPageSize)
        {
            try
            {
                var (dean, lecturers, students) = await _userRepository.GetDeanDetailsWithLecturersAndStudentsAsync(deanId);

                if (dean == null)
                {
                    return new DataResponse<DeanDetailsDto>
                    {
                        Data = null,
                        Message = "Dean not found.",
                        StatusCode = 404
                    };
                }

                // Filter lecturers by name
                if (!string.IsNullOrWhiteSpace(lecturerName))
                {
                    lecturers = lecturers
                        .Where(l => l.Name.ToLower().Contains(lecturerName.ToLower()))
                        .ToList();
                }

                // Filter students by name
                if (!string.IsNullOrWhiteSpace(studentName))
                {
                    students = students
                        .Where(s => s.User.Name.ToLower().Contains(studentName.ToLower()))
                        .ToList();
                }

                // Sort lecturers
                switch (sortBy?.ToLower())
                {
                    case "lecturername":
                        lecturers = isDescending.HasValue && isDescending.Value
                            ? lecturers.OrderByDescending(l => l.Name).ToList()
                            : lecturers.OrderBy(l => l.Name).ToList();
                        break;
                    case "major":
                        lecturers = isDescending.HasValue && isDescending.Value
                            ? lecturers.OrderByDescending(l => l.Major.Name).ToList()
                            : lecturers.OrderBy(l => l.Major.Name).ToList();
                        break;
                }

                // Sort students
                switch (sortBy?.ToLower())
                {
                    case "studentname":
                        students = isDescending.HasValue && isDescending.Value
                            ? students.OrderByDescending(s => s.User.Name).ToList()
                            : students.OrderBy(s => s.User.Name).ToList();
                        break;
                    case "semester":
                        students = isDescending.HasValue && isDescending.Value
                            ? students.OrderByDescending(s => s.Semester.Name).ToList()
                            : students.OrderBy(s => s.Semester.Name).ToList();
                        break;
                }

                // Paginate lecturers
                var lecturerDtos = _mapper.Map<List<LecturerListDto>>(lecturers)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Paginate students
                var paginatedStudents = students
                    .Skip((studentPageNumber - 1) * studentPageSize)
                    .Take(studentPageSize)
                    .ToList();

                var studentDtos = _mapper.Map<List<StudentListDto>>(paginatedStudents);

                var deanDetails = _mapper.Map<DeanDetailsDto>(dean);

                deanDetails.Lecturers = new PagedResponse<List<LecturerListDto>>
                {
                    Items = lecturerDtos,
                    TotalCount = lecturers.Count,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)lecturers.Count / pageSize)
                };

                deanDetails.Students = new PagedResponse<List<StudentListDto>>
                {
                    Items = studentDtos,
                    TotalCount = students.Count,
                    PageSize = studentPageSize,
                    CurrentPage = studentPageNumber,
                    TotalPages = (int)Math.Ceiling((double)students.Count / studentPageSize)
                };

                return new DataResponse<DeanDetailsDto>
                {
                    Data = deanDetails,
                    Message = "Dean details retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeanDetailsDto>
                {
                    Data = null,
                    Message = $"Error retrieving dean details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<string>> AssignLecturersToDeanAsync(AssignLecturersToDeanDto dto)
        {
            var dean = await _userRepository.GetDeanByUserIdAsync(dto.DeanId);
            if (dean == null)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = "Dean not found.",
                    StatusCode = 404
                };
            }

            var lecturers = await _userRepository.GetLecturersByIdsAsync(dto.LecturerIds);
            var invalidLecturers = new List<string>();

            foreach (var lecturer in lecturers)
            {
                if (lecturer.Major == null || lecturer.Major.DepartmentId != dean.DepartmentId)
                {
                    invalidLecturers.Add(lecturer.Email);
                }
                else
                {
                    lecturer.AssignForId = dto.DeanId;
                    lecturer.UpdatedAt = GetVietnamTime();
                }
            }

            if (invalidLecturers.Any())
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Some lecturers are invalid for this department: {string.Join(", ", invalidLecturers)}.",
                    StatusCode = 400
                };
            }

            await _userRepository.UpdateLecturersAsync(lecturers);

            return new DataResponse<string>
            {
                Data = "Lecturers assigned successfully.",
                Message = "Lecturers have been assigned to the Dean successfully.",
                StatusCode = 200
            };
        }

        public async Task<DataResponse<PagedResponse<List<LecturerListDto>>>> GetAllLecturersAsync(
        string? userCode,
        string? name,
        string? majorName,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool? isDescending)
        {
            try
            {
                // Gọi repository
                var lecturers = await _userRepository.GetAllLecturerAsync(userCode, name, majorName, sortBy, isDescending);

                if (lecturers == null || !lecturers.Any())
                {
                    return new DataResponse<PagedResponse<List<LecturerListDto>>>
                    {
                        Data = null,
                        Message = "No lecturers found.",
                        StatusCode = 404
                    };
                }

                // Tính toán phân trang
                var totalLecturers = lecturers.Count;
                var totalPages = (int)Math.Ceiling((double)totalLecturers / pageSize);

                // Lấy dữ liệu theo phân trang
                var paginatedLecturers = lecturers
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Mapping dữ liệu
                var lecturerDtos = _mapper.Map<List<LecturerListDto>>(paginatedLecturers);

                var pagedResponse = new PagedResponse<List<LecturerListDto>>
                {
                    Items = lecturerDtos,
                    TotalCount = totalLecturers,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<LecturerListDto>>>
                {
                    Data = pagedResponse,
                    Message = "Lecturer list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<LecturerListDto>>>
                {
                    Data = null,
                    Message = $"Error retrieving lecturer list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<string>> AssignDepartmentToDeanAsync(int deanId, int departmentId)
        {
            try
            {
                // Kiểm tra điều kiện gán
                var isAssignable = await _userRepository.IsDeanAssignableToDepartmentAsync(deanId);

                if (!isAssignable)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Cannot assign department. The dean has related lecturers or students assigned.",
                        StatusCode = 400
                    };
                }

                // Gán Department
                await _userRepository.AssignDepartmentToDeanAsync(deanId, departmentId);

                return new DataResponse<string>
                {
                    Data = "Success",
                    Message = "Department assigned successfully.",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 400
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Error assigning department: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<string>> AssignMajorToLecturerAsync(int lecturerId, int majorId)
        {
            try
            {
                // Kiểm tra điều kiện gán
                var isAssignable = await _userRepository.IsLecturerAssignableToMajorAsync(lecturerId, majorId);

                if (!isAssignable)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Cannot assign major. The lecturer is either assigned to students or the major is not allowed.",
                        StatusCode = 400
                    };
                }

                // Gán Major
                await _userRepository.AssignMajorToLecturerAsync(lecturerId, majorId);

                return new DataResponse<string>
                {
                    Data = "Success",
                    Message = "Major assigned successfully.",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (InvalidOperationException ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 400
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Error assigning major: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        private DateTime GetVietnamTime()
        {
            return DateTime.UtcNow.AddHours(7);
        }

        //public async Task<DataResponse<UserReadForAuthDTO>> LoginWithGoogleAsync(string token)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(token))
        //        {
        //            return new DataResponse<UserReadForAuthDTO>
        //            {
        //                Data = null,
        //                Message = "Token cannot be empty.",
        //                StatusCode = 400 // Lỗi yêu cầu không hợp lệ
        //            };
        //        }

        //        using (var client = _httpClient)
        //        {

        //            string decodedCode = Uri.UnescapeDataString(token);

        //            var tokenRequestUri = _config["Google:TokenRequestUri"];
        //            var googleClientId = _config["Google:ClientId"];
        //            var googleClientSecret = _config["Google:ClientSecret"];
        //            var redirectUri = _config["Google:RedirectUri"];

        //            var requestContent = new FormUrlEncodedContent(new[]
        //            {
        //                new KeyValuePair<string, string>("code", decodedCode),
        //                new KeyValuePair<string, string>("client_id", googleClientId),
        //                new KeyValuePair<string, string>("client_secret", googleClientSecret),
        //                new KeyValuePair<string, string>("redirect_uri", redirectUri),
        //                new KeyValuePair<string, string>("grant_type", "authorization_code")
        //            });

        //            var tokenResponse = await client.PostAsync(tokenRequestUri, requestContent);

        //            if (!tokenResponse.IsSuccessStatusCode)
        //            {
        //                return new DataResponse<UserReadForAuthDTO>
        //                {
        //                    Data = null,
        //                    Message = "Invalid Google Token.",
        //                    StatusCode = 401 // Lỗi xác thực
        //                };
        //            }

        //            var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
        //            var tokenResponseContentJson = JObject.Parse(tokenResponseContent);

        //            string accessToken = tokenResponseContentJson["access_token"].ToString();
        //            string idToken = tokenResponseContentJson["id_token"].ToString();

        //            //var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
        //            //{
        //            //    Audience = new[] { googleClientId }
        //            //});

        //            var payload = await _googleValidator.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
        //            {
        //                Audience = new[] { googleClientId }
        //            });

        //            if (payload == null)
        //            {
        //                return new DataResponse<UserReadForAuthDTO>
        //                {
        //                    Data = null,
        //                    Message = "Invalid Google Token.",
        //                    StatusCode = 401 // Lỗi xác thực
        //                };
        //            }

        //            var user = await _userRepository.GetUserByEmailAsync(payload.Email);

        //            if (user == null)
        //            {
        //                return new DataResponse<UserReadForAuthDTO>
        //                {
        //                    Data = null,
        //                    Message = "User not found.",
        //                    StatusCode = 404 // Không tìm thấy tài khoản
        //                };
        //            }

        //            if (user.Status == null)
        //            {
        //                return new DataResponse<UserReadForAuthDTO>
        //                {
        //                    Data = null,
        //                    Message = "User account is not activated.",
        //                    StatusCode = 409 // Xung đột tài nguyên
        //                };
        //            }

        //            // Kiểm tra nếu Avatar chưa có, lấy từ Google và cập nhật vào DB
        //            if (string.IsNullOrEmpty(user.Image))
        //            {
        //                user.Image = payload.Picture; // Lấy avatar từ Google
        //                await _userRepository.UpdateUserForAdminAsync(user); // Lưu cập nhật vào DB
        //            }

        //            if (user.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
        //            {
        //                var claims = new List<Claim>
        //                {
        //                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        //                    new Claim(ClaimTypes.Email, user.Email),
        //                    new Claim(ClaimTypes.Role, user.Role.Name)
        //                };

        //                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //                var httpContext = _httpContextAccessor.HttpContext;

        //                if (httpContext == null)
        //                {
        //                    return new DataResponse<UserReadForAuthDTO>
        //                    {
        //                        Data = null,
        //                        Message = "HttpContext not found.",
        //                        StatusCode = 500 // Lỗi phía máy chủ
        //                    };
        //                }

        //                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
        //                {
        //                    IsPersistent = true,
        //                });

        //                var userDto = _mapper.Map<UserReadForAuthDTO>(user);

        //                return new DataResponse<UserReadForAuthDTO>
        //                {
        //                    Data = userDto,
        //                    Message = "Login successful!",
        //                    StatusCode = 200 // Thành công
        //                };
        //            }
        //            else
        //            {
        //                return new DataResponse<UserReadForAuthDTO>
        //                {
        //                    Data = null,
        //                    Message = "User account is not activated.",
        //                    StatusCode = 403 // Không được phép truy cập
        //                };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UserReadForAuthDTO>
        //        {
        //            Data = null,
        //            Message = $"Login failed: {ex.Message}",
        //            StatusCode = 500 // Lỗi phía máy chủ
        //        };
        //    }
        //}

        //public async Task<DataResponse<PagedResponse<List<UserListForAdminDTO>>>> GetAllUsersForAdminAsync(string? name, int? roleId, string? status, int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        var users = await _userRepository.GetAllUsersForAdminAsync(name, roleId, status);

        //        var totalUsers = users.Count();
        //        var totalPages = totalUsers == 0 ? 1 : (int)Math.Ceiling((double)totalUsers / pageSize);

        //        var userDtos = totalUsers > 0 ? _mapper.Map<List<UserListForAdminDTO>>(users).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
        //                                        : new List<UserListForAdminDTO>();

        //        var pagedResponse = new PagedResponse<List<UserListForAdminDTO>>
        //        {
        //            Items = userDtos,
        //            TotalCount = totalUsers,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = totalPages
        //        };

        //        return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
        //        {
        //            Data = pagedResponse,
        //            Message = "User list retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get user list: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving user list: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<object>> LogoutAsync()
        //{
        //    try
        //    {
        //        var httpContext = _httpContextAccessor.HttpContext;

        //        if (httpContext == null)
        //        {
        //            return new DataResponse<object>
        //            {
        //                Data = null,
        //                Message = "HttpContext not found.",
        //                StatusCode = 500 // Lỗi phía máy chủ
        //            };
        //        }

        //        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //        return new DataResponse<object>
        //        {
        //            Data = null,
        //            Message = "Logout successful!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<object>
        //        {
        //            Data = null,
        //            Message = $"Logout failed: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<UserReadForAuthDTO>> GetAuthenticatedUserInfoAsync(ClaimsPrincipal userClaims)
        //{
        //    var email = userClaims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        await LogoutAsync();
        //        return new DataResponse<UserReadForAuthDTO>
        //        {
        //            Data = null,
        //            Message = "Email claim not found.",
        //            StatusCode = 404
        //        };
        //    }

        //    try
        //    {
        //        // Gọi phương thức không đồng bộ để lấy thông tin người dùng
        //        var user = await _userRepository.GetUserByEmailAsync(email);

        //        // Kiểm tra nếu tài khoản không còn hoạt động
        //        if (user.Status != "Active")
        //        {
        //            await LogoutAsync();
        //            return new DataResponse<UserReadForAuthDTO>
        //            {
        //                Data = null,
        //                Message = "User account is no longer active.",
        //                StatusCode = 404
        //            };
        //        }

        //        var userDto = _mapper.Map<UserReadForAuthDTO>(user);

        //        return new DataResponse<UserReadForAuthDTO>
        //        {
        //            Data = userDto,
        //            Message = "User details retrieved successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex) when (ex is KeyNotFoundException || ex.InnerException is KeyNotFoundException)
        //    {
        //        await LogoutAsync();
        //        return new DataResponse<UserReadForAuthDTO>
        //        {
        //            Data = null,
        //            Message = "User account has been deleted or does not exist.",
        //            StatusCode = 404
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await LogoutAsync();
        //        // Xử lý các lỗi khác
        //        return new DataResponse<UserReadForAuthDTO>
        //        {
        //            Data = null,
        //            Message = $"An unexpected error occurred: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //// Admin - UserManagement
        //public async Task<DataResponse<UserDetailForAdminDTO>> GetUserDetailByIdForAdminAsync(int userId)
        //{
        //    try
        //    {
        //        // Lấy danh sách người dùng đã xóa
        //        var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);

        //        // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
        //        if (deletedUsers.Any(u => u.UserId == userId))
        //        {
        //            throw new KeyNotFoundException("User is not in the list.");
        //        }

        //        var user = await _userRepository.GetUserByIdForAdminAsync(userId);

        //        if (user.Status == "Deleted")
        //        {
        //            throw new KeyNotFoundException("User is deleted.");
        //        }

        //        var userDto = _mapper.Map<UserDetailForAdminDTO>(user);

        //        return new DataResponse<UserDetailForAdminDTO>
        //        {
        //            Data = userDto,
        //            Message = "User details retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<UserDetailForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UserDetailForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get user detail: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UserDetailForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving user details: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<AddUserForAdminDTO>> AddUserForAdminAsync(AddUserForAdminDTO addUserForAdminDTO)
        //{
        //    try
        //    {
        //        // Tạo người dùng mới
        //        var user = new User
        //        {
        //            Email = addUserForAdminDTO.Email,
        //            RoleId = addUserForAdminDTO.RoleId,
        //            Name = addUserForAdminDTO.Name,
        //            UserCode = addUserForAdminDTO.UserCode,
        //            Information = addUserForAdminDTO.Information
        //        };

        //        // Thêm người dùng vào cơ sở dữ liệu
        //        var addUserResult = await _userRepository.AddUserForAdminAsync(user);

        //        // Cập nhật thời gian tạo vào DTO trả về
        //        addUserForAdminDTO.CreatedAt = addUserResult.CreatedAt;

        //        return new DataResponse<AddUserForAdminDTO>
        //        {
        //            Data = addUserForAdminDTO,
        //            Message = "User added successfully!",
        //            StatusCode = 201
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp email đã tồn tại
        //        return new DataResponse<AddUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<AddUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while add user: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<AddUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Error adding user: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<UpdateUserForAdminDTO>> UpdateUserForAdminAsync(UpdateUserForAdminDTO updateUserForAdminDTO)
        //{
        //    try
        //    {
        //        // Tạo đối tượng người dùng mới từ DTO
        //        var user = new User
        //        {
        //            UserId = updateUserForAdminDTO.UserId,
        //            Email = updateUserForAdminDTO.Email,
        //            //RoleId = updateUserForAdminDTO.RoleId,
        //            Name = updateUserForAdminDTO.Name,
        //            UserCode = updateUserForAdminDTO.UserCode,
        //            Information = updateUserForAdminDTO.Information
        //        };

        //        // Cập nhật thông tin người dùng trong cơ sở dữ liệu
        //        var updatedUserResult = await _userRepository.UpdateUserForAdminAsync(user);

        //        var userDto = _mapper.Map<UpdateUserForAdminDTO>(updatedUserResult);

        //        return new DataResponse<UpdateUserForAdminDTO>
        //        {
        //            Data = userDto,
        //            Message = "User updated successfully!",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<UpdateUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp email hoặc UserCode đã tồn tại
        //        return new DataResponse<UpdateUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UpdateUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while update user: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UpdateUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Error updating user: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<UpdateUserStatusForAdminDTO>> UpdateUserStatusForAdminAsync(UpdateUserStatusForAdminDTO updateUserStatusForAdminDTO)
        //{
        //    try
        //    {

        //        // Lấy thông tin người dùng từ cơ sở dữ liệu để kiểm tra vai trò
        //        var existingUser = await _userRepository.GetUserByIdForAdminAsync(updateUserStatusForAdminDTO.UserId);

        //        // Kiểm tra xem vai trò của người dùng có phải là "Admin" hay không
        //        if (existingUser != null && existingUser.Role != null && existingUser.Role.Name == "Admin")
        //        {
        //            throw new InvalidOperationException("Cannot update the status of a user with the 'Admin' role.");
        //        }

        //        // Tạo đối tượng người dùng mới từ DTO
        //        var user = new User
        //        {
        //            UserId = updateUserStatusForAdminDTO.UserId,
        //            Status = updateUserStatusForAdminDTO.Status
        //        };

        //        // Cập nhật thông tin người dùng trong cơ sở dữ liệu
        //        var updatedUserStatusResult = await _userRepository.UpdateUserForAdminAsync(user);

        //        var userDto = _mapper.Map<UpdateUserStatusForAdminDTO>(updatedUserStatusResult);


        //        return new DataResponse<UpdateUserStatusForAdminDTO>
        //        {
        //            Data = userDto,
        //            Message = "User updated successfully!",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<UpdateUserStatusForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp email hoặc UserCode đã tồn tại
        //        return new DataResponse<UpdateUserStatusForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UpdateUserStatusForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while update user status: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UpdateUserStatusForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Error updating user: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<DeleteUserForAdminDTO>> SoftDeleteUserForAdminAsync(DeleteUserForAdminDTO deleteUserForAdminDTO)
        //{
        //    try
        //    {
        //        // Xóa người dùng trong cơ sở dữ liệu
        //        var deletedUserResult = await _userRepository.SoftDeleteUserForAdminAsync(deleteUserForAdminDTO.UserId);

        //        var userDto = _mapper.Map<DeleteUserForAdminDTO>(deletedUserResult);

        //        return new DataResponse<DeleteUserForAdminDTO>
        //        {
        //            Data = userDto,
        //            Message = "User has been marked as deleted successfully.",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<DeleteUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<DeleteUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while soft delete user: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<DeleteUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Error marking user as deleted: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<MemoryStream>> GenerateUserTemplateForAdminAsync()
        //{
        //    try
        //    {
        //        // Tạo một MemoryStream mới để lưu trữ template
        //        var memoryStream = new MemoryStream();

        //        // Sử dụng một thư viện như EPPlus, ClosedXML, hoặc NPOI để tạo file Excel
        //        using (var package = new ExcelPackage(memoryStream))
        //        {
        //            var worksheet = package.Workbook.Worksheets.Add("User Template For Admin");

        //            // Thêm tiêu đề cột
        //            worksheet.Cells[1, 1].Value = "Email(*)";
        //            worksheet.Cells[1, 2].Value = "FullName(*)";
        //            worksheet.Cells[1, 3].Value = "UserCode(*)";
        //            worksheet.Cells[1, 4].Value = "RoleId(*)";
        //            worksheet.Cells[1, 5].Value = "Information";
        //            worksheet.Cells[1, 6].Value = "MajorCode(* : Bắt buộc với student)";

        //            // Định dạng tiêu đề cột
        //            for (int col = 1; col <= 6; col++)
        //            {
        //                worksheet.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                worksheet.Cells[1, col].Style.Font.Bold = true; // Đặt chữ in đậm
        //            }

        //            // Thêm dữ liệu mẫu cho student
        //            worksheet.Cells[2, 1].Value = "datnthe163935@fpt.edu.vn"; // Email
        //            worksheet.Cells[2, 2].Value = "Nguyễn Tiến Đạt"; // Full Name
        //            worksheet.Cells[2, 3].Value = "HE163935"; // User Code
        //            worksheet.Cells[2, 4].Value = 2; // Role ID
        //            worksheet.Cells[2, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information
        //            worksheet.Cells[2, 6].Value = "SE"; // MajorCode

        //            // Thêm dữ liệu mẫu cho doet
        //            worksheet.Cells[3, 1].Value = "phongdaotaoFPT@fe.edu.vn"; // Email
        //            worksheet.Cells[3, 2].Value = "Phòng đào tạo FPT"; // Full Name
        //            worksheet.Cells[3, 3].Value = "DOET"; // User Code
        //            worksheet.Cells[3, 4].Value = 4; // Role ID
        //            worksheet.Cells[3, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

        //            // Thêm dữ liệu mẫu cho dean
        //            worksheet.Cells[4, 1].Value = "nguyenvananh123@fe.edu.vn"; // Email
        //            worksheet.Cells[4, 2].Value = "Nguyễn Văn Anh"; // Full Name
        //            worksheet.Cells[4, 3].Value = "AnhVN123"; // User Code
        //            worksheet.Cells[4, 4].Value = 5; // Role ID
        //            worksheet.Cells[4, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

        //            // Thêm dữ liệu mẫu cho lecturer
        //            worksheet.Cells[5, 1].Value = "nguyenvanchien456@fe.edu.vn"; // Email
        //            worksheet.Cells[5, 2].Value = "Nguyễn Văn Chiến"; // Full Name
        //            worksheet.Cells[5, 3].Value = "chienVN456"; // User Code
        //            worksheet.Cells[5, 4].Value = 6; // Role ID
        //            worksheet.Cells[5, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

        //            // Thêm dữ liệu mẫu cho company
        //            worksheet.Cells[6, 1].Value = "congtyFPT@gmail.com"; // Email
        //            worksheet.Cells[6, 2].Value = "Công ty FPT"; // Full Name
        //            worksheet.Cells[6, 3].Value = "FPT (Viết tắt tên công ty)"; // User Code
        //            worksheet.Cells[6, 4].Value = 3; // Role ID
        //            worksheet.Cells[6, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

        //            // Thêm tiêu đề "Hướng dẫn điền Role" chiếm 2 cột
        //            worksheet.Cells[4, 7].Value = "Hướng dẫn điền Role";
        //            worksheet.Cells[4, 7, 4, 8].Merge = true; // Ghép ô từ F4 đến G4
        //            worksheet.Cells[4, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //            worksheet.Cells[4, 7].Style.Font.Bold = true; // Đặt chữ in đậm

        //            // Thêm tiêu đề cho bảng hướng dẫn
        //            worksheet.Cells[5, 7].Value = "Role ID";
        //            worksheet.Cells[5, 8].Value = "Role Name";

        //            // Định dạng tiêu đề Role ID và Role Name
        //            worksheet.Cells[5, 7, 5, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //            worksheet.Cells[5, 7, 5, 8].Style.Font.Bold = true; // Đặt chữ in đậm

        //            // Thêm dữ liệu mẫu cho các role
        //            worksheet.Cells[6, 7].Value = 2; // Role ID
        //            worksheet.Cells[6, 8].Value = "Student"; // Role Name

        //            worksheet.Cells[7, 7].Value = 3; // Role ID
        //            worksheet.Cells[7, 8].Value = "Company"; // Role Name

        //            worksheet.Cells[8, 7].Value = 4; // Role ID
        //            worksheet.Cells[8, 8].Value = "DOET"; // Role Name

        //            worksheet.Cells[9, 7].Value = 5; // Role ID
        //            worksheet.Cells[9, 8].Value = "Dean"; // Role Name

        //            worksheet.Cells[10, 7].Value = 6; // Role ID
        //            worksheet.Cells[10, 8].Value = "Lecturer"; // Role Name

        //            // Định dạng bảng hướng dẫn
        //            worksheet.Cells[4, 7, 10, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //            worksheet.Cells[5, 7, 10, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //            // Căn giữa cho tất cả các ô
        //            for (int row = 2; row <= 10; row++)
        //            {
        //                for (int col = 1; col <= 6; col++)
        //                {
        //                    worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                }
        //            }

        //            // Kéo dãn cột
        //            worksheet.Column(1).Width = 30; // Email
        //            worksheet.Column(2).Width = 25; // Full Name
        //            worksheet.Column(3).Width = 27; // User Code
        //            worksheet.Column(4).Width = 10; // Role ID
        //            worksheet.Column(5).Width = 60; // Information
        //            worksheet.Column(6).Width = 40; // MajorCode
        //            worksheet.Column(7).Width = 15; // Role ID (guidance)
        //            worksheet.Column(8).Width = 20; // Role Name (guidance)

        //            // Thêm ghi chú cho các trường
        //            worksheet.Cells[13, 7].Value = "Ghi chú:"; // Cột 6
        //            worksheet.Cells[14, 7].Value = "(*) : Bắt buộc điền."; // Cột 6
        //            worksheet.Cells[15, 7].Value = "Giới hạn UserCode <= 50 ký tự."; // Cột 6
        //            worksheet.Cells[16, 7].Value = "Email cần đúng định dạng (vd: example@mail.com)."; // Cột 6
        //            worksheet.Cells[17, 7].Value = "Role: Phải là số theo mẫu và dựa vào template."; // Cột 6
        //            worksheet.Cells[18, 7].Value = "MajorCode: Bắt buộc với role Student (2), là mã của ngành học."; // Ghi chú cho cột Major

        //            // Định dạng ghi chú
        //            for (int row = 13; row <= 18; row++)
        //            {
        //                worksheet.Cells[row, 7].Style.Font.Bold = true;
        //                worksheet.Cells[row, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
        //            }

        //            // Lưu file vào MemoryStream
        //            package.Save();
        //        }

        //        // Đặt lại vị trí của MemoryStream về đầu
        //        memoryStream.Position = 0;

        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = memoryStream,
        //            Message = "Template generated successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (IOException ioEx)
        //    {
        //        // Xử lý lỗi liên quan đến I/O
        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = null,
        //            Message = $"I/O error while generating template: {ioEx.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = null,
        //            Message = $"Access denied while generating template: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (OutOfMemoryException memEx)
        //    {
        //        // Xử lý lỗi không đủ bộ nhớ
        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = null,
        //            Message = $"Out of memory while generating template: {memEx.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = null,
        //            Message = $"Error generating template: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<object>> ImportUsersForAdminAsync(IFormFile file)
        //{
        //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //    if (file == null || file.Length == 0)
        //    {
        //        return new DataResponse<object>
        //        {
        //            Data = null,
        //            Message = "File is empty or not provided.",
        //            StatusCode = 400
        //        };
        //    }

        //    try
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //                var users = new List<User>();
        //                var errorMessages = new List<string>();

        //                // Xác định số hàng cuối cùng trong phạm vi từ cột A đến cột E
        //                int lastRow = 1;
        //                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        //                {
        //                    bool hasDataInRange = false;
        //                    for (int col = 1; col <= 6; col++) // Chỉ từ cột A đến E (1 đến 6)
        //                    {
        //                        if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col]?.Text))
        //                        {
        //                            hasDataInRange = true;
        //                            break;
        //                        }
        //                    }
        //                    if (hasDataInRange)
        //                    {
        //                        lastRow = row;
        //                    }
        //                }

        //                // Tạo dictionary để lưu mối quan hệ giữa email và majorId
        //                var majorMapping = new Dictionary<string, int?>(); // Lưu email và MajorId tương ứng

        //                for (int row = 2; row <= lastRow; row++)
        //                {
        //                    var email = worksheet.Cells[row, 1].Value?.ToString().Trim();
        //                    var fullName = worksheet.Cells[row, 2].Value?.ToString().Trim();
        //                    var userCode = worksheet.Cells[row, 3].Value?.ToString().Trim();
        //                    var roleIdValue = worksheet.Cells[row, 4].Value?.ToString().Trim();
        //                    var information = worksheet.Cells[row, 5].Value?.ToString().Trim();
        //                    var majorCode = worksheet.Cells[row, 6].Value?.ToString().Trim();

        //                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(roleIdValue))
        //                    {
        //                        errorMessages.Add($"Row {row}: Missing required data.");
        //                        continue;
        //                    }

        //                    // Kiểm tra email hợp lệ
        //                    if (!IsValidEmail(email))
        //                    {
        //                        errorMessages.Add($"Row {row}: Invalid email format.");
        //                        continue;
        //                    }

        //                    // Kiểm tra độ dài của UserCode
        //                    if (userCode.Length > 50)
        //                    {
        //                        errorMessages.Add($"Row {row}: UserCode must not exceed 50 characters.");
        //                        continue;
        //                    }

        //                    if (email.Length > 50)
        //                    {
        //                        errorMessages.Add($"Row {row}: Email must not exceed 50 characters.");
        //                        continue;
        //                    }

        //                    if (fullName.Length > 350)
        //                    {
        //                        errorMessages.Add($"Row {row}: FullName must not exceed 350 characters.");
        //                        continue;
        //                    }

        //                    // Kiểm tra và parse RoleId
        //                    if (!int.TryParse(roleIdValue, out int roleId) || !(new[] { 2, 3, 4, 5, 6 }.Contains(roleId)))
        //                    {
        //                        errorMessages.Add($"Row {row}: RoleId must follow the template and cannot be a letter. It must be one of the following values: 2, 3, 4, 5, 6.");
        //                        continue;
        //                    }

        //                    // Nếu RoleId là Student, kiểm tra Major
        //                    int? majorId = null;
        //                    if (roleId == 2)
        //                    {
        //                        if (string.IsNullOrWhiteSpace(majorCode))
        //                        {
        //                            errorMessages.Add($"Row {row}: Major is required for RoleId = 2 (Student).");
        //                            continue;
        //                        }

        //                        // Kiểm tra Major Code có tồn tại trong bảng Major không
        //                        var major = await _majorRepository.GetMajorByCodeAsync(majorCode);
        //                        if (major == null)
        //                        {
        //                            errorMessages.Add($"Row {row}: Major code '{majorCode}' does not exist.");
        //                            continue;
        //                        }

        //                        // Kiểm tra trạng thái Active của Major
        //                        if (major.Status.ToLower() != "active")
        //                        {
        //                            errorMessages.Add($"Row {row}: Major code '{majorCode}' is not active and cannot be assigned to students.");
        //                            continue;
        //                        }

        //                        majorId = major.MajorId; // Lấy MajorId từ bảng Major
        //                    }

        //                    // Lưu MajorId vào dictionary
        //                    majorMapping[email] = majorId;

        //                    // Kiểm tra nếu UserCode hoặc Email đã tồn tại
        //                    var isUserCodeExists = await _userRepository.IsUserCodeExistsAsync(userCode);
        //                    var isEmailExists = await _userRepository.IsEmailExistsAsync(email);

        //                    if (isUserCodeExists)
        //                    {
        //                        errorMessages.Add($"Row {row}: UserCode '{userCode}' already exists.");
        //                        continue;
        //                    }

        //                    if (isEmailExists)
        //                    {
        //                        errorMessages.Add($"Row {row}: Email '{email}' already exists.");
        //                        continue;
        //                    }

        //                    var user = new User
        //                    {
        //                        Email = email,
        //                        Name = fullName,
        //                        UserCode = userCode,
        //                        RoleId = roleId,
        //                        Information = information,
        //                        Status = "Active",
        //                        CreatedAt = DateTime.Now,
        //                        UpdatedAt = DateTime.Now
        //                    };

        //                    users.Add(user);
        //                }

        //                // Nếu có bất kỳ lỗi nào, trả về lỗi và không thêm vào DB
        //                if (errorMessages.Any())
        //                {
        //                    return new DataResponse<object>
        //                    {
        //                        Data = new
        //                        {
        //                            SuccessCount = 0,
        //                            ErrorCount = errorMessages.Count,
        //                            Errors = errorMessages
        //                        },
        //                        Message = $"Import failed. There were {errorMessages.Count} errors. Please fix the reported errors to successfully add the file.",
        //                        StatusCode = 400
        //                    };
        //                }


        //                await _userRepository.AddUsersForAdminAsync(users);

        //                var studentUsers = new List<Student>();
        //                var companyUsers = new List<Company>();

        //                foreach (var user in users)
        //                {
        //                    // Lấy lại UserId sau khi lưu
        //                    var addedUser = await _userRepository.GetUserByEmailAsync(user.Email);

        //                    if (addedUser != null)
        //                    {
        //                        if (addedUser.RoleId == 2) // RoleId = 2 là Student
        //                        {
        //                            var majorId = majorMapping[user.Email]; // Lấy MajorId từ dictionary
        //                            studentUsers.Add(new Student { UserId = addedUser.UserId, MajorId = majorId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
        //                        }
        //                        else if (addedUser.RoleId == 3) // RoleId = 3 là Company
        //                        {
        //                            companyUsers.Add(new Company { UserId = addedUser.UserId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
        //                        }
        //                    }
        //                }

        //                // Thực hiện thêm các bản ghi Student và Company nếu có
        //                await _userRepository.AddStudentsAndCompaniesForAdminAsync(studentUsers, companyUsers);

        //                var successCount = users.Count;

        //                var resultMessage = $"Import completed. Successfully added {successCount} users.";

        //                return new DataResponse<object>
        //                {
        //                    Data = new
        //                    {
        //                        SuccessCount = successCount,
        //                        ErrorCount = 0,
        //                        Errors = errorMessages
        //                    },
        //                    Message = resultMessage,
        //                    StatusCode = 200
        //                };
        //            }
        //        }
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<object>
        //        {
        //            Data = null,
        //            Message = $"Access denied while importing users: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<object>
        //        {
        //            Data = null,
        //            Message = $"Error importing users: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}
        //// Hàm kiểm tra định dạng email
        //private bool IsValidEmail(string email)
        //{
        //    try
        //    {
        //        var mailAddress = new System.Net.Mail.MailAddress(email);
        //        return mailAddress.Address == email;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //public async Task<DataResponse<List<StatusUserListForAdminDTO>>> GetAllStatusesUserForAdminAsync()
        //{
        //    try
        //    {
        //        // Tạo danh sách trạng thái
        //        var statuses = new List<StatusUserListForAdminDTO>
        //        {
        //            new StatusUserListForAdminDTO { Status = "Active" },
        //            new StatusUserListForAdminDTO { Status = "Inactive" }
        //        };

        //        return new DataResponse<List<StatusUserListForAdminDTO>>
        //        {
        //            Data = statuses,
        //            Message = "Status List retrieved successfully!",
        //            StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<List<StatusUserListForAdminDTO>>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<List<StatusUserListForAdminDTO>>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get status list: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý ngoại lệ nếu có
        //        return new DataResponse<List<StatusUserListForAdminDTO>>
        //        {
        //            Data = null,
        //            Message = $"Error occurred while retrieving statuses: {ex.Message}",
        //            StatusCode = 500 // Lỗi server
        //        };
        //    }
        //}

        //public async Task<DataResponse<PagedResponse<List<UserListForAdminDTO>>>> GetAllUsersStoredForAdmin(string? name, int? roleId, int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        var users = await _userRepository.GetAllUsersStoredAsync(name, roleId);

        //        var totalUsers = users.Count();
        //        var totalPages = totalUsers == 0 ? 1 : (int)Math.Ceiling((double)totalUsers / pageSize);

        //        var userDtos = totalUsers > 0 ? _mapper.Map<List<UserListForAdminDTO>>(users).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
        //                                        : new List<UserListForAdminDTO>();

        //        var pagedResponse = new PagedResponse<List<UserListForAdminDTO>>
        //        {
        //            Items = userDtos,
        //            TotalCount = totalUsers,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = totalPages
        //        };

        //        return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
        //        {
        //            Data = pagedResponse,
        //            Message = "User stored list retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get user stored list: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<PagedResponse<List<UserListForAdminDTO>>>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving user stored list: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<UserDetailForAdminDTO>> GetUserStoredDetailByIdForAdminAsync(int userId)
        //{
        //    try
        //    {
        //        // Lấy danh sách người dùng đã xóa
        //        var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);
        //        // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
        //        if (!deletedUsers.Any(u => u.UserId == userId))
        //        {
        //            throw new KeyNotFoundException("User is not in the stored list.");
        //        }

        //        var user = await _userRepository.GetUserStoredByIdForAdminAsync(userId);

        //        // Kiểm tra trạng thái người dùng
        //        if (user.Status != "Deleted")
        //        {
        //            throw new KeyNotFoundException("User is not deleted.");
        //        }

        //        var userDto = _mapper.Map<UserDetailForAdminDTO>(user);

        //        return new DataResponse<UserDetailForAdminDTO>
        //        {
        //            Data = userDto,
        //            Message = "User Stored details retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<UserDetailForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UserDetailForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get user stored detail: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UserDetailForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving user stored details: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<DeleteUserForAdminDTO>> HardDeleteUserStoredForAdminAsync(DeleteUserForAdminDTO deleteUserForAdminDTO)
        //{
        //    try
        //    {
        //        // Lấy danh sách người dùng đã xóa
        //        var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);
        //        // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
        //        if (!deletedUsers.Any(u => u.UserId == deleteUserForAdminDTO.UserId))
        //        {
        //            throw new KeyNotFoundException("User is not in the stored list.");
        //        }

        //        // Xóa người dùng trong cơ sở dữ liệu
        //        var deletedUserResult = await _userRepository.HardDeleteUserStoredAsync(deleteUserForAdminDTO.UserId);

        //        var userDto = _mapper.Map<DeleteUserForAdminDTO>(deletedUserResult);

        //        return new DataResponse<DeleteUserForAdminDTO>
        //        {
        //            Data = userDto,
        //            Message = "User Stored has been permanently deleted successfully.",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<DeleteUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp user không ở trạng thái Deleted
        //        return new DataResponse<DeleteUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<DeleteUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while hard delete user stored: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<DeleteUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Error permanently deleting user stored: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<RestoreUserForAdminDTO>> RestoreUserForAdminAsync(RestoreUserForAdminDTO restoreUserForAdminDTO)
        //{
        //    try
        //    {
        //        // Lấy danh sách người dùng đã xóa
        //        var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);
        //        // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
        //        if (!deletedUsers.Any(u => u.UserId == restoreUserForAdminDTO.UserId))
        //        {
        //            throw new KeyNotFoundException("User is not in the stored list.");
        //        }

        //        // Xóa người dùng trong cơ sở dữ liệu
        //        var deletedUserResult = await _userRepository.RestoreUserStoredAsync(restoreUserForAdminDTO.UserId);

        //        var userDto = _mapper.Map<RestoreUserForAdminDTO>(deletedUserResult);

        //        return new DataResponse<RestoreUserForAdminDTO>
        //        {
        //            Data = userDto,
        //            Message = "User Stored has been restored successfully.",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<RestoreUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp user không ở trạng thái Deleted
        //        return new DataResponse<RestoreUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<RestoreUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while restore user stored: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<RestoreUserForAdminDTO>
        //        {
        //            Data = null,
        //            Message = $"Error restoring user stored: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //// Doet - UserManagement
        //public async Task<DataResponse<PagedResponse<List<UserListForDoetDTO>>>> GetAllUsersForDoetAsync(string? name, int? roleId, string? status, int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        var users = await _userRepository.GetAllUsersForDoetAsync(name, roleId, status);

        //        var totalUsers = users.Count();
        //        var totalPages = totalUsers == 0 ? 1 : (int)Math.Ceiling((double)totalUsers / pageSize);

        //        var userDtos = totalUsers > 0 ? _mapper.Map<List<UserListForDoetDTO>>(users).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
        //                                        : new List<UserListForDoetDTO>();

        //        var pagedResponse = new PagedResponse<List<UserListForDoetDTO>>
        //        {
        //            Items = userDtos,
        //            TotalCount = totalUsers,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = totalPages
        //        };


        //        return new DataResponse<PagedResponse<List<UserListForDoetDTO>>>
        //        {
        //            Data = pagedResponse,
        //            Message = "User list retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<PagedResponse<List<UserListForDoetDTO>>>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<PagedResponse<List<UserListForDoetDTO>>>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get user list: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<PagedResponse<List<UserListForDoetDTO>>>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving user list: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<UserDetailForDoetDTO>> GetUserDetailByIdForDoetAsync(int userId)
        //{
        //    try
        //    {
        //        // Lấy danh sách người dùng đã xóa
        //        var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);

        //        // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
        //        if (deletedUsers.Any(u => u.UserId == userId))
        //        {
        //            throw new KeyNotFoundException("User is not in the list.");
        //        }

        //        var user = await _userRepository.GetUserByIdForDoetAsync(userId);

        //        if (user.Status == "Deleted")
        //        {
        //            throw new KeyNotFoundException("User is deleted.");
        //        }

        //        var userDto = _mapper.Map<UserDetailForDoetDTO>(user);

        //        return new DataResponse<UserDetailForDoetDTO>
        //        {
        //            Data = userDto,
        //            Message = "User details retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<UserDetailForDoetDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UserDetailForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get user detail: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UserDetailForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving user details: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<AddUserForDoetDTO>> AddUserForDoetAsync(AddUserForDoetDTO addUserForDoetDTO)
        //{
        //    try
        //    {
        //        // Tạo người dùng mới
        //        var user = new User
        //        {
        //            Email = addUserForDoetDTO.Email,
        //            RoleId = addUserForDoetDTO.RoleId,
        //            Name = addUserForDoetDTO.Name,
        //            UserCode = addUserForDoetDTO.UserCode,
        //            Information = addUserForDoetDTO.Information
        //        };

        //        // Thêm người dùng vào cơ sở dữ liệu
        //        var addUserResult = await _userRepository.AddUserForDoetAsync(user);

        //        // Cập nhật thời gian tạo vào DTO trả về
        //        addUserForDoetDTO.CreatedAt = addUserResult.CreatedAt;

        //        return new DataResponse<AddUserForDoetDTO>
        //        {
        //            Data = addUserForDoetDTO,
        //            Message = "User added successfully!",
        //            StatusCode = 201
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp email đã tồn tại
        //        return new DataResponse<AddUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<AddUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while add user: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<AddUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Error adding user: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<UpdateUserForDoetDTO>> UpdateUserForDoetAsync(UpdateUserForDoetDTO updateUserForDoetDTO)
        //{
        //    try
        //    {
        //        // Tạo đối tượng người dùng mới từ DTO
        //        var user = new User
        //        {
        //            UserId = updateUserForDoetDTO.UserId,
        //            Email = updateUserForDoetDTO.Email,
        //            //RoleId = updateUserForDoetDTO.RoleId,
        //            Name = updateUserForDoetDTO.Name,
        //            UserCode = updateUserForDoetDTO.UserCode,
        //            Information = updateUserForDoetDTO.Information
        //        };

        //        // Cập nhật thông tin người dùng trong cơ sở dữ liệu
        //        var updatedUserResult = await _userRepository.UpdateUserForDoetAsync(user);

        //        var userDto = _mapper.Map<UpdateUserForDoetDTO>(updatedUserResult);

        //        return new DataResponse<UpdateUserForDoetDTO>
        //        {
        //            Data = userDto,
        //            Message = "User updated successfully!",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<UpdateUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp email hoặc UserCode đã tồn tại
        //        return new DataResponse<UpdateUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UpdateUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while update user: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UpdateUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Error updating user: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<UpdateUserStatusForDoetDTO>> UpdateUserStatusForDoetAsync(UpdateUserStatusForDoetDTO updateUserStatusForDoetDTO)
        //{
        //    try
        //    {
        //        var existingUser = await _userRepository.GetUserByIdForDoetAsync(updateUserStatusForDoetDTO.UserId);

        //        if (existingUser.Role != null && existingUser.Role.Name == "DOET")
        //        {
        //            throw new InvalidOperationException("Cannot update the status of a user with the 'DOET' role.");
        //        }

        //        // Tạo đối tượng người dùng mới từ DTO
        //        var user = new User
        //        {
        //            UserId = updateUserStatusForDoetDTO.UserId,
        //            Status = updateUserStatusForDoetDTO.Status
        //        };

        //        // Cập nhật thông tin người dùng trong cơ sở dữ liệu
        //        var updatedUserStatusResult = await _userRepository.UpdateUserForDoetAsync(user);

        //        var userDto = _mapper.Map<UpdateUserStatusForDoetDTO>(updatedUserStatusResult);

        //        return new DataResponse<UpdateUserStatusForDoetDTO>
        //        {
        //            Data = userDto,
        //            Message = "User updated successfully!",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<UpdateUserStatusForDoetDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp email hoặc UserCode đã tồn tại
        //        return new DataResponse<UpdateUserStatusForDoetDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UpdateUserStatusForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while update user status: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UpdateUserStatusForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Error updating user: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<DeleteUserForDoetDTO>> SoftDeleteUserForDoetAsync(DeleteUserForDoetDTO deleteUserForDoetDTO)
        //{
        //    try
        //    {
        //        // Xóa người dùng trong cơ sở dữ liệu
        //        var deletedUserResult = await _userRepository.SoftDeleteUserForDoetAsync(deleteUserForDoetDTO.UserId);

        //        var userDto = _mapper.Map<DeleteUserForDoetDTO>(deletedUserResult);

        //        return new DataResponse<DeleteUserForDoetDTO>
        //        {
        //            Data = userDto,
        //            Message = "User has been marked as deleted successfully.",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<DeleteUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<DeleteUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while soft delete user: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<DeleteUserForDoetDTO>
        //        {
        //            Data = null,
        //            Message = $"Error marking user as deleted: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<MemoryStream>> GenerateUserTemplateForDoetAsync()
        //{
        //    try
        //    {
        //        // Tạo một MemoryStream mới để lưu trữ template
        //        var memoryStream = new MemoryStream();

        //        // Sử dụng một thư viện như EPPlus, ClosedXML, hoặc NPOI để tạo file Excel
        //        using (var package = new ExcelPackage(memoryStream))
        //        {
        //            var worksheet = package.Workbook.Worksheets.Add("User Template For Doet");

        //            // Thêm tiêu đề cột
        //            worksheet.Cells[1, 1].Value = "Email(*)";
        //            worksheet.Cells[1, 2].Value = "FullName(*)";
        //            worksheet.Cells[1, 3].Value = "UserCode(*)";
        //            worksheet.Cells[1, 4].Value = "RoleId(*)";
        //            worksheet.Cells[1, 5].Value = "Information";
        //            worksheet.Cells[1, 6].Value = "MajorCode(* : Bắt buộc với student)";

        //            // Định dạng tiêu đề cột
        //            for (int col = 1; col <= 6; col++)
        //            {
        //                worksheet.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                worksheet.Cells[1, col].Style.Font.Bold = true; // Đặt chữ in đậm
        //            }

        //            // Thêm dữ liệu mẫu student
        //            worksheet.Cells[2, 1].Value = "datnthe163935@fpt.edu.vn"; // Email
        //            worksheet.Cells[2, 2].Value = "Nguyễn Tiến Đạt"; // Full Name
        //            worksheet.Cells[2, 3].Value = "HE163935"; // User Code
        //            worksheet.Cells[2, 4].Value = 2; // Role ID
        //            worksheet.Cells[2, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information
        //            worksheet.Cells[2, 6].Value = "SE"; // MajorCode

        //            // Thêm dữ liệu mẫu cho dean
        //            worksheet.Cells[3, 1].Value = "nguyenvananh123@fe.edu.vn"; // Email
        //            worksheet.Cells[3, 2].Value = "Nguyễn Văn Anh"; // Full Name
        //            worksheet.Cells[3, 3].Value = "AnhVN123"; // User Code
        //            worksheet.Cells[3, 4].Value = 5; // Role ID
        //            worksheet.Cells[3, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

        //            // Thêm dữ liệu mẫu cho lecturer
        //            worksheet.Cells[4, 1].Value = "nguyenvanchien456@fe.edu.vn"; // Email
        //            worksheet.Cells[4, 2].Value = "Nguyễn Văn Chiến"; // Full Name
        //            worksheet.Cells[4, 3].Value = "chienVN456"; // User Code
        //            worksheet.Cells[4, 4].Value = 6; // Role ID
        //            worksheet.Cells[4, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

        //            // Thêm dữ liệu mẫu cho company
        //            worksheet.Cells[5, 1].Value = "congtyFPT@gmail.com"; // Email
        //            worksheet.Cells[5, 2].Value = "Công ty FPT"; // Full Name
        //            worksheet.Cells[5, 3].Value = "FPT (Viết tắt tên công ty)"; // User Code
        //            worksheet.Cells[5, 4].Value = 3; // Role ID
        //            worksheet.Cells[5, 5].Value = "SĐT: 123456789, Địa chỉ: Hà Nội"; // Information

        //            // Thêm tiêu đề "Hướng dẫn điền Role" chiếm 2 cột
        //            worksheet.Cells[4, 7].Value = "Hướng dẫn điền Role";
        //            worksheet.Cells[4, 7, 4, 8].Merge = true; // Ghép ô từ F4 đến G4
        //            worksheet.Cells[4, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //            worksheet.Cells[4, 7].Style.Font.Bold = true; // Đặt chữ in đậm

        //            // Thêm tiêu đề cho bảng hướng dẫn
        //            worksheet.Cells[5, 7].Value = "Role ID";
        //            worksheet.Cells[5, 8].Value = "Role Name";

        //            // Định dạng tiêu đề Role ID và Role Name
        //            worksheet.Cells[5, 7, 5, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //            worksheet.Cells[5, 7, 5, 8].Style.Font.Bold = true; // Đặt chữ in đậm

        //            // Thêm dữ liệu mẫu cho các role
        //            worksheet.Cells[6, 7].Value = 2; // Role ID
        //            worksheet.Cells[6, 8].Value = "Student"; // Role Name

        //            worksheet.Cells[7, 7].Value = 3; // Role ID
        //            worksheet.Cells[7, 8].Value = "Company"; // Role Name

        //            worksheet.Cells[8, 7].Value = 5; // Role ID
        //            worksheet.Cells[8, 8].Value = "Dean"; // Role Name

        //            worksheet.Cells[9, 7].Value = 6; // Role ID
        //            worksheet.Cells[9, 8].Value = "Lecturer"; // Role Name

        //            // Định dạng bảng hướng dẫn
        //            worksheet.Cells[4, 7, 9, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //            worksheet.Cells[5, 7, 9, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //            // Căn giữa cho tất cả các ô
        //            for (int row = 2; row <= 9; row++)
        //            {
        //                for (int col = 1; col <= 6; col++)
        //                {
        //                    worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                }
        //            }

        //            // Kéo dãn cột
        //            worksheet.Column(1).Width = 30; // Email
        //            worksheet.Column(2).Width = 25; // Full Name
        //            worksheet.Column(3).Width = 27; // User Code
        //            worksheet.Column(4).Width = 10; // Role ID
        //            worksheet.Column(5).Width = 60; // Information
        //            worksheet.Column(6).Width = 40; // MajorCode
        //            worksheet.Column(7).Width = 15; // Role ID (guidance)
        //            worksheet.Column(8).Width = 20; // Role Name (guidance)

        //            // Thêm ghi chú cho các trường
        //            worksheet.Cells[13, 7].Value = "Ghi chú:"; // Cột 6
        //            worksheet.Cells[14, 7].Value = "(*) : Bắt buộc điền."; // Cột 6
        //            worksheet.Cells[15, 7].Value = "Giới hạn UserCode <= 50 ký tự."; // Cột 6
        //            worksheet.Cells[16, 7].Value = "Email cần đúng định dạng (vd: example@mail.com)."; // Cột 6
        //            worksheet.Cells[17, 7].Value = "Role: Phải là số theo mẫu và dựa vào template."; // Cột 6
        //            worksheet.Cells[18, 7].Value = "MajorCode: Bắt buộc với role Student (2), là mã của ngành học."; // Ghi chú cho cột Major

        //            // Định dạng ghi chú
        //            for (int row = 13; row <= 18; row++)
        //            {
        //                worksheet.Cells[row, 7].Style.Font.Bold = true;
        //                worksheet.Cells[row, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
        //            }

        //            // Lưu file vào MemoryStream
        //            package.Save();
        //        }

        //        // Đặt lại vị trí của MemoryStream về đầu
        //        memoryStream.Position = 0;

        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = memoryStream,
        //            Message = "Template generated successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (IOException ioEx)
        //    {
        //        // Xử lý lỗi liên quan đến I/O
        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = null,
        //            Message = $"I/O error while generating template: {ioEx.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = null,
        //            Message = $"Access denied while generating template: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (OutOfMemoryException memEx)
        //    {
        //        // Xử lý lỗi không đủ bộ nhớ
        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = null,
        //            Message = $"Out of memory while generating template: {memEx.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<MemoryStream>
        //        {
        //            Data = null,
        //            Message = $"Error generating template: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<object>> ImportUsersForDoetAsync(IFormFile file)
        //{
        //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //    if (file == null || file.Length == 0)
        //    {
        //        return new DataResponse<object>
        //        {
        //            Data = null,
        //            Message = "File is empty or not provided.",
        //            StatusCode = 400
        //        };
        //    }

        //    try
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //                var users = new List<User>();
        //                var errorMessages = new List<string>();

        //                // Xác định số hàng cuối cùng trong phạm vi từ cột A đến cột E
        //                int lastRow = 1;
        //                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        //                {
        //                    bool hasDataInRange = false;
        //                    for (int col = 1; col <= 5; col++) // Chỉ từ cột A đến E (1 đến 5)
        //                    {
        //                        if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col]?.Text))
        //                        {
        //                            hasDataInRange = true;
        //                            break;
        //                        }
        //                    }
        //                    if (hasDataInRange)
        //                    {
        //                        lastRow = row;
        //                    }
        //                }

        //                // Tạo dictionary để lưu mối quan hệ giữa email và majorId
        //                var majorMapping = new Dictionary<string, int?>(); // Lưu email và MajorId tương ứng

        //                for (int row = 2; row <= lastRow; row++)
        //                {
        //                    var email = worksheet.Cells[row, 1].Value?.ToString().Trim();
        //                    var fullName = worksheet.Cells[row, 2].Value?.ToString().Trim();
        //                    var userCode = worksheet.Cells[row, 3].Value?.ToString().Trim();
        //                    var roleIdValue = worksheet.Cells[row, 4].Value?.ToString().Trim();
        //                    var information = worksheet.Cells[row, 5].Value?.ToString().Trim();
        //                    var majorCode = worksheet.Cells[row, 6].Value?.ToString().Trim();

        //                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(roleIdValue))
        //                    {
        //                        errorMessages.Add($"Row {row}: Missing required data.");
        //                        continue;
        //                    }

        //                    // Kiểm tra email hợp lệ
        //                    if (!IsValidEmail(email))
        //                    {
        //                        errorMessages.Add($"Row {row}: Invalid email format.");
        //                        continue;
        //                    }

        //                    // Kiểm tra độ dài của UserCode
        //                    if (userCode.Length > 50)
        //                    {
        //                        errorMessages.Add($"Row {row}: UserCode must not exceed 50 characters.");
        //                        continue;
        //                    }

        //                    // Kiểm tra và parse RoleId
        //                    if (!int.TryParse(roleIdValue, out int roleId) || !(new[] { 2, 3, 5, 6, 7 }.Contains(roleId)))
        //                    {
        //                        errorMessages.Add($"Row {row}: RoleId must follow the template and cannot be a letter. It must be one of the following values: 2, 3, 5, 6.");
        //                        continue;
        //                    }

        //                    // Nếu RoleId là Student, kiểm tra Major
        //                    int? majorId = null;
        //                    if (roleId == 2)
        //                    {
        //                        if (string.IsNullOrWhiteSpace(majorCode))
        //                        {
        //                            errorMessages.Add($"Row {row}: Major is required for RoleId = 2 (Student).");
        //                            continue;
        //                        }

        //                        // Kiểm tra Major Code có tồn tại trong bảng Major không
        //                        var major = await _majorRepository.GetMajorByCodeAsync(majorCode);
        //                        if (major == null)
        //                        {
        //                            errorMessages.Add($"Row {row}: Major code '{majorCode}' does not exist.");
        //                            continue;
        //                        }

        //                        // Kiểm tra trạng thái Active của Major
        //                        if (major.Status.ToLower() != "active")
        //                        {
        //                            errorMessages.Add($"Row {row}: Major code '{majorCode}' is not active and cannot be assigned to students.");
        //                            continue;
        //                        }

        //                        majorId = major.MajorId; // Lấy MajorId từ bảng Major
        //                    }

        //                    // Lưu MajorId vào dictionary
        //                    majorMapping[email] = majorId;

        //                    // Kiểm tra nếu UserCode hoặc Email đã tồn tại
        //                    var isUserCodeExists = await _userRepository.IsUserCodeExistsAsync(userCode);
        //                    var isEmailExists = await _userRepository.IsEmailExistsAsync(email);

        //                    if (isUserCodeExists)
        //                    {
        //                        errorMessages.Add($"Row {row}: UserCode '{userCode}' already exists.");
        //                        continue;
        //                    }

        //                    if (isEmailExists)
        //                    {
        //                        errorMessages.Add($"Row {row}: Email '{email}' already exists.");
        //                        continue;
        //                    }

        //                    var user = new User
        //                    {
        //                        Email = email,
        //                        Name = fullName,
        //                        UserCode = userCode,
        //                        RoleId = roleId,
        //                        Information = information,
        //                        Status = "Active",
        //                        CreatedAt = DateTime.Now,
        //                        UpdatedAt = DateTime.Now
        //                    };

        //                    users.Add(user);
        //                }

        //                // Nếu có bất kỳ lỗi nào, trả về lỗi và không thêm vào DB
        //                if (errorMessages.Any())
        //                {
        //                    return new DataResponse<object>
        //                    {
        //                        Data = new
        //                        {
        //                            SuccessCount = 0,
        //                            ErrorCount = errorMessages.Count,
        //                            Errors = errorMessages
        //                        },
        //                        Message = $"Import failed. There were {errorMessages.Count} errors. Please fix the reported errors to successfully add the file.",
        //                        StatusCode = 400
        //                    };
        //                }

        //                // Thực hiện thêm các bản ghi hợp lệ
        //                await _userRepository.AddUsersForDoetAsync(users);

        //                var studentUsers = new List<Student>();
        //                var companyUsers = new List<Company>();

        //                foreach (var user in users)
        //                {
        //                    // Lấy lại UserId sau khi lưu
        //                    var addedUser = await _userRepository.GetUserByEmailAsync(user.Email);

        //                    if (addedUser != null)
        //                    {
        //                        if (addedUser.RoleId == 2) // RoleId = 2 là Student
        //                        {
        //                            var majorId = majorMapping[user.Email]; // Lấy MajorId từ dictionary
        //                            studentUsers.Add(new Student { UserId = addedUser.UserId, MajorId = majorId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
        //                        }
        //                        else if (addedUser.RoleId == 3) // RoleId = 3 là Company
        //                        {
        //                            companyUsers.Add(new Company { UserId = addedUser.UserId, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
        //                        }
        //                    }
        //                }

        //                // Thực hiện thêm các bản ghi Student và Company nếu có
        //                await _userRepository.AddStudentsAndCompaniesForDoetAsync(studentUsers, companyUsers);

        //                var successCount = users.Count;

        //                var resultMessage = $"Import completed. Successfully added {successCount} users.";

        //                return new DataResponse<object>
        //                {
        //                    Data = new
        //                    {
        //                        SuccessCount = successCount,
        //                        ErrorCount = 0,
        //                        Errors = errorMessages
        //                    },
        //                    Message = resultMessage,
        //                    StatusCode = 200
        //                };
        //            }
        //        }
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<object>
        //        {
        //            Data = null,
        //            Message = $"Access denied while importing users: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<object>
        //        {
        //            Data = null,
        //            Message = $"Error importing users: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}


        //public async Task<DataResponse<List<StatusUserListForDoetDTO>>> GetAllStatusesUserForDoetAsync()
        //{
        //    try
        //    {
        //        // Tạo danh sách trạng thái
        //        var statuses = new List<StatusUserListForDoetDTO>
        //        {
        //            new StatusUserListForDoetDTO { Status = "Active" },
        //            new StatusUserListForDoetDTO { Status = "Inactive" }
        //        };

        //        return new DataResponse<List<StatusUserListForDoetDTO>>
        //        {
        //            Data = statuses,
        //            Message = "Status List retrieved successfully!",
        //            StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<List<StatusUserListForDoetDTO>>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<List<StatusUserListForDoetDTO>>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get status list: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý ngoại lệ nếu có
        //        return new DataResponse<List<StatusUserListForDoetDTO>>
        //        {
        //            Data = null,
        //            Message = $"Error occurred while retrieving statuses: {ex.Message}",
        //            StatusCode = 500 // Lỗi server
        //        };
        //    }
        //}

        //// Doet - UserManagement
        //public async Task<DataResponse<PagedResponse<List<UserListForCompanyDTO>>>> GetAllUsersForCompanyAsync(int companyId, string? name, int? roleId, string? status, int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        var users = await _userRepository.GetAllUsersForCompanyAsync(companyId, name, roleId, status);

        //        var totalUsers = users.Count();
        //        var totalPages = totalUsers == 0 ? 1 : (int)Math.Ceiling((double)totalUsers / pageSize);

        //        var userDtos = totalUsers > 0 ? _mapper.Map<List<UserListForCompanyDTO>>(users).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
        //                                        : new List<UserListForCompanyDTO>();

        //        var pagedResponse = new PagedResponse<List<UserListForCompanyDTO>>
        //        {
        //            Items = userDtos,
        //            TotalCount = totalUsers,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = totalPages
        //        };


        //        return new DataResponse<PagedResponse<List<UserListForCompanyDTO>>>
        //        {
        //            Data = pagedResponse,
        //            Message = "User list retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<PagedResponse<List<UserListForCompanyDTO>>>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<PagedResponse<List<UserListForCompanyDTO>>>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get user list: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<PagedResponse<List<UserListForCompanyDTO>>>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving user list: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<UserDetailForCompanyDTO>> GetUserDetailByIdForCompanyAsync(int companyId, int userId)
        //{
        //    try
        //    {
        //        // Lấy danh sách người dùng đã xóa
        //        var deletedUsers = await _userRepository.GetAllUsersStoredAsync(null, null);

        //        // Kiểm tra nếu userId có trong danh sách người dùng đã xóa
        //        if (deletedUsers.Any(u => u.UserId == userId))
        //        {
        //            throw new KeyNotFoundException("User is not in the list.");
        //        }

        //        var user = await _userRepository.GetUserByIdForCompanyAsync(companyId, userId);

        //        if (user.Status == "Deleted")
        //        {
        //            throw new KeyNotFoundException("User is deleted.");
        //        }

        //        var userDto = _mapper.Map<UserDetailForCompanyDTO>(user);

        //        return new DataResponse<UserDetailForCompanyDTO>
        //        {
        //            Data = userDto,
        //            Message = "User details retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<UserDetailForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UserDetailForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get user detail: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UserDetailForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving user details: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<AddUserForCompanyDTO>> AddUserForCompanyAsync(int companyId, AddUserForCompanyDTO addUserForCompanyDTO)
        //{
        //    try
        //    {
        //        // Tạo người dùng mới
        //        var user = new User
        //        {
        //            Email = addUserForCompanyDTO.Email,
        //            Name = addUserForCompanyDTO.Name,
        //            Information = addUserForCompanyDTO.Information,
        //            ForCompany = companyId,
        //            RoleId = 7
        //        };

        //        // Thêm người dùng vào cơ sở dữ liệu
        //        var addUserResult = await _userRepository.AddUserForCompanyAsync(companyId, user);

        //        // Cập nhật thời gian tạo vào DTO trả về
        //        addUserForCompanyDTO.CreatedAt = addUserResult.CreatedAt;
        //        addUserForCompanyDTO.RoleId = addUserResult.RoleId.Value;
        //        addUserForCompanyDTO.ForCompany = addUserResult.ForCompany;
        //        addUserForCompanyDTO.UserCode = addUserResult.UserCode;

        //        return new DataResponse<AddUserForCompanyDTO>
        //        {
        //            Data = addUserForCompanyDTO,
        //            Message = "User added successfully!",
        //            StatusCode = 201
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp email đã tồn tại
        //        return new DataResponse<AddUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<AddUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while add user: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<AddUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Error adding user: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<UpdateUserForCompanyDTO>> UpdateUserForCompanyAsync(int companyId, UpdateUserForCompanyDTO updateUserForCompanyDTO)
        //{
        //    try
        //    {
        //        // Tạo đối tượng người dùng mới từ DTO
        //        var user = new User
        //        {
        //            UserId = updateUserForCompanyDTO.UserId,
        //            Email = updateUserForCompanyDTO.Email,
        //            Name = updateUserForCompanyDTO.Name,
        //            Information = updateUserForCompanyDTO.Information
        //        };

        //        // Cập nhật thông tin người dùng trong cơ sở dữ liệu
        //        var updatedUserResult = await _userRepository.UpdateUserForCompanyAsync(companyId, user);

        //        var userDto = _mapper.Map<UpdateUserForCompanyDTO>(updatedUserResult);

        //        return new DataResponse<UpdateUserForCompanyDTO>
        //        {
        //            Data = userDto,
        //            Message = "User updated successfully!",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<UpdateUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp email hoặc UserCode đã tồn tại
        //        return new DataResponse<UpdateUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UpdateUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while update user: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UpdateUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Error updating user: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<UpdateUserStatusForCompanyDTO>> UpdateUserStatusForCompanyAsync(int companyId, UpdateUserStatusForCompanyDTO updateUserStatusForCompanyDTO)
        //{
        //    try
        //    {
        //        var existingUser = await _userRepository.GetUserByIdForCompanyAsync(companyId, updateUserStatusForCompanyDTO.UserId);

        //        if (existingUser.Role != null && existingUser.Role.Name == "Company")
        //        {
        //            throw new InvalidOperationException("Cannot update the status of a user with the 'Company' role.");
        //        }

        //        // Tạo đối tượng người dùng mới từ DTO
        //        var user = new User
        //        {
        //            UserId = updateUserStatusForCompanyDTO.UserId,
        //            Status = updateUserStatusForCompanyDTO.Status
        //        };

        //        // Cập nhật thông tin người dùng trong cơ sở dữ liệu
        //        var updatedUserStatusResult = await _userRepository.UpdateUserForCompanyAsync(companyId, user);

        //        var userDto = _mapper.Map<UpdateUserStatusForCompanyDTO>(updatedUserStatusResult);

        //        return new DataResponse<UpdateUserStatusForCompanyDTO>
        //        {
        //            Data = userDto,
        //            Message = "User updated successfully!",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<UpdateUserStatusForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // Trường hợp email hoặc UserCode đã tồn tại
        //        return new DataResponse<UpdateUserStatusForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message, // Sử dụng thông báo lỗi từ InvalidOperationException
        //            StatusCode = 400 // Bad Request
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<UpdateUserStatusForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while update user status: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UpdateUserStatusForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Error updating user: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}

        //public async Task<DataResponse<DeleteUserForCompanyDTO>> SoftDeleteUserForCompanyAsync(int companyId, DeleteUserForCompanyDTO deleteUserForCompanyDTO)
        //{
        //    try
        //    {
        //        // Xóa người dùng trong cơ sở dữ liệu
        //        var deletedUserResult = await _userRepository.SoftDeleteUserForCompanyAsync(companyId, deleteUserForCompanyDTO.UserId);

        //        var userDto = _mapper.Map<DeleteUserForCompanyDTO>(deletedUserResult);

        //        return new DataResponse<DeleteUserForCompanyDTO>
        //        {
        //            Data = userDto,
        //            Message = "User has been marked as deleted successfully.",
        //            StatusCode = 200 // OK
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        // Trường hợp user ko tồn tại
        //        return new DataResponse<DeleteUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<DeleteUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Access denied while soft delete user: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<DeleteUserForCompanyDTO>
        //        {
        //            Data = null,
        //            Message = $"Error marking user as deleted: {ex.Message}",
        //            StatusCode = 500 // Internal Server Error
        //        };
        //    }
        //}


        //public async Task<DataResponse<List<StatusUserListForCompanyDTO>>> GetAllStatusesUserForCompanyAsync()
        //{
        //    try
        //    {
        //        // Tạo danh sách trạng thái
        //        var statuses = new List<StatusUserListForCompanyDTO>
        //        {
        //            new StatusUserListForCompanyDTO { Status = "Active" },
        //            new StatusUserListForCompanyDTO { Status = "Inactive" }
        //        };

        //        return new DataResponse<List<StatusUserListForCompanyDTO>>
        //        {
        //            Data = statuses,
        //            Message = "Status List retrieved successfully!",
        //            StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<List<StatusUserListForCompanyDTO>>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (UnauthorizedAccessException authEx)
        //    {
        //        // Xử lý lỗi quyền truy cập
        //        return new DataResponse<List<StatusUserListForCompanyDTO>>
        //        {
        //            Data = null,
        //            Message = $"Access denied while get status list: {authEx.Message}",
        //            StatusCode = 403 // Forbidden
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý ngoại lệ nếu có
        //        return new DataResponse<List<StatusUserListForCompanyDTO>>
        //        {
        //            Data = null,
        //            Message = $"Error occurred while retrieving statuses: {ex.Message}",
        //            StatusCode = 500 // Lỗi server
        //        };
        //    }
        //}

        ////For Dean
        //public async Task<DataResponse<UserProfileDto>> ViewProfileAsync()
        //{
        //    try
        //    {
        //        var userId = GetCurrentUserId();
        //        var role = GetCurrentUserRole();

        //        User user = null;
        //        if (role == "Dean")
        //        {
        //            user = await _userRepository.GetDeanByUserIdAsync(userId);
        //        }
        //        else if (role == "Lecturer")
        //        {
        //            user = await _userRepository.GetLecturerByUserIdAsync(userId);
        //        }

        //        if (user == null)
        //        {
        //            return new DataResponse<UserProfileDto>
        //            {
        //                Data = null,
        //                Message = "User not found.",
        //                StatusCode = 404
        //            };
        //        }

        //        var userProfileDto = _mapper.Map<UserProfileDto>(user);

        //        return new DataResponse<UserProfileDto>
        //        {
        //            Data = userProfileDto,
        //            Message = "Profile retrieved successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<UserProfileDto>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving profile: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //// UpdateProfileAsync (for Dean and Lecturer)
        //public async Task<DataResponse<string>> UpdateProfileAsync(UpdateProfileDto dto)
        //{
        //    try
        //    {
        //        var userId = GetCurrentUserId();
        //        var role = GetCurrentUserRole();

        //        User user = null;
        //        if (role == "Dean")
        //        {
        //            user = await _userRepository.GetDeanByUserIdAsync(userId);
        //        }
        //        else if (role == "Lecturer")
        //        {
        //            user = await _userRepository.GetLecturerByUserIdAsync(userId);
        //        }

        //        if (user == null)
        //        {
        //            return new DataResponse<string>
        //            {
        //                Data = null,
        //                Message = "User not found.",
        //                StatusCode = 404
        //            };
        //        }

        //        user.Name = dto.Name;
        //        user.Information = dto.Information;
        //        user.UpdatedAt = DateTime.Now;

        //        if (role == "Dean")
        //        {
        //            await _userRepository.UpdateDeanAsync(user);
        //        }
        //        else if (role == "Lecturer")
        //        {
        //            await _userRepository.UpdateLecturerAsync(user);
        //        }

        //        return new DataResponse<string>
        //        {
        //            Data = "Success",
        //            Message = "Profile updated successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = $"Error updating profile: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //// CreateLecturerAsync
        //public async Task<DataResponse<string>> CreateLecturerAsync(CreateLecturerDto dto)
        //{
        //    try
        //    {
        //        var assignForId = GetCurrentUserId();

        //        var existingUser = await _userRepository.GetLecturerByEmailForDeanAsync(dto.Email);
        //        if (existingUser != null)
        //        {
        //            return new DataResponse<string>
        //            {
        //                Data = null,
        //                Message = "Email already in use.",
        //                StatusCode = 400
        //            };
        //        }

        //        var roleId = await _userRepository.GetRoleIdByNameAsync("Lecturer");
        //        if (roleId == 0)
        //        {
        //            return new DataResponse<string>
        //            {
        //                Data = null,
        //                Message = "Role 'Lecturer' not found.",
        //                StatusCode = 404
        //            };
        //        }

        //        var newLecturer = new User
        //        {
        //            Email = dto.Email,
        //            Name = dto.Name,
        //            RoleId = roleId,
        //            Status = "Active",
        //            AssignForId = assignForId,
        //            CreatedAt = DateTime.Now,
        //            UpdatedAt = DateTime.Now
        //        };

        //        await _userRepository.CreateLecturerForDeanAsync(newLecturer);

        //        return new DataResponse<string>
        //        {
        //            Data = "Lecturer created successfully.",
        //            Message = "Lecturer created successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = $"Error: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //// GetLecturerListForDeanAsync
        //public async Task<DataResponse<PagedResponse<List<LecturerListDto>>>> GetLecturerListForDeanAsync(
        //string? name,
        //string? userCode,
        //string? majorName,
        //string? sortBy,
        //bool isDescending,
        //int pageNumber,
        //int pageSize)
        //{
        //    try
        //    {
        //        var assignForId = GetCurrentUserId();

        //        // Lấy danh sách lecturers từ repository
        //        var lecturers = await _userRepository.GetLecturerListForDeanAsync(assignForId, name, userCode, majorName, sortBy, isDescending);

        //        if (lecturers == null || !lecturers.Any())
        //        {
        //            return new DataResponse<PagedResponse<List<LecturerListDto>>>
        //            {
        //                Data = null,
        //                Message = "No lecturers found.",
        //                StatusCode = 404
        //            };
        //        }

        //        // Phân trang
        //        var totalLecturers = lecturers.Count();
        //        var totalPages = (int)Math.Ceiling((double)totalLecturers / pageSize);

        //        var paginatedLecturers = lecturers
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();

        //        var lecturerDtos = _mapper.Map<List<LecturerListDto>>(paginatedLecturers);

        //        var pagedResponse = new PagedResponse<List<LecturerListDto>>
        //        {
        //            Items = lecturerDtos,
        //            TotalCount = totalLecturers,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = totalPages
        //        };

        //        return new DataResponse<PagedResponse<List<LecturerListDto>>>
        //        {
        //            Data = pagedResponse,
        //            Message = "Lecturer list retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<PagedResponse<List<LecturerListDto>>>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving lecturer list: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}


        //// GetLecturerDetailsAsync
        //public async Task<DataResponse<LecturerDetailsDto>> GetLecturerDetailsAsync(
        //int lecturerId,
        //string? studentName,
        //string? lecturerName,
        //string? semesterName,
        //string? sortBy,
        //bool? isDescending,
        //int pageNumber,
        //int pageSize)
        //{
        //    try
        //    {
        //        // Lấy thông tin Lecturer, Dean, và danh sách sinh viên
        //        var (lecturer, dean, students) = await _userRepository.GetLecturerDetailsWithDeanAndStudentsAsync(lecturerId);

        //        if (lecturer == null)
        //        {
        //            return new DataResponse<LecturerDetailsDto>
        //            {
        //                Data = null,
        //                Message = "Lecturer not found.",
        //                StatusCode = 404
        //            };
        //        }

        //        // Áp dụng tìm kiếm
        //        if (!string.IsNullOrWhiteSpace(studentName))
        //        {
        //            studentName = studentName.ToLower();
        //            students = students.Where(s => s.User.Name.ToLower().Contains(studentName)).ToList();
        //        }

        //        if (!string.IsNullOrWhiteSpace(lecturerName))
        //        {
        //            lecturerName = lecturerName.ToLower();
        //            students = students.Where(s => s.Lecturer.Name.ToLower().Contains(lecturerName)).ToList();
        //        }

        //        if (!string.IsNullOrWhiteSpace(semesterName))
        //        {
        //            semesterName = semesterName.ToLower();
        //            students = students.Where(s => s.Semester.Name.ToLower().Contains(semesterName)).ToList();
        //        }

        //        // Áp dụng sắp xếp
        //        switch (sortBy?.ToLower())
        //        {
        //            case "name":
        //                students = isDescending.HasValue && isDescending.Value
        //                    ? students.OrderByDescending(s => s.User.Name).ToList()
        //                    : students.OrderBy(s => s.User.Name).ToList();
        //                break;
        //            case "lecturername":
        //                students = isDescending.HasValue && isDescending.Value
        //                    ? students.OrderByDescending(s => s.Lecturer.Name).ToList()
        //                    : students.OrderBy(s => s.Lecturer.Name).ToList();
        //                break;
        //            case "semestername":
        //                students = isDescending.HasValue && isDescending.Value
        //                    ? students.OrderByDescending(s => s.Semester.Name).ToList()
        //                    : students.OrderBy(s => s.Semester.Name).ToList();
        //                break;
        //            default:
        //                students = students.OrderBy(s => s.User.Name).ToList();
        //                break;
        //        }

        //        // Phân trang
        //        var totalStudents = students.Count;
        //        var totalPages = (int)Math.Ceiling((double)totalStudents / pageSize);

        //        var paginatedStudents = students
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();

        //        // Mapping dữ liệu
        //        var studentDtos = _mapper.Map<List<StudentListDto>>(paginatedStudents);

        //        var lecturerDetails = _mapper.Map<LecturerDetailsDto>(lecturer);
        //        lecturerDetails.DeanName = dean?.Name; // Gán Dean Name
        //        lecturerDetails.Students = new PagedResponse<List<StudentListDto>>
        //        {
        //            Items = studentDtos,
        //            TotalCount = totalStudents,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = totalPages
        //        };

        //        return new DataResponse<LecturerDetailsDto>
        //        {
        //            Data = lecturerDetails,
        //            Message = "Lecturer details retrieved successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<LecturerDetailsDto>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving lecturer details: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}
        //private int GetCurrentUserId()
        //{
        //    var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        //    {
        //        throw new UnauthorizedAccessException("User not authenticated.");
        //    }

        //    return userId;
        //}

        //private string GetCurrentUserRole()
        //{
        //    var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

        //    if (string.IsNullOrEmpty(roleClaim))
        //    {
        //        throw new UnauthorizedAccessException("User role not found.");
        //    }

        //    return roleClaim;
        //}

        //public async Task<DataResponse<PagedResponse<List<DeanListForAdminDOETDto>>>> GetAllDeansAsync(
        //string? userCode,
        //string? name,
        //string? departmentName,
        //int pageNumber,
        //int pageSize,
        //string? sortBy,
        //bool? isDescending)
        //{
        //    try
        //    {
        //        // Gọi repository
        //        var deans = await _userRepository.GetAllDeansAsync(userCode, name, departmentName, sortBy, isDescending);

        //        if (deans == null || !deans.Any())
        //        {
        //            return new DataResponse<PagedResponse<List<DeanListForAdminDOETDto>>>
        //            {
        //                Data = null,
        //                Message = "No deans found.",
        //                StatusCode = 404
        //            };
        //        }

        //        // Tính toán phân trang
        //        var totalDeans = deans.Count;
        //        var totalPages = (int)Math.Ceiling((double)totalDeans / pageSize);

        //        // Lấy dữ liệu theo phân trang
        //        var paginatedDeans = deans
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();

        //        // Mapping dữ liệu
        //        var deanDtos = _mapper.Map<List<DeanListForAdminDOETDto>>(paginatedDeans);

        //        var pagedResponse = new PagedResponse<List<DeanListForAdminDOETDto>>
        //        {
        //            Items = deanDtos,
        //            TotalCount = totalDeans,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = totalPages
        //        };

        //        return new DataResponse<PagedResponse<List<DeanListForAdminDOETDto>>>
        //        {
        //            Data = pagedResponse,
        //            Message = "Dean list retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<PagedResponse<List<DeanListForAdminDOETDto>>>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving dean list: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<DeanDetailsDto>> GetDeanDetailsAsync(
        //int deanId,
        //int pageNumber,
        //int pageSize,
        //string? sortBy,
        //bool? isDescending,
        //string? lecturerName,
        //string? studentName,
        //int studentPageNumber,
        //int studentPageSize)
        //{
        //    try
        //    {
        //        var (dean, lecturers, students) = await _userRepository.GetDeanDetailsWithLecturersAndStudentsAsync(deanId);

        //        if (dean == null)
        //        {
        //            return new DataResponse<DeanDetailsDto>
        //            {
        //                Data = null,
        //                Message = "Dean not found.",
        //                StatusCode = 404
        //            };
        //        }

        //        // Filter lecturers by name
        //        if (!string.IsNullOrWhiteSpace(lecturerName))
        //        {
        //            lecturers = lecturers
        //                .Where(l => l.Name.ToLower().Contains(lecturerName.ToLower()))
        //                .ToList();
        //        }

        //        // Filter students by name
        //        if (!string.IsNullOrWhiteSpace(studentName))
        //        {
        //            students = students
        //                .Where(s => s.User.Name.ToLower().Contains(studentName.ToLower()))
        //                .ToList();
        //        }

        //        // Sort lecturers
        //        switch (sortBy?.ToLower())
        //        {
        //            case "lecturername":
        //                lecturers = isDescending.HasValue && isDescending.Value
        //                    ? lecturers.OrderByDescending(l => l.Name).ToList()
        //                    : lecturers.OrderBy(l => l.Name).ToList();
        //                break;
        //            case "major":
        //                lecturers = isDescending.HasValue && isDescending.Value
        //                    ? lecturers.OrderByDescending(l => l.Major.Name).ToList()
        //                    : lecturers.OrderBy(l => l.Major.Name).ToList();
        //                break;
        //        }

        //        // Sort students
        //        switch (sortBy?.ToLower())
        //        {
        //            case "studentname":
        //                students = isDescending.HasValue && isDescending.Value
        //                    ? students.OrderByDescending(s => s.User.Name).ToList()
        //                    : students.OrderBy(s => s.User.Name).ToList();
        //                break;
        //            case "semester":
        //                students = isDescending.HasValue && isDescending.Value
        //                    ? students.OrderByDescending(s => s.Semester.Name).ToList()
        //                    : students.OrderBy(s => s.Semester.Name).ToList();
        //                break;
        //        }

        //        // Paginate lecturers
        //        var lecturerDtos = _mapper.Map<List<LecturerListDto>>(lecturers)
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();

        //        // Paginate students
        //        var paginatedStudents = students
        //            .Skip((studentPageNumber - 1) * studentPageSize)
        //            .Take(studentPageSize)
        //            .ToList();

        //        var studentDtos = _mapper.Map<List<StudentListDto>>(paginatedStudents);

        //        var deanDetails = _mapper.Map<DeanDetailsDto>(dean);

        //        deanDetails.Lecturers = new PagedResponse<List<LecturerListDto>>
        //        {
        //            Items = lecturerDtos,
        //            TotalCount = lecturers.Count,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = (int)Math.Ceiling((double)lecturers.Count / pageSize)
        //        };

        //        deanDetails.Students = new PagedResponse<List<StudentListDto>>
        //        {
        //            Items = studentDtos,
        //            TotalCount = students.Count,
        //            PageSize = studentPageSize,
        //            CurrentPage = studentPageNumber,
        //            TotalPages = (int)Math.Ceiling((double)students.Count / studentPageSize)
        //        };

        //        return new DataResponse<DeanDetailsDto>
        //        {
        //            Data = deanDetails,
        //            Message = "Dean details retrieved successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<DeanDetailsDto>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving dean details: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<string>> AssignLecturersToDeanAsync(AssignLecturersToDeanDto dto)
        //{
        //    var dean = await _userRepository.GetDeanByUserIdAsync(dto.DeanId);
        //    if (dean == null)
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = "Dean not found.",
        //            StatusCode = 404
        //        };
        //    }

        //    var lecturers = await _userRepository.GetLecturersByIdsAsync(dto.LecturerIds);
        //    var invalidLecturers = new List<string>();

        //    foreach (var lecturer in lecturers)
        //    {
        //        if (lecturer.Major == null || lecturer.Major.DepartmentId != dean.DepartmentId)
        //        {
        //            invalidLecturers.Add(lecturer.Email);
        //        }
        //        else
        //        {
        //            lecturer.AssignForId = dto.DeanId;
        //            lecturer.UpdatedAt = DateTime.Now;
        //        }
        //    }

        //    if (invalidLecturers.Any())
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = $"Some lecturers are invalid for this department: {string.Join(", ", invalidLecturers)}.",
        //            StatusCode = 400
        //        };
        //    }

        //    await _userRepository.UpdateLecturersAsync(lecturers);

        //    return new DataResponse<string>
        //    {
        //        Data = "Lecturers assigned successfully.",
        //        Message = "Lecturers have been assigned to the Dean successfully.",
        //        StatusCode = 200
        //    };
        //}

        //public async Task<DataResponse<PagedResponse<List<LecturerListDto>>>> GetAllLecturersAsync(
        //string? userCode,
        //string? name,
        //string? majorName,
        //int pageNumber,
        //int pageSize,
        //string? sortBy,
        //bool? isDescending)
        //{
        //    try
        //    {
        //        // Gọi repository
        //        var lecturers = await _userRepository.GetAllLecturerAsync(userCode, name, majorName, sortBy, isDescending);

        //        if (lecturers == null || !lecturers.Any())
        //        {
        //            return new DataResponse<PagedResponse<List<LecturerListDto>>>
        //            {
        //                Data = null,
        //                Message = "No lecturers found.",
        //                StatusCode = 404
        //            };
        //        }

        //        // Tính toán phân trang
        //        var totalLecturers = lecturers.Count;
        //        var totalPages = (int)Math.Ceiling((double)totalLecturers / pageSize);

        //        // Lấy dữ liệu theo phân trang
        //        var paginatedLecturers = lecturers
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();

        //        // Mapping dữ liệu
        //        var lecturerDtos = _mapper.Map<List<LecturerListDto>>(paginatedLecturers);

        //        var pagedResponse = new PagedResponse<List<LecturerListDto>>
        //        {
        //            Items = lecturerDtos,
        //            TotalCount = totalLecturers,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = totalPages
        //        };

        //        return new DataResponse<PagedResponse<List<LecturerListDto>>>
        //        {
        //            Data = pagedResponse,
        //            Message = "Lecturer list retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<PagedResponse<List<LecturerListDto>>>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving lecturer list: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<string>> AssignDepartmentToDeanAsync(int deanId, int departmentId)
        //{
        //    try
        //    {
        //        // Kiểm tra điều kiện gán
        //        var isAssignable = await _userRepository.IsDeanAssignableToDepartmentAsync(deanId);

        //        if (!isAssignable)
        //        {
        //            return new DataResponse<string>
        //            {
        //                Data = null,
        //                Message = "Cannot assign department. The dean has related lecturers or students assigned.",
        //                StatusCode = 400
        //            };
        //        }

        //        // Gán Department
        //        await _userRepository.AssignDepartmentToDeanAsync(deanId, departmentId);

        //        return new DataResponse<string>
        //        {
        //            Data = "Success",
        //            Message = "Department assigned successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 400
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = $"Error assigning department: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        //public async Task<DataResponse<string>> AssignMajorToLecturerAsync(int lecturerId, int majorId)
        //{
        //    try
        //    {
        //        // Kiểm tra điều kiện gán
        //        var isAssignable = await _userRepository.IsLecturerAssignableToMajorAsync(lecturerId, majorId);

        //        if (!isAssignable)
        //        {
        //            return new DataResponse<string>
        //            {
        //                Data = null,
        //                Message = "Cannot assign major. The lecturer is either assigned to students or the major is not allowed.",
        //                StatusCode = 400
        //            };
        //        }

        //        // Gán Major
        //        await _userRepository.AssignMajorToLecturerAsync(lecturerId, majorId);

        //        return new DataResponse<string>
        //        {
        //            Data = "Success",
        //            Message = "Major assigned successfully.",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 400
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<string>
        //        {
        //            Data = null,
        //            Message = $"Error assigning major: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}
    }
}
