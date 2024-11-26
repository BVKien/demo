//using AutoMapper;
//using Microsoft.Extensions.Configuration;
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
//using static OJTEDU.Application.DTOs.NewsFaqDTO;
//using System.Drawing;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class NewsFaqServiceTests
//    {
//        private Mock<INewsFaqRepository> _newsFaqRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private NewsFaqService _newsFaqService;

//        [SetUp]
//        public void Setup()
//        {
//            _newsFaqRepositoryMock = new Mock<INewsFaqRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _newsFaqService = new NewsFaqService(_newsFaqRepositoryMock.Object, _mapperMock.Object);
//        }

//        #region Admin

//        #region Admin - Parent News Management

//        #region GetAllParentNewsForAdminAsync Tests

//        [Test]
//        public async Task GetAllParentNewsForAdminAsync_ShouldReturnPagedParentNewsList_WhenParentNewsExist()
//        {
//            // Arrange
//            var parentNewsList = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "Sample News", Status = "Active" }
//        };
//            var parentNewsDtos = parentNewsList.Select(news => new ParentNewsListForAdminDTO
//            {
//                ParentNewsId = news.NewsFaqid,
//                Title = news.Title
//            }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(parentNewsList);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentNewsListForAdminDTO>>(parentNewsList)).Returns(parentNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForAdminAsync("Sample", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(1, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllParentNewsForAdminAsync_ShouldReturnEmptyPagedList_WhenNoMatchingParentNews()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(new List<NewsFaq>());

