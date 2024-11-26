using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.RoleDTO;

namespace OJTEDU.Application.DTOs
{
    public class NewsFaqDTO
    {
        // Admin - Parent News
        public class ParentNewsListForAdminDTO
        {
            public int ParentNewsId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ParentNewsDetailForAdminDTO
        {
            public int ParentNewsId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public List<RoleListDTO>? Roles { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddParentNewsForAdminDTO
        {
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateParentNewsForAdminDTO
        {
            public int ParentNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateParentNewsStatusForAdminDTO
        {
            public int ParentNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<UpdateChildNewsStatusForAdminDTO>? ChangedStatusChildNews { get; set; }
        }

        public class DeleteParentNewsForAdminDTO
        {
            public int ParentNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? DeletedAt { get; set; }
            public List<DeleteChildNewsForAdminDTO>? DeletedChildNews { get; set; }
        }

        public class StatusNewsListForAdminDTO
        {
            public string? Status { get; set; }
        }

        // Admin - Child News
        public class ChildNewsListForAdminDTO
        {
            public int ChildNewsId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ChildNewsDetailForAdminDTO
        {
            public int ChildNewsId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddChildNewsForAdminDTO
        {
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateChildNewsForAdminDTO
        {
            public int ChildNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateChildNewsStatusForAdminDTO
        {
            public int ChildNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteChildNewsForAdminDTO
        {
            public int ChildNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        // Admin - Parent Faq
        public class ParentFaqListForAdminDTO
        {
            public int ParentFaqId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ParentFaqDetailForAdminDTO
        {
            public int ParentFaqId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public List<RoleListDTO>? Roles { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddParentFaqForAdminDTO
        {
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateParentFaqForAdminDTO
        {
            public int ParentFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateParentFaqStatusForAdminDTO
        {
            public int ParentFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<UpdateChildFaqStatusForAdminDTO>? ChangedStatusChildFaq { get; set; }
        }

        public class DeleteParentFaqForAdminDTO
        {
            public int ParentFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? DeletedAt { get; set; }
            public List<DeleteChildFaqForAdminDTO>? DeletedChildFaq { get; set; }
        }

        public class StatusFaqListForAdminDTO
        {
            public string? Status { get; set; }
        }

        // Admin - Child Faq
        public class ChildFaqListForAdminDTO
        {
            public int ChildFaqId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ChildFaqDetailForAdminDTO
        {
            public int ChildFaqId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddChildFaqForAdminDTO
        {
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateChildFaqForAdminDTO
        {
            public int ChildFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateChildFaqStatusForAdminDTO
        {
            public int ChildFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteChildFaqForAdminDTO
        {
            public int ChildFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        // Doet - Parent News
        public class ParentNewsListForDoetDTO
        {
            public int ParentNewsId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ParentNewsDetailForDoetDTO
        {
            public int ParentNewsId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public List<RoleListDTO>? Roles { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddParentNewsForDoetDTO
        {
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateParentNewsForDoetDTO
        {
            public int ParentNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateParentNewsStatusForDoetDTO
        {
            public int ParentNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<UpdateChildNewsStatusForDoetDTO>? ChangedStatusChildNews { get; set; }
        }

        public class DeleteParentNewsForDoetDTO
        {
            public int ParentNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? DeletedAt { get; set; }
            public List<DeleteChildNewsForDoetDTO>? DeletedChildNews { get; set; }
        }

        public class StatusNewsListForDoetDTO
        {
            public string? Status { get; set; }
        }

        // doet - Child News
        public class ChildNewsListForDoetDTO
        {
            public int ChildNewsId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ChildNewsDetailForDoetDTO
        {
            public int ChildNewsId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddChildNewsForDoetDTO
        {
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateChildNewsForDoetDTO
        {
            public int ChildNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateChildNewsStatusForDoetDTO
        {
            public int ChildNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteChildNewsForDoetDTO
        {
            public int ChildNewsId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public string? Status { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        // doet - Parent Faq
        public class ParentFaqListForDoetDTO
        {
            public int ParentFaqId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ParentFaqDetailForDoetDTO
        {
            public int ParentFaqId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public List<RoleListDTO>? Roles { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddParentFaqForDoetDTO
        {
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateParentFaqForDoetDTO
        {
            public int ParentFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateParentFaqStatusForDoetDTO
        {
            public int ParentFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<UpdateChildFaqStatusForDoetDTO>? ChangedStatusChildFaq { get; set; }
        }

        public class DeleteParentFaqForDoetDTO
        {
            public int ParentFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? ParentFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? DeletedAt { get; set; }
            public List<DeleteChildFaqForDoetDTO>? DeletedChildFaq { get; set; }
        }

        public class StatusFaqListForDoetDTO
        {
            public string? Status { get; set; }
        }

        // DOet - Child Faq
        public class ChildFaqListForDoetDTO
        {
            public int ChildFaqId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class ChildFaqDetailForDoetDTO
        {
            public int ChildFaqId { get; set; }
            public string? User { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddChildFaqForDoetDTO
        {
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateChildFaqForDoetDTO
        {
            public int ChildFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateChildFaqStatusForDoetDTO
        {
            public int ChildFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteChildFaqForDoetDTO
        {
            public int ChildFaqId { get; set; }
            public int? UserId { get; set; }
            public string? Title { get; set; }
            public int? ParentId { get; set; }
            public string? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public string? Status { get; set; }
            public DateTime? DeletedAt { get; set; }
        }


        // Common
        public class NewsFaqListForCommonDTO
        {
            public int NewsFaqId { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? NewsFaqcontent { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class NewsFaqDetailForCommonDTO
        {
            public int NewsFaqId { get; set; }
            public string? CreatedBy { get; set; }
            public string? Title { get; set; }
            public string? Image { get; set; }
            public string? NewsFaqcontent { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
