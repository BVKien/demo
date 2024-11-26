using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IBannerRepository
    {
        // Admin - Banner Management
        Task<IEnumerable<Banner>> GetAllBannerForAdminAsync(DateTime? startEventDate, DateTime? endEventDate, string? status);
        Task<Banner> GetBannerByIdForAdminAsync(int bannerId);
        Task<Banner> GetBannerByEventDateForAdminAsync(DateTime? eventDate);
        Task<Banner> AddBannerForAdminAsync(Banner banner);
        Task<Banner> UpdateBannerForAdminAsync(Banner banner);
        Task<Banner> DeleteBannerForAdminAsync(int bannerId);

        // Common
        Task<IEnumerable<Banner>> GetDisplayedBannersAsync();
    }
}
