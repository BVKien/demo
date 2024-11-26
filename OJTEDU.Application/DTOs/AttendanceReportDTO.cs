using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class AttendanceReportDTO
    {
        public class AttendanceReportDto
        {
            public string MentorName { get; set; }
            public int? TotalPresent { get; set; }
            public int? TotalAbsent { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        // Mentor 
        public class SetCheckInCheckOutTimeForMentorDTO
        {
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
        }

        public class CreateAttendanceReportForMentorDTO
        {
            public int AttendanceReportId { get; set; }
            public int? MentorId { get; set; }
            public int? InternshipId { get; set; }
            public DateTime? Date { get; set; }
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
            public string? Reason { get; set; }
            public string? Status { get; set; }
            public bool? EarlyLeave { get; set; }
            public bool? Late { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UpdateAttendanceReportForMentorDTO
        {
            public int AttendanceReportId { get; set; }
            public int? MentorId { get; set; }
            public int? InternshipId { get; set; }
            public DateTime? Date { get; set; }
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
            public string? Reason { get; set; }
            public string? Status { get; set; }
            public bool? EarlyLeave { get; set; }
            public bool? Late { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class AttendanceReportListFromCsvFileForMentorDTO
        {
            public string? Date { get; set; }
            public string? CheckInTime { get; set; }
            public string? CheckOutTime { get; set; }
            public string? Reason { get; set; }
            public string? Status { get; set; }
            public bool? Late { get; set; }
            public bool? EarlyLeave { get; set; }
        }

        // Mentor, Lecturer
        public class AttendanceReportsListForMentorLecturerDTO
        {
            public int AttendanceReportId { get; set; }
            public int? MentorId { get; set; }
            public int? InternshipId { get; set; }
            public DateTime? Date { get; set; }
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
            public string? Reason { get; set; }
            public string? Status { get; set; }
            public bool? Late { get; set; }
            public bool? EarlyLeave { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }

        // Student
        public class AttendanceReportsListForStudentDTO
        {
            public int AttendanceReportId { get; set; }
            public int? MentorId { get; set; }
            public int? InternshipId { get; set; }
            public DateTime? Date { get; set; }
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
            public string? Reason { get; set; }
            public string? Status { get; set; }
            public bool? Late { get; set; }
            public bool? EarlyLeave { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }
        }
    }
}
