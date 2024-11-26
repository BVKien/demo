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
//using static OJTEDU.Application.DTOs.JobDTO;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class JobServiceTests
//    {
//        private Mock<IJobRepository> _jobRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private JobService _jobService;

//        [SetUp]
//        public void Setup()
//        {
//            _jobRepositoryMock = new Mock<IJobRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _jobService = new JobService(_jobRepositoryMock.Object, _mapperMock.Object);
//        }

//        #region GetAllJobsByCompanyIdAsync

//        [Test]
//        public async Task GetAllJobsByCompanyIdAsync_ShouldReturnJobList_WhenCompanyHasJobs()
//        {
//            // Arrange
//            int companyId = 1;
//            var jobs = new List<Job> { new Job { JobId = 1, Title = "Software Engineer" } };
//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsByCompanyIdAsync(companyId)).ReturnsAsync(jobs);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListByCompanyIdForStudentDTO>>(jobs))
//                       .Returns(new List<JobListByCompanyIdForStudentDTO> { new JobListByCompanyIdForStudentDTO { Title = "Software Engineer" } });

//            // Act
//            var result = await _jobService.GetAllJobsByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Software Engineer", result.Data[0].Title);
//        }

//        [Test]
//        public async Task GetAllJobsByCompanyIdAsync_ShouldReturnEmptyList_WhenNoJobsFound()
//        {
//            // Arrange
//            int companyId = 1;
//            var emptyJobList = new List<Job>();
//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsByCompanyIdAsync(companyId)).ReturnsAsync(emptyJobList);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListByCompanyIdForStudentDTO>>(emptyJobList))
//                       .Returns(new List<JobListByCompanyIdForStudentDTO>());

//            // Act
//            var result = await _jobService.GetAllJobsByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(0, result.Data.Count);
//            Assert.AreEqual("Job list retrieved successfully!", result.Message);
//        }


//        [Test]
//        public async Task GetAllJobsByCompanyIdAsync_ShouldReturnMappedData_WhenValidCompanyIdIsProvided()
//        {
//            // Arrange
//            int companyId = 1;
//            var jobList = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Developer" },
//        new Job { JobId = 2, Title = "Tester" }
//    };
//            var jobDtoList = jobList.Select(j => new JobListByCompanyIdForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsByCompanyIdAsync(companyId)).ReturnsAsync(jobList);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListByCompanyIdForStudentDTO>>(jobList)).Returns(jobDtoList);

//            // Act
//            var result = await _jobService.GetAllJobsByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(jobDtoList.Count, result.Data.Count);
//            Assert.AreEqual("Developer", result.Data[0].Title);
//            Assert.AreEqual("Tester", result.Data[1].Title);
//        }


//        [Test]
//        public async Task GetAllJobsByCompanyIdAsync_ShouldReturnJobs_WhenJobsExistForCompany()
//        {
//            // Arrange
//            var companyId = 1;
//            var jobs = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Developer", CompanyId = companyId },
//        new Job { JobId = 2, Title = "Designer", CompanyId = companyId }
//    };
//            var jobDtos = jobs.Select(j => new JobListByCompanyIdForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsByCompanyIdAsync(companyId)).ReturnsAsync(jobs);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListByCompanyIdForStudentDTO>>(jobs)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.GetAllJobsByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job list retrieved successfully!", result.Message);
//            Assert.AreEqual(jobDtos.Count, result.Data.Count);
//        }

//        [Test]
//        public async Task GetAllJobsByCompanyIdAsync_ShouldReturnJobList_WithCorrectAddress()
//        {
//            // Arrange
//            var companyId = 6;
//            var jobs = new List<Job>
//    {
//        new Job
//        {
//            JobId = 5,
//            Title = "Sales Manager",
//            CompanyId = companyId,
//            AddressedNavigation = new Address
//            {
//                Province = new Province { Name = "New York" }
//            }
//        }
//    };

