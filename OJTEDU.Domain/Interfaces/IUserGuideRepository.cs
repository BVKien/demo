using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IUserGuideRepository
    {
        // Admin
        Task<IEnumerable<UserGuide>> GetAllUserGuidesForAdminAsync(string? title, int? roleId, string? status);
        Task<UserGuide> GetUserGuideByIdForAdminAsync(int userGuideId);
        Task<UserGuide> GetUserGuideByRoleNameAsync(string roleName);
        Task<UserGuide> AddUserGuideForAdminAsync(UserGuide userGuide);
        Task<UserGuide> UpdateUserGuideForAdminAsync(UserGuide userGuide);
        Task<UserGuide> DeleteUserGuideForAdminAsync(int userGuideId);
        Task<bool> UserGuideExistsForRoleAsync(int roleId);
    }
}
