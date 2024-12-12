using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class CvRepository : ICvRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _cvDirectory = "wwwroot/uploads/cvs/";

        public CvRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
            if (!Directory.Exists(_cvDirectory))
            {
                Directory.CreateDirectory(_cvDirectory);
            }
        }

        // Student 
        public async Task<string> UploadCvAsync(int? userId, string? fileName, byte[] fileData)
        {
            try
            {
                var student = _context.Students.Include(s => s.User).ThenInclude(s => s.Role).FirstOrDefault(s => s.UserId == userId);

                // Check if current amount of CVs is more than or equal to 3 and status is 0 or 1
                var cvCount = await _context.Cvs.CountAsync(c => c.StudentId == student.StudentId && (c.Status == "0" || c.Status == "1"));
                if (cvCount >= 3)
                {
                    throw new InvalidOperationException("Student already has 3 CV records, cannot add new.");
                }

                // Create file name format studentId_timestamp_filename
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var newFileName = $"{student.StudentId}_{timestamp}_{fileName}";
                var filePath = Path.Combine(_cvDirectory, newFileName);

                // Save the file to the directory
                await File.WriteAllBytesAsync(filePath, fileData);

                // Create a new CV record
                var newCv = new Cv
                {
                    StudentId = student.StudentId,
                    Name = fileName,
                    CvFile = filePath?.Replace("wwwroot", ""),
                    Status = "0",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Cvs.Add(newCv);
                await _context.SaveChangesAsync();

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> SetPrimaryCvAsync(int? userId, int? cvId)
        {
            try
            {
                var student = _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .FirstOrDefault(s => s.UserId == userId);
                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                // cv list 
                var Cvs = await _context.Cvs.Where(c => c.StudentId == student.StudentId).ToListAsync();
                // check cv exists
                var selectedCv = Cvs.FirstOrDefault(c => c.CvId == cvId);
                if (selectedCv == null)
                {
                    throw new InvalidOperationException("Not found CV for this student.");
                }

                // old primary cv -> none
                foreach (var cv in Cvs)
                {
                    if (cv.Status == "1")
                    {
                        cv.Status = "0";
                    }
                }

                // set primary cv
                selectedCv.Status = "1";
                selectedCv.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Cv>> GetAllCvByStudentIdAsync(int? userId)
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
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                var studentCvs = await _context.Cvs
                    .Where(c => c.StudentId == student.StudentId && c.Status != "2")
                    .ToListAsync();

                return studentCvs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteAndStoredCvAsync(int? cvId)
        {
            try
            {
                bool cvExists = await _context.Cvs
                    .AnyAsync(c => c.CvId == cvId);

                if (!cvExists)
                {
                    throw new KeyNotFoundException("Not found CV.");
                }

                var selectedCv = await _context.Cvs.FirstOrDefaultAsync(c => c.CvId == cvId);

                if (selectedCv.Status == "2")
                {
                    throw new InvalidOperationException("CV already deleted.");
                }

                selectedCv.Status = "2";
                selectedCv.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetPrimaryCvFilePathAsync(int? userId)
        {
            try
            {
                var studentExists = await _context.Students.Include(s => s.User).ThenInclude(s => s.Role)
                    .Where(s => s.User.UserId == userId).FirstOrDefaultAsync();

                if (studentExists == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                bool primaryCvExists = await _context.Cvs
                    .AnyAsync(c => c.StudentId == studentExists.StudentId && c.Status == "1");

                if (!primaryCvExists)
                {
                    throw new KeyNotFoundException("Not found primary CV of student.");
                }

                var studentCv = await _context.Cvs
                    .Where(c => c.StudentId == studentExists.StudentId && c.Status == "1")
                    .FirstOrDefaultAsync();

                var filePath = studentCv.CvFile;
                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