//            _mapperMock.Setup(mapper => mapper.Map<List<ParentNewsListForAdminDTO>>(It.IsAny<List<NewsFaq>>())).Returns(new List<ParentNewsListForAdminDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForAdminAsync("NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllParentNewsForAdminAsync_ShouldReturnUnauthorized_WhenUserNotAuthorized()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForAdminAsync("AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get parent news list: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllParentNewsForAdminAsync_ShouldReturnNotFound_WhenNoParentNewsExist()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new KeyNotFoundException("Parent news not found."));

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForAdminAsync("NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent news not found.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllParentNewsForAdminAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new Exception("Server error occurred"));

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForAdminAsync("AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving parent news list: Server error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllParentNewsForAdminAsync_ShouldReturnFirstPage_WhenPageNumberExceedsTotalPages()
//        {
//            // Arrange
//            var parentNews = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "News1", Status = "Active" },
//            new NewsFaq { NewsFaqid = 2, Title = "News2", Status = "Inactive" }
//        };
//            var parentNewsDtos = parentNews.Select(news => new ParentNewsListForAdminDTO { ParentNewsId = news.NewsFaqid, Title = news.Title }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(parentNews);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentNewsListForAdminDTO>>(parentNews)).Returns(parentNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForAdminAsync(null, null, null, 10, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllParentNewsForAdminAsync_ShouldReturnRequestedPage_WhenMultiplePagesAvailable()
//        {
//            // Arrange
//            var parentNews = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "News1", Status = "Active" },
//            new NewsFaq { NewsFaqid = 2, Title = "News2", Status = "Inactive" },
//            new NewsFaq { NewsFaqid = 3, Title = "News3", Status = "Active" }
//        };
//            var parentNewsDtos = parentNews.Select(news => new ParentNewsListForAdminDTO { ParentNewsId = news.NewsFaqid, Title = news.Title }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(parentNews);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentNewsListForAdminDTO>>(parentNews)).Returns(parentNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForAdminAsync(null, null, null, 1, 2);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(3, result.Data.TotalCount);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual(2, result.Data.TotalPages);
//        }

//        [Test]
//        public async Task GetAllParentNewsForAdminAsync_ShouldReturnFilteredList_WhenTitleFilterIsApplied()
//        {
//            // Arrange
//            var parentNews = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "Admin Guide" },
//            new NewsFaq { NewsFaqid = 2, Title = "Admin Manual" }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForAdminAsync("Admin", null, null))
//                .ReturnsAsync(parentNews);

//            var parentNewsDtos = parentNews.Select(news => new ParentNewsListForAdminDTO { ParentNewsId = news.NewsFaqid, Title = news.Title }).ToList();
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentNewsListForAdminDTO>>(parentNews)).Returns(parentNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForAdminAsync("Admin", null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual("Admin Guide", result.Data.Items[0].Title);
//            Assert.AreEqual("Admin Manual", result.Data.Items[1].Title);
//        }

//        #endregion

//        #region GetParentNewsDetailByIdForAdminAsync

//        [Test]
//        public async Task GetParentNewsDetailByIdForAdminAsync_ShouldReturnNewsDetail_WhenNewsExists()
//        {
//            var newsId = 1;
//            var news = new NewsFaq { NewsFaqid = newsId, Title = "Sample News" };
//            var newsDto = new ParentNewsDetailForAdminDTO { Title = news.Title };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForAdminAsync(newsId)).ReturnsAsync(news);
//            _mapperMock.Setup(mapper => mapper.Map<ParentNewsDetailForAdminDTO>(news)).Returns(newsDto);

//            var result = await _newsFaqService.GetParentNewsDetailByIdForAdminAsync(newsId);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(newsDto, result.Data);
//            _newsFaqRepositoryMock.Verify(repo => repo.GetParentNewsByIdForAdminAsync(newsId), Times.Once);
//            _mapperMock.Verify(mapper => mapper.Map<ParentNewsDetailForAdminDTO>(news), Times.Once);
//        }

//        [Test]
//        public async Task GetParentNewsDetailByIdForAdminAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//        {
//            var newsId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForAdminAsync(newsId)).ThrowsAsync(new KeyNotFoundException("News not found"));
//            var result = await _newsFaqService.GetParentNewsDetailByIdForAdminAsync(newsId);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("News not found", result.Message);
//        }

//        [Test]
//        public async Task GetParentNewsDetailByIdForAdminAsync_ShouldReturnParentNewsDetail_WhenNewsExists()
//        {
//            // Arrange
//            var parentNews = new NewsFaq
//            {
//                NewsFaqid = 1,
//                Title = "Admin News",
//                NewsFaqcontent = "This is admin-specific news.",
//                Status = "Active"
//            };
//            var parentNewsDto = new ParentNewsDetailForAdminDTO
//            {
//                ParentNewsId = 1,
//                Title = "Admin News",
//                ParentNewscontent = "This is admin-specific news.",
//                Status = "Active"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForAdminAsync(parentNews.NewsFaqid))
//                                  .ReturnsAsync(parentNews);
//            _mapperMock.Setup(mapper => mapper.Map<ParentNewsDetailForAdminDTO>(parentNews))
//                       .Returns(parentNewsDto);

//            // Act
//            var result = await _newsFaqService.GetParentNewsDetailByIdForAdminAsync(parentNews.NewsFaqid);

//            // Assert
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news details retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(parentNews.NewsFaqid, result.Data.ParentNewsId);
//            Assert.AreEqual("Admin News", result.Data.Title);
//            Assert.AreEqual("This is admin-specific news.", result.Data.ParentNewscontent);
//        }

//        [Test]
//        public async Task GetParentNewsDetailByIdForAdminAsync_ShouldReturn404_WhenNewsNotFound()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForAdminAsync(It.IsAny<int>()))
//                                  .ThrowsAsync(new KeyNotFoundException("Parent news not found"));

//            // Act
//            var result = await _newsFaqService.GetParentNewsDetailByIdForAdminAsync(999); // Use a non-existent ID

//            // Assert
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent news not found", result.Message);
//            Assert.IsNull(result.Data);
//        }



//        #endregion

//        #region AddParentNewsForAdminAsync

//        [Test]
//        public async Task AddParentNewsForAdminAsync_ShouldReturnSuccess_WhenNewsIsAdded()
//        {
//            var addParentNewsDto = new AddParentNewsForAdminDTO { Title = "New News", ParentNewscontent = "Content of the news", UserId = 1 };
//            var news = new NewsFaq { NewsFaqid = 1, Title = addParentNewsDto.Title, NewsFaqcontent = addParentNewsDto.ParentNewscontent, UserId = addParentNewsDto.UserId };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentNewsForAdminAsync(It.IsAny<NewsFaq>())).ReturnsAsync(news);

//            var result = await _newsFaqService.AddParentNewsForAdminAsync(addParentNewsDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Parent News added successfully!", result.Message);
//            Assert.AreEqual(addParentNewsDto.Title, result.Data.Title);
//            _newsFaqRepositoryMock.Verify(repo => repo.AddParentNewsForAdminAsync(It.IsAny<NewsFaq>()), Times.Once);
//        }

//        [Test]
//        public async Task AddParentNewsForAdminAsync_ShouldReturnForbidden_WhenAccessIsDenied()
//        {
//            var addParentNewsDto = new AddParentNewsForAdminDTO { Title = "New News", ParentNewscontent = "Content of the news", UserId = 1 };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentNewsForAdminAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            var result = await _newsFaqService.AddParentNewsForAdminAsync(addParentNewsDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add parent news: Access denied", result.Message);
//        }




//        #endregion

//        #region UpdateParentNewsForAdminAsync

//        [Test]
//        public async Task UpdateParentNewsForAdminAsync_ShouldReturnSuccess_WhenNewsIsUpdated()
//        {
//            var updateNewsDto = new UpdateParentNewsForAdminDTO { ParentNewsId = 1, Title = "Updated Title" };
//            var news = new NewsFaq { NewsFaqid = updateNewsDto.ParentNewsId, Title = updateNewsDto.Title };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsForAdminAsync(It.IsAny<NewsFaq>())).ReturnsAsync(news);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateParentNewsForAdminDTO>(news)).Returns(updateNewsDto);

//            var result = await _newsFaqService.UpdateParentNewsForAdminAsync(updateNewsDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent News updated successfully!", result.Message);
//            Assert.AreEqual(updateNewsDto.Title, result.Data.Title);
//            _newsFaqRepositoryMock.Verify(repo => repo.UpdateParentNewsForAdminAsync(It.IsAny<NewsFaq>()), Times.Once);
//        }

//        [Test]
//        public async Task UpdateParentNewsForAdminAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//        {
//            var updateNewsDto = new UpdateParentNewsForAdminDTO { ParentNewsId = 1, Title = "Updated Title" };
//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsForAdminAsync(It.IsAny<NewsFaq>())).ThrowsAsync(new KeyNotFoundException("News not found"));

//            var result = await _newsFaqService.UpdateParentNewsForAdminAsync(updateNewsDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("News not found", result.Message);
//        }


//        [Test]
//        public async Task UpdateParentNewsForAdminAsync_ShouldReturn404_WhenNewsNotFound()
//        {
//            // Arrange
//            var updateNewsDto = new UpdateParentNewsForAdminDTO
//            {
//                ParentNewsId = 99,
//                Title = "Updated Title",
//                ParentNewscontent = "Updated content"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsForAdminAsync(It.IsAny<NewsFaq>())).Throws(new KeyNotFoundException("Parent News not found"));

//            // Act
//            var result = await _newsFaqService.UpdateParentNewsForAdminAsync(updateNewsDto);

//            // Assert
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent News not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateParentNewsForAdminAsync_ShouldReturn500_WhenExceptionThrown()
//        {
//            // Arrange
//            var updateNewsDto = new UpdateParentNewsForAdminDTO
//            {
//                ParentNewsId = 1,
//                Title = "Updated Title",
//                ParentNewscontent = "Updated content"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsForAdminAsync(It.IsAny<NewsFaq>()))
//                                  .Throws(new Exception("Database error"));

//            // Act
//            var result = await _newsFaqService.UpdateParentNewsForAdminAsync(updateNewsDto);

//            // Assert
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating parent news: Database error", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        #endregion

//        #region DeleteParentNewsForAdminAsync

//        [Test]
//        public async Task DeleteParentNewsForAdminAsync_ShouldReturnSuccess_WhenNewsIsDeleted()
//        {
//            var deleteDto = new DeleteParentNewsForAdminDTO { ParentNewsId = 1 };
//            var news = new NewsFaq { NewsFaqid = deleteDto.ParentNewsId };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentNewsForAdminAsync(deleteDto.ParentNewsId)).ReturnsAsync(news);
//            _mapperMock.Setup(mapper => mapper.Map<DeleteParentNewsForAdminDTO>(news)).Returns(deleteDto);

//            var result = await _newsFaqService.DeleteParentNewsForAdminAsync(deleteDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent News has been permanently deleted successfully.", result.Message);
//            _newsFaqRepositoryMock.Verify(repo => repo.DeleteParentNewsForAdminAsync(deleteDto.ParentNewsId), Times.Once);
//        }

//        [Test]
//        public async Task DeleteParentNewsForAdminAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//        {
//            var deleteDto = new DeleteParentNewsForAdminDTO { ParentNewsId = 1 };
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentNewsForAdminAsync(deleteDto.ParentNewsId)).ThrowsAsync(new KeyNotFoundException("News not found"));

//            var result = await _newsFaqService.DeleteParentNewsForAdminAsync(deleteDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("News not found", result.Message);
//        }


//        #endregion

//        #region UpdateParentNewsStatusForAdminAsync

//        [Test]
//        public async Task UpdateParentNewsStatusForAdminAsync_ShouldReturnSuccess_WhenStatusIsUpdated()
//        {
//            var updateStatusDto = new UpdateParentNewsStatusForAdminDTO { ParentNewsId = 1, Status = "Active" };
//            var news = new NewsFaq { NewsFaqid = updateStatusDto.ParentNewsId, Status = updateStatusDto.Status };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsStatusForAdminAsync(It.IsAny<NewsFaq>())).ReturnsAsync(news);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateParentNewsStatusForAdminDTO>(news)).Returns(updateStatusDto);

//            var result = await _newsFaqService.UpdateParentNewsStatusForAdminAsync(updateStatusDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent News updated successfully!", result.Message);
//        }

//        [Test]
//        public async Task UpdateParentNewsStatusForAdminAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//        {
//            var updateStatusDto = new UpdateParentNewsStatusForAdminDTO { ParentNewsId = 1, Status = "Active" };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsStatusForAdminAsync(It.IsAny<NewsFaq>())).ThrowsAsync(new KeyNotFoundException("News not found"));

//            var result = await _newsFaqService.UpdateParentNewsStatusForAdminAsync(updateStatusDto);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("News not found", result.Message);
//        }

//        #endregion

//        #region GetAllStatusesNewsForAdminAsync

//        [Test]
//        public async Task GetAllStatusesNewsForAdminAsync_ShouldReturnSuccess_WithStatusList()
//        {
//            var result = await _newsFaqService.GetAllStatusesNewsForAdminAsync();

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Status List retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("Active", result.Data[0].Status);
//            Assert.AreEqual("Inactive", result.Data[1].Status);
//        }



//        #endregion



//        #endregion


//        #region Admin - Child News Management


//        #region GetAllChildNewsForAdminAsync Tests

//        [Test]
//        public async Task GetAllChildNewsForAdminAsync_ShouldReturnPagedChildNewsList_WhenChildNewsExist()
//        {
//            // Arrange
//            int parentId = 1;
//            var childNewsList = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "Sample Child News", ParentId = parentId, Status = "Active" }
//        };
//            var childNewsDtos = childNewsList.Select(news => new ChildNewsListForAdminDTO
//            {
//                ChildNewsId = news.NewsFaqid,
//                Title = news.Title
//            }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForAdminAsync(parentId, It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(childNewsList);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildNewsListForAdminDTO>>(childNewsList)).Returns(childNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForAdminAsync(parentId, "Sample", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(1, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllChildNewsForAdminAsync_ShouldReturnEmptyPagedList_WhenNoMatchingChildNews()
//        {
//            // Arrange
//            int parentId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForAdminAsync(parentId, It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(new List<NewsFaq>());

//            _mapperMock.Setup(mapper => mapper.Map<List<ChildNewsListForAdminDTO>>(It.IsAny<List<NewsFaq>>())).Returns(new List<ChildNewsListForAdminDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForAdminAsync(parentId, "NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllChildNewsForAdminAsync_ShouldReturnUnauthorized_WhenUserNotAuthorized()
//        {
//            // Arrange
//            int parentId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForAdminAsync(parentId, It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForAdminAsync(parentId, "AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get child news list: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllChildNewsForAdminAsync_ShouldReturnNotFound_WhenNoChildNewsExist()
//        {
//            // Arrange
//            int parentId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForAdminAsync(parentId, It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new KeyNotFoundException("Child News not found"));

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForAdminAsync(parentId, "NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child News not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllChildNewsForAdminAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            int parentId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForAdminAsync(parentId, It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new Exception("Server error occurred"));

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForAdminAsync(parentId, "AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving child news list: Server error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllChildNewsForAdminAsync_ShouldReturnFirstPage_WhenPageNumberExceedsTotalPages()
//        {
//            // Arrange
//            int parentId = 1;
//            var childNews = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "ChildNews1", Status = "Active" },
//            new NewsFaq { NewsFaqid = 2, Title = "ChildNews2", Status = "Inactive" }
//        };
//            var childNewsDtos = childNews.Select(news => new ChildNewsListForAdminDTO { ChildNewsId = news.NewsFaqid, Title = news.Title }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForAdminAsync(parentId, It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(childNews);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildNewsListForAdminDTO>>(childNews)).Returns(childNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForAdminAsync(parentId, null, null, null, 10, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllChildNewsForAdminAsync_ShouldReturnRequestedPage_WhenMultiplePagesAvailable()
//        {
//            // Arrange
//            int parentId = 1;
//            var childNews = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "ChildNews1", Status = "Active" },
//            new NewsFaq { NewsFaqid = 2, Title = "ChildNews2", Status = "Inactive" },
//            new NewsFaq { NewsFaqid = 3, Title = "ChildNews3", Status = "Active" }
//        };
//            var childNewsDtos = childNews.Select(news => new ChildNewsListForAdminDTO { ChildNewsId = news.NewsFaqid, Title = news.Title }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForAdminAsync(parentId, It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(childNews);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildNewsListForAdminDTO>>(childNews)).Returns(childNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForAdminAsync(parentId, null, null, null, 1, 2);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(3, result.Data.TotalCount);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual(2, result.Data.TotalPages);
//        }

//        [Test]
//        public async Task GetAllChildNewsForAdminAsync_ShouldReturnFilteredList_WhenTitleFilterIsApplied()
//        {
//            // Arrange
//            int parentId = 1;
//            var childNews = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "Admin Guide" },
//            new NewsFaq { NewsFaqid = 2, Title = "Admin Manual" }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForAdminAsync(parentId, "Admin", null, null))
//                .ReturnsAsync(childNews);

//            var childNewsDtos = childNews.Select(news => new ChildNewsListForAdminDTO { ChildNewsId = news.NewsFaqid, Title = news.Title }).ToList();
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildNewsListForAdminDTO>>(childNews)).Returns(childNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForAdminAsync(parentId, "Admin", null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual("Admin Guide", result.Data.Items[0].Title);
//            Assert.AreEqual("Admin Manual", result.Data.Items[1].Title);
//        }

//        #endregion


//        #region GetChildNewsDetailByIdForAdminAsync

//        [Test]
//            public async Task GetChildNewsDetailByIdForAdminAsync_ShouldReturnNewsDetail_WhenChildNewsExists()
//            {
//                // Arrange
//                var newsId = 1;
//                var news = new NewsFaq { NewsFaqid = newsId, Title = "Sample Child News" };
//                var newsDto = new ChildNewsDetailForAdminDTO { Title = news.Title };

//                _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForAdminAsync(newsId)).ReturnsAsync(news);
//                _mapperMock.Setup(mapper => mapper.Map<ChildNewsDetailForAdminDTO>(news)).Returns(newsDto);

//                // Act
//                var result = await _newsFaqService.GetChildNewsDetailByIdForAdminAsync(newsId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(200, result.StatusCode);
//                Assert.AreEqual(newsDto, result.Data);
//            }

//            [Test]
//            public async Task GetChildNewsDetailByIdForAdminAsync_ShouldReturnNotFound_WhenChildNewsDoesNotExist()
//            {
//                // Arrange
//                var newsId = 1;
//                _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForAdminAsync(newsId))
//                    .ThrowsAsync(new KeyNotFoundException("Child News not found"));

//                // Act
//                var result = await _newsFaqService.GetChildNewsDetailByIdForAdminAsync(newsId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Child News not found", result.Message);
//            }

//        [Test]
//        public async Task GetChildNewsDetailByIdForAdminAsync_ShouldReturnSuccess_WhenChildNewsExists()
//        {
//            // Arrange
//            int newsId = 1;
//            var childNews = new NewsFaq { NewsFaqid = newsId, Title = "Child News 1", Status = "Active" };
//            var childNewsDto = new ChildNewsDetailForAdminDTO { ChildNewsId = newsId, Title = "Child News 1", Status = "Active" };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForAdminAsync(newsId))
//                                  .ReturnsAsync(childNews);
//            _mapperMock.Setup(mapper => mapper.Map<ChildNewsDetailForAdminDTO>(childNews))
//                       .Returns(childNewsDto);

//            // Act
//            var result = await _newsFaqService.GetChildNewsDetailByIdForAdminAsync(newsId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child news details retrieved successfully!", result.Message);
//            Assert.AreEqual(childNewsDto, result.Data);
//            _newsFaqRepositoryMock.Verify(repo => repo.GetChildNewsByIdForAdminAsync(newsId), Times.Once);
//        }



//        [Test]
//        public async Task GetChildNewsDetailByIdForAdminAsync_ShouldReturnError_WhenExceptionOccurs()
//        {
//            // Arrange
//            int newsId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForAdminAsync(newsId))
//                                  .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _newsFaqService.GetChildNewsDetailByIdForAdminAsync(newsId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving child news details: Database error", result.Message);
//            Assert.IsNull(result.Data);
//        }



//        #endregion

//        #region AddChildNewsForAdminAsync Tests

//        [Test]
//        public async Task AddChildNewsForAdminAsync_ShouldAddChildNewsSuccessfully()
//        {
//            // Arrange
//            var childNewsDto = new AddChildNewsForAdminDTO
//            {
//                UserId = 1,
//                ParentId = 2,
//                Title = "Sample Child News",
//                ChildNewscontent = "Content for child news",
//                Image = "image.png"
//            };

//            var childNewsEntity = new NewsFaq
//            {
//                NewsFaqid = 10,
//                UserId = childNewsDto.UserId,
//                ParentId = childNewsDto.ParentId,
//                Title = childNewsDto.Title,
//                NewsFaqcontent = childNewsDto.ChildNewscontent,
//                Image = childNewsDto.Image,
//                CreatedAt = DateTime.Now,
//                Status = "Active",
//                RoleId = 3
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildNewsForAdminAsync(It.IsAny<NewsFaq>()))
//                .ReturnsAsync(childNewsEntity);

//            _mapperMock.Setup(mapper => mapper.Map<NewsFaq>(childNewsDto)).Returns(childNewsEntity);
//            _mapperMock.Setup(mapper => mapper.Map<AddChildNewsForAdminDTO>(childNewsEntity)).Returns(childNewsDto);

//            // Act
//            var result = await _newsFaqService.AddChildNewsForAdminAsync(childNewsDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Child News added successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(childNewsDto.Title, result.Data.Title);
//        }

//        [Test]
//        public async Task AddChildNewsForAdminAsync_ShouldReturnUnauthorized_WhenUserNotAuthorized()
//        {
//            // Arrange
//            var childNewsDto = new AddChildNewsForAdminDTO
//            {
//                UserId = 1,
//                ParentId = 2,
//                Title = "Unauthorized News"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildNewsForAdminAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.AddChildNewsForAdminAsync(childNewsDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add child news: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task AddChildNewsForAdminAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            var childNewsDto = new AddChildNewsForAdminDTO
//            {
//                UserId = 1,
//                ParentId = 2,
//                Title = "Sample News"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildNewsForAdminAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new Exception("Server error occurred"));

//            // Act
//            var result = await _newsFaqService.AddChildNewsForAdminAsync(childNewsDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding child news: Server error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task AddChildNewsForAdminAsync_ShouldAddChildNewsWithDefaultStatus()
//        {
//            // Arrange
//            var childNewsDto = new AddChildNewsForAdminDTO
//            {
//                UserId = 1,
//                ParentId = 2,
//                Title = "Sample Child News with Default Status",
//                ChildNewscontent = "Some content"
//            };

//            var childNewsEntity = new NewsFaq
//            {
//                NewsFaqid = 10,
//                UserId = childNewsDto.UserId,
//                ParentId = childNewsDto.ParentId,
//                Title = childNewsDto.Title,
//                NewsFaqcontent = childNewsDto.ChildNewscontent,
//                CreatedAt = DateTime.Now,
//                Status = "Active" // Default status is Active
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildNewsForAdminAsync(It.IsAny<NewsFaq>()))
//                .ReturnsAsync(childNewsEntity);

//            _mapperMock.Setup(mapper => mapper.Map<NewsFaq>(childNewsDto)).Returns(childNewsEntity);
//            _mapperMock.Setup(mapper => mapper.Map<AddChildNewsForAdminDTO>(childNewsEntity)).Returns(childNewsDto);

//            // Act
//            var result = await _newsFaqService.AddChildNewsForAdminAsync(childNewsDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Child News added successfully!", result.Message);
//            Assert.AreEqual("Active", result.Data.Status);
//        }

//        #endregion

//            #region UpdateChildNewsForAdminAsync

//        [Test]
//            public async Task UpdateChildNewsForAdminAsync_ShouldReturnSuccess_WhenChildNewsIsUpdated()
//            {
//                // Arrange
//                var updateNewsDto = new UpdateChildNewsForAdminDTO { ChildNewsId = 1, Title = "Updated Title" };
//                var news = new NewsFaq { NewsFaqid = updateNewsDto.ChildNewsId, Title = updateNewsDto.Title };

//                _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildNewsForAdminAsync(It.IsAny<NewsFaq>())).ReturnsAsync(news);
//                _mapperMock.Setup(mapper => mapper.Map<UpdateChildNewsForAdminDTO>(news)).Returns(updateNewsDto);

//                // Act
//                var result = await _newsFaqService.UpdateChildNewsForAdminAsync(updateNewsDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(200, result.StatusCode);
//                Assert.AreEqual("Child News updated successfully!", result.Message);
//                Assert.AreEqual(updateNewsDto.Title, result.Data.Title);
//            }

//            [Test]
//            public async Task UpdateChildNewsForAdminAsync_ShouldReturnNotFound_WhenChildNewsDoesNotExist()
//            {
//                // Arrange
//                var updateNewsDto = new UpdateChildNewsForAdminDTO { ChildNewsId = 1, Title = "Updated Title" };

//                _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildNewsForAdminAsync(It.IsAny<NewsFaq>()))
//                    .ThrowsAsync(new KeyNotFoundException("Child News not found"));

//                // Act
//                var result = await _newsFaqService.UpdateChildNewsForAdminAsync(updateNewsDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Child News not found", result.Message);
//            }




//        #endregion

//            #region DeleteChildNewsForAdminAsync

//            [Test]
//            public async Task DeleteChildNewsForAdminAsync_ShouldReturnSuccess_WhenChildNewsIsDeleted()
//            {
//                // Arrange
//                var deleteDto = new DeleteChildNewsForAdminDTO { ChildNewsId = 1 };
//                var news = new NewsFaq { NewsFaqid = deleteDto.ChildNewsId };

//                _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildNewsForAdminAsync(deleteDto.ChildNewsId)).ReturnsAsync(news);
//                _mapperMock.Setup(mapper => mapper.Map<DeleteChildNewsForAdminDTO>(news)).Returns(deleteDto);

//                // Act
//                var result = await _newsFaqService.DeleteChildNewsForAdminAsync(deleteDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(200, result.StatusCode);
//                Assert.AreEqual("Child News has been permanently deleted successfully.", result.Message);
//            }

//            [Test]
//            public async Task DeleteChildNewsForAdminAsync_ShouldReturnNotFound_WhenChildNewsDoesNotExist()
//            {
//                // Arrange
//                var deleteDto = new DeleteChildNewsForAdminDTO { ChildNewsId = 1 };

//                _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildNewsForAdminAsync(deleteDto.ChildNewsId))
//                    .ThrowsAsync(new KeyNotFoundException("Child News not found"));

//                // Act
//                var result = await _newsFaqService.DeleteChildNewsForAdminAsync(deleteDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Child News not found", result.Message);
//            }

//        [Test]
//        public async Task DeleteChildNewsForAdminAsync_ShouldReturnSuccess_WhenNewsIsDeletedSuccessfully()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildNewsForAdminDTO { ChildNewsId = 1 };
//            var news = new NewsFaq { NewsFaqid = deleteDto.ChildNewsId };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildNewsForAdminAsync(deleteDto.ChildNewsId)).ReturnsAsync(news);
//            _mapperMock.Setup(mapper => mapper.Map<DeleteChildNewsForAdminDTO>(news)).Returns(deleteDto);

//            // Act
//            var result = await _newsFaqService.DeleteChildNewsForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child News has been permanently deleted successfully.", result.Message);
//            _newsFaqRepositoryMock.Verify(repo => repo.DeleteChildNewsForAdminAsync(deleteDto.ChildNewsId), Times.Once);
//        }

//        [Test]
//        public async Task DeleteChildNewsForAdminAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildNewsForAdminDTO { ChildNewsId = 1 };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildNewsForAdminAsync(deleteDto.ChildNewsId))
//                                  .ThrowsAsync(new KeyNotFoundException("Child News not found"));

//            // Act
//            var result = await _newsFaqService.DeleteChildNewsForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child News not found", result.Message);
//            Assert.IsNull(result.Data);
//            _newsFaqRepositoryMock.Verify(repo => repo.DeleteChildNewsForAdminAsync(deleteDto.ChildNewsId), Times.Once);
//        }



//        #endregion




//        #endregion


//        #region Admin - Parent Faq Management

//        #region GetAllParentFaqForAdminAsync Tests

//        [Test]
//        public async Task GetAllParentFaqForAdminAsync_ShouldReturnPagedParentFaqList_WhenParentFaqExist()
//        {
//            // Arrange
//            var parentFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "Sample FAQ", Status = "Active" }
//        };
//            var parentFaqDtos = parentFaqs.Select(faq => new ParentFaqListForAdminDTO
//            {
//                ParentFaqId = faq.NewsFaqid,
//                Title = faq.Title
//            }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(parentFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForAdminDTO>>(parentFaqs)).Returns(parentFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForAdminAsync("Sample", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(1, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllParentFaqForAdminAsync_ShouldReturnEmptyList_WhenNoMatchingParentFaq()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(new List<NewsFaq>());
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForAdminDTO>>(It.IsAny<List<NewsFaq>>())).Returns(new List<ParentFaqListForAdminDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForAdminAsync("NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count); // Replaces Assert.IsEmpty
//        }

//        [Test]
//        public async Task GetAllParentFaqForAdminAsync_ShouldReturnUnauthorized_WhenUserNotAuthorized()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForAdminAsync("AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get parent faq list: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllParentFaqForAdminAsync_ShouldReturnNotFound_WhenNoParentFaqExists()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new KeyNotFoundException("Parent Faq not found"));

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForAdminAsync("NonExistent", null, "Inactive", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent Faq not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllParentFaqForAdminAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ThrowsAsync(new Exception("Server error occurred"));

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForAdminAsync("AnyTitle", null, "Active", 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving parent faq list: Server error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllParentFaqForAdminAsync_ShouldReturnFirstPage_WhenPageNumberExceedsTotalPages()
//        {
//            // Arrange
//            var parentFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1", Status = "Active" },
//            new NewsFaq { NewsFaqid = 2, Title = "FAQ 2", Status = "Inactive" }
//        };
//            var parentFaqDtos = parentFaqs.Select(faq => new ParentFaqListForAdminDTO
//            {
//                ParentFaqId = faq.NewsFaqid,
//                Title = faq.Title
//            }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForAdminAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
//                .ReturnsAsync(parentFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForAdminDTO>>(parentFaqs)).Returns(parentFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForAdminAsync(null, null, null, 10, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllParentFaqForAdminAsync_ShouldReturnFilteredList_WhenTitleFilterIsApplied()
//        {
//            // Arrange
//            var parentFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "Admin Guide" },
//            new NewsFaq { NewsFaqid = 2, Title = "Admin Manual" }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForAdminAsync("Admin", null, null))
//                .ReturnsAsync(parentFaqs);

//            var parentFaqDtos = parentFaqs.Select(faq => new ParentFaqListForAdminDTO
//            {
//                ParentFaqId = faq.NewsFaqid,
//                Title = faq.Title
//            }).ToList();
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForAdminDTO>>(parentFaqs)).Returns(parentFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForAdminAsync("Admin", null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual("Admin Guide", result.Data.Items[0].Title);
//            Assert.AreEqual("Admin Manual", result.Data.Items[1].Title);
//        }

//        #endregion


//        #region GetParentFaqDetailByIdForAdminAsync

//        [Test]
//            public async Task GetParentFaqDetailByIdForAdminAsync_ShouldReturnFaqDetail_WhenFaqExists()
//            {
//                // Arrange
//                var faqId = 1;
//                var faq = new NewsFaq { NewsFaqid = faqId, Title = "Sample FAQ" };
//                var faqDto = new ParentFaqDetailForAdminDTO { Title = faq.Title };

//                _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForAdminAsync(faqId)).ReturnsAsync(faq);
//                _mapperMock.Setup(mapper => mapper.Map<ParentFaqDetailForAdminDTO>(faq)).Returns(faqDto);

//                // Act
//                var result = await _newsFaqService.GetParentFaqDetailByIdForAdminAsync(faqId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(200, result.StatusCode);
//                Assert.AreEqual(faqDto, result.Data);
//            }

//            [Test]
//            public async Task GetParentFaqDetailByIdForAdminAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//            {
//                // Arrange
//                var faqId = 1;
//                _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForAdminAsync(faqId))
//                    .ThrowsAsync(new KeyNotFoundException("Parent FAQ not found"));

//                // Act
//                var result = await _newsFaqService.GetParentFaqDetailByIdForAdminAsync(faqId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Parent FAQ not found", result.Message);
//            }


//        [Test]
//        public async Task GetParentFaqDetailByIdForAdminAsync_ShouldReturnParentFaqDetail_WhenFaqExists()
//        {
//            // Arrange
//            int faqId = 1;
//            var parentFaq = new NewsFaq
//            {
//                NewsFaqid = faqId,
//                Title = "FAQ Title",
//                NewsFaqcontent = "FAQ Content",
//                User = new User { Name = "Admin User" },
//                Role = new Role { Name = "Admin" },
//                CreatedAt = DateTime.UtcNow,
//                UpdatedAt = DateTime.UtcNow,
//                Status = "Active"
//            };
//            var parentFaqDto = new ParentFaqDetailForAdminDTO
//            {
//                ParentFaqId = parentFaq.NewsFaqid,
//                Title = parentFaq.Title,
//                User = "Admin User",
//                ParentFaqcontent = parentFaq.NewsFaqcontent,
//                ForRole = "Admin",
//                Status = parentFaq.Status
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForAdminAsync(faqId)).ReturnsAsync(parentFaq);
//            _mapperMock.Setup(mapper => mapper.Map<ParentFaqDetailForAdminDTO>(parentFaq)).Returns(parentFaqDto);

//            // Act
//            var result = await _newsFaqService.GetParentFaqDetailByIdForAdminAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq details retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(faqId, result.Data.ParentFaqId);
//            Assert.AreEqual("FAQ Title", result.Data.Title);
//        }



//        [Test]
//        public async Task GetParentFaqDetailByIdForAdminAsync_ShouldReturnUnauthorized_WhenUserNotAuthorized()
//        {
//            // Arrange
//            int faqId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForAdminAsync(faqId))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.GetParentFaqDetailByIdForAdminAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get Parent faq detail: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetParentFaqDetailByIdForAdminAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            int faqId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForAdminAsync(faqId))
//                .ThrowsAsync(new Exception("Server error occurred"));

//            // Act
//            var result = await _newsFaqService.GetParentFaqDetailByIdForAdminAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving Parent faq details: Server error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        #endregion

//        #region AddParentFaqForAdminAsync Tests

//        [Test]
//        public async Task AddParentFaqForAdminAsync_ShouldReturnCreatedFaq_WhenFaqIsAddedSuccessfully()
//        {
//            // Arrange
//            var addParentFaqDto = new AddParentFaqForAdminDTO
//            {
//                UserId = 1,
//                Title = "FAQ Title",
//                ParentFaqcontent = "FAQ Content",
//                ForRoleId = 2
//            };

//            var newFaq = new NewsFaq
//            {
//                UserId = addParentFaqDto.UserId,
//                RoleId = addParentFaqDto.ForRoleId,
//                Title = addParentFaqDto.Title,
//                NewsFaqcontent = addParentFaqDto.ParentFaqcontent,
//                CreatedAt = DateTime.Now,
//                Status = "Active"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                .ReturnsAsync(newFaq);

//            // Act
//            var result = await _newsFaqService.AddParentFaqForAdminAsync(addParentFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Parent Faq added successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(addParentFaqDto.Title, result.Data.Title);
//        }

//        [Test]
//        public async Task AddParentFaqForAdminAsync_ShouldReturnUnauthorized_WhenUserIsUnauthorized()
//        {
//            // Arrange
//            var addParentFaqDto = new AddParentFaqForAdminDTO
//            {
//                UserId = 1,
//                Title = "Unauthorized FAQ",
//                ParentFaqcontent = "Unauthorized FAQ Content",
//                ForRoleId = 2
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.AddParentFaqForAdminAsync(addParentFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add parent faq: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task AddParentFaqForAdminAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            var addParentFaqDto = new AddParentFaqForAdminDTO
//            {
//                UserId = 1,
//                Title = "Server Error FAQ",
//                ParentFaqcontent = "FAQ Content causing error",
//                ForRoleId = 2
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new Exception("Server error occurred"));

//            // Act
//            var result = await _newsFaqService.AddParentFaqForAdminAsync(addParentFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding parent faq: Server error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        #endregion

//        #region UpdateParentFaqForAdminAsync

 
//        [Test]
//        public async Task UpdateParentFaqForAdminAsync_ShouldReturnUpdatedFaq_WhenUpdateIsSuccessful()
//        {
//            // Arrange
//            var updateFaqDto = new UpdateParentFaqForAdminDTO
//            {
//                ParentFaqId = 1,
//                Title = "Updated FAQ Title",
//                ParentFaqcontent = "Updated FAQ Content",
//                ForRoleId = 2
//            };

//            var updatedFaq = new NewsFaq
//            {
//                NewsFaqid = updateFaqDto.ParentFaqId,
//                Title = updateFaqDto.Title,
//                NewsFaqcontent = updateFaqDto.ParentFaqcontent,
//                RoleId = updateFaqDto.ForRoleId,
//                UpdatedAt = DateTime.Now
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                .ReturnsAsync(updatedFaq);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateParentFaqForAdminDTO>(updatedFaq)).Returns(updateFaqDto);

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqForAdminAsync(updateFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent Faq updated successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(updateFaqDto.Title, result.Data.Title);
//        }

//        [Test]
//        public async Task UpdateParentFaqForAdminAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            var updateFaqDto = new UpdateParentFaqForAdminDTO
//            {
//                ParentFaqId = 1,
//                Title = "Non-Existent FAQ"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new KeyNotFoundException("Parent Faq not found"));

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqForAdminAsync(updateFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent Faq not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateParentFaqForAdminAsync_ShouldReturnUnauthorized_WhenUserIsUnauthorized()
//        {
//            // Arrange
//            var updateFaqDto = new UpdateParentFaqForAdminDTO
//            {
//                ParentFaqId = 1,
//                Title = "Unauthorized Update"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqForAdminAsync(updateFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update parent faq: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateParentFaqForAdminAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            var updateFaqDto = new UpdateParentFaqForAdminDTO
//            {
//                ParentFaqId = 1,
//                Title = "FAQ causing server error"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new Exception("Server error occurred"));

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqForAdminAsync(updateFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating parent faq: Server error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        #endregion

//        #region DeleteParentFaqForAdminAsync


//            [Test]
//            public async Task DeleteParentFaqForAdminAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//            {
//                // Arrange
//                var deleteDto = new DeleteParentFaqForAdminDTO { ParentFaqId = 1 };

//                _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentFaqForAdminAsync(deleteDto.ParentFaqId))
//                    .ThrowsAsync(new KeyNotFoundException("Parent FAQ not found"));

//                // Act
//                var result = await _newsFaqService.DeleteParentFaqForAdminAsync(deleteDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Parent FAQ not found", result.Message);
//            }


//        [Test]
//        public async Task DeleteParentFaqForAdminAsync_ShouldReturnServerError_WhenUnexpectedExceptionOccurs()
//        {
//            // Arrange
//            var parentFaqId = 1;
//            var deleteDto = new DeleteParentFaqForAdminDTO { ParentFaqId = parentFaqId };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqByParentIdForAdminAsync(parentFaqId))
//                                  .ThrowsAsync(new Exception("Server error during deletion."));

//            // Act
//            var result = await _newsFaqService.DeleteParentFaqForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error permanently deleting Parent Faq: Server error during deletion.", result.Message);
//            Assert.IsNull(result.Data);
//        }



//        [Test]
//        public async Task DeleteParentFaqForAdminAsync_ShouldReturnError_WhenMapperFailsToMapDeletedParentFaq()
//        {
//            // Arrange
//            var parentFaqId = 1;
//            var deleteDto = new DeleteParentFaqForAdminDTO { ParentFaqId = parentFaqId };
//            var childFaqs = new List<NewsFaq> { new NewsFaq { NewsFaqid = 2 }, new NewsFaq { NewsFaqid = 3 } };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqByParentIdForAdminAsync(parentFaqId)).ReturnsAsync(childFaqs);
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentFaqForAdminAsync(parentFaqId)).Throws(new AutoMapperMappingException("Mapping failed"));

//            // Act
//            var result = await _newsFaqService.DeleteParentFaqForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error permanently deleting Parent Faq: Mapping failed", result.Message);
//            Assert.IsNull(result.Data);
//        }



//        #endregion

//        #region UpdateParentFaqStatusForAdminAsync


//        [Test]
//            public async Task UpdateParentFaqStatusForAdminAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//            {
//                // Arrange
//                var updateParentFaqDto = new NewsFaqDTO.UpdateParentFaqStatusForAdminDTO
//                {
//                    ParentFaqId = 99, // Non-existent ID
//                    Status = "Inactive"
//                };

//                _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqStatusForAdminAsync(It.IsAny<NewsFaq>()))
//                    .ThrowsAsync(new KeyNotFoundException("Parent FAQ not found"));

//                // Act
//                var result = await _newsFaqService.UpdateParentFaqStatusForAdminAsync(updateParentFaqDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Parent FAQ not found", result.Message);
//                Assert.IsNull(result.Data);

//                _newsFaqRepositoryMock.Verify(repo => repo.UpdateParentFaqStatusForAdminAsync(It.IsAny<NewsFaq>()), Times.Once);
//            }



//        #endregion




//        #endregion


//        #region Admin - Child Faq Management

//        #region GetAllChildFaqForAdminAsync

//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldReturnPagedResult_WhenChildFaqsExist()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;
//            var childFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1", ParentId = parentId },
//            new NewsFaq { NewsFaqid = 2, Title = "FAQ 2", ParentId = parentId }
//        };
//            var childFaqDtos = childFaqs.Select(f => new ChildFaqListForAdminDTO { ChildFaqId = f.NewsFaqid, Title = f.Title }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, null, null, null))
//                                  .ReturnsAsync(childFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForAdminDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(childFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual(pageSize, result.Data.PageSize);
//            Assert.AreEqual(pageNumber, result.Data.CurrentPage);
//        }

//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldReturnEmpty_WhenNoChildFaqsExist()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, null, null, null))
//                                  .ReturnsAsync(new List<NewsFaq>());
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForAdminDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(new List<ChildFaqListForAdminDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldReturnFilteredResults_WhenFiltersAreApplied()
//        {
//            // Arrange
//            int parentId = 1;
//            string title = "FAQ 1";
//            int roleId = 2;
//            string status = "Active";
//            int pageNumber = 1;
//            int pageSize = 1;

//            var childFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = title, RoleId = roleId, Status = status, ParentId = parentId }
//        };
//            var childFaqDtos = new List<ChildFaqListForAdminDTO>
//        {
//            new ChildFaqListForAdminDTO { ChildFaqId = 1, Title = title, ForRole = "Admin", Status = status }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, title, roleId, status))
//                                  .ReturnsAsync(childFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForAdminDTO>>(childFaqs)).Returns(childFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, title, roleId, status, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(1, result.Data.Items.Count);
//            Assert.AreEqual(title, result.Data.Items.First().Title);
//        }

//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldReturn404_WhenParentFaqNotFound()
//        {
//            // Arrange
//            int parentId = 999;
//            int pageNumber = 1;
//            int pageSize = 15;

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, null, null, null))
//                                  .ThrowsAsync(new KeyNotFoundException("Parent Faq not found"));

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent Faq not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldReturn403_WhenAccessDenied()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, null, null, null))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get child faq list: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldReturn500_OnServerError()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, null, null, null))
//                                  .ThrowsAsync(new Exception("Unexpected server error"));

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving child faq list: Unexpected server error", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldReturnSinglePage_WhenTotalFaqsAreLessThanPageSize()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;
//            var childFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1", ParentId = parentId },
//            new NewsFaq { NewsFaqid = 2, Title = "FAQ 2", ParentId = parentId }
//        };
//            var childFaqDtos = childFaqs.Select(f => new ChildFaqListForAdminDTO { ChildFaqId = f.NewsFaqid, Title = f.Title }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, null, null, null))
//                                  .ReturnsAsync(childFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForAdminDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(childFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(childFaqs.Count, result.Data.Items.Count);
//            Assert.AreEqual(1, result.Data.TotalPages);
//        }


//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldReturnEmptyData_WhenRequestedPageExceedsTotalPages()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 3; // Exceeds total pages
//            int pageSize = 1;
//            var childFaqs = new List<NewsFaq>
//    {
//        new NewsFaq { NewsFaqid = 1, Title = "FAQ 1", ParentId = parentId },
//        new NewsFaq { NewsFaqid = 2, Title = "FAQ 2", ParentId = parentId }
//    };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, null, null, null))
//                                  .ReturnsAsync(childFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForAdminDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(new List<ChildFaqListForAdminDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count); // Verifying that Items is empty
//            Assert.AreEqual(2, result.Data.TotalPages); // Only 2 pages available
//        }


//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldReturnPagedData_WhenTitleFilterIsApplied()
//        {
//            // Arrange
//            int parentId = 1;
//            string title = "FAQ 1";
//            int pageNumber = 1;
//            int pageSize = 15;
//            var childFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = title, ParentId = parentId }
//        };
//            var childFaqDtos = childFaqs.Select(f => new ChildFaqListForAdminDTO { ChildFaqId = f.NewsFaqid, Title = f.Title }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, title, null, null))
//                                  .ReturnsAsync(childFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForAdminDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(childFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, title, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(1, result.Data.Items.Count);
//            Assert.AreEqual(title, result.Data.Items.First().Title);
//        }

//        [Test]
//        public async Task GetAllChildFaqForAdminAsync_ShouldSortResultsByStatusAndId()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;
//            var childFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 2, Title = "FAQ 2", ParentId = parentId, Status = "Active" },
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1", ParentId = parentId, Status = "Unactive" }
//        };
//            var childFaqDtos = new List<ChildFaqListForAdminDTO>
//        {
//            new ChildFaqListForAdminDTO { ChildFaqId = 2, Title = "FAQ 2", Status = "Active" },
//            new ChildFaqListForAdminDTO { ChildFaqId = 1, Title = "FAQ 1", Status = "Unactive" }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForAdminAsync(parentId, null, null, null))
//                                  .ReturnsAsync(childFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForAdminDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(childFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForAdminAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual("FAQ 2", result.Data.Items.First().Title); // Active should appear before Unactive
//        }

//        #endregion


//        #region GetChildFaqDetailByIdForAdminAsync

//            [Test]
//            public async Task GetChildFaqDetailByIdForAdminAsync_ShouldReturnFaqDetail_WhenChildFaqExists()
//            {
//                // Arrange
//                var faqId = 1;
//                var faq = new NewsFaq { NewsFaqid = faqId, Title = "Sample Child FAQ" };
//                var faqDto = new ChildFaqDetailForAdminDTO { Title = faq.Title };

//                _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForAdminAsync(faqId)).ReturnsAsync(faq);
//                _mapperMock.Setup(mapper => mapper.Map<ChildFaqDetailForAdminDTO>(faq)).Returns(faqDto);

//                // Act
//                var result = await _newsFaqService.GetChildFaqDetailByIdForAdminAsync(faqId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(200, result.StatusCode);
//                Assert.AreEqual(faqDto, result.Data);
//            }

//            [Test]
//            public async Task GetChildFaqDetailByIdForAdminAsync_ShouldReturnNotFound_WhenChildFaqDoesNotExist()
//            {
//                // Arrange
//                var faqId = 1;
//                _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForAdminAsync(faqId))
//                    .ThrowsAsync(new KeyNotFoundException("Child FAQ not found"));

//                // Act
//                var result = await _newsFaqService.GetChildFaqDetailByIdForAdminAsync(faqId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Child FAQ not found", result.Message);
//            }

//        [Test]
//        public async Task GetChildFaqDetailByIdForAdminAsync_ShouldReturnFaqDetails_WhenFaqExists()
//        {
//            // Arrange
//            int faqId = 1;
//            var childFaq = new NewsFaq
//            {
//                NewsFaqid = faqId,
//                Title = "FAQ Title",
//                User = new User { Name = "AdminUser" },
//                Role = new Role { Name = "Admin" },
//                NewsFaqcontent = "FAQ content",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                DeletedAt = null
//            };
//            var childFaqDto = new ChildFaqDetailForAdminDTO
//            {
//                ChildFaqId = faqId,
//                Title = "FAQ Title",
//                User = "AdminUser",
//                ForRole = "Admin",
//                ChildFaqcontent = "FAQ content",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                DeletedAt = null
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForAdminAsync(faqId)).ReturnsAsync(childFaq);
//            _mapperMock.Setup(mapper => mapper.Map<ChildFaqDetailForAdminDTO>(childFaq)).Returns(childFaqDto);

//            // Act
//            var result = await _newsFaqService.GetChildFaqDetailByIdForAdminAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child faq details retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(faqId, result.Data.ChildFaqId);
//            Assert.AreEqual("FAQ Title", result.Data.Title);
//        }

//        [Test]
//        public async Task GetChildFaqDetailByIdForAdminAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            int faqId = 99;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForAdminAsync(faqId)).ThrowsAsync(new KeyNotFoundException("Child Faq not found"));

//            // Act
//            var result = await _newsFaqService.GetChildFaqDetailByIdForAdminAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child Faq not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetChildFaqDetailByIdForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccess()
//        {
//            // Arrange
//            int faqId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForAdminAsync(faqId)).ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.GetChildFaqDetailByIdForAdminAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get child faq detail: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetChildFaqDetailByIdForAdminAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            int faqId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForAdminAsync(faqId)).ThrowsAsync(new Exception("Internal server error"));

//            // Act
//            var result = await _newsFaqService.GetChildFaqDetailByIdForAdminAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving child faq details: Internal server error", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        #endregion

//        #region AddChildFaqForAdminAsync

//        [Test]
//        public async Task AddChildFaqForAdminAsync_ShouldAddChildFaqSuccessfully()
//        {
//            // Arrange
//            var addChildFaqDto = new AddChildFaqForAdminDTO
//            {
//                UserId = 1,
//                ParentId = 10,
//                Title = "New Child FAQ",
//                ChildFaqcontent = "Child FAQ content",
//                Image = "image_path.jpg"
//            };

//            var childFaqEntity = new NewsFaq
//            {
//                UserId = addChildFaqDto.UserId,
//                ParentId = addChildFaqDto.ParentId,
//                Title = addChildFaqDto.Title,
//                NewsFaqcontent = addChildFaqDto.ChildFaqcontent,
//                Image = addChildFaqDto.Image,
//                CreatedAt = DateTime.Now,
//                Status = "Active",
//                RoleId = 3 // Example role ID
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                                  .ReturnsAsync(childFaqEntity);
//            _mapperMock.Setup(mapper => mapper.Map<AddChildFaqForAdminDTO>(childFaqEntity)).Returns(addChildFaqDto);

//            // Act
//            var result = await _newsFaqService.AddChildFaqForAdminAsync(addChildFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Child Faq added successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(addChildFaqDto.Title, result.Data.Title);
//        }

//        [Test]
//        public async Task AddChildFaqForAdminAsync_ShouldReturnForbidden_WhenUnauthorizedAccessOccurs()
//        {
//            // Arrange
//            var addChildFaqDto = new AddChildFaqForAdminDTO
//            {
//                UserId = 1,
//                ParentId = 10,
//                Title = "New Child FAQ"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access."));

//            // Act
//            var result = await _newsFaqService.AddChildFaqForAdminAsync(addChildFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add child faq: Unauthorized access.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task AddChildFaqForAdminAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var addChildFaqDto = new AddChildFaqForAdminDTO
//            {
//                UserId = 1,
//                ParentId = 10,
//                Title = "New Child FAQ"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new Exception("Server error."));

//            // Act
//            var result = await _newsFaqService.AddChildFaqForAdminAsync(addChildFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding child faq: Server error.", result.Message);
//            Assert.IsNull(result.Data);
//        }



//        #endregion


//          #region UpdateChildFaqForAdminAsync


//        [Test]
//            public async Task UpdateChildFaqForAdminAsync_ShouldReturnNotFound_WhenChildFaqDoesNotExist()
//            {
//                // Arrange
//                var updateFaqDto = new UpdateChildFaqForAdminDTO { ChildFaqId = 1, Title = "Updated Title" };

//                _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                    .ThrowsAsync(new KeyNotFoundException("Child FAQ not found"));

//                // Act
//                var result = await _newsFaqService.UpdateChildFaqForAdminAsync(updateFaqDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Child FAQ not found", result.Message);
//            }

//            [Test]
//            public async Task UpdateChildFaqForAdminAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            var updateDto = new UpdateChildFaqForAdminDTO { ChildFaqId = 999 };
//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildFaqForAdminAsync(It.IsAny<NewsFaq>())).ThrowsAsync(new KeyNotFoundException("Child FAQ not found"));

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqForAdminAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child FAQ not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateChildFaqForAdminAsync_ShouldUpdateChildFaq_WhenValidInputIsProvided()
//        {
//            // Arrange
//            var updateDto = new UpdateChildFaqForAdminDTO
//            {
//                ChildFaqId = 1,
//                Title = "Updated FAQ Title",
//                ChildFaqcontent = "Updated FAQ Content",
//                Image = "updated_image.jpg"
//            };

//            var updatedNewsFaq = new NewsFaq
//            {
//                NewsFaqid = updateDto.ChildFaqId,
//                Title = updateDto.Title,
//                NewsFaqcontent = updateDto.ChildFaqcontent,
//                Image = updateDto.Image,
//                UpdatedAt = DateTime.Now
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildFaqForAdminAsync(It.IsAny<NewsFaq>())).ReturnsAsync(updatedNewsFaq);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateChildFaqForAdminDTO>(updatedNewsFaq)).Returns(updateDto);

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqForAdminAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child Faq updated successfully!", result.Message);
//            Assert.AreEqual(updateDto.Title, result.Data.Title);
//            Assert.AreEqual(updateDto.ChildFaqcontent, result.Data.ChildFaqcontent);
//        }

