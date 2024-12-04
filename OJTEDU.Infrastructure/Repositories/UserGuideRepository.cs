using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OJTEDU.Infrastructure.Repositories
{
    public class UserGuideRepository : IUserGuideRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public UserGuideRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserGuide>> GetAllUserGuidesForAdminAsync(string? title, int? roleId, string? status)
        {
            IQueryable<UserGuide> query = _context.UserGuides.Include(u => u.Role);

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.RoleId == roleId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var userGuides = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (userGuides == null)
            {
                throw new KeyNotFoundException("User Guides not found.");
            }

            var sortedUserGuides = userGuides.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.UserGuideId)
                                           .ToList();

            return sortedUserGuides;
        }

        public async Task<UserGuide> GetUserGuideByIdForAdminAsync(int userGuideId)
        {
            var userGuide = await _context.UserGuides.Include(u => u.Role)
                                                   .FirstOrDefaultAsync(u => u.UserGuideId == userGuideId);
            if (userGuide == null)
            {
                throw new KeyNotFoundException("User Guide not found");
            }
            return userGuide;
        }

        public async Task<UserGuide> AddUserGuideForAdminAsync(UserGuide userGuide)
        {
            userGuide.CreatedAt = GetVietnamTime();
            userGuide.UpdatedAt = GetVietnamTime();
            userGuide.Status = "Active"; // Mặc định trạng thái là Active
            await _context.UserGuides.AddAsync(userGuide);
            await _context.SaveChangesAsync();

            return userGuide;
        }
        public async Task<UserGuide> UpdateUserGuideForAdminAsync(UserGuide userGuide)
        {
            var existingUserGuide = await GetUserGuideByIdForAdminAsync(userGuide.UserGuideId);
            if (existingUserGuide == null)
            {
                throw new KeyNotFoundException("User Guide not found");
            }

            existingUserGuide.Title = userGuide.Title ?? existingUserGuide.Title;
            existingUserGuide.UserGuideFile = userGuide.UserGuideFile ?? existingUserGuide.UserGuideFile;
            existingUserGuide.RoleId = userGuide.RoleId ?? existingUserGuide.RoleId;
            existingUserGuide.Status = userGuide.Status ?? existingUserGuide.Status;
            existingUserGuide.UpdatedAt = GetVietnamTime();
            _context.UserGuides.Update(userGuide);
            await _context.SaveChangesAsync();

            return userGuide;
        }

        public async Task<UserGuide> DeleteUserGuideForAdminAsync(int userGuideId)
        {
            var userGuide = await GetUserGuideByIdForAdminAsync(userGuideId);
            if (userGuide == null)
            {
                throw new KeyNotFoundException("User Guide not found in the list.");
            }

            _context.UserGuides.Remove(userGuide);
            await _context.SaveChangesAsync();
            return userGuide;
        }

        public async Task<bool> UserGuideExistsForRoleAsync(int roleId)
        {
            return await _context.UserGuides
                        .AnyAsync(ug => ug.RoleId == roleId);
        }

        public async Task<UserGuide> GetUserGuideByRoleNameAsync(string roleName)
        {
            var userGuide = await _context.UserGuides.Include(u => u.Role)
                                                   .FirstOrDefaultAsync(u => u.Role.Name.ToLower().Contains(roleName.ToLower()) && u.Status == "Active");
            if (userGuide == null)
            {
                throw new KeyNotFoundException("User Guide not found");
            }
            return userGuide;
        }

        private DateTime GetVietnamTime()
        {
            return DateTime.UtcNow.AddHours(7);
        }
    }
}
