using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OJTEDU.Api.Configuration;
using OJTEDU.WebAPI.Controllers.Dean;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.UnitTests.Controllers.Dean
{
    [TestFixture]
    public class WorkingReportControllerTests
    {
        private Mock<IWorkingReportService> _workingReportServiceMock;
        private WorkingReportController _controller;

        [SetUp]
        public void SetUp()
        {
            _workingReportServiceMock = new Mock<IWorkingReportService>();
            _controller = new WorkingReportController(_workingReportServiceMock.Object);
        }

        #region GetWorkingReportsByStudentIdAsync Tests

        [Test]
        public async Task GetWorkingReportsByStudentIdAsync_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var serviceResponse = new DataResponse<WorkingReportResponseDTO>
            {
                Data = new WorkingReportResponseDTO
                {
                    LecturerName = "Lecturer A",
                    StudentName = "Student B",
                    Week = "01/01 to 07/01",
                    WorkingReports = new List<WorkingReportDto>
                    {
                        new WorkingReportDto { WorkingReportId = "1", ReportTitle = "Report 1" },
                        new WorkingReportDto { WorkingReportId = "2", ReportTitle = "Report 2" }
                    }
                },
                Message = "Success",
                StatusCode = 200
            };

            _workingReportServiceMock
                .Setup(s => s.GetWorkingReportsByStudentIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetWorkingReportsByStudentIdAsync(1, null, null, null, null);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var apiResponse = (ApiResponse<WorkingReportResponseDTO>)okResult.Value;

            Assert.AreEqual(200, serviceResponse.StatusCode);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual("Lecturer A", apiResponse.Data.LecturerName);
        }

        [Test]
        public async Task GetWorkingReportsByStudentIdAsync_ShouldReturnNotFound_WhenNoDataFound()
        {
            // Arrange
            var serviceResponse = new DataResponse<WorkingReportResponseDTO>
            {
                Data = null,
                Message = "No working reports found.",
                StatusCode = 404
            };

            _workingReportServiceMock
                .Setup(s => s.GetWorkingReportsByStudentIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetWorkingReportsByStudentIdAsync(1, null, null, null, null);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(404, objectResult.StatusCode);
        }

        [Test]
        public async Task GetWorkingReportsByStudentIdAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _workingReportServiceMock
                .Setup(s => s.GetWorkingReportsByStudentIdAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool?>(), It.IsAny<string>(), It.IsAny<int?>()))
                .ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _controller.GetWorkingReportsByStudentIdAsync(1, null, null, null, null);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        #endregion

        #region UpdateWorkingReport Tests

        [Test]
        public async Task UpdateWorkingReport_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var serviceResponse = new DataResponse<string>
            {
                Data = "Report updated successfully.",
                Message = "Success",
                StatusCode = 200
            };

            _workingReportServiceMock
                .Setup(s => s.UpdateWorkingReportAsync(It.IsAny<GiveFeedbackOrScoreDto>()))
                .ReturnsAsync(serviceResponse);

            var dto = new GiveFeedbackOrScoreDto { WorkingReportId = 1, Feedback = "Good work", Score = 9 };

            // Act
            var result = await _controller.UpdateWorkingReport(dto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var apiResponse = (ApiResponse<string>)okResult.Value;

            Assert.AreEqual("Report updated successfully.", apiResponse.Data);
        }

        [Test]
        public async Task UpdateWorkingReport_ShouldReturnNotFound_WhenUpdateFails()
        {
            // Arrange
            var serviceResponse = new DataResponse<string>
            {
                Data = null,
                Message = "Working report not found.",
                StatusCode = 404
            };

            _workingReportServiceMock
                .Setup(s => s.UpdateWorkingReportAsync(It.IsAny<GiveFeedbackOrScoreDto>()))
                .ReturnsAsync(serviceResponse);

            var dto = new GiveFeedbackOrScoreDto { WorkingReportId = 1, Feedback = "Good work", Score = 9 };

            // Act
            var result = await _controller.UpdateWorkingReport(dto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(404, objectResult.StatusCode);
        }

        #endregion

        #region GetWeeksForStudentAsync Tests

        [Test]
        public async Task GetWeeksForStudentAsync_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var serviceResponse = new DataResponse<List<string>>
            {
                Data = new List<string> { "01/01 to 07/01", "08/01 to 14/01" },
                Message = "Success",
                StatusCode = 200
            };

            _workingReportServiceMock
                .Setup(s => s.GetWeeksForStudentAsync(It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetWeeksForStudentAsync(1, null);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var apiResponse = (ApiResponse<List<string>>)okResult.Value;

            Assert.AreEqual(200, serviceResponse.StatusCode);
            Assert.AreEqual(2, apiResponse.Data.Count);
        }

        [Test]
        public async Task GetWeeksForStudentAsync_ShouldReturnNotFound_WhenNoWeeksFound()
        {
            // Arrange
            var serviceResponse = new DataResponse<List<string>>
            {
                Data = null,
                Message = "No weeks found.",
                StatusCode = 404
            };

            _workingReportServiceMock
                .Setup(s => s.GetWeeksForStudentAsync(It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetWeeksForStudentAsync(1, null);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.AreEqual(404, objectResult.StatusCode);
        }

        #endregion
    }
}
