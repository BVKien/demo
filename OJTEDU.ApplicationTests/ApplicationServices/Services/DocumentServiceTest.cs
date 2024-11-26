//using AutoMapper;
//using Moq;
//using NUnit.Framework;
//using OJTEDU.Application.ApplicationServices.Services;
//using OJTEDU.Application.DTOs;
//using OJTEDU.Domain.Entities;
//using OJTEDU.Domain.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
//using static OJTEDU.Application.DTOs.DocumentDTO;
//using System.Drawing;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class DocumentServiceTests
//    {
//        private Mock<IDocumentRepository> _documentRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private DocumentService _documentService;

//        [SetUp]
//        public void Setup()
//        {
//            _documentRepositoryMock = new Mock<IDocumentRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _documentService = new DocumentService(_documentRepositoryMock.Object, _mapperMock.Object);
//        }

//        #region admin



//        #region GetAllDocumentsForAdminAsync Tests

//        [Test]
//        public async Task GetAllDocumentsForAdminAsync_ShouldReturnPagedDocumentsList_WhenDocumentsExist()
//        {
//            // Arrange
//            var documents = new List<Document> { new Document { DocumentId = 1, Title = "Sample Document", Status = "Active" } };
//            var documentDtos = documents.Select(doc => new DocumentListForAdminDTO { DocumentId = doc.DocumentId, Title = doc.Title }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForAdminDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsForAdminAsync("Sample", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(1, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllDocumentsForAdminAsync_ShouldReturnEmptyPagedList_WhenNoMatchingDocuments()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(new List<Document>());

//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForAdminDTO>>(It.IsAny<List<Document>>())).Returns(new List<DocumentListForAdminDTO>());

//            // Act
//            var result = await _documentService.GetAllDocumentsForAdminAsync("NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count); // Replaces Assert.IsEmpty
//        }


//        [Test]
//        public async Task GetAllDocumentsForAdminAsync_ShouldReturnUnauthorized_WhenUserNotAuthorized()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _documentService.GetAllDocumentsForAdminAsync("AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get document list: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllDocumentsForAdminAsync_ShouldReturnNotFound_WhenNoDocumentsExist()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new KeyNotFoundException("Documents not found."));

//            // Act
//            var result = await _documentService.GetAllDocumentsForAdminAsync("NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Documents not found.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllDocumentsForAdminAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new Exception("Server error occurred"));

//            // Act
//            var result = await _documentService.GetAllDocumentsForAdminAsync("AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving document list: Server error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllDocumentsForAdminAsync_ShouldReturnFirstPage_WhenPageNumberExceedsTotalPages()
//        {
//            // Arrange
//            var documents = new List<Document>
//    {
//        new Document { DocumentId = 1, Title = "Document1", Status = "Active" },
//        new Document { DocumentId = 2, Title = "Document2", Status = "Inactive" }
//    };
//            var documentDtos = documents.Select(doc => new DocumentListForAdminDTO { DocumentId = doc.DocumentId, Title = doc.Title }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForAdminDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsForAdminAsync(null, null, null, 10, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllDocumentsForAdminAsync_ShouldReturnRequestedPage_WhenMultiplePagesAvailable()
//        {
//            // Arrange
//            var documents = new List<Document>
//    {
//        new Document { DocumentId = 1, Title = "Document1", Status = "Active" },
//        new Document { DocumentId = 2, Title = "Document2", Status = "Inactive" },
//        new Document { DocumentId = 3, Title = "Document3", Status = "Active" }
//    };
//            var documentDtos = documents.Select(doc => new DocumentListForAdminDTO { DocumentId = doc.DocumentId, Title = doc.Title }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForAdminDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsForAdminAsync(null, null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.AreEqual(3, result.Data.TotalCount);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual(2, result.Data.TotalPages);
//        }


//        [Test]
//        public async Task GetAllDocumentsForAdminAsync_ShouldReturnFilteredList_WhenTitleFilterIsApplied()
//        {
//            // Arrange
//            var documents = new List<Document> {
//        new Document { DocumentId = 1, Title = "Admin Guide" },
//        new Document { DocumentId = 2, Title = "Admin Manual" }
//    };

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForAdminAsync("Admin", null, null))
//                .ReturnsAsync(documents);

//            var documentDtos = documents.Select(doc => new DocumentListForAdminDTO { DocumentId = doc.DocumentId, Title = doc.Title }).ToList();
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForAdminDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsForAdminAsync("Admin", null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual("Admin Guide", result.Data.Items[0].Title);
//            Assert.AreEqual("Admin Manual", result.Data.Items[1].Title);
//        }


//        [Test]
//        public async Task GetAllDocumentsForAdminAsync_ShouldReturnPagedList_WhenLargeDataSetIsProvided()
//        {
//            // Arrange
//            var documents = new List<Document>();
//            for (int i = 1; i <= 50; i++)
//            {
//                documents.Add(new Document { DocumentId = i, Title = $"Document {i}", Status = "Active" });
//            }

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);