//            var jobDtos = jobs.Select(j => new JobListByCompanyIdForStudentDTO
//            {
//                JobId = j.JobId,
//                Title = j.Title,
//                Address = j.AddressedNavigation.Province.Name // Ensure correct mapping here
//            }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsByCompanyIdAsync(companyId)).ReturnsAsync(jobs);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListByCompanyIdForStudentDTO>>(jobs)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.GetAllJobsByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job list retrieved successfully!", result.Message);
//            Assert.AreEqual("New York", result.Data.First().Address);
//        }

//        [Test]
//        public async Task GetAllJobsByCompanyIdAsync_ShouldReturnEmptyList_WhenCompanyOnlyHasArchivedJobs()
//        {
//            // Arrange
//            var companyId = 7;
//            var archivedJobs = new List<Job>
//    {
//        new Job { JobId = 6, Title = "Old Position", Status = "archived", CompanyId = companyId }
//    };

//            var emptyJobDtos = new List<JobListByCompanyIdForStudentDTO>();

//            // Ensure setup to return only archived jobs
//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsByCompanyIdAsync(companyId)).ReturnsAsync(archivedJobs);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListByCompanyIdForStudentDTO>>(archivedJobs)).Returns(emptyJobDtos);

//            // Act
//            var result = await _jobService.GetAllJobsByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job list retrieved successfully!", result.Message);
//            Assert.AreEqual(0, result.Data.Count, "Expected no jobs when only archived jobs are present.");
//        }


//        [Test]
//        public async Task GetAllJobsByCompanyIdAsync_ShouldReturnJobs_WithCorrectTitle()
//        {
//            // Arrange
//            var companyId = 4;
//            var jobs = new List<Job> { new Job { JobId = 3, Title = "Analyst", CompanyId = companyId } };
//            var jobDtos = jobs.Select(j => new JobListByCompanyIdForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsByCompanyIdAsync(companyId)).ReturnsAsync(jobs);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListByCompanyIdForStudentDTO>>(jobs)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.GetAllJobsByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job list retrieved successfully!", result.Message);
//            Assert.AreEqual("Analyst", result.Data.First().Title);
//        }

//        [Test]
//        public async Task GetAllJobsByCompanyIdAsync_ShouldReturnJobList_WithFormattedDeadline()
//        {
//            // Arrange
//            var companyId = 5;
//            var deadline = new DateTime(2024, 12, 31);
//            var jobs = new List<Job> { new Job { JobId = 4, Title = "Product Manager", Deadline = deadline, CompanyId = companyId } };
//            var jobDtos = jobs.Select(j => new JobListByCompanyIdForStudentDTO { JobId = j.JobId, Title = j.Title, Deadline = "31-12-2024" }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsByCompanyIdAsync(companyId)).ReturnsAsync(jobs);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListByCompanyIdForStudentDTO>>(jobs)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.GetAllJobsByCompanyIdAsync(companyId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job list retrieved successfully!", result.Message);
//            Assert.AreEqual("31-12-2024", result.Data.First().Deadline);
//        }






//        #endregion



//        #region SearchJobsAsync

//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnPagedJobList_WhenJobsMatchCriteria()
//        {
//            // Arrange
//            int studentId = 1;
//            var jobs = new List<Job> { new Job { JobId = 1, Title = "Junior Developer" } };
//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, "Developer", null, null, null, null, 1, 10))
//                              .ReturnsAsync((jobs, 1));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(jobs))
//                       .Returns(new List<JobListSearchForStudentDTO> { new JobListSearchForStudentDTO { Title = "Junior Developer" } });

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, "Developer", null, null, null, null, 1, 10);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.TotalPages);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Junior Developer", result.Data[0].Title);
//        }


//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnPagedResults_WhenPagingIsApplied()
//        {
//            // Arrange
//            var studentId = 1;
//            var title = "Developer";
//            var majorId = 2;
//            var pageNumber = 1;
//            var pageSize = 15;

//            var jobs = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Developer", MajorId = majorId },
//        new Job { JobId = 2, Title = "Senior Developer", MajorId = majorId },
//        new Job { JobId = 3, Title = "Junior Developer", MajorId = majorId }
//    };

//            var mappedJobs = jobs.Take(pageSize).Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, title, majorId, null, null, null, pageNumber, pageSize))
//                              .ReturnsAsync((jobs.Take(pageSize), jobs.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(It.IsAny<IEnumerable<Job>>())).Returns(mappedJobs);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, title, majorId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(pageSize, result.Data.Count);
//            Assert.AreEqual("Job list retrieved successfully!", result.Message);
//        }