//        [Test]
//        public async Task UpdateChildFaqForAdminAsync_ShouldReturnForbidden_WhenUserHasNoPermission()
//        {
//            // Arrange
//            var updateDto = new UpdateChildFaqForAdminDTO { ChildFaqId = 1 };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildFaqForAdminAsync(It.IsAny<NewsFaq>())).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqForAdminAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update child faq: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateChildFaqForAdminAsync_ShouldReturnInternalError_WhenUnexpectedExceptionOccurs()
//        {
//            // Arrange
//            var updateDto = new UpdateChildFaqForAdminDTO { ChildFaqId = 1 };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildFaqForAdminAsync(It.IsAny<NewsFaq>())).ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqForAdminAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating child faq: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }




//        #endregion

//        #region DeleteChildFaqForAdminAsync



//        [Test]
//        public async Task DeleteChildFaqForAdminAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildFaqForAdminDTO { ChildFaqId = 200 };
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForAdminAsync(deleteDto.ChildFaqId))
//                                  .ThrowsAsync(new KeyNotFoundException("Child FAQ not found"));

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child FAQ not found", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task DeleteChildFaqForAdminAsync_ShouldDeleteChildFaq_WhenValidInputIsProvided()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildFaqForAdminDTO { ChildFaqId = 1 };
//            var deletedChildFaq = new NewsFaq { NewsFaqid = deleteDto.ChildFaqId, Title = "Test FAQ", DeletedAt = DateTime.Now };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForAdminAsync(deleteDto.ChildFaqId)).ReturnsAsync(deletedChildFaq);
//            _mapperMock.Setup(mapper => mapper.Map<DeleteChildFaqForAdminDTO>(deletedChildFaq)).Returns(deleteDto);

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child Faq has been permanently deleted successfully.", result.Message);
//            Assert.AreEqual(deleteDto.ChildFaqId, result.Data.ChildFaqId);
//        }

