using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IUserRepository
    {
        // Common - Authentication
        Task<User> GetUserByEmailAsync(string email);

        // CRUD user operations for admin
        Task<IEnumerable<User>> GetAllUsersForAdminAsync(string? name, int? roleId, string? status);
        Task<User> GetUserByIdForAdminAsync(int userId);
        Task<User> AddUserForAdminAsync(User user);
        Task AddUsersForAdminAsync(IEnumerable<User> users);
        Task AddStudentsAndCompaniesForAdminAsync(IEnumerable<Student> studentUsers, IEnumerable<Company> companyUsers);
        Task<User> UpdateUserForAdminAsync(User user);
        Task<User> SoftDeleteUserForAdminAsync(int userId);

        Task<bool> IsUserCodeExistsAsync(string userCode);
        Task<bool> IsEmailExistsAsync(string email);

        // CRUD user stored operations for admin
        Task<IEnumerable<User>> GetAllUsersStoredAsync(string? name, int? roleId);
        Task<User> GetUserStoredByIdForAdminAsync(int userId);
        Task<User> HardDeleteUserStoredAsync(int userId);
        Task<User> RestoreUserStoredAsync(int userId);

        // CRUD user operations for doet
        Task<IEnumerable<User>> GetAllUsersForDoetAsync(string? name, int? roleId, string? status);
        Task<User> GetUserByIdForDoetAsync(int userId);
        Task<User> AddUserForDoetAsync(User user);
        Task AddUsersForDoetAsync(IEnumerable<User> users);
        Task AddStudentsAndCompaniesForDoetAsync(IEnumerable<Student> studentUsers, IEnumerable<Company> companyUsers);
        Task<User> UpdateUserForDoetAsync(User user);
        Task<User> SoftDeleteUserForDoetAsync(int userId);

        // CRUD user operations for company
        Task<IEnumerable<User>> GetAllUsersForCompanyAsync(int companyId, string? name, int? roleId, string? status);
        Task<User> GetUserByIdForCompanyAsync(int companyId, int userId);
        Task<User> AddUserForCompanyAsync(int companyId, User user);
        Task<User> UpdateUserForCompanyAsync(int companyId, User user);
        Task<User> SoftDeleteUserForCompanyAsync(int companyId, int userId);
        //For Dean 
        Task<(User Lecturer, User Dean, List<Student> Students)> GetLecturerDetailsWithDeanAndStudentsAsync(int lecturerId);
        Task<User> GetDeanByUserIdAsync(int userId);
        Task<User> GetLecturerByUserIdAsync(int userId);
        Task<User> GetUserByIdAsync(int userId);
        Task<int> GetRoleIdByNameAsync(string roleName);
        Task<User> GetLecturerByEmailForDeanAsync(string email);
        Task CreateLecturerForDeanAsync(User user);
        Task UpdateDeanAsync(User dean);
        Task UpdateLecturerAsync(User lecturer);
        Task<IEnumerable<User>> GetLecturerListForDeanAsync(
        int assignForId,
        string? name,
        string? userCode,
        string? majorName,
        string? sortBy,
        bool isDescending);

        //End 

        //Admin, DOET
        Task<List<User>> GetAllDeansAsync(string? userCode, string? name, string? departmentName, string? sortBy, bool? isDescending);
        Task<(User Dean, List<User> Lecturers, List<Student> Students)> GetDeanDetailsWithLecturersAndStudentsAsync(int deanId);
        Task UpdateLecturersAsync(List<User> lecturers);
        Task<List<User>> GetLecturersByIdsAsync(List<int> lecturerIds);
        Task<List<User>> GetAllLecturerAsync(string? userCode, string? name, string? majorName, string? sortBy, bool? isDescending);
    }
}