//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnJobs_WhenMatchingCriteriaExist()
//        {
//            // Arrange
//            int? studentId = 1;
//            string? title = "Developer";
//            int? majorId = 2;
//            int? provinceId = 3;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var matchingJobs = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Software Developer", MajorId = majorId, AddressedNavigation = new Address { ProvinceId = provinceId } },
//        new Job { JobId = 2, Title = "Backend Developer", MajorId = majorId, AddressedNavigation = new Address { ProvinceId = provinceId } }
//    };

//            var jobDtos = matchingJobs.Select(job => new JobListSearchForStudentDTO
//            {
//                JobId = job.JobId,
//                Title = job.Title,
//                Major = "Computer Science"
//            }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, title, majorId, provinceId, null, null, pageNumber, pageSize))
//                              .ReturnsAsync((matchingJobs, matchingJobs.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(matchingJobs)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, title, majorId, provinceId, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.TotalPages);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("Software Developer", result.Data[0].Title);
//        }

//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnEmptyList_WhenNoMatchingCriteriaExist()
//        {
//            // Arrange
//            int? studentId = 1;
//            string? title = "Nonexistent Job";
//            int? pageNumber = 1;
//            int?pageSize = 15;

//            var emptyJobList = new List<Job>();
//            var emptyJobDtos = new List<JobListSearchForStudentDTO>();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, title, null, null, null, null, pageNumber, pageSize))
//                              .ReturnsAsync((emptyJobList, 0));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(emptyJobList)).Returns(emptyJobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, title, null, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(0, result.TotalPages);
//            Assert.AreEqual(0, result.Data.Count);
//        }


//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnJobsFilteredByMajor_WhenMajorIdIsSpecified()
//        {
//            // Arrange
//            int? studentId = 1;
//            int? majorId = 2;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var jobsByMajor = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Junior Developer", MajorId = majorId },
//        new Job { JobId = 2, Title = "Senior Developer", MajorId = majorId }
//    };

//            var jobDtos = jobsByMajor.Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title, Major = "Computer Science" }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, null, majorId, null, null, null, pageNumber, pageSize))
//                              .ReturnsAsync((jobsByMajor, jobsByMajor.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(jobsByMajor)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, null, majorId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.TotalPages);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("Junior Developer", result.Data[0].Title);
//            Assert.AreEqual("Senior Developer", result.Data[1].Title);
//        }

//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnJobsFilteredByLocation_WhenProvinceIdAndDistrictIdAreSpecified()
//        {
//            // Arrange
//            int? studentId = 1;
//            int? provinceId = 3;
//            int? districtId = 5;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var jobsByLocation = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Data Analyst", AddressedNavigation = new Address { ProvinceId = provinceId, DistrictId = districtId } }
//    };

//            var jobDtos = jobsByLocation.Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, null, null, provinceId, districtId, null, pageNumber, pageSize))
//                              .ReturnsAsync((jobsByLocation, jobsByLocation.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(jobsByLocation)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, null, null, provinceId, districtId, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.TotalPages);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Data Analyst", result.Data[0].Title);
//        }


//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnJobs_WhenWardIdIsSpecified()
//        {
//            // Arrange
//            int? studentId = 1;
//            int? wardId = 8;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var jobsWithWard = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Ward Specific Job", AddressedNavigation = new Address { WardId = wardId } }
//    };

//            var jobDtos = jobsWithWard.Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, null, null, null, null, wardId, pageNumber, pageSize))
//                              .ReturnsAsync((jobsWithWard, jobsWithWard.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(jobsWithWard)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, null, null, null, null, wardId, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job list retrieved successfully!", result.Message);
//            Assert.AreEqual("Ward Specific Job", result.Data.First().Title);
//        }

//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnPaginatedResults_WhenMultiplePagesExist()
//        {
//            // Arrange
//            int? studentId = 1;
//            int pageNumber = 2;
//            int pageSize = 15;

