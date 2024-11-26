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
//using static OJTEDU.Application.DTOs.CompanyProposalDTO;

//namespace OJTEDU.Application.UnitTests.ApplicationServices.Services
//{
//    [TestFixture]
//    public class CompanyProposalServiceTests
//    {
//        private Mock<ICompanyProposalRepository> _companyProposalRepositoryMock;
//        private Mock<IMapper> _mapperMock;
//        private CompanyProposalService _companyProposalService;

//        [SetUp]
//        public void Setup()
//        {
//            _companyProposalRepositoryMock = new Mock<ICompanyProposalRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _companyProposalService = new CompanyProposalService(_companyProposalRepositoryMock.Object, _mapperMock.Object);
//        }

//        #region GetAllCompanyProposalByStudentIdAsync

//        [Test]
//        public async Task GetAllCompanyProposalByStudentIdAsync_ShouldReturnCompanyProposals_WhenProposalsExist()
//        {
//            // Arrange
//            var studentId = 1;
//            var proposals = new List<CompanyProposal>
//            {
//                new CompanyProposal { CompanyProposalId = 1, ProposalContent = "Proposal 1", ProposalDate = DateTime.Now },
//                new CompanyProposal { CompanyProposalId = 2, ProposalContent = "Proposal 2", ProposalDate = DateTime.Now }
//            };
//            var proposalDtos = new List<CompanyProposalListForStudentDTO>
//            {
//                new CompanyProposalListForStudentDTO { CompanyProposalId = 1, ProposalContent = "Proposal 1" },
//                new CompanyProposalListForStudentDTO { CompanyProposalId = 2, ProposalContent = "Proposal 2" }
//            };

//            _companyProposalRepositoryMock.Setup(repo => repo.GetAllCompanyProposalByStudentIdAsync(studentId)).ReturnsAsync(proposals);
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanyProposalListForStudentDTO>>(proposals)).Returns(proposalDtos);

//            // Act
//            var result = await _companyProposalService.GetAllCompanyProposalByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company proposal list retrieved successfully!", result.Message);
//            Assert.AreEqual(proposalDtos.Count, result.Data.Count);
//        }

//        [Test]
//        public async Task GetAllCompanyProposalByStudentIdAsync_ShouldReturnEmptyList_WhenNoProposalsExist()
//        {
//            // Arrange
//            var studentId = 1;
//            var emptyList = new List<CompanyProposal>();

//            _companyProposalRepositoryMock.Setup(repo => repo.GetAllCompanyProposalByStudentIdAsync(studentId)).ReturnsAsync(emptyList);
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanyProposalListForStudentDTO>>(emptyList)).Returns(new List<CompanyProposalListForStudentDTO>());

//            // Act
//            var result = await _companyProposalService.GetAllCompanyProposalByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(0, result.Data.Count);
//        }

//        [Test]
//        public async Task GetAllCompanyProposalByStudentIdAsync_ShouldReturnInternalServerError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            var studentId = 1;
//            _companyProposalRepositoryMock.Setup(repo => repo.GetAllCompanyProposalByStudentIdAsync(studentId))
//                                          .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _companyProposalService.GetAllCompanyProposalByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error retrieving company proposal list: Unexpected error. ", result.Message);
//        }

//        [Test]
//        public async Task GetAllCompanyProposalByStudentIdAsync_ShouldReturnEmptyList_WhenStudentHasNoProposals()
//        {
//            // Arrange
//            var studentId = 1;
//            var emptyList = new List<CompanyProposal>();

//            _companyProposalRepositoryMock.Setup(repo => repo.GetAllCompanyProposalByStudentIdAsync(studentId)).ReturnsAsync(emptyList);
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanyProposalListForStudentDTO>>(emptyList)).Returns(new List<CompanyProposalListForStudentDTO>());

