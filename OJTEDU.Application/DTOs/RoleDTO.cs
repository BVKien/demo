using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class RoleDTO
    {
        public class RoleListDTO
        {
            public int RoleId { get; set; }
            public string? Name { get; set; }
        }

        public class RoleListForAdminDTO
        {
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
        }

        public class RoleDetailForAdminDTO
        {
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddRoleForAdminDTO
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateRoleForAdminDTO
        {
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteRoleForAdminDTO
        {
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
        }

        public class RoleListForDoetDTO
        {
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
        }

        public class RoleListForCompanyDTO
        {
            public int RoleId { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
        }
    }
}
