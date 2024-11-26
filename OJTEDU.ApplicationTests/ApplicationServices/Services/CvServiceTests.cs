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
//using static OJTEDU.Application.DTOs.CvDTO;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class CvServiceTests
//    {
//        private Mock<ICvRepository> _cvRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private CvService _cvService;

//        [SetUp]
//        public void Setup()
//        {
//            _cvRepositoryMock = new Mock<ICvRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _cvService = new CvService(_cvRepositoryMock.Object, _mapperMock.Object);
//        }

//        #region UploadCvAsync

//        [Test]
//        public async Task UploadCvAsync_ShouldReturnFilePath_WhenCvIsUploaded()
//        {
//            // Arrange
//            int studentId = 1;
//            string fileName = "cv.pdf";
//            byte[] fileData = new byte[] { 1, 2, 3 };
//            string expectedFilePath = "path/to/cv.pdf";

//            _cvRepositoryMock.Setup(repo => repo.UploadCvAsync(studentId, fileName, fileData)).ReturnsAsync(expectedFilePath);

//            // Act
//            var result = await _cvService.UploadCvAsync(studentId, fileName, fileData);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(expectedFilePath, result.Data);
//        }

//        [Test]
//        public async Task UploadCvAsync_ShouldReturnError_WhenUploadFails()
//        {
//            // Arrange
//            int studentId = 1;
//            string fileName = "cv.pdf";
//            byte[] fileData = new byte[] { 1, 2, 3 };

//            _cvRepositoryMock.Setup(repo => repo.UploadCvAsync(studentId, fileName, fileData)).ThrowsAsync(new Exception("Upload error"));

//            // Act
//            var result = await _cvService.UploadCvAsync(studentId, fileName, fileData);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while uploading cv file.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UploadCvAsync_ShouldReturnError_WhenStudentDoesNotExist()
//        {
//            // Arrange
//            int studentId = 999; // Assume this ID doesn't exist
//            string fileName = "cv.pdf";
//            byte[] fileData = new byte[] { 1, 2, 3 };

//            _cvRepositoryMock.Setup(repo => repo.UploadCvAsync(studentId, fileName, fileData))
//                             .ThrowsAsync(new KeyNotFoundException($"Not found student with id: {studentId}"));

//            // Act
//            var result = await _cvService.UploadCvAsync(studentId, fileName, fileData);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while uploading cv file.", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task UploadCvAsync_ShouldReturnError_WhenFileDataIsEmpty()
//        {
//            // Arrange
//            int studentId = 1;
//            string fileName = "cv.pdf";
//            byte[] fileData = Array.Empty<byte>(); // Empty file data

//            _cvRepositoryMock.Setup(repo => repo.UploadCvAsync(studentId, fileName, fileData))
//                             .ThrowsAsync(new ArgumentException("File data cannot be empty"));

//            // Act
//            var result = await _cvService.UploadCvAsync(studentId, fileName, fileData);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while uploading cv file.", result.Message);
//            Assert.IsNull(result.Data);
//        }





//        #endregion

//        #region SetPrimaryCvAsync

//        [Test]
//        public async Task SetPrimaryCvAsync_ShouldReturnSuccess_WhenPrimaryCvIsSet()
//        {
//            // Arrange
//            int studentId = 1;
//            int cvId = 2;

//            _cvRepositoryMock.Setup(repo => repo.SetPrimaryCvAsync(studentId, cvId)).ReturnsAsync(true);

//            // Act
//            var result = await _cvService.SetPrimaryCvAsync(studentId, cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.IsTrue(result.Data);
//        }

//        [Test]
//        public async Task SetPrimaryCvAsync_ShouldReturnError_WhenCvNotFound()
//        {
//            // Arrange
//            int studentId = 1;
//            int cvId = 999;

//            _cvRepositoryMock.Setup(repo => repo.SetPrimaryCvAsync(studentId, cvId)).ThrowsAsync(new InvalidOperationException("Not found CV for this student."));

//            // Act
//            var result = await _cvService.SetPrimaryCvAsync(studentId, cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsFalse(result.Data);
//        }

//        [Test]
//        public async Task SetPrimaryCvAsync_ShouldReturnError_WhenCvIdIsNull()
//        {
//            // Arrange
//            int studentId = 1;
//            int? cvId = null; // Null CvId