//            var allJobs = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Job 1" },
//        new Job { JobId = 2, Title = "Job 2" },
//        new Job { JobId = 3, Title = "Job 3" },
//        new Job { JobId = 4, Title = "Job 4" }
//    };

//            var pagedJobs = allJobs.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
//            var jobDtos = pagedJobs.Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, null, null, null, null, null, pageNumber, pageSize))
//                              .ReturnsAsync((pagedJobs, allJobs.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(pagedJobs)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, null, null, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Count); // Page size
//            Assert.AreEqual("Job 3", result.Data.First().Title);
//            Assert.AreEqual("Job 4", result.Data.Last().Title);
//        }

//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnJobs_WhenFilteredByMajorId()
//        {
//            // Arrange
//            int? studentId = 1;
//            int? majorId = 3;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var jobsWithMajor = new List<Job>
//    {
//        new Job { JobId = 2, Title = "Software Engineer", MajorId = majorId }
//    };

//            var jobDtos = jobsWithMajor.Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, null, majorId, null, null, null, pageNumber, pageSize))
//                              .ReturnsAsync((jobsWithMajor, jobsWithMajor.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(jobsWithMajor)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, null, majorId, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Software Engineer", result.Data.First().Title);
//        }

//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnJobsFilteredByProvinceAndDistrict()
//        {
//            // Arrange
//            int? studentId = 1;
//            int? provinceId = 5;
//            int? districtId = 9;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var filteredJobs = new List<Job>
//    {
//        new Job { JobId = 3, Title = "Data Analyst", AddressedNavigation = new Address { ProvinceId = provinceId, DistrictId = districtId } }
//    };

//            var jobDtos = filteredJobs.Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, null, null, provinceId, districtId, null, pageNumber, pageSize))
//                              .ReturnsAsync((filteredJobs, filteredJobs.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(filteredJobs)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, null, null, provinceId, districtId, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Data Analyst", result.Data.First().Title);
//        }


//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnJobs_WhenTitleAndLocationFiltersAreApplied()
//        {
//            // Arrange
//            int? studentId = 1;
//            string title = "Engineer";
//            int? provinceId = 5;
//            int? districtId = 7;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var filteredJobs = new List<Job>
//    {
//        new Job { JobId = 4, Title = "Civil Engineer", AddressedNavigation = new Address { ProvinceId = provinceId, DistrictId = districtId } }
//    };

//            var jobDtos = filteredJobs.Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, title, null, provinceId, districtId, null, pageNumber, pageSize))
//                              .ReturnsAsync((filteredJobs, filteredJobs.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(filteredJobs)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, title, null, provinceId, districtId, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Civil Engineer", result.Data.First().Title);
//        }


//        [Test]
//        public async Task SearchJobsAsync_ShouldHandleNullTitleFilter_WhenOtherFiltersArePresent()
//        {
//            // Arrange
//            int? studentId = 1;
//            int? majorId = 2;
//            int? provinceId = 3;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var jobs = new List<Job>
//    {
//        new Job { JobId = 5, Title = "Software Developer", MajorId = majorId, AddressedNavigation = new Address { ProvinceId = provinceId } }
//    };

//            var jobDtos = jobs.Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, null, majorId, provinceId, null, null, pageNumber, pageSize))
//                              .ReturnsAsync((jobs, jobs.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(jobs)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, null, majorId, provinceId, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Software Developer", result.Data.First().Title);
//        }

//        [Test]
//        public async Task SearchJobsAsync_ShouldReturnPagedResults_WhenMoreThanPageSizeJobsExist()
//        {
//            // Arrange
//            int? studentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var jobs = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Job 1" },
//        new Job { JobId = 2, Title = "Job 2" },
//        new Job { JobId = 3, Title = "Job 3" }
//    };

//            var jobDtos = jobs.Take(pageSize).Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, null, null, null, null, null, pageNumber, pageSize))
//                              .ReturnsAsync((jobs.Take(pageSize), jobs.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(jobs.Take(pageSize).ToList())).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, null, null, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("Job list retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task SearchJobsAsync_ShouldHandleJobsWithNullAddressFields()
//        {
//            // Arrange
//            int? studentId = 1;
//            int pageNumber = 1;
//            int pageSize = 15;

