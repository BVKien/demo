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
//using static OJTEDU.Application.DTOs.ProvinceDTO;
//using static OJTEDU.Application.DTOs.DistrictDTO;
//using static OJTEDU.Application.DTOs.WardDTO;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class ProvinceServiceTests
//    {
//        private Mock<IProvinceRepository> _provinceRepositoryMock;
//        private Mock<IDistrictRepository> _districtRepositoryMock;
//        private Mock<IWardRepository> _wardRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private ProvinceService _provinceService;

//        [SetUp]
//        public void Setup()
//        {
//            _provinceRepositoryMock = new Mock<IProvinceRepository>();
//            _districtRepositoryMock = new Mock<IDistrictRepository>();
//            _wardRepositoryMock = new Mock<IWardRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _provinceService = new ProvinceService(
//                _provinceRepositoryMock.Object,
//                _districtRepositoryMock.Object,
//                _wardRepositoryMock.Object,
//                _mapperMock.Object);
//        }

//        #region GetAllLocationsAsync Tests

//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnProvincesOnly_WhenNoProvinceIdOrDistrictIdProvided()
//        {
//            // Arrange
//            var provinces = new List<Province> { new Province { ProvinceId = 1, Name = "Province 1" } };
//            var provinceDtos = new List<ProvinceListForCommonDTO> { new ProvinceListForCommonDTO { ProvinceId = 1, Name = "Province 1" } };

//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(provinces);
//            _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(provinces)).Returns(provinceDtos);

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(null, null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Location list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data.ProvinceList);
//            Assert.AreEqual(1, result.Data.ProvinceList.Count);
//            Assert.IsNull(result.Data.DistrictList);
//            Assert.IsNull(result.Data.WardList);
//        }

//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnProvincesAndDistricts_WhenProvinceIdProvided()
//        {
//            // Arrange
//            var provinces = new List<Province> { new Province { ProvinceId = 1, Name = "Province 1" } };
//            var districts = new List<District> { new District { DistrictId = 1, Name = "District 1", ProvinceId = 1 } };
//            var provinceDtos = new List<ProvinceListForCommonDTO> { new ProvinceListForCommonDTO { ProvinceId = 1, Name = "Province 1" } };
//            var districtDtos = new List<DistrictListForCommonDTO> { new DistrictListForCommonDTO { DistrictId = 1, Name = "District 1" } };

//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(provinces);
//            _districtRepositoryMock.Setup(repo => repo.GetAllDistrictsByProvinceIdAsync(1)).ReturnsAsync(districts);
//            _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(provinces)).Returns(provinceDtos);
//            _mapperMock.Setup(mapper => mapper.Map<List<DistrictListForCommonDTO>>(districts)).Returns(districtDtos);

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(1, null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Location list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data.ProvinceList);
//            Assert.AreEqual(1, result.Data.ProvinceList.Count);
//            Assert.IsNotNull(result.Data.DistrictList);
//            Assert.AreEqual(1, result.Data.DistrictList.Count);
//            Assert.IsNull(result.Data.WardList);
//        }

//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnProvincesDistrictsAndWards_WhenProvinceAndDistrictIdProvided()
//        {
//            // Arrange
//            var provinces = new List<Province> { new Province { ProvinceId = 1, Name = "Province 1" } };
//            var districts = new List<District> { new District { DistrictId = 1, Name = "District 1", ProvinceId = 1 } };
//            var wards = new List<Ward> { new Ward { WardId = 1, Name = "Ward 1", DistrictId = 1 } };
//            var provinceDtos = new List<ProvinceListForCommonDTO> { new ProvinceListForCommonDTO { ProvinceId = 1, Name = "Province 1" } };
//            var districtDtos = new List<DistrictListForCommonDTO> { new DistrictListForCommonDTO { DistrictId = 1, Name = "District 1" } };
//            var wardDtos = new List<WardListForCommonDTO> { new WardListForCommonDTO { WardId = 1, Name = "Ward 1" } };

//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(provinces);
//            _districtRepositoryMock.Setup(repo => repo.GetAllDistrictsByProvinceIdAsync(1)).ReturnsAsync(districts);
//            _wardRepositoryMock.Setup(repo => repo.GetAllWardsByDistrictIdAsync(1)).ReturnsAsync(wards);
//            _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(provinces)).Returns(provinceDtos);
//            _mapperMock.Setup(mapper => mapper.Map<List<DistrictListForCommonDTO>>(districts)).Returns(districtDtos);
//            _mapperMock.Setup(mapper => mapper.Map<List<WardListForCommonDTO>>(wards)).Returns(wardDtos);

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(1, 1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Location list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data.ProvinceList);
//            Assert.AreEqual(1, result.Data.ProvinceList.Count);
//            Assert.IsNotNull(result.Data.DistrictList);
//            Assert.AreEqual(1, result.Data.DistrictList.Count);
//            Assert.IsNotNull(result.Data.WardList);
//            Assert.AreEqual(1, result.Data.WardList.Count);
//        }

