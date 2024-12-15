using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static OJTEDU.Application.DTOs.DistrictDTO;
using static OJTEDU.Application.DTOs.DocumentDTO;
using static OJTEDU.Application.DTOs.ProvinceDTO;
using static OJTEDU.Application.DTOs.WardDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;
        private readonly IDistrictRepository _districtRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IMapper _mapper;
        public ProvinceService(IProvinceRepository provinceRepository,
            IDistrictRepository districtRepository,
            IWardRepository wardRepository, IMapper mapper)
        {
            _provinceRepository = provinceRepository;
            _districtRepository = districtRepository;
            _wardRepository = wardRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse<LocationListForCommonDTO>> GetAllLocationsAsync(int? provinceId, int? districtId)
        {
            try
            {
                var response = new LocationListForCommonDTO();

                if (!provinceId.HasValue && !districtId.HasValue)
                {
                    // Province list
                    var provinceList = await _provinceRepository.GetAllProvincesAsync();
                    response.ProvinceList = _mapper.Map<List<ProvinceListForCommonDTO>>(provinceList);
                }
                else if (provinceId.HasValue && !districtId.HasValue)
                {
                    // Province list
                    var provinceList = await _provinceRepository.GetAllProvincesAsync();
                    response.ProvinceList = _mapper.Map<List<ProvinceListForCommonDTO>>(provinceList);

                    // District list 
                    var districtList = await _districtRepository.GetAllDistrictsByProvinceIdAsync(provinceId);
                    response.DistrictList = _mapper.Map<List<DistrictListForCommonDTO>>(districtList);
                }
                else if (provinceId.HasValue && districtId.HasValue)
                {
                    // Province list 
                    var provinceList = await _provinceRepository.GetAllProvincesAsync();
                    response.ProvinceList = _mapper.Map<List<ProvinceListForCommonDTO>>(provinceList);

                    // District list 
                    var districtList = await _districtRepository.GetAllDistrictsByProvinceIdAsync(provinceId);
                    response.DistrictList = _mapper.Map<List<DistrictListForCommonDTO>>(districtList);

                    // Ward list 
                    var wardList = await _wardRepository.GetAllWardsByDistrictIdAsync(districtId);
                    response.WardList = _mapper.Map<List<WardListForCommonDTO>>(wardList);
                }

                return new DataResponse<LocationListForCommonDTO>
                {
                    StatusCode = 200,
                    Message = "Location list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<LocationListForCommonDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving location list: {ex.Message}.",
                    Data = null
                };
            }
        }
    }
}
