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
    }
}
