using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.RoleDTO;

namespace OJTEDU.Application.DTOs
{
    public class PolicyDTO
    {
        // Admin - Parent Policy
        public class ParentPolicyListForAdminDTO
        {
            public int ParentPolicyId { get; set; }
            public string? User { get; set; }
            public string? ParentPolicycontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ParentPolicyDetailForAdminDTO
        {
            public int ParentPolicyId { get; set; }
            public string? User { get; set; }
            public string? ParentPolicycontent { get; set; }
            public List<RoleListDTO>? Roles { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddParentPolicyForAdminDTO
        {
            public int? UserId { get; set; }
            public string? ParentPolicycontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateParentPolicyForAdminDTO
        {
            public int ParentPolicyId { get; set; }
            public int? UserId { get; set; }
            public string? ParentPolicycontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateParentPolicyStatusForAdminDTO
        {
            public int ParentPolicyId { get; set; }
            public int? UserId { get; set; }
            public string? ParentPolicycontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<UpdateChildPolicyStatusForAdminDTO>? ChangedStatusChildPolicy { get; set; }
        }

        public class DeleteParentPolicyForAdminDTO
        {
            public int ParentPolicyId { get; set; }
            public int? UserId { get; set; }
            public string? ParentPolicycontent { get; set; }
            public string? Status { get; set; }
            public List<DeleteChildPolicyForAdminDTO>? DeletedChildPolicy { get; set; }
        }

        public class StatusPolicyListForAdminDTO
        {
            public string? Status { get; set; }
        }

        // Admin - Child Policy
        public class ChildPolicyListForAdminDTO
        {
            public int ChildPolicyId { get; set; }
            public string? User { get; set; }
            public int? ParentId { get; set; }
            public string? ChildPolicycontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ChildPolicyDetailForAdminDTO
        {
            public int ChildPolicyId { get; set; }
            public string? User { get; set; }
            public int? ParentId { get; set; }
            public string? ChildPolicycontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddChildPolicyForAdminDTO
        {
            public int? UserId { get; set; }
            public int? ParentId { get; set; }
            public string? ChildPolicycontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateChildPolicyForAdminDTO
        {
            public int ChildPolicyId { get; set; }
            public int? UserId { get; set; }
            public int? ParentId { get; set; }
            public string? ChildPolicycontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateChildPolicyStatusForAdminDTO
        {
            public int ChildPolicyId { get; set; }
            public int? UserId { get; set; }
            public int? ParentId { get; set; }
            public string? ChildPolicycontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteChildPolicyForAdminDTO
        {
            public int ChildPolicyId { get; set; }
            public int? UserId { get; set; }
            public int? ParentId { get; set; }
            public string? ChildPolicycontent { get; set; }
            public string? Status { get; set; }
        }

        // Common
        public class PolicyListForCommonDTO
        {
            public int PolicyId { get; set; }
            public string? PolicyContent { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class PolicyDetailForCommonDTO
        {
            public int PolicyId { get; set; }
            public string? CreatedBy { get; set; }
            public string? PolicyContent { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