//        [Test]
//        public async Task DeleteChildFaqForAdminAsync_ShouldReturnNotFound_WhenChildFaqDoesNotExist()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildFaqForAdminDTO { ChildFaqId = 99 };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForAdminAsync(deleteDto.ChildFaqId))
//                                  .ThrowsAsync(new KeyNotFoundException("Child Faq not found in the list."));

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child Faq not found in the list.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteChildFaqForAdminAsync_ShouldReturnForbidden_WhenUserHasNoPermission()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildFaqForAdminDTO { ChildFaqId = 1 };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForAdminAsync(deleteDto.ChildFaqId))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while hard delete child Faq: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteChildFaqForAdminAsync_ShouldReturnInternalError_WhenUnexpectedExceptionOccurs()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildFaqForAdminDTO { ChildFaqId = 1 };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForAdminAsync(deleteDto.ChildFaqId))
//                                  .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error permanently deleting child Faq: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteChildFaqForAdminAsync_ShouldMapDeletedChildFaqDetails_WhenDeletedSuccessfully()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildFaqForAdminDTO { ChildFaqId = 1 };
//            var deletedChildFaq = new NewsFaq
//            {
//                NewsFaqid = deleteDto.ChildFaqId,
//                Title = "Deleted FAQ Title",
//                DeletedAt = DateTime.Now
//            };
//            var expectedDto = new DeleteChildFaqForAdminDTO
//            {
//                ChildFaqId = deletedChildFaq.NewsFaqid,
//                Title = "Deleted FAQ Title",
//                DeletedAt = deletedChildFaq.DeletedAt
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForAdminAsync(deleteDto.ChildFaqId)).ReturnsAsync(deletedChildFaq);
//            _mapperMock.Setup(mapper => mapper.Map<DeleteChildFaqForAdminDTO>(deletedChildFaq)).Returns(expectedDto);

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForAdminAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(expectedDto.ChildFaqId, result.Data.ChildFaqId);
//            Assert.AreEqual(expectedDto.Title, result.Data.Title);
//            Assert.IsNotNull(result.Data.DeletedAt);
//        }



//        #endregion


//        #region UpdateChildFaqStatusForAdminAsync

//        [Test]
//        public async Task UpdateChildNewsStatusForAdminAsync_ShouldReturnUpdatedStatus_WhenChildNewsExistsAndStatusChangeIsAllowed()
//        {
//            // Arrange
//            var updateChildNewsStatusDto = new UpdateChildNewsStatusForAdminDTO
//            {
//                ChildNewsId = 2,
//                Status = "Active"
//            };

//            var existingChildNews = new NewsFaq
//            {
//                NewsFaqid = updateChildNewsStatusDto.ChildNewsId,
//                Status = "Unactive",
//                ParentId = 1
//            };

//            var parentNews = new NewsFaq
//            {
//                NewsFaqid = 1,
//                Status = "Active" // Allows the child news to be updated to "Active"
//            };

//            var updatedChildNews = new NewsFaq
//            {
//                NewsFaqid = updateChildNewsStatusDto.ChildNewsId,
//                Status = updateChildNewsStatusDto.Status,
//                ParentId = existingChildNews.ParentId,
//                UpdatedAt = DateTime.Now
//            };

//            var expectedResponse = new DataResponse<UpdateChildNewsStatusForAdminDTO>
//            {
//                Data = updateChildNewsStatusDto,
//                Message = "Child News updated successfully!",
//                StatusCode = 200
//            };

//            // Mock setup
//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForAdminAsync(updateChildNewsStatusDto.ChildNewsId))
//                .ReturnsAsync(existingChildNews);

//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForAdminAsync(existingChildNews.ParentId.Value))
//                .ReturnsAsync(parentNews);

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildNewsForAdminAsync(It.IsAny<NewsFaq>()))
//                .ReturnsAsync(updatedChildNews);

