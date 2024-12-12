using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.AppllicationDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class AppllicationService : IAppllicationService
    {
        private readonly IAppllicationRepository _appllicationRepository;
        private readonly IMapper _mapper;
        public AppllicationService(IAppllicationRepository appllicationRepository, IMapper mapper)
        {
            _appllicationRepository = appllicationRepository;
            _mapper = mapper;
        }

        // Student
        public async Task<DataResponse<ApplyJobForStudentDTO>> ApplyJobAsync(int? userId, ApplyJobForStudentDTO? applyInfo, string? testFileName, byte[] testFileData, string? cvFileName, byte[] cvFileData)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<ApplyJobForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                if (applyInfo?.JobId == null)
                {
                    return new DataResponse<ApplyJobForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Job is required.",
                        Data = null
                    };
                }

                if (applyInfo?.CvId == null)
                {
                    return new DataResponse<ApplyJobForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "CV is required.",
                        Data = null
                    };
                }

                var apply = new Appllication
                {
                    JobId = applyInfo?.JobId,
                    CoverLetter = applyInfo?.CoverLetter,
                    CvId = applyInfo?.CvId,
                };

                var application = await _appllicationRepository.ApplyJobAsync(userId, apply, testFileName, testFileData, cvFileName, cvFileData);
                var response = _mapper.Map<ApplyJobForStudentDTO>(application);

                return new DataResponse<ApplyJobForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Apply job successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ApplyJobForStudentDTO>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<DataResponse<AppllicationDetailForStudentDTO>> GetApplicationDetailByIdAsync(int? applicationId)
        {
            try
            {
                if (applicationId == null)
                {
                    return new DataResponse<AppllicationDetailForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found application.",
                        Data = null
                    };
                }

                var application = await _appllicationRepository.GetApplicationDetailByIdAsync(applicationId);
                var response = _mapper.Map<AppllicationDetailForStudentDTO>(application);

                return new DataResponse<AppllicationDetailForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Application detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AppllicationDetailForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving application detail: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<List<AppllicationListForStudentDTO>>> GetAllApplicationsByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<AppllicationListForStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var applications = await _appllicationRepository.GetAllApplicationsByUserIdAsync(userId);
                var response = _mapper.Map<List<AppllicationListForStudentDTO>>(applications);

                return new DataResponse<List<AppllicationListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Applications list for student retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<AppllicationListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving applications list for student: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> CompanyOffersActionsAsync(int? userId, int? applicationId, string? studentRejectReason, string? status)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = false
                    };
                }

                if (applicationId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Application is required.",
                        Data = false
                    };
                }

                if (status == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Status is required.",
                        Data = false
                    };
                }

                if (status == "0")
                {
                    if (studentRejectReason == null)
                    {
                        return new DataResponse<bool>
                        {
                            StatusCode = 404,
                            Message = "Reject reason is required.",
                            Data = false
                        };
                    }
                }

                var applications = await _appllicationRepository.CompanyOffersActionsAsync(userId, applicationId, studentRejectReason, status);
                var response = _mapper.Map<bool>(applications);

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Actions to company offers successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"Error actions to company offers for student: {ex.Message}. ",
                    Data = false
                };
            }
        }

        // Company
        public async Task<DataResponse<List<AppllicationListForCompanyDTO>>> GetAllApplicationsByJobIdAsync(int? jobId)
        {
            try
            {
                if (jobId == null)
                {
                    return new DataResponse<List<AppllicationListForCompanyDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found job.",
                        Data = null
                    };
                }

                var applications = await _appllicationRepository.GetAllApplicationsByJobIdAsync(jobId);
                var response = _mapper.Map<List<AppllicationListForCompanyDTO>>(applications);

                return new DataResponse<List<AppllicationListForCompanyDTO>>
                {
                    StatusCode = 200,
                    Message = "Applications list for job retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<AppllicationListForCompanyDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving applications list for job: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<AppllicationDetailForCompanyDTO>> GetApplicationDetailForCompanyAsync(int? applicationId)
        {
            try
            {
                if (applicationId == null)
                {
                    return new DataResponse<AppllicationDetailForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found application.",
                        Data = null
                    };
                }

                var application = await _appllicationRepository.GetApplicationDetailByIdAsync(applicationId);
                var response = _mapper.Map<AppllicationDetailForCompanyDTO>(application);

                return new DataResponse<AppllicationDetailForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Application detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AppllicationDetailForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving application detail: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> StudentApplicationsActionsAsync(int? applicationId, string? feedback, DateTime? interviewDate, string? status)
        {
            try
            {
                if (applicationId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found application.",
                        Data = false
                    };
                }

                if (status == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Status is required.",
                        Data = false
                    };
                }

                if (feedback == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Feedback is required.",
                        Data = false
                    };
                }

                if (feedback == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Feedback is required.",
                        Data = false
                    };
                }

                if (status == "2")
                {
                    if (interviewDate == null)
                    {
                        return new DataResponse<bool>
                        {
                            StatusCode = 404,
                            Message = "Interview date is required.",
                            Data = false
                        };
                    }
                }

                var applications = await _appllicationRepository.StudentApplicationsActionsAsync(applicationId, feedback, interviewDate, status);
                var response = _mapper.Map<bool>(applications);

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Actions to student applications successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"Error actions to student applications for company: {ex.Message}. ",
                    Data = false
                };
            }
        }
    }
}
