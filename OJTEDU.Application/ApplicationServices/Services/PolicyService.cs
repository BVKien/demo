using AutoMapper;
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
using static OJTEDU.Application.DTOs.DocumentDTO;
using static OJTEDU.Application.DTOs.PolicyDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly IMapper _mapper;
        public PolicyService(IPolicyRepository policyRepository, IMapper mapper)
        {
            _policyRepository = policyRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse<PagedResponse<List<ParentPolicyListForAdminDTO>>>> GetAllParentPolicyForAdminAsync(string? content, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var parentPolicy = await _policyRepository.GetAllParentPolicyForAdminAsync(content, roleId, status);

                var totalParentPolicy = parentPolicy.Count();
                var totalPages = totalParentPolicy == 0 ? 1 : (int)Math.Ceiling((double)totalParentPolicy / pageSize);

                // Map thủ công từ Policy sang ParentPolicyListForAdminDTO
                var parentPolicyDtos = parentPolicy
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ParentPolicyListForAdminDTO
                    {
                        ParentPolicyId = doc.PolicyId,
                        User = doc.User?.Name,
                        ParentPolicycontent = doc.PolicyContent,
                        Status = doc.Status,
                        ForRole = doc.PolicyRoles != null && doc.PolicyRoles.Any()
                            ? string.Join(", ", doc.PolicyRoles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ParentPolicyListForAdminDTO>>
                {
                    Items = parentPolicyDtos,
                    TotalCount = totalParentPolicy,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ParentPolicyListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Parent policy list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ParentPolicyListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ParentPolicyListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving parent policy list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ParentPolicyDetailForAdminDTO>> GetParentPolicyDetailByIdForAdminAsync(int policyId)
        {
            try
            {
                var parentPolicy = await _policyRepository.GetParentPolicyByIdForAdminAsync(policyId);

                var parentPolicyDto = _mapper.Map<ParentPolicyDetailForAdminDTO>(parentPolicy);

                return new DataResponse<ParentPolicyDetailForAdminDTO>
                {
                    Data = parentPolicyDto,
                    Message = "Parent policy details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ParentPolicyDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ParentPolicyDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving Parent policy details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddParentPolicyForAdminDTO>> AddParentPolicyForAdminAsync(AddParentPolicyForAdminDTO addParentPolicyForAdminDTO)
        {
            try
            {
                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (addParentPolicyForAdminDTO.ForRoleIds.Contains(null) || addParentPolicyForAdminDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    addParentPolicyForAdminDTO.ForRoleIds = new List<int?> { null };
                }

                var policy = new Policy
                {
                    UserId = addParentPolicyForAdminDTO.UserId,
                    PolicyContent = addParentPolicyForAdminDTO.ParentPolicycontent
                };

                var addResult = await _policyRepository.AddParentPolicyForAdminAsync(policy, addParentPolicyForAdminDTO.ForRoleIds);

                var resultDto = _mapper.Map<AddParentPolicyForAdminDTO>(addResult);

                return new DataResponse<AddParentPolicyForAdminDTO>
                {
                    Data = resultDto,
                    Message = "Parent Policy added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddParentPolicyForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding parent policy: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentPolicyForAdminDTO>> UpdateParentPolicyForAdminAsync(UpdateParentPolicyForAdminDTO updateParentPolicyForAdminDTO)
        {
            try
            {
                var existingPolicy = await _policyRepository.GetParentPolicyByIdForAdminAsync(updateParentPolicyForAdminDTO.ParentPolicyId);
                if (existingPolicy == null)
                {
                    throw new KeyNotFoundException("Policy not found");
                }

                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (updateParentPolicyForAdminDTO.ForRoleIds.Contains(null) || updateParentPolicyForAdminDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    updateParentPolicyForAdminDTO.ForRoleIds = new List<int?> { null };
                }

                // Cập nhật Policy cha
                var policyToUpdate = new Policy
                {
                    PolicyId = updateParentPolicyForAdminDTO.ParentPolicyId,
                    PolicyContent = updateParentPolicyForAdminDTO.ParentPolicycontent
                };

                var updatedResult = await _policyRepository.UpdateParentPolicyForAdminAsync(policyToUpdate, updateParentPolicyForAdminDTO.ForRoleIds);

                var parentPolicyDto = _mapper.Map<UpdateParentPolicyForAdminDTO>(updatedResult);

                return new DataResponse<UpdateParentPolicyForAdminDTO>
                {
                    Data = parentPolicyDto,
                    Message = "Parent Policy updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentPolicyForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentPolicyForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating parent policy: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteParentPolicyForAdminDTO>> DeleteParentPolicyForAdminAsync(DeleteParentPolicyForAdminDTO deleteParentPolicyForAdminDTO)
        {
            try
            {
                var childList = await _policyRepository.GetAllChildPolicyByParentIdForAdminAsync(deleteParentPolicyForAdminDTO.ParentPolicyId);

                var deletedResult = await _policyRepository.DeleteParentPolicyForAdminAsync(deleteParentPolicyForAdminDTO.ParentPolicyId);

                var childPolicyDtoList = _mapper.Map<List<DeleteChildPolicyForAdminDTO>>(childList);

                var parentPolicyDto = _mapper.Map<DeleteParentPolicyForAdminDTO>(deletedResult);
                parentPolicyDto.DeletedChildPolicy = childPolicyDtoList;

                return new DataResponse<DeleteParentPolicyForAdminDTO>
                {
                    Data = parentPolicyDto,
                    Message = "Parent Policy has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteParentPolicyForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteParentPolicyForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting Parent Policy: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentPolicyStatusForAdminDTO>> UpdateParentPolicyStatusForAdminAsync(UpdateParentPolicyStatusForAdminDTO updateParentPolicyStatusForAdminDTO)
        {
            try
            {
                var policy = new Policy
                {
                    PolicyId = updateParentPolicyStatusForAdminDTO.ParentPolicyId,
                    Status = updateParentPolicyStatusForAdminDTO.Status
                };

                var updatedParentPolicyStatusResult = await _policyRepository.UpdateParentPolicyStatusForAdminAsync(policy);

                var childPolicyList = await _policyRepository.GetAllChildPolicyByParentIdForAdminAsync(updateParentPolicyStatusForAdminDTO.ParentPolicyId);

                var childPolicyDtoList = _mapper.Map<List<UpdateChildPolicyStatusForAdminDTO>>(childPolicyList);

                var parentPolicyDto = _mapper.Map<UpdateParentPolicyStatusForAdminDTO>(updatedParentPolicyStatusResult);

                parentPolicyDto.ChangedStatusChildPolicy = childPolicyDtoList;

                return new DataResponse<UpdateParentPolicyStatusForAdminDTO>
                {
                    Data = parentPolicyDto,
                    Message = "Parent Policy updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentPolicyStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentPolicyStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating Parent Policy: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<List<StatusPolicyListForAdminDTO>>> GetAllStatusesPolicyForAdminAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusPolicyListForAdminDTO>
                {
                    new StatusPolicyListForAdminDTO { Status = "Active" },
                    new StatusPolicyListForAdminDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusPolicyListForAdminDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusPolicyListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusPolicyListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        public async Task<DataResponse<PagedResponse<List<ChildPolicyListForAdminDTO>>>> GetAllChildPolicyForAdminAsync(int? parentId, string? content, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var childPolicy = await _policyRepository.GetAllChildPolicyForAdminAsync(parentId, content, roleId, status);

                var totalChildPolicy = childPolicy.Count();
                var totalPages = totalChildPolicy == 0 ? 1 : (int)Math.Ceiling((double)totalChildPolicy / pageSize);

                // Map thủ công từ Policy sang ChildPolicyListForAdminDTO
                var childPolicyDtos = childPolicy
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ChildPolicyListForAdminDTO
                    {
                        ChildPolicyId = doc.PolicyId,
                        User = doc.User?.Name,
                        ParentId = doc.ParentId,
                        ChildPolicycontent = doc.PolicyContent,
                        Status = doc.Status,
                        ForRole = doc.PolicyRoles != null && doc.PolicyRoles.Any()
                            ? string.Join(", ", doc.PolicyRoles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ChildPolicyListForAdminDTO>>
                {
                    Items = childPolicyDtos,
                    TotalCount = totalChildPolicy,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ChildPolicyListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Parent policy list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ChildPolicyListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ChildPolicyListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving child policy list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ChildPolicyDetailForAdminDTO>> GetChildPolicyDetailByIdForAdminAsync(int policyId)
        {
            try
            {
                var childPolicy = await _policyRepository.GetChildPolicyByIdForAdminAsync(policyId);

                var childPolicyDto = _mapper.Map<ChildPolicyDetailForAdminDTO>(childPolicy);

                return new DataResponse<ChildPolicyDetailForAdminDTO>
                {
                    Data = childPolicyDto,
                    Message = "Child policy details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ChildPolicyDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ChildPolicyDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving child policy details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddChildPolicyForAdminDTO>> AddChildPolicyForAdminAsync(AddChildPolicyForAdminDTO addChildPolicyForAdminDTO)
        {
            try
            {
                var policy = new Policy
                {
                    UserId = addChildPolicyForAdminDTO.UserId,
                    ParentId = addChildPolicyForAdminDTO.ParentId,
                    PolicyContent = addChildPolicyForAdminDTO.ChildPolicycontent
                };

                var addResult = await _policyRepository.AddChildPolicyForAdminAsync(policy);

                // Cập nhật thời gian tạo vào DTO trả về
                addChildPolicyForAdminDTO.CreatedAt = addResult.CreatedAt;
                addChildPolicyForAdminDTO.Status = addResult.Status;

                return new DataResponse<AddChildPolicyForAdminDTO>
                {
                    Data = addChildPolicyForAdminDTO,
                    Message = "Child policy added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddChildPolicyForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding child policy: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildPolicyForAdminDTO>> UpdateChildPolicyForAdminAsync(UpdateChildPolicyForAdminDTO updateChildPolicyForAdminDTO)
        {
            try
            {
                var policy = new Policy
                {
                    PolicyId = updateChildPolicyForAdminDTO.ChildPolicyId,
                    PolicyContent = updateChildPolicyForAdminDTO.ChildPolicycontent
                };

                var updatedResult = await _policyRepository.UpdateChildPolicyForAdminAsync(policy);

                var childPolicyDto = _mapper.Map<UpdateChildPolicyForAdminDTO>(updatedResult);

                return new DataResponse<UpdateChildPolicyForAdminDTO>
                {
                    Data = childPolicyDto,
                    Message = "Child policy updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildPolicyForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildPolicyForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating child policy: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteChildPolicyForAdminDTO>> DeleteChildPolicyForAdminAsync(DeleteChildPolicyForAdminDTO deleteChildPolicyForAdminDTO)
        {
            try
            {
                var deletedResult = await _policyRepository.DeleteChildPolicyForAdminAsync(deleteChildPolicyForAdminDTO.ChildPolicyId);

                var childPolicyDto = _mapper.Map<DeleteChildPolicyForAdminDTO>(deletedResult);

                return new DataResponse<DeleteChildPolicyForAdminDTO>
                {
                    Data = childPolicyDto,
                    Message = "Child policy has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteChildPolicyForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteChildPolicyForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting child policy: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildPolicyStatusForAdminDTO>> UpdateChildPolicyStatusForAdminAsync(UpdateChildPolicyStatusForAdminDTO updateChildPolicyStatusForAdminDTO)
        {
            try
            {
                var existingChildPolicy = await _policyRepository.GetChildPolicyByIdForAdminAsync(updateChildPolicyStatusForAdminDTO.ChildPolicyId);

                if (existingChildPolicy == null)
                {
                    throw new KeyNotFoundException("Child Policy not found");
                }

                var parentPolicy = await _policyRepository.GetParentPolicyByIdForAdminAsync(existingChildPolicy.ParentId.Value);

                if (parentPolicy == null)
                {
                    throw new KeyNotFoundException("Parent Policy not found");
                }

                if (parentPolicy.Status == "Unactive" && updateChildPolicyStatusForAdminDTO.Status == "Active")
                {
                    throw new InvalidOperationException("Cannot update status child policy to Active when parent policy is Unactive");
                }

                var policy = new Policy
                {
                    PolicyId = updateChildPolicyStatusForAdminDTO.ChildPolicyId,
                    Status = updateChildPolicyStatusForAdminDTO.Status
                };

                var updatedChildPolicyStatusResult = await _policyRepository.UpdateChildPolicyForAdminAsync(policy);

                var childPolicyDto = _mapper.Map<UpdateChildPolicyStatusForAdminDTO>(updatedChildPolicyStatusResult);

                return new DataResponse<UpdateChildPolicyStatusForAdminDTO>
                {
                    Data = childPolicyDto,
                    Message = "Child policy updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildPolicyStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildPolicyStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating child policy: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<PagedResponse<List<PolicyListForCommonDTO>>>> GetAllPolicyAsync(string role, string? content, int pageNumber, int pageSize)
        {
            try
            {
                var policyList = await _policyRepository.GetAllPolicyAsync(role, content);

                var totalPolicy = policyList.Count();
                var totalPages = totalPolicy == 0 ? 1 : (int)Math.Ceiling((double)totalPolicy / pageSize);

                var policyDtos = totalPolicy > 0 ? _mapper.Map<List<PolicyListForCommonDTO>>(policyList)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<PolicyListForCommonDTO>();

                var pagedResponse = new PagedResponse<List<PolicyListForCommonDTO>>
                {
                    Items = policyDtos,
                    TotalCount = totalPolicy,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<PolicyListForCommonDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Policy list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<PolicyListForCommonDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<PolicyListForCommonDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving policy list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<PolicyDetailForCommonDTO>> GetPolicyDetailAsync(int? policyId, string role)
        {
            try
            {
                var policy = await _policyRepository.GetPolicyDetailAsync(policyId, role);
                var policyDto = _mapper.Map<PolicyDetailForCommonDTO>(policy);

                return new DataResponse<PolicyDetailForCommonDTO>
                {
                    StatusCode = 200,
                    Message = "Policy detail retrieved successfully!",
                    Data = policyDto
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PolicyDetailForCommonDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PolicyDetailForCommonDTO>
                {
                    Data = null,
                    Message = $"Error retrieving policy details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<List<PolicyListForCommonDTO>>> GetAllPolicyContentForPolicyParentAsync(int? parentId, string role)
        {
            try
            {
                var policyList = await _policyRepository.GetAllPolicyContentForPolicyParentAsync(parentId, role);
                var policyListDto = _mapper.Map<List<PolicyListForCommonDTO>>(policyList);

                return new DataResponse<List<PolicyListForCommonDTO>>
                {
                    StatusCode = 200,
                    Message = "Policy content list for news parent retrieved successfully!",
                    Data = policyListDto
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<PolicyListForCommonDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<PolicyListForCommonDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving policy content list for parent: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