//            var jobsWithNullAddress = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Job Without Address", AddressedNavigation = null }
//    };

//            var jobDtos = jobsWithNullAddress.Select(j => new JobListSearchForStudentDTO { JobId = j.JobId, Title = j.Title, Address = null }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.SearchJobsAsync(studentId, null, null, null, null, null, pageNumber, pageSize))
//                              .ReturnsAsync((jobsWithNullAddress, jobsWithNullAddress.Count));
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListSearchForStudentDTO>>(jobsWithNullAddress)).Returns(jobDtos);

//            // Act
//            var result = await _jobService.SearchJobsAsync(studentId, null, null, null, null, null, pageNumber, pageSize);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job Without Address", result.Data.First().Title);
//            Assert.IsNull(result.Data.First().Address);
//        }



//        #endregion


//        #region GetAllJobsAsync

//        [Test]
//        public async Task GetAllJobsAsync_ShouldReturnJobList_WhenJobsExist()
//        {
//            // Arrange
//            var jobs = new List<Job> { new Job { JobId = 1, Title = "Data Scientist" } };
//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobs);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListForStudentDTO>>(jobs))
//                       .Returns(new List<JobListForStudentDTO> { new JobListForStudentDTO { Title = "Data Scientist" } });

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Data Scientist", result.Data[0].Title);
//        }


//        [Test]
//        public async Task GetAllJobsAsync_ShouldHandleLargeJobListEfficiently()
//        {
//            // Arrange
//            var largeJobList = Enumerable.Range(1, 1000).Select(i => new Job { JobId = i, Title = $"Job {i}" }).ToList();
//            var largeJobDtoList = largeJobList.Select(j => new JobListForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(largeJobList);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListForStudentDTO>>(largeJobList)).Returns(largeJobDtoList);

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1000, result.Data.Count);
//        }


//        [Test]
//        public async Task GetAllJobsAsync_ShouldReturnJobs_WithCorrectMappingForEachField()
//        {
//            // Arrange
//            var job = new Job
//            {
//                JobId = 1,
//                Title = "Data Analyst",
//                Description = "Analyze data",
//                SalaryRange = "5000-7000"
//            };
//            var jobDto = new JobListForStudentDTO
//            {
//                JobId = job.JobId,
//                Title = job.Title,
//                Description = job.Description,
//                SalaryRange = job.SalaryRange
//            };

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(new List<Job> { job });
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListForStudentDTO>>(It.IsAny<List<Job>>())).Returns(new List<JobListForStudentDTO> { jobDto });

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Data Analyst", result.Data[0].Title);
//            Assert.AreEqual("Analyze data", result.Data[0].Description);
//            Assert.AreEqual("5000-7000", result.Data[0].SalaryRange);
//        }


//        [Test]
//        public async Task GetAllJobsAsync_ShouldHandleLongJobTitlesGracefully()
//        {
//            // Arrange
//            var longTitle = new string('A', 300); // 300 characters long
//            var jobList = new List<Job> { new Job { JobId = 1, Title = longTitle } };
//            var jobDto = new JobListForStudentDTO { JobId = 1, Title = longTitle };

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobList);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListForStudentDTO>>(jobList)).Returns(new List<JobListForStudentDTO> { jobDto });

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual(longTitle, result.Data[0].Title);
//        }


//        [Test]
//        public async Task GetAllJobsAsync_ShouldReturnCorrectJobCount()
//        {
//            // Arrange
//            var jobList = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Job A" },
//        new Job { JobId = 2, Title = "Job B" },
//        new Job { JobId = 3, Title = "Job C" }
//    };

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobList);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListForStudentDTO>>(jobList))
//                       .Returns(jobList.Select(j => new JobListForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList());

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(3, result.Data.Count);
//        }

//        [Test]
//        public async Task GetAllJobsAsync_ShouldHandleJobsWithNullFields()
//        {
//            // Arrange
//            var jobsWithNullFields = new List<Job>
//    {
//        new Job { JobId = 1, Title = null, Description = null }
//    };

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobsWithNullFields);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListForStudentDTO>>(jobsWithNullFields))
//                       .Returns(jobsWithNullFields.Select(j => new JobListForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList());

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.IsNull(result.Data.First().Title);
//        }

