using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IAttendanceReportRepository
    {
        /*
         + Attendance Report status: 
        0: Absent 
        1: Present 
        3: Excused
        Late - bool
        Early Leave - bool
        Total time
         */

        Task<List<AttendanceReport>> GetAttendanceReportsByStudentIdAsync(int studentId); // cân nhắc bỏ 1

        // Mentor 
        Task<Company> SetCheckInCheckOutTimeAsync(int? userId, Company? info);
        Task<bool> CreateAutoAttendanceReportAsync(int? userId, TimeSpan? checkInTime, TimeSpan? checkOutTime);
        Task<AttendanceReport> UpdateAttendanceReportAsync(int? attendanceReportId, AttendanceReport? info);
        Task<bool> CreateAttendanceReportFileAsync(int? userId, int[]? internshipIds, string? fileName, byte[] fileData);
        Task<bool> InsertAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData);
        Task<IEnumerable<AttendanceReport>> ListAttendanceReportsFromExcelAsync(int? userId, string fileName, byte[] fileData);
        Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForMentorAsync(int? userId);

        // Mentor, Lecturer
        Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsByInternshipIdAsync(int? internshipId);

        // Lecturer
        Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForLecturerAsync(int? userId);

        // Student
        Task<IEnumerable<AttendanceReport>> GetAllAttendanceReportsForStudentAsync(int? userId);
    }
}
