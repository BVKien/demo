using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IJobRepository
    {
        /*
         + Job status: 
        0: Deleted
        1: Active 
        2: Stored 
         */

        // Student
        Task<Dictionary<int?, int>> GetJobCountsByCompanyIdsAsync(int[] companyIds);
        Task<IEnumerable<Job>> GetAllJobsByCompanyIdAsync(int? companyId);
        Task<(IEnumerable<Job>, int totalRecords)> SearchJobsAsync(int? userId, string? title, int? majorId,
            int? provinceId, int? districtId, int? wardId, int? pageNumber, int? pageSize);
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<Job> GetJobDetailAsync(int? jobId); // + Company 

        // Company 
        Task<IEnumerable<Job>> GetAllJobsByUserIdAsync(int? userId);
        Task<Job> CreateJobAsync(int? userId, string? fileName, byte[] fileData, Job? info, Address? addressInfo); // Address new or select company address - done 
        Task<Job> UpdateJobAsync(int? userId, int? jobId, string? fileName, byte[] fileData, Job? info, Address? addressInfo); // check if file path nnull -> note update file paths
        // Thiếu: API xóa mềm lưu trữ job đã xóa mềm
    }
}
