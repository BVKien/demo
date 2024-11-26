using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.PolicyDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IPolicyService
    {
        // Admin - Parent Policy Management
        Task<DataResponse<PagedResponse<List<ParentPolicyListForAdminDTO>>>> GetAllParentPolicyForAdminAsync(string? content, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ParentPolicyDetailForAdminDTO>> GetParentPolicyDetailByIdForAdminAsync(int policyId);
        Task<DataResponse<AddParentPolicyForAdminDTO>> AddParentPolicyForAdminAsync(AddParentPolicyForAdminDTO addParentPolicyForAdminDTO);
        Task<DataResponse<UpdateParentPolicyForAdminDTO>> UpdateParentPolicyForAdminAsync(UpdateParentPolicyForAdminDTO updateParentPolicyForAdminDTO);
        Task<DataResponse<UpdateParentPolicyStatusForAdminDTO>> UpdateParentPolicyStatusForAdminAsync(UpdateParentPolicyStatusForAdminDTO updateParentPolicyStatusForAdminDTO);
        Task<DataResponse<DeleteParentPolicyForAdminDTO>> DeleteParentPolicyForAdminAsync(DeleteParentPolicyForAdminDTO deleteParentPolicyForAdminDTO);
        Task<DataResponse<List<StatusPolicyListForAdminDTO>>> GetAllStatusesPolicyForAdminAsync();

        // Admin - Child Policy Management
        Task<DataResponse<PagedResponse<List<ChildPolicyListForAdminDTO>>>> GetAllChildPolicyForAdminAsync(int parentId, string? content, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ChildPolicyDetailForAdminDTO>> GetChildPolicyDetailByIdForAdminAsync(int policyId);
        Task<DataResponse<AddChildPolicyForAdminDTO>> AddChildPolicyForAdminAsync(AddChildPolicyForAdminDTO addChildPolicyForAdminDTO);
        Task<DataResponse<UpdateChildPolicyForAdminDTO>> UpdateChildPolicyForAdminAsync(UpdateChildPolicyForAdminDTO updateChildPolicyForAdminDTO);
        Task<DataResponse<UpdateChildPolicyStatusForAdminDTO>> UpdateChildPolicyStatusForAdminAsync(UpdateChildPolicyStatusForAdminDTO updateChildPolicyStatusForAdminDTO);
        Task<DataResponse<DeleteChildPolicyForAdminDTO>> DeleteChildPolicyForAdminAsync(DeleteChildPolicyForAdminDTO deleteChildPolicyForAdminDTO);

        // Common - Policy
        Task<DataResponse<PagedResponse<List<PolicyListForCommonDTO>>>> GetAllPolicyAsync(string role, string? content, int pageNumber, int pageSize);
        Task<DataResponse<PolicyDetailForCommonDTO>> GetPolicyDetailAsync(int? policyId, string role);
        Task<DataResponse<List<PolicyListForCommonDTO>>> GetAllPolicyContentForPolicyParentAsync(int? parentId, string role);
    }
}
