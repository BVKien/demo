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

namespace OJTEDU.Infrastructure.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public PolicyRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Admin - Parent Policy Management

        public async Task<IEnumerable<Policy>> GetAllParentPolicyForAdminAsync(string? content, int? roleId, string? status)
        {
            IQueryable<Policy> query = _context.Policies.Include(u => u.PolicyRoles).ThenInclude(u => u.Role)
                                       .Include(u => u.User)
                                       .Where(u => u.ParentId == null && u.User.Role.Name.Equals("Admin"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(content))
            {
                content = content.ToLower();
                query = query.Where(n => n.PolicyContent.ToLower().Contains(content));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.PolicyRoles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.PolicyRoles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            var parentPolicy = await query.ToListAsync();

            if (parentPolicy == null)
            {
                throw new KeyNotFoundException("Parent Policies not found.");
            }

            var sortedParentPolicy = parentPolicy.OrderByDescending(u => u.Status == "Active")
                               .ThenByDescending(u => u.Status == "Unactive")
                               .ThenByDescending(u => u.PolicyId)
                               .ToList();

            return sortedParentPolicy;
        }

        public async Task<Policy> GetParentPolicyByIdForAdminAsync(int policyId)
        {
            var parentPolicy = await _context.Policies.Include(u => u.PolicyRoles).ThenInclude(dr => dr.Role)
                                                    .Include(u => u.User)
                                                    .FirstOrDefaultAsync(u => u.PolicyId == policyId && u.ParentId == null && u.User.Role.Name.Equals("Admin"));
            if (parentPolicy == null)
            {
                throw new KeyNotFoundException("Parent Policy not found");
            }
            return parentPolicy;
        }

        public async Task<Policy> AddParentPolicyForAdminAsync(Policy policy, List<int?> roleIds)
        {
            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                policy.CreatedAt = GetVietnamTime();
                policy.UpdatedAt = GetVietnamTime();
                policy.Status = "Active"; // Set trạng thái mặc định là Active
                await _context.Policies.AddAsync(policy);
                await _context.SaveChangesAsync();

                // Thêm các bản ghi vào bảng DocumentRoles
                foreach (var roleId in roleIds)
                {
                    var policyRole = new PolicyRole
                    {
                        PolicyId = policy.PolicyId,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.PolicyRoles.AddAsync(policyRole);
                }

                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return policy;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding policy with roles: {ex.Message}");
            }
        }

        public async Task<Policy> UpdateParentPolicyForAdminAsync(Policy policy, List<int?> newRoleIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingParentPolicy = await _context.Policies.Include(u => u.PolicyRoles).ThenInclude(u => u.Role)
                                                            .Include(u => u.User)
                                                            .FirstOrDefaultAsync(u => u.PolicyId == policy.PolicyId && u.ParentId == null && u.User.Role.Name.Equals("Admin"));
                if (existingParentPolicy == null)
                {
                    throw new KeyNotFoundException("Parent Policy not found");
                }

                existingParentPolicy.PolicyContent = policy.PolicyContent ?? existingParentPolicy.PolicyContent;
                existingParentPolicy.UserId = policy.UserId ?? existingParentPolicy.UserId;
                existingParentPolicy.Status = policy.Status ?? existingParentPolicy.Status;
                existingParentPolicy.UpdatedAt = GetVietnamTime();

                // Cập nhật PolicyRoles cho Policy cha
                if (newRoleIds != null && newRoleIds.Any())
                {
                    // Xóa các PolicyRoles hiện tại
                    var existingRoles = _context.PolicyRoles.Where(pr => pr.PolicyId == existingParentPolicy.PolicyId).ToList();
                    _context.PolicyRoles.RemoveRange(existingRoles);
                    await _context.SaveChangesAsync();

                    // Thêm mới PolicyRoles
                    foreach (var roleId in newRoleIds)
                    {
                        var policyRole = new PolicyRole
                        {
                            PolicyId = existingParentPolicy.PolicyId,
                            RoleId = roleId == 0 ? null : roleId // Xử lý Role Guest
                        };
                        await _context.PolicyRoles.AddAsync(policyRole);
                    }
                    await _context.SaveChangesAsync();
                }

                var childPolicies = await GetAllChildPolicyByParentIdForAdminAsync(existingParentPolicy.PolicyId);
                if (childPolicies.Any())
                {
                    foreach (var childPolicy in childPolicies)
                    {
                        // Cập nhật RoleIds của Policy con để khớp với RoleIds của Policy cha
                        var childExistingRoles = _context.PolicyRoles.Where(pr => pr.PolicyId == childPolicy.PolicyId).ToList();
                        _context.PolicyRoles.RemoveRange(childExistingRoles);

                        foreach (var roleId in newRoleIds)
                        {
                            var childPolicyRole = new PolicyRole
                            {
                                PolicyId = childPolicy.PolicyId,
                                RoleId = roleId == 0 ? null : roleId
                            };
                            await _context.PolicyRoles.AddAsync(childPolicyRole);
                        }

                        childPolicy.UpdatedAt = GetVietnamTime();
                        _context.Policies.Update(childPolicy);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingParentPolicy;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error updating parent policy: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Policy>> GetAllChildPolicyByParentIdForAdminAsync(int? parentId)
        {
            return await _context.Policies.Include(p => p.PolicyRoles).ThenInclude(u => u.Role).Include(u => u.User).Where(n => n.ParentId == parentId && n.User.Role.Name.Equals("Admin")).ToListAsync();
        }

        public async Task<Policy> DeleteParentPolicyForAdminAsync(int policyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Lấy Policy cha
                var parentPolicy = await GetParentPolicyByIdForAdminAsync(policyId);
                if (parentPolicy == null)
                {
                    throw new KeyNotFoundException("Parent policy not found.");
                }

                // Xóa tất cả PolicyRoles liên quan đến Policy cha
                var parentPolicyRoles = _context.PolicyRoles.Where(pr => pr.PolicyId == policyId).ToList();
                if (parentPolicyRoles.Any())
                {
                    _context.PolicyRoles.RemoveRange(parentPolicyRoles);
                }

                // Lấy danh sách Policy con liên quan đến Policy cha
                var childPolicies = await GetAllChildPolicyByParentIdForAdminAsync(policyId);
                if (childPolicies != null && childPolicies.Any())
                {
                    // Xóa PolicyRoles liên quan đến các Policy con
                    var childPolicyIds = childPolicies.Select(cp => cp.PolicyId).ToList();
                    var childPolicyRoles = _context.PolicyRoles.Where(pr => childPolicyIds.Contains((int)pr.PolicyId)).ToList();
                    if (childPolicyRoles.Any())
                    {
                        _context.PolicyRoles.RemoveRange(childPolicyRoles);
                    }

                    // Xóa các Policy con
                    _context.Policies.RemoveRange(childPolicies);
                }

                // Xóa Policy cha
                _context.Policies.Remove(parentPolicy);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return parentPolicy;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error deleting parent policy: {ex.Message}");
            }
        }

        public async Task<Policy> UpdateParentPolicyStatusForAdminAsync(Policy policy)
        {
            var existingParentPolicy = await _context.Policies.Include(u => u.PolicyRoles).ThenInclude(u => u.Role)
                                                            .Include(u => u.User)
                                                            .FirstOrDefaultAsync(u => u.PolicyId == policy.PolicyId && u.ParentId == null && u.User.Role.Name.Equals("Admin"));
            if (existingParentPolicy == null)
            {
                throw new KeyNotFoundException("Parent policy not found");
            }

            var childPolicyList = await GetAllChildPolicyByParentIdForAdminAsync(existingParentPolicy.PolicyId);
            if (childPolicyList != null && childPolicyList.Any())
            {
                foreach (var childPolicy in childPolicyList)
                {
                    childPolicy.Status = policy.Status ?? childPolicy.Status;
                    childPolicy.UpdatedAt = GetVietnamTime();
                }

                _context.Policies.UpdateRange(childPolicyList);
            }

            existingParentPolicy.PolicyContent = policy.PolicyContent ?? existingParentPolicy.PolicyContent;
            existingParentPolicy.UserId = policy.UserId ?? existingParentPolicy.UserId;
            existingParentPolicy.Status = policy.Status ?? existingParentPolicy.Status;
            existingParentPolicy.UpdatedAt = GetVietnamTime();

            _context.Policies.Update(existingParentPolicy);
            await _context.SaveChangesAsync();
            return existingParentPolicy;
        }

        // Admin - Child Policy Management

        public async Task<IEnumerable<Policy>> GetAllChildPolicyForAdminAsync(int parentId, string? content, int? roleId, string? status)
        {
            IQueryable<Policy> query = _context.Policies.Include(u => u.PolicyRoles).ThenInclude(u => u.Role)
                                                   .Include(u => u.User)
                                                   .Where(u => u.ParentId == parentId && u.User.Role.Name.Equals("Admin"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(content))
            {
                content = content.ToLower();
                query = query.Where(n => n.PolicyContent.ToLower().Contains(content));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.PolicyRoles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.PolicyRoles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            var childPolicy = await query.ToListAsync();

            if (childPolicy == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Child policy not found");
            }

            var sortedChildPolicy = childPolicy.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.PolicyId)
                                           .ToList();

            return sortedChildPolicy;
        }

        public async Task<Policy> GetChildPolicyByIdForAdminAsync(int policyId)
        {
            var childPolicy = await _context.Policies.Include(u => u.PolicyRoles).ThenInclude(u => u.Role)
                                                    .Include(u => u.User)
                                                    .FirstOrDefaultAsync(u => u.PolicyId == policyId && u.ParentId != null && u.User.Role.Name.Equals("Admin"));
            if (childPolicy == null)
            {
                throw new KeyNotFoundException("Child policy not found");
            }
            return childPolicy;
        }

        public async Task<Policy> AddChildPolicyForAdminAsync(Policy policy)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lấy Policy cha
                var parentPolicy = await GetParentPolicyByIdForAdminAsync(policy.ParentId.Value);
                if (parentPolicy == null)
                {
                    throw new KeyNotFoundException("Parent policy not found.");
                }

                // Set thông tin Policy con
                policy.CreatedAt = GetVietnamTime();
                policy.UpdatedAt = GetVietnamTime();
                policy.Status = "Active"; // Set trạng thái mặc định là Active

                // Thêm Policy con vào bảng Policies
                await _context.Policies.AddAsync(policy);
                await _context.SaveChangesAsync();

                // Lấy danh sách RoleId của Policy cha từ PolicyRoles
                var parentPolicyRoles = await _context.PolicyRoles
                    .Where(pr => pr.PolicyId == parentPolicy.PolicyId)
                    .Select(pr => pr.RoleId)
                    .ToListAsync();

                // Thêm RoleId của Policy cha vào PolicyRoles cho Policy con
                if (parentPolicyRoles != null && parentPolicyRoles.Any())
                {
                    foreach (var roleId in parentPolicyRoles)
                    {
                        var policyRole = new PolicyRole
                        {
                            PolicyId = policy.PolicyId,
                            RoleId = roleId == 0 ? null : roleId
                        };
                        await _context.PolicyRoles.AddAsync(policyRole);
                    }
                    await _context.SaveChangesAsync();
                }

                // Commit transaction
                await transaction.CommitAsync();

                return policy;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding child policy with roles: {ex.Message}");
            }
        }

        public async Task<Policy> UpdateChildPolicyForAdminAsync(Policy policy)
        {
            var existingChildPolicy = await _context.Policies.Include(u => u.PolicyRoles).ThenInclude(u => u.Role)
                                                           .Include(u => u.User)
                                                           .FirstOrDefaultAsync(u => u.PolicyId == policy.PolicyId && u.ParentId != null && u.User.Role.Name.Equals("Admin"));
            if (existingChildPolicy == null)
            {
                throw new KeyNotFoundException("Child policy not found");
            }

            existingChildPolicy.PolicyContent = policy.PolicyContent ?? existingChildPolicy.PolicyContent;
            existingChildPolicy.UserId = policy.UserId ?? existingChildPolicy.UserId;
            existingChildPolicy.ParentId = policy.ParentId ?? existingChildPolicy.ParentId;
            existingChildPolicy.Status = policy.Status ?? existingChildPolicy.Status;
            existingChildPolicy.UpdatedAt = GetVietnamTime();

            _context.Policies.Update(existingChildPolicy);
            await _context.SaveChangesAsync();
            return existingChildPolicy;
        }

        public async Task<Policy> DeleteChildPolicyForAdminAsync(int policyId)
        {
            var childPolicy = await GetChildPolicyByIdForAdminAsync(policyId);
            if (childPolicy == null)
            {
                throw new KeyNotFoundException("Child policy not found in the list.");
            }

            var policyRoles = _context.PolicyRoles.Where(dr => dr.PolicyId == policyId).ToList();
            if (policyRoles.Any())
            {
                _context.PolicyRoles.RemoveRange(policyRoles);
            }

            _context.Policies.Remove(childPolicy);
            await _context.SaveChangesAsync();

            return childPolicy;
        }

        // Common - Policy

        public async Task<IEnumerable<Policy>> GetAllPolicyAsync(string role, string? content)
        {
            var policyQuery = _context.Policies.Include(u => u.PolicyRoles).ThenInclude(u => u.Role)
                                             .Include(n => n.User)
                                             .Where(n => n.ParentId == null && n.Status == "Active");
            if (role == "guest")
            {
                policyQuery = policyQuery.Where(d => d.PolicyRoles.All(dr => dr.RoleId == null));
            }
            else
            {
                policyQuery = policyQuery.Where(d => d.PolicyRoles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            if (!string.IsNullOrEmpty(content))
            {
                content = content.ToLower();
                policyQuery = policyQuery.Where(n => n.PolicyContent.ToLower().Contains(content));
            }

            policyQuery = policyQuery.OrderByDescending(d => d.PolicyRoles.Any(dr => dr.Role != null && dr.Role.Name.Equals(role))) // Vai trò đăng nhập lên đầu
                .ThenBy(d => d.PolicyRoles.Any(dr => dr.RoleId == null)) // Sau đó là guest
                .ThenByDescending(d => d.PolicyId); // Sắp xếp theo DocumentId giảm dần

            var policyList = await policyQuery.ToListAsync();

            if (policyList == null)
            {
                throw new KeyNotFoundException("No policy found for the specified role.");
            }

            return policyList;
        }

        public async Task<Policy> GetPolicyDetailAsync(int? policyId, string role)
        {
            var policyQuery = _context.Policies.Include(u => u.PolicyRoles).ThenInclude(u => u.Role)
                                             .Include(n => n.User)
                                             .Where(n => n.Status == "Active");

            if (role == "guest")
            {
                policyQuery = policyQuery.Where(d => d.PolicyRoles.All(dr => dr.RoleId == null));
            }
            else
            {
                policyQuery = policyQuery.Where(d => d.PolicyRoles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            var policyDetail = await policyQuery.FirstOrDefaultAsync(n => n.PolicyId == policyId);

            if (policyDetail == null)
            {
                throw new KeyNotFoundException("Policy detail not found.");
            }

            return policyDetail;
        }

        public async Task<IEnumerable<Policy>> GetAllPolicyContentForPolicyParentAsync(int? parentId, string role)
        {
            var parentPolicyList = await GetAllPolicyAsync(role, null);
            var parentPolicyExists = parentPolicyList.Any(n => n.PolicyId == parentId);

            if (!parentPolicyExists)
            {
                throw new KeyNotFoundException($"Not found policy parent with id: {parentId}");
            }

            var policyQuery = _context.Policies.Include(u => u.PolicyRoles).ThenInclude(u => u.Role)
                                              .Include(n => n.User)
                                              .Where(n => n.ParentId == parentId && n.Status == "Active");

            if (role == "guest")
            {
                policyQuery = policyQuery.Where(d => d.PolicyRoles.All(dr => dr.RoleId == null));
            }
            else
            {
                policyQuery = policyQuery.Where(d => d.PolicyRoles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            var policyList = await policyQuery.ToListAsync();

            var sortedPolicy = policyList.OrderBy(n => n.PolicyId).ToList();

            return sortedPolicy;
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
