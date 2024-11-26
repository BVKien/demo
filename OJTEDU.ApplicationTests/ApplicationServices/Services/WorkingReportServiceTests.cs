//using AutoMapper;
//using Moq;
//using NUnit.Framework;
//using OJTEDU.Application.ApplicationServices.Services;
//using OJTEDU.Application.DTOs;
//using OJTEDU.Domain.Entities;
//using OJTEDU.Domain.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using static OJTEDU.Application.DTOs.WorkingReportDTO;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class WorkingReportServiceTests
//    {
//        private Mock<IWorkingReportRepository> _workingReportRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private WorkingReportService _workingReportService;

//        [SetUp]
//        public void Setup()
//        {
//            _workingReportRepositoryMock = new Mock<IWorkingReportRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _workingReportService = new WorkingReportService(_workingReportRepositoryMock.Object, _mapperMock.Object);
//        }

//        #region GetAllByStudentIdAsync Tests

//        [Test]
//        public async Task GetAllByStudentIdAsync_ShouldReturnWorkingReports_WhenReportsExist()
//        {
//            // Arrange
//            var studentId = 1;
//            var workingReports = new List<WorkingReport> { new WorkingReport { WorkingReportId = 1, ReportContent = "Report Content" } };
//            var reportDto = new List<WorkingReportListForStudentDTO> { new WorkingReportListForStudentDTO { WorkingReportId = 1, ReportContent = "Report Content" } };

//            _workingReportRepositoryMock.Setup(repo => repo.GetAllByStudentIdAsync(studentId)).ReturnsAsync(workingReports);
//            _mapperMock.Setup(mapper => mapper.Map<List<WorkingReportListForStudentDTO>>(workingReports)).Returns(reportDto);

//            // Act
//            var result = await _workingReportService.GetAllByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Working report list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(reportDto, result.Data);
//        }

//        [Test]
//        public async Task GetAllByStudentIdAsync_ShouldReturnReportsWithScores_WhenScoresExist()
//        {
//            // Arrange
//            int studentId = 5;
//            var workingReports = new List<WorkingReport>
//        {
//            new WorkingReport { WorkingReportId = 1, MentorScore = 9.0, LecturerScore = 8.5 }
//        };
//            var reportDtos = new List<WorkingReportListForStudentDTO>
//        {
//            new WorkingReportListForStudentDTO { WorkingReportId = 1, MentorScore = 9.0, LecturerScore = 8.5 }
//        };

//            _workingReportRepositoryMock.Setup(repo => repo.GetAllByStudentIdAsync(studentId)).ReturnsAsync(workingReports);
//            _mapperMock.Setup(mapper => mapper.Map<List<WorkingReportListForStudentDTO>>(workingReports)).Returns(reportDtos);

//            // Act
//            var result = await _workingReportService.GetAllByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Working report list retrieved successfully!", result.Message);
//            Assert.AreEqual(reportDtos, result.Data);
//        }

//        [Test]
//        public async Task GetAllByStudentIdAsync_ShouldReturnReportsWithAttachments_WhenAttachmentsExist()
//        {
//            // Arrange
//            int studentId = 6;
//            var workingReports = new List<WorkingReport>
//        {
//            new WorkingReport { WorkingReportId = 1, FileAttachment = "report1.pdf" }
//        };
//            var reportDtos = new List<WorkingReportListForStudentDTO>
//        {
//            new WorkingReportListForStudentDTO { WorkingReportId = 1, FileAttachment = "report1.pdf" }
//        };

//            _workingReportRepositoryMock.Setup(repo => repo.GetAllByStudentIdAsync(studentId)).ReturnsAsync(workingReports);
//            _mapperMock.Setup(mapper => mapper.Map<List<WorkingReportListForStudentDTO>>(workingReports)).Returns(reportDtos);

//            // Act
//            var result = await _workingReportService.GetAllByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Working report list retrieved successfully!", result.Message);
//            Assert.AreEqual(reportDtos, result.Data);
//        }


