using AutoMapper;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AddressDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        public AddressService(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        // Admin - Province Management
        public async Task<DataResponse<PagedResponse<List<ProvinceListForAdminDTO>>>> GetAllProvinceForAdminAsync(string? name, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var provinces = await _addressRepository.GetAllProvinceAsync(name, status);

                var totalProvinces = provinces.Count();
                var totalPages = totalProvinces == 0 ? 1 : (int)Math.Ceiling((double)totalProvinces / pageSize);

                var provinceDtos = totalProvinces > 0 ? _mapper.Map<List<ProvinceListForAdminDTO>>(provinces)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<ProvinceListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<ProvinceListForAdminDTO>>
                {
                    Items = provinceDtos,
                    TotalCount = totalProvinces,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ProvinceListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Province list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ProvinceListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ProvinceListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving province list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ProvinceDetailForAdminDTO>> GetProvinceDetailByIdForAdminAsync(int provinceId)
        {
            try
            {
                var province = await _addressRepository.GetProvinceByIdAsync(provinceId);

                if (province == null)
                {
                    return new DataResponse<ProvinceDetailForAdminDTO>
                    {
                        Data = null,
                        Message = "Province not found!",
                        StatusCode = 404
                    };
                }

                var provinceDto = _mapper.Map<ProvinceDetailForAdminDTO>(province);

                return new DataResponse<ProvinceDetailForAdminDTO>
                {
                    Data = provinceDto,
                    Message = "Province details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ProvinceDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving province details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddProvinceForAdminDTO>> AddProvinceForAdminAsync(AddProvinceForAdminDTO addProvinceForAdminDTO)
        {
            try
            {
                var existingProvince = await _addressRepository.GetProvinceByNameAsync(addProvinceForAdminDTO.ProvinceName);

                if (existingProvince != null)
                {
                    return new DataResponse<AddProvinceForAdminDTO>
                    {
                        Data = null,
                        Message = "Province name already exists!",
                        StatusCode = 400 // Bad Request
                    };
                }

                var province = _mapper.Map<Province>(addProvinceForAdminDTO);
                province.CreatedAt = DateTime.Now;
                province.UpdatedAt = DateTime.Now;
                province.Status = "Active";
                await _addressRepository.AddProvinceAsync(province);

                var addedProvinceDto = _mapper.Map<AddProvinceForAdminDTO>(province);

                return new DataResponse<AddProvinceForAdminDTO>
                {
                    Data = addedProvinceDto,
                    Message = "Province added successfully!",
                    StatusCode = 201 // Created
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddProvinceForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding province: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateProvinceForAdminDTO>> UpdateProvinceForAdminAsync(UpdateProvinceForAdminDTO updateProvinceForAdminDTO)
        {
            try
            {
                var province = await _addressRepository.GetProvinceByIdAsync(updateProvinceForAdminDTO.ProvinceId);

                if (province == null)
                {
                    return new DataResponse<UpdateProvinceForAdminDTO>
                    {
                        Data = null,
                        Message = "Province not found!",
                        StatusCode = 404
                    };
                }

                var existingProvinceWithName = await _addressRepository.GetProvinceByNameAsync(updateProvinceForAdminDTO.ProvinceName);
                if (existingProvinceWithName != null && existingProvinceWithName.ProvinceId != updateProvinceForAdminDTO.ProvinceId)
                {
                    return new DataResponse<UpdateProvinceForAdminDTO>
                    {
                        Data = null,
                        Message = "Province name already exists!",
                        StatusCode = 400
                    };
                }

                province.Name = updateProvinceForAdminDTO.ProvinceName ?? province.Name;
                province.UpdatedAt = DateTime.Now;

                await _addressRepository.UpdateProvinceAsync(province);

                var updatedDto = _mapper.Map<UpdateProvinceForAdminDTO>(province);

                return new DataResponse<UpdateProvinceForAdminDTO>
                {
                    Data = updatedDto,
                    Message = "Province updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateProvinceForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating province: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateProvinceStatusForAdminDTO>> UpdateProvinceStatusForAdminAsync(UpdateProvinceStatusForAdminDTO updateProvinceStatusForAdminDTO)
        {
            try
            {
                var province = await _addressRepository.GetProvinceByIdAsync(updateProvinceStatusForAdminDTO.ProvinceId);

                if (province == null)
                {
                    return new DataResponse<UpdateProvinceStatusForAdminDTO>
                    {
                        Data = null,
                        Message = "Province not found!",
                        StatusCode = 404
                    };
                }

                province.Status = updateProvinceStatusForAdminDTO.Status ?? province.Status;
                province.UpdatedAt = DateTime.Now;
                await _addressRepository.UpdateProvinceAsync(province);

                // Lấy tất cả các District liên kết với Province này
                var districts = await _addressRepository.GetAllDistrictByProvinceIdAsync(province.ProvinceId, null);
                foreach (var district in districts)
                {
                    district.Status = province.Status;
                    district.UpdatedAt = DateTime.Now;
                    await _addressRepository.UpdateDistrictAsync(district);

                    // Lấy tất cả các Ward liên kết với District này và cập nhật trạng thái
                    var wards = await _addressRepository.GetAllWardByDistrictIdAsync(district.DistrictId, null);
                    foreach (var ward in wards)
                    {
                        ward.Status = district.Status;
                        ward.UpdatedAt = DateTime.Now;
                        await _addressRepository.UpdateWardAsync(ward);
                    }
                }

                var updatedDto = _mapper.Map<UpdateProvinceStatusForAdminDTO>(province);

                return new DataResponse<UpdateProvinceStatusForAdminDTO>
                {
                    Data = updatedDto,
                    Message = "Province status updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateProvinceStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating province status: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteProvinceForAdminDTO>> DeleteProvinceForAdminAsync(DeleteProvinceForAdminDTO deleteProvinceForAdminDTO)
        {
            try
            {
                var province = await _addressRepository.GetProvinceByIdAsync(deleteProvinceForAdminDTO.ProvinceId);

                if (province == null)
                {
                    return new DataResponse<DeleteProvinceForAdminDTO>
                    {
                        Data = null,
                        Message = "Province not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra ràng buộc
                bool hasDependencies = await _addressRepository.CheckProvinceDependenciesAsync(province.ProvinceId);

                if (hasDependencies)
                {
                    // Đổi trạng thái thành 'Unactive' thay vì xóa
                    province.Status = "Unactive";
                    province.UpdatedAt = DateTime.Now;
                    await _addressRepository.UpdateProvinceAsync(province);

                    // Lấy tất cả các District liên kết với Province này
                    var districts = await _addressRepository.GetAllDistrictByProvinceIdAsync(province.ProvinceId, null);
                    foreach (var district in districts)
                    {
                        district.Status = "Unactive";
                        district.UpdatedAt = DateTime.Now;
                        await _addressRepository.UpdateDistrictAsync(district);

                        // Lấy tất cả các Ward liên kết với District này và cập nhật trạng thái
                        var wards = await _addressRepository.GetAllWardByDistrictIdAsync(district.DistrictId, null);
                        foreach (var ward in wards)
                        {
                            ward.Status = "Unactive";
                            ward.UpdatedAt = DateTime.Now;
                            await _addressRepository.UpdateWardAsync(ward);
                        }
                    }

                    return new DataResponse<DeleteProvinceForAdminDTO>
                    {
                        Data = _mapper.Map<DeleteProvinceForAdminDTO>(province),
                        Message = "Province is in use and cannot be deleted. The status has been updated to 'Unactive' to disable it while keeping associated data intact.",
                        StatusCode = 200
                    };
                }
                else
                {
                    // Xóa nếu không có ràng buộc
                    await _addressRepository.DeleteProvinceAsync(province);

                    return new DataResponse<DeleteProvinceForAdminDTO>
                    {
                        Data = _mapper.Map<DeleteProvinceForAdminDTO>(province),
                        Message = "Province deleted successfully.",
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteProvinceForAdminDTO>
                {
                    Data = null,
                    Message = $"Error deleting province: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        // Admin - District Management
        public async Task<DataResponse<PagedResponse<List<DistrictListForAdminDTO>>>> GetAllDistrictForAdminAsync(int? provinceId, string? name, int pageNumber, int pageSize)
        {
            try
            {
                var districts = await _addressRepository.GetAllDistrictByProvinceIdAsync(provinceId, name);

                var totalDistricts = districts.Count();
                var totalPages = totalDistricts == 0 ? 1 : (int)Math.Ceiling((double)totalDistricts / pageSize);

                var districtDtos = totalDistricts > 0 ? _mapper.Map<List<DistrictListForAdminDTO>>(districts)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<DistrictListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<DistrictListForAdminDTO>>
                {
                    Items = districtDtos,
                    TotalCount = totalDistricts,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<DistrictListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "District list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<DistrictListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<DistrictListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving province list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DistrictDetailForAdminDTO>> GetDistrictDetailByIdForAdminAsync(int districtId)
        {
            try
            {
                var district = await _addressRepository.GetDistrictByIdAsync(districtId);

                if (district == null)
                {
                    return new DataResponse<DistrictDetailForAdminDTO>
                    {
                        Data = null,
                        Message = "District not found!",
                        StatusCode = 404
                    };
                }

                var districtDto = _mapper.Map<DistrictDetailForAdminDTO>(district);

                return new DataResponse<DistrictDetailForAdminDTO>
                {
                    Data = districtDto,
                    Message = "District details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DistrictDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving district details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddDistrictForAdminDTO>> AddDistrictForAdminAsync(AddDistrictForAdminDTO addDistrictForAdminDTO)
        {
            try
            {
                var existingDistrict = await _addressRepository.GetDistrictByNameAsync(addDistrictForAdminDTO.DistrictName);

                if (existingDistrict != null)
                {
                    return new DataResponse<AddDistrictForAdminDTO>
                    {
                        Data = null,
                        Message = "District name already exists!",
                        StatusCode = 400 // Bad Request
                    };
                }

                var district = _mapper.Map<District>(addDistrictForAdminDTO);
                district.CreatedAt = DateTime.Now;
                district.UpdatedAt = DateTime.Now;
                district.Status = "Active";
                await _addressRepository.AddDistrictAsync(district);

                var addedDistrictDto = _mapper.Map<AddDistrictForAdminDTO>(district);

                return new DataResponse<AddDistrictForAdminDTO>
                {
                    Data = addedDistrictDto,
                    Message = "District added successfully!",
                    StatusCode = 201 // Created
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddDistrictForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding district: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateDistrictForAdminDTO>> UpdateDistrictForAdminAsync(UpdateDistrictForAdminDTO updateDistrictForAdminDTO)
        {
            try
            {
                var district = await _addressRepository.GetDistrictByIdAsync(updateDistrictForAdminDTO.DistrictId);

                if (district == null)
                {
                    return new DataResponse<UpdateDistrictForAdminDTO>
                    {
                        Data = null,
                        Message = "District not found!",
                        StatusCode = 404
                    };
                }

                var existingDistrictWithName = await _addressRepository.GetDistrictByNameAsync(updateDistrictForAdminDTO.DistrictName);
                if (existingDistrictWithName != null && existingDistrictWithName.DistrictId != updateDistrictForAdminDTO.DistrictId)
                {
                    return new DataResponse<UpdateDistrictForAdminDTO>
                    {
                        Data = null,
                        Message = "District name already exists!",
                        StatusCode = 400
                    };
                }

                district.Name = updateDistrictForAdminDTO.DistrictName ?? district.Name;
                district.UpdatedAt = DateTime.Now;

                await _addressRepository.UpdateDistrictAsync(district);

                var updatedDto = _mapper.Map<UpdateDistrictForAdminDTO>(district);

                return new DataResponse<UpdateDistrictForAdminDTO>
                {
                    Data = updatedDto,
                    Message = "District updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDistrictForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating district: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateDistrictStatusForAdminDTO>> UpdateDistrictStatusForAdminAsync(UpdateDistrictStatusForAdminDTO updateDistrictStatusForAdminDTO)
        {
            try
            {
                var district = await _addressRepository.GetDistrictByIdAsync(updateDistrictStatusForAdminDTO.DistrictId);

                if (district == null)
                {
                    return new DataResponse<UpdateDistrictStatusForAdminDTO>
                    {
                        Data = null,
                        Message = "District not found!",
                        StatusCode = 404
                    };
                }

                // Lấy thông tin Province cha của District
                var province = await _addressRepository.GetProvinceByIdAsync(district.ProvinceId.Value);
                if (province == null)
                {
                    return new DataResponse<UpdateDistrictStatusForAdminDTO>
                    {
                        Data = null,
                        Message = "Province not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra nếu District chuyển từ Unactive sang Active trong khi Province đang Unactive
                if (province.Status == "Unactive" && updateDistrictStatusForAdminDTO.Status == "Active")
                {
                    return new DataResponse<UpdateDistrictStatusForAdminDTO>
                    {
                        Data = null,
                        Message = "Cannot update status district to Active when province is Unactive.",
                        StatusCode = 400 // Bad Request
                    };
                }

                district.Status = updateDistrictStatusForAdminDTO.Status ?? district.Status;
                district.UpdatedAt = DateTime.Now;
                await _addressRepository.UpdateDistrictAsync(district);

                // Cập nhật trạng thái của tất cả các Ward liên kết với District này
                var wards = await _addressRepository.GetAllWardByDistrictIdAsync(district.DistrictId, null);
                foreach (var ward in wards)
                {
                    ward.Status = district.Status;
                    ward.UpdatedAt = DateTime.Now;
                    await _addressRepository.UpdateWardAsync(ward);
                }

                var updatedDto = _mapper.Map<UpdateDistrictStatusForAdminDTO>(district);

                return new DataResponse<UpdateDistrictStatusForAdminDTO>
                {
                    Data = updatedDto,
                    Message = "District status updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDistrictStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating district status: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteDistrictForAdminDTO>> DeleteDistrictForAdminAsync(DeleteDistrictForAdminDTO deleteDistrictForAdminDTO)
        {
            try
            {
                var district = await _addressRepository.GetDistrictByIdAsync(deleteDistrictForAdminDTO.DistrictId);

                if (district == null)
                {
                    return new DataResponse<DeleteDistrictForAdminDTO>
                    {
                        Data = null,
                        Message = "District not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra ràng buộc
                bool hasDependencies = await _addressRepository.CheckDistrictDependenciesAsync(district.DistrictId);

                if (hasDependencies)
                {
                    // Đổi trạng thái thành 'Unactive' thay vì xóa
                    district.Status = "Unactive";
                    district.UpdatedAt = DateTime.Now;
                    await _addressRepository.UpdateDistrictAsync(district);

                    var wards = await _addressRepository.GetAllWardByDistrictIdAsync(district.DistrictId, null);
                    foreach (var ward in wards)
                    {
                        ward.Status = "Unactive";
                        ward.UpdatedAt = DateTime.Now;
                        await _addressRepository.UpdateWardAsync(ward);
                    }

                    return new DataResponse<DeleteDistrictForAdminDTO>
                    {
                        Data = _mapper.Map<DeleteDistrictForAdminDTO>(district),
                        Message = "District is in use and cannot be deleted. The status has been updated to 'Unactive' to disable it while keeping associated data intact.",
                        StatusCode = 200
                    };
                }
                else
                {
                    // Xóa nếu không có ràng buộc
                    await _addressRepository.DeleteDistrictAsync(district);

                    return new DataResponse<DeleteDistrictForAdminDTO>
                    {
                        Data = _mapper.Map<DeleteDistrictForAdminDTO>(district),
                        Message = "District deleted successfully.",
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteDistrictForAdminDTO>
                {
                    Data = null,
                    Message = $"Error deleting district: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        // Admin - Ward Management
        public async Task<DataResponse<PagedResponse<List<WardListForAdminDTO>>>> GetAllWardForAdminAsync(int? districtId, string? name, int pageNumber, int pageSize)
        {
            try
            {
                var wards = await _addressRepository.GetAllWardByDistrictIdAsync(districtId, name);

                var totalWards = wards.Count();
                var totalPages = totalWards == 0 ? 1 : (int)Math.Ceiling((double)totalWards / pageSize);

                var wardDtos = totalWards > 0 ? _mapper.Map<List<WardListForAdminDTO>>(wards)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<WardListForAdminDTO>();

                var pagedResponse = new PagedResponse<List<WardListForAdminDTO>>
                {
                    Items = wardDtos,
                    TotalCount = totalWards,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<WardListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Ward list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<WardListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<WardListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving ward list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<WardDetailForAdminDTO>> GetWardDetailByIdForAdminAsync(int wardId)
        {
            try
            {
                var ward = await _addressRepository.GetWardByIdAsync(wardId);

                if (ward == null)
                {
                    return new DataResponse<WardDetailForAdminDTO>
                    {
                        Data = null,
                        Message = "District not found!",
                        StatusCode = 404
                    };
                }

                var wardDto = _mapper.Map<WardDetailForAdminDTO>(ward);

                return new DataResponse<WardDetailForAdminDTO>
                {
                    Data = wardDto,
                    Message = "Ward details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<WardDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving ward details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddWardForAdminDTO>> AddWardForAdminAsync(AddWardForAdminDTO addWardForAdminDTO)
        {
            try
            {
                var existingWard = await _addressRepository.GetWardByNameAsync(addWardForAdminDTO.WardName);

                if (existingWard != null)
                {
                    return new DataResponse<AddWardForAdminDTO>
                    {
                        Data = null,
                        Message = "Ward name already exists!",
                        StatusCode = 400 // Bad Request
                    };
                }

                var ward = _mapper.Map<Ward>(addWardForAdminDTO);
                ward.CreatedAt = DateTime.Now;
                ward.UpdatedAt = DateTime.Now;
                ward.Status = "Active";
                await _addressRepository.AddWardAsync(ward);

                var addedWardDto = _mapper.Map<AddWardForAdminDTO>(ward);

                return new DataResponse<AddWardForAdminDTO>
                {
                    Data = addedWardDto,
                    Message = "Ward added successfully!",
                    StatusCode = 201 // Created
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddWardForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding ward: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateWardForAdminDTO>> UpdateWardForAdminAsync(UpdateWardForAdminDTO updateWardForAdminDTO)
        {
            try
            {
                var ward = await _addressRepository.GetWardByIdAsync(updateWardForAdminDTO.WardId);

                if (ward == null)
                {
                    return new DataResponse<UpdateWardForAdminDTO>
                    {
                        Data = null,
                        Message = "Ward not found!",
                        StatusCode = 404
                    };
                }

                var existingWardWithName = await _addressRepository.GetWardByNameAsync(updateWardForAdminDTO.WardName);
                if (existingWardWithName != null && existingWardWithName.WardId != updateWardForAdminDTO.WardId)
                {
                    return new DataResponse<UpdateWardForAdminDTO>
                    {
                        Data = null,
                        Message = "Ward name already exists!",
                        StatusCode = 400
                    };
                }

                ward.Name = updateWardForAdminDTO.WardName ?? ward.Name;
                ward.UpdatedAt = DateTime.Now;

                await _addressRepository.UpdateWardAsync(ward);

                var updatedDto = _mapper.Map<UpdateWardForAdminDTO>(ward);

                return new DataResponse<UpdateWardForAdminDTO>
                {
                    Data = updatedDto,
                    Message = "Ward updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateWardForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating ward: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateWardStatusForAdminDTO>> UpdateWardStatusForAdminAsync(UpdateWardStatusForAdminDTO updateWardStatusForAdminDTO)
        {
            try
            {
                var ward = await _addressRepository.GetWardByIdAsync(updateWardStatusForAdminDTO.WardId);

                if (ward == null)
                {
                    return new DataResponse<UpdateWardStatusForAdminDTO>
                    {
                        Data = null,
                        Message = "Ward not found!",
                        StatusCode = 404
                    };
                }

                var district = await _addressRepository.GetDistrictByIdAsync(ward.DistrictId.Value);
                if (district == null)
                {
                    return new DataResponse<UpdateWardStatusForAdminDTO>
                    {
                        Data = null,
                        Message = "District not found!",
                        StatusCode = 404
                    };
                }

                if (district.Status == "Unactive" && updateWardStatusForAdminDTO.Status == "Active")
                {
                    return new DataResponse<UpdateWardStatusForAdminDTO>
                    {
                        Data = null,
                        Message = "Cannot update status ward to Active when district is Unactive.",
                        StatusCode = 400 // Bad Request
                    };
                }

                ward.Status = updateWardStatusForAdminDTO.Status ?? ward.Status;
                ward.UpdatedAt = DateTime.Now;
                await _addressRepository.UpdateWardAsync(ward);

                var updatedDto = _mapper.Map<UpdateWardStatusForAdminDTO>(ward);

                return new DataResponse<UpdateWardStatusForAdminDTO>
                {
                    Data = updatedDto,
                    Message = "Ward status updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateWardStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating ward status: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteWardForAdminDTO>> DeleteWardForAdminAsync(DeleteWardForAdminDTO deleteWardForAdminDTO)
        {
            try
            {
                var ward = await _addressRepository.GetWardByIdAsync(deleteWardForAdminDTO.WardId);

                if (ward == null)
                {
                    return new DataResponse<DeleteWardForAdminDTO>
                    {
                        Data = null,
                        Message = "Ward not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra ràng buộc
                bool hasDependencies = await _addressRepository.CheckWardDependenciesAsync(ward.WardId);

                if (hasDependencies)
                {
                    // Đổi trạng thái thành 'Unactive' thay vì xóa
                    ward.Status = "Unactive";
                    ward.UpdatedAt = DateTime.Now;
                    await _addressRepository.UpdateWardAsync(ward);

                    return new DataResponse<DeleteWardForAdminDTO>
                    {
                        Data = _mapper.Map<DeleteWardForAdminDTO>(ward),
                        Message = "Ward is in use and cannot be deleted. The status has been updated to 'Unactive' to disable it while keeping associated data intact.",
                        StatusCode = 200
                    };
                }
                else
                {
                    // Xóa nếu không có ràng buộc
                    await _addressRepository.DeleteWardAsync(ward);

                    return new DataResponse<DeleteWardForAdminDTO>
                    {
                        Data = _mapper.Map<DeleteWardForAdminDTO>(ward),
                        Message = "Ward deleted successfully.",
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteWardForAdminDTO>
                {
                    Data = null,
                    Message = $"Error deleting ward: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        // Admin - Status List
        public async Task<DataResponse<List<StatusAddressListForAdminDTO>>> GetAllStatusesAddressForAdminAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusAddressListForAdminDTO>
                {
                    new StatusAddressListForAdminDTO { Status = "Active" },
                    new StatusAddressListForAdminDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusAddressListForAdminDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusAddressListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusAddressListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        public async Task<DataResponse<MemoryStream>> GenerateAddressTemplateAsync()
        {
            try
            {
                var memoryStream = new MemoryStream();
                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Address Template");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Province Name(*)";
                    worksheet.Cells[1, 2].Value = "District Name(*)";
                    worksheet.Cells[1, 3].Value = "Ward Name(*)";

                    for (int col = 1; col <= 3; col++)
                    {
                        worksheet.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, col].Style.Font.Bold = true; // Đặt chữ in đậm
                    }

                    // Example rows
                    worksheet.Cells[2, 1].Value = "Hà Nội";
                    worksheet.Cells[2, 2].Value = "Ba Đình";
                    worksheet.Cells[2, 3].Value = "Phúc Xá";

                    worksheet.Cells[3, 1].Value = "Phú Thọ";
                    worksheet.Cells[3, 2].Value = "Việt Trì";
                    worksheet.Cells[3, 3].Value = "Thọ Sơn";

                    // Căn giữa cho tất cả các ô
                    for (int row = 2; row <= 1000; row++)
                    {
                        for (int col = 1; col <= 3; col++)
                        {
                            worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }

                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 30;

                    // Thêm ghi chú cho các trường
                    worksheet.Cells[1, 5].Value = "Ghi chú:"; // Cột 6
                    worksheet.Cells[2, 5].Value = "(*) : Bắt buộc điền."; // Cột 6
                    worksheet.Cells[3, 5].Value = "Hãy xóa dữ liệu mẫu trước khi điền tránh trùng lặp."; // Cột 6

                    for (int row = 1; row <= 3; row++)
                    {
                        worksheet.Cells[row, 5].Style.Font.Bold = true;
                        worksheet.Cells[row, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }

                    package.Save();
                }

                memoryStream.Position = 0;
                return new DataResponse<MemoryStream>
                {
                    Data = memoryStream,
                    Message = "Template generated successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"Error generating template: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        public async Task<DataResponse<object>> ImportAddressFileAsync(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (file == null || file.Length == 0)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = "File is empty or not provided.",
                    StatusCode = 400
                };
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return new DataResponse<object>
                            {
                                Data = null,
                                Message = "The uploaded file does not contain valid data.",
                                StatusCode = 400
                            };
                        }

                        var errorMessages = new List<string>();
                        var rowTracker = new Dictionary<string, int>(); // Track duplicates in the file
                        var validRows = new List<(string Province, string District, string Ward)>();
                        int lastRow = 1;
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            bool hasDataInRange = false;
                            for (int col = 1; col <= 3; col++) // Chỉ từ cột A đến E (1 đến 6)
                            {
                                if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col]?.Text))
                                {
                                    hasDataInRange = true;
                                    break;
                                }
                            }
                            if (hasDataInRange)
                            {
                                lastRow = row;
                            }
                        }

                        // Step 1: Validate rows and check for duplicates in the file
                        for (int row = 2; row <= lastRow; row++)
                        {
                            var provinceName = worksheet.Cells[row, 1]?.Text.Trim();
                            var districtName = worksheet.Cells[row, 2]?.Text.Trim();
                            var wardName = worksheet.Cells[row, 3]?.Text.Trim();

                            // Validate required fields
                            if (string.IsNullOrEmpty(provinceName) || string.IsNullOrEmpty(districtName) || string.IsNullOrEmpty(wardName))
                            {
                                errorMessages.Add($"Row {row}: Missing required data (Province, District, or Ward).");
                                continue;
                            }

                            // Check for duplicate rows in the file
                            var uniqueKey = $"{provinceName}|{districtName}|{wardName}";
                            if (rowTracker.ContainsKey(uniqueKey))
                            {
                                var firstOccurrenceRow = rowTracker[uniqueKey];
                                errorMessages.Add($"Row {row}: Duplicate entry in the file. Matches Row {firstOccurrenceRow} (Province: '{provinceName}', District: '{districtName}', Ward: '{wardName}').");
                                continue;
                            }
                            else
                            {
                                rowTracker[uniqueKey] = row;
                            }

                            // Add to valid rows for database validation
                            validRows.Add((provinceName, districtName, wardName));
                        }

                        // Step 2: Validate rows against the database
                        foreach (var (provinceName, districtName, wardName) in validRows)
                        {
                            // Check if Province exists
                            var provinceEntity = await _addressRepository.GetProvinceByNameAsync(provinceName);
                            if (provinceEntity != null)
                            {
                                // Check if District exists under the Province
                                var districtEntity = await _addressRepository.GetDistrictByNameAndProvinceIdAsync(districtName, provinceEntity.ProvinceId);
                                if (districtEntity != null)
                                {
                                    // Check if Ward exists under the District
                                    var wardEntity = await _addressRepository.GetWardByNameAndDistrictIdAsync(wardName, districtEntity.DistrictId);
                                    if (wardEntity != null)
                                    {
                                        errorMessages.Add($"Row {rowTracker[$"{provinceName}|{districtName}|{wardName}"]}: Ward '{wardName}' already exists under District '{districtName}' in Province '{provinceName}'.");
                                    }
                                }
                            }
                        }

                        // If there are any errors, return them and halt the process
                        if (errorMessages.Any())
                        {
                            return new DataResponse<object>
                            {
                                Data = new
                                {
                                    SuccessCount = 0,
                                    ErrorCount = errorMessages.Count,
                                    Errors = errorMessages
                                },
                                Message = $"Import failed. There were {errorMessages.Count} errors. Please fix the reported errors to successfully add the file.",
                                StatusCode = 400
                            };
                        }

                        // Step 3: Process valid rows and add to the database
                        int successCount = 0;
                        foreach (var (provinceName, districtName, wardName) in validRows)
                        {
                            // Add Province if it doesn't exist
                            var provinceEntity = await _addressRepository.GetProvinceByNameAsync(provinceName) ?? await _addressRepository.AddProvince1Async(new Province
                            {
                                Name = provinceName,
                                Status = "Active",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            });

                            // Add District if it doesn't exist
                            var districtEntity = await _addressRepository.GetDistrictByNameAndProvinceIdAsync(districtName, provinceEntity.ProvinceId) ?? await _addressRepository.AddDistrict1Async(new District
                            {
                                Name = districtName,
                                ProvinceId = provinceEntity.ProvinceId,
                                Status = "Active",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            });

                            // Add Ward if it doesn't exist
                            await _addressRepository.AddWardAsync(new Ward
                            {
                                Name = wardName,
                                DistrictId = districtEntity.DistrictId,
                                Status = "Active",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            });

                            successCount++;
                        }

                        return new DataResponse<object>
                        {
                            Data = new
                            {
                                SuccessCount = successCount,
                                ErrorCount = 0,
                                Errors = errorMessages
                            },
                            Message = $"Import completed. Successfully added {successCount} addresses.",
                            StatusCode = 200
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = $"Error occurred during import: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
