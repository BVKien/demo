using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class InternshipProcessRepository : IInternshipProcessRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        public InternshipProcessRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InternshipProcess>> GetAllInternshipProcessAsync(string? title, bool? isVisible)
        {
            IQueryable<InternshipProcess> query = _context.InternshipProcesses.Include(u => u.CreatedByNavigation);

            // Apply search filters if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(title));
            }

            if (isVisible.HasValue)
            {
                query = query.Where(u => u.IsVisible == isVisible.Value);
            }

            // Fetch the filtered result from the database
            var internshipProcesses = await query.ToListAsync();

            // If no users match the search criteria, throw an exception
            if (internshipProcesses == null)
            {
                throw new KeyNotFoundException("Internship Processes not found.");
            }

            var sortedInternshipProcesses = internshipProcesses.OrderByDescending(u => u.IsVisible == true)
                                           .ThenByDescending(u => u.IsVisible == false)
                                           .ThenByDescending(u => u.IntershipProcessId)
                                           .ToList();

            return sortedInternshipProcesses;
        }

        public async Task<InternshipProcess> GetInternshipProcessByIdAsync(int internshipProcessId)
        {
            var internshipProcess = await _context.InternshipProcesses.Include(u => u.CreatedByNavigation)
                                                   .FirstOrDefaultAsync(u => u.IntershipProcessId == internshipProcessId);
            if (internshipProcess == null)
            {
                throw new KeyNotFoundException("Internship Process not found");
            }
            return internshipProcess;
        }

        public async Task<InternshipProcess> AddInternshipProcessAsync(InternshipProcess internshipProcess)
        {
            internshipProcess.CreatedAt = DateTime.Now;
            internshipProcess.UpdatedAt = DateTime.Now;
            internshipProcess.IsVisible = false; 
            await _context.InternshipProcesses.AddAsync(internshipProcess);
            await _context.SaveChangesAsync();

            return internshipProcess;
        }

        public async Task<InternshipProcess> UpdateInternshipProcessAsync(InternshipProcess internshipProcess)
        {
            var existingIntershipProcess = await GetInternshipProcessByIdAsync(internshipProcess.IntershipProcessId);
            if (existingIntershipProcess == null)
            {
                throw new KeyNotFoundException("Intership Process not found");
            }

            existingIntershipProcess.Title = internshipProcess.Title ?? existingIntershipProcess.Title;
            existingIntershipProcess.FilePath = internshipProcess.FilePath ?? existingIntershipProcess.FilePath;
            existingIntershipProcess.CreatedBy = internshipProcess.CreatedBy ?? existingIntershipProcess.CreatedBy;
            existingIntershipProcess.IsVisible = internshipProcess.IsVisible ?? existingIntershipProcess.IsVisible;
            existingIntershipProcess.UpdatedAt = DateTime.Now;
            _context.InternshipProcesses.Update(internshipProcess);
            await _context.SaveChangesAsync();

            return internshipProcess;
        }

        public async Task<InternshipProcess> DeleteInternshipProcessAsync(int internshipProcessId)
        {
            var intershipProcess = await GetInternshipProcessByIdAsync(internshipProcessId);
            if (intershipProcess == null)
            {
                throw new KeyNotFoundException("Intership Process not found in the list.");
            }

            _context.InternshipProcesses.Remove(intershipProcess);
            await _context.SaveChangesAsync();
            return intershipProcess;
        }

        public async Task<InternshipProcess> GetInternshipProcessByVisibleAsync()
        {
            var internshipProcess = await _context.InternshipProcesses.Include(u => u.CreatedByNavigation)
                                                   .FirstOrDefaultAsync(u => u.IsVisible == true);
            if (internshipProcess == null)
            {
                throw new KeyNotFoundException("No Internship Process is currently set to be visible.");
            }
            return internshipProcess;
        }
    }
}