//        [Test]
//        public async Task GetAllByStudentIdAsync_ShouldReturnSingleReport_WhenOnlyOneReportExists()
//        {
//            // Arrange
//            int studentId = 11;
//            var report = new WorkingReport { WorkingReportId = 1, StudentId = studentId, ReportContent = "Single Report" };
//            var reportDto = new WorkingReportListForStudentDTO { WorkingReportId = 1, ReportContent = "Single Report" };

//            _workingReportRepositoryMock.Setup(repo => repo.GetAllByStudentIdAsync(studentId)).ReturnsAsync(new List<WorkingReport> { report });
//            _mapperMock.Setup(mapper => mapper.Map<List<WorkingReportListForStudentDTO>>(It.IsAny<List<WorkingReport>>())).Returns(new List<WorkingReportListForStudentDTO> { reportDto });

//            // Act
//            var result = await _workingReportService.GetAllByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Working report list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Single Report", result.Data.First().ReportContent);
//        }

//        [Test]
//        public async Task GetAllByStudentIdAsync_ShouldHandleLargeReportList()
//        {
//            // Arrange
//            int studentId = 12;
//            var reports = Enumerable.Range(1, 1000).Select(i => new WorkingReport
//            {
//                WorkingReportId = i,
//                StudentId = studentId,
//                ReportContent = $"Report Content {i}"
//            }).ToList();
//            var reportDtos = reports.Select(r => new WorkingReportListForStudentDTO
//            {
//                WorkingReportId = r.WorkingReportId,
//                ReportContent = r.ReportContent
//            }).ToList();

//            _workingReportRepositoryMock.Setup(repo => repo.GetAllByStudentIdAsync(studentId)).ReturnsAsync(reports);
//            _mapperMock.Setup(mapper => mapper.Map<List<WorkingReportListForStudentDTO>>(reports)).Returns(reportDtos);

//            // Act
//            var result = await _workingReportService.GetAllByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Working report list retrieved successfully!", result.Message);
//            Assert.AreEqual(1000, result.Data.Count);
//            Assert.AreEqual("Report Content 1", result.Data.First().ReportContent);
//        }

//        [Test]
//        public async Task GetAllByStudentIdAsync_ShouldReturnPartialData_WhenMapperFailsOnSomeItems()
//        {
//            // Arrange
//            int studentId = 13;
//            var reports = new List<WorkingReport>
//    {
//        new WorkingReport { WorkingReportId = 1, StudentId = studentId, ReportContent = "Valid Report" },
//        null
//    };
//            var reportDtos = new List<WorkingReportListForStudentDTO>
//    {
//        new WorkingReportListForStudentDTO { WorkingReportId = 1, ReportContent = "Valid Report" }
//    };

//            _workingReportRepositoryMock.Setup(repo => repo.GetAllByStudentIdAsync(studentId)).ReturnsAsync(reports);
//            _mapperMock.Setup(mapper => mapper.Map<List<WorkingReportListForStudentDTO>>(It.IsAny<List<WorkingReport>>())).Returns(reportDtos);

//            // Act
//            var result = await _workingReportService.GetAllByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Working report list retrieved successfully!", result.Message);
//            Assert.AreEqual(1, result.Data.Count); // Only the valid report is mapped
//        }

//        [Test]
//        public async Task GetAllByStudentIdAsync_ShouldReturnReportsInOrderByDate()
//        {
//            // Arrange
//            int studentId = 14;
//            var reports = new List<WorkingReport>
//    {
//        new WorkingReport { WorkingReportId = 1, StudentId = studentId, ReportContent = "Report A", ReportDate = new DateTime(2023, 1, 1) },
//        new WorkingReport { WorkingReportId = 2, StudentId = studentId, ReportContent = "Report B", ReportDate = new DateTime(2023, 1, 2) }
//    };
//            var reportDtos = reports.Select(r => new WorkingReportListForStudentDTO
//            {
//                WorkingReportId = r.WorkingReportId,
//                ReportContent = r.ReportContent
//            }).ToList();

//            _workingReportRepositoryMock.Setup(repo => repo.GetAllByStudentIdAsync(studentId)).ReturnsAsync(reports);
//            _mapperMock.Setup(mapper => mapper.Map<List<WorkingReportListForStudentDTO>>(It.IsAny<List<WorkingReport>>())).Returns(reportDtos);

