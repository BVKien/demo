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
//using static OJTEDU.Application.DTOs.StudentDTO;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class StudentServiceTests
//    {
//        private Mock<IStudentRepository> _studentRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private StudentService _studentService;

//        [SetUp]
//        public void Setup()
//        {
//            _studentRepositoryMock = new Mock<IStudentRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _studentService = new StudentService(_studentRepositoryMock.Object, _mapperMock.Object);
//        }

//        #region GetStudentDetailByUserIdAsync Tests

//        [Test]
//        public async Task GetStudentDetailByUserIdAsync_ShouldReturnStudentDetails_WhenStudentExists()
//        {
//            // Arrange
//            var userId = 1;
//            var student = new Student { UserId = userId, AlternativeEmail = "student@example.com" };
//            var studentDto = new StudentDetailForStudentDTO { Email = "student@example.com" };

//            _studentRepositoryMock.Setup(repo => repo.GetStudentDetailByUserIdAsync(userId)).ReturnsAsync(student);
//            _mapperMock.Setup(mapper => mapper.Map<StudentDetailForStudentDTO>(student)).Returns(studentDto);

//            // Act
//            var result = await _studentService.GetStudentDetailByUserIdAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Student information retrieved successfully!", result.Message);
//            Assert.AreEqual(studentDto, result.Data);
//        }

//        [Test]
//        public async Task GetStudentDetailByUserIdAsync_ShouldReturnError_WhenStudentDoesNotExist()
//        {
//            // Arrange
//            var userId = 999;
//            _studentRepositoryMock.Setup(repo => repo.GetStudentDetailByUserIdAsync(userId)).ThrowsAsync(new KeyNotFoundException("Student not found"));

//            // Act
//            var result = await _studentService.GetStudentDetailByUserIdAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving student information Student not found. ", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task GetStudentDetailByUserIdAsync_ShouldReturnError_WhenDatabaseFails()
//        {
//            // Arrange
//            var userId = 3;
//            _studentRepositoryMock.Setup(repo => repo.GetStudentDetailByUserIdAsync(userId))
//                .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _studentService.GetStudentDetailByUserIdAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving student information Database error. ", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetStudentDetailByUserIdAsync_ShouldReturnPartialData_WhenSomeFieldsAreMissing()
//        {
//            // Arrange
//            var userId = 4;
//            var student = new Student { UserId = userId, AlternativeEmail = "student@example.com" };
//            var studentDto = new StudentDetailForStudentDTO
//            {
//                Email = "student@example.com",
//                // Intentionally leaving other fields as null to simulate partial data
//            };

//            _studentRepositoryMock.Setup(repo => repo.GetStudentDetailByUserIdAsync(userId)).ReturnsAsync(student);
//            _mapperMock.Setup(mapper => mapper.Map<StudentDetailForStudentDTO>(student)).Returns(studentDto);

//            // Act
//            var result = await _studentService.GetStudentDetailByUserIdAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Student information retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual("student@example.com", result.Data.Email);
//            Assert.IsNull(result.Data.Name); // Fields left null in DTO should be null in the result as well
//        }


//        [Test]
//        public async Task GetStudentDetailByUserIdAsync_ShouldReturnEmptyAddress_WhenAddressIsNull()
//        {
//            // Arrange
//            var userId = 10;
//            var student = new Student { UserId = userId, AlternativeEmail = "student@example.com", Address = null };
//            var studentDto = new StudentDetailForStudentDTO
//            {
//                Email = "student@example.com",
//                Address = null
//            };

//            _studentRepositoryMock.Setup(repo => repo.GetStudentDetailByUserIdAsync(userId)).ReturnsAsync(student);
//            _mapperMock.Setup(mapper => mapper.Map<StudentDetailForStudentDTO>(student)).Returns(studentDto);

//            // Act
//            var result = await _studentService.GetStudentDetailByUserIdAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Student information retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data.Address);
//        }

//        [Test]
//        public async Task GetStudentDetailByUserIdAsync_ShouldReturnGenderAsUnknown_WhenGenderIsNotSet()
//        {
//            // Arrange
//            var userId = 11;
//            var student = new Student { UserId = userId, Gender = null, AlternativeEmail = "student@example.com" };
//            var studentDto = new StudentDetailForStudentDTO
//            {
//                Email = "student@example.com",
//                Gender = "Unknown"
//            };

//            _studentRepositoryMock.Setup(repo => repo.GetStudentDetailByUserIdAsync(userId)).ReturnsAsync(student);
//            _mapperMock.Setup(mapper => mapper.Map<StudentDetailForStudentDTO>(student)).Returns(studentDto);

//            // Act
//            var result = await _studentService.GetStudentDetailByUserIdAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Student information retrieved successfully!", result.Message);
//            Assert.AreEqual("Unknown", result.Data.Gender);
//        }


