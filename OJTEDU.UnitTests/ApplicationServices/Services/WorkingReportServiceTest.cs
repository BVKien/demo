using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.UnitTests.ApplicationServices.Services
{
    [TestFixture]
    public class WorkingReportServiceTests
    {
        private Mock<IWorkingReportRepository> _repositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private WorkingReportService _service;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IWorkingReportRepository>();
            _mapperMock = new Mock<IMapper>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            // Mock HttpContextAccessor
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Dean")
            }));

            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(mockHttpContext);

            _service = new WorkingReportService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Test]
        public async Task GetWeeksForStudentAsync_ShouldReturnWeeks_WhenStudentExists()
        {
            // Arrange
            int studentId = 1;
            int year = 2024;
            var weeks = new List<string> { "01/04 to 07/04", "08/04 to 14/04" };

            _repositoryMock.Setup(r => r.GetWeeksForStudentAsync(studentId, year))
                .ReturnsAsync(weeks);

            // Act
            var result = await _service.GetWeeksForStudentAsync(studentId, year);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Weeks retrieved successfully!", result.Message);
            Assert.AreEqual(weeks, result.Data);
        }

        [Test]
        public async Task GetWeeksForStudentAsync_ShouldReturnError_WhenNoWeeksFound()
        {
            // Arrange
            int studentId = 1;
            int year = 2024;

            _repositoryMock.Setup(r => r.GetWeeksForStudentAsync(studentId, year))
                .ReturnsAsync((List<string>)null);

            // Act
            var result = await _service.GetWeeksForStudentAsync(studentId, year);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("No weeks found for the specified student and year.", result.Message);
        }

        [Test]
        public async Task GetWorkingReportsByStudentIdAsync_ShouldReturnReports_WhenValidRequest()
        {
            // Arrange
            int studentId = 1;
            string week = "01/04 to 07/04";
            var student = new Student { User = new User { Name = "John Doe" }, Lecturer = new User { Name = "Lecturer" } };
            var workingReports = new List<WorkingReport>
            {
                new WorkingReport { WorkingReportId = 1, ReportTitle = "Report 1", CreatedAt = DateTime.Now },
                new WorkingReport { WorkingReportId = 2, ReportTitle = "Report 2", CreatedAt = DateTime.Now }
            };

            _repositoryMock.Setup(r => r.GetStudentDetailsByIdAsync(studentId, 1, "Dean"))
                .ReturnsAsync(student);

            _repositoryMock.Setup(r => r.GetWorkingReportsByStudentIdAsync(studentId, 1, "Dean", null, null, week, null))
                .ReturnsAsync(workingReports);

            _mapperMock.Setup(m => m.Map<List<WorkingReportDto>>(workingReports))
                .Returns(new List<WorkingReportDto>
                {
                    new WorkingReportDto { WorkingReportId = "1", ReportTitle = "Report 1" },
                    new WorkingReportDto { WorkingReportId = "2", ReportTitle = "Report 2" }
                });

            // Act
            var result = await _service.GetWorkingReportsByStudentIdAsync(studentId, null, null, week, null);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Working reports retrieved successfully.", result.Message);
            Assert.AreEqual("John Doe", result.Data.StudentName);
            Assert.AreEqual("Lecturer", result.Data.LecturerName);
            Assert.AreEqual(2, result.Data.WorkingReports.Count);
        }

        [Test]
        public async Task GetWorkingReportsByStudentIdAsync_ShouldReturnError_WhenStudentNotFound()
        {
            // Arrange
            int studentId = 1;

            _repositoryMock.Setup(r => r.GetStudentDetailsByIdAsync(studentId, 1, "Dean"))
                .ReturnsAsync((Student)null);

            // Act
            var result = await _service.GetWorkingReportsByStudentIdAsync(studentId, null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(403, result.StatusCode);
            Assert.AreEqual("Access denied or student not found.", result.Message);
        }

        [Test]
        public async Task UpdateWorkingReportAsync_ShouldReturnSuccess_WhenValidRequest()
        {
            // Arrange
            var dto = new GiveFeedbackOrScoreDto
            {
                WorkingReportId = 1,
                Feedback = "Good Job",
                Score = 8
            };

            _repositoryMock.Setup(r => r.UpdateWorkingReportAsync(dto.WorkingReportId, 1, "Dean", dto.Feedback, dto.Score))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateWorkingReportAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Report updated successfully.", result.Message);
        }

        [Test]
        public async Task UpdateWorkingReportAsync_ShouldReturnError_WhenReportNotFound()
        {
            // Arrange
            var dto = new GiveFeedbackOrScoreDto
            {
                WorkingReportId = 1,
                Feedback = "Good Job",
                Score = 8
            };

            _repositoryMock.Setup(r => r.UpdateWorkingReportAsync(dto.WorkingReportId, 1, "Dean", dto.Feedback, dto.Score))
                .ReturnsAsync(false);

            // Act
            var result = await _service.UpdateWorkingReportAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Failed to update report. Working report not found or access denied.", result.Message);
        }
    }
}
