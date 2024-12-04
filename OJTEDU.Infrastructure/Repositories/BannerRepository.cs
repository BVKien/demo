using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public BannerRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Admin - Banner Management
        public async Task<IEnumerable<Banner>> GetAllBannerForAdminAsync(DateTime? startEventDate, DateTime? endEventDate, string? status)
        {
            IQueryable<Banner> query = _context.Banners.Include(u => u.User)
                                                       .Where(u => u.User.Role.Name.Equals("Admin"));

            // Apply search filters if provided
            if (startEventDate.HasValue && endEventDate.HasValue)
            {
                query = query.Where(b => b.EventDate >= startEventDate.Value && b.EventDate <= endEventDate.Value);
            }
            else if (startEventDate.HasValue)
            {
                query = query.Where(b => b.EventDate >= startEventDate.Value);
            }
            else if (endEventDate.HasValue)
            {
                query = query.Where(b => b.EventDate <= endEventDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var banners = await query.ToListAsync();

            if (banners == null)
            {
                throw new KeyNotFoundException("Banners not found.");
            }

            var sortedBanners = banners.OrderByDescending(u => u.Status == "Active")
                               .ThenByDescending(u => u.Status == "Unactive")
                               .ThenBy(b => b.EventDate)
                               .ToList();

            return sortedBanners;
        }

        public async Task<Banner> GetBannerByIdForAdminAsync(int bannerId)
        {
            var banner = await _context.Banners.Include(u => u.User)
                                               .FirstOrDefaultAsync(u => u.BannerId == bannerId && u.User.Role.Name.Equals("Admin"));
            if (banner == null)
            {
                throw new KeyNotFoundException("Banner not found");
            }
            return banner;
        }

        public async Task<Banner?> GetBannerByEventDateForAdminAsync(DateTime? eventDate)
        {
            var banner = await _context.Banners.Include(u => u.User)
                                               .FirstOrDefaultAsync(u => u.EventDate == eventDate && u.User.Role.Name.Equals("Admin"));
            return banner;
        }

        public async Task<Banner> AddBannerForAdminAsync(Banner banner)
        {
            banner.CreatedAt = GetVietnamTime();
            banner.UpdatedAt = GetVietnamTime();
            banner.Status = "Active"; // Set trạng thái mặc định là Active
            await _context.Banners.AddAsync(banner);
            await _context.SaveChangesAsync();

            return banner;
        }

        public async Task<Banner> UpdateBannerForAdminAsync(Banner banner)
        {
            var existingBanner = await _context.Banners.Include(u => u.User)
                                                       .FirstOrDefaultAsync(u => u.BannerId == banner.BannerId && u.User.Role.Name.Equals("Admin"));
            if (existingBanner == null)
            {
                throw new KeyNotFoundException("Banner not found");
            }

            existingBanner.Link = banner.Link ?? existingBanner.Link;
            existingBanner.EventDate = banner.EventDate ?? existingBanner.EventDate;
            existingBanner.Image = banner.Image ?? existingBanner.Image;
            existingBanner.UserId = banner.UserId ?? existingBanner.UserId;
            existingBanner.Status = banner.Status ?? existingBanner.Status;
            existingBanner.UpdatedAt = GetVietnamTime();

            _context.Banners.Update(existingBanner);
            await _context.SaveChangesAsync();
            return existingBanner;
        }

        public async Task<Banner> DeleteBannerForAdminAsync(int bannerId)
        {
            var banner = await GetBannerByIdForAdminAsync(bannerId);
            if (banner == null)
            {
                throw new KeyNotFoundException("Banner not found in the list.");
            }

            banner.DeletedAt = GetVietnamTime(); // Cập nhật thời gian xóa
            _context.Banners.Remove(banner);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return banner;
        }

        // Common
        public async Task<IEnumerable<Banner>> GetDisplayedBannersAsync()
        {
            return await _context.Banners.Where(b => b.Status == "Displayed")
                                 .OrderBy(b => b.CreatedAt)
                                 .ToListAsync();
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
