using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AttendanceReportDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IAttendanceReportService
    {
        Task<DataResponse<PagedResponse<List<AttendanceReportDto>>>> GetAttendanceReportsByStudentIdAsync(
        int studentId, int pageNumber, int pageSize);

        // Mentor 
        Task<DataResponse<SetCheckInCheckOutTimeForMentorDTO>> SetCheckInCheckOutTimeAsync(int? userId, SetCheckInCheckOutTimeForMentorDTO? info);
        Task<DataResponse<bool>> CreateAutoAttendanceReportAsync(int? userId, TimeSpan? checkInTime, TimeSpan? checkOutTime);
        Task<DataResponse<UpdateAttendanceReportForMentorDTO>> UpdateAttendanceReportAsync(int? attendanceReportId, UpdateAttendanceReportForMentorDTO? info);
        Task<DataResponse<bool>> InsertAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData);
        Task<DataResponse<List<AttendanceReportListFromCsvFileForMentorDTO>>> ListAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData);
        Task<DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>> GetAllAttendanceReportsForMentorAsync(int? userId);

        // Mentor, Lecturer
        Task<DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>> GetAllAttendanceReportsByInternshipIdAsync(int? internshipId);

        // Lecturer
        Task<DataResponse<List<AttendanceReportsListForMentorLecturerDTO>>> GetAllAttendanceReportsForLecturerAsync(int? userId);

        // Student
        Task<DataResponse<List<AttendanceReportsListForStudentDTO>>> GetAllAttendanceReportsForStudentAsync(int? userId);
    }
}
