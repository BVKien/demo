using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Api.Input.MentorControllers.ContractController;
using static OJTEDU.Application.DTOs.ContractDTO;

namespace OJTEDU.Api.Controllers.MentorControllers
{
    [Route("api/mentor/contract")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [Authorize(Roles = "Mentor")]
        [HttpPost("files/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/contracts/files/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                return Ok(new
                {
                    Data = file.FileName
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string>
                {
                    Message = $"An error occurred while uploading file.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }

        [Authorize(Roles = "Mentor")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignContract(int? internshipId, AssignContractInput? input)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var filePath = Path.Combine("wwwroot/uploads/contracts/files/", input.ContractFile);

                // Initialize 
                byte[]? fileData = null;

                // Read content file if it is not null
                if (!string.IsNullOrEmpty(input.ContractFile) && System.IO.File.Exists(filePath))
                {
                    fileData = await System.IO.File.ReadAllBytesAsync(filePath);
                }

                var contractDto = new AssignContractInternshipForMentorDTO
                {
                    Name = input?.Name
                };

                var dataResponse = await _contractService.AssignContractAsync(userId, internshipId, input?.ContractFile, fileData, contractDto);

                if (dataResponse.StatusCode == 404)
                {
                    return BadRequest(new ApiResponse<AssignContractInternshipForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 400)
                {
                    return BadRequest(new ApiResponse<AssignContractInternshipForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 500)
                {
                    return StatusCode(500, new ApiResponse<AssignContractInternshipForMentorDTO>
                    {
                        Message = dataResponse.Message,
                        Data = null
                    });
                }

                if (dataResponse.StatusCode == 200)
                {
                    if (fileData != null && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                var apiResponse = new ApiResponse<AssignContractInternshipForMentorDTO>
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
                    Message = "An error occurred while assign contract for internship.",
                    Data = ex.Message
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