//            _mapperMock.Setup(mapper => mapper.Map<UpdateChildNewsStatusForAdminDTO>(updatedChildNews))
//                .Returns(updateChildNewsStatusDto);

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsStatusForAdminAsync(updateChildNewsStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(expectedResponse.StatusCode, result.StatusCode);
//            Assert.AreEqual(expectedResponse.Message, result.Message);
//            Assert.AreEqual(expectedResponse.Data.ChildNewsId, result.Data.ChildNewsId);
//            Assert.AreEqual(expectedResponse.Data.Status, result.Data.Status);

//            _newsFaqRepositoryMock.Verify(repo => repo.GetChildNewsByIdForAdminAsync(updateChildNewsStatusDto.ChildNewsId), Times.Once);
//            _newsFaqRepositoryMock.Verify(repo => repo.GetParentNewsByIdForAdminAsync(existingChildNews.ParentId.Value), Times.Once);
//            _newsFaqRepositoryMock.Verify(repo => repo.UpdateChildNewsForAdminAsync(It.IsAny<NewsFaq>()), Times.Once);
//            _mapperMock.Verify(mapper => mapper.Map<UpdateChildNewsStatusForAdminDTO>(updatedChildNews), Times.Once);
//        }

//        [Test]
//        public async Task UpdateChildNewsStatusForAdminAsync_ShouldReturn500_WhenUnhandledExceptionOccurs()
//        {
//            // Arrange
//            var updateChildNewsStatusDto = new UpdateChildNewsStatusForAdminDTO
//            {
//                ChildNewsId = 2,
//                Status = "Active"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForAdminAsync(updateChildNewsStatusDto.ChildNewsId))
//                .ThrowsAsync(new Exception("Unhandled exception occurred"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsStatusForAdminAsync(updateChildNewsStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating child News: Unhandled exception occurred", result.Message);
//            _newsFaqRepositoryMock.Verify(repo => repo.GetChildNewsByIdForAdminAsync(updateChildNewsStatusDto.ChildNewsId), Times.Once);
//            _newsFaqRepositoryMock.Verify(repo => repo.UpdateChildNewsForAdminAsync(It.IsAny<NewsFaq>()), Times.Never);
//        }

//        [Test]
//        public async Task UpdateChildNewsStatusForAdminAsync_ShouldReturn403_WhenUnauthorizedAccessExceptionThrown()
//        {
//            // Arrange
//            var updateChildNewsStatusDto = new UpdateChildNewsStatusForAdminDTO
//            {
//                ChildNewsId = 2,
//                Status = "Active"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForAdminAsync(updateChildNewsStatusDto.ChildNewsId))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied while updating child News status"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsStatusForAdminAsync(updateChildNewsStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while updating child News status: Access denied while updating child News status", result.Message);
//            _newsFaqRepositoryMock.Verify(repo => repo.GetChildNewsByIdForAdminAsync(updateChildNewsStatusDto.ChildNewsId), Times.Once);
//            _newsFaqRepositoryMock.Verify(repo => repo.UpdateChildNewsForAdminAsync(It.IsAny<NewsFaq>()), Times.Never);
//        }

//        [Test]
//        public async Task UpdateChildNewsStatusForAdminAsync_ShouldReturn404_WhenParentNewsNotFound()
//        {
//            // Arrange
//            var updateChildNewsStatusDto = new UpdateChildNewsStatusForAdminDTO
//            {
//                ChildNewsId = 2,
//                Status = "Active"
//            };

//            var existingChildNews = new NewsFaq
//            {
//                NewsFaqid = updateChildNewsStatusDto.ChildNewsId,
//                Status = "Unactive",
//                ParentId = 1
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForAdminAsync(updateChildNewsStatusDto.ChildNewsId))
//                .ReturnsAsync(existingChildNews);

//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForAdminAsync(existingChildNews.ParentId.Value))
//                .ThrowsAsync(new KeyNotFoundException("Parent News not found"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsStatusForAdminAsync(updateChildNewsStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent News not found", result.Message);
//            _newsFaqRepositoryMock.Verify(repo => repo.GetParentNewsByIdForAdminAsync(existingChildNews.ParentId.Value), Times.Once);
//            _newsFaqRepositoryMock.Verify(repo => repo.UpdateChildNewsForAdminAsync(It.IsAny<NewsFaq>()), Times.Never);
//        }

//        [Test]
//        public async Task UpdateChildNewsStatusForAdminAsync_ShouldReturn404_WhenChildNewsNotFound()
//        {
//            // Arrange
//            var updateChildNewsStatusDto = new UpdateChildNewsStatusForAdminDTO
//            {
//                ChildNewsId = 2,
//                Status = "Active"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForAdminAsync(updateChildNewsStatusDto.ChildNewsId))
//                .ThrowsAsync(new KeyNotFoundException("Child News not found"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsStatusForAdminAsync(updateChildNewsStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child News not found", result.Message);
//            _newsFaqRepositoryMock.Verify(repo => repo.GetChildNewsByIdForAdminAsync(updateChildNewsStatusDto.ChildNewsId), Times.Once);
//            _newsFaqRepositoryMock.Verify(repo => repo.UpdateChildNewsForAdminAsync(It.IsAny<NewsFaq>()), Times.Never);
//        }




//        #endregion


//        #endregion

//        #endregion



//        #region  DOET

//        #region DOET - Parent News Management

//        #region GetAllParentNewsForDoetAsync


//        [Test]
//        public async Task GetAllParentNewsForDoetAsync_ShouldReturnFilteredResults_WhenTitleAndStatusAreProvided()
//        {
//            // Arrange
//            string titleFilter = "News";
//            string statusFilter = "Active";
//            int pageNumber = 1;
//            int pageSize = 15;

//            var filteredParentNews = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "News 1", Status = "Active" },
//            new NewsFaq { NewsFaqid = 2, Title = "News 2", Status = "Active" }
//        };

//            var parentNewsDtos = filteredParentNews.Select(n => new ParentNewsListForDoetDTO { ParentNewsId = n.NewsFaqid, Title = n.Title }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForDoetAsync(titleFilter, null, statusFilter)).ReturnsAsync(filteredParentNews);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentNewsListForDoetDTO>>(filteredParentNews)).Returns(parentNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForDoetAsync(titleFilter, null, statusFilter, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(filteredParentNews.Count, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllParentNewsForDoetAsync_ShouldReturnEmptyResults_WhenNoMatchingNewsExist()
//        {
//            // Arrange
//            string titleFilter = "Nonexistent";
//            string statusFilter = "Inactive";
//            int pageNumber = 1;
//            int pageSize = 15;

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForDoetAsync(titleFilter, null, statusFilter))
//                                  .ReturnsAsync(new List<NewsFaq>());
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentNewsListForDoetDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(new List<ParentNewsListForDoetDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForDoetAsync(titleFilter, null, statusFilter, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count); // Replace IsEmpty with Count check
//        }

//        [Test]
//        public async Task GetAllParentNewsForDoetAsync_ShouldHandlePageExceedingTotalPages_ReturnEmptyPage()
//        {
//            // Arrange
//            int pageNumber = 10; // Exceeds the total pages
//            int pageSize = 15;
//            var parentNews = new List<NewsFaq>
//    {
//        new NewsFaq { NewsFaqid = 1, Title = "News 1", Status = "Active" }
//    };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForDoetAsync(null, null, null))
//                                  .ReturnsAsync(parentNews);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentNewsListForDoetDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(new List<ParentNewsListForDoetDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForDoetAsync(null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count); // Replace IsEmpty with Count check
//        }


//        [Test]
//        public async Task GetAllParentNewsForDoetAsync_ShouldReturnNotFound_WhenParentNewsDoesNotExist()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForDoetAsync(null, null, null))
//                                  .ThrowsAsync(new KeyNotFoundException("Parent News not found"));

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForDoetAsync(null, null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent News not found", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task GetAllParentNewsForDoetAsync_ShouldHandleServerError_WhenExceptionIsThrown()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentNewsForDoetAsync(null, null, null))
//                                  .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.GetAllParentNewsForDoetAsync(null, null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving parent news list: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }
    

//    #endregion


//        #region GetParentNewsDetailByIdForDoetAsync

//    [Test]
//            public async Task GetParentNewsDetailByIdForDoetAsync_ShouldReturnNewsDetail_WhenNewsExists()
//            {
//                // Arrange
//                var newsId = 1;
//                var news = new NewsFaq { NewsFaqid = newsId, Title = "Sample Doet News" };
//                var newsDto = new ParentNewsDetailForDoetDTO { Title = news.Title };

//                _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForDoetAsync(newsId)).ReturnsAsync(news);
//                _mapperMock.Setup(mapper => mapper.Map<ParentNewsDetailForDoetDTO>(news)).Returns(newsDto);

//                // Act
//                var result = await _newsFaqService.GetParentNewsDetailByIdForDoetAsync(newsId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(200, result.StatusCode);
//                Assert.AreEqual(newsDto, result.Data);
//            }

//            [Test]
//            public async Task GetParentNewsDetailByIdForDoetAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//            {
//                // Arrange
//                var newsId = 1;
//                _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForDoetAsync(newsId))
//                    .ThrowsAsync(new KeyNotFoundException("Doet parent news not found"));

//                // Act
//                var result = await _newsFaqService.GetParentNewsDetailByIdForDoetAsync(newsId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Doet parent news not found", result.Message);
//            }

//        [Test]
//        public async Task GetParentNewsDetailByIdForDoetAsync_ShouldReturnParentNewsDetail_WhenNewsExists()
//        {
//            // Arrange
//            int newsId = 1;
//            var parentNews = new NewsFaq
//            {
//                NewsFaqid = newsId,
//                Title = "Test News",
//                NewsFaqcontent = "Content of the news",
//                User = new User { Name = "Test User" },
//                Role = new Role { Name = "DOET" },
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            };

//            var expectedDto = new ParentNewsDetailForDoetDTO
//            {
//                ParentNewsId = newsId,
//                Title = "Test News",
//                ParentNewscontent = "Content of the news",
//                User = "Test User",
//                ForRole = "DOET",
//                Status = "Active"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForDoetAsync(newsId)).ReturnsAsync(parentNews);
//            _mapperMock.Setup(mapper => mapper.Map<ParentNewsDetailForDoetDTO>(parentNews)).Returns(expectedDto);

//            // Act
//            var result = await _newsFaqService.GetParentNewsDetailByIdForDoetAsync(newsId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news details retrieved successfully!", result.Message);
//            Assert.AreEqual(expectedDto.ParentNewsId, result.Data.ParentNewsId);
//            Assert.AreEqual(expectedDto.Title, result.Data.Title);
//        }


//        [Test]
//        public async Task GetParentNewsDetailByIdForDoetAsync_ShouldReturnForbidden_WhenAccessDenied()
//        {
//            // Arrange
//            int newsId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForDoetAsync(newsId))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.GetParentNewsDetailByIdForDoetAsync(newsId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get Parent news detail: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetParentNewsDetailByIdForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            int newsId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentNewsByIdForDoetAsync(newsId))
//                .ThrowsAsync(new Exception("Database connection error"));

//            // Act
//            var result = await _newsFaqService.GetParentNewsDetailByIdForDoetAsync(newsId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving Parent news details: Database connection error", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        #endregion


//        #region AddParentNewsForDoetAsync

//        [Test]
//        public async Task AddParentNewsForDoetAsync_ShouldAddParentNewsSuccessfully()
//        {
//            // Arrange
//            var addDto = new AddParentNewsForDoetDTO
//            {
//                UserId = 1,
//                Title = "New Parent News",
//                ParentNewscontent = "Content for the new parent news",
//                Image = "image.png",
//                ForRoleId = 2
//            };
//            var addedNewsFaq = new NewsFaq
//            {
//                NewsFaqid = 1,
//                UserId = addDto.UserId,
//                RoleId = addDto.ForRoleId,
//                Title = addDto.Title,
//                NewsFaqcontent = addDto.ParentNewscontent,
//                Image = addDto.Image,
//                Status = "Active",
//                CreatedAt = DateTime.Now
//            };

//            var expectedDto = new AddParentNewsForDoetDTO
//            {
//                UserId = addDto.UserId,
//                Title = addDto.Title,
//                ParentNewscontent = addDto.ParentNewscontent,
//                Image = addDto.Image,
//                ForRoleId = addDto.ForRoleId,
//                Status = "Active",
//                CreatedAt = addedNewsFaq.CreatedAt
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentNewsForDoetAsync(It.IsAny<NewsFaq>())).ReturnsAsync(addedNewsFaq);
//            _mapperMock.Setup(mapper => mapper.Map<AddParentNewsForDoetDTO>(addedNewsFaq)).Returns(expectedDto);

//            // Act
//            var result = await _newsFaqService.AddParentNewsForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Parent News added successfully!", result.Message);
//            Assert.AreEqual(expectedDto.Title, result.Data.Title);
//            Assert.AreEqual(expectedDto.ParentNewscontent, result.Data.ParentNewscontent);
//            Assert.AreEqual(expectedDto.Status, result.Data.Status);
//        }

//        [Test]
//        public async Task AddParentNewsForDoetAsync_ShouldReturnForbidden_WhenAccessDenied()
//        {
//            // Arrange
//            var addDto = new AddParentNewsForDoetDTO { Title = "Restricted News" };
//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.AddParentNewsForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add parent news: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task AddParentNewsForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var addDto = new AddParentNewsForDoetDTO { Title = "Error News" };
//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _newsFaqService.AddParentNewsForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding parent news: Database error", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task AddParentNewsForDoetAsync_ShouldReturnSuccessWithDefaultValues_WhenOptionalFieldsAreNull()
//        {
//            // Arrange
//            var addDto = new AddParentNewsForDoetDTO
//            {
//                UserId = 1,
//                Title = "News with Defaults",
//                ForRoleId = 2
//            };
//            var addedNewsFaq = new NewsFaq
//            {
//                NewsFaqid = 1,
//                UserId = addDto.UserId,
//                RoleId = addDto.ForRoleId,
//                Title = addDto.Title,
//                NewsFaqcontent = null,
//                Image = null,
//                Status = "Active",
//                CreatedAt = DateTime.Now
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentNewsForDoetAsync(It.IsAny<NewsFaq>())).ReturnsAsync(addedNewsFaq);
//            _mapperMock.Setup(mapper => mapper.Map<AddParentNewsForDoetDTO>(addedNewsFaq)).Returns(addDto);

//            // Act
//            var result = await _newsFaqService.AddParentNewsForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Parent News added successfully!", result.Message);
//            Assert.AreEqual("Active", result.Data.Status);
//            Assert.IsNull(result.Data.ParentNewscontent);
//            Assert.IsNull(result.Data.Image);
//        }


//        #endregion


//        #region UpdateParentNewsForDoetAsync


//        [Test]
//            public async Task UpdateParentNewsForDoetAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//            {
//                // Arrange
//                var updateNewsDto = new UpdateParentNewsForDoetDTO { ParentNewsId = 1, Title = "Updated Doet Title" };

//                _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                    .ThrowsAsync(new KeyNotFoundException("Doet parent news not found"));

//                // Act
//                var result = await _newsFaqService.UpdateParentNewsForDoetAsync(updateNewsDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Doet parent news not found", result.Message);
//            }


//        [Test]
//        public async Task UpdateParentNewsForDoetAsync_ShouldUpdateParentNews_WhenNewsExists()
//        {
//            // Arrange
//            var updateDto = new UpdateParentNewsForDoetDTO
//            {
//                ParentNewsId = 1,
//                Title = "Updated News",
//                ParentNewscontent = "Updated content",
//                Image = "updated_image.png",
//                ForRoleId = 2
//            };
//            var updatedNewsFaq = new NewsFaq
//            {
//                NewsFaqid = updateDto.ParentNewsId,
//                Title = updateDto.Title,
//                NewsFaqcontent = updateDto.ParentNewscontent,
//                Image = updateDto.Image,
//                RoleId = updateDto.ForRoleId,
//                UpdatedAt = DateTime.Now
//            };
//            var expectedDto = new UpdateParentNewsForDoetDTO
//            {
//                ParentNewsId = updateDto.ParentNewsId,
//                Title = updateDto.Title,
//                ParentNewscontent = updateDto.ParentNewscontent,
//                Image = updateDto.Image,
//                ForRoleId = updateDto.ForRoleId,
//                UpdatedAt = DateTime.Now
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                .ReturnsAsync(updatedNewsFaq);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateParentNewsForDoetDTO>(updatedNewsFaq)).Returns(expectedDto);

//            // Act
//            var result = await _newsFaqService.UpdateParentNewsForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent News updated successfully!", result.Message);
//            Assert.AreEqual(expectedDto.Title, result.Data.Title);
//            Assert.AreEqual(expectedDto.ParentNewscontent, result.Data.ParentNewscontent);
//        }



//        [Test]
//        public async Task UpdateParentNewsForDoetAsync_ShouldReturnForbidden_WhenAccessDenied()
//        {
//            // Arrange
//            var updateDto = new UpdateParentNewsForDoetDTO { ParentNewsId = 1 };
//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.UpdateParentNewsForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update parent news: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateParentNewsForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var updateDto = new UpdateParentNewsForDoetDTO { ParentNewsId = 1 };
//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new Exception("Database connection error"));

