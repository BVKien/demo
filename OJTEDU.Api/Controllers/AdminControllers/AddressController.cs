using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using static OJTEDU.Api.Input.AdminControllers.AddressController;
using static OJTEDU.Application.DTOs.AddressDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // Admin - Province

        [HttpGet("province/list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProvince(string? name, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _addressService.GetAllProvinceForAdminAsync(name, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ProvinceListForAdminDTO>>>
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

        [HttpGet("province/details/{provinceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProvinceDetail(int? provinceId)
        {
            try
            {
                if (!provinceId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "provinceId is required."
                    });
                }

                var dataResponse = await _addressService.GetProvinceDetailByIdForAdminAsync(provinceId.Value);

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

                var apiResponse = new ApiResponse<ProvinceDetailForAdminDTO>
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

        [HttpPost("province")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProvince([FromForm] AddProvinceRequestForAdmin request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.ProvinceName))
                {
                    errorMessages.Add("ProvinceName is required.");
                }
                else if (request.ProvinceName.Length > 100)
                {
                    errorMessages.Add("ProvinceName must not exceed 100 characters.");
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

                var addDto = new AddProvinceForAdminDTO
                {
                    ProvinceName = request.ProvinceName
                };

                var dataResponse = await _addressService.AddProvinceForAdminAsync(addDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during province add."
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

                var apiResponse = new ApiResponse<AddProvinceForAdminDTO>
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

        [HttpPut("province/{provinceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDepartment(int? provinceId, [FromForm] UpdateProvinceRequestForAdmin request)
        {
            try
            {
                if (!provinceId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "provinceId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.ProvinceName))
                {
                    errorMessages.Add("ProvinceName is required.");
                }
                else if (request.ProvinceName.Length > 100)
                {
                    errorMessages.Add("ProvinceName must not exceed 100 characters.");
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

                var updateDto = new UpdateProvinceForAdminDTO
                {
                    ProvinceId = provinceId.Value,
                    ProvinceName = request.ProvinceName
                };

                var dataResponse = await _addressService.UpdateProvinceForAdminAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during province update."
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
                var apiResponse = new ApiResponse<UpdateProvinceForAdminDTO>
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

        [HttpPatch("province/{provinceId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProvinceStatus(int? provinceId, UpdateProvinceStatusRequestForAdmin request)
        {
            try
            {
                if (!provinceId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "provinceId is required."
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

                var updateDto = new UpdateProvinceStatusForAdminDTO
                {
                    ProvinceId = provinceId.Value,
                    Status = request.Status
                };

                var dataResponse = await _addressService.UpdateProvinceStatusForAdminAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during province update status."
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
                var apiResponse = new ApiResponse<UpdateProvinceStatusForAdminDTO>
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

        [HttpDelete("province/{provinceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProvince(int? provinceId)
        {
            try
            {
                if (!provinceId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "provinceId is required."
                    });
                }

                var deleteDto = new DeleteProvinceForAdminDTO
                {
                    ProvinceId = provinceId.Value
                };

                var dataResponse = await _addressService.DeleteProvinceForAdminAsync(deleteDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during province delete."
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
                var apiResponse = new ApiResponse<DeleteProvinceForAdminDTO>
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

        // Admin - District

        [HttpGet("district/list/{provinceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDistrict(int provinceId, string? name, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _addressService.GetAllDistrictForAdminAsync(provinceId, name, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<DistrictListForAdminDTO>>>
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

        [HttpGet("district/details/{districtId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDistrictDetail(int? districtId)
        {
            try
            {
                if (!districtId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "districtId is required."
                    });
                }

                var dataResponse = await _addressService.GetDistrictDetailByIdForAdminAsync(districtId.Value);

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

                var apiResponse = new ApiResponse<DistrictDetailForAdminDTO>
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

        [HttpPost("district")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDistrict([FromForm] AddDistrictRequestForAdmin request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.DistrictName))
                {
                    errorMessages.Add("DistrictName is required.");
                }
                else if (request.DistrictName.Length > 100)
                {
                    errorMessages.Add("DistrictName must not exceed 100 characters.");
                }

                if (request.ProvinceId <= 0)
                {
                    errorMessages.Add("ProvinceId is required.");
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

                var addDto = new AddDistrictForAdminDTO
                {
                    DistrictName = request.DistrictName,
                    ProvinceId = request.ProvinceId
                };

                var dataResponse = await _addressService.AddDistrictForAdminAsync(addDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during district add."
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

                var apiResponse = new ApiResponse<AddDistrictForAdminDTO>
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

        [HttpPut("district/{districtId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDistrict(int? districtId, [FromForm] UpdateDistrictRequestForAdmin request)
        {
            try
            {
                if (!districtId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "districtId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.DistrictName))
                {
                    errorMessages.Add("DistrictName is required.");
                }
                else if (request.DistrictName.Length > 100)
                {
                    errorMessages.Add("DistrictName must not exceed 100 characters.");
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

                var updateDto = new UpdateDistrictForAdminDTO
                {
                    DistrictId = districtId.Value,
                    DistrictName = request.DistrictName
                };

                var dataResponse = await _addressService.UpdateDistrictForAdminAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during district update."
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
                var apiResponse = new ApiResponse<UpdateDistrictForAdminDTO>
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

        [HttpPatch("district/{districtId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDistrictStatus(int? districtId, UpdateDistrictStatusRequestForAdmin request)
        {
            try
            {
                if (!districtId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "districtId is required."
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

                var updateDto = new UpdateDistrictStatusForAdminDTO
                {
                    DistrictId = districtId.Value,
                    Status = request.Status
                };

                var dataResponse = await _addressService.UpdateDistrictStatusForAdminAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during district update status."
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
                var apiResponse = new ApiResponse<UpdateDistrictStatusForAdminDTO>
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

        [HttpDelete("district/{districtId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDistrict(int? districtId)
        {
            try
            {
                if (!districtId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "districtId is required."
                    });
                }

                var deleteDto = new DeleteDistrictForAdminDTO
                {
                    DistrictId = districtId.Value
                };

                var dataResponse = await _addressService.DeleteDistrictForAdminAsync(deleteDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during district delete."
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
                var apiResponse = new ApiResponse<DeleteDistrictForAdminDTO>
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

        // Admin - Ward

        [HttpGet("ward/list/{districtId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllWard(int districtId, string? name, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _addressService.GetAllDistrictForAdminAsync(districtId, name, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<DistrictListForAdminDTO>>>
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

        [HttpGet("ward/details/{wardId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetWardDetail(int? wardId)
        {
            try
            {
                if (!wardId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "wardId is required."
                    });
                }

                var dataResponse = await _addressService.GetWardDetailByIdForAdminAsync(wardId.Value);

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

                var apiResponse = new ApiResponse<WardDetailForAdminDTO>
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

        [HttpPost("ward")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddWard([FromForm] AddWardRequestForAdmin request)
        {
            try
            {
                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.WardName))
                {
                    errorMessages.Add("WardName is required.");
                }
                else if (request.WardName.Length > 100)
                {
                    errorMessages.Add("WardName must not exceed 100 characters.");
                }

                if (request.DistrictId <= 0)
                {
                    errorMessages.Add("DistrictId is required.");
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

                var addDto = new AddWardForAdminDTO
                {
                    WardName = request.WardName,
                    DistrictId = request.DistrictId
                };

                var dataResponse = await _addressService.AddWardForAdminAsync(addDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during ward add."
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

                var apiResponse = new ApiResponse<AddWardForAdminDTO>
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

        [HttpPut("ward/{wardId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateWard(int? wardId, [FromForm] UpdateWardRequestForAdmin request)
        {
            try
            {
                if (!wardId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "wardId is required."
                    });
                }

                // Danh sách để lưu thông báo lỗi
                var errorMessages = new List<string>();

                // Kiểm tra từng thuộc tính

                if (string.IsNullOrWhiteSpace(request.WardName))
                {
                    errorMessages.Add("WardName is required.");
                }
                else if (request.WardName.Length > 100)
                {
                    errorMessages.Add("WardName must not exceed 100 characters.");
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

                var updateDto = new UpdateWardForAdminDTO
                {
                    WardId = wardId.Value,
                    WardName = request.WardName
                };

                var dataResponse = await _addressService.UpdateWardForAdminAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during ward update."
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
                var apiResponse = new ApiResponse<UpdateWardForAdminDTO>
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

        [HttpPatch("ward/{wardId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateWardStatus(int? wardId, UpdateWardStatusRequestForAdmin request)
        {
            try
            {
                if (!wardId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "wardId is required."
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

                var updateDto = new UpdateWardStatusForAdminDTO
                {
                    WardId = wardId.Value,
                    Status = request.Status
                };

                var dataResponse = await _addressService.UpdateWardStatusForAdminAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during ward update status."
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
                var apiResponse = new ApiResponse<UpdateWardStatusForAdminDTO>
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

        [HttpDelete("ward/{wardId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteWard(int? wardId)
        {
            try
            {
                if (!wardId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "wardId is required."
                    });
                }

                var deleteDto = new DeleteWardForAdminDTO
                {
                    WardId = wardId.Value
                };

                var dataResponse = await _addressService.DeleteWardForAdminAsync(deleteDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during ward delete."
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
                var apiResponse = new ApiResponse<DeleteWardForAdminDTO>
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

        // Admin - Status List
        [HttpGet("status-list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStatusesAddressForAdmin()
        {
            try
            {
                var dataResponse = await _addressService.GetAllStatusesAddressForAdminAsync();

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

                var apiResponse = new ApiResponse<List<StatusAddressListForAdminDTO>>
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
