using Microsoft.AspNetCore.Http;
using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AddressDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IAddressService
    {
        // Admin - Status List
        Task<DataResponse<List<StatusAddressListForAdminDTO>>> GetAllStatusesAddressForAdminAsync();

        // Admin - Province Management
        Task<DataResponse<PagedResponse<List<ProvinceListForAdminDTO>>>> GetAllProvinceForAdminAsync(string? name, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ProvinceDetailForAdminDTO>> GetProvinceDetailByIdForAdminAsync(int provinceId);
        Task<DataResponse<AddProvinceForAdminDTO>> AddProvinceForAdminAsync(AddProvinceForAdminDTO addProvinceForAdminDTO);
        Task<DataResponse<UpdateProvinceForAdminDTO>> UpdateProvinceForAdminAsync(UpdateProvinceForAdminDTO updateProvinceForAdminDTO);
        Task<DataResponse<UpdateProvinceStatusForAdminDTO>> UpdateProvinceStatusForAdminAsync(UpdateProvinceStatusForAdminDTO updateProvinceStatusForAdminDTO);
        Task<DataResponse<DeleteProvinceForAdminDTO>> DeleteProvinceForAdminAsync(DeleteProvinceForAdminDTO deleteProvinceForAdminDTO);
        Task<DataResponse<MemoryStream>> GenerateAddressTemplateAsync();
        Task<DataResponse<object>> ImportAddressFileAsync(IFormFile file);

        // Admin - District Management
        Task<DataResponse<PagedResponse<List<DistrictListForAdminDTO>>>> GetAllDistrictForAdminAsync(int? provinceId, string? name, int pageNumber, int pageSize);
        Task<DataResponse<DistrictDetailForAdminDTO>> GetDistrictDetailByIdForAdminAsync(int districtId);
        Task<DataResponse<AddDistrictForAdminDTO>> AddDistrictForAdminAsync(AddDistrictForAdminDTO addDistrictForAdminDTO);
        Task<DataResponse<UpdateDistrictForAdminDTO>> UpdateDistrictForAdminAsync(UpdateDistrictForAdminDTO updateDistrictForAdminDTO);
        Task<DataResponse<UpdateDistrictStatusForAdminDTO>> UpdateDistrictStatusForAdminAsync(UpdateDistrictStatusForAdminDTO updateDistrictStatusForAdminDTO);
        Task<DataResponse<DeleteDistrictForAdminDTO>> DeleteDistrictForAdminAsync(DeleteDistrictForAdminDTO deleteDistrictForAdminDTO);

        // Admin - Ward Management
        Task<DataResponse<PagedResponse<List<WardListForAdminDTO>>>> GetAllWardForAdminAsync(int? districtId, string? name, int pageNumber, int pageSize);
        Task<DataResponse<WardDetailForAdminDTO>> GetWardDetailByIdForAdminAsync(int wardId);
        Task<DataResponse<AddWardForAdminDTO>> AddWardForAdminAsync(AddWardForAdminDTO addWardForAdminDTO);
        Task<DataResponse<UpdateWardForAdminDTO>> UpdateWardForAdminAsync(UpdateWardForAdminDTO updateWardForAdminDTO);
        Task<DataResponse<UpdateWardStatusForAdminDTO>> UpdateWardStatusForAdminAsync(UpdateWardStatusForAdminDTO updateWardStatusForAdminDTO);
        Task<DataResponse<DeleteWardForAdminDTO>> DeleteWardForAdminAsync(DeleteWardForAdminDTO deleteWardForAdminDTO);
    }
}
