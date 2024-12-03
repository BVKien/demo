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
        public JobService(IJobRepository jobRepository, IMapper mapper, HttpClient httpClient, IConfiguration config, IGoogleJsonWebSignatureValidator googleValidator, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _googleValidator = googleValidator;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
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

        //// Common - Authentication
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
    }
}
