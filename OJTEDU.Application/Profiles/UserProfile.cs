using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.UserDTO;

namespace OJTEDU.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserReadForAuthDTO>()
               .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
               .ReverseMap();

            CreateMap<User, UserListForAdminDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ReverseMap();

            CreateMap<User, UserDetailForAdminDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ReverseMap();

            CreateMap<User, DeleteUserForAdminDTO>()
                .ReverseMap();

            CreateMap<User, UpdateUserForAdminDTO>()
                .ReverseMap();

            CreateMap<User, UpdateUserStatusForAdminDTO>()
                .ReverseMap();

            CreateMap<User, RestoreUserForAdminDTO>()
                 .ReverseMap();

            CreateMap<User, UserListForDoetDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ReverseMap();

            CreateMap<User, UserDetailForDoetDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ReverseMap();

            CreateMap<User, DeleteUserForDoetDTO>()
                .ReverseMap();

            CreateMap<User, UpdateUserForDoetDTO>()
                .ReverseMap();

            CreateMap<User, UpdateUserStatusForDoetDTO>()
                .ReverseMap();

            CreateMap<User, UserListForCompanyDTO>()
               .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
               .ReverseMap();

            CreateMap<User, UserDetailForCompanyDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ReverseMap();

            CreateMap<User, DeleteUserForCompanyDTO>()
                .ReverseMap();

            CreateMap<User, UpdateUserForCompanyDTO>()
                .ReverseMap();

            CreateMap<User, UpdateUserStatusForCompanyDTO>()
                .ReverseMap();
            //For Dean
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Information, opt => opt.MapFrom(src => src.Information))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<User, LecturerListDto>()
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.Name));
               

            CreateMap<User, LecturerDetailsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.UserCode))
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major != null ? src.Major.Name : null))  // Đảm bảo ánh xạ đúng
                .ForMember(dest => dest.Students, opt => opt.Ignore());

            CreateMap<User, DeanListForAdminDOETDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));
            

            CreateMap<User, DeanDetailsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.Lecturers, opt => opt.Ignore());
        }
    }
}
