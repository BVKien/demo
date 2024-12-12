using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.NotificationDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface INotificationService
    {
        // Uni, Company, Student
        Task<DataResponse<List<NotificationForUniCompanyStudentDTO>>> GetAllNotificationsByUserIdAsync(int? userId);
    }
}
