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
                user.UpdatedAt = DateTime.Now; // Update the timestamp

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
                    address.UpdatedAt = DateTime.Now; // Update the timestamp
                }

                student.UpdatedAt = DateTime.Now; // Update the timestamp

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
        public async Task<IEnumerable<Student>> GetStudentListAsync(int userId, string role, string? studentName, string? lecturerName)
        {
            IQueryable<Student> query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .Include(s => s.Lecturer)
                .Where(s => s.User.Status != "Deleted");

            if (role == "Dean")
            {
                var dean = await GetDeanByUserIdAsync(userId);
                if (dean == null || !dean.MajorId.HasValue)
                {
                    throw new KeyNotFoundException("Don't have student invalid.");
                }
                var deanMajorId = dean.MajorId.Value;
                query = query.Where(s => s.MajorId == deanMajorId);
            }
            else if (role == "Lecturer")
            {
                query = query.Where(s => s.LecturerId == userId);
            }

            if (!string.IsNullOrWhiteSpace(studentName))
            {
                studentName = studentName.ToLower();
                query = query.Where(s => s.User.Name.ToLower().Contains(studentName));
            }

            if (role == "Dean" && !string.IsNullOrWhiteSpace(lecturerName))
            {
                lecturerName = lecturerName.ToLower();
                query = query.Where(s => s.Lecturer != null && s.Lecturer.Name.ToLower().Contains(lecturerName));
            }

            var students = await query.ToListAsync();

            if (students == null || students.Count == 0)
            {
                throw new KeyNotFoundException("Not Found Student.");
            }

            return students;
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
            var query = _context.Students
                .Include(s => s.User)
                .Include(s => s.Semester)
                .Include(s => s.Major)
                .Include(s => s.Lecturer)
                .Include(s => s.Address)
                    .ThenInclude(a => a.Ward)
                .Include(s => s.Address.District)
                .Include(s => s.Address.Province)
                .Where(s => s.StudentId == studentId && s.User.Status != "Deleted");

            if (role == "Lecturer")
            {
                query = query.Where(s => s.LecturerId == userId);
            }
            else if (role == "Dean")
            {
                var dean = await GetDeanByUserIdAsync(userId);
                if (dean == null || !dean.MajorId.HasValue)
                {
                    throw new KeyNotFoundException("Don't have student invalid.");
                }
                var deanMajorId = dean.MajorId.Value;
                query = query.Where(s => s.MajorId == deanMajorId);
            }

            var student = await query.FirstOrDefaultAsync();

            return student;
        }
        //End
    }
}