//            // Act
//            var result = await _workingReportService.GetAllByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Working report list retrieved successfully!", result.Message);
//            Assert.AreEqual("Report A", result.Data.First().ReportContent);
//        }


//        [Test]
//        public async Task GetAllByStudentIdAsync_ShouldHandleReportsWithNullReportContent()
//        {
//            // Arrange
//            int studentId = 19;
//            var reports = new List<WorkingReport>
//    {
//        new WorkingReport { WorkingReportId = 1, StudentId = studentId, ReportContent = null }
//    };
//            var reportDtos = new List<WorkingReportListForStudentDTO>
//    {
//        new WorkingReportListForStudentDTO { WorkingReportId = 1, ReportContent = null }
//    };

//            _workingReportRepositoryMock.Setup(repo => repo.GetAllByStudentIdAsync(studentId)).ReturnsAsync(reports);
//            _mapperMock.Setup(mapper => mapper.Map<List<WorkingReportListForStudentDTO>>(reports)).Returns(reportDtos);

//            // Act
//            var result = await _workingReportService.GetAllByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Working report list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.IsNull(result.Data.First().ReportContent);
//        }


//        #endregion

//        #region CreateWorkingReportAsync Tests

//        [Test]
//        public async Task CreateWorkingReportAsync_ShouldReturnCreatedReport_WhenDataIsValid()
//        {
//            // Arrange
//            var reportInfo = new CreateWorkingReportForStudentDTO { MentorId = 1, LecturerId = 1, StudentId = 1, ReportContent = "Content" };
//            var createdReport = new WorkingReport { WorkingReportId = 1, ReportContent = "Content" };
//            var reportDto = new CreateWorkingReportForStudentDTO { MentorId = 1, LecturerId = 1, StudentId = 1, ReportContent = "Content" };

//            _workingReportRepositoryMock.Setup(repo => repo.CreateWorkingReportAsync(It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>())).ReturnsAsync(createdReport);
//            _mapperMock.Setup(mapper => mapper.Map<CreateWorkingReportForStudentDTO>(createdReport)).Returns(reportDto);

//            // Act
//            var result = await _workingReportService.CreateWorkingReportAsync(reportInfo, "report.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Create working report successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(reportDto, result.Data);
//        }



//        [Test]
//        public async Task CreateWorkingReportAsync_ShouldReturnSuccess_WhenReportIsCreatedSuccessfully()
//        {
//            // Arrange
//            var reportDto = new CreateWorkingReportForStudentDTO { MentorId = 1, LecturerId = 2, StudentId = 3, ReportContent = "Sample Report" };
//            var report = new WorkingReport { WorkingReportId = 1, MentorId = 1, LecturerId = 2, StudentId = 3, ReportContent = "Sample Report" };

//            _workingReportRepositoryMock.Setup(repo => repo.CreateWorkingReportAsync(It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                .ReturnsAsync(report);
//            _mapperMock.Setup(mapper => mapper.Map<CreateWorkingReportForStudentDTO>(report)).Returns(reportDto);

//            // Act
//            var result = await _workingReportService.CreateWorkingReportAsync(reportDto, "report.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Create working report successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(reportDto, result.Data);
//        }


//        [Test]
//        public async Task CreateWorkingReportAsync_ShouldReturnError_WhenLecturerDoesNotExist()
//        {
//            // Arrange
//            var reportDto = new CreateWorkingReportForStudentDTO { MentorId = 1, LecturerId = 99, StudentId = 3, ReportContent = "Sample Report" };
//            _workingReportRepositoryMock.Setup(repo => repo.CreateWorkingReportAsync(It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                .ThrowsAsync(new KeyNotFoundException("Not found lecturer with id: 99"));

//            // Act
//            var result = await _workingReportService.CreateWorkingReportAsync(reportDto, "report.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error create working report jpb: Not found lecturer with id: 99. ", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task CreateWorkingReportAsync_ShouldReturnError_WhenStudentDoesNotExist()
//        {
//            // Arrange
//            var reportDto = new CreateWorkingReportForStudentDTO { MentorId = 1, LecturerId = 2, StudentId = 99, ReportContent = "Sample Report" };
//            _workingReportRepositoryMock.Setup(repo => repo.CreateWorkingReportAsync(It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                .ThrowsAsync(new KeyNotFoundException("Not found student with id: 99"));

