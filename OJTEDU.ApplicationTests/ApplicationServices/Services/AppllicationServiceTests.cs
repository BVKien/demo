//using Moq;
//using NUnit.Framework;
//using System;
//using System.Threading.Tasks;
//using OJTEDU.Application.DTOs;
//using OJTEDU.Application.ApplicationServices.Interfaces;
//using OJTEDU.Application.ApplicationServices.Services;
//using OJTEDU.Domain.Entities;
//using OJTEDU.Domain.Interfaces;
//using AutoMapper;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
//using static OJTEDU.Application.DTOs.AppllicationDTO;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class AppllicationServiceTests
//    {
//        private Mock<IAppllicationRepository> _appllicationRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private AppllicationService _appllicationService;

//        [SetUp]
//        public void Setup()
//        {
//            _appllicationRepositoryMock = new Mock<IAppllicationRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _appllicationService = new AppllicationService(_appllicationRepositoryMock.Object, _mapperMock.Object);
//        }

//        #region ApplyJobAsync

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnSuccess_WhenApplicationIsSuccessful()
//        {
//            // Arrange
//            var applyInfo = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CvId = 3, CoverLetter = "Cover Letter" };
//            var application = new Appllication { ApplicationId = 1, StudentId = applyInfo.StudentId, JobId = applyInfo.JobId };
//            var responseDto = new ApplyJobForStudentDTO { StudentId = applyInfo.StudentId, JobId = applyInfo.JobId };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                       .ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<ApplyJobForStudentDTO>(application)).Returns(responseDto);

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyInfo, "test.pdf", new byte[0], "cv.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Apply job successfully!", result.Message);
//            Assert.AreEqual(responseDto, result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnNotFound_WhenStudentDoesNotExist()
//        {
//            // Arrange
//            var applyInfo = new ApplyJobForStudentDTO { StudentId = 99, JobId = 2, CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new KeyNotFoundException("Not found student with id: 99"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyInfo, "test.pdf", new byte[0], "cv.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Not found student with id: 99"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnNotFound_WhenJobDoesNotExist()
//        {
//            // Arrange
//            var applyInfo = new ApplyJobForStudentDTO { StudentId = 1, JobId = 99, CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new KeyNotFoundException("Not found job with id: 99"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyInfo, "test.pdf", new byte[0], "cv.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Not found job with id: 99"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenDuplicateApplicationExists()
//        {
//            // Arrange
//            var applyInfo = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new KeyNotFoundException("Application already exists"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyInfo, "test.pdf", new byte[0], "cv.pdf", new byte[0]);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Application already exists"));
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnNotFound_WhenCvDoesNotExist()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 99 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new KeyNotFoundException("Not found CV with id: 99"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Not found CV with id: 99"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnAlreadyExists_WhenApplicationAlreadyExists()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new KeyNotFoundException("Application already exists"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Application already exists"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnServerError_WhenTestFileIsNull()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };
//            var application = new Appllication { ApplicationId = 1, StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), null, null, "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<ApplyJobForStudentDTO>(application)).Returns(applyDto);

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, null, null, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Apply job successfully!", result.Message);
//            Assert.AreEqual(applyDto, result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnServerError_WhenCvFileIsNull()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };
//            var application = new Appllication { ApplicationId = 1, StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), null, null))
//                                       .ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<ApplyJobForStudentDTO>(application)).Returns(applyDto);

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, null, null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Apply job successfully!", result.Message);
//            Assert.AreEqual(applyDto, result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenInvalidStudentId()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = -1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new ArgumentException("Invalid student ID"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Invalid student ID"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenCvIdIsInactive()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 99 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new InvalidOperationException("CV is inactive"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("CV is inactive"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenCoverLetterIsNull()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = null, CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new ArgumentException("Cover letter is required"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Cover letter is required"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenJobIdIsNull()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = null, CoverLetter = "Cover Letter", CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new ArgumentException("Job ID is required"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Job ID is required"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenCvFileIsNotProvidedButCvIdExists()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), null, null))
//                                       .ThrowsAsync(new ArgumentException("CV file is required if CV ID is provided"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, null, null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("CV file is required if CV ID is provided"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnSuccess_WhenCoverLetterAndFilesAreProvided()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Detailed Cover Letter", CvId = 3 };
//            var application = new Appllication { ApplicationId = 1, StudentId = 1, JobId = 2, CoverLetter = applyDto.CoverLetter, CvId = 3 };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<ApplyJobForStudentDTO>(application)).Returns(applyDto);

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Apply job successfully!", result.Message);
//            Assert.AreEqual(applyDto, result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenJobAlreadyApplied()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new InvalidOperationException("Job already applied for this student"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Job already applied for this student"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenStudentIsNotEligibleForJob()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new InvalidOperationException("Student is not eligible for this job"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Student is not eligible for this job"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenCvIdIsNullAndCvFileIsProvided()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = null };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new ArgumentException("CV ID must be provided if a CV file is uploaded"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("CV ID must be provided if a CV file is uploaded"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnSuccess_WhenOnlyCoverLetterAndJobIdAreProvided()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = null };
//            var application = new Appllication { ApplicationId = 1, StudentId = 1, JobId = 2, CoverLetter = applyDto.CoverLetter };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), null, null, null, null)).ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<ApplyJobForStudentDTO>(application)).Returns(applyDto);

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, null, null, null, null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Apply job successfully!", result.Message);
//            Assert.AreEqual(applyDto, result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenRepositoryThrowsUnexpectedException()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };
//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new Exception("Unexpected repository error"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Unexpected repository error"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenTestFileSizeExceedsLimit()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "largeTestFile.pdf", new byte[10485761], "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new ArgumentException("Test file size exceeds limit"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "largeTestFile.pdf", new byte[10485761], "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Test file size exceeds limit"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenStudentIsNotFound()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 999, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "testFile.pdf", It.IsAny<byte[]>(), "cvFile.pdf", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new KeyNotFoundException("Student not found"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "testFile.pdf", new byte[] { }, "cvFile.pdf", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Student not found"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task ApplyJobAsync_ShouldReturnError_WhenFileExtensionsAreInvalid()
//        {
//            // Arrange
//            var applyDto = new ApplyJobForStudentDTO { StudentId = 1, JobId = 2, CoverLetter = "Cover Letter", CvId = 3 };

