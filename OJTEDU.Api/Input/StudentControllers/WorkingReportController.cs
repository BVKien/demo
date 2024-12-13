using OJTEDU.Application.DTOs;
using static OJTEDU.Application.DTOs.InternshipDTO;

namespace OJTEDU.Api.Input.StudentControllers
{
    public class WorkingReportController
    {
        public class CreateWorkingReportInput
        {
            public string? ReportTitle { get; set; }
            public string? ReportContent { get; set; }
            public string? FileAttachmentName { get; set; }
            public string? FileAttachmentPath{ get; set; }
        }

        public class UpdateWorkingReportInput
        {
            public string? ReportTitle { get; set; }
            public string? ReportContent { get; set; }
            public string? FileAttachment { get; set; }
        }
    }
}