//            _cvRepositoryMock.Setup(repo => repo.SetPrimaryCvAsync(studentId, cvId))
//                             .ThrowsAsync(new ArgumentException("CvId cannot be null"));

//            // Act
//            var result = await _cvService.SetPrimaryCvAsync(studentId, cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while set primary CV. CvId cannot be null", result.Message);
//            Assert.IsFalse(result.Data);
//        }

//        [Test]
//        public async Task SetPrimaryCvAsync_ShouldReturnError_WhenStudentHasNoCvs()
//        {
//            // Arrange
//            int studentId = 1;
//            int cvId = 2;

//            _cvRepositoryMock.Setup(repo => repo.SetPrimaryCvAsync(studentId, cvId))
//                             .ThrowsAsync(new KeyNotFoundException("No CVs found for the student."));

//            // Act
//            var result = await _cvService.SetPrimaryCvAsync(studentId, cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while set primary CV. No CVs found for the student.", result.Message);
//            Assert.IsFalse(result.Data);
//        }

//        [Test]
//        public async Task SetPrimaryCvAsync_ShouldReturnError_WhenCvNotBelongToStudent()
//        {
//            // Arrange
//            int studentId = 1;
//            int cvId = 999; // Assuming 999 does not belong to student 1

//            _cvRepositoryMock.Setup(repo => repo.SetPrimaryCvAsync(studentId, cvId))
//                             .ThrowsAsync(new InvalidOperationException("CV does not belong to this student."));

//            // Act
//            var result = await _cvService.SetPrimaryCvAsync(studentId, cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while set primary CV. CV does not belong to this student.", result.Message);
//            Assert.IsFalse(result.Data);
//        }

//        [Test]
//        public async Task SetPrimaryCvAsync_ShouldReturnError_WhenStudentIdIsNull()
//        {
//            // Arrange
//            int? studentId = null; // Null studentId
//            int cvId = 1;

//            _cvRepositoryMock.Setup(repo => repo.SetPrimaryCvAsync(studentId, cvId))
//                             .ThrowsAsync(new ArgumentException("StudentId cannot be null"));

//            // Act
//            var result = await _cvService.SetPrimaryCvAsync(studentId, cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while set primary CV. StudentId cannot be null", result.Message);
//            Assert.IsFalse(result.Data);
//        }

//        [Test]
//        public async Task SetPrimaryCvAsync_ShouldReturnError_WhenDatabaseUpdateFails()
//        {
//            // Arrange
//            int studentId = 1;
//            int cvId = 3;

//            _cvRepositoryMock.Setup(repo => repo.SetPrimaryCvAsync(studentId, cvId))
//                             .ThrowsAsync(new Exception("Database update error"));

//            // Act
//            var result = await _cvService.SetPrimaryCvAsync(studentId, cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while set primary CV. Database update error", result.Message);
//            Assert.IsFalse(result.Data);
//        }

//        [Test]
//        public async Task SetPrimaryCvAsync_ShouldReturnSuccess_WhenSettingPrimaryCvTwice()
//        {
//            // Arrange
//            int studentId = 1;
//            int cvId = 2;

//            _cvRepositoryMock.SetupSequence(repo => repo.SetPrimaryCvAsync(studentId, cvId))
//                             .ReturnsAsync(true)
//                             .ReturnsAsync(true); // Simulate setting the same CV as primary twice

//            // Act
//            var firstAttempt = await _cvService.SetPrimaryCvAsync(studentId, cvId);
//            var secondAttempt = await _cvService.SetPrimaryCvAsync(studentId, cvId);

//            // Assert
//            Assert.IsNotNull(firstAttempt);
//            Assert.AreEqual(200, firstAttempt.StatusCode);
//            Assert.IsTrue(firstAttempt.Data);
//            Assert.AreEqual("Set primary CV successfully.", firstAttempt.Message);

//            Assert.IsNotNull(secondAttempt);
//            Assert.AreEqual(200, secondAttempt.StatusCode);
//            Assert.IsTrue(secondAttempt.Data);
//            Assert.AreEqual("Set primary CV successfully.", secondAttempt.Message);
//        }

