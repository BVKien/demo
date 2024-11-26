using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IInternshipProcessRepository
    {
        // Admin
        Task<IEnumerable<InternshipProcess>> GetAllInternshipProcessAsync(string? title, bool? isVisible);
        Task<InternshipProcess> GetInternshipProcessByIdAsync(int internshipProcessId);
        Task<InternshipProcess> AddInternshipProcessAsync(InternshipProcess internshipProcess);
        Task<InternshipProcess> UpdateInternshipProcessAsync(InternshipProcess internshipProcess);
        Task<InternshipProcess> DeleteInternshipProcessAsync(int internshipProcessId);
        Task<InternshipProcess> GetInternshipProcessByVisibleAsync();
    }
}
