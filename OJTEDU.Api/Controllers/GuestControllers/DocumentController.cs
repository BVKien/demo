//using Microsoft.AspNetCore.Mvc;
//using OJTEDU.Api.Configuration;
//using OJTEDU.Application.ApplicationServices.Interfaces;
//using static OJTEDU.Application.DTOs.DocumentDTO;

//namespace OJTEDU.Api.Controllers.GuestControllers
//{
//    [Route("api/guest/document")]
//    [ApiController]
//    public class DocumentController : ControllerBase
//    {
//        private readonly IDocumentService _documentService;
//        public DocumentController(IDocumentService documentService)
//        {
//            _documentService = documentService;
//        }

//        [HttpGet("internship-process")]
//        public async Task<IActionResult> GetInternshipProcessDocument()
//        {
//            try
//            {
//                var dataResponse = await _documentService.GetInternshipProcessDocumentAsync();

//                var apiResponse = new ApiResponse<DocumentInternshipProcessForGuestDTO>
//                {
//                    Message = dataResponse.Message,
//                    Data = dataResponse.Data
//                };

//                return Ok(apiResponse);
//            }
//            catch (Exception ex)
//            {
//                var errorResponse = new ApiResponse<object>
//                {
//                    Message = $"An error occurred while get internship process document. ",
//                    Data = new { Details = ex.Message }
//                };

//                return StatusCode(500, errorResponse);
//            }
//        }
//    }
//}
