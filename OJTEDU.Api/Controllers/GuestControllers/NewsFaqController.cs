//using Microsoft.AspNetCore.Mvc;
//using OJTEDU.Api.Configuration;
//using OJTEDU.Application.ApplicationServices.Interfaces;
//using static OJTEDU.Application.DTOs.NewsFaqDTO;

//namespace OJTEDU.Api.Controllers.GuestControllers
//{
//    [Route("api/guest/news-faq")]
//    [ApiController]
//    public class NewsFaqController : ControllerBase
//    {
//        private readonly INewsFaqService _newsFaqService;
//        public NewsFaqController(INewsFaqService newsFaqService)
//        {
//            _newsFaqService = newsFaqService;
//        }

//        [HttpGet("news-list")]
//        public async Task<IActionResult> GetAllNews()
//        {
//            try
//            {
//                var dataResponse = await _newsFaqService.GetAllNewsForGuestAsync();

//                var apiResponse = new ApiResponse<List<NewsFaqListForGuestDTO>>
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
//                    Message = $"An error occurred while get news list. ",
//                    Data = new { Details = ex.Message }
//                };

//                return StatusCode(500, errorResponse);
//            }
//        }

//        [HttpGet("news-detail/{newsId}")]
//        public async Task<IActionResult> GetNewsDetail(int? newsId)
//        {
//            try
//            {
//                var dataResponse = await _newsFaqService.GetNewsDetailAsync(newsId);

//                var apiResponse = new ApiResponse<NewsDetailForGuestDTO>
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
//                    Message = $"An error occurred while get news detail. ",
//                    Data = new { Details = ex.Message }
//                };

//                return StatusCode(500, errorResponse);
//            }
//        }

//        [HttpGet("news-content-list/{parentId}")]
//        public async Task<IActionResult> GetAllNewsContentForNewsParent(int? parentId)
//        {
//            try
//            {
//                var dataResponse = await _newsFaqService.GetAllNewsContentForNewsParentAsync(parentId);

//                var apiResponse = new ApiResponse<List<NewsFaqListForGuestDTO>>
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
//                    Message = $"An error occurred while get news content list for news parent with id {parentId}. ",
//                    Data = new { Details = ex.Message }
//                };

//                return StatusCode(500, errorResponse);
//            }
//        }

//        [HttpGet("faqs-list")]
//        public async Task<IActionResult> GetAllFaqs()
//        {
//            try
//            {
//                var dataResponse = await _newsFaqService.GetAllFaqsForGuestAsync();

//                var apiResponse = new ApiResponse<List<NewsFaqListForGuestDTO>>
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
//                    Message = $"An error occurred while get faqs list. ",
//                    Data = new { Details = ex.Message }
//                };

//                return StatusCode(500, errorResponse);
//            }
//        }

//        [HttpGet("faqs-content-list/{parentId}")]
//        public async Task<IActionResult> GetAllFaqsContentForFaqsParent(int? parentId)
//        {
//            try
//            {
//                var dataResponse = await _newsFaqService.GetAllFaqsContentForFaqsParentAsync(parentId);

//                var apiResponse = new ApiResponse<List<NewsFaqListForGuestDTO>>
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
//                    Message = $"An error occurred while get faqs content list for faqs parent with id {parentId}. ",
//                    Data = new { Details = ex.Message }
//                };

//                return StatusCode(500, errorResponse);
//            }
//        }
//    }
//}
