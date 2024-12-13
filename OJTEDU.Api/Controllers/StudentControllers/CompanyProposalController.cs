using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using System.Security.Claims;
using static OJTEDU.Api.Input.StudentControllers.CompanyProposalController;
using static OJTEDU.Api.Input.StudentControllers.WorkingReportController;
using static OJTEDU.Application.DTOs.CompanyProposalDTO;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.Api.Controllers.StudentControllers
{
    [Route("api/student/company-proposal")]
    [ApiController]
    public class CompanyProposalController : ControllerBase
    {
        private readonly ICompanyProposalService _companyProposalService;
        public CompanyProposalController(ICompanyProposalService companyProposalService)
        {
            _companyProposalService = companyProposalService;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("list/{studentId}")]
        public async Task<IActionResult> GetAllCompanyProposal()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var dataResponse = await _companyProposalService.GetAllCompanyProposalByStudentIdAsync(userId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<List<CompanyProposalListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<List<CompanyProposalListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<List<CompanyProposalListForStudentDTO>>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<List<CompanyProposalListForStudentDTO>>
                {
                    Message = dataResponse.Message,
                    Data = dataResponse.Data
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while get company proposal list.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("detail/{companyProposalId}")]
        public async Task<IActionResult> GetCompanyProposalDetail(int? companyProposalId)
        {
            try
            {
                var dataResponse = await _companyProposalService.GetCompanyProposalDetailByIdAsync(companyProposalId);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CompanyProposalDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CompanyProposalDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CompanyProposalDetailForStudentDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                var apiResponse = new ApiResponse<CompanyProposalDetailForStudentDTO>
                {
                    Message = dataResponse.Message,
                    Data = dataResponse.Data
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while get company proposal detail. ",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        //[Authorize(Roles = "Student")]
        //[HttpPost("files/upload")]
        //public async Task<IActionResult> UploadFile()
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //            return BadRequest("No file uploaded.");

        //        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/companyproposals/files/");
        //        if (!Directory.Exists(uploadPath))
        //        {
        //            Directory.CreateDirectory(uploadPath);
        //        }

        //        var filePath = Path.Combine(uploadPath, file.FileName);
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        byte[] fileData;
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(memoryStream);
        //            fileData = memoryStream.ToArray();
        //        }

        //        return Ok(new
        //        {
        //            Data = file.FileName
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorResponse = new ApiResponse<string>
        //        {
        //            Message = "An error occurred while uploading file.",
        //            Data = ex.Message
        //        };

        //        return StatusCode(500, errorResponse);
        //    }
        //}

        [Authorize(Roles = "Student")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCompanyProposal([FromBody] CreateCompanyProposalInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                //var filePath = Path.Combine("wwwroot/uploads/companyproposals/files/", input.Contract);

                //// Initialize 
                //byte[]? fileData = null;

                //// Read content file if it is not null
                //if (!string.IsNullOrEmpty(input.Contract) && System.IO.File.Exists(filePath))
                //{
                //    fileData = await System.IO.File.ReadAllBytesAsync(filePath);
                //}

                var companyProposalInfo = new CreateCompanyProposalForStudentDTO
                {
                    ProposalTitle = input.ProposalTitle,
                    ProposalContent = input.ProposalContent
                };

                var apiResponse = await _companyProposalService.CreateCompanyProposalAsync(userId, companyProposalInfo, input.Contract, input.Contract);

                if (apiResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<CreateCompanyProposalForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<CreateCompanyProposalForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                if (apiResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<CreateCompanyProposalForStudentDTO>
                    {
                        Message = apiResponse.Message,
                        Data = null
                    });
                }

                //if (apiResponse.StatusCode == 200)
                //{
                //    if (fileData != null && System.IO.File.Exists(filePath))
                //    {
                //        System.IO.File.Delete(filePath);
                //    }
                //}

                return Ok(new ApiResponse<CreateCompanyProposalForStudentDTO>
                {
                    Message = apiResponse.Message,
                    Data = apiResponse.Data
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = "An error occurred while create company proposal.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
