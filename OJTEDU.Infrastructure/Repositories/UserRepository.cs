using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OJTEDU.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly OJTEDU_DB_V1Context _context;

        public UserRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found");
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user: " + ex.Message, ex);
            }
        }

        // Lấy thông tin người dùng dựa trên ID
        public async Task<User> GetUserByIdForAdminAsync(int userId)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId && u.Status != "Deleted");
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            return user;
        }

        // Lấy danh sách tất cả người dùng
        public async Task<IEnumerable<User>> GetAllUsersForAdminAsync(string? name, int? roleId, string? status)
        {
            IQueryable<User> query = _context.Users.Include(u => u.Role)
                                           .Where(u => u.Status != "Deleted");

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(name));
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
            var users = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (users == null)
            {
                throw new KeyNotFoundException("Users not found.");
            }

            // Apply sorting: Admins first, Active users next, then Unactive
            var sortedUsers = users.OrderByDescending(u => u.Role.Name == "Admin")
                                   .ThenByDescending(u => u.Status == "Active")
                                   .ThenBy(u => u.Status == "Unactive")
                                   .ToList();

            return sortedUsers;
        }

        // Thêm mới người dùng
        public async Task<User> AddUserForAdminAsync(User user)
        {
            // Kiểm tra xem RoleId có hợp lệ không
            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException("Role does not exist.");
            }

            // Kiểm tra xem vai trò có phải là "Mentor" hay không
            if (role.Name == "Mentor")
            {
                // Nếu vai trò là "Mentor", ném ngoại lệ
                throw new InvalidOperationException("Cannot add a user with the 'Mentor' role.");
            }

            // Kiểm tra xem email đã tồn tại chưa
            var existingUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUserByEmail != null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new InvalidOperationException("A user with the same email already exists.");
            }

            // Kiểm tra xem user code đã tồn tại chưa
            var existingUserByUserCode = await _context.Users.FirstOrDefaultAsync(u => u.UserCode == user.UserCode);
            if (existingUserByUserCode != null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new InvalidOperationException("A user with the same UserCode already exists.");
            }

            // Nếu cả email và user code chưa tồn tại, tiếp tục thêm người dùng mới
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.Status = "Active"; // Set trạng thái mặc định là Active
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            if (role.Name == "Student")
            {
                // Tạo đối tượng Student và gán UserId
                var student = new Student
                {
                    UserId = user.UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now

                };
                await _context.Students.AddAsync(student);
            }
            else if (role.Name == "Company")
            {
                // Tạo đối tượng Company và gán UserId
                var company = new Company
                {
                    UserId = user.UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _context.Companies.AddAsync(company);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return user; // Trả về người dùng đã thêm
        }

        public async Task AddUsersForAdminAsync(IEnumerable<User> users)
        {
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }

        public async Task AddStudentsAndCompaniesForAdminAsync(IEnumerable<Student> studentUsers, IEnumerable<Company> companyUsers)
        {
            await _context.Students.AddRangeAsync(studentUsers);
            await _context.Companies.AddRangeAsync(companyUsers);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserCodeExistsAsync(string userCode)
        {
            return await _context.Users.AnyAsync(u => u.UserCode == userCode);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Cập nhật thông tin người dùng
        public async Task<User> UpdateUserForAdminAsync(User user)
        {
            var existingUser = await GetUserByIdForAdminAsync(user.UserId);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Kiểm tra xem email đã tồn tại cho người dùng khác không
            if (existingUser.Email != user.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (emailExists)
                {
                    throw new InvalidOperationException("A user with the same email already exists.");
                }
            }

            // Kiểm tra xem user code đã tồn tại cho người dùng khác không
            if (existingUser.UserCode != user.UserCode)
            {
                var userCodeExists = await _context.Users.AnyAsync(u => u.UserCode == user.UserCode);
                if (userCodeExists)
                {
                    throw new InvalidOperationException("A user with the same UserCode already exists.");
                }
            }

            // Cập nhật các thuộc tính của người dùng
            existingUser.Email = user.Email ?? existingUser.Email;
            existingUser.Name = user.Name ?? existingUser.Name;
            existingUser.RoleId = user.RoleId ?? existingUser.RoleId;
            existingUser.Status = user.Status ?? existingUser.Status;
            existingUser.UserCode = user.UserCode ?? existingUser.UserCode;
            existingUser.Image = user.Image ?? existingUser.Image;
            existingUser.Information = user.Information ?? existingUser.Information;
            existingUser.UpdatedAt = DateTime.Now;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            return existingUser;

        }

        // Xóa người dùng mềm
        public async Task<User> SoftDeleteUserForAdminAsync(int userId)
        {
            var user = await GetUserByIdForAdminAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Status = "Deleted"; // Cập nhật trạng thái thành "Deleted"
            user.DeletedAt = DateTime.Now; // Cập nhật thời gian xóa

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task<IEnumerable<User>> GetAllUsersStoredAsync(string? name, int? roleId)
        {
            IQueryable<User> query = _context.Users.Include(u => u.Role)
                                                      .Where(u => u.Status == "Deleted");

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(name));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.RoleId == roleId.Value);
            }

            // Fetch the filtered result from the database
            var users = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (users == null)
            {
                throw new KeyNotFoundException("Users Stored not found.");
            }

            var sortedUsers = users.OrderByDescending(u => u.UpdatedAt)
                                   .ToList();

            return sortedUsers;
        }

        public async Task<User> GetUserStoredByIdForAdminAsync(int userId)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId && u.Status == "Deleted");
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            return user;
        }

        public async Task<User> HardDeleteUserStoredAsync(int userId)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId && u.Status == "Deleted");
            if (user == null)
            {
                throw new KeyNotFoundException("User not found in the stored user list.");
            }
            if (user.Status != "Deleted")
            {
                throw new InvalidOperationException("Cannot permanently delete the user because it does not exist in the stored user list.");
            }

            // Xóa các bảng liên quan trước khi xóa user
            var newsFAQs = _context.NewsFaqs.Where(n => n.UserId == userId);
            var documents = _context.Documents.Where(d => d.UniversityId == userId);
            var document2s = _context.Documents.Where(d => d.UserId == userId);
            var banners = _context.Banners.Where(b => b.UserId == userId);
            var policies = _context.Policies.Where(p => p.UserId == userId);
            var companies = _context.Companies.Where(p => p.UserId == userId);
            var students = _context.Students.Where(p => p.UserId == userId);
            var messages = _context.Messages.Where(p => p.UniversiryId == userId);
            var evaluations = _context.Evaluations.Where(e => e.LecturerId == userId);
            var feedbacks = _context.Feedbacks.Where(f => f.UniversityId == userId);
            var groupChats = _context.GroupChats.Where(gc => gc.UniversityId == userId);
            var internships = _context.Internships.Where(i => i.LecturerId == userId);
            var messageGroups = _context.MessageGroups.Where(mg => mg.UniversityId == userId);
            var notifications = _context.Notifications.Where(n => n.UniversityId == userId);
            var supportRequests = _context.SupportRequests.Where(sr => sr.UniversityId == userId);
            var workingReports = _context.WorkingReports.Where(wr => wr.LecturerId == userId);
            var companyProposals = _context.CompanyProposals.Where(cp => cp.UniversityId == userId);
            var internshipProcesses = _context.InternshipProcesses.Where(cp => cp.CreatedBy == userId);

            _context.Banners.RemoveRange(banners);
            _context.Documents.RemoveRange(documents);
            _context.Messages.RemoveRange(messages);
            _context.Students.RemoveRange(students);
            _context.Companies.RemoveRange(companies);
            _context.Evaluations.RemoveRange(evaluations);
            _context.Feedbacks.RemoveRange(feedbacks);
            _context.GroupChats.RemoveRange(groupChats);
            _context.Internships.RemoveRange(internships);
            _context.MessageGroups.RemoveRange(messageGroups);
            _context.NewsFaqs.RemoveRange(newsFAQs);
            _context.Notifications.RemoveRange(notifications);
            _context.Policies.RemoveRange(policies);
            _context.SupportRequests.RemoveRange(supportRequests);
            _context.WorkingReports.RemoveRange(workingReports);
            _context.CompanyProposals.RemoveRange(companyProposals);
            _context.InternshipProcesses.RemoveRange(internshipProcesses);

            user.DeletedAt = DateTime.Now; // Cập nhật thời gian xóa

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> RestoreUserStoredAsync(int userId)
        {
            var user = await GetUserStoredByIdForAdminAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found in the stored user list.");
            }
            if (user.Status != "Deleted")
            {
                throw new InvalidOperationException("Cannot restore the user because it does not exist in the stored user list.");
            }

            user.Status = "Active";
            user.DeletedAt = null;
            user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByIdForDoetAsync(int userId)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name != "Admin" && u.Status != "Deleted");
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            return user;
        }

        // Lấy danh sách tất cả người dùng
        public async Task<IEnumerable<User>> GetAllUsersForDoetAsync(string? name, int? roleId, string? status)
        {
            IQueryable<User> query = _context.Users.Include(u => u.Role)
                                           .Where(u => u.Status != "Deleted" && u.Role.Name != "Admin");

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(name));
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
            var users = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (users == null)
            {
                throw new KeyNotFoundException("Users not found.");
            }

            // Apply sorting: Admins first, Active users next, then Unactive
            var sortedUsers = users.OrderByDescending(u => u.Role.Name == "DOET")
                                   .ThenByDescending(u => u.Status == "Active")
                                   .ThenBy(u => u.Status == "Unactive")
                                   .ToList();

            return sortedUsers;
        }

        // Thêm mới người dùng
        public async Task<User> AddUserForDoetAsync(User user)
        {
            // Kiểm tra xem RoleId có hợp lệ không
            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException("Role does not exist.");
            }

            // Kiểm tra xem vai trò có phải là "Admin" hay "Mentor" hay không
            if (role.Name == "Admin" || role.Name == "Mentor")
            {
                // Nếu vai trò là "Admin" hoặc "DOET" hoac "Mentor", ném ngoại lệ
                throw new InvalidOperationException("Cannot add a user with the 'Admin' or 'Mentor' role.");
            }

            // Kiểm tra xem email đã tồn tại chưa
            var existingUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUserByEmail != null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new InvalidOperationException("A user with the same email already exists.");
            }

            // Kiểm tra xem user code đã tồn tại chưa
            var existingUserByUserCode = await _context.Users.FirstOrDefaultAsync(u => u.UserCode == user.UserCode);
            if (existingUserByUserCode != null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new InvalidOperationException("A user with the same UserCode already exists.");
            }

            // Nếu cả email và user code chưa tồn tại, tiếp tục thêm người dùng mới
            user.CreatedAt = DateTime.Now;
            user.Status = "Active"; // Set trạng thái mặc định là Active
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            if (role.Name == "Student")
            {
                // Tạo đối tượng Student và gán UserId
                var student = new Student
                {
                    UserId = user.UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now

                };
                await _context.Students.AddAsync(student);
            }
            else if (role.Name == "Company")
            {
                // Tạo đối tượng Company và gán UserId
                var company = new Company
                {
                    UserId = user.UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _context.Companies.AddAsync(company);


                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
            }

            return user; // Trả về người dùng đã thêm
        }

        public async Task AddUsersForDoetAsync(IEnumerable<User> users)
        {
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }

        public async Task AddStudentsAndCompaniesForDoetAsync(IEnumerable<Student> studentUsers, IEnumerable<Company> companyUsers)
        {
            await _context.Students.AddRangeAsync(studentUsers);
            await _context.Companies.AddRangeAsync(companyUsers);
            await _context.SaveChangesAsync();
        }

        // Cập nhật thông tin người dùng
        public async Task<User> UpdateUserForDoetAsync(User user)
        {
            // Lấy danh sách người dùng được quản lý bởi DOET
            var doetUsers = await GetAllUsersForDoetAsync(null, null, null);

            // Tìm người dùng cần cập nhật từ danh sách
            var existingUser = doetUsers.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found in the list of Doet-managed users.");
            }

            // Kiểm tra vai trò mới từ RoleId
            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role != null && (role.Name == "Admin"))
            {
                // Nếu vai trò là "Admin" hoặc "DOET", ném ngoại lệ
                throw new InvalidOperationException("Cannot update the user to have the 'Admin' role.");
            }

            // Kiểm tra xem email đã tồn tại cho người dùng khác không
            if (existingUser.Email != user.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (emailExists)
                {
                    throw new InvalidOperationException("A user with the same email already exists.");
                }
            }

            // Kiểm tra xem user code đã tồn tại cho người dùng khác không
            if (existingUser.UserCode != user.UserCode)
            {
                var userCodeExists = await _context.Users.AnyAsync(u => u.UserCode == user.UserCode);
                if (userCodeExists)
                {
                    throw new InvalidOperationException("A user with the same UserCode already exists.");
                }
            }

            // Cập nhật các thuộc tính của người dùng
            existingUser.Email = user.Email ?? existingUser.Email;
            existingUser.Name = user.Name ?? existingUser.Name;
            existingUser.RoleId = user.RoleId ?? existingUser.RoleId;
            existingUser.Status = user.Status ?? existingUser.Status;
            existingUser.UserCode = user.UserCode ?? existingUser.UserCode;
            existingUser.Image = user.Image ?? existingUser.Image;
            existingUser.Information = user.Information ?? existingUser.Information;
            existingUser.UpdatedAt = DateTime.Now;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            return existingUser;

        }

        // Xóa người dùng mềm
        public async Task<User> SoftDeleteUserForDoetAsync(int userId)
        {
            // Lấy danh sách người dùng mà DOET có thể quản lý
            var doetUsers = await GetAllUsersForDoetAsync(null, null, null);

            // Kiểm tra xem người dùng có nằm trong danh sách được phép quản lý không
            var user = doetUsers.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found in the list of Doet-managed users.");
            }

            // Lấy vai trò của người dùng để kiểm tra
            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role != null && (role.Name == "Admin" || role.Name == "DOET"))
            {
                throw new InvalidOperationException("Cannot soft delete a user with the 'Admin' or 'DOET' role.");
            }

            // Cập nhật trạng thái thành "Deleted" và thời gian xóa
            user.Status = "Deleted";
            user.DeletedAt = DateTime.Now;

            // Cập nhật người dùng trong cơ sở dữ liệu
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }


        // Lấy danh sách tất cả người dùng
        public async Task<IEnumerable<User>> GetAllUsersForCompanyAsync(int companyId, string? name, int? roleId, string? status)
        {
            IQueryable<User> query = _context.Users.Include(u => u.Role)
                                                    .Where(u => u.Status != "Deleted"
                                                                && (u.Role.Name == "Mentor" || u.Role.Name == "Company")
                                                                && (u.UserId == companyId || u.ForCompany == companyId));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(name));
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
            var users = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (users == null)
            {
                throw new KeyNotFoundException("Users not found.");
            }

            // Apply sorting: Admins first, Active users next, then Unactive
            var sortedUsers = users.OrderByDescending(u => u.Role.Name == "Company")
                                   .ThenByDescending(u => u.Status == "Active")
                                   .ThenBy(u => u.Status == "Unactive")
                                   .ToList();

            return sortedUsers;
        }

        public async Task<User> GetUserByIdForCompanyAsync(int companyId, int userId)
        {
            var users = await GetAllUsersForCompanyAsync(companyId, null, null, null);

            var user = users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return user;
        }

        // Thêm mới người dùng
        public async Task<User> AddUserForCompanyAsync(int companyId, User user)
        {
            // Kiểm tra xem RoleId có hợp lệ không
            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException("Role does not exist.");
            }

            // Kiểm tra xem email đã tồn tại chưa
            var existingUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUserByEmail != null)
            {
                // Ném ngoại lệ với thông báo chi tiết
                throw new InvalidOperationException("A user with the same email already exists.");
            }

            // Nếu cả email và user code chưa tồn tại, tiếp tục thêm người dùng mới
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.Status = "Active"; // Set trạng thái mặc định là Active
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var addedUserId = user.UserId;
            var userCodeCompany = await _context.Users.Where(u => u.UserId == companyId).Select(u => u.UserCode).FirstOrDefaultAsync();

            user.UserCode = $"Mentor_{addedUserId}_{userCodeCompany}";
            // Cập nhật lại UserCode
            _context.Users.Update(user);
            await _context.SaveChangesAsync();


            var company = new Company
            {
                UserId = addedUserId, // Gán UserId của Mentor cho Company
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu


            return user; // Trả về người dùng đã thêm
        }


        // Cập nhật thông tin người dùng
        public async Task<User> UpdateUserForCompanyAsync(int companyId, User user)
        {
            var companyUsers = await GetAllUsersForCompanyAsync(companyId, null, null, null);

            // Tìm người dùng cần cập nhật từ danh sách
            var existingUser = companyUsers.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found in the list of Company-managed users.");
            }

            // Kiểm tra xem email đã tồn tại cho người dùng khác không
            if (existingUser.Email != user.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (emailExists)
                {
                    throw new InvalidOperationException("A user with the same email already exists.");
                }
            }

            // Cập nhật các thuộc tính của người dùng
            existingUser.Email = user.Email ?? existingUser.Email;
            existingUser.Name = user.Name ?? existingUser.Name;
            existingUser.RoleId = user.RoleId ?? existingUser.RoleId;
            existingUser.Status = user.Status ?? existingUser.Status;
            existingUser.UserCode = user.UserCode ?? existingUser.UserCode;
            existingUser.Image = user.Image ?? existingUser.Image;
            existingUser.Information = user.Information ?? existingUser.Information;
            existingUser.UpdatedAt = DateTime.Now;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            return existingUser;

        }

        // Xóa người dùng mềm
        public async Task<User> SoftDeleteUserForCompanyAsync(int companyId, int userId)
        {
            var companyUsers = await GetAllUsersForCompanyAsync(companyId, null, null, null);

            var user = companyUsers.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found in the list of Company-managed users.");
            }

            // Lấy vai trò của người dùng để kiểm tra
            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role != null && role.Name == "Company")
            {
                throw new InvalidOperationException("Cannot soft delete a user with the 'Company' role.");
            }

            // Cập nhật trạng thái thành "Deleted" và thời gian xóa
            user.Status = "Deleted";
            user.DeletedAt = DateTime.Now;

            // Cập nhật người dùng trong cơ sở dữ liệu
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        //For Dean
        // Get Dean by UserId
        public async Task<User> GetDeanByUserIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Dean" );
        }

        // Get Lecturer by UserId
        public async Task<User> GetLecturerByUserIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Lecturer" );
        }

        // Get user by ID
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId );
        }

        // Get role ID by name
        public async Task<int> GetRoleIdByNameAsync(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            return role?.RoleId ?? 0;
        }

        // Get lecturer by email
        public async Task<User> GetLecturerByEmailForDeanAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Role.Name == "Lecturer");
        }

        // Create lecturer for dean
        public async Task CreateLecturerForDeanAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Update dean
        public async Task UpdateDeanAsync(User dean)
        {
            _context.Users.Update(dean);
            await _context.SaveChangesAsync();
        }

        // Update lecturer
        public async Task UpdateLecturerAsync(User lecturer)
        {
            _context.Users.Update(lecturer);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> GetLecturerListForDeanAsync(
        int assignForId,
        string? name,
        string? userCode,
        string? majorName,
        string? sortBy,
        bool isDescending)
        {
            IQueryable<User> query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Major)
                .Where(u => u.Status != "Deleted" && u.Role.Name == "Lecturer" && u.AssignForId == assignForId);

            // Tìm kiếm theo Name
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(name));
            }

            // Tìm kiếm theo UserCode
            if (!string.IsNullOrWhiteSpace(userCode))
            {
                userCode = userCode.ToLower();
                query = query.Where(u => u.UserCode.ToLower().Contains(userCode));
            }

            // Tìm kiếm theo MajorName
            if (!string.IsNullOrWhiteSpace(majorName))
            {
                majorName = majorName.ToLower();
                query = query.Where(u => u.Major.Name.ToLower().Contains(majorName));
            }

            // Sắp xếp
            switch (sortBy?.ToLower())
            {
                case "name":
                    query = isDescending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name);
                    break;
                case "usercode":
                    query = isDescending ? query.OrderByDescending(u => u.UserCode) : query.OrderBy(u => u.UserCode);
                    break;
                case "majorname":
                    query = isDescending ? query.OrderByDescending(u => u.Major.Name) : query.OrderBy(u => u.Major.Name);
                    break;
            }

            return await query.ToListAsync();
        }


        // Get lecturer details with assigned students
        public async Task<(User Lecturer, User Dean, List<Student> Students)> GetLecturerDetailsWithDeanAndStudentsAsync(int lecturerId)
        {
            // Lấy thông tin giảng viên
            var lecturer = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Major)
                .FirstOrDefaultAsync(u => u.Status != "Deleted" && u.UserId == lecturerId && u.Role.Name == "Lecturer" );

            if (lecturer == null)
            {
                return (null, null, null);
            }

            // Lấy thông tin Dean thông qua AssignForId
            var dean = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == lecturer.AssignForId && u.Role.Name == "Dean");

            // Lấy danh sách sinh viên thuộc giảng viên này
            var students = await _context.Students
                .Where(s => s.LecturerId == lecturerId)
                .Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .ToListAsync();

            return (lecturer, dean, students);
        }

        //End

        //Admin, Doet
        public async Task<List<User>> GetAllDeansAsync(string? userCode, string? name, string? departmentName, string? sortBy, bool? isDescending)
        {
            IQueryable<User> query = _context.Users
                .Where(u => u.Role.Name == "Dean" && u.Status != "Deleted" )
                .Include(u => u.Role)
                .Include(u => u.Department);

            // Lọc theo UserCode
            if (!string.IsNullOrWhiteSpace(userCode))
            {
                userCode = userCode.ToLower();
                query = query.Where(u => u.UserCode.ToLower().Contains(userCode));
            }

            // Lọc theo Name
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(name));
            }

            // Lọc theo DepartmentName
            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                departmentName = departmentName.ToLower();
                query = query.Where(u => u.Department != null && u.Department.Name.ToLower().Contains(departmentName));
            }

            // Sắp xếp theo sortBy
            switch (sortBy?.ToLower())
            {
                case "usercode":
                    query = isDescending.HasValue && isDescending.Value
                        ? query.OrderByDescending(u => u.UserCode)
                        : query.OrderBy(u => u.UserCode);
                    break;
                case "name":
                    query = isDescending.HasValue && isDescending.Value
                        ? query.OrderByDescending(u => u.Name)
                        : query.OrderBy(u => u.Name);
                    break;
                case "departmentname":
                    query = isDescending.HasValue && isDescending.Value
                        ? query.OrderByDescending(u => u.Department.Name)
                        : query.OrderBy(u => u.Department.Name);
                    break;
            }

            return await query.ToListAsync();
        }


        public async Task<(User Dean, List<User> Lecturers, List<Student> Students)> GetDeanDetailsWithLecturersAndStudentsAsync(int deanId)
        {
            // Lấy thông tin Dean
            var dean = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Role)
                .Where(u => u.Role.Name == "Dean" && u.UserId == deanId)
                .FirstOrDefaultAsync();

            if (dean == null)
            {
                throw new KeyNotFoundException("Dean not found.");
            }

            // Lấy danh sách Lecturers thuộc Dean
            var lecturers = await _context.Users
                .Where(u => u.Status != "Deleted" && u.Role.Name == "Lecturer" && u.AssignForId == deanId )
                .Include(u => u.Major)
                .ToListAsync();

            // Lấy danh sách Students do Dean trực tiếp quản lý
            var students = await _context.Students
                .Where(s => s.LecturerId == deanId && s.User.Status != "Deleted" )
                .Include(s => s.User)
                .Include(s => s.Major)
                .Include(s => s.Semester)
                .ToListAsync();

            return (dean, lecturers, students);
        }


        public async Task<List<User>> GetLecturersByIdsAsync(List<int> lecturerIds)
        {
            return await _context.Users
                .Where(u => lecturerIds.Contains(u.UserId) && u.Role.Name == "Lecturer" && u.Status != "Deleted")
                .Include(u => u.Major)
                .ToListAsync();
        }

        public async Task UpdateLecturersAsync(List<User> lecturers)
        {
            _context.Users.UpdateRange(lecturers);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllLecturerAsync(string? userCode, string? name, string? majorName, string? sortBy, bool? isDescending)
        {
            IQueryable<User> query = _context.Users
                .Where(u => u.Role.Name == "Lecturer" && u.Status != "Deleted")
                .Include(u => u.Role)
                .Include(u => u.Major);

            // Lọc theo UserCode
            if (!string.IsNullOrWhiteSpace(userCode))
            {
                userCode = userCode.ToLower();
                query = query.Where(u => u.UserCode.ToLower().Contains(userCode));
            }

            // Lọc theo Name
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(name));
            }

            // Lọc theo DepartmentName
            if (!string.IsNullOrWhiteSpace(majorName))
            {
                majorName = majorName.ToLower();
                query = query.Where(u => u.Department != null && u.Major.Name.ToLower().Contains(majorName));
            }

            // Sắp xếp theo sortBy
            switch (sortBy?.ToLower())
            {
                case "usercode":
                    query = isDescending.HasValue && isDescending.Value
                        ? query.OrderByDescending(u => u.UserCode)
                        : query.OrderBy(u => u.UserCode);
                    break;
                case "name":
                    query = isDescending.HasValue && isDescending.Value
                        ? query.OrderByDescending(u => u.Name)
                        : query.OrderBy(u => u.Name);
                    break;
                case "majorname":
                    query = isDescending.HasValue && isDescending.Value
                        ? query.OrderByDescending(u => u.Major.Name)
                        : query.OrderBy(u => u.Major.Name);
                    break;
            }

            return await query.ToListAsync();
        }
        public async Task<bool> IsDeanAssignableToDepartmentAsync(int deanId)
        {
            // Kiểm tra trong bảng Users nếu Lecturer có AssignForId trùng với deanId
            var hasLecturerAssigned = await _context.Users
                .AnyAsync(u => u.Role.Name == "Lecturer" && u.Status != "Deleted" && u.AssignForId == deanId);

            // Kiểm tra trong bảng Students nếu có LecturerId trùng với deanId
            var hasStudentWithDeanLecturer = await _context.Internships
                .AnyAsync(s => s.LecturerId == deanId);

            return !hasLecturerAssigned && !hasStudentWithDeanLecturer;
        }

        // Gán Department cho Dean
        public async Task AssignDepartmentToDeanAsync(int deanId, int departmentId)
        {
            // Lấy thông tin Dean
            var dean = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == deanId && u.Role.Name == "Dean" && u.Status != "Deleted");

            if (dean == null)
            {
                throw new KeyNotFoundException("Dean not found.");
            }

            // Kiểm tra nếu đã gán DepartmentId giống như departmentId hiện tại
            if (dean.DepartmentId == departmentId)
            {
                throw new InvalidOperationException("This dean is already assigned to the specified department.");
            }

            // Lấy thông tin Department
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == departmentId);
            if (department == null)
            {
                throw new KeyNotFoundException("Department not found.");
            }

            // Kiểm tra trạng thái của Department (chỉ cho phép cập nhật nếu Active)
            if (department.Status != "Active")
            {
                throw new InvalidOperationException("Cannot assign to an inactive department.");
            }

            // Cập nhật DepartmentId
            dean.DepartmentId = departmentId;
            dean.UpdatedAt = DateTime.Now;

            // Lưu thay đổi
            _context.Users.Update(dean);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsLecturerAssignableToMajorAsync(int lecturerId, int majorId)
        {
            // Lấy thông tin Lecturer
            var lecturer = await _context.Users
                .Include(l => l.Role)
                .FirstOrDefaultAsync(u => u.UserId == lecturerId && u.Role.Name == "Lecturer" && u.Status != "Deleted" );

            if (lecturer == null)
            {
                throw new KeyNotFoundException("Lecturer not found.");
            }

            // Nếu Lecturer có AssignForId, kiểm tra Major thuộc Department của AssignForId
            if (lecturer.AssignForId.HasValue)
            {
                // Lấy thông tin Dean (người được AssignForId)
                var dean = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == lecturer.AssignForId && u.Role.Name == "Dean" );

                if (dean == null || !dean.DepartmentId.HasValue)
                {
                    throw new InvalidOperationException("Dean assigned to the lecturer is invalid.");
                }

                // Kiểm tra Major có thuộc Department của Dean không
                var isMajorInDepartment = await _context.Majors
                    .AnyAsync(m => m.MajorId == majorId && m.DepartmentId == dean.DepartmentId);

                if (!isMajorInDepartment)
                {
                    return false;
                }
            }

            // Kiểm tra nếu Student nào đã được gán LecturerId trùng với lecturerId
            var hasStudentAssigned = await _context.Internships
                .AnyAsync(s => s.LecturerId == lecturerId);

            return !hasStudentAssigned;
        }

        // Gán Major cho Lecturer
        public async Task AssignMajorToLecturerAsync(int lecturerId, int majorId)
        {
            // Load lecturer
            var lecturer = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == lecturerId && u.Role.Name == "Lecturer" && u.Status != "Deleted");

            if (lecturer == null)
            {
                throw new KeyNotFoundException("Lecturer not found.");
            }

            // Load major
            var major = await _context.Majors.FirstOrDefaultAsync(m => m.MajorId == majorId);
            if (major == null)
            {
                throw new KeyNotFoundException("Major not found.");
            }

            if (major.Status != "Active")
            {
                throw new InvalidOperationException("Major is inactive and cannot be assigned.");
            }

            // Check if lecturer is already assigned to this major
            if (lecturer.MajorId == majorId)
            {
                // Throw exception indicating duplication
                throw new InvalidOperationException("This lecturer is already assigned to the specified major.");
            }

            lecturer.MajorId = majorId;
            lecturer.UpdatedAt = DateTime.Now;

            _context.Users.Update(lecturer);
            await _context.SaveChangesAsync();
        }
    }
}
