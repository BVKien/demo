using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IPolicyRepository
    {
        // Admin - parent policy management
        Task<IEnumerable<Policy>> GetAllParentPolicyForAdminAsync(string? content, int? roleId, string? status);
        Task<Policy> GetParentPolicyByIdForAdminAsync(int policyId);
        Task<Policy> AddParentPolicyForAdminAsync(Policy policy, List<int?> roleIds);
        Task<Policy> UpdateParentPolicyForAdminAsync(Policy policy, List<int?> newRoleIds);
        Task<Policy> UpdateParentPolicyStatusForAdminAsync(Policy policy);
        Task<Policy> DeleteParentPolicyForAdminAsync(int policyId);

        // Admin - Child Policy Management
        Task<IEnumerable<Policy>> GetAllChildPolicyByParentIdForAdminAsync(int? parentId);
        Task<IEnumerable<Policy>> GetAllChildPolicyForAdminAsync(int parentId, string? content, int? roleId, string? status);
        Task<Policy> GetChildPolicyByIdForAdminAsync(int policyId);
        Task<Policy> AddChildPolicyForAdminAsync(Policy policy);
        Task<Policy> UpdateChildPolicyForAdminAsync(Policy policy);
        Task<Policy> DeleteChildPolicyForAdminAsync(int policyId);

        // Common - Policy
        Task<IEnumerable<Policy>> GetAllPolicyAsync(string role, string? content);
        Task<Policy> GetPolicyDetailAsync(int? policyId, string role);
        Task<IEnumerable<Policy>> GetAllPolicyContentForPolicyParentAsync(int? parentId, string role);
    }
}
