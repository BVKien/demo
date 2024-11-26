using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.PolicyDTO;
using static OJTEDU.Application.DTOs.RoleDTO;

namespace OJTEDU.Application.Profiles
{
    public class PolicyProfile : Profile
    {
        public PolicyProfile()
        {
            // Admin - Policy
            CreateMap<Policy, ParentPolicyListForAdminDTO>()
                .ForMember(dest => dest.ParentPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ParentPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.PolicyRoles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<Policy, ParentPolicyDetailForAdminDTO>()
                .ForMember(dest => dest.ParentPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.PolicyRoles))
                .ForMember(dest => dest.ParentPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            // Map từ PolicyRole sang RoleListDTO
            CreateMap<PolicyRole, RoleListDTO>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.RoleId == null || src.RoleId == 0 ? "Guest" : src.Role.Name));

            CreateMap<Policy, AddParentPolicyForAdminDTO>()
                .ForMember(dest => dest.ParentPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.PolicyRoles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Policy, DeleteParentPolicyForAdminDTO>()
                .ForMember(dest => dest.ParentPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.ParentPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ReverseMap();

            CreateMap<Policy, UpdateParentPolicyForAdminDTO>()
                .ForMember(dest => dest.ParentPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.PolicyRoles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ForMember(dest => dest.ParentPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Policy, UpdateParentPolicyStatusForAdminDTO>()
                .ForMember(dest => dest.ParentPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.ParentPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Policy, ChildPolicyListForAdminDTO>()
                .ForMember(dest => dest.ChildPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.PolicyRoles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<Policy, ChildPolicyDetailForAdminDTO>()
                .ForMember(dest => dest.ChildPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Policy, AddChildPolicyForAdminDTO>()
                .ForMember(dest => dest.ChildPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Policy, DeleteChildPolicyForAdminDTO>()
                .ForMember(dest => dest.ChildPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.ChildPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ReverseMap();

            CreateMap<Policy, UpdateChildPolicyForAdminDTO>()
                .ForMember(dest => dest.ChildPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.ChildPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Policy, UpdateChildPolicyStatusForAdminDTO>()
                .ForMember(dest => dest.ChildPolicyId, opt => opt.MapFrom(src => src.PolicyId))
                .ForMember(dest => dest.ChildPolicycontent, opt => opt.MapFrom(src => src.PolicyContent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            // Common 
            CreateMap<Policy, PolicyListForCommonDTO>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Policy, PolicyDetailForCommonDTO>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();
        }
    }
}