//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnServerError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(null, null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving location list: Database error.", result.Message);
//            Assert.IsNull(result.Data);
//        }


//            [Test]
//            public async Task GetAllLocationsAsync_ShouldReturnServerError_WhenDistrictRepositoryThrowsException()
//            {
//                // Arrange
//                var provinces = new List<Province> { new Province { ProvinceId = 1, Name = "Province 1" } };
//                var provinceDtos = new List<ProvinceListForCommonDTO> { new ProvinceListForCommonDTO { ProvinceId = 1, Name = "Province 1" } };

//                _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(provinces);
//                _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(provinces)).Returns(provinceDtos);
//                _districtRepositoryMock.Setup(repo => repo.GetAllDistrictsByProvinceIdAsync(1)).ThrowsAsync(new Exception("Database error in District"));

//                // Act
//                var result = await _provinceService.GetAllLocationsAsync(1, null);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(500, result.StatusCode);
//                Assert.AreEqual("Error retrieving location list: Database error in District.", result.Message);
//                Assert.IsNull(result.Data);
//            }




//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnServerError_WhenWardRepositoryThrowsException()
//        {
//            // Arrange
//            var provinces = new List<Province> { new Province { ProvinceId = 1, Name = "Province A" } };
//            var districts = new List<District> { new District { DistrictId = 1, Name = "District A", ProvinceId = 1 } };

//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(provinces);
//            _districtRepositoryMock.Setup(repo => repo.GetAllDistrictsByProvinceIdAsync(1)).ReturnsAsync(districts);
//            _wardRepositoryMock.Setup(repo => repo.GetAllWardsByDistrictIdAsync(1)).ThrowsAsync(new Exception("Ward repository exception"));

