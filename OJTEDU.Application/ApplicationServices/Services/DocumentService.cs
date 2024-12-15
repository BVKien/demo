using AutoMapper;
using Microsoft.Extensions.Logging;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.DocumentDTO;
using static OJTEDU.Application.DTOs.NewsFaqDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class DocumentService : IDocumentService
    {
        //private readonly IDocumentRepository _documentRepository;
        //private readonly IMapper _mapper;
        //public DocumentService(IDocumentRepository documentRepository, IMapper mapper)
        //{
        //    _documentRepository = documentRepository;
        //    _mapper = mapper;
        //}

        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentService> _logger; // Add logger

        public DocumentService(IDocumentRepository documentRepository, IMapper mapper, ILogger<DocumentService> logger)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        //// Admin - DocumentManagement
        //public async Task<DataResponse<PagedResponse<List<DocumentListForAdminDTO>>>> GetAllDocumentsForAdminAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        var documents = await _documentRepository.GetAllDocumentsForAdminAsync(title, roleId, status);

        //        var totalDocuments = documents.Count();
        //        var totalPages = totalDocuments == 0 ? 1 : (int)Math.Ceiling((double)totalDocuments / pageSize);

        //        // Map thủ công từ Document sang DocumentListForAdminDTO
        //        var documentDtos = documents
        //            .Skip((pageNumber - 1) * pageSize)
        //            .Take(pageSize)
        //            .Select(doc => new DocumentListForAdminDTO
        //            {
        //                DocumentId = doc.DocumentId,
        //                University = doc.University?.Name,
        //                Title = doc.Title,
        //                DocumentFile = doc.DocumentFile,
        //                Description = doc.Description,
        //                Status = doc.Status,
        //                ForRole = doc.DocumentRoles != null && doc.DocumentRoles.Any()
        //                    ? string.Join(", ", doc.DocumentRoles.Select(dr => dr.Role?.Name ?? "Guest"))
        //                    : "Guest" // Nếu không có role, mặc định là Guest
        //            })
        //            .ToList();

        //        var pagedResponse = new PagedResponse<List<DocumentListForAdminDTO>>
        //        {
        //            Items = documentDtos,
        //            TotalCount = totalDocuments,
        //            PageSize = pageSize,
        //            CurrentPage = pageNumber,
        //            TotalPages = totalPages
        //        };

        //        return new DataResponse<PagedResponse<List<DocumentListForAdminDTO>>>
        //        {
        //            Data = pagedResponse,
        //            Message = "Document list retrieved successfully!",
        //            StatusCode = 200
        //        };
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return new DataResponse<PagedResponse<List<DocumentListForAdminDTO>>>
        //        {
        //            Data = null,
        //            Message = ex.Message,
        //            StatusCode = 404
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<PagedResponse<List<DocumentListForAdminDTO>>>
        //        {
        //            Data = null,
        //            Message = $"Error retrieving document list: {ex.Message}",
        //            StatusCode = 500
        //        };
        //    }
        //}

        // Admin - DocumentManagement
        public async Task<DataResponse<PagedResponse<List<DocumentListForAdminDTO>>>> GetAllDocumentsForAdminAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            _logger.LogInformation("GetAllDocumentsForAdminAsync called with parameters: Title={Title}, RoleId={RoleId}, Status={Status}, PageNumber={PageNumber}, PageSize={PageSize}", title, roleId, status, pageNumber, pageSize);

            try
            {
                var documents = await _documentRepository.GetAllDocumentsForAdminAsync(title, roleId, status);
                _logger.LogInformation("Fetched {DocumentCount} documents from the repository", documents.Count());

                var totalDocuments = documents.Count();
                var totalPages = totalDocuments == 0 ? 1 : (int)Math.Ceiling((double)totalDocuments / pageSize);

                _logger.LogDebug("TotalDocuments={TotalDocuments}, TotalPages={TotalPages}, PageNumber={PageNumber}, PageSize={PageSize}", totalDocuments, totalPages, pageNumber, pageSize);

                // Map manually from Document to DocumentListForAdminDTO
                var documentDtos = documents
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new DocumentListForAdminDTO
                    {
                        DocumentId = doc.DocumentId,
                        University = doc.University?.Name,
                        Title = doc.Title,
                        DocumentFile = doc.DocumentFile,
                        Description = doc.Description,
                        Status = doc.Status,
                        ForRole = doc.DocumentRoles != null && doc.DocumentRoles.Any()
                            ? string.Join(", ", doc.DocumentRoles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Default to Guest if no role is found
                    })
                    .ToList();

                _logger.LogInformation("Mapped {DocumentCount} documents to DTOs", documentDtos.Count);

                var pagedResponse = new PagedResponse<List<DocumentListForAdminDTO>>
                {
                    Items = documentDtos,
                    TotalCount = totalDocuments,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                _logger.LogInformation("Successfully created a paged response with TotalPages={TotalPages} and CurrentPage={CurrentPage}", totalPages, pageNumber);

                return new DataResponse<PagedResponse<List<DocumentListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Document list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "KeyNotFoundException: {Message}", ex.Message);
                return new DataResponse<PagedResponse<List<DocumentListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document list");
                return new DataResponse<PagedResponse<List<DocumentListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving document list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DocumentDetailForAdminDTO>> GetDocumentDetailByIdForAdminAsync(int documentId)
        {
            try
            {
                var document = await _documentRepository.GetDocumentByIdForAdminAsync(documentId);

                var documentDto = _mapper.Map<DocumentDetailForAdminDTO>(document);

                return new DataResponse<DocumentDetailForAdminDTO>
                {
                    Data = documentDto,
                    Message = "Document details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DocumentDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DocumentDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving document details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddDocumentForAdminDTO>> AddDocumentForAdminAsync(AddDocumentForAdminDTO addDocumentForAdminDTO)
        {
            try
            {
                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (addDocumentForAdminDTO.ForRoleIds.Contains(null) || addDocumentForAdminDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    addDocumentForAdminDTO.ForRoleIds = new List<int?> { null };
                }

                // Tạo tài liệu mới
                var document = new Document
                {
                    UniversityId = addDocumentForAdminDTO.UniversityId,
                    Title = addDocumentForAdminDTO.Title,
                    Description = addDocumentForAdminDTO.Description,
                    DocumentFile = addDocumentForAdminDTO.DocumentFile
                };

                // Gọi repository để thêm document và các RoleIds
                var addedDocument = await _documentRepository.AddDocumentForAdminAsync(document, addDocumentForAdminDTO.ForRoleIds);

                var resultDto = _mapper.Map<AddDocumentForAdminDTO>(addedDocument);

                return new DataResponse<AddDocumentForAdminDTO>
                {
                    Data = resultDto,
                    Message = "Document added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddDocumentForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding document: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteDocumentForAdminDTO>> DeleteDocumentForAdminAsync(DeleteDocumentForAdminDTO deleteDocumentForAdminDTO)
        {
            try
            {
                var deletedDocumentResult = await _documentRepository.DeleteDocumentForAdminAsync(deleteDocumentForAdminDTO.DocumentId);

                var documentDto = _mapper.Map<DeleteDocumentForAdminDTO>(deletedDocumentResult);

                return new DataResponse<DeleteDocumentForAdminDTO>
                {
                    Data = documentDto,
                    Message = "Document has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteDocumentForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteDocumentForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting document: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateDocumentForAdminDTO>> UpdateDocumentForAdminAsync(UpdateDocumentForAdminDTO updateDocumentForAdminDTO)
        {
            try
            {
                var existingDocument = await _documentRepository.GetDocumentByIdForAdminAsync(updateDocumentForAdminDTO.DocumentId);
                if (existingDocument == null)
                {
                    throw new KeyNotFoundException("Document not found");
                }

                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (updateDocumentForAdminDTO.ForRoleIds.Contains(null) || updateDocumentForAdminDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    updateDocumentForAdminDTO.ForRoleIds = new List<int?> { null };
                }

                // Cập nhật thông tin
                existingDocument.Title = updateDocumentForAdminDTO.Title ?? existingDocument.Title;
                existingDocument.Description = updateDocumentForAdminDTO.Description ?? existingDocument.Description;
                existingDocument.DocumentFile = updateDocumentForAdminDTO.DocumentFile ?? existingDocument.DocumentFile;
                existingDocument.UpdatedAt = DateTime.Now;

                // Xử lý DocumentRoles
                if (updateDocumentForAdminDTO.ForRoleIds != null)
                {
                    await _documentRepository.UpdateDocumentRolesAsync(existingDocument.DocumentId, updateDocumentForAdminDTO.ForRoleIds);
                }

                var updatedDocumentResult = await _documentRepository.UpdateDocumentForAdminAsync(existingDocument);

                var documentDto = _mapper.Map<UpdateDocumentForAdminDTO>(updatedDocumentResult);

                return new DataResponse<UpdateDocumentForAdminDTO>
                {
                    Data = documentDto,
                    Message = "Document updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateDocumentForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDocumentForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating document: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateDocumentStatusForAdminDTO>> UpdateDocumentStatusForAdminAsync(UpdateDocumentStatusForAdminDTO updateDocumentStatusForAdminDTO)
        {
            try
            {
                var document = new Document
                {
                    DocumentId = updateDocumentStatusForAdminDTO.DocumentId,
                    Status = updateDocumentStatusForAdminDTO.Status
                };

                var updatedDocumentStatusResult = await _documentRepository.UpdateDocumentForAdminAsync(document);

                var documentDto = _mapper.Map<UpdateDocumentStatusForAdminDTO>(updatedDocumentStatusResult);

                return new DataResponse<UpdateDocumentStatusForAdminDTO>
                {
                    Data = documentDto,
                    Message = "Document updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateDocumentStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDocumentStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating document: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<List<StatusDocumentListForAdminDTO>>> GetAllStatusesDocumentForAdminAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusDocumentListForAdminDTO>
                {
                    new StatusDocumentListForAdminDTO { Status = "Active" },
                    new StatusDocumentListForAdminDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusDocumentListForAdminDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusDocumentListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusDocumentListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Doet - DocumentManagement
        public async Task<DataResponse<PagedResponse<List<DocumentListForDoetDTO>>>> GetAllDocumentsForDoetAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var documents = await _documentRepository.GetAllDocumentsForDoetAsync(title, roleId, status);

                var totalDocuments = documents.Count();
                var totalPages = totalDocuments == 0 ? 1 : (int)Math.Ceiling((double)totalDocuments / pageSize);

                // Map thủ công từ Document sang DocumentListForDoetDTO
                var documentDtos = documents
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new DocumentListForDoetDTO
                    {
                        DocumentId = doc.DocumentId,
                        University = doc.University?.Name,
                        Title = doc.Title,
                        DocumentFile = doc.DocumentFile,
                        Description = doc.Description,
                        Status = doc.Status,
                        ForRole = doc.DocumentRoles != null && doc.DocumentRoles.Any()
                            ? string.Join(", ", doc.DocumentRoles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<DocumentListForDoetDTO>>
                {
                    Items = documentDtos,
                    TotalCount = totalDocuments,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<DocumentListForDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Document list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<DocumentListForDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<DocumentListForDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving document list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DocumentDetailForDoetDTO>> GetDocumentDetailByIdForDoetAsync(int documentId)
        {
            try
            {
                var document = await _documentRepository.GetDocumentByIdForDoetAsync(documentId);

                var documentDto = _mapper.Map<DocumentDetailForDoetDTO>(document);

                return new DataResponse<DocumentDetailForDoetDTO>
                {
                    Data = documentDto,
                    Message = "Document details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DocumentDetailForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DocumentDetailForDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving document details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddDocumentForDoetDTO>> AddDocumentForDoetAsync(AddDocumentForDoetDTO addDocumentForDoetDTO)
        {
            try
            {
                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (addDocumentForDoetDTO.ForRoleIds.Contains(null) || addDocumentForDoetDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    addDocumentForDoetDTO.ForRoleIds = new List<int?> { null };
                }

                // Tạo tài liệu mới
                var document = new Document
                {
                    UniversityId = addDocumentForDoetDTO.UniversityId,
                    Title = addDocumentForDoetDTO.Title,
                    Description = addDocumentForDoetDTO.Description,
                    DocumentFile = addDocumentForDoetDTO.DocumentFile
                };

                // Gọi repository để thêm document và các RoleIds
                var addedDocument = await _documentRepository.AddDocumentForDoetAsync(document, addDocumentForDoetDTO.ForRoleIds);

                var resultDto = _mapper.Map<AddDocumentForDoetDTO>(addedDocument);

                return new DataResponse<AddDocumentForDoetDTO>
                {
                    Data = resultDto,
                    Message = "Document added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddDocumentForDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding document: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteDocumentForDoetDTO>> DeleteDocumentForDoetAsync(DeleteDocumentForDoetDTO deleteDocumentForDoetDTO)
        {
            try
            {
                var deletedDocumentResult = await _documentRepository.DeleteDocumentForDoetAsync(deleteDocumentForDoetDTO.DocumentId);

                var documentDto = _mapper.Map<DeleteDocumentForDoetDTO>(deletedDocumentResult);

                return new DataResponse<DeleteDocumentForDoetDTO>
                {
                    Data = documentDto,
                    Message = "Document has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteDocumentForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteDocumentForDoetDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting document: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateDocumentForDoetDTO>> UpdateDocumentForDoetAsync(UpdateDocumentForDoetDTO updateDocumentForDoetDTO)
        {
            try
            {
                var existingDocument = await _documentRepository.GetDocumentByIdForDoetAsync(updateDocumentForDoetDTO.DocumentId);
                if (existingDocument == null)
                {
                    throw new KeyNotFoundException("Document not found");
                }

                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (updateDocumentForDoetDTO.ForRoleIds.Contains(null) || updateDocumentForDoetDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    updateDocumentForDoetDTO.ForRoleIds = new List<int?> { null };
                }

                // Cập nhật thông tin
                existingDocument.Title = updateDocumentForDoetDTO.Title ?? existingDocument.Title;
                existingDocument.Description = updateDocumentForDoetDTO.Description ?? existingDocument.Description;
                existingDocument.DocumentFile = updateDocumentForDoetDTO.DocumentFile ?? existingDocument.DocumentFile;
                existingDocument.UpdatedAt = DateTime.Now;

                // Xử lý DocumentRoles
                if (updateDocumentForDoetDTO.ForRoleIds != null)
                {
                    await _documentRepository.UpdateDocumentRolesAsync(existingDocument.DocumentId, updateDocumentForDoetDTO.ForRoleIds);
                }

                var updatedDocumentResult = await _documentRepository.UpdateDocumentForDoetAsync(existingDocument);

                var documentDto = _mapper.Map<UpdateDocumentForDoetDTO>(updatedDocumentResult);

                return new DataResponse<UpdateDocumentForDoetDTO>
                {
                    Data = documentDto,
                    Message = "Document updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateDocumentForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDocumentForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating document: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateDocumentStatusForDoetDTO>> UpdateDocumentStatusForDoetAsync(UpdateDocumentStatusForDoetDTO updateDocumentStatusForDoetDTO)
        {
            try
            {
                var document = new Document
                {
                    DocumentId = updateDocumentStatusForDoetDTO.DocumentId,
                    Status = updateDocumentStatusForDoetDTO.Status
                };

                var updatedDocumentStatusResult = await _documentRepository.UpdateDocumentForDoetAsync(document);

                var documentDto = _mapper.Map<UpdateDocumentStatusForDoetDTO>(updatedDocumentStatusResult);

                return new DataResponse<UpdateDocumentStatusForDoetDTO>
                {
                    Data = documentDto,
                    Message = "Document updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateDocumentStatusForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDocumentStatusForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating document: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<List<StatusDocumentListForDoetDTO>>> GetAllStatusesDocumentForDoetAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusDocumentListForDoetDTO>
                {
                    new StatusDocumentListForDoetDTO { Status = "Active" },
                    new StatusDocumentListForDoetDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusDocumentListForDoetDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusDocumentListForDoetDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusDocumentListForDoetDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Common 
        public async Task<DataResponse<PagedResponse<List<DocumentListForCommonDTO>>>> GetAllDocumentsAsync(string role, string? title, int pageNumber, int pageSize)
        {
            try
            {
                var documentsList = await _documentRepository.GetAllDocumentsAsync(role, title);

                var totalDocuments = documentsList.Count();
                var totalPages = totalDocuments == 0 ? 1 : (int)Math.Ceiling((double)totalDocuments / pageSize);

                var DocumentsDtos = totalDocuments > 0 ? _mapper.Map<List<DocumentListForCommonDTO>>(documentsList)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<DocumentListForCommonDTO>();

                var pagedResponse = new PagedResponse<List<DocumentListForCommonDTO>>
                {
                    Items = DocumentsDtos,
                    TotalCount = totalDocuments,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<DocumentListForCommonDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Documents list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<DocumentListForCommonDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<DocumentListForCommonDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving documents list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        public async Task<DataResponse<DocumentDetailForCommonDTO>> GetDocumentDetailAsync(int documentId, string role)
        {
            try
            {
                var document = await _documentRepository.GetDocumentDetailAsync(documentId, role);
                var documentDto = _mapper.Map<DocumentDetailForCommonDTO>(document);

                return new DataResponse<DocumentDetailForCommonDTO>
                {
                    StatusCode = 200,
                    Message = "Document detail retrieved successfully!",
                    Data = documentDto
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DocumentDetailForCommonDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DocumentDetailForCommonDTO>
                {
                    Data = null,
                    Message = $"Error retrieving document details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // Company
        public async Task<DataResponse<CreateDocumentTestFilesForCompanyDTO>> CreateDocumentsByUserIdAsync(int? userId, string? fileName, string? fileData, CreateDocumentTestFilesForCompanyDTO? info)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateDocumentTestFilesForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var documentInfo = new Document
                {
                    Title = info?.Title,
                    Description = info?.Description
                };

                var document = await _documentRepository.CreateDocumentsByUserIdAsync(userId, fileName, fileData, documentInfo);
                var response = _mapper.Map<CreateDocumentTestFilesForCompanyDTO>(document);

                return new DataResponse<CreateDocumentTestFilesForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Create test file successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateDocumentTestFilesForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<DocumentTestFilesListForCompanyDTO>>> GetAllDocumentsByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<DocumentTestFilesListForCompanyDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var documents = await _documentRepository.GetAllDocumentsByUserIdAsync(userId);
                var response = _mapper.Map<List<DocumentTestFilesListForCompanyDTO>>(documents);

                return new DataResponse<List<DocumentTestFilesListForCompanyDTO>>
                {
                    StatusCode = 200,
                    Message = "Test files list retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<DocumentTestFilesListForCompanyDTO>>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> StoredDocumentsByUserIdAsync(int? documentId)
        {
            try
            {
                if (documentId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found document.",
                        Data = false
                    };
                }

                var document = await _documentRepository.StoredDocumentsByUserIdAsync(documentId);

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Test files deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = false
                };
            }
        }

        public async Task<DataResponse<UpdateDocumentTestFilesForCompanyDTO>> UpdateDocumentAsync(int? documentId, string? fileName, byte[] fileData, UpdateDocumentTestFilesForCompanyDTO? info)
        {
            try
            {
                if (documentId == null)
                {
                    return new DataResponse<UpdateDocumentTestFilesForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found document.",
                        Data = null
                    };
                }

                var documentInfo = new Document
                {
                    Title = info?.Title,
                    Description = info?.Description
                };

                var document = await _documentRepository.UpdateDocumentAsync(documentId, fileName, fileData, documentInfo);
                var response = _mapper.Map<UpdateDocumentTestFilesForCompanyDTO>(document);

                return new DataResponse<UpdateDocumentTestFilesForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Update test file successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDocumentTestFilesForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        // Guest 
        //public async Task<DataResponse<DocumentInternshipProcessForGuestDTO>> GetInternshipProcessDocumentAsync()
        //{
        //    try
        //    {
        //        var document = await _documentRepository.GetInternshipProcessDocumentAsync();
        //        var response = _mapper.Map<DocumentInternshipProcessForGuestDTO>(document);

        //        return new DataResponse<DocumentInternshipProcessForGuestDTO>
        //        {
        //            StatusCode = 200,
        //            Message = "Document internship process retrieved successfully!",
        //            Data = response
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataResponse<DocumentInternshipProcessForGuestDTO>
        //        {
        //            StatusCode = 500,
        //            Message = $"Error retrieving document internship process: {ex.Message}. ",
        //            Data = null
        //        };
        //    }
        //}
    }
}
