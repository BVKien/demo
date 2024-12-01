using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.WorkingReportDTO;

namespace OJTEDU.Application.Profiles
{
    public class WorkingReportProfile : Profile
    {
        public WorkingReportProfile()
        {
            // Student 
            CreateMap<WorkingReport, WorkingReportListForStudentDTO>()
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.Name))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturer.Name))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<WorkingReport, WorkingReportDetailForStudentDTO>()
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.Name))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturer.Name))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<WorkingReport, CreateWorkingReportForStudentDTO>().ReverseMap();
            CreateMap<WorkingReport, UpdateWorkingReportForStudentDTO>().ReverseMap();
            // For Dean
            CreateMap<WorkingReport, WorkingReportDto>()
                .ForMember(dest => dest.FeedbackFromLecturer, opt => opt.MapFrom(src => src.FeedbackFromLecturer))
                .ForMember(dest => dest.LecturerScore, opt => opt.MapFrom(src => src.LecturerScore))
                .ForMember(dest => dest.MentorScore, opt => opt.MapFrom(src => src.MentorScore))
                .ForMember(dest => dest.FeedbackFromMentor, opt => opt.MapFrom(src => src.FeedbackFromMentor))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            CreateMap<WorkingReport, WorkingReportResponseDTO>()
                 .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                 .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturer.Name))
                 .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.Name));


            // Mentor 
            CreateMap<WorkingReport, WorkingReportListForMentorDTO>()
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.Name))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturer.Name))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<WorkingReport, WorkingReportDetailForMentorDTO>()
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.Name))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.Lecturer.Name))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.Name))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.ReportDate.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.Value.ToString("dd-MM-yyyy HH:mm:ss")))
                .ReverseMap();

            CreateMap<WorkingReport, CreateFeedbackWorkingReportForMentorDTO>().ReverseMap();
        }
    }
}
