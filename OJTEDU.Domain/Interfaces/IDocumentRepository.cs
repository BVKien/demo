using OJTEDU.Domain.Entities;

namespace OJTEDU.Domain.Interfaces
{
    public interface IDocumentRepository
    {
        // CRUD document operations for admin
        Task<IEnumerable<Document>> GetAllDocumentsForAdminAsync(string? title, int? roleId, string? status);
        Task<Document> GetDocumentByIdForAdminAsync(int documentId);
        Task<Document> AddDocumentForAdminAsync(Document document, List<int?> roleIds);
        Task<Document> UpdateDocumentForAdminAsync(Document document);
        Task<Document> DeleteDocumentForAdminAsync(int documentId);
        Task UpdateDocumentRolesAsync(int documentId, List<int?> newRoleIds);

        // CRUD document operations for doet
        Task<IEnumerable<Document>> GetAllDocumentsForDoetAsync(string? title, int? roleId, string? status);
        Task<Document> GetDocumentByIdForDoetAsync(int documentId);
        Task<Document> AddDocumentForDoetAsync(Document document, List<int?> roleIds);
        Task<Document> UpdateDocumentForDoetAsync(Document document);
        Task<Document> DeleteDocumentForDoetAsync(int documentId);

        // Common
        Task<IEnumerable<Document>> GetAllDocumentsAsync(string role, string? title);
        Task<Document> GetDocumentDetailAsync(int? documentId, string role);

        // Guest 
        //Task<Document> GetInternshipProcessDocumentAsync();

        // Company 
        Task<IEnumerable<Document>> GetAllDocumentsByUserIdAsync(int? userId);
        Task<Document> CreateDocumentsByUserIdAsync(int? userId, string? fileName, string? fileData, Document? info);
        Task<Document> UpdateDocumentAsync(int? documentId, string? fileName, byte[] fileData, Document? info);
        Task<bool> StoredDocumentsByUserIdAsync(int? documentId);
    }
}
