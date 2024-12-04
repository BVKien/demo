using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Document = OJTEDU.Domain.Entities.Document;

namespace OJTEDU.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _documentDirectory = "wwwroot/uploads/documents/testfiles/";
        public DocumentRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;

            if (!Directory.Exists(_documentDirectory))
            {
                Directory.CreateDirectory(_documentDirectory);
            }
        }

        // Admin
        public async Task<IEnumerable<Document>> GetAllDocumentsForAdminAsync(string? title, int? roleId, string? status)
        {
            IQueryable<Document> query = _context.Documents.Include(d => d.DocumentRoles).ThenInclude(dr => dr.Role).Include(u => u.University)
                                                       .Where(u => u.University.Role.Name.Equals("Admin"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var documents = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (documents == null)
            {
                throw new KeyNotFoundException("Documents not found.");
            }

            var sortedDocuments = documents.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.DocumentId)
                                           .ToList();

            return sortedDocuments;
        }

        public async Task<Document> GetDocumentByIdForAdminAsync(int documentId)
        {
            var document = await _context.Documents.Include(u => u.DocumentRoles).ThenInclude(dr => dr.Role).Include(u => u.University)
                                                   .FirstOrDefaultAsync(u => u.University.Role.Name.Equals("Admin") && u.DocumentId == documentId);
            if (document == null)
            {
                throw new KeyNotFoundException("Document not found");
            }
            return document;
        }

        public async Task<Document> AddDocumentForAdminAsync(Document document, List<int?> roleIds)
        {
            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Thêm tài liệu vào bảng Document
                document.CreatedAt = GetVietnamTime();
                document.UpdatedAt = GetVietnamTime();
                document.Status = "Active"; // Mặc định trạng thái là Active
                await _context.Documents.AddAsync(document);
                await _context.SaveChangesAsync();

                // Thêm các bản ghi vào bảng DocumentRoles
                foreach (var roleId in roleIds)
                {
                    var documentRole = new DocumentRole
                    {
                        DocumentId = document.DocumentId,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.DocumentRoles.AddAsync(documentRole);
                }

                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return document;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding document with roles: {ex.Message}");
            }
        }

        public async Task UpdateDocumentRolesAsync(int documentId, List<int?> newRoleIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Xóa các DocumentRoles hiện tại
                var existingRoles = _context.DocumentRoles.Where(dr => dr.DocumentId == documentId).ToList();
                _context.DocumentRoles.RemoveRange(existingRoles);
                await _context.SaveChangesAsync();

                // Thêm mới DocumentRoles
                foreach (var roleId in newRoleIds)
                {
                    var documentRole = new DocumentRole
                    {
                        DocumentId = documentId,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.DocumentRoles.AddAsync(documentRole);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error updating roles for document: {ex.Message}");
            }
        }

        public async Task<Document> UpdateDocumentForAdminAsync(Document document)
        {
            var existingDocument = await GetDocumentByIdForAdminAsync(document.DocumentId);
            if (existingDocument == null)
            {
                throw new KeyNotFoundException("Document not found");
            }

            existingDocument.Title = document.Title ?? existingDocument.Title;
            existingDocument.DocumentFile = document.DocumentFile ?? existingDocument.DocumentFile;
            existingDocument.Description = document.Description ?? existingDocument.Description;
            existingDocument.UniversityId = document.UniversityId ?? existingDocument.UniversityId;
            existingDocument.Status = document.Status ?? existingDocument.Status;
            existingDocument.UpdatedAt = GetVietnamTime();

            _context.Documents.Update(existingDocument);
            await _context.SaveChangesAsync();
            return existingDocument;
        }

        public async Task<Document> DeleteDocumentForAdminAsync(int documentId)
        {
            var document = await GetDocumentByIdForAdminAsync(documentId);
            if (document == null)
            {
                throw new KeyNotFoundException("Document not found in the list.");
            }

            var documentRoles = _context.DocumentRoles.Where(dr => dr.DocumentId == documentId).ToList();
            if (documentRoles.Any())
            {
                _context.DocumentRoles.RemoveRange(documentRoles);
            }

            document.DeletedAt = GetVietnamTime(); // Cập nhật thời gian xóa

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return document;
        }

        // Doet

        public async Task<IEnumerable<Document>> GetAllDocumentsForDoetAsync(string? title, int? roleId, string? status)
        {
            IQueryable<Document> query = _context.Documents.Include(d => d.DocumentRoles).ThenInclude(dr => dr.Role).Include(u => u.University)
                                                       .Where(u => u.University.Role.Name.Equals("DOET"));

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (roleId.HasValue)
            {
                if (roleId.Value == 0)
                {
                    // Lọc các Document có RoleId là null
                    query = query.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == null));
                }
                else
                {
                    // Lọc các Document có RoleId bằng giá trị roleId
                    query = query.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == roleId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToLower();
                query = query.Where(u => u.Status.ToLower().Equals(status));
            }

            // Fetch the filtered result from the database
            var documents = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (documents == null)
            {
                throw new KeyNotFoundException("Documents not found.");
            }

            var sortedDocuments = documents.OrderByDescending(u => u.Status == "Active")
                                           .ThenByDescending(u => u.Status == "Unactive")
                                           .ThenByDescending(u => u.DocumentId)
                                           .ToList();

            return sortedDocuments;
        }

        public async Task<Document> GetDocumentByIdForDoetAsync(int documentId)
        {
            var document = await _context.Documents.Include(u => u.DocumentRoles).ThenInclude(dr => dr.Role).Include(u => u.University)
                                                   .FirstOrDefaultAsync(u => u.University.Role.Name.Equals("DOET") && u.DocumentId == documentId);
            if (document == null)
            {
                throw new KeyNotFoundException("Document not found");
            }
            return document;
        }

        public async Task<Document> AddDocumentForDoetAsync(Document document, List<int?> roleIds)
        {
            // Bắt đầu transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Thêm tài liệu vào bảng Document
                document.CreatedAt = GetVietnamTime();
                document.UpdatedAt = GetVietnamTime();
                document.Status = "Active"; // Mặc định trạng thái là Active
                await _context.Documents.AddAsync(document);
                await _context.SaveChangesAsync();

                // Thêm các bản ghi vào bảng DocumentRoles
                foreach (var roleId in roleIds)
                {
                    var documentRole = new DocumentRole
                    {
                        DocumentId = document.DocumentId,
                        RoleId = roleId == 0 ? null : roleId
                    };
                    await _context.DocumentRoles.AddAsync(documentRole);
                }

                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return document;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw new Exception($"Error adding document with roles: {ex.Message}");
            }
        }

        public async Task<Document> UpdateDocumentForDoetAsync(Document document)
        {
            var existingDocument = await GetDocumentByIdForDoetAsync(document.DocumentId);
            if (existingDocument == null)
            {
                throw new KeyNotFoundException("Document not found");
            }

            existingDocument.Title = document.Title ?? existingDocument.Title;
            existingDocument.DocumentFile = document.DocumentFile ?? existingDocument.DocumentFile;
            existingDocument.Description = document.Description ?? existingDocument.Description;
            existingDocument.UniversityId = document.UniversityId ?? existingDocument.UniversityId;
            existingDocument.Status = document.Status ?? existingDocument.Status;
            existingDocument.UpdatedAt = GetVietnamTime();

            _context.Documents.Update(existingDocument);
            await _context.SaveChangesAsync();
            return existingDocument;

        }

        public async Task<Document> DeleteDocumentForDoetAsync(int documentId)
        {
            var document = await GetDocumentByIdForDoetAsync(documentId);
            if (document == null)
            {
                throw new KeyNotFoundException("Document not found in the list.");
            }

            var documentRoles = _context.DocumentRoles.Where(dr => dr.DocumentId == documentId).ToList();
            if (documentRoles.Any())
            {
                _context.DocumentRoles.RemoveRange(documentRoles);
            }

            document.DeletedAt = GetVietnamTime(); // Cập nhật thời gian xóa

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return document;
        }

        //// Common 
        public async Task<IEnumerable<Document>> GetAllDocumentsAsync(string role, string? title)
        {
            var documentsQuery = _context.Documents
                                 .Include(d => d.DocumentRoles).ThenInclude(dr => dr.Role)
                                 .Include(d => d.University)
                                 .Where(d => d.Status == "Active");

            if (role == "guest")
            {
                // Nếu role là "guest", chỉ lấy tài liệu có RoleId là null và sắp xếp theo DocumentId giảm dần
                documentsQuery = documentsQuery.Where(d => d.DocumentRoles.All(dr => dr.RoleId == null));
            }
            else
            {
                // Nếu role không phải là "guest", lấy cả tài liệu dành cho vai trò của người dùng và tài liệu dành cho guest
                documentsQuery = documentsQuery.Where(d => d.DocumentRoles.Any(dr => dr.RoleId == null || dr.Role.Name.Equals(role)));
            }

            if (!string.IsNullOrEmpty(title))
            {
                title = title.ToLower();
                documentsQuery = documentsQuery.Where(n => n.Title.ToLower().Contains(title));
            }

            // Sắp xếp: Tài liệu của vai trò đăng nhập lên đầu, sau đó mới đến tài liệu dành cho guest
            documentsQuery = documentsQuery
                .OrderByDescending(d => d.DocumentRoles.Any(dr => dr.Role != null && dr.Role.Name.Equals(role))) // Vai trò đăng nhập lên đầu
                .ThenBy(d => d.DocumentRoles.Any(dr => dr.RoleId == null)) // Sau đó là guest
                .ThenByDescending(d => d.DocumentId); // Sắp xếp theo DocumentId giảm dần

            var documentsList = await documentsQuery.ToListAsync();

            if (documentsList == null)
            {
                throw new KeyNotFoundException("No documents found for the specified role.");
            }

            return documentsList;
        }

        public async Task<Document> GetDocumentDetailAsync(int? documentId, string role)
        {
            var allDocuments = await GetAllDocumentsAsync(role, null);

            var documentDetail = allDocuments.FirstOrDefault(n => n.DocumentId == documentId);

            if (documentDetail == null)
            {
                throw new KeyNotFoundException("Document detail not found.");
            }

            return documentDetail;
        }

        //// Guest
        //public async Task<Document> GetInternshipProcessDocumentAsync()
        //{
        //    try
        //    {
        //        var document = await _context.Documents
        //            .Include(d => d.University)
        //            .Where(d => d.RoleId == null)
        //            .FirstOrDefaultAsync();

        //        return document;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"An error occurred while get internship process document. " + ex.Message);
        //    }
        //}

        // Company
        public async Task<Document> CreateDocumentsByUserIdAsync(int? userId, string? fileName, byte[] fileData, Document? info)
        {
            try
            {
                var company = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Company");
                if (company == null)
                {
                    throw new KeyNotFoundException("Not found company.");
                }

                // Create file name format userId_timestamp_filename
                var timestamp = GetVietnamTime().ToString("yyyyMMddHHmmssfff");
                var newFileName = fileName != null ? $"{userId}_{timestamp}_{fileName}" : null;

                var filePath = newFileName != null ? Path.Combine(_documentDirectory, newFileName) : null;

                // Save files to folders
                if (fileData != null && filePath != null)
                {
                    await File.WriteAllBytesAsync(filePath, fileData);
                }

                // If null 
                if (fileName == null || fileData == null)
                {
                    filePath = null;
                }

                var document = new Document
                {
                    UserId = userId,
                    Title = info?.Title,
                    DocumentFile = filePath?.Replace("wwwroot", ""),
                    Description = info?.Description,
                    Status = "Active",
                    CreatedAt = GetVietnamTime(),
                    UpdatedAt = GetVietnamTime()
                };

                await _context.Documents.AddAsync(document);
                await _context.SaveChangesAsync();

                return document;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> StoredDocumentsByUserIdAsync(int? documentId)
        {
            try
            {
                var document = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId);
                if (document == null)
                {
                    throw new KeyNotFoundException("Not found document.");
                }

                document.Status = "Deleted";
                document.UpdatedAt = GetVietnamTime();
                document.DeletedAt = GetVietnamTime();
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsByUserIdAsync(int? userId)
        {
            try
            {
                var documents = await _context.Documents
                    .Where(d => d.UserId == userId && d.Status == "Active")
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync();
                if (documents == null)
                {
                    throw new KeyNotFoundException("Not found test files list.");
                }

                return documents;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Document> UpdateDocumentAsync(int? documentId, string? fileName, byte[] fileData, Document? info)
        {
            try
            {
                var document = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId);
                if (document == null)
                {
                    throw new KeyNotFoundException("Not found document.");
                }

                // Create file name format documentId_timestamp_filename
                var timestampValid = GetVietnamTime().ToString("yyyyMMddHHmmssfff");
                var newFileNameValid = fileName != null ? $"{document.DocumentId}_{timestampValid}_{fileName}" : null;

                var filePathValid = newFileNameValid != null ? Path.Combine(_documentDirectory, newFileNameValid) : null;

                // Save files to folders
                if (fileData != null && filePathValid != null)
                {
                    await File.WriteAllBytesAsync(filePathValid, fileData);
                }

                // If null 
                if (fileName == null || fileData == null)
                {
                    filePathValid = null;
                }

                var filePath = filePathValid?.Replace("wwwroot", "");

                if (filePath == null)
                {
                    document.Title = info?.Title;
                    document.Description = info?.Description;
                    document.UpdatedAt = GetVietnamTime();

                    await _context.SaveChangesAsync();

                    return document;
                }

                document.Title = info?.Title;
                document.DocumentFile = filePath;
                document.DocumentFile = info?.DocumentFile;
                document.Description = info?.Description;
                document.UpdatedAt = GetVietnamTime();

                await _context.SaveChangesAsync();

                return document;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
