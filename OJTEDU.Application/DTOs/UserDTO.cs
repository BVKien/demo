using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.StudentDTO;

namespace OJTEDU.Application.DTOs
{
    public class UserDTO
    {
        public class UserReadForAuthDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Role { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Image { get; set; }
        }

        public class UserListForAdminDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public string? Role { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Image { get; set; }
            public string? Information { get; set; }
        }

        public class UserDetailForAdminDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public string? Role { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Image { get; set; }
            public string? Information { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddUserForAdminDTO
        {
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateUserForAdminDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateUserStatusForAdminDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteUserForAdminDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Status { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class StatusUserListForAdminDTO
        {
            public string? Status { get; set; }
        }

        public class RestoreUserForAdminDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Status { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UserListForDoetDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public string? Role { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Image { get; set; }
            public string? Information { get; set; }
        }

        public class UserDetailForDoetDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public string? Role { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Image { get; set; }
            public string? Information { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddUserForDoetDTO
        {
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateUserForDoetDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateUserStatusForDoetDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteUserForDoetDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Status { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class StatusUserListForDoetDTO
        {
            public string? Status { get; set; }
        }

        public class UserListForCompanyDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public string? Role { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Image { get; set; }
            public string? Information { get; set; }
            public int? ForCompany { get; set; }
        }

        public class UserDetailForCompanyDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public string? Role { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Image { get; set; }
            public string? Information { get; set; }
            public int? ForCompany { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddUserForCompanyDTO
        {
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public int? ForCompany { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateUserForCompanyDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public int? ForCompany { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateUserStatusForCompanyDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public string? Status { get; set; }
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public int? ForCompany { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteUserForCompanyDTO
        {
            public int UserId { get; set; }
            public string? Email { get; set; }
            public int RoleId { get; set; }
            public string? Status { get; set; }
            public string? Name { get; set; }
            public string? UserCode { get; set; }
            public string? Information { get; set; }
            public int? ForCompany { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class StatusUserListForCompanyDTO
        {
            public string? Status { get; set; }
        }
        //For Dean
        public class UpdateProfileDto
        {
            public string Name { get; set; }
            public string Information { get; set; }
        }
        public class UserProfileDto
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Information { get; set; }
            public string Image { get; set; }
            public string DepartmentName { get; set; }
            public string RoleName { get; set; }
        }

        public class CreateLecturerDto
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }

        public class LecturerListDto
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string UserCode { get; set; }
            public string Email { get; set; }
            public string MajorName { get; set; }
        }

        public class LecturerDetailsDto
        {
            public string Name { get; set; }
            public string UserCode { get; set; }
            public string MajorName { get; set; }
            public string DeanName { get; set; }
            public PagedResponse<List<StudentListDto>> Students { get; set; }
        }

        public class DeanListForAdminDOETDto
        {
            public int UserId { get; set; }
            public string UserCode { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string DepartmentName { get; set; }
        }

        public class DeanDetailsDto
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Department { get; set; }
            public PagedResponse<List<LecturerListDto>> Lecturers { get; set; }
            public PagedResponse<List<StudentListDto>> Students { get; set; }
        }

        public class AssignLecturersToDeanDto
        {
            public int DeanId { get; set; } // ID của dean cần assign các giảng viên
            public List<int> LecturerIds { get; set; } // Danh sách các ID của lecturer cần assign
        }
    }
}
