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
//using static OJTEDU.Application.DTOs.MajorDTO;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class MajorServiceTests
//    {
//        private Mock<IMajorRepository> _majorRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private MajorService _majorService;

//        [SetUp]
//        public void Setup()
//        {
//            _majorRepositoryMock = new Mock<IMajorRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _majorService = new MajorService(_majorRepositoryMock.Object, _mapperMock.Object);
//        }

//        #region GetAllMajorsAsync

//        [Test]
//        public async Task GetAllMajorsAsync_ShouldReturnMajorList_WhenMajorsExist()
//        {
//            // Arrange
//            var majors = new List<Major> { new Major { MajorId = 1, Name = "Computer Science" } };
//            var majorDtos = majors.Select(m => new MajorListForStudentDTO { MajorId = m.MajorId, Name = m.Name }).ToList();

//            _majorRepositoryMock.Setup(repo => repo.GetAllMajorsAsync()).ReturnsAsync(majors);
//            _mapperMock.Setup(mapper => mapper.Map<List<MajorListForStudentDTO>>(majors)).Returns(majorDtos);

//            // Act
//            var result = await _majorService.GetAllMajorsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Major list retrieved successfully!", result.Message);
//            Assert.AreEqual(majorDtos, result.Data);
//        }

//        [Test]
//        public async Task GetAllMajorsAsync_ShouldReturnEmptyList_WhenNoMajorsExist()
//        {
//            // Arrange
//            var emptyMajors = new List<Major>();
//            _majorRepositoryMock.Setup(repo => repo.GetAllMajorsAsync()).ReturnsAsync(emptyMajors);
//            _mapperMock.Setup(mapper => mapper.Map<List<MajorListForStudentDTO>>(emptyMajors)).Returns(new List<MajorListForStudentDTO>());

//            // Act
//            var result = await _majorService.GetAllMajorsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Major list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Count);
//        }

//        [Test]
//        public async Task GetAllMajorsAsync_ShouldReturnServerError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            _majorRepositoryMock.Setup(repo => repo.GetAllMajorsAsync()).ThrowsAsync(new Exception("Database failure"));

//            // Act
//            var result = await _majorService.GetAllMajorsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving major list Database failure. ", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllMajorsAsync_ShouldHandleNullDataResponse_WhenMapperReturnsNull()
//        {
//            // Arrange
//            var majors = new List<Major> { new Major { MajorId = 1, Name = "Business" } };
//            _majorRepositoryMock.Setup(repo => repo.GetAllMajorsAsync()).ReturnsAsync(majors);
//            _mapperMock.Setup(mapper => mapper.Map<List<MajorListForStudentDTO>>(majors)).Returns((List<MajorListForStudentDTO>)null);

//            // Act
//            var result = await _majorService.GetAllMajorsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Major list retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data);  // Should be null if the mapper returns null
//        }

//        [Test]
//        public async Task GetAllMajorsAsync_ShouldReturnEmptyData_WhenRepositoryReturnsNull()
//        {
//            // Arrange
//            _majorRepositoryMock.Setup(repo => repo.GetAllMajorsAsync()).ReturnsAsync((List<Major>)null);

//            // Act
//            var result = await _majorService.GetAllMajorsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Major list retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllMajorsAsync_ShouldReturnPartialData_WhenMapperFailsForSomeRecords()
//        {
//            // Arrange
//            var majors = new List<Major>
//            {
//                new Major { MajorId = 1, Name = "Economics" },
//                new Major { MajorId = 2, Name = "History" }
//            };
//            var mappedData = new List<MajorListForStudentDTO>
//            {
//                new MajorListForStudentDTO { MajorId = 1, Name = "Economics" }
//                // Simulating mapper issue for the second record
//            };

//            _majorRepositoryMock.Setup(repo => repo.GetAllMajorsAsync()).ReturnsAsync(majors);
//            _mapperMock.Setup(mapper => mapper.Map<List<MajorListForStudentDTO>>(majors)).Returns(mappedData);

//            // Act
//            var result = await _majorService.GetAllMajorsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Major list retrieved successfully!", result.Message);
//            Assert.AreEqual(1, result.Data.Count);  // Only partial data mapped
//        }

//        [Test]
//        public async Task GetAllMajorsAsync_ShouldReturnMultipleMajorsSuccessfully()
//        {
//            // Arrange
//            var majors = new List<Major>
//            {
//                new Major { MajorId = 1, Name = "Science" },
//                new Major { MajorId = 2, Name = "Arts" },
//                new Major { MajorId = 3, Name = "Commerce" }
//            };
//            var majorDtos = majors.Select(m => new MajorListForStudentDTO { MajorId = m.MajorId, Name = m.Name }).ToList();

//            _majorRepositoryMock.Setup(repo => repo.GetAllMajorsAsync()).ReturnsAsync(majors);
//            _mapperMock.Setup(mapper => mapper.Map<List<MajorListForStudentDTO>>(majors)).Returns(majorDtos);

//            // Act
//            var result = await _majorService.GetAllMajorsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Major list retrieved successfully!", result.Message);
//            Assert.AreEqual(majorDtos, result.Data);  // Validating all mapped data returned
//        }

//        [Test]
//        public async Task GetAllMajorsAsync_ShouldReturnDataOrderedByName_WhenRepositoryReturnsUnorderedList()
//        {
//            // Arrange
//            var unorderedMajors = new List<Major>
//            {
//                new Major { MajorId = 2, Name = "Business" },
//                new Major { MajorId = 1, Name = "Arts" }
//            };
//            var orderedMajorDtos = new List<MajorListForStudentDTO>
//            {
//                new MajorListForStudentDTO { MajorId = 1, Name = "Arts" },
//                new MajorListForStudentDTO { MajorId = 2, Name = "Business" }
//            };

//            _majorRepositoryMock.Setup(repo => repo.GetAllMajorsAsync()).ReturnsAsync(unorderedMajors);
//            _mapperMock.Setup(mapper => mapper.Map<List<MajorListForStudentDTO>>(unorderedMajors)).Returns(orderedMajorDtos);

//            // Act
//            var result = await _majorService.GetAllMajorsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Major list retrieved successfully!", result.Message);
//            Assert.AreEqual(orderedMajorDtos, result.Data);  // Checking ordered response
//        }


//        #endregion
//    }
//}