//            // Act
//            var result = await _companyProposalService.GetAllCompanyProposalByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(0, result.Data.Count);
//            Assert.AreEqual("Company proposal list retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task GetAllCompanyProposalByStudentIdAsync_ShouldReturnPartialList_WhenOnlySomeProposalsExist()
//        {
//            // Arrange
//            var studentId = 1;
//            var partialList = new List<CompanyProposal>
//    {
//        new CompanyProposal { CompanyProposalId = 1, ProposalContent = "Proposal 1" }
//    };

//            _companyProposalRepositoryMock.Setup(repo => repo.GetAllCompanyProposalByStudentIdAsync(studentId)).ReturnsAsync(partialList);
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanyProposalListForStudentDTO>>(partialList))
//                       .Returns(partialList.Select(cp => new CompanyProposalListForStudentDTO
//                       {
//                           CompanyProposalId = cp.CompanyProposalId,
//                           ProposalContent = cp.ProposalContent
//                       }).ToList());

//            // Act
//            var result = await _companyProposalService.GetAllCompanyProposalByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(1, result.Data.Count);
//            Assert.AreEqual("Company proposal list retrieved successfully!", result.Message);
//        }

//        [Test]
//        public async Task GetAllCompanyProposalByStudentIdAsync_ShouldHandleMultipleProposalsForSameStudent()
//        {
//            // Arrange
//            var studentId = 1;
//            var proposals = new List<CompanyProposal>
//    {
//        new CompanyProposal { CompanyProposalId = 1, ProposalContent = "Proposal 1" },
//        new CompanyProposal { CompanyProposalId = 2, ProposalContent = "Proposal 2" }
//    };

//            _companyProposalRepositoryMock.Setup(repo => repo.GetAllCompanyProposalByStudentIdAsync(studentId)).ReturnsAsync(proposals);
//            _mapperMock.Setup(mapper => mapper.Map<List<CompanyProposalListForStudentDTO>>(proposals))
//                       .Returns(proposals.Select(cp => new CompanyProposalListForStudentDTO
//                       {
//                           CompanyProposalId = cp.CompanyProposalId,
//                           ProposalContent = cp.ProposalContent
//                       }).ToList());

//            // Act
//            var result = await _companyProposalService.GetAllCompanyProposalByStudentIdAsync(studentId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual(2, result.Data.Count);
//            Assert.AreEqual("Proposal 1", result.Data[0].ProposalContent);
//            Assert.AreEqual("Proposal 2", result.Data[1].ProposalContent);
//            Assert.AreEqual("Company proposal list retrieved successfully!", result.Message);
//        }


//        #endregion

//        #region GetCompanyProposalDetailByIdAsync

//        [Test]
//        public async Task GetCompanyProposalDetailByIdAsync_ShouldReturnProposalDetail_WhenProposalExists()
//        {
//            // Arrange
//            var proposalId = 1;
//            var proposal = new CompanyProposal
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "Sample proposal",
//                ResponseContent = "Sample response"
//            };
//            var proposalDto = new CompanyProposalDetailForStudentDTO
//            {
//                CompanyProposalId = proposal.CompanyProposalId,
//                ProposalContent = proposal.ProposalContent,
//                ResponseContent = proposal.ResponseContent
//            };

//            _companyProposalRepositoryMock.Setup(repo => repo.GetCompanyProposalDetailByIdAsync(proposalId)).ReturnsAsync(proposal);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyProposalDetailForStudentDTO>(proposal)).Returns(proposalDto);

//            // Act
//            var result = await _companyProposalService.GetCompanyProposalDetailByIdAsync(proposalId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company proposal detail retrieved successfully!", result.Message);
//            Assert.AreEqual(proposalDto, result.Data);
//        }