//            // Act
//            var result = await _workingReportService.CreateWorkingReportAsync(reportDto, "report.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error create working report jpb: Not found student with id: 99. ", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task CreateWorkingReportAsync_ShouldHandleNullFileData()
//        {
//            // Arrange
//            var reportDto = new CreateWorkingReportForStudentDTO { MentorId = 1, LecturerId = 2, StudentId = 3, ReportContent = "Sample Report" };
//            var report = new WorkingReport { WorkingReportId = 1, MentorId = 1, LecturerId = 2, StudentId = 3, ReportContent = "Sample Report" };

//            _workingReportRepositoryMock.Setup(repo => repo.CreateWorkingReportAsync(It.IsAny<WorkingReport>(), "report.pdf", null))
//                .ReturnsAsync(report);
//            _mapperMock.Setup(mapper => mapper.Map<CreateWorkingReportForStudentDTO>(report)).Returns(reportDto);

//            // Act
//            var result = await _workingReportService.CreateWorkingReportAsync(reportDto, "report.pdf", null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Create working report successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(reportDto, result.Data);
//        }

//        [Test]
//        public async Task CreateWorkingReportAsync_ShouldReturnError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            var reportDto = new CreateWorkingReportForStudentDTO { MentorId = 1, LecturerId = 2, StudentId = 3, ReportContent = "Sample Report" };
//            _workingReportRepositoryMock.Setup(repo => repo.CreateWorkingReportAsync(It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _workingReportService.CreateWorkingReportAsync(reportDto, "report.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error create working report jpb: Database error. ", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task CreateWorkingReportAsync_ShouldReturnServerError_WhenDatabaseConnectionFails()
//        {
//            // Arrange
//            var reportDto = new CreateWorkingReportForStudentDTO { MentorId = 1, LecturerId = 2, StudentId = 3, ReportContent = "Sample Report" };
//            _workingReportRepositoryMock.Setup(repo => repo.CreateWorkingReportAsync(It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                .ThrowsAsync(new Exception("Database connection failed"));

//            // Act
//            var result = await _workingReportService.CreateWorkingReportAsync(reportDto, "report.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error create working report jpb: Database connection failed. ", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task CreateWorkingReportAsync_ShouldReturnSuccess_WhenReportContentContainsSpecialCharacters()
//        {
//            // Arrange
//            var reportDto = new CreateWorkingReportForStudentDTO { MentorId = 1, LecturerId = 2, StudentId = 3, ReportContent = "Report with special characters: !@#$%^&*()" };
//            var report = new WorkingReport { WorkingReportId = 1, MentorId = 1, LecturerId = 2, StudentId = 3, ReportContent = reportDto.ReportContent };

//            _workingReportRepositoryMock.Setup(repo => repo.CreateWorkingReportAsync(It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                .ReturnsAsync(report);
//            _mapperMock.Setup(mapper => mapper.Map<CreateWorkingReportForStudentDTO>(report)).Returns(reportDto);

//            // Act
//            var result = await _workingReportService.CreateWorkingReportAsync(reportDto, "special_characters_report.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Create working report successfully!", result.Message);
//            Assert.AreEqual(reportDto, result.Data);
//        }


//        #endregion

//        #region UpdateWorkingReportAsync Tests

//        [Test]
//        public async Task UpdateWorkingReportAsync_ShouldReturnUpdatedReport_WhenDataIsValid()
//        {
//            // Arrange
//            var workingReportId = 1;
//            var reportInfo = new UpdateWorkingReportForStudentDTO { ReportContent = "Updated Content" };
//            var updatedReport = new WorkingReport { WorkingReportId = workingReportId, ReportContent = "Updated Content" };
//            var reportDto = new UpdateWorkingReportForStudentDTO { ReportContent = "Updated Content" };

//            _workingReportRepositoryMock.Setup(repo => repo.UpdateWorkingReportAsync(workingReportId, It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>())).ReturnsAsync(updatedReport);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateWorkingReportForStudentDTO>(updatedReport)).Returns(reportDto);