//        [Test]
//        public async Task SetPrimaryCvAsync_ShouldReturnError_WhenCvAlreadyPrimary()
//        {
//            // Arrange
//            int studentId = 1;
//            int cvId = 4; // Assume CV 4 is already primary

//            _cvRepositoryMock.Setup(repo => repo.SetPrimaryCvAsync(studentId, cvId))
//                             .ThrowsAsync(new InvalidOperationException("CV is already set as primary."));

//            // Act
//            var result = await _cvService.SetPrimaryCvAsync(studentId, cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while set primary CV. CV is already set as primary.", result.Message);
//            Assert.IsFalse(result.Data);
//        }



//        #endregion

//        #region GetAllCvByStudentIdAsync

//        [Test]
//        public async Task GetAllCvByStudentIdAsync_ShouldReturnCvList_WhenCvsExist()
//        {
//            // Arrange
//            int studentId = 1;
//            var cvList = new List<Cv> { new Cv { CvId = 1, Name = "CV1" }, new Cv { CvId = 2, Name = "CV2" } };
//            var cvDtoList = new List<CvListForStudentDTO> { new CvListForStudentDTO { CvId = 1, Name = "CV1" }, new CvListForStudentDTO { CvId = 2, Name = "CV2" } };

//            _cvRepositoryMock.Setup(repo => repo.GetAllCvByStudentIdAsync(studentId)).ReturnsAsync(cvList);
//            _mapperMock.Setup(mapper => mapper.Map<List<CvListForStudentDTO>>(cvList)).Returns(cvDtoList);

//            // Act
//            var result = await _cvService.GetAllCvByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Count);
//        }

//        [Test]
//        public async Task GetAllCvByStudentIdAsync_ShouldReturnError_WhenStudentNotFound()
//        {
//            // Arrange
//            int studentId = 999;

//            _cvRepositoryMock.Setup(repo => repo.GetAllCvByStudentIdAsync(studentId)).ThrowsAsync(new KeyNotFoundException("Not found student with id"));

//            // Act
//            var result = await _cvService.GetAllCvByStudentIdAsync(studentId);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllCvByStudentIdAsync_ShouldReturnCvList_WhenStudentHasMultipleCvs()
//        {
//            // Arrange
//            int studentId = 2;
//            var cvList = new List<Cv>
//    {
//        new Cv { CvId = 1, StudentId = studentId, Name = "Resume1", Status = "0" },
//        new Cv { CvId = 2, StudentId = studentId, Name = "Resume2", Status = "1" }
//    };

//            var expectedDtoList = new List<CvListForStudentDTO>
//    {
//        new CvListForStudentDTO { CvId = 1, Name = "Resume1", Status = "non-primary" },
//        new CvListForStudentDTO { CvId = 2, Name = "Resume2", Status = "primary" }
//    };

//            _cvRepositoryMock.Setup(repo => repo.GetAllCvByStudentIdAsync(studentId))
//                             .ReturnsAsync(cvList);
//            _mapperMock.Setup(mapper => mapper.Map<List<CvListForStudentDTO>>(cvList))
//                       .Returns(expectedDtoList);

//            // Act
//            var result = await _cvService.GetAllCvByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("Resume1", result.Data[0].Name);
//            Assert.AreEqual("Resume2", result.Data[1].Name);
//            Assert.AreEqual("CV list retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task GetAllCvByStudentIdAsync_ShouldReturnOnlyNonStoredCvs_WhenStudentHasMixedStatusCvs()
//        {
//            // Arrange
//            int studentId = 3;
//            var cvList = new List<Cv>
//    {
//        new Cv { CvId = 1, StudentId = studentId, Status = "1" }, // primary CV
//        new Cv { CvId = 2, StudentId = studentId, Status = "0" }, // non-primary active CV
//        new Cv { CvId = 3, StudentId = studentId, Status = "2" }, // stored CV
//        new Cv { CvId = 4, StudentId = studentId, Status = "0" }  // another non-primary active CV
//    };

//            var expectedDtoList = new List<CvListForStudentDTO>
//    {
//        new CvListForStudentDTO { CvId = 1, Status = "primary" },
//        new CvListForStudentDTO { CvId = 2, Status = "non-primary" },
//        new CvListForStudentDTO { CvId = 4, Status = "non-primary" }
//    };

