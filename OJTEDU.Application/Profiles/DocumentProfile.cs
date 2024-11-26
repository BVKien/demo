using AutoMapper;
using OJTEDU.Domain.Entities;
using static OJTEDU.Application.DTOs.DocumentDTO;
using static OJTEDU.Application.DTOs.RoleDTO;

namespace OJTEDU.Application.Profiles
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            // Admin 
            CreateMap<Document, DocumentListForAdminDTO>()
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.DocumentRoles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University.Name))
                .ReverseMap();

            CreateMap<Document, DocumentDetailForAdminDTO>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.DocumentRoles))
                .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University.Name))
                .ReverseMap();

            // Map từ DocumentRole sang RoleListDTO
            CreateMap<DocumentRole, RoleListDTO>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.RoleId == null || src.RoleId == 0 ? "Guest" : src.Role.Name));

            CreateMap<Document, DeleteDocumentForAdminDTO>()
                .ReverseMap();

            CreateMap<Document, AddDocumentForAdminDTO>()
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.DocumentRoles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<Document, UpdateDocumentForAdminDTO>()
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.DocumentRoles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<Document, UpdateDocumentStatusForAdminDTO>()
                .ReverseMap();


            // Doet
            CreateMap<Document, DocumentListForDoetDTO>()
                .ForMember(dest => dest.ForRole, opt => opt.MapFrom(src =>
                    string.Join(", ", src.DocumentRoles.Select(dr => dr.Role.Name ?? "Guest")))) // Thêm "Guest" nếu Role.Name = null
                .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University.Name))
                .ReverseMap();

            CreateMap<Document, DocumentDetailForDoetDTO>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.DocumentRoles))
                .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University.Name))
                .ReverseMap();

            CreateMap<Document, DeleteDocumentForDoetDTO>()
                .ReverseMap();

            CreateMap<Document, AddDocumentForDoetDTO>()
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.DocumentRoles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<Document, UpdateDocumentForDoetDTO>()
                .ForMember(dest => dest.ForRoleIds, opt =>
                    opt.MapFrom(src => src.DocumentRoles
                                          .Select(dr => dr.RoleId ?? 0) // Nếu RoleId là null, thay bằng 0
                                          .ToList()))
                .ReverseMap();

            CreateMap<Document, UpdateDocumentStatusForDoetDTO>()
                .ReverseMap();

            // Common
            CreateMap<Document, DocumentListForCommonDTO>()
                .ReverseMap();

            CreateMap<Document, DocumentDetailForCommonDTO>()
                .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University.Name))
                .ReverseMap();


            // Guest 
            //CreateMap<Document, DocumentInternshipProcessForGuestDTO>()
            //    .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.University.Name))
            //    .ReverseMap();

            // Company
            CreateMap<Document, DocumentTestFilesListForCompanyDTO>().ReverseMap();
            CreateMap<Document, CreateDocumentTestFilesForCompanyDTO>().ReverseMap();
            CreateMap<Document, UpdateDocumentTestFilesForCompanyDTO>().ReverseMap();
        }
    }
}
