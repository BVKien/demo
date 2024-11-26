namespace OJTEDU.Api.Input.MentorControllers
{
    public class AttendanceReportController
    {

        public class SetCheckInCheckOutTimeInput
        {
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
        }

        public class CreateAttendanceReportInput
        {
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
            public string? Reason { get; set; }
            public string? Status { get; set; }
            public bool? EarlyLeave { get; set; }
            public bool? Late { get; set; }
        }

        public class UpdateAttendanceReportInput
        {
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
            public string? Reason { get; set; }
            public string? Status { get; set; }
            public bool? EarlyLeave { get; set; }
            public bool? Late { get; set; }
        }

        public class CreateAutoAttendanceReportInput
        {
            public TimeSpan? CheckInTime { get; set; }
            public TimeSpan? CheckOutTime { get; set; }
        }

        public class InsertAttendanceReportInput
        {
            public string? FileName { get; set; }
        }
    }
}