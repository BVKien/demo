using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class AddressDTO
    {
        // Admin - Province
        public class ProvinceListForAdminDTO
        {
            public int ProvinceId { get; set; }
            public string? ProvinceName { get; set; }
            public string? Status { get; set; }
        }

        public class ProvinceDetailForAdminDTO
        {
            public int ProvinceId { get; set; }
            public string? ProvinceName { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddProvinceForAdminDTO
        {
            public string? ProvinceName { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateProvinceForAdminDTO
        {
            public int ProvinceId { get; set; }
            public string? ProvinceName { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateProvinceStatusForAdminDTO
        {
            public int ProvinceId { get; set; }
            public string? ProvinceName { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteProvinceForAdminDTO
        {
            public int ProvinceId { get; set; }
            public string? ProvinceName { get; set; }
            public string? Status { get; set; }
        }

        public class StatusAddressListForAdminDTO
        {
            public string? Status { get; set; }
        }

        // Admin - District
        public class DistrictListForAdminDTO
        {
            public int? DistrictId { get; set; }
            public string? DistrictName { get; set; }
            public string? ParentProvinceName { get; set; }
            public string? Status { get; set; }
        }

        public class DistrictDetailForAdminDTO
        {
            public int DistrictId { get; set; }
            public string? DistrictName { get; set; }
            public int? ProvinceId { get; set; }
            public string? ParentProvinceName { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddDistrictForAdminDTO
        {
            public string? DistrictName { get; set; }
            public int? ProvinceId { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateDistrictForAdminDTO
        {
            public int DistrictId { get; set; }
            public string? DistrictName { get; set; }
            public int? ProvinceId { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateDistrictStatusForAdminDTO
        {
            public int DistrictId { get; set; }
            public string? DistrictName { get; set; }
            public int? ProvinceId { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteDistrictForAdminDTO
        {
            public int DistrictId { get; set; }
            public string? DistrictName { get; set; }
            public int? ProvinceId { get; set; }
            public string? Status { get; set; }
        }

        // Admin - Ward
        public class WardListForAdminDTO
        {
            public int? WardId { get; set; }
            public string? WardName { get; set; }
            public string? ParentDistrictName { get; set; }
            public string? Status { get; set; }
        }

        public class WardDetailForAdminDTO
        {
            public int WardId { get; set; }
            public string? WardName { get; set; }
            public int? DistrictId { get; set; }
            public string? ParentDistrictName { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AddWardForAdminDTO
        {
            public string? WardName { get; set; }
            public int? DistrictId { get; set; }
            public string? Status { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class UpdateWardForAdminDTO
        {
            public int WardId { get; set; }
            public string? WardName { get; set; }
            public int? DistrictId { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateWardStatusForAdminDTO
        {
            public int WardId { get; set; }
            public string? WardName { get; set; }
            public int? DistrictId { get; set; }
            public string? Status { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class DeleteWardForAdminDTO
        {
            public int WardId { get; set; }
            public string? WardName { get; set; }
            public int? DistrictId { get; set; }
            public string? Status { get; set; }
        }

        public class AddressForCompanyDTO
        {
            public int AddressId { get; set; }
            public int? WardId { get; set; }
            public int? DistrictId { get; set; }
            public int? ProvinceId { get; set; }
            public string? Detail { get; set; }
        }
    }
}
