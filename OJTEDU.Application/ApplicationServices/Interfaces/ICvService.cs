using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.CvDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface ICvService
    {
        // Student 
        //Task<DataResponse<string>> UploadCvAsync(int? userId, string? fileName, byte[] fileData);
        Task<DataResponse<string>> UploadCvAsync(int? userId, string? fileName, string? filePath);

        Task<DataResponse<bool>> SetPrimaryCvAsync(int? userId, int? cvId);

        Task<DataResponse<List<CvListForStudentDTO>>> GetAllCvByStudentIdAsync(int? userId);

        Task<DataResponse<bool>> DeleteAndStoredCvAsync(int? cvId);

        Task<DataResponse<string>> GetPrimaryCvFilePathAsync(int? userId);
    }
}
