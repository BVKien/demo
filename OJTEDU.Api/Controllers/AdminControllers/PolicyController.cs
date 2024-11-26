using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static OJTEDU.Api.Input.AdminControllers.PolicyController;
using static OJTEDU.Application.DTOs.PolicyDTO;

namespace OJTEDU.Api.Controllers.AdminControllers
{
    [Route("api/admin")]
    [ApiController]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyService _policyService;

        public PolicyController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        [HttpGet("parent-policy/list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllParentPolicyForAdmin(string? content, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _policyService.GetAllParentPolicyForAdminAsync(content, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ParentPolicyListForAdminDTO>>>
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

        [HttpGet("parent-policy/details/{policyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetParentPolicyDetailForAdmin(int? policyId)
        {
            try
            {
                if (!policyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "policyId is required."
                    });
                }

                var dataResponse = await _policyService.GetParentPolicyDetailByIdForAdminAsync(policyId.Value);

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

                var apiResponse = new ApiResponse<ParentPolicyDetailForAdminDTO>
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

        [HttpPost("parent-policy")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddParentPolicyForAdmin([FromForm] AddParentPolicyRequestForAdmin request)
        {
            try
            {
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.ParentPolicycontent))
                {
                    errorMessages.Add("ParentPolicycontent is required.");
                }

                //if (request.ForRoleIds == null || !request.ForRoleIds.Any())
                //{
                //    errorMessages.Add("At least one role is required.");
                //}

                if (string.IsNullOrWhiteSpace(request.ForRoleIds))
                {
                    errorMessages.Add("At least one role is required.");
                }
                else
                {
                    // Kiểm tra xem `ForRoleIds` có chỉ chứa số hay không
                    if (!Regex.IsMatch(request.ForRoleIds, @"^(\d+\s*,\s*)*\d+$"))
                    {
                        errorMessages.Add("ForRoleIds must only contain numbers separated by commas.");
                    }
                }

                if (errorMessages.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = $"Validation errors occurred: {string.Join(", ", errorMessages)}"
                    });
                }

                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var forRoleIdsList = request.ForRoleIds
            .Split(',')
            .Select(id =>
            {
                int? parsedId = int.TryParse(id.Trim(), out int result) ? (int?)result : null;
                return parsedId;
            })
            .ToList();

                var parentPolicyDto = new AddParentPolicyForAdminDTO
                {
                    UserId = int.Parse(createdByUserId),
                    ParentPolicycontent = request.ParentPolicycontent,
                    ForRoleIds = forRoleIdsList
                };

                var dataResponse = await _policyService.AddParentPolicyForAdminAsync(parentPolicyDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent policy add."
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

                var apiResponse = new ApiResponse<AddParentPolicyForAdminDTO>
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

        [HttpPut("parent-policy/{policyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateParentPolicyForAdmin(int? policyId, [FromForm] UpdateParentPolicyRequestForAdmin request)
        {
            try
            {
                if (!policyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "policyId is required."
                    });
                }

                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.ParentPolicycontent))
                {
                    errorMessages.Add("ParentPolicycontent is required.");
                }

                //if (request.ForRoleIds == null || !request.ForRoleIds.Any())
                //{
                //    errorMessages.Add("At least one role is required.");
                //}

                if (string.IsNullOrWhiteSpace(request.ForRoleIds))
                {
                    errorMessages.Add("At least one role is required.");
                }
                else
                {
                    // Kiểm tra xem `ForRoleIds` có chỉ chứa số hay không
                    if (!Regex.IsMatch(request.ForRoleIds, @"^(\d+\s*,\s*)*\d+$"))
                    {
                        errorMessages.Add("ForRoleIds must only contain numbers separated by commas.");
                    }
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

                var existingParentPolicy = await _policyService.GetParentPolicyDetailByIdForAdminAsync(policyId.Value);
                if (existingParentPolicy == null || existingParentPolicy.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Parent Policy not found."
                    });
                }

                var forRoleIdsList = request.ForRoleIds
            .Split(',')
            .Select(id =>
            {
                int? parsedId = int.TryParse(id.Trim(), out int result) ? (int?)result : null;
                return parsedId;
            })
            .ToList();

                var updateDto = new UpdateParentPolicyForAdminDTO
                {
                    ParentPolicyId = policyId.Value,
                    ParentPolicycontent = request.ParentPolicycontent,
                    ForRoleIds = forRoleIdsList
                };

                var dataResponse = await _policyService.UpdateParentPolicyForAdminAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent news update."
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
                var apiResponse = new ApiResponse<UpdateParentPolicyForAdminDTO>
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

        [HttpDelete("parent-policy/{policyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteParentPolicyForAdmin(int? policyId)
        {
            try
            {
                if (!policyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "policyId is required."
                    });
                }

                var parentPolicyDto = new DeleteParentPolicyForAdminDTO
                {
                    ParentPolicyId = policyId.Value
                };

                var dataResponse = await _policyService.DeleteParentPolicyForAdminAsync(parentPolicyDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent policy delete."
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
                var apiResponse = new ApiResponse<DeleteParentPolicyForAdminDTO>
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

        [HttpPatch("parent-policy/{policyId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateParentPolicyStatusForAdmin(int? policyId, UpdateParentPolicyStatusRequestForAdmin request)
        {
            try
            {
                if (!policyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "policyId is required."
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

                var parentPolicyDto = new UpdateParentPolicyStatusForAdminDTO
                {
                    ParentPolicyId = policyId.Value,
                    Status = request.Status
                };

                var dataResponse = await _policyService.UpdateParentPolicyStatusForAdminAsync(parentPolicyDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during parent policy update status."
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
                var apiResponse = new ApiResponse<UpdateParentPolicyStatusForAdminDTO>
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

        [HttpGet("parent-policy/status-list")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStatusesPolicyForAdmin()
        {
            try
            {
                var dataResponse = await _policyService.GetAllStatusesPolicyForAdminAsync();

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

                var apiResponse = new ApiResponse<List<StatusPolicyListForAdminDTO>>
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

        [HttpGet("child-policy/list/{parentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllChildPolicyForAdmin(int? parentId, string? content, int? roleId, string? status, int? pageNumber, int? pageSize)
        {
            try
            {
                if (!parentId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "parentId is required."
                    });
                }

                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                var dataResponse = await _policyService.GetAllChildPolicyForAdminAsync(parentId.Value, content, roleId, status, actualPageNumber, actualPageSize);

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

                var apiResponse = new ApiResponse<PagedResponse<List<ChildPolicyListForAdminDTO>>>
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

        [HttpGet("child-policy/details/{policyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetChildPolicyDetailForAdmin(int? policyId)
        {
            try
            {
                if (!policyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "policyId is required."
                    });
                }

                var dataResponse = await _policyService.GetChildPolicyDetailByIdForAdminAsync(policyId.Value);

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

                var apiResponse = new ApiResponse<ChildPolicyDetailForAdminDTO>
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

        [HttpPost("child-policy")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddChildPolicyForAdmin([FromForm] AddChildPolicyRequestForAdmin request)
        {
            try
            {
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.ChildPolicycontent))
                {
                    errorMessages.Add("ChildPolicycontent is required.");
                }


                if (request.ParentPolicyId <= 0)
                {
                    errorMessages.Add("ParentNewsId is required and must be a valid number.");
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
                string createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var childPolicyDto = new AddChildPolicyForAdminDTO
                {
                    UserId = int.Parse(createdByUserId),
                    ChildPolicycontent = request.ChildPolicycontent,
                    ParentId = request.ParentPolicyId
                };

                var dataResponse = await _policyService.AddChildPolicyForAdminAsync(childPolicyDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child policy add."
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

                var apiResponse = new ApiResponse<AddChildPolicyForAdminDTO>
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

        [HttpPut("child-policy/{policyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateChildPolicyForAdmin(int? policyId, [FromForm] UpdateChildPolicyRequestForAdmin request)
        {
            try
            {
                if (!policyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "policyId is required."
                    });
                }

                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(request.ChildPolicycontent))
                {
                    errorMessages.Add("ChildPolicycontent is required.");
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

                var existingChildPolicy = await _policyService.GetChildPolicyDetailByIdForAdminAsync(policyId.Value);
                if (existingChildPolicy == null || existingChildPolicy.Data == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Child Policy not found."
                    });
                }

                // Cập nhật thông tin cho DTO mới
                var updateDto = new UpdateChildPolicyForAdminDTO
                {
                    ChildPolicyId = policyId.Value,
                    ChildPolicycontent = request.ChildPolicycontent
                };

                var dataResponse = await _policyService.UpdateChildPolicyForAdminAsync(updateDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child policy update."
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
                var apiResponse = new ApiResponse<UpdateChildPolicyForAdminDTO>
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

        [HttpDelete("child-policy/{policyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChildPolicyForAdmin(int? policyId)
        {
            try
            {
                if (!policyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "policyId is required."
                    });
                }

                var childPolicyDto = new DeleteChildPolicyForAdminDTO
                {
                    ChildPolicyId = policyId.Value
                };

                var dataResponse = await _policyService.DeleteChildPolicyForAdminAsync(childPolicyDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child policy delete."
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
                var apiResponse = new ApiResponse<DeleteChildPolicyForAdminDTO>
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

        [HttpPatch("child-policy/{policyId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateChildPolicyStatusForAdmin(int? policyId, UpdateChildPolicyStatusRequestForAdmin request)
        {
            try
            {
                if (!policyId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "policyId is required."
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

                var childPolicyDto = new UpdateChildPolicyStatusForAdminDTO
                {
                    ChildPolicyId = policyId.Value,
                    Status = request.Status
                };

                var dataResponse = await _policyService.UpdateChildPolicyStatusForAdminAsync(childPolicyDto);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred during child policy update status."
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
                var apiResponse = new ApiResponse<UpdateChildPolicyStatusForAdminDTO>
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
