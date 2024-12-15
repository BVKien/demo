//using OJTEDU.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OJTEDU.Domain.Interfaces
//{
//    public interface IInternshipRepository
//    {
//        /*
//         + Internship status: 
//        0: Failed 
//        1: In progress
//        2: Passed
//         */

//        // Mentor 
//        // fix 
//        Task<IEnumerable<Internship>> GetAllInternshipsByUserIdAsync(int? userId); // filter, search, sort...
//        Task<Internship> GetInternshipDetailAsync(int? internshipId); // + Company

//        // Company 
//        // fix 
//        Task<IEnumerable<Internship>> GetAllInternshipsByUserIdForCompanyAsync(int? userId); // filter, search, sort...
//        Task<bool> AssignInternshipsForMentorAsync(int? userId, int? mentorId, int[]? internshipIds);
//        Task<Internship> CreateInternshipAsync(int? studentId);
//        Task<List<Internship>> GetAllInternshipsAsync(
//        int userId,
//        string role,
//        string? searchTerm,
//        DateTime? startDate,
//        DateTime? endDate,
//        string? statusFilter,
//        string? sortBy,
//        bool isDescending);
//        Task<(Internship, List<WorkingReport>)> GetInternshipDetailsWithWorkingReportsAsync(int internshipId, int userId, string role);
//        Task<List<Internship>> GetInternshipsByIdsAsync(List<int> internshipIds);
//        Task UpdateInternshipsAsync(List<Internship> internships);
//    }
//}


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
        Task<List<Internship>> GetAllInternshipsAsync(
        int userId,
        string role,
        string? searchTerm,
        DateTime? startDate,
        DateTime? endDate,
        string? statusFilter,
        string? sortBy,
        bool isDescending);
        Task<List<Internship>> GetAllInternshipsForDeanAsync(
       int userId,
string role,
string? searchTerm,
DateTime? startDate,
DateTime? endDate,
string? statusFilter,
string? sortBy,
bool isDescending);
        Task<(Internship, List<WorkingReport>)> GetInternshipDetailsWithWorkingReportsAsync(int internshipId, int userId, string role);
        Task<List<Internship>> GetInternshipsByIdsAsync(List<int> internshipIds);
        Task UpdateInternshipsAsync(List<Internship> internships);
    }
}