//            // Act
//            var result = await _workingReportService.UpdateWorkingReportAsync(workingReportId, reportInfo, "report_updated.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Update working report successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(reportDto, result.Data);
//        }

//        [Test]
//        public async Task UpdateWorkingReportAsync_ShouldReturnUpdatedReport_WhenValidUpdateProvided()
//        {
//            // Arrange
//            int reportId = 5;
//            var updatedContent = "Updated Report Content";
//            var existingReport = new WorkingReport { WorkingReportId = reportId, ReportContent = "Old Content", FileAttachment = "oldfile.pdf" };
//            var updatedReportDto = new UpdateWorkingReportForStudentDTO { ReportContent = updatedContent, FileAttachment = "newfile.pdf" };

//            _workingReportRepositoryMock.Setup(repo => repo.UpdateWorkingReportAsync(reportId, It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                        .ReturnsAsync(new WorkingReport { WorkingReportId = reportId, ReportContent = updatedContent, FileAttachment = "newfile.pdf" });

//            _mapperMock.Setup(mapper => mapper.Map<UpdateWorkingReportForStudentDTO>(It.IsAny<WorkingReport>()))
//                       .Returns(updatedReportDto);

//            // Act
//            var result = await _workingReportService.UpdateWorkingReportAsync(reportId, updatedReportDto, "newfile.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Update working report successfully!", result.Message);
//            Assert.AreEqual(updatedContent, result.Data.ReportContent);
//            Assert.AreEqual("newfile.pdf", result.Data.FileAttachment);
//        }

//        [Test]
//        public async Task UpdateWorkingReportAsync_ShouldReturnError_WhenReportIdDoesNotExist()
//        {
//            // Arrange
//            int reportId = 999; // Non-existent ID
//            var reportDto = new UpdateWorkingReportForStudentDTO { ReportContent = "New Content", FileAttachment = "newfile.pdf" };

//            _workingReportRepositoryMock.Setup(repo => repo.UpdateWorkingReportAsync(reportId, It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                        .ThrowsAsync(new KeyNotFoundException("Not found working report with id: 999"));

//            // Act
//            var result = await _workingReportService.UpdateWorkingReportAsync(reportId, reportDto, "newfile.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Error update working report jpb"));
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task UpdateWorkingReportAsync_ShouldReturnError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            int reportId = 8;
//            var reportDto = new UpdateWorkingReportForStudentDTO { ReportContent = "Updated Content", FileAttachment = "newfile.pdf" };

//            _workingReportRepositoryMock.Setup(repo => repo.UpdateWorkingReportAsync(reportId, It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                        .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _workingReportService.UpdateWorkingReportAsync(reportId, reportDto, "newfile.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Error update working report jpb"));
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task UpdateWorkingReportAsync_ShouldReturnError_WhenStudentNotFound()
//        {
//            // Arrange
//            int reportId = 11;
//            var reportDto = new UpdateWorkingReportForStudentDTO { ReportContent = "New Content", FileAttachment = "file.pdf" };

//            _workingReportRepositoryMock.Setup(repo => repo.UpdateWorkingReportAsync(reportId, It.IsAny<WorkingReport>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                        .ThrowsAsync(new KeyNotFoundException("Student not found"));

//            // Act
//            var result = await _workingReportService.UpdateWorkingReportAsync(reportId, reportDto, "file.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Error update working report jpb"));
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task UpdateWorkingReportAsync_ShouldReturnError_WhenFileSaveFails()
//        {
//            // Arrange
//            int reportId = 14;
//            var reportDto = new UpdateWorkingReportForStudentDTO { ReportContent = "Content with file save failure", FileAttachment = "failedfile.pdf" };
//            var fileData = Encoding.UTF8.GetBytes("File data");

//            _workingReportRepositoryMock.Setup(repo => repo.UpdateWorkingReportAsync(reportId, It.IsAny<WorkingReport>(), It.IsAny<string>(), fileData))
//                                        .ThrowsAsync(new IOException("File save failed"));

//            // Act
//            var result = await _workingReportService.UpdateWorkingReportAsync(reportId, reportDto, "failedfile.pdf", fileData);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Error update working report jpb"));
//            Assert.IsNull(result.Data);
//        }

       


//        #endregion
//    }
//}
