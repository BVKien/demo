using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IStudentRepository
    {
        // Student
        Task<Student> GetStudentDetailByUserIdAsync(int? userId);
        Task<Student> UpdateStudentByUserIdAsync(int? userId, User? updateUser, Student? updateInformation, Address? updateAddress);
        //For Dean
        Task<User> GetDeanByUserIdAsync(int userId);
        Task<IEnumerable<Student>> GetStudentListAsync(
        int userId,
        string role,
        string? code,
        string? studentName,      
        string? lecturerName,
        string? majorName,
        string? sortBy,
        bool? isDescending);
        Task<List<Student>> GetStudentsByIdsAsync(List<int> studentIds);
        Task UpdateStudentsAsync(List<Student> students);
        Task<Student> GetStudentDetailsByIdAsync(int studentId, int userId, string role);
        Task<IEnumerable<Student>> GetOjtStudentListAsync(int userId);
        Task<Student> GetStudentByIdAsync(int studentId);
        Task<Semester> GetSemesterByNameAsync(string semesterName);
        Task<Major> GetMajorByNameAsync(string majorName);
        Task UpdateStudentAsync(Student student);
    }
}
