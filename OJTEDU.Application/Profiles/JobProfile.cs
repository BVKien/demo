using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using static OJTEDU.Application.DTOs.JobDTO;

namespace OJTEDU.Application.Profiles
{
    public class JobProfile : Profile
    {
        public JobProfile()
        {
            // Admin - DOET
            CreateMap<Job, JobListByCompanyIdForAdminDooetDTO>()
               .ReverseMap();

            // Student
            CreateMap<Job, JobListByCompanyIdForStudentDTO>()
                .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline.HasValue ? src.Deadline.Value.ToString("dd-MM-yyyy") : null))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AddressedNavigation.Province.Name))
                .ReverseMap();

            CreateMap<Job, JobListSearchForStudentDTO>()
                .ForMember(dest => dest.CompanyImage, opt => opt.MapFrom(src => src.Company.User.Image))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.User.Name))
                .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline.HasValue ? src.Deadline.Value.ToString("dd-MM-yyyy") : null))
                .ForMember(dest => dest.Major, opt => opt.MapFrom(src => src.Major.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AddressedNavigation.Detail + ", " +
                (src.AddressedNavigation.Ward.Name) + ", " +
                (src.AddressedNavigation.District.Name) + ", " +
                (src.AddressedNavigation.Province.Name)))
                .ReverseMap();

            CreateMap<Job, JobListForStudentDTO>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.User.Name))
                .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline.HasValue ? src.Deadline.Value.ToString("dd-MM-yyyy") : null))
                .ForMember(dest => dest.Major, opt => opt.MapFrom(src => src.Major.Name))
                .ForMember(dest => dest.Address, opt => opt
                .MapFrom(src => src.AddressedNavigation.Detail + ", " +
                (src.AddressedNavigation.Ward.Name) + ", " +
                (src.AddressedNavigation.District.Name) + ", " +
                (src.AddressedNavigation.Province.Name)))
                .ReverseMap();

            CreateMap<Job, JobDetailForStudentDTO>()
                .ForMember(dest => dest.CompanyImage, opt => opt.MapFrom(src => src.Company.User.Image))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.User.Name))
                .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline.HasValue ? src.Deadline.Value.ToString("dd-MM-yyyy") : null))
                .ForMember(dest => dest.Major, opt => opt.MapFrom(src => src.Major.Name))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Company.CompanyId))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Company.Phone))
                .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Company.Website))
                .ForMember(dest => dest.Address, opt => opt
                .MapFrom(src => src.AddressedNavigation.Detail + ", " +
                (src.AddressedNavigation.Ward.Name) + ", " +
                (src.AddressedNavigation.District.Name) + ", " +
                (src.AddressedNavigation.Province.Name)))
                .ReverseMap();

            // Company
            CreateMap<Job, JobListForCompanyDTO>()
                    .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline.HasValue ? src.Deadline.Value.ToString("dd-MM-yyyy") : null))
                    .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.Name))
                    .ForMember(dest => dest.Address, opt => opt
                    .MapFrom(src => src.AddressedNavigation.Detail + ", " +
                            (src.AddressedNavigation.Ward.Name) + ", " +
                            (src.AddressedNavigation.District.Name) + ", " +
                            (src.AddressedNavigation.Province.Name)))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == "1" ? "Active" : src.Status == "0" ? "Deleted" : src.Status == "2" ? "Stored" : "Unknown"))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                    .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                    .ReverseMap();

            CreateMap<Job, JobDetailForCompanyDTO>()
                .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline.HasValue ? src.Deadline.Value.ToString("dd-MM-yyyy") : null))
                .ForMember(dest => dest.MajorName, opt => opt.MapFrom(src => src.Major.Name))
                .ForMember(dest => dest.Address, opt => opt
                .MapFrom(src => src.AddressedNavigation.Detail + ", " +
                        (src.AddressedNavigation.Ward.Name) + ", " +
                        (src.AddressedNavigation.District.Name) + ", " +
                        (src.AddressedNavigation.Province.Name)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == "1" ? "Active" : src.Status == "0" ? "Deleted" : src.Status == "2" ? "Stored" : "Unknown"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<Job, CreateJobForCompanyDTO>().ReverseMap();
            CreateMap<Job, UpdateJobForCompanyDTO>().ReverseMap();
        }
    }
}