//            _appllicationRepositoryMock.Setup(repo => repo.ApplyJobAsync(It.IsAny<Appllication>(), "invalidTestFile.exe", It.IsAny<byte[]>(), "invalidCvFile.txt", It.IsAny<byte[]>()))
//                                       .ThrowsAsync(new ArgumentException("File extension not allowed"));

//            // Act
//            var result = await _appllicationService.ApplyJobAsync(applyDto, "invalidTestFile.exe", new byte[] { }, "invalidCvFile.txt", new byte[] { });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("File extension not allowed"));
//            Assert.IsNull(result.Data);
//        }


//        #endregion

//        #region GetApplicationDetailByIdAsync

//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnApplicationDetail_WhenApplicationExists()
//        {
//            // Arrange
//            var applicationId = 1;
//            var application = new Appllication
//            {
//                ApplicationId = applicationId,
//                StudentId = 1,
//                JobId = 2,
//                CoverLetter = "Cover Letter",
//                CvId = 3,
//                TestFile = "testFile.pdf",
//                CvFile = "cvFile.pdf",
//                Status = "Active",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            };

//            var applicationDto = new AppllicationDetailForStudentDTO
//            {
//                StudentId = application.StudentId,
//                JobId = application.JobId,
//                CoverLetter = application.CoverLetter,
//                CvId = application.CvId,
//                TestFile = application.TestFile,
//                CvFile = application.CvFile,
//                Status = application.Status,
//                CreatedAt = application.CreatedAt,
//                UpdatedAt = application.UpdatedAt
//            };

//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId)).ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<AppllicationDetailForStudentDTO>(application)).Returns(applicationDto);

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Application detail retrieved successfully!", result.Message);
//            Assert.AreEqual(applicationDto, result.Data);
//        }

//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnNotFound_WhenApplicationDoesNotExist()
//        {
//            // Arrange
//            var applicationId = 99;
//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId))
//                                       .ThrowsAsync(new KeyNotFoundException("Not found application with id: 99"));

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Not found application with id: 99"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnServerError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            var applicationId = 1;
//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId))
//                                       .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving application detail: Database error. ", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnApplicationDetailWithEmptyCvFile_WhenNoCvFileExists()
//        {
//            // Arrange
//            var applicationId = 1;
//            var application = new Appllication
//            {
//                ApplicationId = applicationId,
//                StudentId = 1,
//                JobId = 2,
//                CoverLetter = "Cover Letter",
//                CvId = 3,
//                TestFile = "testFile.pdf",
//                CvFile = null, // Simulate no CV file
//                Status = "Active",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            };

//            var applicationDto = new AppllicationDetailForStudentDTO
//            {
//                StudentId = application.StudentId,
//                JobId = application.JobId,
//                CoverLetter = application.CoverLetter,
//                CvId = application.CvId,
//                TestFile = application.TestFile,
//                CvFile = null, // Expected mapped result
//                Status = application.Status,
//                CreatedAt = application.CreatedAt,
//                UpdatedAt = application.UpdatedAt
//            };

//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId)).ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<AppllicationDetailForStudentDTO>(application)).Returns(applicationDto);

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Application detail retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data.CvFile); // Check that CvFile is null
//        }

