using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface ICvRepository
    {
        /*
        + Cv status 
        0: Normal
        1: Primary
        2: Deleted(stored in backend server)
         */

        // Student 
        //Task<string> UploadCvAsync(int? userId, string? fileName, byte[] fileData);
        Task<string> UploadCvAsync(int? userId, string? fileName, string? filePath);
        Task<bool> SetPrimaryCvAsync(int? userId, int? cvIdd);
        Task<IEnumerable<Cv>> GetAllCvByStudentIdAsync(int? userId);
        Task<bool> DeleteAndStoredCvAsync(int? cvId);
        Task<string> GetPrimaryCvFilePathAsync(int? userId);
    }
}