//            _cvRepositoryMock.Setup(repo => repo.GetAllCvByStudentIdAsync(studentId))
//                             .ReturnsAsync(cvList.Where(c => c.Status != "2").ToList());
//            _mapperMock.Setup(mapper => mapper.Map<List<CvListForStudentDTO>>(It.Is<List<Cv>>(list => list.All(c => c.Status != "2"))))
//                       .Returns(expectedDtoList);

//            // Act
//            var result = await _cvService.GetAllCvByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(3, result.Data.Count);
//            Assert.IsTrue(result.Data.All(cv => cv.Status != "stored"));
//            Assert.AreEqual("CV list retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task GetAllCvByStudentIdAsync_ShouldHandleExceptionGracefully_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            int studentId = 5;
//            _cvRepositoryMock.Setup(repo => repo.GetAllCvByStudentIdAsync(studentId))
//                             .ThrowsAsync(new Exception("Database connection failed"));

//            // Act
//            var result = await _cvService.GetAllCvByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving CV list: Database connection failed. ", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllCvByStudentIdAsync_ShouldReturnCorrectStatusMapping_WhenCvsHaveVariousStatuses()
//        {
//            // Arrange
//            int studentId = 6;
//            var cvList = new List<Cv>
//    {
//        new Cv { CvId = 1, StudentId = studentId, Status = "1" }, // primary CV
//        new Cv { CvId = 2, StudentId = studentId, Status = "0" }, // non-primary active CV
//        new Cv { CvId = 3, StudentId = studentId, Status = "2" }  // stored CV
//    };

//            var expectedDtoList = new List<CvListForStudentDTO>
//    {
//        new CvListForStudentDTO { CvId = 1, Status = "primary" },
//        new CvListForStudentDTO { CvId = 2, Status = "non-primary" }
//    };

//            _cvRepositoryMock.Setup(repo => repo.GetAllCvByStudentIdAsync(studentId))
//                             .ReturnsAsync(cvList.Where(c => c.Status != "2").ToList());
//            _mapperMock.Setup(mapper => mapper.Map<List<CvListForStudentDTO>>(It.IsAny<List<Cv>>()))
//                       .Returns(expectedDtoList);

//            // Act
//            var result = await _cvService.GetAllCvByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("primary", result.Data[0].Status);
//            Assert.AreEqual("non-primary", result.Data[1].Status);
//            Assert.AreEqual("CV list retrieved successfully!", result.Message);
//        }


//        #endregion

//        #region DeleteAndStoredCvAsync

//        [Test]
//        public async Task DeleteAndStoredCvAsync_ShouldReturnSuccess_WhenCvIsDeleted()
//        {
//            // Arrange
//            int cvId = 1;
//            _cvRepositoryMock.Setup(repo => repo.DeleteAndStoredCvAsync(cvId)).ReturnsAsync(true);

//            // Act
//            var result = await _cvService.DeleteAndStoredCvAsync(cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.IsTrue(result.Data);
//        }

//        [Test]
//        public async Task DeleteAndStoredCvAsync_ShouldReturnError_WhenCvNotFound()
//        {
//            // Arrange
//            int cvId = 999;

//            _cvRepositoryMock.Setup(repo => repo.DeleteAndStoredCvAsync(cvId)).ThrowsAsync(new KeyNotFoundException("Not found CV with id"));

//            // Act
//            var result = await _cvService.DeleteAndStoredCvAsync(cvId);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsFalse(result.Data);
//        }

//        [Test]
//        public async Task DeleteAndStoredCvAsync_ShouldReturnSuccess_WhenCvIsDeletedAndStored()
//        {
//            // Arrange
//            int cvId = 1;
//            _cvRepositoryMock.Setup(repo => repo.DeleteAndStoredCvAsync(cvId)).ReturnsAsync(true);

//            // Act
//            var result = await _cvService.DeleteAndStoredCvAsync(cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Delete and stored CV successfully.", result.Message);
//            Assert.IsTrue(result.Data);
//        }


//        [Test]
//        public async Task DeleteAndStoredCvAsync_ShouldHandleExceptionGracefully_WhenRepositoryThrowsUnexpectedException()
//        {
//            // Arrange
//            int cvId = 4;
//            _cvRepositoryMock.Setup(repo => repo.DeleteAndStoredCvAsync(cvId))
//                             .ThrowsAsync(new Exception("Database connection failed"));