//            // Act
//            var result = await _newsFaqService.UpdateParentNewsForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating parent news: Database connection error", result.Message);
//            Assert.IsNull(result.Data);
//        }






//        #endregion


//            #region DeleteParentNewsForDoetAsync

//            [Test]
//            public async Task DeleteParentNewsForDoetAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//            {
//                // Arrange
//                var deleteDto = new DeleteParentNewsForDoetDTO { ParentNewsId = 1 };

//                _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentNewsForDoetAsync(deleteDto.ParentNewsId))
//                    .ThrowsAsync(new KeyNotFoundException("Doet parent news not found"));

//                // Act
//                var result = await _newsFaqService.DeleteParentNewsForDoetAsync(deleteDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Doet parent news not found", result.Message);
//            }

//        [Test]
//        public async Task DeleteParentNewsForDoetAsync_ShouldDeleteParentAndChildNewsSuccessfully()
//        {
//            // Arrange
//            var parentNewsId = 1;
//            var deleteDto = new DeleteParentNewsForDoetDTO { ParentNewsId = parentNewsId };
//            var deletedParentNews = new NewsFaq { NewsFaqid = parentNewsId, DeletedAt = DateTime.Now };

//            var childNewsList = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 2, ParentId = parentNewsId, DeletedAt = DateTime.Now },
//            new NewsFaq { NewsFaqid = 3, ParentId = parentNewsId, DeletedAt = DateTime.Now }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsByParentIdForDoetAsync(parentNewsId))
//                .ReturnsAsync(childNewsList);
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentNewsForDoetAsync(parentNewsId))
//                .ReturnsAsync(deletedParentNews);

//            var childNewsDtoList = new List<DeleteChildNewsForDoetDTO>
//        {
//            new DeleteChildNewsForDoetDTO { ChildNewsId = 2 },
//            new DeleteChildNewsForDoetDTO { ChildNewsId = 3 }
//        };
//            var mappedParentNewsDto = new DeleteParentNewsForDoetDTO
//            {
//                ParentNewsId = parentNewsId,
//                DeletedChildNews = childNewsDtoList
//            };

//            _mapperMock.Setup(mapper => mapper.Map<List<DeleteChildNewsForDoetDTO>>(childNewsList))
//                .Returns(childNewsDtoList);
//            _mapperMock.Setup(mapper => mapper.Map<DeleteParentNewsForDoetDTO>(deletedParentNews))
//                .Returns(mappedParentNewsDto);

//            // Act
//            var result = await _newsFaqService.DeleteParentNewsForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent News has been permanently deleted successfully.", result.Message);
//            Assert.AreEqual(parentNewsId, result.Data.ParentNewsId);
//            Assert.AreEqual(2, result.Data.DeletedChildNews.Count);
//        }

//        [Test]
//        public async Task DeleteParentNewsForDoetAsync_ShouldReturnNotFound_WhenParentNewsDoesNotExist()
//        {
//            // Arrange
//            var parentNewsId = 99;
//            var deleteDto = new DeleteParentNewsForDoetDTO { ParentNewsId = parentNewsId };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsByParentIdForDoetAsync(parentNewsId))
//                .ReturnsAsync(new List<NewsFaq>());
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentNewsForDoetAsync(parentNewsId))
//                .ThrowsAsync(new KeyNotFoundException("Parent News not found in the list."));

//            // Act
//            var result = await _newsFaqService.DeleteParentNewsForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent News not found in the list.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteParentNewsForDoetAsync_ShouldReturnForbidden_WhenAccessIsDenied()
//        {
//            // Arrange
//            var parentNewsId = 1;
//            var deleteDto = new DeleteParentNewsForDoetDTO { ParentNewsId = parentNewsId };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsByParentIdForDoetAsync(parentNewsId))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied."));

//            // Act
//            var result = await _newsFaqService.DeleteParentNewsForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while hard delete Parent News: Access denied.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteParentNewsForDoetAsync_ShouldHandleServerError_AndReturnInternalServerError()
//        {
//            // Arrange
//            var parentNewsId = 1;
//            var deleteDto = new DeleteParentNewsForDoetDTO { ParentNewsId = parentNewsId };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsByParentIdForDoetAsync(parentNewsId))
//                .ThrowsAsync(new Exception("Unexpected server error"));

//            // Act
//            var result = await _newsFaqService.DeleteParentNewsForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error permanently deleting Parent News: Unexpected server error", result.Message);
//            Assert.IsNull(result.Data);
//        }



//        #endregion


//        #region GetAllStatusesNewsForDoetAsync



//        #endregion




//        #endregion


//        #region DOET - Child News Management

//        #region  GetAllChildNewsForDoetAsync

//        [Test]
//        public async Task GetAllChildNewsForDoetAsync_ShouldReturnPagedResults_WhenChildNewsExists()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;
//            var childNews = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "News 1", ParentId = parentId, Status = "Active" },
//            new NewsFaq { NewsFaqid = 2, Title = "News 2", ParentId = parentId, Status = "Inactive" }
//        };
//            var mappedNewsDtos = childNews.Select(n => new ChildNewsListForDoetDTO
//            {
//                ChildNewsId = n.NewsFaqid,
//                Title = n.Title,
//                Status = n.Status
//            }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForDoetAsync(parentId, null, null, null))
//                                  .ReturnsAsync(childNews);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildNewsListForDoetDTO>>(childNews))
//                       .Returns(mappedNewsDtos);

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForDoetAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(mappedNewsDtos.Count, result.Data.Items.Count);
//            Assert.AreEqual(pageNumber, result.Data.CurrentPage);
//            Assert.AreEqual(pageSize, result.Data.PageSize);
//        }