//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnApplicationWithOnlyMandatoryFields_WhenOptionalFieldsAreNull()
//        {
//            // Arrange
//            var applicationId = 1;
//            var application = new Appllication
//            {
//                ApplicationId = applicationId,
//                StudentId = 1,
//                JobId = 2,
//                CoverLetter = null, // No cover letter provided
//                CvId = 3,
//                TestFile = null,
//                CvFile = null,
//                Status = "Pending",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            };
//            var applicationDto = new AppllicationDetailForStudentDTO
//            {
//                StudentId = application.StudentId,
//                JobId = application.JobId,
//                CoverLetter = null,
//                CvId = application.CvId,
//                TestFile = null,
//                CvFile = null,
//                Status = application.Status,
//                CreatedAt = application.CreatedAt,
//                UpdatedAt = application.UpdatedAt
//            };

//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId)).ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<AppllicationDetailForStudentDTO>(application)).Returns(applicationDto);

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Application detail retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data.CoverLetter);
//        }

//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnEmptyResult_WhenIdIsNull()
//        {
//            // Arrange
//            int? applicationId = null;
//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId))
//                                       .ThrowsAsync(new ArgumentNullException("Application ID cannot be null"));

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Application ID cannot be null"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnCorrectResponse_WhenApplicationStatusIsRejected()
//        {
//            // Arrange
//            var applicationId = 1;
//            var application = new Appllication
//            {
//                ApplicationId = applicationId,
//                StudentId = 1,
//                JobId = 2,
//                CoverLetter = "Sample Cover Letter",
//                CvId = 3,
//                Status = "Rejected",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            };
//            var applicationDto = new AppllicationDetailForStudentDTO
//            {
//                StudentId = application.StudentId,
//                JobId = application.JobId,
//                CoverLetter = application.CoverLetter,
//                CvId = application.CvId,
//                Status = application.Status,
//                CreatedAt = application.CreatedAt,
//                UpdatedAt = application.UpdatedAt
//            };

//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId)).ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<AppllicationDetailForStudentDTO>(application)).Returns(applicationDto);

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Application detail retrieved successfully!", result.Message);
//            Assert.AreEqual("Rejected", result.Data.Status);
//        }

//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnCorrectTimestamps_WhenApplicationExists()
//        {
//            // Arrange
//            var applicationId = 1;
//            var createdDate = DateTime.Now.AddDays(-10);
//            var updatedDate = DateTime.Now.AddDays(-5);
//            var application = new Appllication
//            {
//                ApplicationId = applicationId,
//                StudentId = 1,
//                JobId = 2,
//                CreatedAt = createdDate,
//                UpdatedAt = updatedDate
//            };
//            var applicationDto = new AppllicationDetailForStudentDTO
//            {
//                StudentId = application.StudentId,
//                JobId = application.JobId,
//                CreatedAt = createdDate,
//                UpdatedAt = updatedDate
//            };

//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId)).ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<AppllicationDetailForStudentDTO>(application)).Returns(applicationDto);

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Application detail retrieved successfully!", result.Message);
//            Assert.AreEqual(createdDate, result.Data.CreatedAt);
//            Assert.AreEqual(updatedDate, result.Data.UpdatedAt);
//        }


//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnError_WhenRepositoryThrowsUnexpectedException()
//        {
//            // Arrange
//            var applicationId = 1;
//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId))
//                                       .ThrowsAsync(new Exception("Unexpected repository error"));

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsTrue(result.Message.Contains("Unexpected repository error"));
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetApplicationDetailByIdAsync_ShouldReturnApplicationWithEmptyFiles_WhenFilesNotAvailable()
//        {
//            // Arrange
//            var applicationId = 1;
//            var application = new Appllication
//            {
//                ApplicationId = applicationId,
//                StudentId = 1,
//                JobId = 2,
//                CoverLetter = "Sample Cover Letter",
//                CvId = 3,
//                TestFile = null, // No test file
//                CvFile = null, // No CV file
//                Status = "Pending",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            };
//            var applicationDto = new AppllicationDetailForStudentDTO
//            {
//                StudentId = application.StudentId,
//                JobId = application.JobId,
//                CoverLetter = application.CoverLetter,
//                CvId = application.CvId,
//                TestFile = null,
//                CvFile = null,
//                Status = application.Status,
//                CreatedAt = application.CreatedAt,
//                UpdatedAt = application.UpdatedAt
//            };

//            _appllicationRepositoryMock.Setup(repo => repo.GetApplicationDetailByIdAsync(applicationId)).ReturnsAsync(application);
//            _mapperMock.Setup(mapper => mapper.Map<AppllicationDetailForStudentDTO>(application)).Returns(applicationDto);

//            // Act
//            var result = await _appllicationService.GetApplicationDetailByIdAsync(applicationId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Application detail retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data.TestFile);
//            Assert.IsNull(result.Data.CvFile);
//        }


//        #endregion

//    }
//}