//        [Test]
//        public async Task GetCompanyProposalDetailByIdAsync_ShouldReturnProposalWithFullDetails_WhenProposalExists()
//        {
//            // Arrange
//            var proposalId = 1;
//            var proposal = new CompanyProposal
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "Proposal Content",
//                ResponseContent = "Response Content",
//                ProposalDate = DateTime.Now.AddDays(-10),
//                ResponseDate = DateTime.Now.AddDays(-5),
//                Contract = "contract.pdf",
//                CreatedAt = DateTime.Now.AddDays(-15),
//            };
//            var proposalDto = new CompanyProposalDetailForStudentDTO
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "Proposal Content",
//                ResponseContent = "Response Content",
//                ProposalDate = proposal.ProposalDate,
//                ResponseDate = proposal.ResponseDate,
//                Contract = "contract.pdf",
//                CreatedAt = proposal.CreatedAt
//            };

//            _companyProposalRepositoryMock.Setup(repo => repo.GetCompanyProposalDetailByIdAsync(proposalId)).ReturnsAsync(proposal);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyProposalDetailForStudentDTO>(proposal)).Returns(proposalDto);

//            // Act
//            var result = await _companyProposalService.GetCompanyProposalDetailByIdAsync(proposalId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company proposal detail retrieved successfully!", result.Message);
//            Assert.AreEqual(proposalDto, result.Data);
//        }

//        [Test]
//        public async Task GetCompanyProposalDetailByIdAsync_ShouldHandleNullResponseDate_WhenResponseNotGiven()
//        {
//            // Arrange
//            var proposalId = 1;
//            var proposal = new CompanyProposal
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "Proposal Content",
//                ResponseContent = null, // No response content
//                ProposalDate = DateTime.Now.AddDays(-10),
//                ResponseDate = null, // No response date
//                Contract = "contract.pdf",
//                CreatedAt = DateTime.Now.AddDays(-15),
//            };
//            var proposalDto = new CompanyProposalDetailForStudentDTO
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "Proposal Content",
//                ResponseContent = null,
//                ProposalDate = proposal.ProposalDate,
//                ResponseDate = null,
//                Contract = "contract.pdf",
//                CreatedAt = proposal.CreatedAt
//            };

//            _companyProposalRepositoryMock.Setup(repo => repo.GetCompanyProposalDetailByIdAsync(proposalId)).ReturnsAsync(proposal);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyProposalDetailForStudentDTO>(proposal)).Returns(proposalDto);

//            // Act
//            var result = await _companyProposalService.GetCompanyProposalDetailByIdAsync(proposalId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company proposal detail retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data.ResponseDate);
//            Assert.IsNull(result.Data.ResponseContent);
//        }


//        [Test]
//        public async Task GetCompanyProposalDetailByIdAsync_ShouldReturnProposalWithNullContract_WhenNoContractFileExists()
//        {
//            // Arrange
//            var proposalId = 1;
//            var proposal = new CompanyProposal
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "Proposal without contract",
//                ProposalDate = DateTime.Now.AddDays(-5),
//                Contract = null, // No contract file associated
//                CreatedAt = DateTime.Now.AddDays(-10),
//            };
//            var proposalDto = new CompanyProposalDetailForStudentDTO
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "Proposal without contract",
//                ProposalDate = proposal.ProposalDate,
//                Contract = null, // No contract file associated in DTO as well
//                CreatedAt = proposal.CreatedAt
//            };

//            _companyProposalRepositoryMock.Setup(repo => repo.GetCompanyProposalDetailByIdAsync(proposalId)).ReturnsAsync(proposal);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyProposalDetailForStudentDTO>(proposal)).Returns(proposalDto);

//            // Act
//            var result = await _companyProposalService.GetCompanyProposalDetailByIdAsync(proposalId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.IsNull(result.Data.Contract);
//            Assert.AreEqual("Company proposal detail retrieved successfully!", result.Message);
//            Assert.AreEqual(proposalDto, result.Data);
//        }

//        [Test]
//        public async Task GetCompanyProposalDetailByIdAsync_ShouldHandleEmptyProposalContent_WhenContentIsNotProvided()
//        {
//            // Arrange
//            var proposalId = 1;
//            var proposal = new CompanyProposal
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "", // Empty proposal content
//                ProposalDate = DateTime.Now.AddDays(-5),
//                Contract = "contract.pdf",
//                CreatedAt = DateTime.Now.AddDays(-10),
//            };
//            var proposalDto = new CompanyProposalDetailForStudentDTO
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "", // Empty content in DTO as well
//                ProposalDate = proposal.ProposalDate,
//                Contract = "contract.pdf",
//                CreatedAt = proposal.CreatedAt
//            };

