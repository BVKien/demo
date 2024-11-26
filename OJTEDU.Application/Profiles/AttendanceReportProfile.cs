using AutoMapper;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AttendanceReportDTO;

namespace OJTEDU.Application.Profiles
{
    public class AttendanceReportProfile : Profile
    {
        public AttendanceReportProfile()
        {

            //For Dean
            CreateMap<AttendanceReport, AttendanceReportDto>()
                    .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.Name))
                    //.ForMember(dest => dest.TotalPresent, opt => opt.MapFrom(src => src.TotalPresent))
                    //.ForMember(dest => dest.TotalAbsent, opt => opt.MapFrom(src => src.TotalAbsent))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // Mentor 
            CreateMap<Company, SetCheckInCheckOutTimeForMentorDTO>().ReverseMap();
            CreateMap<AttendanceReport, CreateAttendanceReportForMentorDTO>().ReverseMap();
            CreateMap<AttendanceReport, UpdateAttendanceReportForMentorDTO>().ReverseMap();
            CreateMap<AttendanceReport, AttendanceReportListFromCsvFileForMentorDTO>().ReverseMap();

            // Mentor, Lecturer, Dean
            CreateMap<AttendanceReport, AttendanceReportsListForMentorLecturerDTO>().ReverseMap();

            // Student
            CreateMap<AttendanceReport, AttendanceReportsListForStudentDTO>().ReverseMap();
        }

    }
}
