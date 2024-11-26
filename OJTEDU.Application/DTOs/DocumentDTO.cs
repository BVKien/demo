using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.RoleDTO;

namespace OJTEDU.Application.DTOs
{
    public class DocumentDTO
    {
        // Admin
        public class DocumentListForAdminDTO
        {
            public int DocumentId { get; set; }
            public string? University { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class DocumentDetailForAdminDTO
        {
            public int DocumentId { get; set; }
            public string? University { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public List<RoleListDTO>? Roles { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddDocumentForAdminDTO
        {
            public int UniversityId { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateDocumentForAdminDTO
        {
            public int DocumentId { get; set; }
            public int UniversityId { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateDocumentStatusForAdminDTO
        {
            public int DocumentId { get; set; }
            public int UniversityId { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteDocumentForAdminDTO
        {
            public int DocumentId { get; set; }
            public int UniversityId { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class StatusDocumentListForAdminDTO
        {
            public string? Status { get; set; }
        }

        // Doet
        public class DocumentListForDoetDTO
        {
            public int DocumentId { get; set; }
            public string? University { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public string? ForRole { get; set; }
            public string? Status { get; set; }
        }

        public class DocumentDetailForDoetDTO
        {
            public int DocumentId { get; set; }
            public string? University { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public List<RoleListDTO>? Roles { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class AddDocumentForDoetDTO
        {
            public int UniversityId { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateDocumentForDoetDTO
        {
            public int DocumentId { get; set; }
            public int UniversityId { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public List<int?>? ForRoleIds { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateDocumentStatusForDoetDTO
        {
            public int DocumentId { get; set; }
            public int UniversityId { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteDocumentForDoetDTO
        {
            public int DocumentId { get; set; }
            public int UniversityId { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        public class StatusDocumentListForDoetDTO
        {
            public string? Status { get; set; }
        }

        // Common
        public class DocumentListForCommonDTO
        {
            public int DocumentId { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DocumentDetailForCommonDTO
        {
            public int DocumentId { get; set; }
            public string? University { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        // Guest 
        public class DocumentInternshipProcessForGuestDTO
        {
            public int DocumentId { get; set; }
            public string? University { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
        }

        // Company 
        public class DocumentTestFilesListForCompanyDTO
        {
            public int DocumentId { get; set; }
            public int? UniversityId { get; set; }
            public int? UserId { get; set; }
            public string? CompanyName { get; set; }
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class CreateDocumentTestFilesForCompanyDTO
        {
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateDocumentTestFilesForCompanyDTO
        {
            public string? Title { get; set; }
            public string? DocumentFile { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
