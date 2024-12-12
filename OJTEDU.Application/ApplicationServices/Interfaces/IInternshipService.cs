using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.InternshipDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IInternshipService
    {
        // Mentor 
        Task<DataResponse<List<InternshipListForMentorDTO>>> GetAllInternshipsByUserIdAsync(int? userId); // fixing 
        Task<DataResponse<InternshipDetailForMentorDTO>> GetInternshipDetailAsync(int? internshipId);

        // Company 
        Task<DataResponse<List<InternshipListForCompanyDTO>>> GetAllInternshipsByUserIdForCompanyAsync(int? userId); // fixing 
        Task<DataResponse<bool>> AssignInternshipsForMentorAsync(int? userId, int? mentorId, int[]? internshipIds);
        Task<DataResponse<CreateInternshipForCompanyDTO>> CreateInternshipAsync(int? studentId);

        //Admin DOET Dean Lecturer
        Task<DataResponse<PagedResponse<List<InternshipDto>>>> GetAllInternshipsAsync(
        int userId,
        string role,
        string? searchTerm,
        DateTime? startDate,
        DateTime? endDate,
        string? statusFilter,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize);
        Task<DataResponse<InternshipDetailWithReportsDTO>> GetInternshipDetailsAsync(
        int internshipId,
        string? sortBy,
        bool? isDescending,
        string? week,
        int userId,
        string role,
        int? year = null);
        Task<DataResponse<string>> AssignLecturerForInternshipsAsync(string role, AssignLecturerForInternshipDto dto);
    }
}
