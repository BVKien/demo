using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OJTEDU.Domain.Entities;
using OJTEDU.Infrastructure.Data;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OJTEDU.UnitTests.Repositories
{
    [TestFixture]
    public class WorkingReportRepositoryTests
    {
        private OJTEDU_DB_V1Context _context;
        private WorkingReportRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<OJTEDU_DB_V1Context>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new OJTEDU_DB_V1Context(options);
            _repository = new WorkingReportRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetDeanByUserIdAsync Tests

        [Test]
        public async Task GetDeanByUserIdAsync_ShouldReturnDean_WhenDeanExists()
        {
            // Arrange
            var deanRole = new Role { Name = "Dean" };
            var dean = new User { UserId = 1, Name = "Dean A", Role = deanRole };
            await _context.Users.AddAsync(dean);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetDeanByUserIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Dean A", result.Name);
            Assert.AreEqual("Dean", result.Role.Name);
        }

        [Test]
        public async Task GetDeanByUserIdAsync_ShouldReturnNull_WhenDeanDoesNotExist()
        {
            // Act
            var result = await _repository.GetDeanByUserIdAsync(99);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetWeeksForStudentAsync Tests

        [Test]
        public async Task GetWeeksForStudentAsync_ShouldReturnWeeks_WhenInternshipExists()
        {
            // Arrange
            var internship = new Internship
            {
                IntershipId = 1,
                StudentId = 1,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 31)
            };
            await _context.Internships.AddAsync(internship);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetWeeksForStudentAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count); // January 2023 has 5 weeks
            Assert.AreEqual("02/01 to 08/01", result.First());
        }

        [Test]
        public void GetWeeksForStudentAsync_ShouldThrowKeyNotFoundException_WhenInternshipDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetWeeksForStudentAsync(99));
        }

        #endregion

        #region GetStudentDetailsByIdAsync Tests

        [Test]
        public async Task GetStudentDetailsByIdAsync_ShouldReturnStudent_WhenStudentExistsAndAccessIsAllowed()
        {
            // Arrange
            var student = new Student
            {
                StudentId = 1,
                User = new User { UserId = 1, Name = "Student A", Status = "Active" },
                Major = new Major { Name = "Major A" },
                Semester = new Semester { Name = "Semester 1" }
            };
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetStudentDetailsByIdAsync(1, 1, "Lecturer");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Student A", result.User.Name);
            Assert.AreEqual("Major A", result.Major.Name);
        }

        [Test]
        public void GetStudentDetailsByIdAsync_ShouldThrowKeyNotFoundException_WhenStudentDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _repository.GetStudentDetailsByIdAsync(99, 1, "Lecturer"));
        }

        #endregion

        #region GetWorkingReportsByStudentIdAsync Tests

        [Test]
        public async Task GetWorkingReportsByStudentIdAsync_ShouldReturnReports_WhenReportsExist()
        {
            // Arrange
            var internship = new Internship
            {
                IntershipId = 1,
                StudentId = 1,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 31)
            };
            var report1 = new WorkingReport { StudentId = 1, CreatedAt = new DateTime(2023, 1, 10) };
            var report2 = new WorkingReport { StudentId = 1, CreatedAt = new DateTime(2023, 1, 20) };

            await _context.Internships.AddAsync(internship);
            await _context.WorkingReports.AddRangeAsync(report1, report2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetWorkingReportsByStudentIdAsync(1, 1, "Lecturer", null, null, null, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetWorkingReportsByStudentIdAsync_ShouldReturnEmpty_WhenNoReportsExist()
        {
            // Arrange
            var internship = new Internship
            {
                IntershipId = 1,
                StudentId = 1,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 31)
            };
            await _context.Internships.AddAsync(internship);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetWorkingReportsByStudentIdAsync(1, 1, "Lecturer", null, null, null, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region UpdateWorkingReportAsync Tests

        [Test]
        public async Task UpdateWorkingReportAsync_ShouldUpdateReport_WhenReportExists()
        {
            // Arrange
            var report = new WorkingReport
            {
                WorkingReportId = 1,
                StudentId = 1,
                LecturerScore = 7,
                CreatedAt = DateTime.Now
            };
            await _context.WorkingReports.AddAsync(report);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.UpdateWorkingReportAsync(1, 1, "Lecturer", "Excellent", 8);

            // Assert
            Assert.IsTrue(result);
            var updatedReport = await _context.WorkingReports.FirstOrDefaultAsync(r => r.WorkingReportId == 1);
            Assert.AreEqual(8, updatedReport.LecturerScore);
            Assert.AreEqual("Excellent", updatedReport.FeedbackFromLecturer);
        }

        [Test]
        public async Task UpdateWorkingReportAsync_ShouldReturnFalse_WhenReportDoesNotExist()
        {
            // Act
            var result = await _repository.UpdateWorkingReportAsync(99, 1, "Lecturer", "Good", 9);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion
    }
}
