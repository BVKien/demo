using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OJTEDU.Infrastructure.Repositories
{
    public class NewsFaqRepository : INewsFaqRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public NewsFaqRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Admin - Parent News Management
        public async Task<IEnumerable<NewsFaq>> GetAllParentNewsForAdminAsync(string? title, int? roleId, string? status)
        {
            IQueryable<NewsFaq> query = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                       .Include(u => u.User)
                                       .Where(u => u.IsNews == true && u.ParentId == null && u.User.Role.Name.Equals("Admin"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var parentNews = await query.ToListAsync();

            if (parentNews == null)
            {
                throw new KeyNotFoundException("Parent News not found.");
            }

            var sortedParentNews = parentNews.OrderByDescending(u => u.Status == "Active")
                               .ThenByDescending(u => u.Status == "Unactive")
                               .ThenByDescending(u => u.NewsFaqid)
                               .ToList();

            return sortedParentNews;
        }

        public async Task<NewsFaq> GetParentNewsByIdForAdminAsync(int newsId)
        {
            var parentNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                    .Include(u => u.User)
                                                    .FirstOrDefaultAsync(u => u.NewsFaqid == newsId && u.ParentId == null && u.IsNews == true && u.User.Role.Name.Equals("Admin"));
            if (parentNews == null)
            {
                throw new KeyNotFoundException("Parent News not found");
            }
            return parentNews;
        }

        public async Task<NewsFaq> AddParentNewsForAdminAsync(NewsFaq newsFaq, List<int?> roleIds)
        {
            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                newsFaq.CreatedAt = GetVietnamTime();
                newsFaq.UpdatedAt = GetVietnamTime();
                newsFaq.Status = "Active"; // Set trạng thái mặc định là Active
                newsFaq.IsNews = true;
                await _context.NewsFaqs.AddAsync(newsFaq);
                await _context.SaveChangesAsync();

                // Thêm các bản ghi vào bảng DocumentRoles
                foreach (var roleId in roleIds)
                {
                    var newsFaqrole = new NewsFaqrole
                    {
                        NewsFaqid = newsFaq.NewsFaqid,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.NewsFaqroles.AddAsync(newsFaqrole);
                }

                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return newsFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding parent news with roles: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateParentNewsForAdminAsync(NewsFaq newsFaq, List<int?> newRoleIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingParentNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                            .Include(u => u.User)
                                                            .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId == null && u.IsNews == true && u.User.Role.Name.Equals("Admin"));
                if (existingParentNews == null)
                {
                    throw new KeyNotFoundException("Parent News not found");
                }

                existingParentNews.Title = newsFaq.Title ?? existingParentNews.Title;
                existingParentNews.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingParentNews.NewsFaqcontent;
                existingParentNews.Image = newsFaq.Image ?? existingParentNews.Image;
                existingParentNews.UserId = newsFaq.UserId ?? existingParentNews.UserId;
                existingParentNews.Status = newsFaq.Status ?? existingParentNews.Status;
                existingParentNews.UpdatedAt = GetVietnamTime();

                // Cập nhật PolicyRoles cho Policy cha
                if (newRoleIds != null && newRoleIds.Any())
                {
                    // Xóa các PolicyRoles hiện tại
                    var existingRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == existingParentNews.NewsFaqid).ToList();
                    _context.NewsFaqroles.RemoveRange(existingRoles);
                    await _context.SaveChangesAsync();

                    // Thêm mới PolicyRoles
                    foreach (var roleId in newRoleIds)
                    {
                        var newsFaqrole = new NewsFaqrole
                        {
                            NewsFaqid = existingParentNews.NewsFaqid,
                            RoleId = roleId == 0 ? null : roleId // Xử lý Role Guest
                        };
                        await _context.NewsFaqroles.AddAsync(newsFaqrole);
                    }
                    await _context.SaveChangesAsync();
                }

                var childNews = await GetAllChildNewsByParentIdForAdminAsync(existingParentNews.NewsFaqid);
                if (childNews.Any())
                {
                    foreach (var childNew in childNews)
                    {
                        // Cập nhật RoleIds của Policy con để khớp với RoleIds của Policy cha
                        var childExistingRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == childNew.NewsFaqid).ToList();
                        _context.NewsFaqroles.RemoveRange(childExistingRoles);

                        foreach (var roleId in newRoleIds)
                        {
                            var childNewsrole = new NewsFaqrole
                            {
                                NewsFaqid = childNew.NewsFaqid,
                                RoleId = roleId == 0 ? null : roleId
                            };
                            await _context.NewsFaqroles.AddAsync(childNewsrole);
                        }

                        childNew.UpdatedAt = GetVietnamTime();
                        _context.NewsFaqs.Update(childNew);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingParentNews;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error updating parent news: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateParentNewsStatusForAdminAsync(NewsFaq newsFaq)
        {
            var existingParentNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                            .Include(u => u.User)
                                                            .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId == null && u.IsNews == true && u.User.Role.Name.Equals("Admin"));
            if (existingParentNews == null)
            {
                throw new KeyNotFoundException("Parent News not found");
            }

            // Cập nhật trạng thái cho tất cả các ChildNews có cùng ParentId
            var childNewsList = await GetAllChildNewsByParentIdForAdminAsync(existingParentNews.NewsFaqid);
            if (childNewsList != null && childNewsList.Any())
            {
                foreach (var childNews in childNewsList)
                {
                    childNews.Status = newsFaq.Status ?? childNews.Status;
                    childNews.UpdatedAt = GetVietnamTime();
                }

                _context.NewsFaqs.UpdateRange(childNewsList);
            }

            existingParentNews.Title = newsFaq.Title ?? existingParentNews.Title;
            existingParentNews.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingParentNews.NewsFaqcontent;
            existingParentNews.Image = newsFaq.Image ?? existingParentNews.Image;
            existingParentNews.UserId = newsFaq.UserId ?? existingParentNews.UserId;
            existingParentNews.Status = newsFaq.Status ?? existingParentNews.Status;
            existingParentNews.UpdatedAt = GetVietnamTime();

            _context.NewsFaqs.Update(existingParentNews);
            await _context.SaveChangesAsync();
            return existingParentNews;

        }

        public async Task<IEnumerable<NewsFaq>> GetAllChildNewsByParentIdForAdminAsync(int? parentId)
        {
            return await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role).Include(u => u.User).Where(n => n.ParentId == parentId && n.IsNews == true && n.User.Role.Name.Equals("Admin")).ToListAsync();
        }

        public async Task<NewsFaq> DeleteParentNewsForAdminAsync(int newsId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Tìm kiếm parent news theo ID
                var parentNews = await GetParentNewsByIdForAdminAsync(newsId);
                if (parentNews == null)
                {
                    throw new KeyNotFoundException("Parent News not found in the list.");
                }

                // Xóa tất cả PolicyRoles liên quan đến Policy cha
                var parentNewsRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == newsId).ToList();
                if (parentNewsRoles.Any())
                {
                    _context.NewsFaqroles.RemoveRange(parentNewsRoles);
                }

                // Lấy tất cả các child news có ParentId trùng với newsId của parent
                var childNews = await GetAllChildNewsByParentIdForAdminAsync(newsId);
                if (childNews != null && childNews.Any())
                {
                    // Xóa PolicyRoles liên quan đến các Policy con
                    var childNewsIds = childNews.Select(cp => cp.NewsFaqid).ToList();
                    var childNewsRoles = _context.NewsFaqroles.Where(pr => childNewsIds.Contains((int)pr.NewsFaqid)).ToList();
                    if (childNewsRoles.Any())
                    {
                        _context.NewsFaqroles.RemoveRange(childNewsRoles);
                    }

                    // Xóa các Policy con
                    _context.NewsFaqs.RemoveRange(childNews);
                }

                // Xóa Policy cha
                _context.NewsFaqs.Remove(parentNews);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return parentNews;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error deleting parent news: {ex.Message}");
            }
        }

        // Admin - Child News Management
        public async Task<IEnumerable<NewsFaq>> GetAllChildNewsForAdminAsync(int parentId, string? title, int? roleId, string? status)
        {
            IQueryable<NewsFaq> query = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                   .Include(u => u.User)
                                                   .Where(u => u.IsNews == true && u.ParentId == parentId && u.User.Role.Name.Equals("Admin"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var childNews = await query.ToListAsync();

            if (childNews == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Child News not found");
            }

            var sortedChildNews = childNews.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.NewsFaqid)
                                           .ToList();

            return sortedChildNews;
        }

        public async Task<NewsFaq> GetChildNewsByIdForAdminAsync(int newsId)
        {
            var childNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                    .Include(u => u.User)
                                                    .FirstOrDefaultAsync(u => u.NewsFaqid == newsId && u.ParentId != null && u.IsNews == true && u.User.Role.Name.Equals("Admin"));
            if (childNews == null)
            {
                throw new KeyNotFoundException("Child News not found");
            }
            return childNews;
        }

        public async Task<NewsFaq> AddChildNewsForAdminAsync(NewsFaq newsFaq)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var parentNews = await GetParentNewsByIdForAdminAsync(newsFaq.ParentId.Value);
                if (parentNews == null)
                {
                    throw new KeyNotFoundException("Parent News not found.");
                }

                newsFaq.CreatedAt = GetVietnamTime();
                newsFaq.UpdatedAt = GetVietnamTime();
                newsFaq.Status = "Active"; // Set trạng thái mặc định là Active
                newsFaq.IsNews = true;
                await _context.NewsFaqs.AddAsync(newsFaq);
                await _context.SaveChangesAsync();

                var parentNewsRoles = await _context.NewsFaqroles
                    .Where(pr => pr.NewsFaqid == parentNews.NewsFaqid)
                    .Select(pr => pr.RoleId)
                    .ToListAsync();

                if (parentNewsRoles != null && parentNewsRoles.Any())
                {
                    foreach (var roleId in parentNewsRoles)
                    {
                        var newsFaqrole = new NewsFaqrole
                        {
                            NewsFaqid = newsFaq.NewsFaqid,
                            RoleId = roleId == 0 ? null : roleId
                        };
                        await _context.NewsFaqroles.AddAsync(newsFaqrole);
                    }
                    await _context.SaveChangesAsync();
                }

                // Commit transaction
                await transaction.CommitAsync();

                return newsFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding child news with roles: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateChildNewsForAdminAsync(NewsFaq newsFaq)
        {
            var existingChildNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                           .Include(u => u.User)
                                                           .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId != null && u.IsNews == true && u.User.Role.Name.Equals("Admin"));
            if (existingChildNews == null)
            {
                throw new KeyNotFoundException("Child News not found");
            }

            existingChildNews.Title = newsFaq.Title ?? existingChildNews.Title;
            existingChildNews.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingChildNews.NewsFaqcontent;
            existingChildNews.Image = newsFaq.Image ?? existingChildNews.Image;
            existingChildNews.UserId = newsFaq.UserId ?? existingChildNews.UserId;
            existingChildNews.ParentId = newsFaq.ParentId ?? existingChildNews.ParentId;
            existingChildNews.Status = newsFaq.Status ?? existingChildNews.Status;
            existingChildNews.UpdatedAt = GetVietnamTime();

            _context.NewsFaqs.Update(existingChildNews);
            await _context.SaveChangesAsync();
            return existingChildNews;

        }

        public async Task<NewsFaq> DeleteChildNewsForAdminAsync(int newsId)
        {
            var childNews = await GetChildNewsByIdForAdminAsync(newsId);
            if (childNews == null)
            {
                throw new KeyNotFoundException("Child News not found in the list.");
            }

            var newsRoles = _context.NewsFaqroles.Where(dr => dr.NewsFaqid == newsId).ToList();
            if (newsRoles.Any())
            {
                _context.NewsFaqroles.RemoveRange(newsRoles);
            }

            childNews.DeletedAt = GetVietnamTime();
            _context.NewsFaqs.Remove(childNews);
            await _context.SaveChangesAsync();

            return childNews;
        }

        // Admin - Parent Faq Management
        public async Task<IEnumerable<NewsFaq>> GetAllParentFaqForAdminAsync(string? title, int? roleId, string? status)
        {
            IQueryable<NewsFaq> query = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                       .Include(u => u.User)
                                       .Where(u => u.IsNews == false && u.ParentId == null && u.User.Role.Name.Equals("Admin"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var parentFaq = await query.ToListAsync();

            if (parentFaq == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Parent Faq not found");
            }
            var sortedParentFaq = parentFaq.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.NewsFaqid)
                                           .ToList();

            return sortedParentFaq;
        }

        public async Task<NewsFaq> GetParentFaqByIdForAdminAsync(int faqId)
        {
            var parentFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                   .Include(u => u.User)
                                                   .FirstOrDefaultAsync(u => u.NewsFaqid == faqId && u.ParentId == null && u.IsNews == false && u.User.Role.Name.Equals("Admin"));
            if (parentFaq == null)
            {
                throw new KeyNotFoundException("Parent Faq not found");
            }
            return parentFaq;
        }

        public async Task<NewsFaq> AddParentFaqForAdminAsync(NewsFaq newsFaq, List<int?> roleIds)
        {
            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                newsFaq.CreatedAt = GetVietnamTime();
                newsFaq.UpdatedAt = GetVietnamTime();
                newsFaq.Status = "Active"; // Set trạng thái mặc định là Active
                newsFaq.IsNews = false;
                await _context.NewsFaqs.AddAsync(newsFaq);
                await _context.SaveChangesAsync();

                // Thêm các bản ghi vào bảng DocumentRoles
                foreach (var roleId in roleIds)
                {
                    var newsFaqrole = new NewsFaqrole
                    {
                        NewsFaqid = newsFaq.NewsFaqid,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.NewsFaqroles.AddAsync(newsFaqrole);
                }

                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return newsFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding parent faq with roles: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateParentFaqForAdminAsync(NewsFaq newsFaq, List<int?> faqRoleIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingParentFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                            .Include(u => u.User)
                                                            .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId == null && u.IsNews == false && u.User.Role.Name.Equals("Admin"));
                if (existingParentFaq == null)
                {
                    throw new KeyNotFoundException("Parent News not found");
                }

                existingParentFaq.Title = newsFaq.Title ?? existingParentFaq.Title;
                existingParentFaq.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingParentFaq.NewsFaqcontent;
                existingParentFaq.Image = newsFaq.Image ?? existingParentFaq.Image;
                existingParentFaq.UserId = newsFaq.UserId ?? existingParentFaq.UserId;
                existingParentFaq.Status = newsFaq.Status ?? existingParentFaq.Status;
                existingParentFaq.UpdatedAt = GetVietnamTime();

                // Cập nhật PolicyRoles cho Policy cha
                if (faqRoleIds != null && faqRoleIds.Any())
                {
                    // Xóa các PolicyRoles hiện tại
                    var existingRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == existingParentFaq.NewsFaqid).ToList();
                    _context.NewsFaqroles.RemoveRange(existingRoles);
                    await _context.SaveChangesAsync();

                    // Thêm mới PolicyRoles
                    foreach (var roleId in faqRoleIds)
                    {
                        var newsFaqrole = new NewsFaqrole
                        {
                            NewsFaqid = existingParentFaq.NewsFaqid,
                            RoleId = roleId == 0 ? null : roleId // Xử lý Role Guest
                        };
                        await _context.NewsFaqroles.AddAsync(newsFaqrole);
                    }
                    await _context.SaveChangesAsync();
                }

                var childFaqs = await GetAllChildFaqByParentIdForAdminAsync(existingParentFaq.NewsFaqid);
                if (childFaqs.Any())
                {
                    foreach (var childFaq in childFaqs)
                    {
                        // Cập nhật RoleIds của Policy con để khớp với RoleIds của Policy cha
                        var childExistingRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == childFaq.NewsFaqid).ToList();
                        _context.NewsFaqroles.RemoveRange(childExistingRoles);

                        foreach (var roleId in faqRoleIds)
                        {
                            var childNewsrole = new NewsFaqrole
                            {
                                NewsFaqid = childFaq.NewsFaqid,
                                RoleId = roleId == 0 ? null : roleId
                            };
                            await _context.NewsFaqroles.AddAsync(childNewsrole);
                        }

                        childFaq.UpdatedAt = GetVietnamTime();
                        _context.NewsFaqs.Update(childFaq);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingParentFaq;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error updating parent faq: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateParentFaqStatusForAdminAsync(NewsFaq newsFaq)
        {
            var existingParentFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                           .Include(u => u.User)
                                                           .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId == null && u.IsNews == false && u.User.Role.Name.Equals("Admin"));
            if (existingParentFaq == null)
            {
                throw new KeyNotFoundException("Parent Faq not found");
            }

            var childFaqList = await GetAllChildFaqByParentIdForAdminAsync(existingParentFaq.NewsFaqid);
            if (childFaqList != null && childFaqList.Any())
            {
                foreach (var childFaq in childFaqList)
                {
                    childFaq.Status = newsFaq.Status ?? childFaq.Status;
                    childFaq.UpdatedAt = GetVietnamTime();
                }

                _context.NewsFaqs.UpdateRange(childFaqList);
            }

            existingParentFaq.Title = newsFaq.Title ?? existingParentFaq.Title;
            existingParentFaq.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingParentFaq.NewsFaqcontent;
            existingParentFaq.Image = newsFaq.Image ?? existingParentFaq.Image;
            existingParentFaq.UserId = newsFaq.UserId ?? existingParentFaq.UserId;
            existingParentFaq.Status = newsFaq.Status ?? existingParentFaq.Status;
            existingParentFaq.UpdatedAt = GetVietnamTime();

            _context.NewsFaqs.Update(existingParentFaq);
            await _context.SaveChangesAsync();
            return existingParentFaq;

        }

        public async Task<IEnumerable<NewsFaq>> GetAllChildFaqByParentIdForAdminAsync(int? parentId)
        {
            return await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role).Include(u => u.User).Where(n => n.ParentId == parentId && n.IsNews == false && n.User.Role.Name.Equals("Admin")).ToListAsync();
        }

        public async Task<NewsFaq> DeleteParentFaqForAdminAsync(int faqId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parentFaq = await GetParentFaqByIdForAdminAsync(faqId);
                if (parentFaq == null)
                {
                    throw new KeyNotFoundException("Parent Faq not found in the list.");
                }

                // Xóa tất cả PolicyRoles liên quan đến Policy cha
                var parentFaqRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == faqId).ToList();
                if (parentFaqRoles.Any())
                {
                    _context.NewsFaqroles.RemoveRange(parentFaqRoles);
                }

                var childFaqs = await GetAllChildFaqByParentIdForAdminAsync(faqId);
                if (childFaqs != null && childFaqs.Any())
                {
                    // Xóa PolicyRoles liên quan đến các Policy con
                    var childFaqsIds = childFaqs.Select(cp => cp.NewsFaqid).ToList();
                    var childFaqsRoles = _context.NewsFaqroles.Where(pr => childFaqsIds.Contains((int)pr.NewsFaqid)).ToList();
                    if (childFaqsRoles.Any())
                    {
                        _context.NewsFaqroles.RemoveRange(childFaqsRoles);
                    }

                    // Xóa các Policy con
                    _context.NewsFaqs.RemoveRange(childFaqs);
                }

                // Xóa Policy cha
                _context.NewsFaqs.Remove(parentFaq);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return parentFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error deleting parent faq: {ex.Message}");
            }
        }

        // Admin - Child News Management
        public async Task<IEnumerable<NewsFaq>> GetAllChildFaqForAdminAsync(int parentId, string? title, int? roleId, string? status)
        {
            IQueryable<NewsFaq> query = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                       .Include(u => u.User)
                                       .Where(u => u.IsNews == false && u.ParentId == parentId && u.User.Role.Name.Equals("Admin"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var childFaq = await query.ToListAsync();

            if (childFaq == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Child Faq not found");
            }

            var sortedChildFaq = childFaq.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.NewsFaqid)
                                           .ToList();

            return sortedChildFaq;
        }

        public async Task<NewsFaq> GetChildFaqByIdForAdminAsync(int faqId)
        {
            var childFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                  .Include(u => u.User)
                                                  .FirstOrDefaultAsync(u => u.NewsFaqid == faqId && u.ParentId != null && u.IsNews == false && u.User.Role.Name.Equals("Admin"));
            if (childFaq == null)
            {
                throw new KeyNotFoundException("Child Faq not found");
            }
            return childFaq;
        }

        public async Task<NewsFaq> AddChildFaqForAdminAsync(NewsFaq newsFaq)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var parentFaq = await GetParentFaqByIdForAdminAsync(newsFaq.ParentId.Value);
                if (parentFaq == null)
                {
                    throw new KeyNotFoundException("Parent Faq not found.");
                }

                newsFaq.CreatedAt = GetVietnamTime();
                newsFaq.UpdatedAt = GetVietnamTime();
                newsFaq.Status = "Active"; // Set trạng thái mặc định là Active
                newsFaq.IsNews = false;
                await _context.NewsFaqs.AddAsync(newsFaq);
                await _context.SaveChangesAsync();

                var parentFaqRoles = await _context.NewsFaqroles
                    .Where(pr => pr.NewsFaqid == parentFaq.NewsFaqid)
                    .Select(pr => pr.RoleId)
                    .ToListAsync();

                if (parentFaqRoles != null && parentFaqRoles.Any())
                {
                    foreach (var roleId in parentFaqRoles)
                    {
                        var newsFaqrole = new NewsFaqrole
                        {
                            NewsFaqid = newsFaq.NewsFaqid,
                            RoleId = roleId == 0 ? null : roleId
                        };
                        await _context.NewsFaqroles.AddAsync(newsFaqrole);
                    }
                    await _context.SaveChangesAsync();
                }

                // Commit transaction
                await transaction.CommitAsync();

                return newsFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding child faqs with roles: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateChildFaqForAdminAsync(NewsFaq newsFaq)
        {
            var existingChildFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                          .Include(u => u.User)
                                                          .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId != null && u.IsNews == false && u.User.Role.Name.Equals("Admin"));
            if (existingChildFaq == null)
            {
                throw new KeyNotFoundException("Child Faq not found");
            }

            existingChildFaq.Title = newsFaq.Title ?? existingChildFaq.Title;
            existingChildFaq.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingChildFaq.NewsFaqcontent;
            existingChildFaq.Image = newsFaq.Image ?? existingChildFaq.Image;
            existingChildFaq.UserId = newsFaq.UserId ?? existingChildFaq.UserId;
            existingChildFaq.ParentId = newsFaq.ParentId ?? existingChildFaq.ParentId;
            existingChildFaq.Status = newsFaq.Status ?? existingChildFaq.Status;
            existingChildFaq.UpdatedAt = GetVietnamTime();

            _context.NewsFaqs.Update(existingChildFaq);
            await _context.SaveChangesAsync();
            return existingChildFaq;

        }

        public async Task<NewsFaq> DeleteChildFaqForAdminAsync(int faqId)
        {
            var childFaq = await GetChildFaqByIdForAdminAsync(faqId);
            if (childFaq == null)
            {
                throw new KeyNotFoundException("Child Faq not found in the list.");
            }

            var faqsRoles = _context.NewsFaqroles.Where(dr => dr.NewsFaqid == faqId).ToList();
            if (faqsRoles.Any())
            {
                _context.NewsFaqroles.RemoveRange(faqsRoles);
            }

            childFaq.DeletedAt = GetVietnamTime();
            _context.NewsFaqs.Remove(childFaq);
            await _context.SaveChangesAsync();

            return childFaq;
        }

        // Doet - Parent News Management
        public async Task<IEnumerable<NewsFaq>> GetAllParentNewsForDoetAsync(string? title, int? roleId, string? status)
        {
            IQueryable<NewsFaq> query = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                       .Include(u => u.User)
                                       .Where(u => u.IsNews == true && u.ParentId == null && u.User.Role.Name.Equals("DOET"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var parentNews = await query.ToListAsync();

            if (parentNews == null)
            {
                throw new KeyNotFoundException("Parent News not found.");
            }

            var sortedParentNews = parentNews.OrderByDescending(u => u.Status == "Active")
                               .ThenByDescending(u => u.Status == "Unactive")
                               .ThenByDescending(u => u.NewsFaqid)
                               .ToList();

            return sortedParentNews;
        }

        public async Task<NewsFaq> GetParentNewsByIdForDoetAsync(int newsId)
        {
            var parentNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                    .Include(u => u.User)
                                                    .FirstOrDefaultAsync(u => u.NewsFaqid == newsId && u.ParentId == null && u.IsNews == true && u.User.Role.Name.Equals("DOET"));
            if (parentNews == null)
            {
                throw new KeyNotFoundException("Parent News not found");
            }
            return parentNews;
        }

        public async Task<NewsFaq> AddParentNewsForDoetAsync(NewsFaq newsFaq, List<int?> roleIds)
        {
            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                newsFaq.CreatedAt = GetVietnamTime();
                newsFaq.UpdatedAt = GetVietnamTime();
                newsFaq.Status = "Active"; // Set trạng thái mặc định là Active
                newsFaq.IsNews = true;
                await _context.NewsFaqs.AddAsync(newsFaq);
                await _context.SaveChangesAsync();

                // Thêm các bản ghi vào bảng DocumentRoles
                foreach (var roleId in roleIds)
                {
                    var newsFaqrole = new NewsFaqrole
                    {
                        NewsFaqid = newsFaq.NewsFaqid,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.NewsFaqroles.AddAsync(newsFaqrole);
                }

                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return newsFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding parent news with roles: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateParentNewsForDoetAsync(NewsFaq newsFaq, List<int?> newRoleIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingParentNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                            .Include(u => u.User)
                                                            .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId == null && u.IsNews == true && u.User.Role.Name.Equals("DOET"));
                if (existingParentNews == null)
                {
                    throw new KeyNotFoundException("Parent News not found");
                }

                existingParentNews.Title = newsFaq.Title ?? existingParentNews.Title;
                existingParentNews.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingParentNews.NewsFaqcontent;
                existingParentNews.Image = newsFaq.Image ?? existingParentNews.Image;
                existingParentNews.UserId = newsFaq.UserId ?? existingParentNews.UserId;
                existingParentNews.Status = newsFaq.Status ?? existingParentNews.Status;
                existingParentNews.UpdatedAt = GetVietnamTime();

                // Cập nhật PolicyRoles cho Policy cha
                if (newRoleIds != null && newRoleIds.Any())
                {
                    // Xóa các PolicyRoles hiện tại
                    var existingRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == existingParentNews.NewsFaqid).ToList();
                    _context.NewsFaqroles.RemoveRange(existingRoles);
                    await _context.SaveChangesAsync();

                    // Thêm mới PolicyRoles
                    foreach (var roleId in newRoleIds)
                    {
                        var newsFaqrole = new NewsFaqrole
                        {
                            NewsFaqid = existingParentNews.NewsFaqid,
                            RoleId = roleId == 0 ? null : roleId // Xử lý Role Guest
                        };
                        await _context.NewsFaqroles.AddAsync(newsFaqrole);
                    }
                    await _context.SaveChangesAsync();
                }

                var childNews = await GetAllChildNewsByParentIdForDoetAsync(existingParentNews.NewsFaqid);
                if (childNews.Any())
                {
                    foreach (var childNew in childNews)
                    {
                        // Cập nhật RoleIds của Policy con để khớp với RoleIds của Policy cha
                        var childExistingRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == childNew.NewsFaqid).ToList();
                        _context.NewsFaqroles.RemoveRange(childExistingRoles);

                        foreach (var roleId in newRoleIds)
                        {
                            var childNewsrole = new NewsFaqrole
                            {
                                NewsFaqid = childNew.NewsFaqid,
                                RoleId = roleId == 0 ? null : roleId
                            };
                            await _context.NewsFaqroles.AddAsync(childNewsrole);
                        }

                        childNew.UpdatedAt = GetVietnamTime();
                        _context.NewsFaqs.Update(childNew);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingParentNews;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error updating parent news: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateParentNewsStatusForDoetAsync(NewsFaq newsFaq)
        {
            var existingParentNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                            .Include(u => u.User)
                                                            .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId == null && u.IsNews == true && u.User.Role.Name.Equals("DOET"));
            if (existingParentNews == null)
            {
                throw new KeyNotFoundException("Parent News not found");
            }

            // Cập nhật trạng thái cho tất cả các ChildNews có cùng ParentId
            var childNewsList = await GetAllChildNewsByParentIdForDoetAsync(existingParentNews.NewsFaqid);
            if (childNewsList != null && childNewsList.Any())
            {
                foreach (var childNews in childNewsList)
                {
                    childNews.Status = newsFaq.Status ?? childNews.Status;
                    childNews.UpdatedAt = GetVietnamTime();
                }

                _context.NewsFaqs.UpdateRange(childNewsList);
            }

            existingParentNews.Title = newsFaq.Title ?? existingParentNews.Title;
            existingParentNews.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingParentNews.NewsFaqcontent;
            existingParentNews.Image = newsFaq.Image ?? existingParentNews.Image;
            existingParentNews.UserId = newsFaq.UserId ?? existingParentNews.UserId;
            existingParentNews.Status = newsFaq.Status ?? existingParentNews.Status;
            existingParentNews.UpdatedAt = GetVietnamTime();

            _context.NewsFaqs.Update(existingParentNews);
            await _context.SaveChangesAsync();
            return existingParentNews;

        }

        public async Task<IEnumerable<NewsFaq>> GetAllChildNewsByParentIdForDoetAsync(int? parentId)
        {
            return await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role).Include(u => u.User).Where(n => n.ParentId == parentId && n.IsNews == true && n.User.Role.Name.Equals("DOET")).ToListAsync();
        }

        public async Task<NewsFaq> DeleteParentNewsForDoetAsync(int newsId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Tìm kiếm parent news theo ID
                var parentNews = await GetParentNewsByIdForDoetAsync(newsId);
                if (parentNews == null)
                {
                    throw new KeyNotFoundException("Parent News not found in the list.");
                }

                // Xóa tất cả PolicyRoles liên quan đến Policy cha
                var parentNewsRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == newsId).ToList();
                if (parentNewsRoles.Any())
                {
                    _context.NewsFaqroles.RemoveRange(parentNewsRoles);
                }

                // Lấy tất cả các child news có ParentId trùng với newsId của parent
                var childNews = await GetAllChildNewsByParentIdForDoetAsync(newsId);
                if (childNews != null && childNews.Any())
                {
                    // Xóa PolicyRoles liên quan đến các Policy con
                    var childNewsIds = childNews.Select(cp => cp.NewsFaqid).ToList();
                    var childNewsRoles = _context.NewsFaqroles.Where(pr => childNewsIds.Contains((int)pr.NewsFaqid)).ToList();
                    if (childNewsRoles.Any())
                    {
                        _context.NewsFaqroles.RemoveRange(childNewsRoles);
                    }

                    // Xóa các Policy con
                    _context.NewsFaqs.RemoveRange(childNews);
                }

                // Xóa Policy cha
                _context.NewsFaqs.Remove(parentNews);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return parentNews;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error deleting parent news: {ex.Message}");
            }
        }

        // Doet - Child News Management
        public async Task<IEnumerable<NewsFaq>> GetAllChildNewsForDoetAsync(int parentId, string? title, int? roleId, string? status)
        {
            IQueryable<NewsFaq> query = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                   .Include(u => u.User)
                                                   .Where(u => u.IsNews == true && u.ParentId == parentId && u.User.Role.Name.Equals("DOET"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var childNews = await query.ToListAsync();

            if (childNews == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Child News not found");
            }

            var sortedChildNews = childNews.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.NewsFaqid)
                                           .ToList();

            return sortedChildNews;
        }

        public async Task<NewsFaq> GetChildNewsByIdForDoetAsync(int newsId)
        {
            var childNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                    .Include(u => u.User)
                                                    .FirstOrDefaultAsync(u => u.NewsFaqid == newsId && u.ParentId != null && u.IsNews == true && u.User.Role.Name.Equals("DOET"));
            if (childNews == null)
            {
                throw new KeyNotFoundException("Child News not found");
            }
            return childNews;
        }

        public async Task<NewsFaq> AddChildNewsForDoetAsync(NewsFaq newsFaq)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var parentNews = await GetParentNewsByIdForDoetAsync(newsFaq.ParentId.Value);
                if (parentNews == null)
                {
                    throw new KeyNotFoundException("Parent News not found.");
                }

                newsFaq.CreatedAt = GetVietnamTime();
                newsFaq.UpdatedAt = GetVietnamTime();
                newsFaq.Status = "Active"; // Set trạng thái mặc định là Active
                newsFaq.IsNews = true;
                await _context.NewsFaqs.AddAsync(newsFaq);
                await _context.SaveChangesAsync();

                var parentNewsRoles = await _context.NewsFaqroles
                    .Where(pr => pr.NewsFaqid == parentNews.NewsFaqid)
                    .Select(pr => pr.RoleId)
                    .ToListAsync();

                if (parentNewsRoles != null && parentNewsRoles.Any())
                {
                    foreach (var roleId in parentNewsRoles)
                    {
                        var newsFaqrole = new NewsFaqrole
                        {
                            NewsFaqid = newsFaq.NewsFaqid,
                            RoleId = roleId == 0 ? null : roleId
                        };
                        await _context.NewsFaqroles.AddAsync(newsFaqrole);
                    }
                    await _context.SaveChangesAsync();
                }

                // Commit transaction
                await transaction.CommitAsync();

                return newsFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding child news with roles: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateChildNewsForDoetAsync(NewsFaq newsFaq)
        {
            var existingChildNews = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                           .Include(u => u.User)
                                                           .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId != null && u.IsNews == true && u.User.Role.Name.Equals("DOET"));
            if (existingChildNews == null)
            {
                throw new KeyNotFoundException("Child News not found");
            }

            existingChildNews.Title = newsFaq.Title ?? existingChildNews.Title;
            existingChildNews.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingChildNews.NewsFaqcontent;
            existingChildNews.Image = newsFaq.Image ?? existingChildNews.Image;
            existingChildNews.UserId = newsFaq.UserId ?? existingChildNews.UserId;
            existingChildNews.ParentId = newsFaq.ParentId ?? existingChildNews.ParentId;
            existingChildNews.Status = newsFaq.Status ?? existingChildNews.Status;
            existingChildNews.UpdatedAt = GetVietnamTime();

            _context.NewsFaqs.Update(existingChildNews);
            await _context.SaveChangesAsync();
            return existingChildNews;

        }

        public async Task<NewsFaq> DeleteChildNewsForDoetAsync(int newsId)
        {
            var childNews = await GetChildNewsByIdForDoetAsync(newsId);
            if (childNews == null)
            {
                throw new KeyNotFoundException("Child News not found in the list.");
            }

            var newsRoles = _context.NewsFaqroles.Where(dr => dr.NewsFaqid == newsId).ToList();
            if (newsRoles.Any())
            {
                _context.NewsFaqroles.RemoveRange(newsRoles);
            }

            childNews.DeletedAt = GetVietnamTime();
            _context.NewsFaqs.Remove(childNews);
            await _context.SaveChangesAsync();

            return childNews;
        }

        // Doet - Parent Faq Management
        public async Task<IEnumerable<NewsFaq>> GetAllParentFaqForDoetAsync(string? title, int? roleId, string? status)
        {
            IQueryable<NewsFaq> query = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                       .Include(u => u.User)
                                       .Where(u => u.IsNews == false && u.ParentId == null && u.User.Role.Name.Equals("DOET"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var parentFaq = await query.ToListAsync();

            if (parentFaq == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Parent Faq not found");
            }
            var sortedParentFaq = parentFaq.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.NewsFaqid)
                                           .ToList();

            return sortedParentFaq;
        }

        public async Task<NewsFaq> GetParentFaqByIdForDoetAsync(int faqId)
        {
            var parentFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                   .Include(u => u.User)
                                                   .FirstOrDefaultAsync(u => u.NewsFaqid == faqId && u.ParentId == null && u.IsNews == false && u.User.Role.Name.Equals("DOET"));
            if (parentFaq == null)
            {
                throw new KeyNotFoundException("Parent Faq not found");
            }
            return parentFaq;
        }

        public async Task<NewsFaq> AddParentFaqForDoetAsync(NewsFaq newsFaq, List<int?> roleIds)
        {
            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                newsFaq.CreatedAt = GetVietnamTime();
                newsFaq.UpdatedAt = GetVietnamTime();
                newsFaq.Status = "Active"; // Set trạng thái mặc định là Active
                newsFaq.IsNews = false;
                await _context.NewsFaqs.AddAsync(newsFaq);
                await _context.SaveChangesAsync();

                // Thêm các bản ghi vào bảng DocumentRoles
                foreach (var roleId in roleIds)
                {
                    var newsFaqrole = new NewsFaqrole
                    {
                        NewsFaqid = newsFaq.NewsFaqid,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.NewsFaqroles.AddAsync(newsFaqrole);
                }

                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return newsFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding parent faq with roles: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateParentFaqForDoetAsync(NewsFaq newsFaq, List<int?> faqRoleIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingParentFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                            .Include(u => u.User)
                                                            .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId == null && u.IsNews == false && u.User.Role.Name.Equals("DOET"));
                if (existingParentFaq == null)
                {
                    throw new KeyNotFoundException("Parent News not found");
                }

                existingParentFaq.Title = newsFaq.Title ?? existingParentFaq.Title;
                existingParentFaq.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingParentFaq.NewsFaqcontent;
                existingParentFaq.Image = newsFaq.Image ?? existingParentFaq.Image;
                existingParentFaq.UserId = newsFaq.UserId ?? existingParentFaq.UserId;
                existingParentFaq.Status = newsFaq.Status ?? existingParentFaq.Status;
                existingParentFaq.UpdatedAt = GetVietnamTime();

                // Cập nhật PolicyRoles cho Policy cha
                if (faqRoleIds != null && faqRoleIds.Any())
                {
                    // Xóa các PolicyRoles hiện tại
                    var existingRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == existingParentFaq.NewsFaqid).ToList();
                    _context.NewsFaqroles.RemoveRange(existingRoles);
                    await _context.SaveChangesAsync();

                    // Thêm mới PolicyRoles
                    foreach (var roleId in faqRoleIds)
                    {
                        var newsFaqrole = new NewsFaqrole
                        {
                            NewsFaqid = existingParentFaq.NewsFaqid,
                            RoleId = roleId == 0 ? null : roleId // Xử lý Role Guest
                        };
                        await _context.NewsFaqroles.AddAsync(newsFaqrole);
                    }
                    await _context.SaveChangesAsync();
                }

                var childFaqs = await GetAllChildFaqByParentIdForDoetAsync(existingParentFaq.NewsFaqid);
                if (childFaqs.Any())
                {
                    foreach (var childFaq in childFaqs)
                    {
                        // Cập nhật RoleIds của Policy con để khớp với RoleIds của Policy cha
                        var childExistingRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == childFaq.NewsFaqid).ToList();
                        _context.NewsFaqroles.RemoveRange(childExistingRoles);

                        foreach (var roleId in faqRoleIds)
                        {
                            var childNewsrole = new NewsFaqrole
                            {
                                NewsFaqid = childFaq.NewsFaqid,
                                RoleId = roleId == 0 ? null : roleId
                            };
                            await _context.NewsFaqroles.AddAsync(childNewsrole);
                        }

                        childFaq.UpdatedAt = GetVietnamTime();
                        _context.NewsFaqs.Update(childFaq);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingParentFaq;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error updating parent faq: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateParentFaqStatusForDoetAsync(NewsFaq newsFaq)
        {
            var existingParentFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                           .Include(u => u.User)
                                                           .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId == null && u.IsNews == false && u.User.Role.Name.Equals("DOET"));
            if (existingParentFaq == null)
            {
                throw new KeyNotFoundException("Parent Faq not found");
            }

            var childFaqList = await GetAllChildFaqByParentIdForDoetAsync(existingParentFaq.NewsFaqid);
            if (childFaqList != null && childFaqList.Any())
            {
                foreach (var childFaq in childFaqList)
                {
                    childFaq.Status = newsFaq.Status ?? childFaq.Status;
                    childFaq.UpdatedAt = GetVietnamTime();
                }

                _context.NewsFaqs.UpdateRange(childFaqList);
            }

            existingParentFaq.Title = newsFaq.Title ?? existingParentFaq.Title;
            existingParentFaq.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingParentFaq.NewsFaqcontent;
            existingParentFaq.Image = newsFaq.Image ?? existingParentFaq.Image;
            existingParentFaq.UserId = newsFaq.UserId ?? existingParentFaq.UserId;
            existingParentFaq.Status = newsFaq.Status ?? existingParentFaq.Status;
            existingParentFaq.UpdatedAt = GetVietnamTime();

            _context.NewsFaqs.Update(existingParentFaq);
            await _context.SaveChangesAsync();
            return existingParentFaq;

        }

        public async Task<IEnumerable<NewsFaq>> GetAllChildFaqByParentIdForDoetAsync(int? parentId)
        {
            return await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role).Include(u => u.User).Where(n => n.ParentId == parentId && n.IsNews == false && n.User.Role.Name.Equals("DOET")).ToListAsync();
        }

        public async Task<NewsFaq> DeleteParentFaqForDoetAsync(int faqId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parentFaq = await GetParentFaqByIdForDoetAsync(faqId);
                if (parentFaq == null)
                {
                    throw new KeyNotFoundException("Parent Faq not found in the list.");
                }

                // Xóa tất cả PolicyRoles liên quan đến Policy cha
                var parentFaqRoles = _context.NewsFaqroles.Where(pr => pr.NewsFaqid == faqId).ToList();
                if (parentFaqRoles.Any())
                {
                    _context.NewsFaqroles.RemoveRange(parentFaqRoles);
                }

                var childFaqs = await GetAllChildFaqByParentIdForDoetAsync(faqId);
                if (childFaqs != null && childFaqs.Any())
                {
                    // Xóa PolicyRoles liên quan đến các Policy con
                    var childFaqsIds = childFaqs.Select(cp => cp.NewsFaqid).ToList();
                    var childFaqsRoles = _context.NewsFaqroles.Where(pr => childFaqsIds.Contains((int)pr.NewsFaqid)).ToList();
                    if (childFaqsRoles.Any())
                    {
                        _context.NewsFaqroles.RemoveRange(childFaqsRoles);
                    }

                    // Xóa các Policy con
                    _context.NewsFaqs.RemoveRange(childFaqs);
                }

                // Xóa Policy cha
                _context.NewsFaqs.Remove(parentFaq);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return parentFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error deleting parent faq: {ex.Message}");
            }
        }

        // doet - Child News Management
        public async Task<IEnumerable<NewsFaq>> GetAllChildFaqForDoetAsync(int parentId, string? title, int? roleId, string? status)
        {
            IQueryable<NewsFaq> query = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                       .Include(u => u.User)
                                       .Where(u => u.IsNews == false && u.ParentId == parentId && u.User.Role.Name.Equals("DOET"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var childFaq = await query.ToListAsync();

            if (childFaq == null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new KeyNotFoundException("Child Faq not found");
            }

            var sortedChildFaq = childFaq.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.NewsFaqid)
                                           .ToList();

            return sortedChildFaq;
        }

        public async Task<NewsFaq> GetChildFaqByIdForDoetAsync(int faqId)
        {
            var childFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                  .Include(u => u.User)
                                                  .FirstOrDefaultAsync(u => u.NewsFaqid == faqId && u.ParentId != null && u.IsNews == false && u.User.Role.Name.Equals("DOET"));
            if (childFaq == null)
            {
                throw new KeyNotFoundException("Child Faq not found");
            }
            return childFaq;
        }

        public async Task<NewsFaq> AddChildFaqForDoetAsync(NewsFaq newsFaq)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var parentFaq = await GetParentFaqByIdForDoetAsync(newsFaq.ParentId.Value);
                if (parentFaq == null)
                {
                    throw new KeyNotFoundException("Parent Faq not found.");
                }

                newsFaq.CreatedAt = GetVietnamTime();
                newsFaq.UpdatedAt = GetVietnamTime();
                newsFaq.Status = "Active"; // Set trạng thái mặc định là Active
                newsFaq.IsNews = false;
                await _context.NewsFaqs.AddAsync(newsFaq);
                await _context.SaveChangesAsync();

                var parentFaqRoles = await _context.NewsFaqroles
                    .Where(pr => pr.NewsFaqid == parentFaq.NewsFaqid)
                    .Select(pr => pr.RoleId)
                    .ToListAsync();

                if (parentFaqRoles != null && parentFaqRoles.Any())
                {
                    foreach (var roleId in parentFaqRoles)
                    {
                        var newsFaqrole = new NewsFaqrole
                        {
                            NewsFaqid = newsFaq.NewsFaqid,
                            RoleId = roleId == 0 ? null : roleId
                        };
                        await _context.NewsFaqroles.AddAsync(newsFaqrole);
                    }
                    await _context.SaveChangesAsync();
                }

                // Commit transaction
                await transaction.CommitAsync();

                return newsFaq;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding child faqs with roles: {ex.Message}");
            }
        }

        public async Task<NewsFaq> UpdateChildFaqForDoetAsync(NewsFaq newsFaq)
        {
            var existingChildFaq = await _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                                          .Include(u => u.User)
                                                          .FirstOrDefaultAsync(u => u.NewsFaqid == newsFaq.NewsFaqid && u.ParentId != null && u.IsNews == false && u.User.Role.Name.Equals("DOET"));
            if (existingChildFaq == null)
            {
                throw new KeyNotFoundException("Child Faq not found");
            }

            existingChildFaq.Title = newsFaq.Title ?? existingChildFaq.Title;
            existingChildFaq.NewsFaqcontent = newsFaq.NewsFaqcontent ?? existingChildFaq.NewsFaqcontent;
            existingChildFaq.Image = newsFaq.Image ?? existingChildFaq.Image;
            existingChildFaq.UserId = newsFaq.UserId ?? existingChildFaq.UserId;
            existingChildFaq.ParentId = newsFaq.ParentId ?? existingChildFaq.ParentId;
            existingChildFaq.Status = newsFaq.Status ?? existingChildFaq.Status;
            existingChildFaq.UpdatedAt = GetVietnamTime();

            _context.NewsFaqs.Update(existingChildFaq);
            await _context.SaveChangesAsync();
            return existingChildFaq;

        }

        public async Task<NewsFaq> DeleteChildFaqForDoetAsync(int faqId)
        {
            var childFaq = await GetChildFaqByIdForDoetAsync(faqId);
            if (childFaq == null)
            {
                throw new KeyNotFoundException("Child Faq not found in the list.");
            }

            var faqsRoles = _context.NewsFaqroles.Where(dr => dr.NewsFaqid == faqId).ToList();
            if (faqsRoles.Any())
            {
                _context.NewsFaqroles.RemoveRange(faqsRoles);
            }

            childFaq.DeletedAt = GetVietnamTime();
            _context.NewsFaqs.Remove(childFaq);
            await _context.SaveChangesAsync();

            return childFaq;
        }

        // Common - News
        public async Task<IEnumerable<NewsFaq>> GetAllNewsAsync(string role, string? title)
        {
            var newsQuery = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                             .Include(n => n.User)
                                             .Where(n => n.IsNews == true && n.ParentId == null && n.Status == "Active");
            if (role == "guest")
            {
                newsQuery = newsQuery.Where(d => d.NewsFaqroles.All(dr => dr.RoleId == null));
            }
            else
            {
                newsQuery = newsQuery.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            if (!string.IsNullOrEmpty(title))
            {
                title = title.ToLower();
                newsQuery = newsQuery.Where(n => n.Title.ToLower().Contains(title));
            }

            newsQuery = newsQuery.OrderByDescending(d => d.NewsFaqroles.Any(dr => dr.Role != null && dr.Role.Name.Equals(role))) // Vai trò đăng nhập lên đầu
                .ThenBy(d => d.NewsFaqroles.Any(dr => dr.RoleId == null)) // Sau đó là guest
                .ThenByDescending(d => d.NewsFaqid); // Sắp xếp theo DocumentId giảm dần

            var newsList = await newsQuery.ToListAsync();

            if (newsList == null)
            {
                throw new KeyNotFoundException("No news found for the specified role.");
            }

            return newsList;
        }

        public async Task<NewsFaq> GetNewsDetailAsync(int? newsId, string role)
        {
            var newsQuery = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                             .Include(n => n.User)
                                             .Where(n => n.IsNews == true && n.Status == "Active");

            if (role == "guest")
            {
                newsQuery = newsQuery.Where(d => d.NewsFaqroles.All(dr => dr.RoleId == null));
            }
            else
            {
                newsQuery = newsQuery.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            var newsDetail = await newsQuery.FirstOrDefaultAsync(n => n.NewsFaqid == newsId);

            if (newsDetail == null)
            {
                throw new KeyNotFoundException("News detail not found.");
            }

            return newsDetail;
        }

        public async Task<IEnumerable<NewsFaq>> GetAllNewsContentForNewsParentAsync(int? parentId, string role)
        {
            var parentNewsList = await GetAllNewsAsync(role, null);
            var parentNewsExists = parentNewsList.Any(n => n.NewsFaqid == parentId);

            if (!parentNewsExists)
            {
                throw new KeyNotFoundException($"Not found news parent with id: {parentId}");
            }

            var newsQuery = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                              .Include(n => n.User)
                                              .Where(n => n.IsNews == true && n.ParentId == parentId && n.Status == "Active");

            if (role == "guest")
            {
                newsQuery = newsQuery.Where(d => d.NewsFaqroles.All(dr => dr.RoleId == null));
            }
            else
            {
                newsQuery = newsQuery.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            var newsList = await newsQuery.ToListAsync();

            var sortedNews = newsList.OrderBy(n => n.NewsFaqid).ToList();

            return sortedNews;
        }

        // Common - Faqs
        public async Task<IEnumerable<NewsFaq>> GetAllFaqsAsync(string role, string? title)
        {
            var faqsQuery = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                             .Include(n => n.User)
                                             .Where(n => n.IsNews == false && n.ParentId == null && n.Status == "Active");
            if (role == "guest")
            {
                faqsQuery = faqsQuery.Where(d => d.NewsFaqroles.All(dr => dr.RoleId == null));
            }
            else
            {
                faqsQuery = faqsQuery.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            if (!string.IsNullOrEmpty(title))
            {
                title = title.ToLower();
                faqsQuery = faqsQuery.Where(n => n.Title.ToLower().Contains(title));
            }

            faqsQuery = faqsQuery.OrderByDescending(d => d.NewsFaqroles.Any(dr => dr.Role != null && dr.Role.Name.Equals(role))) // Vai trò đăng nhập lên đầu
                .ThenBy(d => d.NewsFaqroles.Any(dr => dr.RoleId == null)) // Sau đó là guest
                .ThenByDescending(d => d.NewsFaqid); // Sắp xếp theo DocumentId giảm dần

            var faqsList = await faqsQuery.ToListAsync();

            if (faqsList == null)
            {
                throw new KeyNotFoundException("No faqs found for the specified role.");
            }

            return faqsList;
        }

        public async Task<NewsFaq> GetFaqsDetailAsync(int? faqId, string role)
        {
            var faqQuery = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                             .Include(n => n.User)
                                             .Where(n => n.IsNews == false && n.Status == "Active");

            if (role == "guest")
            {
                faqQuery = faqQuery.Where(d => d.NewsFaqroles.All(dr => dr.RoleId == null));
            }
            else
            {
                faqQuery = faqQuery.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            var faqDetail = await faqQuery.FirstOrDefaultAsync(n => n.NewsFaqid == faqId);

            if (faqDetail == null)
            {
                throw new KeyNotFoundException("Faqs detail not found.");
            }

            return faqDetail;
        }

        public async Task<IEnumerable<NewsFaq>> GetAllFaqsContentForFaqsParentAsync(int? parentId, string role)
        {
            var parentFaqsList = await GetAllFaqsAsync(role, null);
            var parentFaqsExists = parentFaqsList.Any(n => n.NewsFaqid == parentId);

            if (!parentFaqsExists)
            {
                throw new KeyNotFoundException($"Not found faqs parent with id: {parentId}");
            }

            var faqsQuery = _context.NewsFaqs.Include(u => u.NewsFaqroles).ThenInclude(u => u.Role)
                                              .Include(n => n.User)
                                              .Where(n => n.IsNews == false && n.ParentId == parentId && n.Status == "Active");

            if (role == "guest")
            {
                faqsQuery = faqsQuery.Where(d => d.NewsFaqroles.All(dr => dr.RoleId == null));
            }
            else
            {
                faqsQuery = faqsQuery.Where(d => d.NewsFaqroles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            var newsList = await faqsQuery.ToListAsync();

            var sortedNews = newsList.OrderBy(n => n.NewsFaqid).ToList();

            return sortedNews;
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
