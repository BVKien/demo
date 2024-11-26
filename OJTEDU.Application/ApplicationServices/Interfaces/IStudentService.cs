using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.StudentDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IStudentService
    {
        // Student 
        Task<DataResponse<StudentDetailForStudentDTO>> GetStudentDetailByUserIdAsync(int? userId);

        Task<DataResponse<UpdateStudentForStudentDTO>> UpdateStudentByUserIdAsync(int? userId, UpdateStudentForStudentDTO? updateInformation);
        //For Dean
        // 1. AssignLecturerForStudentsAsync
        Task<DataResponse<string>> AssignLecturerForStudentsAsync(AssignLecturerForStudentDto dto);

        // 2. GetStudentListAsync
        Task<DataResponse<PagedResponse<List<StudentListDto>>>> GetStudentListAsync(
            string? studentName,
            string? lecturerName,
            int pageNumber,
            int pageSize);

        // KienBV - fix
        Task<DataResponse<List<StudentListDto>>> GetOjtStudentListAsync();

        // 3. GetStudentDetailsAsync
        Task<DataResponse<StudentDetailsDto>> GetStudentDetailsAsync(int studentId);
    }
}
