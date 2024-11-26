using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IAddressRepository
    {
        // Admin - Province Management
        Task<IEnumerable<Province>> GetAllProvinceAsync(string? name, string? status);
        Task<Province> GetProvinceByIdAsync(int provinceId);
        Task<Province> GetProvinceByNameAsync(string? name);
        Task AddProvinceAsync(Province province);
        Task UpdateProvinceAsync(Province province);
        Task DeleteProvinceAsync(Province province);
        Task<bool> CheckProvinceDependenciesAsync(int provinceId);

        // Admin - District Management
        Task<IEnumerable<District>> GetAllDistrictByProvinceIdAsync(int provinceId, string? name);
        Task<District> GetDistrictByIdAsync(int districtId);
        Task<District> GetDistrictByNameAsync(string? name);
        Task AddDistrictAsync(District district);
        Task UpdateDistrictAsync(District district);
        Task DeleteDistrictAsync(District district);
        Task<bool> CheckDistrictDependenciesAsync(int districtId);

        // Admin - Ward Management
        Task<IEnumerable<Ward>> GetAllWardByDistrictIdAsync(int districtId, string? name);
        Task<Ward> GetWardByIdAsync(int wardId);
        Task<Ward> GetWardByNameAsync(string? name);
        Task AddWardAsync(Ward ward);
        Task UpdateWardAsync(Ward ward);
        Task DeleteWardAsync(Ward ward);
        Task<bool> CheckWardDependenciesAsync(int wardId);

        //
        Task<int> AddAddressAsync(Address address);
        Task<int> UpdateAddressAsync(Address address);
        Task<bool> IsValidAddressAsync(int? wardId, int? districtId, int? provinceId);
    }
}
