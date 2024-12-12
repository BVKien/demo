using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OJTEDU.Infrastructure.Repositories
{
    public class AppllicationRepository : IAppllicationRepository
    {
        private readonly OJTEDU_DB_V1Context _context;
        private readonly string _testFileDirectory = "wwwroot/uploads/applications/testFiles/";
        private readonly string _cvFileDirectory = "wwwroot/uploads/applications/cvFiles/";
        private readonly INotificationRepository _notificationRepository;

        public AppllicationRepository(OJTEDU_DB_V1Context context, INotificationRepository notificationRepository)
        {
            _context = context;

            if (!Directory.Exists(_testFileDirectory))
            {
                Directory.CreateDirectory(_testFileDirectory);
            }

            if (!Directory.Exists(_cvFileDirectory))
            {
                Directory.CreateDirectory(_cvFileDirectory);
            }
            _notificationRepository = notificationRepository;
        }

        // Student
        public async Task<Appllication> ApplyJobAsync(int? userId, Appllication? applyInfo, string? testFileName, byte[]? testFileData, string? cvFileName, byte[]? cvFileData)
        {
            try
            {
                var studentExists = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
                if (studentExists == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var jobExists = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == applyInfo.JobId);
                if (jobExists == null)
                {
                    throw new KeyNotFoundException("Not found job.");
                }

                bool cvExists = await _context.Cvs.AnyAsync(c => c.CvId == applyInfo.CvId && c.StudentId == studentExists.StudentId && (c.Status == "1" || c.Status == "0"));
                if (!cvExists)
                {
                    throw new KeyNotFoundException("Not found CV.");
                }

                bool applicationExists = await _context.Appllications
                    .AnyAsync(a => a.StudentId == studentExists.StudentId && a.JobId == applyInfo.JobId);
                if (applicationExists)
                {
                    throw new KeyNotFoundException("Application already exists for this job.");
                }

                // Create file name format studentId_timestamp_filename
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var newTestFileName = testFileName != null ? $"{studentExists.StudentId}_{timestamp}_{testFileName}" : null;
                var newCvFileName = cvFileName != null ? $"{studentExists.StudentId}_{timestamp}_{cvFileName}" : null;

                var testFilePath = newTestFileName != null ? Path.Combine(_testFileDirectory, newTestFileName) : null;
                var cvFilePath = newCvFileName != null ? Path.Combine(_cvFileDirectory, newCvFileName) : null;

                // Save files to folders
                if (testFileData != null && testFilePath != null)
                {
                    await File.WriteAllBytesAsync(testFilePath, testFileData);
                }

                if (cvFileData != null && cvFilePath != null)
                {
                    await File.WriteAllBytesAsync(cvFilePath, cvFileData);
                }

                // If null 
                if (testFileName == null || testFileData == null)
                {
                    testFilePath = null;
                }

                //if (cvFileName == null || cvFileData == null)
                //{
                //    cvFilePath = null;
                //}

                //var application = new Appllication
                //{
                //    StudentId = studentExists.StudentId,
                //    JobId = applyInfo.JobId,
                //    TestFile = testFilePath?.Replace("wwwroot", ""),
                //    CoverLetter = applyInfo.CoverLetter,
                //    CvId = applyInfo.CvId,
                //    CvFile = cvFilePath?.Replace("wwwroot", ""),
                //    Status = "1",
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now,
                //};

                var application = new Appllication
                {
                    StudentId = studentExists.StudentId,
                    JobId = applyInfo.JobId,
                    TestFile = testFilePath?.Replace("wwwroot", ""),
                    CoverLetter = applyInfo.CoverLetter,
                    CvId = applyInfo.CvId,
                    CvFile = cvFileName,
                    Status = "1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _context.Appllications.Add(application);
                await _context.SaveChangesAsync();

                // company
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == jobExists.CompanyId);

                if (company == null)
                {
                    throw new Exception("Not found company.");
                }

                // Notification
                var notificationContent = $"{studentExists?.User?.Name} has applied for the position {jobExists.Title} at {company?.User?.Name}.";
                var notiInfo = new Notification
                {
                    NotificationContent = notificationContent,
                    Image = studentExists?.User?.Image,
                    StudentId = studentExists?.StudentId,
                    CompanyId = company?.CompanyId,
                    ApplicationId = application.ApplicationId,
                };

                await _notificationRepository.CreateNotificationAsync(notiInfo);

                return application;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Appllication> GetApplicationDetailByIdAsync(int? applicationId)
        {
            try
            {
                var application = await _context.Appllications
                    .Include(a => a.Student)
                        .ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Job)
                    .Include(a => a.Cv)
                    .Where(a => a.ApplicationId == applicationId)
                    .FirstOrDefaultAsync();

                if (application == null)
                {
                    throw new KeyNotFoundException("Not found application.");
                }

                return application;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Appllication>> GetAllApplicationsByUserIdAsync(int? userId)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.User).ThenInclude(s => s.Role)
                    .Where(s => s.UserId == userId)
                    .FirstOrDefaultAsync();

                if (student == null)
                {
                    throw new KeyNotFoundException("Not found student.");
                }

                var applications = await _context.Appllications
                    .Include(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Job)
                    .Include(a => a.Cv)
                    .Where(a => a.StudentId == student.StudentId)
                    .ToListAsync();

                if (applications == null)
                {
                    throw new KeyNotFoundException("Not found applications list.");
                }

                return applications;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CompanyOffersActionsAsync(int? userId, int? applicationId, string? studentRejectReason, string? status)
        {
            try
            {
                if (status != "0" && status != "2" && status != "3" && status != "4" && status != "5")
                {
                    throw new KeyNotFoundException("Invalid status, must be 0, 2, 3, 4 or 5.");
                }

                var application = await _context.Appllications
                    .Include(a => a.Student)
                        .ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Job)
                    .Include(a => a.Cv)
                    .Where(a => a.ApplicationId == applicationId && (a.Status == "1" || a.Status == "2" || a.Status == "3" || a.Status == "4" || a.Status == "5"))
                    .FirstOrDefaultAsync();

                if (application == null)
                {
                    throw new KeyNotFoundException("Not found application.");
                }

                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == application.JobId);

                if (job == null)
                {
                    throw new Exception("Not found job.");
                }

                var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == job.CompanyId);

                if (company == null)
                {
                    throw new Exception("Not found company.");
                }

                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == application.StudentId);

                if (student == null)
                {
                    throw new Exception("Not found student.");
                }

                if (status == "3")
                {
                    // Accept offer
                    application.StudentRejectReason = null;
                    application.Status = status;
                    application.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification
                    var notificationContent = $"{student?.User?.Name} has accepted the interview invitation for the position {job?.Title} and will attend the interview at {company?.User?.Name}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = student?.User?.Image,
                        StudentId = student?.StudentId,
                        CompanyId = company?.CompanyId,
                        ApplicationId = application.ApplicationId,
                    };
                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                if (status == "0")
                {
                    // Reject an offer
                    application.StudentRejectReason = studentRejectReason;
                    application.Status = status;
                    application.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification
                    var notificationContent = $"{student?.User?.Name} has declined offer for positions at {company?.User?.Name}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = student?.User?.Image,
                        StudentId = student?.StudentId,
                        CompanyId = company?.CompanyId,
                        ApplicationId = application.ApplicationId,
                    };
                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                if (status == "5")
                {
                    var applicationAssign = await _context.Appllications
                        .Include(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                        .Include(a => a.Job)
                        .Include(a => a.Cv)
                        .Where(a => a.ApplicationId == applicationId && (a.Status == "4"))
                        .FirstOrDefaultAsync();

                    // Internship Comfirmed
                    applicationAssign.Status = status;
                    applicationAssign.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    var studentValid = await _context.Students
                        .Include(c => c.User).ThenInclude(c => c.Role)
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    if (studentValid == null)
                    {
                        throw new KeyNotFoundException("Not found student.");
                    }

                    var applicationValid = await _context.Appllications.FirstOrDefaultAsync(a => a.StudentId == studentValid.StudentId);

                    if (application == null)
                    {
                        throw new KeyNotFoundException("Not found application of this student.");
                    }

                    var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.SemesterId == studentValid.SemesterId);

                    if (semester == null)
                    {
                        throw new KeyNotFoundException("Not found semester.");
                    }

                    var internship = new Internship
                    {
                        StudentId = studentValid.StudentId,
                        JobId = application.JobId,
                        StartDate = DateTime.Now,
                        EndDate = semester.EndDate,
                        Status = "1",
                        SemesterId = studentValid.SemesterId,
                        MajorId = studentValid.MajorId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    await _context.Internships.AddAsync(internship);
                    await _context.SaveChangesAsync();

                    // Notification
                    var notificationContent = $"{student?.User?.Name} has accepted the internship offer for the position {job?.Title} and will begin the internship at {company?.User?.Name}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = student?.User?.Image,
                        StudentId = student?.StudentId,
                        CompanyId = company?.CompanyId,
                        ApplicationId = application.ApplicationId,
                    };
                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Company
        public async Task<IEnumerable<Appllication>> GetAllApplicationsByJobIdAsync(int? jobId)
        {
            try
            {
                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == jobId);

                if (job == null)
                {
                    throw new KeyNotFoundException("Not found job.");
                }

                var applications = await _context.Appllications
                    .Include(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Job)
                    .Include(a => a.Cv)
                    .Where(a => a.JobId == jobId)
                    .ToListAsync();

                return applications;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> StudentApplicationsActionsAsync(int? applicationId, string? feedback, DateTime? interviewDate, string? status)
        {
            try
            {
                if (status != "0" && status != "2" && status != "3" && status != "4" && status != "5")
                {
                    throw new KeyNotFoundException($"Invalid status, must be 0, 2, 3, 4 or 5.");
                }

                var application = await _context.Appllications
                    .Include(a => a.Student)
                        .ThenInclude(a => a.User).ThenInclude(a => a.Role)
                    .Include(a => a.Job)
                    .Include(a => a.Cv)
                    .Where(a => a.ApplicationId == applicationId && (a.Status == "1" || a.Status == "2" || a.Status == "3" || a.Status == "4" || a.Status == "5"))
                    .FirstOrDefaultAsync();

                if (application == null)
                {
                    throw new KeyNotFoundException("Not found application. ");
                }

                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == application.JobId);

                if (job == null)
                {
                    throw new Exception("Not found job.");
                }

                var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == job.CompanyId);

                if (company == null)
                {
                    throw new Exception("Not found company.");
                }

                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == application.StudentId);

                if (student == null)
                {
                    throw new Exception("Not found student.");
                }

                if (status == "2")
                {
                    // Accept application  
                    application.Feedback = feedback;
                    application.InterviewDate = interviewDate;
                    application.Status = status;
                    application.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification
                    var notificationContent = $"{company?.User?.Name} has accepted your application for the position {job?.Title} and is inviting you for an interview.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = company?.User?.Image,
                        StudentId = student?.StudentId,
                        CompanyId = company?.CompanyId,
                        ApplicationId = application.ApplicationId,
                    };
                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                if (status == "0")
                {
                    // Reject an application 
                    application.Feedback = feedback;
                    application.Status = status;
                    application.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification
                    var notificationContent = $"{company?.User?.Name} has rejected your application for the position {job?.Title}.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = company?.User?.Image,
                        StudentId = student?.StudentId,
                        CompanyId = company?.CompanyId,
                        ApplicationId = application.ApplicationId,
                    };
                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                if (status == "4")
                {
                    var applicationValid = await _context.Appllications
                        .Include(a => a.Student).ThenInclude(a => a.User).ThenInclude(a => a.Role)
                        .Include(a => a.Job)
                        .Include(a => a.Cv)
                        .Where(a => a.ApplicationId == applicationId && (a.Status == "3"))
                        .FirstOrDefaultAsync();

                    // Internship Accepted
                    applicationValid.Status = status;
                    applicationValid.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Notification
                    var notificationContent = $"{company?.User?.Name} has accepted your internship application for the position {job?.Title} after a successful interview.";
                    var notiInfo = new Notification
                    {
                        NotificationContent = notificationContent,
                        Image = company?.User?.Image,
                        StudentId = student?.StudentId,
                        CompanyId = company?.CompanyId,
                        ApplicationId = application.ApplicationId,
                    };
                    await _notificationRepository.CreateNotificationAsync(notiInfo);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}