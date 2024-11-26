using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IRoleRepository
    {
        // CRUD role operations
        Task<IEnumerable<Role>> GetAllRolesToAddUpdateForAdminAsync();
        Task<IEnumerable<Role>> GetAllRolesToAddUpdateForDoetAsync();
        Task<IEnumerable<Role>> GetAllRolesForAdminAsync();
        Task<IEnumerable<Role>> GetAllRolesForDoetAsync();
        Task<IEnumerable<Role>> GetAllRolesForCompanyAsync();
        Task<Role> GetRoleByIdAsync(int roleId);
        Task<Role> AddRoleAsync(Role role);
        Task<Role> UpdateRoleAsync(Role role);
        Task<Role> DeleteRoleAsync(int roleId);
        Task<bool> CheckRoleDependenciesAsync(int roleId);
    }
}