//        [Test]
//        public async Task GetAllChildNewsForDoetAsync_ShouldReturnEmptyList_WhenNoMatchingNewsExists()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForDoetAsync(parentId, "Nonexistent", null, "Inactive"))
//                                  .ReturnsAsync(new List<NewsFaq>());
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildNewsListForDoetDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(new List<ChildNewsListForDoetDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForDoetAsync(parentId, "Nonexistent", null, "Inactive", pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllChildNewsForDoetAsync_ShouldReturnEmptyPage_WhenRequestedPageExceedsTotalPages()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 3;
//            int pageSize = 15;
//            var childNews = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "News 1", ParentId = parentId, Status = "Active" }
//        };
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForDoetAsync(parentId, null, null, null))
//                                  .ReturnsAsync(childNews);
//            _mapperMock.Setup(mapper => mapper.Map<List<ChildNewsListForDoetDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(new List<ChildNewsListForDoetDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForDoetAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent news list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count);
//            Assert.AreEqual(1, result.Data.TotalPages);
//        }

//        [Test]
//        public async Task GetAllChildNewsForDoetAsync_ShouldHandleUnauthorizedAccessException()
//        {
//            // Arrange
//            int parentId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForDoetAsync(parentId, null, null, null))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForDoetAsync(parentId, null, null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get child news list: Access denied", result.Message);
//        }

//        [Test]
//        public async Task GetAllChildNewsForDoetAsync_ShouldHandleKeyNotFoundException()
//        {
//            // Arrange
//            int parentId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForDoetAsync(parentId, null, null, null))
//                                  .ThrowsAsync(new KeyNotFoundException("Child News not found"));

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForDoetAsync(parentId, null, null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child News not found", result.Message);
//        }

//        [Test]
//        public async Task GetAllChildNewsForDoetAsync_ShouldHandleGeneralException()
//        {
//            // Arrange
//            int parentId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildNewsForDoetAsync(parentId, null, null, null))
//                                  .ThrowsAsync(new Exception("General error"));

//            // Act
//            var result = await _newsFaqService.GetAllChildNewsForDoetAsync(parentId, null, null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving child news list: General error", result.Message);
//        }
//        #endregion


//        #region GetChildNewsDetailByIdForDoetAsync

//        [Test]
//            public async Task GetChildNewsDetailByIdForDoetAsync_ShouldReturnNewsDetail_WhenNewsExists()
//            {
//                // Arrange
//                var newsId = 1;
//                var news = new NewsFaq { NewsFaqid = newsId, Title = "Sample Child News" };
//                var newsDto = new ChildNewsDetailForDoetDTO { Title = news.Title };

//                _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForDoetAsync(newsId)).ReturnsAsync(news);
//                _mapperMock.Setup(mapper => mapper.Map<ChildNewsDetailForDoetDTO>(news)).Returns(newsDto);

//                // Act
//                var result = await _newsFaqService.GetChildNewsDetailByIdForDoetAsync(newsId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(200, result.StatusCode);
//                Assert.AreEqual(newsDto, result.Data);
//            }

//            [Test]
//            public async Task GetChildNewsDetailByIdForDoetAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//            {
//                // Arrange
//                var newsId = 1;
//                _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForDoetAsync(newsId))
//                    .ThrowsAsync(new KeyNotFoundException("Child news not found"));

//                // Act
//                var result = await _newsFaqService.GetChildNewsDetailByIdForDoetAsync(newsId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Child news not found", result.Message);
//            }


//        [Test]
//        public async Task GetChildNewsDetailByIdForDoetAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var newsId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForDoetAsync(newsId))
//                                  .ThrowsAsync(new Exception("An unexpected error occurred"));

//            // Act
//            var result = await _newsFaqService.GetChildNewsDetailByIdForDoetAsync(newsId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving child news details: An unexpected error occurred", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetChildNewsDetailByIdForDoetAsync_ShouldReturnChildNewsDetails_WhenNewsExists()
//        {
//            // Arrange
//            int newsId = 1;
//            var childNews = new NewsFaq
//            {
//                NewsFaqid = newsId,
//                Title = "Sample News",
//                User = new User { Name = "Admin User" },
//                Role = new Role { Name = "DOET" },
//                NewsFaqcontent = "News Content",
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now,
//                Status = "Active"
//            };
//            var mappedNewsDto = new ChildNewsDetailForDoetDTO
//            {
//                ChildNewsId = newsId,
//                Title = "Sample News",
//                User = "Admin User",
//                ForRole = "DOET",
//                ChildNewscontent = "News Content",
//                Status = "Active"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForDoetAsync(newsId)).ReturnsAsync(childNews);
//            _mapperMock.Setup(mapper => mapper.Map<ChildNewsDetailForDoetDTO>(childNews)).Returns(mappedNewsDto);

//            // Act
//            var result = await _newsFaqService.GetChildNewsDetailByIdForDoetAsync(newsId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child news details retrieved successfully!", result.Message);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual(newsId, result.Data.ChildNewsId);
//            Assert.AreEqual("Sample News", result.Data.Title);
//        }



//        [Test]
//        public async Task GetChildNewsDetailByIdForDoetAsync_ShouldReturnUnauthorized_WhenAccessIsDenied()
//        {
//            // Arrange
//            int newsId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForDoetAsync(newsId))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.GetChildNewsDetailByIdForDoetAsync(newsId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get child news detail: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }



//        #endregion


//        #region UpdateChildNewsForDoetAsync

//        [Test]
//            public async Task UpdateChildNewsForDoetAsync_ShouldReturnSuccess_WhenNewsIsUpdated()
//            {
//                // Arrange
//                var updateNewsDto = new UpdateChildNewsForDoetDTO { ChildNewsId = 1, Title = "Updated Child Title" };
//                var news = new NewsFaq { NewsFaqid = updateNewsDto.ChildNewsId, Title = updateNewsDto.Title };

//                _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildNewsForDoetAsync(It.IsAny<NewsFaq>())).ReturnsAsync(news);
//                _mapperMock.Setup(mapper => mapper.Map<UpdateChildNewsForDoetDTO>(news)).Returns(updateNewsDto);

//                // Act
//                var result = await _newsFaqService.UpdateChildNewsForDoetAsync(updateNewsDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(200, result.StatusCode);
//                Assert.AreEqual("Child News updated successfully!", result.Message);
//                Assert.AreEqual(updateNewsDto.Title, result.Data.Title);
//            }

//            [Test]
//            public async Task UpdateChildNewsForDoetAsync_ShouldReturnNotFound_WhenNewsDoesNotExist()
//            {
//                // Arrange
//                var updateNewsDto = new UpdateChildNewsForDoetDTO { ChildNewsId = 1, Title = "Updated Child Title" };

//                _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                    .ThrowsAsync(new KeyNotFoundException("Child news not found"));

//                // Act
//                var result = await _newsFaqService.UpdateChildNewsForDoetAsync(updateNewsDto);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(404, result.StatusCode);
//                Assert.AreEqual("Child news not found", result.Message);
//            }


//        [Test]
//        public async Task UpdateChildNewsForDoetAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var updateDto = new UpdateChildNewsForDoetDTO { ChildNewsId = 1 };
//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating child news: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task UpdateChildNewsForDoetAsync_ShouldReturnNotFound_WhenChildNewsDoesNotExist()
//        {
//            // Arrange
//            var updateDto = new UpdateChildNewsForDoetDTO { ChildNewsId = 999 };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new KeyNotFoundException("Child News not found"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child News not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateChildNewsForDoetAsync_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
//        {
//            // Arrange
//            var updateDto = new UpdateChildNewsForDoetDTO { ChildNewsId = 1 };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update child news: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }




//        #endregion

//        #region AddChildNewsForDoetAsync


//        [Test]
//        public async Task AddChildNewsForDoetAsync_ShouldReturnSuccess_WhenValidDataIsProvided()
//        {
//            // Arrange
//            var addDto = new AddChildNewsForDoetDTO
//            {
//                UserId = 1,
//                ParentId = 10,
//                Title = "New Child News",
//                ChildNewscontent = "Content of the child news",
//                Image = "image.jpg"
//            };

//            var addedNewsFaq = new NewsFaq
//            {
//                NewsFaqid = 20,
//                UserId = 1,
//                ParentId = 10,
//                Title = "New Child News",
//                NewsFaqcontent = "Content of the child news",
//                Image = "image.jpg",
//                CreatedAt = DateTime.Now,
//                Status = "Active",
//                RoleId = 2
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                                  .ReturnsAsync(addedNewsFaq);

//            _mapperMock.Setup(mapper => mapper.Map<AddChildNewsForDoetDTO>(addedNewsFaq))
//                       .Returns(addDto);

//            // Act
//            var result = await _newsFaqService.AddChildNewsForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Child News added successfully!", result.Message);
//            Assert.AreEqual(addDto.Title, result.Data.Title);
//            Assert.AreEqual(addDto.ChildNewscontent, result.Data.ChildNewscontent);
//            Assert.AreEqual("Active", result.Data.Status);
//        }

//        [Test]
//        public async Task AddChildNewsForDoetAsync_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
//        {
//            // Arrange
//            var addDto = new AddChildNewsForDoetDTO
//            {
//                UserId = 1,
//                ParentId = 10,
//                Title = "New Child News"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Unauthorized access"));

//            // Act
//            var result = await _newsFaqService.AddChildNewsForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add child news: Unauthorized access", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task AddChildNewsForDoetAsync_ShouldHandleServerError()
//        {
//            // Arrange
//            var addDto = new AddChildNewsForDoetDTO
//            {
//                UserId = 1,
//                ParentId = 10,
//                Title = "New Child News"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildNewsForDoetAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new Exception("Server error"));

//            // Act
//            var result = await _newsFaqService.AddChildNewsForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding child news: Server error", result.Message);
//            Assert.IsNull(result.Data);
//        }





//        #endregion

//        #region DeleteChildNewsForDoetAsync


//        [Test]
//        public async Task DeleteChildNewsForDoetAsync_ShouldReturnSuccess_WhenChildNewsIsDeleted()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildNewsForDoetDTO { ChildNewsId = 1 };
//            var news = new NewsFaq { NewsFaqid = deleteDto.ChildNewsId };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildNewsForDoetAsync(deleteDto.ChildNewsId)).ReturnsAsync(news);
//            _mapperMock.Setup(mapper => mapper.Map<DeleteChildNewsForDoetDTO>(news)).Returns(deleteDto);

//            // Act
//            var result = await _newsFaqService.DeleteChildNewsForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child News has been permanently deleted successfully.", result.Message);
//            _newsFaqRepositoryMock.Verify(repo => repo.DeleteChildNewsForDoetAsync(deleteDto.ChildNewsId), Times.Once);
//        }

//        [Test]
//        public async Task DeleteChildNewsForDoetAsync_ShouldReturnNotFound_WhenChildNewsDoesNotExist()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildNewsForDoetDTO { ChildNewsId = 1 };
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildNewsForDoetAsync(deleteDto.ChildNewsId))
//                                  .ThrowsAsync(new KeyNotFoundException("Child news not found"));

//            // Act
//            var result = await _newsFaqService.DeleteChildNewsForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child news not found", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task DeleteChildNewsForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var childNewsId = 1;
//            var deleteDto = new DeleteChildNewsForDoetDTO { ChildNewsId = childNewsId };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildNewsForDoetAsync(childNewsId))
//                .ThrowsAsync(new Exception("Unexpected error during deletion"));

//            // Act
//            var result = await _newsFaqService.DeleteChildNewsForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error permanently deleting child News: Unexpected error during deletion", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        #endregion

//        #region UpdateChildNewsStatusForDoetAsync



//        [Test]
//        public async Task UpdateChildNewsStatusForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var updateDto = new UpdateChildNewsStatusForDoetDTO { ChildNewsId = 1, Status = "Active" };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForDoetAsync(updateDto.ChildNewsId))
//                .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsStatusForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating child News: Unexpected error", result.Message);
//        }


//        [Test]
//        public async Task UpdateChildNewsStatusForDoetAsync_ShouldReturnNotFound_WhenChildNewsDoesNotExist()
//        {
//            // Arrange
//            var updateDto = new UpdateChildNewsStatusForDoetDTO { ChildNewsId = 1, Status = "Active" };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForDoetAsync(updateDto.ChildNewsId))
//                .ThrowsAsync(new KeyNotFoundException("Child News not found"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsStatusForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child News not found", result.Message);
//        }


//        [Test]
//        public async Task UpdateChildNewsStatusForDoetAsync_ShouldReturnForbidden_WhenUnauthorizedAccessOccurs()
//        {
//            // Arrange
//            var updateDto = new UpdateChildNewsStatusForDoetDTO { ChildNewsId = 1, Status = "Active" };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildNewsByIdForDoetAsync(updateDto.ChildNewsId))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.UpdateChildNewsStatusForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while updating child News status: Access denied", result.Message);
//        }



//        #endregion





//        #endregion


//        #region DOET - Parent Faq Management

//        #region GetAllParentFaqForDoetAsync

//        [Test]
//        public async Task GetAllParentFaqForDoetAsync_ShouldReturnPagedParentFaqs_WhenParentFaqsExist()
//        {
//            // Arrange
//            int pageNumber = 1;
//            int pageSize = 15;
//            var parentFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1" },
//            new NewsFaq { NewsFaqid = 2, Title = "FAQ 2" }
//        };

//            var parentFaqDtos = parentFaqs.Select(f => new ParentFaqListForDoetDTO
//            {
//                ParentFaqId = f.NewsFaqid,
//                Title = f.Title
//            }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForDoetAsync(null, null, null)).ReturnsAsync(parentFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForDoetDTO>>(parentFaqs)).Returns(parentFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForDoetAsync(null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(parentFaqDtos.Count, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllParentFaqForDoetAsync_ShouldReturnFilteredResults_WhenTitleAndStatusProvided()
//        {
//            // Arrange
//            string titleFilter = "FAQ";
//            string statusFilter = "Active";
//            int pageNumber = 1;
//            int pageSize = 15;
//            var parentFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1", Status = "Active" },
//            new NewsFaq { NewsFaqid = 2, Title = "FAQ 2", Status = "Inactive" }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForDoetAsync(titleFilter, null, statusFilter))
//                .ReturnsAsync(parentFaqs.Where(f => f.Title.Contains(titleFilter) && f.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList());

//            var mappedFaqDtos = new List<ParentFaqListForDoetDTO> { new ParentFaqListForDoetDTO { ParentFaqId = 1, Title = "FAQ 1", Status = "Active" } };
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForDoetDTO>>(It.IsAny<List<NewsFaq>>())).Returns(mappedFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForDoetAsync(titleFilter, null, statusFilter, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(1, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllParentFaqForDoetAsync_ShouldReturnEmptyData_WhenRequestedPageExceedsTotalPages()
//        {
//            // Arrange
//            int pageNumber = 10; // Exceeds total pages
//            int pageSize = 15;
//            var parentFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1" }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForDoetAsync(null, null, null)).ReturnsAsync(parentFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForDoetDTO>>(parentFaqs)).Returns(new List<ParentFaqListForDoetDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForDoetAsync(null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(1, result.Data.TotalPages);
//            Assert.AreEqual(0, result.Data.Items.Count);
//        }


//        [Test]
//        public async Task GetAllParentFaqForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForDoetAsync(null, null, null))
//                .ThrowsAsync(new Exception("Unexpected error during retrieval"));

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForDoetAsync(null, null, null, 1, 15);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving parent faq list: Unexpected error during retrieval", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task GetAllParentFaqForDoetAsync_ShouldReturnResults_FilteredByStatus()
//        {
//            // Arrange
//            string statusFilter = "Active";
//            int pageNumber = 1;
//            int pageSize = 15;
//            var parentFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1", Status = "Active" },
//            new NewsFaq { NewsFaqid = 2, Title = "FAQ 2", Status = "Inactive" }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForDoetAsync(null, null, statusFilter))
//                .ReturnsAsync(parentFaqs.Where(f => f.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList());

//            var mappedFaqDtos = new List<ParentFaqListForDoetDTO> { new ParentFaqListForDoetDTO { ParentFaqId = 1, Title = "FAQ 1", Status = "Active" } };
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForDoetDTO>>(It.IsAny<List<NewsFaq>>())).Returns(mappedFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForDoetAsync(null, null, statusFilter, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(1, result.Data.Items.Count);
//        }

//        [Test]
//        public async Task GetAllParentFaqForDoetAsync_ShouldReturnOnlyFirstPage_WhenMultiplePagesExist()
//        {
//            // Arrange
//            int pageNumber = 1;
//            int pageSize = 1; // Create multiple pages
//            var parentFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1" },
//            new NewsFaq { NewsFaqid = 2, Title = "FAQ 2" }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForDoetAsync(null, null, null)).ReturnsAsync(parentFaqs);
//            var mappedFaqDtos = new List<ParentFaqListForDoetDTO> { new ParentFaqListForDoetDTO { ParentFaqId = 1, Title = "FAQ 1" } };
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForDoetDTO>>(It.IsAny<List<NewsFaq>>()))
//                .Returns(mappedFaqDtos);

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForDoetAsync(null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(1, result.Data.Items.Count);
//            Assert.AreEqual(2, result.Data.TotalPages); // Total pages based on pageSize
//        }


//        [Test]
//        public async Task GetAllParentFaqForDoetAsync_ShouldHandleSinglePageExactlyFull()
//        {
//            // Arrange
//            int pageNumber = 1;
//            int pageSize = 2;
//            var parentFaqs = new List<NewsFaq>
//        {
//            new NewsFaq { NewsFaqid = 1, Title = "FAQ 1" },
//            new NewsFaq { NewsFaqid = 2, Title = "FAQ 2" }
//        };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllParentFaqForDoetAsync(null, null, null)).ReturnsAsync(parentFaqs);
//            _mapperMock.Setup(mapper => mapper.Map<List<ParentFaqListForDoetDTO>>(parentFaqs)).Returns(parentFaqs.Select(f => new ParentFaqListForDoetDTO { ParentFaqId = f.NewsFaqid, Title = f.Title }).ToList());

//            // Act
//            var result = await _newsFaqService.GetAllParentFaqForDoetAsync(null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Items.Count);
//            Assert.AreEqual(1, result.Data.TotalPages); // Exact page fit
//        }

//        #endregion


//        #region GetParentFaqDetailByIdForDoetAsync

//        [Test]
//        public async Task GetParentFaqDetailByIdForDoetAsync_ShouldReturnFaqDetail_WhenFaqExists()
//        {
//            // Arrange
//            var faqId = 1;
//            var faq = new NewsFaq { NewsFaqid = faqId, Title = "Sample Faq" };
//            var faqDto = new ParentFaqDetailForDoetDTO { Title = faq.Title };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForDoetAsync(faqId)).ReturnsAsync(faq);
//            _mapperMock.Setup(mapper => mapper.Map<ParentFaqDetailForDoetDTO>(faq)).Returns(faqDto);

//            // Act
//            var result = await _newsFaqService.GetParentFaqDetailByIdForDoetAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(faqDto, result.Data);
//        }

//        [Test]
//        public async Task GetParentFaqDetailByIdForDoetAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            var faqId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForDoetAsync(faqId))
//                                  .ThrowsAsync(new KeyNotFoundException("FAQ not found"));

//            // Act
//            var result = await _newsFaqService.GetParentFaqDetailByIdForDoetAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("FAQ not found", result.Message);
//        }


//        [Test]
//        public async Task GetParentFaqDetailByIdForDoetAsync_ShouldReturnForbidden_WhenUnauthorizedAccessOccurs()
//        {
//            // Arrange
//            int faqId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForDoetAsync(faqId)).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.GetParentFaqDetailByIdForDoetAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get Parent faq detail: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetParentFaqDetailByIdForDoetAsync_ShouldReturnServerError_WhenUnhandledExceptionOccurs()
//        {
//            // Arrange
//            int faqId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForDoetAsync(faqId)).ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.GetParentFaqDetailByIdForDoetAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving Parent faq details: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        #endregion

//        #region AddParentFaqForDoetAsync

//        [Test]
//        public async Task AddParentFaqForDoetAsync_ShouldReturnSuccess_WhenFaqIsAdded()
//        {
//            // Arrange
//            var addParentFaqDto = new AddParentFaqForDoetDTO
//            {
//                Title = "New Faq",
//                ParentFaqcontent = "Content of the faq",
//                UserId = 1
//            };

//            var faq = new NewsFaq { NewsFaqid = 1, Title = addParentFaqDto.Title, NewsFaqcontent = addParentFaqDto.ParentFaqcontent, UserId = addParentFaqDto.UserId };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentFaqForDoetAsync(It.IsAny<NewsFaq>())).ReturnsAsync(faq);

//            // Act
//            var result = await _newsFaqService.AddParentFaqForDoetAsync(addParentFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Parent Faq added successfully!", result.Message);
//            Assert.AreEqual(addParentFaqDto.Title, result.Data.Title);
//        }


//        [Test]
//        public async Task AddParentFaqForDoetAsync_ShouldReturnForbidden_WhenUnauthorizedAccessOccurs()
//        {
//            // Arrange
//            var addDto = new AddParentFaqForDoetDTO
//            {
//                UserId = 1,
//                Title = "Unauthorized FAQ",
//                ParentFaqcontent = "Unauthorized content",
//                Image = "unauth.jpg",
//                ForRoleId = 2
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentFaqForDoetAsync(It.IsAny<NewsFaq>())).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.AddParentFaqForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add parent faq: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task AddParentFaqForDoetAsync_ShouldReturnServerError_WhenUnhandledExceptionOccurs()
//        {
//            // Arrange
//            var addDto = new AddParentFaqForDoetDTO
//            {
//                UserId = 1,
//                Title = "FAQ with Error",
//                ParentFaqcontent = "Error content",
//                Image = "error.jpg",
//                ForRoleId = 2
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddParentFaqForDoetAsync(It.IsAny<NewsFaq>())).ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.AddParentFaqForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding parent faq: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        #endregion

//        #region UpdateParentFaqForDoetAsync

//        [Test]
//        public async Task UpdateParentFaqForDoetAsync_ShouldReturnSuccess_WhenFaqIsUpdated()
//        {
//            // Arrange
//            var updateFaqDto = new UpdateParentFaqForDoetDTO { ParentFaqId = 1, Title = "Updated Title" };
//            var faq = new NewsFaq { NewsFaqid = updateFaqDto.ParentFaqId, Title = updateFaqDto.Title };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqForDoetAsync(It.IsAny<NewsFaq>())).ReturnsAsync(faq);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateParentFaqForDoetDTO>(faq)).Returns(updateFaqDto);

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqForDoetAsync(updateFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent Faq updated successfully!", result.Message);
//            Assert.AreEqual(updateFaqDto.Title, result.Data.Title);
//        }


//        [Test]
//        public async Task UpdateParentFaqForDoetAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            var updateParentFaqDto = new UpdateParentFaqForDoetDTO
//            {
//                ParentFaqId = 1,
//                Title = "Nonexistent FAQ"
//            };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqForDoetAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new KeyNotFoundException("FAQ not found"));

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqForDoetAsync(updateParentFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("FAQ not found", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateParentFaqForDoetAsync_ShouldReturnForbidden_WhenUnauthorizedAccessOccurs()
//        {
//            // Arrange
//            var updateDto = new UpdateParentFaqForDoetDTO { ParentFaqId = 1, Title = "Unauthorized Title" };
//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqForDoetAsync(It.IsAny<NewsFaq>())).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update parent faq: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateParentFaqForDoetAsync_ShouldReturnServerError_WhenUnhandledExceptionOccurs()
//        {
//            // Arrange
//            var updateDto = new UpdateParentFaqForDoetDTO { ParentFaqId = 1, Title = "Error Title" };
//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqForDoetAsync(It.IsAny<NewsFaq>())).ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating parent faq: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        #endregion

//        #region DeleteParentFaqForDoetAsync

//        [Test]
//        public async Task DeleteParentFaqForDoetAsync_ShouldReturnSuccess_WhenFaqIsDeleted()
//        {
//            // Arrange
//            var deleteDto = new DeleteParentFaqForDoetDTO { ParentFaqId = 1 };
//            var faq = new NewsFaq { NewsFaqid = deleteDto.ParentFaqId };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentFaqForDoetAsync(deleteDto.ParentFaqId)).ReturnsAsync(faq);
//            _mapperMock.Setup(mapper => mapper.Map<DeleteParentFaqForDoetDTO>(faq)).Returns(deleteDto);

//            // Act
//            var result = await _newsFaqService.DeleteParentFaqForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent Faq has been permanently deleted successfully.", result.Message);
//        }


//        [Test]
//        public async Task DeleteParentFaqForDoetAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            var deleteDto = new DeleteParentFaqForDoetDTO { ParentFaqId = 1 };
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentFaqForDoetAsync(deleteDto.ParentFaqId))
//                                  .ThrowsAsync(new KeyNotFoundException("FAQ not found"));

//            // Act
//            var result = await _newsFaqService.DeleteParentFaqForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("FAQ not found", result.Message);
//        }

//        [Test]
//        public async Task DeleteParentFaqForDoetAsync_ShouldReturnNotFound_WhenParentFaqDoesNotExist()
//        {
//            // Arrange
//            var deleteDto = new DeleteParentFaqForDoetDTO { ParentFaqId = 99 };
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentFaqForDoetAsync(deleteDto.ParentFaqId))
//                .ThrowsAsync(new KeyNotFoundException("Parent Faq not found in the list."));

//            // Act
//            var result = await _newsFaqService.DeleteParentFaqForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent Faq not found in the list.", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteParentFaqForDoetAsync_ShouldReturnForbidden_WhenUnauthorizedAccessOccurs()
//        {
//            // Arrange
//            var deleteDto = new DeleteParentFaqForDoetDTO { ParentFaqId = 1 };
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentFaqForDoetAsync(deleteDto.ParentFaqId))
//                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.DeleteParentFaqForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while hard delete Parent Faq: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteParentFaqForDoetAsync_ShouldReturnServerError_WhenUnhandledExceptionOccurs()
//        {
//            // Arrange
//            var deleteDto = new DeleteParentFaqForDoetDTO { ParentFaqId = 1 };
//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteParentFaqForDoetAsync(deleteDto.ParentFaqId))
//                .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.DeleteParentFaqForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error permanently deleting Parent Faq: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        #endregion

//        #region UpdateParentFaqStatusForDoetAsync

//        [Test]
//        public async Task UpdateParentFaqStatusForDoetAsync_ShouldReturnSuccess_WhenStatusIsUpdated()
//        {
//            // Arrange
//            var updateStatusDto = new UpdateParentFaqStatusForDoetDTO { ParentFaqId = 1, Status = "Active" };
//            var faq = new NewsFaq { NewsFaqid = updateStatusDto.ParentFaqId, Status = updateStatusDto.Status };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqStatusForDoetAsync(It.IsAny<NewsFaq>())).ReturnsAsync(faq);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateParentFaqStatusForDoetDTO>(faq)).Returns(updateStatusDto);

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqStatusForDoetAsync(updateStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent Faq updated successfully!", result.Message);
//        }


//        [Test]
//        public async Task UpdateParentFaqStatusForDoetAsync_ShouldReturnNotFound_WhenParentFaqDoesNotExist()
//        {
//            // Arrange
//            var updateDto = new UpdateParentFaqStatusForDoetDTO
//            {
//                ParentFaqId = 99,
//                Status = "Active"
//            };
//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateParentFaqStatusForDoetAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new KeyNotFoundException("Parent FAQ not found"));

//            // Act
//            var result = await _newsFaqService.UpdateParentFaqStatusForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent FAQ not found", result.Message);
//        }



//        #endregion



//        #endregion


//        #region DOET - Child Faq Management

//        #region GetAllChildFaqForDoetAsync

//        [Test]
//        public async Task GetAllChildFaqForDoetAsync_ShouldReturnPagedResults_WhenFaqsExist()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15; // default pageSize
//            var childFaqs = Enumerable.Range(1, 20).Select(i => new NewsFaq
//            {
//                NewsFaqid = i,
//                Title = $"FAQ {i}",
//                ParentId = parentId,
//                Status = i % 2 == 0 ? "Active" : "Inactive"
//            }).ToList();

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForDoetAsync(parentId, null, null, null))
//                                  .ReturnsAsync(childFaqs);

//            var mappedFaqs = childFaqs.Select(faq => new ChildFaqListForDoetDTO
//            {
//                ChildFaqId = faq.NewsFaqid,
//                Title = faq.Title,
//                Status = faq.Status,
//                ParentId = faq.ParentId
//            }).ToList();

//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForDoetDTO>>(childFaqs))
//                       .Returns(mappedFaqs);

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForDoetAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(15, result.Data.Items.Count); // Should be 15 as per pageSize
//            Assert.AreEqual(2, result.Data.TotalPages);
//            Assert.AreEqual(pageNumber, result.Data.CurrentPage);
//        }

//        [Test]
//        public async Task GetAllChildFaqForDoetAsync_ShouldReturnEmptyList_WhenNoFaqsExist()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForDoetAsync(parentId, null, null, null))
//                                  .ReturnsAsync(new List<NewsFaq>());

//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForDoetDTO>>(It.IsAny<IEnumerable<NewsFaq>>()))
//                       .Returns(new List<ChildFaqListForDoetDTO>());

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForDoetAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count); // Replace Assert.IsEmpty with Assert.AreEqual(0, ...)
//            Assert.AreEqual(0, result.Data.TotalCount);
//            Assert.AreEqual(1, result.Data.TotalPages);
//        }

//        [Test]
//        public async Task GetAllChildFaqForDoetAsync_ShouldHandlePageExceedingTotalPages_ReturnEmptyPage()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 3; // Exceeds total pages
//            int pageSize = 15;
//            var childFaqs = new List<NewsFaq>
//    {
//        new NewsFaq { NewsFaqid = 1, Title = "FAQ 1", ParentId = parentId, Status = "Active" }
//    };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForDoetAsync(parentId, null, null, null))
//                                  .ReturnsAsync(childFaqs);

//            var mappedFaqs = childFaqs.Select(faq => new ChildFaqListForDoetDTO
//            {
//                ChildFaqId = faq.NewsFaqid,
//                Title = faq.Title,
//                Status = faq.Status,
//                ParentId = faq.ParentId
//            }).ToList();

//            _mapperMock.Setup(mapper => mapper.Map<List<ChildFaqListForDoetDTO>>(childFaqs))
//                       .Returns(mappedFaqs);

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForDoetAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Parent faq list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Items.Count); // Replace Assert.IsEmpty with Assert.AreEqual(0, ...)
//            Assert.AreEqual(1, result.Data.TotalPages); // Only 1 page available
//        }