//        [Test]
//        public async Task GetAllJobsAsync_ShouldReturnListWithCorrectFields()
//        {
//            // Arrange
//            var jobList = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Engineer", SalaryRange = "50-60K", Deadline = DateTime.Now.AddMonths(1) }
//    };

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobList);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListForStudentDTO>>(jobList))
//                       .Returns(jobList.Select(j => new JobListForStudentDTO
//                       {
//                           JobId = j.JobId,
//                           Title = j.Title,
//                           SalaryRange = j.SalaryRange,
//                           Deadline = j.Deadline?.ToString("dd-MM-yyyy")
//                       }).ToList());

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("Engineer", result.Data.First().Title);
//            Assert.AreEqual("50-60K", result.Data.First().SalaryRange);
//        }

//        [Test]
//        public async Task GetAllJobsAsync_ShouldReturnError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ThrowsAsync(new Exception("Database error"));

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving job list Database error. ", result.Message);
//            Assert.IsNull(result.Data);
//        }

//        [Test]
//        public async Task GetAllJobsAsync_ShouldReturnListSortedByTitle()
//        {
//            // Arrange
//            var jobList = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Zookeeper" },
//        new Job { JobId = 2, Title = "Accountant" },
//        new Job { JobId = 3, Title = "Engineer" }
//    };

//            var sortedJobDtos = jobList.OrderBy(j => j.Title).Select(j => new JobListForStudentDTO { JobId = j.JobId, Title = j.Title }).ToList();

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobList);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListForStudentDTO>>(jobList)).Returns(sortedJobDtos);

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("Accountant", result.Data.First().Title);
//            Assert.AreEqual("Zookeeper", result.Data.Last().Title);
//        }

//        [Test]
//        public async Task GetAllJobsAsync_ShouldReturnCorrectSalaryRange()
//        {
//            // Arrange
//            var jobList = new List<Job>
//    {
//        new Job { JobId = 1, Title = "Job X", SalaryRange = "40-50K" },
//        new Job { JobId = 2, Title = "Job Y", SalaryRange = "60-70K" }
//    };

//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobList);
//            _mapperMock.Setup(mapper => mapper.Map<List<JobListForStudentDTO>>(jobList))
//                       .Returns(jobList.Select(j => new JobListForStudentDTO { JobId = j.JobId, Title = j.Title, SalaryRange = j.SalaryRange }).ToList());

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("40-50K", result.Data.First().SalaryRange);
//            Assert.AreEqual("60-70K", result.Data.Last().SalaryRange);
//        }


//        [Test]
//        public async Task GetAllJobsAsync_ShouldReturnServerError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            _jobRepositoryMock.Setup(repo => repo.GetAllJobsAsync()).ThrowsAsync(new Exception("Database failure"));

//            // Act
//            var result = await _jobService.GetAllJobsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving job list Database failure. ", result.Message);
//            Assert.IsNull(result.Data);
//        }






//        #endregion


//        #region GetJobDetailAsync

//        [Test]
//        public async Task GetJobDetailAsync_ShouldReturnJobDetail_WhenJobExists()
//        {
//            // Arrange
//            int jobId = 1;
//            var job = new Job { JobId = jobId, Title = "Frontend Developer" };
//            var jobDetailDto = new JobDetailForStudentDTO { Title = "Frontend Developer" };
//            _jobRepositoryMock.Setup(repo => repo.GetJobDetailAsync(jobId)).ReturnsAsync(job);
//            _mapperMock.Setup(mapper => mapper.Map<JobDetailForStudentDTO>(job)).Returns(jobDetailDto);

//            // Act
//            var result = await _jobService.GetJobDetailAsync(jobId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Frontend Developer", result.Data.Title);
//        }


//        [Test]
//        public async Task GetJobDetailAsync_ShouldReturnJobDetail_WithCorrectCompanyName()
//        {
//            // Arrange
//            var jobId = 3;
//            var job = new Job { JobId = jobId, Title = "Project Manager", Company = new Company { User = new User { Name = "Tech Corp" } } };
//            var jobDto = new JobDetailForStudentDTO { JobId = jobId, Title = "Project Manager", CompanyName = "Tech Corp" };

