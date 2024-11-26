using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class BannerDTO
    {
        // Admin
        public class BannerListForAdminDTO
        {
            public int BannerId { get; set; }
            public string? Image { get; set; }
            public DateTime? EventDate { get; set; }
            public string? Link { get; set; }
            public string? User { get; set; }
            public string? Status { get; set; }
        }

        public class BannerDetailForAdminDTO
        {
            public int BannerId { get; set; }
            public string? Image { get; set; }
            public DateTime? EventDate { get; set; }
            public string? Link { get; set; }
            public string? User { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddBannerForAdminDTO
        {
            public int? UserId { get; set; }
            public string? Image { get; set; }
            public DateTime? EventDate { get; set; }
            public string? Link { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateBannerForAdminDTO
        {
            public int BannerId { get; set; }
            public int? UserId { get; set; }
            public string? Image { get; set; }
            public DateTime? EventDate { get; set; }
            public string? Link { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateBannerStatusForAdminDTO
        {
            public int BannerId { get; set; }
            public int? UserId { get; set; }
            public string? Image { get; set; }
            public DateTime? EventDate { get; set; }
            public string? Link { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteBannerForAdminDTO
        {
            public int BannerId { get; set; }
            public int? UserId { get; set; }
            public string? Image { get; set; }
            public DateTime? EventDate { get; set; }
            public string? Link { get; set; }
            public string? Status { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class StatusBannerListForAdminDTO
        {
            public string? Status { get; set; }
        }

        // Common
        public class BannerListForCommonDTO
        {
            public int BannerId { get; set; }
            public string? Image { get; set; }
            public string? Link { get; set; }
        }
    }
}