//        [Test]
//        public async Task GetAllChildFaqForDoetAsync_ShouldReturn403_WhenUnauthorizedAccessExceptionIsThrown()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForDoetAsync(parentId, null, null, null))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForDoetAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while get child faq list: Access denied", result.Message);
//        }

//        [Test]
//        public async Task GetAllChildFaqForDoetAsync_ShouldReturn500_WhenGenericExceptionIsThrown()
//        {
//            // Arrange
//            int parentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;

//            _newsFaqRepositoryMock.Setup(repo => repo.GetAllChildFaqForDoetAsync(parentId, null, null, null))
//                                  .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.GetAllChildFaqForDoetAsync(parentId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving child faq list: Unexpected error", result.Message);
//        }

//        #endregion

//        #region GetChildFaqDetailByIdForDoetAsync

//        [Test]
//        public async Task GetChildFaqDetailByIdForDoetAsync_ShouldReturnChildFaqDetail_WhenFaqExists()
//        {
//            // Arrange
//            var faqId = 1;
//            var faq = new NewsFaq { NewsFaqid = faqId, Title = "Sample FAQ" };
//            var faqDto = new ChildFaqDetailForDoetDTO { Title = faq.Title };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForDoetAsync(faqId)).ReturnsAsync(faq);
//            _mapperMock.Setup(mapper => mapper.Map<ChildFaqDetailForDoetDTO>(faq)).Returns(faqDto);

//            // Act
//            var result = await _newsFaqService.GetChildFaqDetailByIdForDoetAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(faqDto, result.Data);
//        }

//        [Test]
//        public async Task GetChildFaqDetailByIdForDoetAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            var faqId = 1;
//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForDoetAsync(faqId))
//                .ThrowsAsync(new KeyNotFoundException("Child FAQ not found"));

//            // Act
//            var result = await _newsFaqService.GetChildFaqDetailByIdForDoetAsync(faqId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child FAQ not found", result.Message);
//        }

       

//            [Test]
//            public async Task GetChildFaqDetailByIdForDoetAsync_ShouldReturnUnauthorized_WhenAccessIsDenied()
//            {
//                // Arrange
//                int faqId = 1;
//                _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForDoetAsync(faqId))
//                                      .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//                // Act
//                var result = await _newsFaqService.GetChildFaqDetailByIdForDoetAsync(faqId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(403, result.StatusCode);
//                Assert.AreEqual("Access denied while get child faq detail: Access denied", result.Message);
//                Assert.IsNull(result.Data);
//            }

//            [Test]
//            public async Task GetChildFaqDetailByIdForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//            {
//                // Arrange
//                int faqId = 1;
//                _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForDoetAsync(faqId))
//                                      .ThrowsAsync(new Exception("Unexpected error"));

//                // Act
//                var result = await _newsFaqService.GetChildFaqDetailByIdForDoetAsync(faqId);

//                // Assert
//                Assert.IsNotNull(result);
//                Assert.AreEqual(500, result.StatusCode);
//                Assert.AreEqual("Error retrieving child faq details: Unexpected error", result.Message);
//                Assert.IsNull(result.Data);
//            }
        



//        #endregion


//        #region AddChildFaqForDoetAsync

//        [Test]
//        public async Task AddChildFaqForDoetAsync_ShouldReturnSuccess_WhenFaqIsAdded()
//        {
//            // Arrange
//            var addFaqDto = new AddChildFaqForDoetDTO { Title = "New FAQ", ChildFaqcontent = "FAQ Content", ParentId = 1 };
//            var faq = new NewsFaq { NewsFaqid = 1, Title = addFaqDto.Title, NewsFaqcontent = addFaqDto.ChildFaqcontent, ParentId = addFaqDto.ParentId };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildFaqForAdminAsync(It.IsAny<NewsFaq>())).ReturnsAsync(faq);

//            // Act
//            var result = await _newsFaqService.AddChildFaqForDoetAsync(addFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(201, result.StatusCode);
//            Assert.AreEqual("Child Faq added successfully!", result.Message);
//            Assert.AreEqual(addFaqDto.Title, result.Data.Title);
//        }


//        [Test]
//        public async Task AddChildFaqForDoetAsync_ShouldReturnForbidden_WhenAccessIsDenied()
//        {
//            // Arrange
//            var addDto = new AddChildFaqForDoetDTO { UserId = 1, ParentId = 2, Title = "Unauthorized FAQ" };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.AddChildFaqForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while add child faq: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }


//        [Test]
//        public async Task AddChildFaqForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var addDto = new AddChildFaqForDoetDTO { UserId = 1, ParentId = 2, Title = "FAQ with Server Error" };

//            _newsFaqRepositoryMock.Setup(repo => repo.AddChildFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.AddChildFaqForDoetAsync(addDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error adding child faq: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        #endregion

//        #region UpdateChildFaqForDoetAsync

//        [Test]
//        public async Task UpdateChildFaqForDoetAsync_ShouldReturnSuccess_WhenFaqIsUpdated()
//        {
//            // Arrange
//            var updateFaqDto = new UpdateChildFaqForDoetDTO { ChildFaqId = 1, Title = "Updated Title" };
//            var faq = new NewsFaq { NewsFaqid = updateFaqDto.ChildFaqId, Title = updateFaqDto.Title };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildFaqForAdminAsync(It.IsAny<NewsFaq>())).ReturnsAsync(faq);
//            _mapperMock.Setup(mapper => mapper.Map<UpdateChildFaqForDoetDTO>(faq)).Returns(updateFaqDto);

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqForDoetAsync(updateFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child Faq updated successfully!", result.Message);
//            Assert.AreEqual(updateFaqDto.Title, result.Data.Title);
//        }

//        [Test]
//        public async Task UpdateChildFaqForDoetAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            var updateFaqDto = new UpdateChildFaqForDoetDTO { ChildFaqId = 1, Title = "Updated Title" };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                .ThrowsAsync(new KeyNotFoundException("Child FAQ not found"));

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqForDoetAsync(updateFaqDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child FAQ not found", result.Message);
//        }

//        [Test]
//        public async Task UpdateChildFaqForDoetAsync_ShouldReturnForbidden_WhenAccessIsDenied()
//        {
//            // Arrange
//            var updateDto = new UpdateChildFaqForDoetDTO { ChildFaqId = 1, Title = "Unauthorized Update" };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while update child faq: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task UpdateChildFaqForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var updateDto = new UpdateChildFaqForDoetDTO { ChildFaqId = 1, Title = "FAQ with Server Error" };

//            _newsFaqRepositoryMock.Setup(repo => repo.UpdateChildFaqForAdminAsync(It.IsAny<NewsFaq>()))
//                                  .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqForDoetAsync(updateDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error updating child faq: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        #endregion

//        #region DeleteChildFaqForDoetAsync

//        [Test]
//        public async Task DeleteChildFaqForDoetAsync_ShouldReturnSuccess_WhenFaqIsDeleted()
//        {
//            // Arrange
//            var faqId = 1;
//            var faq = new NewsFaq { NewsFaqid = faqId };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForDoetAsync(faqId)).ReturnsAsync(faq);

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForDoetAsync(new DeleteChildFaqForDoetDTO { ChildFaqId = faqId });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child Faq has been permanently deleted successfully.", result.Message);
//        }

//        [Test]
//        public async Task DeleteChildFaqForDoetAsync_ShouldReturnNotFound_WhenFaqDoesNotExist()
//        {
//            // Arrange
//            var faqId = 1;

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForDoetAsync(faqId))
//                .ThrowsAsync(new KeyNotFoundException("Child FAQ not found"));

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForDoetAsync(new DeleteChildFaqForDoetDTO { ChildFaqId = faqId });

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child FAQ not found", result.Message);
//        }

//        [Test]
//        public async Task DeleteChildFaqForDoetAsync_ShouldDeleteSuccessfully_WhenChildFaqExists()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildFaqForDoetDTO { ChildFaqId = 1 };
//            var deletedChildFaq = new NewsFaq { NewsFaqid = 1, DeletedAt = DateTime.Now };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForDoetAsync(deleteDto.ChildFaqId))
//                                  .ReturnsAsync(deletedChildFaq);

//            _mapperMock.Setup(mapper => mapper.Map<DeleteChildFaqForDoetDTO>(deletedChildFaq))
//                       .Returns(deleteDto);

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Child Faq has been permanently deleted successfully.", result.Message);
//            Assert.AreEqual(deleteDto.ChildFaqId, result.Data.ChildFaqId);
//        }


//        [Test]
//        public async Task DeleteChildFaqForDoetAsync_ShouldReturnForbidden_WhenAccessIsDenied()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildFaqForDoetDTO { ChildFaqId = 1 };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForDoetAsync(deleteDto.ChildFaqId))
//                                  .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(403, result.StatusCode);
//            Assert.AreEqual("Access denied while hard delete child Faq: Access denied", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task DeleteChildFaqForDoetAsync_ShouldReturnServerError_WhenExceptionOccurs()
//        {
//            // Arrange
//            var deleteDto = new DeleteChildFaqForDoetDTO { ChildFaqId = 1 };

//            _newsFaqRepositoryMock.Setup(repo => repo.DeleteChildFaqForDoetAsync(deleteDto.ChildFaqId))
//                                  .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _newsFaqService.DeleteChildFaqForDoetAsync(deleteDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error permanently deleting child Faq: Unexpected error", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        #endregion

//        #region UpdateChildFaqStatusForDoetAsync



//        [Test]
//        public async Task UpdateChildFaqStatusForDoetAsync_ShouldReturnNotFound_WhenChildFaqDoesNotExist()
//        {
//            // Arrange
//            var updateStatusDto = new UpdateChildFaqStatusForDoetDTO { ChildFaqId = 1, Status = "Active" };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForDoetAsync(updateStatusDto.ChildFaqId))
//                .ThrowsAsync(new KeyNotFoundException("Child Faq not found"));

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqStatusForDoetAsync(updateStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Child Faq not found", result.Message);
//        }

//        [Test]
//        public async Task UpdateChildFaqStatusForDoetAsync_ShouldReturnNotFound_WhenParentFaqDoesNotExist()
//        {
//            // Arrange
//            var updateStatusDto = new UpdateChildFaqStatusForDoetDTO { ChildFaqId = 1, Status = "Active" };
//            var childFaq = new NewsFaq { NewsFaqid = updateStatusDto.ChildFaqId, ParentId = 2 };

//            _newsFaqRepositoryMock.Setup(repo => repo.GetChildFaqByIdForDoetAsync(updateStatusDto.ChildFaqId)).ReturnsAsync(childFaq);
//            _newsFaqRepositoryMock.Setup(repo => repo.GetParentFaqByIdForDoetAsync(childFaq.ParentId.Value))
//                .ThrowsAsync(new KeyNotFoundException("Parent Faq not found"));

//            // Act
//            var result = await _newsFaqService.UpdateChildFaqStatusForDoetAsync(updateStatusDto);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(404, result.StatusCode);
//            Assert.AreEqual("Parent Faq not found", result.Message);
//        }




//        #endregion




//        #endregion

//        #endregion






//    }
//}
