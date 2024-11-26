using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.SupportRequestDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface ISupportRequestService
    {
        // Student
        Task<DataResponse<List<SupportRequestListForStudentDTO>>> GetAllSupportRequestByUserIdAsync(int? userId);
        Task<DataResponse<SupportRequestDetailForStudentDTO>> GetSupportRequestDetailAsync(int? supportRequestId);
        Task<DataResponse<CreateSupportRequestForStudentDTO>> CreateSupportRequestAsync(int? userId, CreateSupportRequestForStudentDTO? info);
        Task<DataResponse<bool>> DeleteForStoredSupportRequestAsync(int? supportRequestId);
        Task<DataResponse<PagedResponse<List<SupportRequestListForDOETDto>>>> GetAllSupportRequestsForDOETAsync(
         string? studentName,
         string? DOETName,
         string? status,
         int pageNumber,
         int pageSize,
         string? sortBy,
         bool? isDescending);
        Task<DataResponse<string>> UpdateSupportRequestForDOETAsync(int supportRequestId, UpdateSupportRequestForDOETDto dto, int universityUserId);
        Task<DataResponse<string>> DeleteSupportRequestForDOETAsync(int supportRequestId);

    }
}
