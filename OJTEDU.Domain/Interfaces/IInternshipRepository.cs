using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IInternshipRepository
    {
        /*
         + Internship status: 
        0: Failed 
        1: In progress
        2: Passed
         */

        // Mentor 
        // fix 
        Task<IEnumerable<Internship>> GetAllInternshipsByUserIdAsync(int? userId); // filter, search, sort...
        Task<Internship> GetInternshipDetailAsync(int? internshipId); // + Company

        // Company 
        // fix 
        Task<IEnumerable<Internship>> GetAllInternshipsByUserIdForCompanyAsync(int? userId); // filter, search, sort...
        Task<bool> AssignInternshipsForMentorAsync(int? userId, int? mentorId, int[]? internshipIds);
        Task<Internship> CreateInternshipAsync(int? studentId);
    }
}
