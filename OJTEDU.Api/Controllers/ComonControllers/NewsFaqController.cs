using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using System.Security.Claims;
using static OJTEDU.Application.DTOs.NewsFaqDTO;

namespace OJTEDU.Api.Controllers.ComonControllers
{
    [Route("api/common/news-faq")]
    [ApiController]
    public class NewsFaqController : ControllerBase
    {
        private readonly INewsFaqService _newsFaqService;
        public NewsFaqController(INewsFaqService newsFaqService)
        {
            _newsFaqService = newsFaqService;
        }

        [HttpGet("news-list")]
        public async Task<IActionResult> GetAllNews(string? title, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                string role = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.Role)?.Value : "guest";

                var dataResponse = await _newsFaqService.GetAllNewsAsync(role, title, actualPageNumber, actualPageSize);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                var apiResponse = new ApiResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("news-detail/{newsId}")]
        public async Task<IActionResult> GetNewsDetail(int? newsId)
        {
            try
            {
                if (!newsId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "newsId is required."
                    });
                }

                string role = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.Role)?.Value : "guest";

                var dataResponse = await _newsFaqService.GetNewsDetailAsync(newsId.Value, role);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                var apiResponse = new ApiResponse<NewsFaqDetailForCommonDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("news-content-list/{parentId}")]
        public async Task<IActionResult> GetAllNewsContentForNewsParent(int? parentId)
        {
            try
            {
                string role = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.Role)?.Value : "guest";

                var dataResponse = await _newsFaqService.GetAllNewsContentForNewsParentAsync(parentId, role);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                //if (dataResponse.Data == null)
                //{
                //    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                //    {
                //        Data = null,
                //        Message = dataResponse.Message
                //    });
                //}

                var apiResponse = new ApiResponse<List<NewsFaqListForCommonDTO>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("faqs-list")]
        public async Task<IActionResult> GetAllFaqs(string? title, int? pageNumber, int? pageSize)
        {
            try
            {
                int actualPageNumber = pageNumber ?? 1;
                int actualPageSize = pageSize ?? 15;

                string role = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.Role)?.Value : "guest";

                var dataResponse = await _newsFaqService.GetAllFaqsAsync(role, title, actualPageNumber, actualPageSize);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                var apiResponse = new ApiResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("faqs-detail/{faqId}")]
        public async Task<IActionResult> GetFaqsDetail(int? faqId)
        {
            try
            {
                if (!faqId.HasValue) // Sử dụng HasValue để kiểm tra Nullable
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Data = null,
                        Message = "faqId is required."
                    });
                }

                string role = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.Role)?.Value : "guest";

                var dataResponse = await _newsFaqService.GetFaqsDetailAsync(faqId.Value, role);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                if (dataResponse.Data == null)
                {
                    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                    {
                        Data = null,
                        Message = dataResponse.Message
                    });
                }

                var apiResponse = new ApiResponse<NewsFaqDetailForCommonDTO>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }

        [HttpGet("faqs-content-list/{parentId}")]
        public async Task<IActionResult> GetAllFaqsContentForFaqsParent(int? parentId)
        {
            try
            {
                string role = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.Role)?.Value : "guest";

                var dataResponse = await _newsFaqService.GetAllFaqsContentForFaqsParentAsync(parentId, role);

                if (dataResponse == null)
                {
                    return StatusCode(500, new ApiResponse<object>
                    {
                        Data = null,
                        Message = "Unexpected error occurred."
                    });
                }

                //if (dataResponse.Data == null)
                //{
                //    return StatusCode(dataResponse.StatusCode, new ApiResponse<object>
                //    {
                //        Data = null,
                //        Message = dataResponse.Message
                //    });
                //}

                var apiResponse = new ApiResponse<List<NewsFaqListForCommonDTO>>
                {
                    Data = dataResponse.Data,
                    Message = dataResponse.Message
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}"
                });
            }
        }
    }
}