//            _companyProposalRepositoryMock.Setup(repo => repo.GetCompanyProposalDetailByIdAsync(proposalId)).ReturnsAsync(proposal);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyProposalDetailForStudentDTO>(proposal)).Returns(proposalDto);

//            // Act
//            var result = await _companyProposalService.GetCompanyProposalDetailByIdAsync(proposalId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company proposal detail retrieved successfully!", result.Message);
//            Assert.AreEqual(proposalDto, result.Data);
//            Assert.AreEqual(0, result.Data.ProposalContent.Length); // Check if the ProposalContent is empty
//        }


//        [Test]
//        public async Task GetCompanyProposalDetailByIdAsync_ShouldReturnProposalWithoutUniversity_WhenUniversityNotLinked()
//        {
//            // Arrange
//            var proposalId = 1;
//            var proposal = new CompanyProposal
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "Proposal without university",
//                ProposalDate = DateTime.Now.AddDays(-7),
//                CreatedAt = DateTime.Now.AddDays(-10),
//                UniversityId = null // No university linked
//            };
//            var proposalDto = new CompanyProposalDetailForStudentDTO
//            {
//                CompanyProposalId = proposalId,
//                ProposalContent = "Proposal without university",
//                ProposalDate = proposal.ProposalDate,
//                CreatedAt = proposal.CreatedAt,
//                University = null // Expected university field to be null in DTO
//            };

//            _companyProposalRepositoryMock.Setup(repo => repo.GetCompanyProposalDetailByIdAsync(proposalId)).ReturnsAsync(proposal);
//            _mapperMock.Setup(mapper => mapper.Map<CompanyProposalDetailForStudentDTO>(proposal)).Returns(proposalDto);

//            // Act
//            var result = await _companyProposalService.GetCompanyProposalDetailByIdAsync(proposalId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Company proposal detail retrieved successfully!", result.Message);
//            Assert.IsNull(result.Data.University);
//            Assert.AreEqual(proposalDto, result.Data);
//        }


//        #endregion

//        #region  CreateCompanyProposalAsync

//        [Test]
//        public async Task CreateCompanyProposalAsync_ShouldReturnSuccess_WhenProposalIsCreated()
//        {
//            // Arrange
//            var createProposalDto = new CreateCompanyProposalForStudentDTO
//            {
//                StudentId = 1,
//                UniversityId = 1,
//                ProposalContent = "Sample Proposal Content"
//            };
//            var proposal = new CompanyProposal
//            {
//                CompanyProposalId = 1,
//                StudentId = createProposalDto.StudentId,
//                UniversityId = createProposalDto.UniversityId,
//                ProposalContent = createProposalDto.ProposalContent,
//                ProposalDate = DateTime.Now
//            };
//            var proposalDto = new CreateCompanyProposalForStudentDTO
//            {
//                StudentId = proposal.StudentId,
//                UniversityId = proposal.UniversityId,
//                ProposalContent = proposal.ProposalContent
//            };
//            var fileData = new byte[] { 0x01, 0x02 };

//            _companyProposalRepositoryMock.Setup(repo => repo.CreateCompanyProposalAsync(It.IsAny<CompanyProposal>(), "contract.pdf", fileData)).ReturnsAsync(proposal);
//            _mapperMock.Setup(mapper => mapper.Map<CreateCompanyProposalForStudentDTO>(proposal)).Returns(proposalDto);

//            // Act
//            var result = await _companyProposalService.CreateCompanyProposalAsync(createProposalDto, "contract.pdf", fileData);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Create company proposal successfully!", result.Message);
//            Assert.AreEqual(proposalDto.ProposalContent, result.Data.ProposalContent);
//        }

