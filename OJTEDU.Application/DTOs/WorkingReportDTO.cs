using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.Application.DTOs
{
    public class WorkingReportDTO
    {
        // Student 
        public class WorkingReportListForStudentDTO
        {
            public int WorkingReportId { get; set; }
            public string? MentorName { get; set; }
            public string? LecturerName { get; set; }
            public string? StudentName { get; set; }
            public string? ReportDate { get; set; }
            public double? LecturerScore { get; set; }
            public double? MentorScore { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
        }

        public class WorkingReportDetailForStudentDTO
        {
            public int WorkingReportId { get; set; }
            public string? MentorName { get; set; }
            public string? LecturerName { get; set; }
            public string? StudentName { get; set; }
            public string? ReportContent { get; set; }
            public string? ReportDate { get; set; }
            public string? FeedbackFromLecturer { get; set; }
            public string? FeedbackFromMentor { get; set; }
            public string? FileAttachment { get; set; }
            public double? LecturerScore { get; set; }
            public double? MentorScore { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
        }

        public class CreateWorkingReportForStudentDTO
        {
            public int? MentorId { get; set; }
            public int? LecturerId { get; set; }
            public int? StudentId { get; set; }
            public string? ReportTitle { get; set; }
            public string? ReportContent { get; set; }
            public string? FileAttachment { get; set; }
        }

        public class UpdateWorkingReportForStudentDTO
        {
            public string? ReportTitle { get; set; }
            public string? ReportContent { get; set; }
            public string? FileAttachment { get; set; }
        }
        //ForDean
        public class WorkingReportDto
        {
            public string WorkingReportId { get; set; }
            public string ReportTitle { get; set; }
            public string FeedbackFromLecturer { get; set; }
            public double? LecturerScore { get; set; }
            public double? MentorScore { get; set; }
            public string FeedbackFromMentor { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
        public class WorkingReportResponseDTO
        {
            public string MentorName { get; set; }
            public string LecturerName { get; set; }
            public string StudentName { get; set; }
            public string Week { get; set; }
            public List<WorkingReportDto> WorkingReports { get; set; }
        }
        public class GiveFeedbackOrScoreDto
        {
            public int WorkingReportId { get; set; }
            public double? Score { get; set; }
            public string? Feedback { get; set; }
        }

        // Mentor 
        public class WorkingReportListForMentorDTO
        {
            public int WorkingReportId { get; set; }
            public string? MentorName { get; set; }
            public string? LecturerName { get; set; }
            public string? StudentName { get; set; }
            public string? ReportDate { get; set; }
            public double? LecturerScore { get; set; }
            public double? MentorScore { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
        }

        public class WorkingReportDetailForMentorDTO
        {
            public int WorkingReportId { get; set; }
            public string? MentorName { get; set; }
            public string? LecturerName { get; set; }
            public string? StudentName { get; set; }
            public string? ReportContent { get; set; }
            public string? ReportDate { get; set; }
            public string? FeedbackFromLecturer { get; set; }
            public string? FeedbackFromMentor { get; set; }
            public string? FileAttachment { get; set; }
            public double? LecturerScore { get; set; }
            public double? MentorScore { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
        }

        public class CreateFeedbackWorkingReportForMentorDTO
        {
            public string? FeedbackFromMentor { get; set; }
            public double? MentorScore { get; set; }
        }
    }
}