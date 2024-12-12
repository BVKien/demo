using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.NotificationDTO;

namespace OJTEDU.Application.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile() 
        {
            // Uni, Company, Student
            CreateMap<Notification, NotificationForUniCompanyStudentDTO>().ReverseMap();
        }
    }
}
