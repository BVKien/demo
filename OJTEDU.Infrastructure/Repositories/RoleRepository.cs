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
    public class RoleRepository : IRoleRepository
    {
        private readonly OJTEDU_DB_V1Context _context;

        public RoleRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllRolesToAddUpdateForAdminAsync()
        {
            var roles = await _context.Roles
                 .Where(r => r.Name != "Mentor")
                 .ToListAsync();
            if (roles == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Roles not found");
            }
            return roles;
        }

        public async Task<IEnumerable<Role>> GetAllRolesForAdminAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            if (roles == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Roles not found");
            }
            return roles;
        }

        public async Task<IEnumerable<Role>> GetAllRolesToAddUpdateForDoetAsync()
        {
            var roles = await _context.Roles
                 .Where(r => r.Name != "Admin" && r.Name != "DOET" && r.Name != "Mentor")
                 .ToListAsync();
            if (roles == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Roles not found");
            }
            return roles;
        }

        public async Task<IEnumerable<Role>> GetAllRolesForDoetAsync()
        {
            var roles = await _context.Roles
                             .Where(r => r.Name != "Admin")
                             .ToListAsync();
            if (roles == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Roles not found");
            }
            return roles;
        }

        public async Task<IEnumerable<Role>> GetAllRolesForCompanyAsync()
        {
            var roles = await _context.Roles
                              .Where(r => r.Name == "Company" || r.Name == "Mentor")
                              .ToListAsync();
            if (roles == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Roles not found");
            }
            return roles;
        }

        public async Task<Role> GetRoleByIdAsync(int roleId)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(u => u.RoleId == roleId);
            if (role == null)
            {
                throw new KeyNotFoundException("Role not found");
            }
            return role;
        }

        public async Task<Role> AddRoleAsync(Role role)
        {
            var existingRoleByName = await _context.Roles.FirstOrDefaultAsync(u => u.Name.Contains(role.Name));
            if (existingRoleByName != null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new InvalidOperationException("A role with the same name already exists.");
            }

            role.CreatedAt = GetVietnamTime();
            role.UpdatedAt = GetVietnamTime();
            role.Status = "Active";
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return role;
        }

        public async Task<Role> UpdateRoleAsync(Role role)
        {

            var existingRole = await _context.Roles.FirstOrDefaultAsync(u => u.RoleId == role.RoleId);
            if (existingRole == null)
            {
                throw new KeyNotFoundException("Role not found");
            }

            if (existingRole.Name != role.Name)
            {
                var nameExists = await _context.Roles.AnyAsync(u => u.Name.Contains(role.Name));
                if (nameExists)
                {
                    throw new InvalidOperationException("A role with the same name already exists.");
                }
            }

            existingRole.Name = role.Name ?? existingRole.Name;
            existingRole.Status = role.Status ?? existingRole.Status;
            existingRole.Description = role.Description ?? existingRole.Description;
            existingRole.UpdatedAt = GetVietnamTime();

            _context.Roles.Update(existingRole);
            await _context.SaveChangesAsync();
            return existingRole;

        }

        public async Task<Role> DeleteRoleAsync(int roleId)
        {
            var role = await GetRoleByIdAsync(roleId);
            if (role == null)
            {
                throw new KeyNotFoundException("Role not found in the role list.");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> CheckRoleDependenciesAsync(int roleId)
        {
            bool hasUsers = await _context.Users.AnyAsync(e => e.RoleId == roleId);
            return hasUsers;
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
