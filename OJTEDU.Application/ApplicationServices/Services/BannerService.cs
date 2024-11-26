using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.BannerDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IMapper _mapper;
        public BannerService(IBannerRepository bannerRepository, IMapper mapper)
        {
            _bannerRepository = bannerRepository;
            _mapper = mapper;
        }

        // Admin - Banner Management
        public async Task<DataResponse<PagedResponse<List<BannerListForAdminDTO>>>> GetAllBannerForAdminAsync(DateTime? startEventDate, DateTime? endEventDate, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var banners = await _bannerRepository.GetAllBannerForAdminAsync(startEventDate, endEventDate, status);

                var totalBanners = banners.Count();
                var totalPages = totalBanners == 0 ? 1 : (int)Math.Ceiling((double)totalBanners / pageSize);

                var bannerDtos = totalBanners > 0 ? _mapper.Map<List<BannerListForAdminDTO>>(banners)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<BannerListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<BannerListForAdminDTO>>
                {
                    Items = bannerDtos,
                    TotalCount = totalBanners,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<BannerListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Banner list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<BannerListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<PagedResponse<List<BannerListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Access denied while get banner list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<BannerListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving banner list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<BannerDetailForAdminDTO>> GetBannerDetailByIdForAdminAsync(int bannerId)
        {
            try
            {
                var banner = await _bannerRepository.GetBannerByIdForAdminAsync(bannerId);

                var bannerDto = _mapper.Map<BannerDetailForAdminDTO>(banner);

                return new DataResponse<BannerDetailForAdminDTO>
                {
                    Data = bannerDto,
                    Message = "Banner details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<BannerDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<BannerDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while get banner detail: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<BannerDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving banner details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddBannerForAdminDTO>> AddBannerForAdminAsync(BannerDTO.AddBannerForAdminDTO addBannerForAdminDTO)
        {
            try
            {
                // Kiểm tra xem có banner nào đã tồn tại với EventDate trùng không
                var existingBanner = await _bannerRepository.GetBannerByEventDateForAdminAsync(addBannerForAdminDTO.EventDate);

                if (existingBanner != null)
                {
                    return new DataResponse<AddBannerForAdminDTO>
                    {
                        Data = null,
                        Message = "A banner with the same EventDate already exists.",
                        StatusCode = 400 // Bad Request
                    };
                }

                var banner = new Banner
                {
                    UserId = addBannerForAdminDTO.UserId,
                    Link = addBannerForAdminDTO.Link,
                    EventDate = addBannerForAdminDTO.EventDate,
                    Image = addBannerForAdminDTO.Image
                };

                var addBannerResult = await _bannerRepository.AddBannerForAdminAsync(banner);

                // Cập nhật thời gian tạo vào DTO trả về
                addBannerForAdminDTO.CreatedAt = addBannerResult.CreatedAt;
                addBannerForAdminDTO.Status = addBannerResult.Status;

                return new DataResponse<AddBannerForAdminDTO>
                {
                    Data = addBannerForAdminDTO,
                    Message = "Banner added successfully!",
                    StatusCode = 201
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<AddBannerForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while add banner: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddBannerForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding parent banner: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateBannerForAdminDTO>> UpdateBannerForAdminAsync(UpdateBannerForAdminDTO updateBannerForAdminDTO)
        {
            try
            {
                // Kiểm tra xem có banner nào đã tồn tại với EventDate trùng không
                var existingBanner = await _bannerRepository.GetBannerByEventDateForAdminAsync(updateBannerForAdminDTO.EventDate);

                if (existingBanner != null && existingBanner.BannerId != updateBannerForAdminDTO.BannerId)
                {
                    return new DataResponse<UpdateBannerForAdminDTO>
                    {
                        Data = null,
                        Message = "Another banner with the same EventDate already exists.",
                        StatusCode = 400 // Bad Request
                    };
                }

                var banner = new Banner
                {
                    BannerId = updateBannerForAdminDTO.BannerId,
                    Link = updateBannerForAdminDTO.Link,
                    EventDate = updateBannerForAdminDTO.EventDate,
                    Image = updateBannerForAdminDTO.Image
                };

                var updatedResult = await _bannerRepository.UpdateBannerForAdminAsync(banner);

                var bannerDto = _mapper.Map<UpdateBannerForAdminDTO>(updatedResult);

                return new DataResponse<UpdateBannerForAdminDTO>
                {
                    Data = bannerDto,
                    Message = "Banner updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateBannerForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateBannerForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while update banner: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateBannerForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating banner: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateBannerStatusForAdminDTO>> UpdateBannerStatusForAdminAsync(UpdateBannerStatusForAdminDTO updateBannerStatusForAdminDTO)
        {
            try
            {
                var banner = new Banner
                {
                    BannerId = updateBannerStatusForAdminDTO.BannerId,
                    Status = updateBannerStatusForAdminDTO.Status
                };

                var updatedBannerStatusResult = await _bannerRepository.UpdateBannerForAdminAsync(banner);

                var bannerDto = _mapper.Map<UpdateBannerStatusForAdminDTO>(updatedBannerStatusResult);

                return new DataResponse<UpdateBannerStatusForAdminDTO>
                {
                    Data = bannerDto,
                    Message = "Banner updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateBannerStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<UpdateBannerStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while updating banner status: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateBannerStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating banner: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteBannerForAdminDTO>> DeleteBannerForAdminAsync(DeleteBannerForAdminDTO deleteBannerForAdminDTO)
        {
            try
            {
                var deletedResult = await _bannerRepository.DeleteBannerForAdminAsync(deleteBannerForAdminDTO.BannerId);

                var bannerDto = _mapper.Map<DeleteBannerForAdminDTO>(deletedResult);

                return new DataResponse<DeleteBannerForAdminDTO>
                {
                    Data = bannerDto,
                    Message = "Banner has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteBannerForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<DeleteBannerForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while hard delete banner: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteBannerForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting banner: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<List<StatusBannerListForAdminDTO>>> GetAllStatusesBannerForAdminAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusBannerListForAdminDTO>
                {
                    new StatusBannerListForAdminDTO { Status = "Displayed" },
                    new StatusBannerListForAdminDTO { Status = "Active" },
                    new StatusBannerListForAdminDTO { Status = "Inactive" },
                };

                return new DataResponse<List<StatusBannerListForAdminDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusBannerListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<List<StatusBannerListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Access denied while get status list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusBannerListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Common
        public async Task<DataResponse<List<BannerListForCommonDTO>>> GetDisplayedBannersAsync()
        {
            try
            {
                var banners = await _bannerRepository.GetDisplayedBannersAsync();

                var responseDto = _mapper.Map<List<BannerListForCommonDTO>>(banners);

                return new DataResponse<List<BannerListForCommonDTO>>
                {
                    Data = responseDto,
                    Message = "Banners retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<BannerListForCommonDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving banners: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