//            _jobRepositoryMock.Setup(repo => repo.GetJobDetailAsync(jobId)).ReturnsAsync(job);
//            _mapperMock.Setup(mapper => mapper.Map<JobDetailForStudentDTO>(job)).Returns(jobDto);

//            // Act
//            var result = await _jobService.GetJobDetailAsync(jobId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job detail retrieved successfully!", result.Message);
//            Assert.AreEqual("Tech Corp", result.Data.CompanyName);
//        }

//        [Test]
//        public async Task GetJobDetailAsync_ShouldReturnJobDetail_WithFormattedDeadline()
//        {
//            // Arrange
//            var jobId = 4;
//            var deadline = new DateTime(2024, 12, 31);
//            var job = new Job { JobId = jobId, Title = "Data Scientist", Deadline = deadline };
//            var jobDto = new JobDetailForStudentDTO { JobId = jobId, Title = "Data Scientist", Deadline = "31-12-2024" };

//            _jobRepositoryMock.Setup(repo => repo.GetJobDetailAsync(jobId)).ReturnsAsync(job);
//            _mapperMock.Setup(mapper => mapper.Map<JobDetailForStudentDTO>(job)).Returns(jobDto);

//            // Act
//            var result = await _jobService.GetJobDetailAsync(jobId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job detail retrieved successfully!", result.Message);
//            Assert.AreEqual("31-12-2024", result.Data.Deadline);
//        }

//        [Test]
//        public async Task GetJobDetailAsync_ShouldReturnJobDetail_WithRequiredSkills()
//        {
//            // Arrange
//            var jobId = 5;
//            var job = new Job { JobId = jobId, Title = "Analyst", SkillRequirements = "SQL, Excel" };
//            var jobDto = new JobDetailForStudentDTO { JobId = jobId, Title = "Analyst", SkillRequirements = "SQL, Excel" };

//            _jobRepositoryMock.Setup(repo => repo.GetJobDetailAsync(jobId)).ReturnsAsync(job);
//            _mapperMock.Setup(mapper => mapper.Map<JobDetailForStudentDTO>(job)).Returns(jobDto);

//            // Act
//            var result = await _jobService.GetJobDetailAsync(jobId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job detail retrieved successfully!", result.Message);
//            Assert.AreEqual("SQL, Excel", result.Data.SkillRequirements);
//        }

//        [Test]
//        public async Task GetJobDetailAsync_ShouldReturnJobDetail_WithCorrectAddress()
//        {
//            // Arrange
//            var jobId = 6;
//            var job = new Job { JobId = jobId, Title = "Consultant", AddressedNavigation = new Address { Province = new Province { Name = "California" } } };
//            var jobDto = new JobDetailForStudentDTO { JobId = jobId, Title = "Consultant", Address = "California" };

//            _jobRepositoryMock.Setup(repo => repo.GetJobDetailAsync(jobId)).ReturnsAsync(job);
//            _mapperMock.Setup(mapper => mapper.Map<JobDetailForStudentDTO>(job)).Returns(jobDto);

//            // Act
//            var result = await _jobService.GetJobDetailAsync(jobId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job detail retrieved successfully!", result.Message);
//            Assert.AreEqual("California", result.Data.Address);
//        }

//        [Test]
//        public async Task GetJobDetailAsync_ShouldReturnJobDetail_WithNullBenefits_WhenBenefitsNotProvided()
//        {
//            // Arrange
//            var jobId = 7;
//            var job = new Job { JobId = jobId, Title = "Marketing Specialist", Benefits = null };
//            var jobDto = new JobDetailForStudentDTO { JobId = jobId, Title = "Marketing Specialist", Benefits = null };

//            _jobRepositoryMock.Setup(repo => repo.GetJobDetailAsync(jobId)).ReturnsAsync(job);
//            _mapperMock.Setup(mapper => mapper.Map<JobDetailForStudentDTO>(job)).Returns(jobDto);

//            // Act
//            var result = await _jobService.GetJobDetailAsync(jobId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Job detail retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data.Benefits);
//        }






//        #endregion
//    }
//}
