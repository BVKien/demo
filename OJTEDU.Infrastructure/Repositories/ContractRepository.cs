using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _contractDirectory = "wwwroot/uploads/contracts/contractfiles/";

        public ContractRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;

            if (!Directory.Exists(_contractDirectory))
            {
                Directory.CreateDirectory(_contractDirectory);
            }
        }

        // Mentor 
        public async Task<Contract> AssignContractAsync(int? userId, int? internshipId, string? fileName, byte[] fileData, Contract? info)
        {
            try
            {
                var mentor = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.Role.Name == "Mentor");
                if (mentor == null)
                {
                    throw new KeyNotFoundException("Not found mentor.");
                }

                var internshipExists = await _context.Internships.FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                if (internshipExists == null)
                {
                    throw new KeyNotFoundException("Not found internship.");
                }

                var internshipContract = await _context.Internships.FirstOrDefaultAsync(i => i.IntershipId == internshipId && i.ContractId != null);

                if (internshipContract != null)
                {
                    throw new Exception("Cannot assign contract. Contract already assigned for internship.");
                }

                var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == mentor.ForCompany);

                // Create file name format internshipId_timestamp_filename
                var timestamp = GetVietnamTime().ToString("yyyyMMddHHmmssfff");
                var newFileName = fileName != null ? $"{internshipId}_{timestamp}_{fileName}" : null;

                var filePath = newFileName != null ? Path.Combine(_contractDirectory, newFileName) : null;

                // Save files to folders
                if (fileData != null && filePath != null)
                {
                    await File.WriteAllBytesAsync(filePath, fileData);
                }

                // If null 
                if (fileName == null || fileData == null)
                {
                    filePath = null;
                }

                var contract = new Contract
                {
                    ContractTypeId = 1, // contract type where company - uni collaboration 
                    CompanyId = company?.CompanyId,
                    Name = info?.Name,
                    ContractFile = filePath?.Replace("wwwroot", ""),
                    Status = "1",
                    CreatedAt = GetVietnamTime(),
                    UpdatedAt = GetVietnamTime()
                };

                await _context.Contracts.AddAsync(contract);
                await _context.SaveChangesAsync();

                // Assign for internship 
                internshipExists.ContractId = contract.ContractId;
                internshipExists.UpdatedAt = GetVietnamTime();

                await _context.SaveChangesAsync();

                return contract;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}
