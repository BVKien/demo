using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.AdminControllers.CompanyController;
using static OJTEDU.Application.DTOs.CompanyDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin-doet/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [Authorize(Roles = "Admin,DOET")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAllCompanies(string? companyName, string? companyCode, string? status, int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _companyService.GetAllCompaniesForAdminDoetAsync(companyName, companyCode, status, provinceId, districtId, wardId, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<CompanyListForAdminDoetDTO>>>
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

        [Authorize(Roles = "Admin,DOET")]
        [HttpGet("details/{companyId}")]
        public async Task<IActionResult> GetCompanyDetail(int? companyId)
        {
            try
            {
                if (!companyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "companyId is required."
                    });
                }

                var dataResponse = await _companyService.GetCompanyDetailForAdminDoetAsync(companyId.Value);

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

                var apiResponse = new ApiResponse<CompanyDetailForAdminDoetDTO>
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

        [Authorize(Roles = "Admin,DOET")]
        [HttpPut("{companyId}")]
        public async Task<IActionResult> UpdateCompanyForAdminDoet(int? companyId, [FromForm] UpdateCompanyRequestForAdminDoet request)
        {
            try
            {
                if (!companyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "companyId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.CompanyName))
                {
                    errorMessages.Add("CompanyName is required.");
                }

                // Kiểm tra mã số thuế (TaxCode) - bắt buộc, chỉ cho phép số và có độ dài tối đa 50 ký tự
                if (string.IsNullOrWhiteSpace(request.TaxCode))
                {
                    errorMessages.Add("TaxCode is required.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(request.TaxCode, @"^\d+$"))
                {
                    errorMessages.Add("TaxCode must contain only digits.");
                }
                else if (request.TaxCode.Length > 50)
                {
                    errorMessages.Add("TaxCode must not exceed 50 characters.");
                }

                if (!string.IsNullOrWhiteSpace(request.ContactEmail))
                {
                    if (!IsValidEmail(request.ContactEmail)) // Hàm kiểm tra định dạng email
                    {
                        errorMessages.Add("Invalid ContactEmail format.");
                    }
                    else if (request.ContactEmail.Length > 50)
                    {
                        errorMessages.Add("ContactEmail must not exceed 50 characters.");
                    }
                }

                // Kiểm tra số điện thoại (Phone) - bắt buộc, chỉ cho phép số
                if (string.IsNullOrWhiteSpace(request.Phone))
                {
                    errorMessages.Add("Phone is required.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(request.Phone, @"^\d+$"))
                {
                    errorMessages.Add("Phone must contain only digits.");
                }
                else if (request.Phone.Length > 20)
                {
                    errorMessages.Add("Phone must not exceed 20 digits.");
                }

                if (!string.IsNullOrWhiteSpace(request.Website))
                {
                    if (request.Website.Length > 100)
                    {
                        errorMessages.Add("Website must not exceed 100 characters.");
                    }
                    else
                    {
                        Uri uriResult;
                        bool isValidUrl = Uri.TryCreate(request.Website, UriKind.Absolute, out uriResult) &&
                                          (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                        if (!isValidUrl)
                        {
                            errorMessages.Add("Website is not a valid URL.");
                        }
                    }
                }

                if (!request.ProvinceId.HasValue || request.ProvinceId <= 0)
                {
                    errorMessages.Add("ProvinceId is required.");
                }

                // Kiểm tra mã quận (DistrictId) - bắt buộc, phải là số dương
                if (!request.DistrictId.HasValue || request.DistrictId <= 0)
                {
                    errorMessages.Add("DistrictId is required.");
                }

                // Kiểm tra mã phường (WardId) - bắt buộc, phải là số dương
                if (!request.WardId.HasValue || request.WardId <= 0)
                {
                    errorMessages.Add("WardId is required.");
                }

                // Kiểm tra địa chỉ chi tiết (AddressDetail) - bắt buộc
                if (string.IsNullOrWhiteSpace(request.AddressDetail))
                {
                    errorMessages.Add("AddressDetail is required.");
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

                var updateDto = new UpdateCompanyForAdminDoetDTO
                {
                    CompanyId = companyId.Value,
                    CompanyName = request.CompanyName,
                    TaxCode = request.TaxCode,
                    ContactEmail = request.ContactEmail,
                    Phone = request.Phone,
                    Website = request.Website,
                    Description = request.Description
                };

                var dataResponse = await _companyService.UpdateCompanyForAdminDoetAsync(updateDto, request.ProvinceId, request.DistrictId, request.WardId, request.AddressDetail);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during company update."
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
                var apiResponse = new ApiResponse<UpdateCompanyForAdminDoetDTO>
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
    }
}
