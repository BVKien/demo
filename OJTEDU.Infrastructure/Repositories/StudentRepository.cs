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

namespace OJTEDU.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public StudentRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // Student 
        public async Task<Student> GetStudentDetailByUserIdAsync(int? userId)
        {
            try
            {
                bool userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

                if (!userExists)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                var student = await _context.Students
                    .Include(s => s.User)
                    .Include(s => s.Lecturer)
                    .Include(s => s.Semester)
                    .Include(s => s.Major)
                    .Include(s => s.Address)
                        .ThenInclude(a => a.Ward)
                        .ThenInclude(a => a.District)
                        .ThenInclude(a => a.Province)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                return student;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Student> UpdateStudentByUserIdAsync(int? userId, User? updateUser, Student? updateInformation, Address? updateAddress)
        {
            if (userId == null || updateInformation == null)
            {
                throw new ArgumentNullException("User id or update information cannot be null.");
            }

            try
            {
                // Check if user exists
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    throw new KeyNotFoundException("Not found user.");
                }

                // Find the student by userId
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                // Update Student information
                // TBL User
                user.Image = updateUser.Image ?? user.Image;
                user.UpdatedAt = GetVietnamTime(); // Update the timestamp

                // TBL Student
                student.AlternativeEmail = updateInformation.AlternativeEmail ?? student.AlternativeEmail;
                student.Phone = updateInformation.Phone ?? student.Phone;
                student.Dob = updateInformation.Dob ?? student.Dob;
                student.Gender = updateInformation.Gender ?? student.Gender;

                // TBL Address
                if (updateAddress != null && student.AddressId.HasValue)
                {
                    var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == student.AddressId.Value);

                    if (address == null)
                    {
                        throw new KeyNotFoundException("Not found address.");
                    }

                    // Update address details
                    address.Detail = updateAddress.Detail ?? address.Detail;
                    address.WardId = updateAddress.WardId ?? address.WardId;
                    address.DistrictId = updateAddress.DistrictId ?? address.DistrictId;
                    address.ProvinceId = updateAddress.ProvinceId ?? address.ProvinceId;
                    address.UpdatedAt = GetVietnamTime(); // Update the timestamp
                }

                student.UpdatedAt = GetVietnamTime(); // Update the timestamp

                // Save all changes to the database
                await _context.SaveChangesAsync();

                return student;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //For Dean
        public async Task<User> GetDeanByUserIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Dean");
        }
        public async Task<IEnumerable<Student>> GetStudentListAsync(
        int userId,
        string role,
        string? code,
        string? studentName,
        string? lecturerName,
        string? majorName,
        string? sortBy,
        bool? isDescending)
        {
            // Khởi tạo truy vấn
            IQueryable<Student> query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .Include(s => s.Lecturer)
                .Where(s => s.User.Status != "Deleted" );

            if (role == "Dean")
            {
                // Lấy thông tin Dean và kiểm tra hợp lệ
                var dean = await _context.Users
                    .Include(u => u.Department) // Kết nối với bảng Department
                    .FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Dean");

                if (dean == null || !dean.DepartmentId.HasValue)
                {
                    throw new KeyNotFoundException("Dean not found or department not assigned.");
                }

                // Lấy danh sách MajorId thuộc Department của Dean
                var majorIdsInDepartment = await _context.Majors
                    .Where(m => m.DepartmentId  == dean.DepartmentId && m.Status == "Active")
                    .Select(m => m.MajorId)
                    .ToListAsync();

                if (!majorIdsInDepartment.Any())
                {
                    throw new KeyNotFoundException("Dean does not manage any majors.");
                }

                // Lọc danh sách sinh viên theo MajorId trong Department của Dean
                query = query.Where(s => s.MajorId.HasValue && majorIdsInDepartment.Contains(s.MajorId.Value));

            }
            else if (role == "Lecturer")
            {
                // Lọc theo LecturerId
                query = query.Where(s => s.LecturerId == userId);
            }
            else if (role == "Admin" || role == "DOET")
            {
                // Admin và DOET có quyền truy cập toàn bộ sinh viên, không cần thêm điều kiện
            }
            else
            {
                throw new UnauthorizedAccessException("Role not authorized to view student list.");
            }
            // Tìm kiếm theo Student Name
            if (!string.IsNullOrWhiteSpace(studentName))
            {
                studentName = studentName.ToLower();
                query = query.Where(s => s.User.Name.ToLower().Contains(studentName));
            }
            if (!string.IsNullOrWhiteSpace(code))
            {
                code = code.ToLower();
                query = query.Where(s => s.User.UserCode.ToLower().Contains(code));
            }

            // Tìm kiếm theo Lecturer Name (chỉ áp dụng cho Dean)
            if ((role == "Dean" || role == "Lecturer" || role == "Admin") && !string.IsNullOrWhiteSpace(lecturerName))
            {
                lecturerName = lecturerName.ToLower();
                query = query.Where(s => s.Lecturer != null && s.Lecturer.Name.ToLower().Contains(lecturerName));
            }

            // Tìm kiếm theo Major Name
            if (!string.IsNullOrWhiteSpace(majorName))
            {
                majorName = majorName.ToLower();
                query = query.Where(s => s.Major != null && s.Major.Name.ToLower().Contains(majorName));
            }

            // Sắp xếp
            switch (sortBy?.ToLower())
            {
                case "studentname":
                    query = isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(s => s.User.Name)
                        : query.OrderBy(s => s.User.Name);
                    break;

                case "lecturername":
                    query = isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(s => s.Lecturer.Name)
                        : query.OrderBy(s => s.Lecturer.Name);
                    break;

                case "majorname":
                    query = isDescending.GetValueOrDefault()
                        ? query.OrderByDescending(s => s.Major.Name)
                        : query.OrderBy(s => s.Major.Name);
                    break;

                default:
                    query = query.OrderBy(s => s.User.Name); // Mặc định sắp xếp theo Student Name tăng dần
                    break;
            }

            return await query.ToListAsync();
        }

        // KienBV - fixed 
        public async Task<IEnumerable<Student>> GetOjtStudentListAsync(int userId)
        {
            var dean = await _context.Users.Include(u => u.Role)
                .Where(u => u.Role.Name == "Dean" && u.UserId == userId)
                .FirstOrDefaultAsync();

            if (dean == null)
            {
                throw new KeyNotFoundException("Not found dean.");
            }

            var majors = await _context.Majors
                .Include(m => m.Department)
                .Where(m => m.DepartmentId == dean.DepartmentId)
                .ToListAsync();

            if (majors == null)
            {
                throw new KeyNotFoundException("Not majors list of department.");
            }

            var majorIds = majors.Select(m => m.MajorId).ToList();

            var students = await _context.Students
                .Include(s => s.User)
                .ThenInclude(u => u.Role)
                .Where(s => majorIds.Contains((int)s.MajorId))
                .ToListAsync();

            if (students == null)
            {
                throw new KeyNotFoundException("Not students have major of department.");
            }

            return students;
        }

        // Get students by IDs
        public async Task<List<Student>> GetStudentsByIdsAsync(List<int> studentIds)
        {
            return await _context.Students
                .Where(s => studentIds.Contains(s.StudentId))
                .ToListAsync();
        }

        // Update students
        public async Task UpdateStudentsAsync(List<Student> students)
        {
            _context.Students.UpdateRange(students);
            await _context.SaveChangesAsync();
        }

        // Get student details for Dean or Lecturer
        public async Task<Student> GetStudentDetailsByIdAsync(int studentId, int userId, string role)
        {
            // Khởi tạo query lấy thông tin sinh viên
            var query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .Include(s => s.Lecturer)
                .Include(s => s.Address)
                    .ThenInclude(a => a.Ward)
                .Include(s => s.Address.District)
                .Include(s => s.Address.Province)
                .Where(s => s.StudentId == studentId && s.User.Status != "Deleted" );

            // Logic cho Lecturer
            if (role == "Lecturer")
            {
                query = query.Where(s => s.LecturerId == userId);
            }
            // Logic cho Dean
            else if (role == "Dean")
            {
                // Lấy thông tin Dean
                var dean = await GetDeanByUserIdAsync(userId);
                if (dean == null || !dean.DepartmentId.HasValue)
                {
                    throw new KeyNotFoundException("Dean not found or doesn't manage any department.");
                }

                // Lấy danh sách MajorId thuộc Department mà Dean quản lý
                var majorIdsInDepartment = await _context.Majors
                    .Where(m => m.DepartmentId == dean.DepartmentId)
                    .Select(m => m.MajorId)
                    .ToListAsync();

                if (!majorIdsInDepartment.Any())
                {
                    throw new KeyNotFoundException("Dean does not manage any majors.");
                }

                // Kiểm tra MajorId của sinh viên có thuộc MajorId trong Department không
                query = query.Where(s => s.MajorId.HasValue && majorIdsInDepartment.Contains(s.MajorId.Value));
            }
            else if (role == "Admin"|| role == "DOET") {

            }
            // Lấy sinh viên đầu tiên phù hợp
            var student = await query.FirstOrDefaultAsync();

            // Nếu không tìm thấy sinh viên
            if (student == null)
            {
                throw new KeyNotFoundException("Student not found or access denied.");
            }

            return student;
        }

        public async Task<Major> GetMajorByIdAsync(int majorId)
        {
            return await _context.Majors.FirstOrDefaultAsync(m => m.MajorId == majorId);
        }

        public async Task<Semester> GetSemesterByIdAsync(int semesterId)
        {
            return await _context.Semesters.FirstOrDefaultAsync(s => s.SemesterId == semesterId);
        }

        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
        //End
    }
}