//            var documentDtos = documents.Select(doc => new DocumentListForAdminDTO { DocumentId = doc.DocumentId, Title = doc.Title, Status = doc.Status }).ToList();
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForAdminDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsForAdminAsync(null, null, null, 2, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.AreEqual(15, result.Data.Items.Count);
//            Assert.AreEqual(2, result.Data.CurrentPage);
//            Assert.AreEqual(4, result.Data.TotalPages); // 50 documents / 15 per page = 4 pages
//        }

//        #endregion


//        #region GetDocumentDetailByIdForAdminAsync

//        // Test for GetDocumentDetailByIdForAdminAsync
//        [Test]
//        public async Task GetDocumentDetailByIdForAdminAsync_ShouldReturnDocumentDetail_WhenDocumentExists()
//        {
//            var document = new Document { DocumentId = 1, Title = "Document 1" };
//            var documentDto = new DocumentDetailForAdminDTO { DocumentId = 1, Title = "Document 1" };

//            _documentRepositoryMock.Setup(repo => repo.GetDocumentByIdForAdminAsync(1)).ReturnsAsync(document);
//            _mapperMock.Setup(m => m.Map<DocumentDetailForAdminDTO>(document)).Returns(documentDto);

//            var result = await _documentService.GetDocumentDetailByIdForAdminAsync(1);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document details retrieved successfully!", result.Message);
//            Assert.AreEqual(documentDto, result.Data);
//        }

//        [Test]
//        public async Task GetDocumentDetailByIdForAdminAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
//        {
//            _documentRepositoryMock.Setup(repo => repo.GetDocumentByIdForAdminAsync(1))
//                .ThrowsAsync(new KeyNotFoundException("Document not found"));

//            var result = await _documentService.GetDocumentDetailByIdForAdminAsync(1);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Document not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetDocumentDetailByIdForAdminAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            _documentRepositoryMock.Setup(repo => repo.GetDocumentByIdForAdminAsync(It.IsAny<int>()))
//                .ThrowsAsync(new Exception("Unexpected database error"));

//            var result = await _documentService.GetDocumentDetailByIdForAdminAsync(1);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving document details: Unexpected database error", result.Message);
//        }

//        [Test]
//        public async Task GetDocumentDetailByIdForAdminAsync_ShouldReturnUnauthorized_WhenAccessIsDenied()
//        {
//            _documentRepositoryMock.Setup(repo => repo.GetDocumentByIdForAdminAsync(It.IsAny<int>()))
//                                   .ThrowsAsync(new UnauthorizedAccessException("Access denied"));
//            var result = await _documentService.GetDocumentDetailByIdForAdminAsync(1);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get document detail: Access denied", result.Message);
//        }

//        #endregion


//        #region AddDocumentForAdminAsync

//        // Test for AddDocumentForAdminAsync
//        [Test]
//        public async Task AddDocumentForAdminAsync_ShouldAddDocument_WhenDataIsValid()
//        {
//            var addDocumentDto = new AddDocumentForAdminDTO
//            {
//                Title = "New Document",
//                DocumentFile = "newfile.pdf"
//            };

//            var document = new Document
//            {
//                Title = addDocumentDto.Title,
//                DocumentFile = addDocumentDto.DocumentFile,
//                CreatedAt = DateTime.Now
//            };

//            _documentRepositoryMock.Setup(repo => repo.AddDocumentForAdminAsync(It.IsAny<Document>())).ReturnsAsync(document);

//            var result = await _documentService.AddDocumentForAdminAsync(addDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Document added successfully!", result.Message);
//            Assert.AreEqual(addDocumentDto.Title, result.Data.Title);
//        }

//        [Test]
//        public async Task AddDocumentForAdminAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            var addDocumentDto = new AddDocumentForAdminDTO
//            {
//                Title = "New Document",
//                DocumentFile = "newfile.pdf"
//            };

//            _documentRepositoryMock.Setup(repo => repo.AddDocumentForAdminAsync(It.IsAny<Document>()))
//                .ThrowsAsync(new Exception("Database error"));

//            var result = await _documentService.AddDocumentForAdminAsync(addDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding document: Database error", result.Message);
//        }

//        [Test]
//        public async Task AddDocumentForAdminAsync_ShouldHandleUnauthorizedAccessException()
//        {
//            var addDocumentDto = new AddDocumentForAdminDTO
//            {
//                Title = "New Document",
//                DocumentFile = "newfile.pdf"
//            };

//            _documentRepositoryMock.Setup(repo => repo.AddDocumentForAdminAsync(It.IsAny<Document>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            var result = await _documentService.AddDocumentForAdminAsync(addDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add document: Access denied", result.Message);
//        }

//        #endregion


//        #region UpdateDocumentForAdminAsync & UpdateDocumentStatusForAdminAsync

//        // Test for UpdateDocumentForAdminAsync & UpdateDocumentStatusForAdminAsync

//        [Test]
//        public async Task UpdateDocumentForAdminAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
//        {
//            var updateDocumentDto = new UpdateDocumentForAdminDTO { DocumentId = 1, Title = "Updated Document" };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>()))
//                .ThrowsAsync(new KeyNotFoundException("Document not found"));

//            var result = await _documentService.UpdateDocumentForAdminAsync(updateDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Document not found", result.Message);
//        }

//        [Test]
//        public async Task UpdateDocumentForAdminAsync_ShouldHandleUnauthorizedAccessException()
//        {
//            var updateDocumentDto = new UpdateDocumentForAdminDTO { DocumentId = 1, Title = "Updated Document" };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            var result = await _documentService.UpdateDocumentForAdminAsync(updateDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update document: Access denied", result.Message);
//        }

//        [Test]
//        public async Task UpdateDocumentStatusForAdminAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
//        {
//            var updateDocumentStatusDto = new UpdateDocumentStatusForAdminDTO { DocumentId = 999, Status = "Inactive" };
//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>()))
//                                   .ThrowsAsync(new KeyNotFoundException("Document not found"));

//            var result = await _documentService.UpdateDocumentStatusForAdminAsync(updateDocumentStatusDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Document not found", result.Message);
//        }

//        [Test]
//        public async Task UpdateDocumentStatusForAdminAsync_ShouldReturnUnauthorized_WhenAccessIsDenied()
//        {
//            var updateDocumentStatusDto = new UpdateDocumentStatusForAdminDTO { DocumentId = 1, Status = "Inactive" };
//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>()))
//                                   .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            var result = await _documentService.UpdateDocumentStatusForAdminAsync(updateDocumentStatusDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update document status: Access denied", result.Message);
//        }


//        [Test]
//        public async Task UpdateDocumentForAdminAsync_ShouldReturnSuccess_WhenDocumentIsUpdatedSuccessfully()
//        {
//            var updateDocumentDto = new UpdateDocumentForAdminDTO { DocumentId = 1, Title = "Updated Title" };
//            var document = new Document { DocumentId = 1, Title = "Updated Title" };
//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>())).ReturnsAsync(document);
//            _mapperMock.Setup(m => m.Map<UpdateDocumentForAdminDTO>(It.IsAny<Document>())).Returns(updateDocumentDto);

//            var result = await _documentService.UpdateDocumentForAdminAsync(updateDocumentDto);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document updated successfully!", result.Message);
//        }


//        [Test]
//        public async Task UpdateDocumentForAdminAsync_ShouldReturnInternalServerError_WhenUnexpectedErrorOccurs()
//        {
//            // Arrange
//            var updateDocumentDto = new UpdateDocumentForAdminDTO { DocumentId = 1, Title = "Updated Title" };
//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>()))
//                                   .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _documentService.UpdateDocumentForAdminAsync(updateDocumentDto);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating document: Database error", result.Message);
//        }

//        #endregion


//        #region DeleteDocumentForAdminAsync

//        // Test for DeleteDocumentForAdminAsync
//        [Test]
//        public async Task DeleteDocumentForAdminAsync_ShouldDeleteDocument_WhenDocumentExists()
//        {
//            var deleteDocumentDto = new DeleteDocumentForAdminDTO { DocumentId = 1 };
//            var document = new Document { DocumentId = 1, Title = "Document to Delete" };

//            _documentRepositoryMock.Setup(repo => repo.DeleteDocumentForAdminAsync(1)).ReturnsAsync(document);
//            _mapperMock.Setup(m => m.Map<DeleteDocumentForAdminDTO>(document)).Returns(deleteDocumentDto);

//            var result = await _documentService.DeleteDocumentForAdminAsync(deleteDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document has been permanently deleted successfully.", result.Message);
//        }

//        [Test]
//        public async Task DeleteDocumentForAdminAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
//        {
//            var deleteDocumentDto = new DeleteDocumentForAdminDTO { DocumentId = 1 };

//            _documentRepositoryMock.Setup(repo => repo.DeleteDocumentForAdminAsync(1))
//                .ThrowsAsync(new KeyNotFoundException("Document not found"));

//            var result = await _documentService.DeleteDocumentForAdminAsync(deleteDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Document not found", result.Message);
//        }

//        [Test]
//        public async Task DeleteDocumentForAdminAsync_ShouldHandleUnauthorizedAccessException()
//        {
//            var deleteDocumentDto = new DeleteDocumentForAdminDTO { DocumentId = 1 };

//            _documentRepositoryMock.Setup(repo => repo.DeleteDocumentForAdminAsync(It.IsAny<int>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            var result = await _documentService.DeleteDocumentForAdminAsync(deleteDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while hard delete document: Access denied", result.Message);
//        }

//        [Test]
//        public async Task DeleteDocumentForAdminAsync_ShouldReturnServerError_WhenRepositoryThrowsException()
//        {
//            var deleteDocumentDto = new DeleteDocumentForAdminDTO { DocumentId = 1 };

//            _documentRepositoryMock.Setup(repo => repo.DeleteDocumentForAdminAsync(It.IsAny<int>()))
//                .ThrowsAsync(new Exception("Database error"));

//            var result = await _documentService.DeleteDocumentForAdminAsync(deleteDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error permanently deleting document: Database error", result.Message);
//        }


//        #endregion


//        #region GetAllStatusesDocumentForAdminAsync

//        // Test for GetAllStatusesDocumentForAdminAsync

//        [Test]
//        public async Task GetAllStatusesDocumentForAdminAsync_ShouldReturnStatusList_WhenCalled()
//        {
//            // Act
//            var result = await _documentService.GetAllStatusesDocumentForAdminAsync();

//            // Assert
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Status List retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(2, result.Data.Count); // Expecting "Active" and "Inactive"
//            Assert.IsTrue(result.Data.Exists(s => s.Status == "Active"));
//            Assert.IsTrue(result.Data.Exists(s => s.Status == "Inactive"));
//        }

//        [Test]
//        public async Task GetAllStatusesDocumentForAdminAsync_ShouldReturnEmptyList_WhenNoStatusesDefined()
//        {
//            // Arrange
//            _documentService = new DocumentService(_documentRepositoryMock.Object, _mapperMock.Object);

//            // Act
//            var result = await _documentService.GetAllStatusesDocumentForAdminAsync();

//            // Assert
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Status List retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(2, result.Data.Count);
//        }

//        #endregion

//        #endregion



//        #region Doet

//        #region GetAllDocumentsForDoetAsync Tests

//        [Test]
//        public async Task GetAllDocumentsForDoetAsync_ShouldReturnPagedDocumentsList_WhenDocumentsExist()
//        {
//            // Arrange
//            var documents = new List<Document> { new Document { DocumentId = 1, Title = "Sample Document", Status = "Active" } };
//            var documentDtos = documents.Select(doc => new DocumentListForDoetDTO { DocumentId = doc.DocumentId, Title = doc.Title }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForDoetAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForDoetDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsForDoetAsync("Sample", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(1, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllDocumentsForDoetAsync_ShouldReturnEmptyPagedList_WhenNoMatchingDocuments()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForDoetAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(new List<Document>());

//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForDoetDTO>>(It.IsAny<List<Document>>())).Returns(new List<DocumentListForDoetDTO>());

//            // Act
//            var result = await _documentService.GetAllDocumentsForDoetAsync("NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count); // Verifies empty list
//        }

//        [Test]
//        public async Task GetAllDocumentsForDoetAsync_ShouldReturnUnauthorized_WhenUserNotAuthorized()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForDoetAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _documentService.GetAllDocumentsForDoetAsync("AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get document list: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllDocumentsForDoetAsync_ShouldReturnNotFound_WhenNoDocumentsExist()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForDoetAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new KeyNotFoundException("Documents not found."));

//            // Act
//            var result = await _documentService.GetAllDocumentsForDoetAsync("NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Documents not found.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllDocumentsForDoetAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForDoetAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new Exception("Server error occurred"));

//            // Act
//            var result = await _documentService.GetAllDocumentsForDoetAsync("AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving document list: Server error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllDocumentsForDoetAsync_ShouldReturnFirstPage_WhenPageNumberExceedsTotalPages()
//        {
//            // Arrange
//            var documents = new List<Document>
//    {
//        new Document { DocumentId = 1, Title = "Document1", Status = "Active" },
//        new Document { DocumentId = 2, Title = "Document2", Status = "Inactive" }
//    };
//            var documentDtos = documents.Select(doc => new DocumentListForDoetDTO { DocumentId = doc.DocumentId, Title = doc.Title }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForDoetAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForDoetDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsForDoetAsync(null, null, null, 10, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllDocumentsForDoetAsync_ShouldReturnFilteredList_WhenTitleFilterIsApplied()
//        {
//            // Arrange
//            var documents = new List<Document> {
//        new Document { DocumentId = 1, Title = "Doet Guide" },
//        new Document { DocumentId = 2, Title = "Doet Manual" }
//    };

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForDoetAsync("Doet", null, null))
//                .ReturnsAsync(documents);

