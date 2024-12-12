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
using static OJTEDU.Application.DTOs.NotificationDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        // Uni, Company, Student
        public async Task<DataResponse<List<NotificationForUniCompanyStudentDTO>>> GetAllNotificationsByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<NotificationForUniCompanyStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found user.",
                        Data = null
                    };
                }

                var notis = await _notificationRepository.GetAllNotificationsByUserIdAsync(userId);
                var response = _mapper.Map<List<NotificationForUniCompanyStudentDTO>>(notis);

                return new DataResponse<List<NotificationForUniCompanyStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Notifications list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<NotificationForUniCompanyStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving notifications list: {ex.Message}.",
                    Data = null
                };
            }
        }
    }
}