//            _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(provinces)).Returns(new List<ProvinceListForCommonDTO>());
//            _mapperMock.Setup(mapper => mapper.Map<List<DistrictListForCommonDTO>>(districts)).Returns(new List<DistrictListForCommonDTO>());

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(1, 1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving location list: Ward repository exception.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnDistrictAndWardLists_WhenProvinceAndDistrictIdsAreValid()
//        {
//            // Arrange
//            var provinces = new List<Province> { new Province { ProvinceId = 1, Name = "Province A" } };
//            var districts = new List<District> { new District { DistrictId = 1, Name = "District A", ProvinceId = 1 } };
//            var wards = new List<Ward> { new Ward { WardId = 1, Name = "Ward A", DistrictId = 1 } };

//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(provinces);
//            _districtRepositoryMock.Setup(repo => repo.GetAllDistrictsByProvinceIdAsync(1)).ReturnsAsync(districts);
//            _wardRepositoryMock.Setup(repo => repo.GetAllWardsByDistrictIdAsync(1)).ReturnsAsync(wards);

//            _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(provinces)).Returns(new List<ProvinceListForCommonDTO> { new ProvinceListForCommonDTO { ProvinceId = 1, Name = "Province A" } });
//            _mapperMock.Setup(mapper => mapper.Map<List<DistrictListForCommonDTO>>(districts)).Returns(new List<DistrictListForCommonDTO> { new DistrictListForCommonDTO { DistrictId = 1, Name = "District A" } });
//            _mapperMock.Setup(mapper => mapper.Map<List<WardListForCommonDTO>>(wards)).Returns(new List<WardListForCommonDTO> { new WardListForCommonDTO { WardId = 1, Name = "Ward A" } });

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(1, 1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Location list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data.ProvinceList);
//            Assert.IsNotNull(result.Data.DistrictList);
//            Assert.IsNotNull(result.Data.WardList);
//            Assert.AreEqual(1, result.Data.ProvinceList.Count);
//            Assert.AreEqual(1, result.Data.DistrictList.Count);
//            Assert.AreEqual(1, result.Data.WardList.Count);
//        }

//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnEmptyLists_WhenNoDataExists()
//        {
//            // Arrange
//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(new List<Province>());
//            _districtRepositoryMock.Setup(repo => repo.GetAllDistrictsByProvinceIdAsync(It.IsAny<int>())).ReturnsAsync(new List<District>());
//            _wardRepositoryMock.Setup(repo => repo.GetAllWardsByDistrictIdAsync(It.IsAny<int>())).ReturnsAsync(new List<Ward>());

//            _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(It.IsAny<List<Province>>())).Returns(new List<ProvinceListForCommonDTO>());
//            _mapperMock.Setup(mapper => mapper.Map<List<DistrictListForCommonDTO>>(It.IsAny<List<District>>())).Returns(new List<DistrictListForCommonDTO>());
//            _mapperMock.Setup(mapper => mapper.Map<List<WardListForCommonDTO>>(It.IsAny<List<Ward>>())).Returns(new List<WardListForCommonDTO>());

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(null, null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Location list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(0, result.Data.ProvinceList.Count); // Check empty list with Count
//            Assert.IsNull(result.Data.DistrictList);
//            Assert.IsNull(result.Data.WardList);
//        }

//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnEmptyDistrictList_WhenProvinceHasNoDistricts()
//        {
//            // Arrange
//            var provinceList = new List<Province> { new Province { ProvinceId = 1, Name = "Province A" } };
//            var provinceDtos = new List<ProvinceListForCommonDTO> { new ProvinceListForCommonDTO { ProvinceId = 1, Name = "Province A" } };

//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(provinceList);
//            _districtRepositoryMock.Setup(repo => repo.GetAllDistrictsByProvinceIdAsync(1)).ReturnsAsync(new List<District>());

//            _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(provinceList)).Returns(provinceDtos);
//            _mapperMock.Setup(mapper => mapper.Map<List<DistrictListForCommonDTO>>(It.IsAny<List<District>>())).Returns(new List<DistrictListForCommonDTO>());

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(1, null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Location list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data.ProvinceList);
//            Assert.IsNotNull(result.Data.DistrictList);
//            Assert.AreEqual(0, result.Data.DistrictList.Count); // Check empty list with Count
//            Assert.IsNull(result.Data.WardList);
//        }

//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnEmptyWardList_WhenDistrictHasNoWards()
//        {
//            // Arrange
//            var provinceList = new List<Province> { new Province { ProvinceId = 1, Name = "Province A" } };
//            var districtList = new List<District> { new District { DistrictId = 1, Name = "District A", ProvinceId = 1 } };
//            var provinceDtos = new List<ProvinceListForCommonDTO> { new ProvinceListForCommonDTO { ProvinceId = 1, Name = "Province A" } };
//            var districtDtos = new List<DistrictListForCommonDTO> { new DistrictListForCommonDTO { DistrictId = 1, Name = "District A" } };

//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(provinceList);
//            _districtRepositoryMock.Setup(repo => repo.GetAllDistrictsByProvinceIdAsync(1)).ReturnsAsync(districtList);
//            _wardRepositoryMock.Setup(repo => repo.GetAllWardsByDistrictIdAsync(1)).ReturnsAsync(new List<Ward>());

//            _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(provinceList)).Returns(provinceDtos);
//            _mapperMock.Setup(mapper => mapper.Map<List<DistrictListForCommonDTO>>(districtList)).Returns(districtDtos);
//            _mapperMock.Setup(mapper => mapper.Map<List<WardListForCommonDTO>>(It.IsAny<List<Ward>>())).Returns(new List<WardListForCommonDTO>());

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(1, 1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Location list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data.ProvinceList);
//            Assert.IsNotNull(result.Data.DistrictList);
//            Assert.IsNotNull(result.Data.WardList);
//            Assert.AreEqual(0, result.Data.WardList.Count); // Check empty list with Count
//        }

//        [Test]
//        public async Task GetAllLocationsAsync_ShouldReturnEmptyLists_WhenProvinceAndDistrictIdsAreNull()
//        {
//            // Arrange
//            _provinceRepositoryMock.Setup(repo => repo.GetAllProvincesAsync()).ReturnsAsync(new List<Province>());
//            _mapperMock.Setup(mapper => mapper.Map<List<ProvinceListForCommonDTO>>(It.IsAny<List<Province>>())).Returns(new List<ProvinceListForCommonDTO>());

//            // Act
//            var result = await _provinceService.GetAllLocationsAsync(null, null);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Location list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.ProvinceList.Count); // Check empty list with Count
//            Assert.IsNull(result.Data.DistrictList);
//            Assert.IsNull(result.Data.WardList);
//        }


//        #endregion



//    }
//}
