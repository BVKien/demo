using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.NewsFaqDTO;
using static OJTEDU.Application.DTOs.RoleDTO;

namespace OJTEDU.Application.Profiles
{
    public class NewsFaqProfile : Profile
    {
        public NewsFaqProfile()
        {
            // Map từ NewsFaqrole sang RoleListDTO
            CreateMap<NewsFaqrole, RoleListDTO>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.RoleId == null || src.RoleId == 0 ? "Guest" : src.Role.Name));

            // Admin - News
            CreateMap<NewsFaq, ParentNewsListForAdminDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.NewsFaqroles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<NewsFaq, ParentNewsDetailForAdminDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.NewsFaqroles))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, AddParentNewsForAdminDTO>()
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.NewsFaqroles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<NewsFaq, DeleteParentNewsForAdminDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateParentNewsForAdminDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.NewsFaqroles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateParentNewsStatusForAdminDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, ChildNewsListForAdminDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.NewsFaqroles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<NewsFaq, ChildNewsDetailForAdminDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, AddChildNewsForAdminDTO>()
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, DeleteChildNewsForAdminDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateChildNewsForAdminDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateChildNewsStatusForAdminDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            // Admin - Faq

            CreateMap<NewsFaq, ParentFaqListForAdminDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.NewsFaqroles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<NewsFaq, ParentFaqDetailForAdminDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.NewsFaqroles))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, AddParentFaqForAdminDTO>()
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.NewsFaqroles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<NewsFaq, DeleteParentFaqForAdminDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateParentFaqForAdminDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.NewsFaqroles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateParentFaqStatusForAdminDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, ChildFaqListForAdminDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.NewsFaqroles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<NewsFaq, ChildFaqDetailForAdminDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, AddChildFaqForAdminDTO>()
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, DeleteChildFaqForAdminDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateChildFaqForAdminDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateChildFaqStatusForAdminDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            // Doet - News
            CreateMap<NewsFaq, ParentNewsListForDoetDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.NewsFaqroles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<NewsFaq, ParentNewsDetailForDoetDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.NewsFaqroles))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, AddParentNewsForDoetDTO>()
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.NewsFaqroles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<NewsFaq, DeleteParentNewsForDoetDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateParentNewsForDoetDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.NewsFaqroles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateParentNewsStatusForDoetDTO>()
                .ForMember(dest => dest.ParentNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, ChildNewsListForDoetDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.NewsFaqroles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<NewsFaq, ChildNewsDetailForDoetDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, AddChildNewsForDoetDTO>()
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, DeleteChildNewsForDoetDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateChildNewsForDoetDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateChildNewsStatusForDoetDTO>()
                .ForMember(dest => dest.ChildNewsId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildNewscontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            // DOet - Faq

            CreateMap<NewsFaq, ParentFaqListForDoetDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.NewsFaqroles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<NewsFaq, ParentFaqDetailForDoetDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.NewsFaqroles))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, AddParentFaqForDoetDTO>()
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.NewsFaqroles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<NewsFaq, DeleteParentFaqForDoetDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateParentFaqForDoetDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.NewsFaqroles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateParentFaqStatusForDoetDTO>()
                .ForMember(dest => dest.ParentFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ParentFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, ChildFaqListForDoetDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.NewsFaqroles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ReverseMap();

            CreateMap<NewsFaq, ChildFaqDetailForDoetDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, AddChildFaqForDoetDTO>()
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, DeleteChildFaqForDoetDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateChildFaqForDoetDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, UpdateChildFaqStatusForDoetDTO>()
                .ForMember(dest => dest.ChildFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.ChildFaqcontent, opt => opt.MapFrom(src => src.NewsFaqcontent))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            // Common 
            CreateMap<NewsFaq, NewsFaqListForCommonDTO>()
                .ForMember(dest => dest.NewsFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<NewsFaq, NewsFaqDetailForCommonDTO>()
                .ForMember(dest => dest.NewsFaqId, opt => opt.MapFrom(src => src.NewsFaqid))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();
        }
    }
}