//            var documentDtos = documents.Select(doc => new DocumentListForDoetDTO { DocumentId = doc.DocumentId, Title = doc.Title }).ToList();
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForDoetDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsForDoetAsync("Doet", null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual("Doet Guide", result.Data.Items[0].Title);
//            Assert.AreEqual("Doet Manual", result.Data.Items[1].Title);
//        }

//        [Test]
//        public async Task GetAllDocumentsForDoetAsync_ShouldReturnPagedList_WhenLargeDataSetIsProvided()
//        {
//            // Arrange
//            var documents = new List<Document>();
//            for (int i = 1; i <= 50; i++)
//            {
//                documents.Add(new Document { DocumentId = i, Title = $"Document {i}", Status = "Active" });
//            }

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsForDoetAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);

//            var documentDtos = documents.Select(doc => new DocumentListForDoetDTO { DocumentId = doc.DocumentId, Title = doc.Title, Status = doc.Status }).ToList();
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForDoetDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsForDoetAsync(null, null, null, 2, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document list retrieved successfully!", result.Message);
//            Assert.AreEqual(15, result.Data.Items.Count);
//            Assert.AreEqual(2, result.Data.CurrentPage);
//            Assert.AreEqual(4, result.Data.TotalPages); // 50 documents / 15 per page = 4 pages
//        }

//        #endregion


//        #region GetDocumentDetailByIdForDoetAsync

//        // Test for GetDocumentDetailByIdForDoetAsync

//        [Test]
//        public async Task GetDocumentDetailByIdForDoetAsync_ShouldReturnDocumentDetail_WhenDocumentExists()
//        {
//            var doetDocument = new Document { DocumentId = 1, Title = "Doet Document 1", Role = new Role { Name = "DOET" } };
//            var doetDocumentDto = new DocumentDetailForDoetDTO { DocumentId = 1, Title = "Doet Document 1" };

//            _documentRepositoryMock.Setup(repo => repo.GetDocumentByIdForDoetAsync(1)).ReturnsAsync(doetDocument);
//            _mapperMock.Setup(m => m.Map<DocumentDetailForDoetDTO>(doetDocument)).Returns(doetDocumentDto);

//            var result = await _documentService.GetDocumentDetailByIdForDoetAsync(1);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document details retrieved successfully!", result.Message);
//            Assert.AreEqual(doetDocumentDto, result.Data);
//        }


//        [Test]
//        public async Task GetDocumentDetailByIdForDoetAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetDocumentByIdForDoetAsync(1))
//                .ThrowsAsync(new KeyNotFoundException("Document not found"));

//            // Act
//            var result = await _documentService.GetDocumentDetailByIdForDoetAsync(1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Document not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetDocumentDetailByIdForDoetAsync_ShouldReturnUnauthorized_WhenAccessIsDenied()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetDocumentByIdForDoetAsync(It.IsAny<int>()))
//                                   .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _documentService.GetDocumentDetailByIdForDoetAsync(1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get document detail: Access denied", result.Message);
//        }

//        [Test]
//        public async Task GetDocumentDetailByIdForDoetAsync_ShouldReturnServerError_WhenUnexpectedErrorOccurs()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetDocumentByIdForDoetAsync(It.IsAny<int>()))
//                                   .ThrowsAsync(new Exception("Unexpected database error"));

//            // Act
//            var result = await _documentService.GetDocumentDetailByIdForDoetAsync(1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving document details: Unexpected database error", result.Message);
//        }

//        #endregion

//        #region AddDocumentForDoetAsync Tests

//        [Test]
//        public async Task AddDocumentForDoetAsync_ShouldReturnAddedDocument_WhenDocumentIsAddedSuccessfully()
//        {
//            // Arrange
//            var addDocumentDto = new AddDocumentForDoetDTO
//            {
//                UniversityId = 1,
//                ForRoleId = 2,
//                Title = "New Document",
//                Description = "Document description",
//                DocumentFile = "file.pdf"
//            };

//            var document = new Document
//            {
//                DocumentId = 1,
//                UniversityId = addDocumentDto.UniversityId,
//                RoleId = addDocumentDto.ForRoleId,
//                Title = addDocumentDto.Title,
//                Description = addDocumentDto.Description,
//                DocumentFile = addDocumentDto.DocumentFile,
//                CreatedAt = DateTime.Now,
//                Status = "Active"
//            };

//            _documentRepositoryMock.Setup(repo => repo.AddDocumentForDoetAsync(It.IsAny<Document>()))
//                .ReturnsAsync(document);
//            _mapperMock.Setup(mapper => mapper.Map<Document>(addDocumentDto)).Returns(document);
//            _mapperMock.Setup(mapper => mapper.Map<AddDocumentForDoetDTO>(document)).Returns(addDocumentDto);

//            // Act
//            var result = await _documentService.AddDocumentForDoetAsync(addDocumentDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Document added successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(addDocumentDto.Title, result.Data.Title);
//            Assert.AreEqual(addDocumentDto.UniversityId, result.Data.UniversityId);
//            Assert.AreEqual(addDocumentDto.DocumentFile, result.Data.DocumentFile);
//            Assert.IsNotNull(result.Data.CreatedAt);
//        }

//        [Test]
//        public async Task AddDocumentForDoetAsync_ShouldReturnUnauthorized_WhenUserNotAuthorized()
//        {
//            // Arrange
//            var addDocumentDto = new AddDocumentForDoetDTO
//            {
//                UniversityId = 1,
//                ForRoleId = 2,
//                Title = "Unauthorized Document",
//                Description = "Unauthorized access test",
//                DocumentFile = "file.pdf"
//            };

//            _documentRepositoryMock.Setup(repo => repo.AddDocumentForDoetAsync(It.IsAny<Document>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _documentService.AddDocumentForDoetAsync(addDocumentDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add document: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task AddDocumentForDoetAsync_ShouldHandleServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var addDocumentDto = new AddDocumentForDoetDTO
//            {
//                UniversityId = 1,
//                ForRoleId = 2,
//                Title = "Error Document",
//                Description = "Server error test",
//                DocumentFile = "file.pdf"
//            };

//            _documentRepositoryMock.Setup(repo => repo.AddDocumentForDoetAsync(It.IsAny<Document>()))
//                .ThrowsAsync(new Exception("Server error"));

//            // Act
//            var result = await _documentService.AddDocumentForDoetAsync(addDocumentDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding document: Server error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task AddDocumentForDoetAsync_ShouldAssignDefaultStatus_WhenDocumentIsAdded()
//        {
//            // Arrange
//            var addDocumentDto = new AddDocumentForDoetDTO
//            {
//                UniversityId = 1,
//                ForRoleId = 2,
//                Title = "Document with Default Status",
//                Description = "Default status test",
//                DocumentFile = "file.pdf"
//            };

//            var document = new Document
//            {
//                DocumentId = 1,
//                UniversityId = addDocumentDto.UniversityId,
//                RoleId = addDocumentDto.ForRoleId,
//                Title = addDocumentDto.Title,
//                Description = addDocumentDto.Description,
//                DocumentFile = addDocumentDto.DocumentFile,
//                CreatedAt = DateTime.Now,
//                Status = "Active"
//            };

//            _documentRepositoryMock.Setup(repo => repo.AddDocumentForDoetAsync(It.IsAny<Document>()))
//                .ReturnsAsync(document);
//            _mapperMock.Setup(mapper => mapper.Map<Document>(addDocumentDto)).Returns(document);
//            _mapperMock.Setup(mapper => mapper.Map<AddDocumentForDoetDTO>(document)).Returns(addDocumentDto);

//            // Act
//            var result = await _documentService.AddDocumentForDoetAsync(addDocumentDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Document added successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual("Active", document.Status); // Ensures default status is set to Active
//        }

//        #endregion


//        #region DeleteDocumentForDoetAsync

//        // Test for DeleteDocumentForDoetAsync

//        [Test]
//        public async Task DeleteDocumentForDoetAsync_ShouldDeleteDocument_WhenDocumentExists()
//        {
//            var deleteDocumentDto = new DeleteDocumentForDoetDTO { DocumentId = 1 };
//            var document = new Document { DocumentId = 1, Title = "Doet Document to Delete", Role = new Role { Name = "DOET" } };

//            _documentRepositoryMock.Setup(repo => repo.DeleteDocumentForDoetAsync(1)).ReturnsAsync(document);
//            _mapperMock.Setup(m => m.Map<DeleteDocumentForDoetDTO>(document)).Returns(deleteDocumentDto);

//            var result = await _documentService.DeleteDocumentForDoetAsync(deleteDocumentDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document has been permanently deleted successfully.", result.Message);
//        }


//        [Test]
//        public async Task DeleteDocumentForDoetAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
//        {
//            // Arrange
//            var deleteDocumentDto = new DeleteDocumentForDoetDTO { DocumentId = 1 };

//            _documentRepositoryMock.Setup(repo => repo.DeleteDocumentForDoetAsync(1))
//                .ThrowsAsync(new KeyNotFoundException("Document not found"));

//            // Act
//            var result = await _documentService.DeleteDocumentForDoetAsync(deleteDocumentDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Document not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteDocumentForDoetAsync_ShouldHandleUnauthorizedAccessException()
//        {
//            // Arrange
//            var deleteDocumentDto = new DeleteDocumentForDoetDTO { DocumentId = 1 };

//            _documentRepositoryMock.Setup(repo => repo.DeleteDocumentForDoetAsync(It.IsAny<int>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _documentService.DeleteDocumentForDoetAsync(deleteDocumentDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while hard delete document: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteDocumentForDoetAsync_ShouldReturnServerError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            var deleteDocumentDto = new DeleteDocumentForDoetDTO { DocumentId = 1 };

//            _documentRepositoryMock.Setup(repo => repo.DeleteDocumentForDoetAsync(It.IsAny<int>()))
//                .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _documentService.DeleteDocumentForDoetAsync(deleteDocumentDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error permanently deleting document: Database error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        #endregion


//        #region UpdateDocumentForDoetAsync
//        [Test]
//        public async Task UpdateDocumentForDoetAsync_ShouldUpdateDocumentSuccessfully_WhenDataIsValid()
//        {
//            // Arrange
//            var updateDocumentDto = new UpdateDocumentForDoetDTO { DocumentId = 1, Title = "Updated Title" };
//            var document = new Document { DocumentId = 1, Title = "Updated Title" };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForDoetAsync(It.IsAny<Document>()))
//                                   .ReturnsAsync(document);
//            _mapperMock.Setup(m => m.Map<UpdateDocumentForDoetDTO>(It.IsAny<Document>())).Returns(updateDocumentDto);

//            // Act
//            var result = await _documentService.UpdateDocumentForDoetAsync(updateDocumentDto);

//            // Assert
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document updated successfully!", result.Message);
//            Assert.AreEqual("Updated Title", result.Data.Title);
//        }

//        [Test]
//        public async Task UpdateDocumentForDoetAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
//        {
//            // Arrange
//            var updateDocumentDto = new UpdateDocumentForDoetDTO { DocumentId = 1, Title = "Non-existing Document" };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForDoetAsync(It.IsAny<Document>()))
//                                   .ThrowsAsync(new KeyNotFoundException("Document not found"));

//            // Act
//            var result = await _documentService.UpdateDocumentForDoetAsync(updateDocumentDto);

//            // Assert
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Document not found", result.Message);
//        }

//        [Test]
//        public async Task UpdateDocumentForDoetAsync_ShouldHandleUnauthorizedAccessException()
//        {
//            // Arrange
//            var updateDocumentDto = new UpdateDocumentForDoetDTO { DocumentId = 1, Title = "Restricted Document" };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForDoetAsync(It.IsAny<Document>()))
//                                   .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _documentService.UpdateDocumentForDoetAsync(updateDocumentDto);

//            // Assert
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update document: Access denied", result.Message);
//        }


//        [Test]
//        public async Task UpdateDocumentForDoetAsync_ShouldReturnInternalServerError_WhenUnexpectedErrorOccurs()
//        {
//            // Arrange
//            var updateDocumentDto = new UpdateDocumentForDoetDTO { DocumentId = 1, Title = "Valid Title" };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForDoetAsync(It.IsAny<Document>()))
//                                   .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _documentService.UpdateDocumentForDoetAsync(updateDocumentDto);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating document: Database error", result.Message);
//        }

//        #endregion


//        #region UpdateDocumentStatusForDoetAsync

//        // Test for a successful status update
//        [Test]
//        public async Task UpdateDocumentStatusForDoetAsync_ShouldUpdateStatusSuccessfully_WhenDataIsValid()
//        {
//            // Arrange
//            var updateStatusDto = new UpdateDocumentStatusForDoetDTO
//            {
//                DocumentId = 1,
//                Status = "Inactive"
//            };

//            var document = new Document
//            {
//                DocumentId = 1,
//                Status = "Active"
//            };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>())).ReturnsAsync(document);
//            _mapperMock.Setup(m => m.Map<UpdateDocumentStatusForDoetDTO>(document)).Returns(updateStatusDto);

//            // Act
//            var result = await _documentService.UpdateDocumentStatusForDoetAsync(updateStatusDto);

//            // Assert
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document updated successfully!", result.Message);
//            Assert.AreEqual("Inactive", result.Data.Status);
//        }

//        // Test for missing document
//        [Test]
//        public async Task UpdateDocumentStatusForDoetAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
//        {
//            // Arrange
//            var updateStatusDto = new UpdateDocumentStatusForDoetDTO
//            {
//                DocumentId = 999, // Non-existing ID
//                Status = "Active"
//            };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>()))
//                                   .ThrowsAsync(new KeyNotFoundException("Document not found"));

//            // Act
//            var result = await _documentService.UpdateDocumentStatusForDoetAsync(updateStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Document not found", result.Message);
//        }

//        // Test for unauthorized access exception
//        [Test]
//        public async Task UpdateDocumentStatusForDoetAsync_ShouldReturnUnauthorized_WhenAccessIsDenied()
//        {
//            // Arrange
//            var updateStatusDto = new UpdateDocumentStatusForDoetDTO
//            {
//                DocumentId = 1,
//                Status = "Inactive"
//            };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>()))
//                                   .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _documentService.UpdateDocumentStatusForDoetAsync(updateStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update document status: Access denied", result.Message);
//        }

//        // Test for a general server error
//        [Test]
//        public async Task UpdateDocumentStatusForDoetAsync_ShouldReturnServerError_WhenUnexpectedErrorOccurs()
//        {
//            // Arrange
//            var updateStatusDto = new UpdateDocumentStatusForDoetDTO
//            {
//                DocumentId = 1,
//                Status = "Active"
//            };

//            _documentRepositoryMock.Setup(repo => repo.UpdateDocumentForAdminAsync(It.IsAny<Document>()))
//                                   .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _documentService.UpdateDocumentStatusForDoetAsync(updateStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating document: Database error", result.Message);
//        }

//        #endregion


//        #region GetAllStatusesDocumentForDoetAsync

//        // Test for successful status retrieval
//        [Test]
//        public async Task GetAllStatusesDocumentForDoetAsync_ShouldReturnStatusList_WhenCalled()
//        {
//            // Act
//            var result = await _documentService.GetAllStatusesDocumentForDoetAsync();

//            // Assert
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Status List retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(2, result.Data.Count); // Expecting "Active" and "Inactive"
//            Assert.IsTrue(result.Data.Exists(s => s.Status == "Active"));
//            Assert.IsTrue(result.Data.Exists(s => s.Status == "Inactive"));
//        }

//        // Test for handling an empty status list
//        [Test]
//        public async Task GetAllStatusesDocumentForDoetAsync_ShouldReturnEmptyList_WhenNoStatusesDefined()
//        {
//            // Arrange
//            var emptyStatusList = new List<StatusDocumentListForDoetDTO>();
//            _mapperMock.Setup(m => m.Map<List<StatusDocumentListForDoetDTO>>(It.IsAny<List<Document>>())).Returns(emptyStatusList);

//            // Act
//            var result = await _documentService.GetAllStatusesDocumentForDoetAsync();

//            // Assert
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Status List retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(2, result.Data.Count);
//        }





//        #endregion

//        #endregion



//        #region Guest


//        [Test]
//        public async Task GetInternshipProcessDocumentAsync_ShouldReturnDocument_WhenDocumentExists()
//        {
//            // Arrange
//            var document = new Document
//            {
//                DocumentId = 1,
//                Title = "Internship Process",
//                DocumentFile = "process.pdf",
//                Description = "Document for internship process",
              
//            };

//            var documentDto = new DocumentInternshipProcessForGuestDTO
//            {
//                DocumentId = document.DocumentId,
//                Title = document.Title,
//                DocumentFile = document.DocumentFile,
//                Description = document.Description,
               
//            };

//            _documentRepositoryMock.Setup(repo => repo.GetInternshipProcessDocumentAsync()).ReturnsAsync(document);
//            _mapperMock.Setup(mapper => mapper.Map<DocumentInternshipProcessForGuestDTO>(document)).Returns(documentDto);

//            // Act
//            var result = await _documentService.GetInternshipProcessDocumentAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document internship process retrieved successfully!", result.Message);
//            Assert.AreEqual(documentDto, result.Data);
//        }


//        [Test]
//        public async Task GetInternshipProcessDocumentAsync_ShouldReturnEmptyUniversity_WhenNoUniversityExists()
//        {
//            // Arrange
//            var document = new Document
//            {
//                DocumentId = 1,
//                Title = "Internship Process",
//                DocumentFile = "process.pdf",
//                Description = "Document without a university"
//            };

//            var documentDto = new DocumentInternshipProcessForGuestDTO
//            {
//                DocumentId = document.DocumentId,
//                Title = document.Title,
//                DocumentFile = document.DocumentFile,
//                Description = document.Description,
//                University = null
//            };

//            _documentRepositoryMock.Setup(repo => repo.GetInternshipProcessDocumentAsync()).ReturnsAsync(document);
//            _mapperMock.Setup(mapper => mapper.Map<DocumentInternshipProcessForGuestDTO>(document)).Returns(documentDto);

//            // Act
//            var result = await _documentService.GetInternshipProcessDocumentAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document internship process retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.IsNull(result.Data.University);
//            Assert.AreEqual("Document without a university", result.Data.Description);
//        }



//        #endregion

//        #region Common

//        #region GetAllDocumentsAsync Tests

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldReturnPagedDocumentsList_WhenDocumentsExist()
//        {
//            // Arrange
//            var documents = new List<Document>
//    {
//        new Document { DocumentId = 1, Title = "Sample Document", Status = "Active" }
//    };
//            var documentDtos = documents.Select(doc => new DocumentListForCommonDTO
//            {
//                DocumentId = doc.DocumentId,
//                Title = doc.Title,
//                UpdatedAt = doc.UpdatedAt
//            }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync(It.IsAny<string>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForCommonDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("admin", "Sample", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Documents list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(1, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldReturnEmptyPagedList_WhenNoMatchingDocuments()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync(It.IsAny<string>(), It.IsAny<string>()))
//                .ReturnsAsync(new List<Document>());

//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForCommonDTO>>(It.IsAny<List<Document>>())).Returns(new List<DocumentListForCommonDTO>());

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("admin", "NonExistent", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Documents list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldReturnNotFound_WhenNoDocumentsExist()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync(It.IsAny<string>(), It.IsAny<string>()))
//                .ThrowsAsync(new KeyNotFoundException("No documents found for the specified role."));

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("guest", "Sample", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("No documents found for the specified role.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldHandleServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync(It.IsAny<string>(), It.IsAny<string>()))
//                .ThrowsAsync(new Exception("Server error"));

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("admin", "Sample", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving documents list: Server error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldReturnFilteredList_WhenTitleContainsKeyword()
//        {
//            // Arrange
//            var documents = new List<Document>
//    {
//        new Document { DocumentId = 1, Title = "Guide to Admin" },
//        new Document { DocumentId = 2, Title = "Admin Manual" }
//    };
//            var documentDtos = documents.Select(doc => new DocumentListForCommonDTO
//            {
//                DocumentId = doc.DocumentId,
//                Title = doc.Title
//            }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync("admin", "Admin"))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForCommonDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("admin", "Admin", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Documents list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual("Guide to Admin", result.Data.Items[0].Title);
//            Assert.AreEqual("Admin Manual", result.Data.Items[1].Title);
//        }

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldReturnPagedList_WhenLargeDataSetIsProvided()
//        {
//            // Arrange
//            var documents = new List<Document>();
//            for (int i = 1; i <= 50; i++)
//            {
//                documents.Add(new Document { DocumentId = i, Title = $"Document {i}", Status = "Active" });
//            }

//            var documentDtos = documents.Select(doc => new DocumentListForCommonDTO
//            {
//                DocumentId = doc.DocumentId,
//                Title = doc.Title
//            }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync(It.IsAny<string>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForCommonDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("admin", null, 2, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Documents list retrieved successfully!", result.Message);
//            Assert.AreEqual(50, result.Data.TotalCount);
//            Assert.AreEqual(15, result.Data.Items.Count);
//            Assert.AreEqual(2, result.Data.CurrentPage);
//            Assert.AreEqual(4, result.Data.TotalPages); // 50 documents / 15 per page = 4 pages
//        }

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldReturnEmptyList_WhenRoleDoesNotMatch()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync("guest", It.IsAny<string>()))
//                .ReturnsAsync(new List<Document>());

//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForCommonDTO>>(It.IsAny<List<Document>>())).Returns(new List<DocumentListForCommonDTO>());

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("guest", null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Documents list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldReturnFirstPage_WhenPageNumberExceedsTotalPages()
//        {
//            // Arrange
//            var documents = new List<Document>
//    {
//        new Document { DocumentId = 1, Title = "Document1", Status = "Active" },
//        new Document { DocumentId = 2, Title = "Document2", Status = "Inactive" }
//    };
//            var documentDtos = documents.Select(doc => new DocumentListForCommonDTO
//            {
//                DocumentId = doc.DocumentId,
//                Title = doc.Title
//            }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync(It.IsAny<string>(), It.IsAny<string>()))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForCommonDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("admin", null, 10, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Documents list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldReturnFilteredPagedResults_WhenBothRoleAndTitleAreSpecified()
//        {
//            // Arrange
//            var documents = new List<Document>
//    {
//        new Document { DocumentId = 1, Title = "Admin Policy" },
//        new Document { DocumentId = 2, Title = "Admin Guidelines" }
//    };
//            var documentDtos = documents.Select(doc => new DocumentListForCommonDTO
//            {
//                DocumentId = doc.DocumentId,
//                Title = doc.Title
//            }).ToList();

//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync("admin", "Admin"))
//                .ReturnsAsync(documents);
//            _mapperMock.Setup(mapper => mapper.Map<List<DocumentListForCommonDTO>>(documents)).Returns(documentDtos);

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("admin", "Admin", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Documents list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(2, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllDocumentsAsync_ShouldReturnError_WhenTitleAndRoleAreEmpty()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetAllDocumentsAsync(It.IsAny<string>(), null))
//                .ThrowsAsync(new ArgumentException("Role is required"));

//            // Act
//            var result = await _documentService.GetAllDocumentsAsync("", null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving documents list: Role is required", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        #endregion

//        #region GetDocumentDetailAsync Tests

//        [Test]
//        public async Task GetDocumentDetailAsync_ShouldReturnDocumentDetail_WhenDocumentExists()
//        {
//            // Arrange
//            var document = new Document { DocumentId = 1, Title = "Sample Document", Status = "Active" };
//            var documentDto = new DocumentDetailForCommonDTO
//            {
//                DocumentId = document.DocumentId,
//                Title = document.Title,
//                UpdatedAt = document.UpdatedAt
//            };

//            _documentRepositoryMock.Setup(repo => repo.GetDocumentDetailAsync(It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(document);
//            _mapperMock.Setup(mapper => mapper.Map<DocumentDetailForCommonDTO>(document)).Returns(documentDto);

//            // Act
//            var result = await _documentService.GetDocumentDetailAsync(1, "admin");

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Document detail retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual("Sample Document", result.Data.Title);
//        }

//        [Test]
//        public async Task GetDocumentDetailAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetDocumentDetailAsync(It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new KeyNotFoundException("Document detail not found."));

//            // Act
//            var result = await _documentService.GetDocumentDetailAsync(99, "admin");

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Document detail not found.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetDocumentDetailAsync_ShouldHandleServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            _documentRepositoryMock.Setup(repo => repo.GetDocumentDetailAsync(It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new Exception("Server error"));

//            // Act
//            var result = await _documentService.GetDocumentDetailAsync(1, "admin");

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving document details: Server error", result.Message);
//            Assert.IsNull(result.Data);
//        }




//        #endregion

//        #endregion




//    }
//}