//            // Act
//            var result = await _cvService.DeleteAndStoredCvAsync(cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while delete and stored CV. Database connection failed", result.Message);
//            Assert.IsFalse(result.Data);
//        }


//        [Test]
//        public async Task DeleteAndStoredCvAsync_ShouldReturnInternalServerError_WhenDatabaseOperationFails()
//        {
//            // Arrange
//            int cvId = 6;
//            _cvRepositoryMock.Setup(repo => repo.DeleteAndStoredCvAsync(cvId))
//                             .ThrowsAsync(new Exception("Database operation failed during delete and store."));

//            // Act
//            var result = await _cvService.DeleteAndStoredCvAsync(cvId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while delete and stored CV. Database operation failed during delete and store.", result.Message);
//            Assert.IsFalse(result.Data);
//        }





//        #endregion

//        #region GetPrimaryCvFilePathAsync

//        [Test]
//        public async Task GetPrimaryCvFilePathAsync_ShouldReturnFilePath_WhenPrimaryCvExists()
//        {
//            // Arrange
//            int studentId = 1;
//            string expectedFilePath = "path/to/primary_cv.pdf";

//            _cvRepositoryMock.Setup(repo => repo.GetPrimaryCvFilePathAsync(studentId)).ReturnsAsync(expectedFilePath);

//            // Act
//            var result = await _cvService.GetPrimaryCvFilePathAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(expectedFilePath, result.Data);
//        }

//        [Test]
//        public async Task GetPrimaryCvFilePathAsync_ShouldReturnError_WhenStudentNotFound()
//        {
//            // Arrange
//            int studentId = 999;

//            _cvRepositoryMock.Setup(repo => repo.GetPrimaryCvFilePathAsync(studentId)).ThrowsAsync(new KeyNotFoundException("Not found student with id"));

//            // Act
//            var result = await _cvService.GetPrimaryCvFilePathAsync(studentId);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task GetPrimaryCvFilePathAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            int studentId = 1;
//            _cvRepositoryMock.Setup(repo => repo.GetPrimaryCvFilePathAsync(studentId))
//                             .ThrowsAsync(new Exception("Unexpected database error."));

//            // Act
//            var result = await _cvService.GetPrimaryCvFilePathAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while retrieving cv file path. Unexpected database error.", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task GetPrimaryCvFilePathAsync_ShouldReturnDefaultCvPath_WhenStudentHasNoUploadedPrimaryCv()
//        {
//            // Arrange
//            int studentId = 1;
//            string defaultFilePath = "wwwroot/uploads/cvs/default_cv.pdf";
//            _cvRepositoryMock.Setup(repo => repo.GetPrimaryCvFilePathAsync(studentId)).ReturnsAsync(defaultFilePath);

//            // Act
//            var result = await _cvService.GetPrimaryCvFilePathAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Cv file path retrieved successfully!", result.Message);
//            Assert.AreEqual(defaultFilePath, result.Data);
//        }

//        [Test]
//        public async Task GetPrimaryCvFilePathAsync_ShouldHandleSpecialCharactersInFilePath()
//        {
//            // Arrange
//            int studentId = 4;
//            string specialFilePath = "wwwroot/uploads/cvs/student_4_primary_cv_#@!$.pdf";
//            _cvRepositoryMock.Setup(repo => repo.GetPrimaryCvFilePathAsync(studentId)).ReturnsAsync(specialFilePath);

//            // Act
//            var result = await _cvService.GetPrimaryCvFilePathAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Cv file path retrieved successfully!", result.Message);
//            Assert.AreEqual(specialFilePath, result.Data);
//        }

//        [Test]
//        public async Task GetPrimaryCvFilePathAsync_ShouldReturnNull_WhenRepositoryReturnsNullForPath()
//        {
//            // Arrange
//            int studentId = 5;
//            _cvRepositoryMock.Setup(repo => repo.GetPrimaryCvFilePathAsync(studentId)).ReturnsAsync((string)null);

//            // Act
//            var result = await _cvService.GetPrimaryCvFilePathAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Cv file path retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data);
//        }



//        #endregion
//    }
//}
