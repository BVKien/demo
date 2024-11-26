using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.BannerDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IBannerService
    {
        // Admin - Banner Management
        Task<DataResponse<PagedResponse<List<BannerListForAdminDTO>>>> GetAllBannerForAdminAsync(DateTime? startEventDate, DateTime? endEventDate, string? status, int pageNumber, int pageSize);
        Task<DataResponse<BannerDetailForAdminDTO>> GetBannerDetailByIdForAdminAsync(int bannerId);
        Task<DataResponse<AddBannerForAdminDTO>> AddBannerForAdminAsync(AddBannerForAdminDTO addBannerForAdminDTO);
        Task<DataResponse<UpdateBannerForAdminDTO>> UpdateBannerForAdminAsync(UpdateBannerForAdminDTO updateBannerForAdminDTO);
        Task<DataResponse<UpdateBannerStatusForAdminDTO>> UpdateBannerStatusForAdminAsync(UpdateBannerStatusForAdminDTO updateBannerStatusForAdminDTO);
        Task<DataResponse<DeleteBannerForAdminDTO>> DeleteBannerForAdminAsync(DeleteBannerForAdminDTO deleteBannerForAdminDTO);
        Task<DataResponse<List<StatusBannerListForAdminDTO>>> GetAllStatusesBannerForAdminAsync();

        // Common 
        Task<DataResponse<List<BannerListForCommonDTO>>> GetDisplayedBannersAsync();
    }
}