//        [Test]
//        public async Task GetStudentDetailByUserIdAsync_ShouldReturnCorrectEmail_WhenStudentEmailHasMultipleFormats()
//        {
//            // Arrange
//            var userId = 14;
//            var student = new Student { UserId = userId, AlternativeEmail = "STUDENT@example.COM" };
//            var studentDto = new StudentDetailForStudentDTO
//            {
//                Email = "student@example.com"
//            };

//            _studentRepositoryMock.Setup(repo => repo.GetStudentDetailByUserIdAsync(userId)).ReturnsAsync(student);
//            _mapperMock.Setup(mapper => mapper.Map<StudentDetailForStudentDTO>(student)).Returns(studentDto);

//            // Act
//            var result = await _studentService.GetStudentDetailByUserIdAsync(userId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Student information retrieved successfully!", result.Message);
//            Assert.AreEqual("student@example.com", result.Data.Email);
//        }


//        #endregion

//        #region UpdateStudentByUserIdAsync Tests

//        [Test]
//        public async Task UpdateStudentByUserIdAsync_ShouldReturnUpdatedStudent_WhenUpdateIsSuccessful()
//        {
//            // Arrange
//            var userId = 1;
//            var updateDto = new UpdateStudentForStudentDTO { Image = "newImage.png", Phone = "123456789" };
//            var updatedStudent = new Student { UserId = userId, Phone = "123456789" };
//            var updatedStudentDto = new UpdateStudentForStudentDTO { Phone = "123456789" };

//            _studentRepositoryMock.Setup(repo => repo.UpdateStudentByUserIdAsync(userId, It.IsAny<User>(), It.IsAny<Student>(), It.IsAny<Address>()))
//                .ReturnsAsync(updatedStudent);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateStudentForStudentDTO>(updatedStudent)).Returns(updatedStudentDto);

//            // Act
//            var result = await _studentService.UpdateStudentByUserIdAsync(userId, updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Student information retrieved successfully!", result.Message);
//            Assert.AreEqual(updatedStudentDto, result.Data);
//        }


//        [Test]
//        public async Task UpdateStudentByUserIdAsync_ShouldReturnServerError_WhenUnexpectedExceptionOccurs()
//        {
//            // Arrange
//            int userId = 10;
//            var updateInfo = new UpdateStudentForStudentDTO
//            {
//                Image = "newImage.png",
//                AlternativeEmail = "new.email@example.com",
//                Phone = "123456789",
//                Dob = DateTime.Now.AddYears(-20),
//                Gender = true,
//                Detail = "New address detail",
//                WardId = 1,
//                DistrictId = 2,
//                ProvinceId = 3
//            };

//            _studentRepositoryMock
//                .Setup(repo => repo.UpdateStudentByUserIdAsync(userId, It.IsAny<User>(), It.IsAny<Student>(), It.IsAny<Address>()))
//                .ThrowsAsync(new Exception("Unexpected server error"));

//            // Act
//            var result = await _studentService.UpdateStudentByUserIdAsync(userId, updateInfo);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("An error occurred while updating student information for user id 10: Unexpected server error.", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task UpdateStudentByUserIdAsync_ShouldReturnSuccess_WhenAddressOnlyIsUpdated()
//        {
//            // Arrange
//            int userId = 15;
//            var updateInfo = new UpdateStudentForStudentDTO
//            {
//                Detail = "New Address Detail",
//                WardId = 10,
//                DistrictId = 20,
//                ProvinceId = 30
//            };

//            var updatedStudent = new Student
//            {
//                UserId = userId
//            };

//            var updatedStudentDto = new UpdateStudentForStudentDTO
//            {
//                Detail = updateInfo.Detail,
//                WardId = updateInfo.WardId,
//                DistrictId = updateInfo.DistrictId,
//                ProvinceId = updateInfo.ProvinceId
//            };

//            _studentRepositoryMock
//                .Setup(repo => repo.UpdateStudentByUserIdAsync(userId, It.IsAny<User>(), It.IsAny<Student>(), It.IsAny<Address>()))
//                .ReturnsAsync(updatedStudent);
//            _mapperMock
//                .Setup(mapper => mapper.Map<UpdateStudentForStudentDTO>(updatedStudent))
//                .Returns(updatedStudentDto);

//            // Act
//            var result = await _studentService.UpdateStudentByUserIdAsync(userId, updateInfo);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Student information retrieved successfully!", result.Message);
//            Assert.AreEqual(updatedStudentDto.Detail, result.Data.Detail);
//            Assert.AreEqual(updatedStudentDto.WardId, result.Data.WardId);
//            Assert.AreEqual(updatedStudentDto.DistrictId, result.Data.DistrictId);
//            Assert.AreEqual(updatedStudentDto.ProvinceId, result.Data.ProvinceId);
//        }




//        #endregion
//    }
//}