//        [Test]
//        public async Task CreateCompanyProposalAsync_ShouldReturnError_WhenRepositoryThrowsException()
//        {
//            // Arrange
//            var createProposalDto = new CreateCompanyProposalForStudentDTO
//            {
//                StudentId = 1,
//                UniversityId = 1,
//                ProposalContent = "Sample Proposal Content"
//            };
//            var fileData = new byte[] { 0x01, 0x02 };

//            _companyProposalRepositoryMock.Setup(repo => repo.CreateCompanyProposalAsync(It.IsAny<CompanyProposal>(), "contract.pdf", fileData))
//                                          .ThrowsAsync(new Exception("Unexpected error"));

//            // Act
//            var result = await _companyProposalService.CreateCompanyProposalAsync(createProposalDto, "contract.pdf", fileData);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(500, result.StatusCode);
//            Assert.AreEqual("Error create company proposal jpb: Unexpected error. ", result.Message);
//        }

//        [Test]
//        public async Task CreateCompanyProposalAsync_ShouldReturnProposalWithNullContract_WhenNoFileProvided()
//        {
//            // Arrange
//            var proposalDto = new CreateCompanyProposalForStudentDTO
//            {
//                StudentId = 1,
//                UniversityId = 1,
//                ProposalContent = "Proposal content",
//                Contract = null // No file provided
//            };
//            var fileName = (string?)null;
//            var fileData = (byte[]?)null;

//            var companyProposal = new CompanyProposal
//            {
//                CompanyProposalId = 1,
//                StudentId = proposalDto.StudentId,
//                UniversityId = proposalDto.UniversityId,
//                ProposalContent = proposalDto.ProposalContent,
//                Contract = null, // No contract file path
//                ProposalDate = DateTime.Now
//            };

//            _companyProposalRepositoryMock
//                .Setup(repo => repo.CreateCompanyProposalAsync(It.IsAny<CompanyProposal>(), fileName, fileData))
//                .ReturnsAsync(companyProposal);

//            _mapperMock.Setup(mapper => mapper.Map<CreateCompanyProposalForStudentDTO>(companyProposal)).Returns(proposalDto);

//            // Act
//            var result = await _companyProposalService.CreateCompanyProposalAsync(proposalDto, fileName, fileData);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Create company proposal successfully!", result.Message);
//            Assert.IsNull(result.Data.Contract); // Verify that contract is null when no file is provided
//        }


//        [Test]
//        public async Task CreateCompanyProposalAsync_ShouldReturnError_WhenFileNameIsNull()
//        {
//            // Arrange
//            var proposalDto = new CreateCompanyProposalForStudentDTO
//            {
//                StudentId = 1,
//                UniversityId = 1,
//                ProposalContent = "Proposal content",
//                Contract = null // No file name provided
//            };
//            var fileName = (string?)null;
//            var fileData = new byte[] { 0x01, 0x02 };

//            var companyProposal = new CompanyProposal
//            {
//                CompanyProposalId = 1,
//                StudentId = proposalDto.StudentId,
//                UniversityId = proposalDto.UniversityId,
//                ProposalContent = proposalDto.ProposalContent,
//                Contract = null // Contract path should remain null as no file is provided
//            };

//            _companyProposalRepositoryMock
//                .Setup(repo => repo.CreateCompanyProposalAsync(It.IsAny<CompanyProposal>(), fileName, fileData))
//                .ReturnsAsync(companyProposal);

//            _mapperMock.Setup(mapper => mapper.Map<CreateCompanyProposalForStudentDTO>(companyProposal)).Returns(proposalDto);

//            // Act
//            var result = await _companyProposalService.CreateCompanyProposalAsync(proposalDto, fileName, fileData);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.AreEqual("Create company proposal successfully!", result.Message);
//            Assert.IsNull(result.Data.Contract); // Confirm that no contract file is saved
//        }




//        #endregion
//    }
//}
