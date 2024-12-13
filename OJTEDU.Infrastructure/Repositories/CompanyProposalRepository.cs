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
    public class CompanyProposalRepository : ICompanyProposalRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _fileDirectory = "wwwroot/uploads/companyproposals/contracts/";
        private readonly INotificationRepository _notificationRepository;
        public CompanyProposalRepository(OJTEDU_DB_V1Context context, INotificationRepository notificationRepository)
        {
            _context = context;

            if (!Directory.Exists(_fileDirectory))
            {
                Directory.CreateDirectory(_fileDirectory);
            }
            _notificationRepository = notificationRepository;
        }

        // Student
        public async Task<IEnumerable<CompanyProposal>> GetAllCompanyProposalByStudentIdAsync(int? userId)
        {
            try
            {
                bool studentExists = await _context.Users
                    .AnyAsync(s => s.UserId == userId);

                if (!studentExists)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var student = await _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                var companyProposals = await _context.CompanyProposals
                    .Include(c => c.Student).ThenInclude(c => c.User)
                    .Include(c => c.University)
                    .Where(c => c.StudentId == student.StudentId)
                    .ToListAsync();

                return companyProposals;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CompanyProposal> GetCompanyProposalDetailByIdAsync(int? companyProposalId)
        {
            try
            {
                var companyProposal = await _context.CompanyProposals
                    .Include(c => c.Student).ThenInclude(c => c.User)
                    .Include(c => c.University)
                    .Where(c => c.CompanyProposalId == companyProposalId)
                    .FirstOrDefaultAsync();

                if (companyProposal == null)
                {
                    throw new KeyNotFoundException("Not found company proposal.");
                }

                return companyProposal;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CompanyProposal> CreateCompanyProposalAsync(int? userId, CompanyProposal? companyProposalInfo, string? fileName)
        {
            try
            {
                var studentExists = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
                if (studentExists == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var universityExists = await _context.Users.FirstOrDefaultAsync(u => u.Role.Name == "DOET");
                if (universityExists == null)
                {
                    throw new KeyNotFoundException("Not found Department Of Education and Training.");
                }

                var student = await _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                //// Create file name format studentId_timestamp_filename
                //var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                //var newFileName = fileName != null ? $"{studentExists.StudentId}_{timestamp}_{fileName}" : null;

                //var filePath = newFileName != null ? Path.Combine(_fileDirectory, newFileName) : null;

                //// Save files to folders
                //if (fileData != null && filePath != null)
                //{
                //    await File.WriteAllBytesAsync(filePath, fileData);
                //}

                //// If null 
                //if (fileName == null || fileData == null)
                //{
                //    filePath = null;
                //}

                var companyProposal = new CompanyProposal
                {
                    StudentId = studentExists?.StudentId,
                    UniversityId = universityExists?.UserId,
                    ProposalTitle = companyProposalInfo?.ProposalTitle,
                    ProposalContent = companyProposalInfo?.ProposalContent,
                    ProposalDate = DateTime.Now,
                    Contract = fileName,
                    Status = "1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.CompanyProposals.Add(companyProposal);
                await _context.SaveChangesAsync();

                // Notification 
                var notificationContent = $"{student?.User?.Name} has submitted a proposal to {universityExists?.Name} requesting approval for an internship at an external company.";
                var notiInfo = new Notification
                {
                    NotificationContent = notificationContent,
                    Image = student?.User?.Image,
                    StudentId = student?.StudentId,
                    UniversityId = universityExists?.UserId,
                    CompanyProposalId = companyProposal?.CompanyProposalId
                };

                await _notificationRepository.CreateNotificationAsync(notiInfo);

                return companyProposal;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
