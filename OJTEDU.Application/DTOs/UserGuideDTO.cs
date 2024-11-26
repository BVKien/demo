using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class UserGuideDTO
    {
        // Admin
        public class UserGuideListForAdminDTO
        {
            public int UserGuideId { get; set; }
            public string? Title { get; set; }
            public string? UserGuideFile { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class UserGuideDetailForAdminDTO
        {
            public int UserGuideId { get; set; }
            public string? Title { get; set; }
            public string? UserGuideFile { get; set; }
            public int? ForRoleId { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddUserGuideForAdminDTO
        {
            public string? Title { get; set; }
            public string? UserGuideFile { get; set; }
            public int? ForRoleId { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateUserGuideForAdminDTO
        {
            public int UserGuideId { get; set; }
            public string? Title { get; set; }
            public string? UserGuideFile { get; set; }
            public int? ForRoleId { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateUserGuideStatusForAdminDTO
        {
            public int UserGuideId { get; set; }
            public string? Title { get; set; }
            public string? UserGuideFile { get; set; }
            public int? ForRoleId { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteUserGuideForAdminDTO
        {
            public int UserGuideId { get; set; }
            public string? Title { get; set; }
            public string? UserGuideFile { get; set; }
            public int? ForRoleId { get; set; }
        }

        public class StatusUserGuideListForAdminDTO
        {
            public string? Status { get; set; }
        }
    }
}
