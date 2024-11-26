using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;
using static OJTEDU.Application.DTOs.GroupChatDTO;
using static OJTEDU.Application.DTOs.InternshipDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class InternshipService : IInternshipService
    {
        private readonly IInternshipRepository _internshipRepository;
        private readonly IMapper _mapper;
        public InternshipService(IInternshipRepository internshipRepository, IMapper mapper)
        {
            _internshipRepository = internshipRepository;
            _mapper = mapper;
        }

        // Mentor 
        public async Task<DataResponse<List<InternshipListForMentorDTO>>> GetAllInternshipsByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<InternshipListForMentorDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found mentor.",
                        Data = null
                    };
                }

                var internships = await _internshipRepository.GetAllInternshipsByUserIdAsync(userId);
                var response = _mapper.Map<List<InternshipListForMentorDTO>>(internships);

                return new DataResponse<List<InternshipListForMentorDTO>>
                {
                    StatusCode = 200,
                    Message = "Internships list for mentor retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<InternshipListForMentorDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving internship list for mentor: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<InternshipDetailForMentorDTO>> GetInternshipDetailAsync(int? internshipId)
        {
            try
            {
                if (internshipId == null)
                {
                    return new DataResponse<InternshipDetailForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found internship.",
                        Data = null
                    };
                }

                var internship = await _internshipRepository.GetInternshipDetailAsync(internshipId);
                var response = _mapper.Map<InternshipDetailForMentorDTO>(internship);

                return new DataResponse<InternshipDetailForMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Internship detail retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<InternshipDetailForMentorDTO>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving internship detail: {ex.Message}. ",
                    Data = null
                };
            }
        }

        // Company 
        public async Task<DataResponse<List<InternshipListForCompanyDTO>>> GetAllInternshipsByUserIdForCompanyAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<InternshipListForCompanyDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = null
                    };
                }

                var internships = await _internshipRepository.GetAllInternshipsByUserIdForCompanyAsync(userId);
                var response = _mapper.Map<List<InternshipListForCompanyDTO>>(internships);

                return new DataResponse<List<InternshipListForCompanyDTO>>
                {
                    StatusCode = 200,
                    Message = "Internships list for company retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<InternshipListForCompanyDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving internship list for company: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> AssignInternshipsForMentorAsync(int? userId, int? mentorId, int[]? internshipIds)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found company.",
                        Data = false
                    };
                }

                if (mentorId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Mentor is required.",
                        Data = false
                    };
                }

                if (internshipIds == null || !internshipIds.Any())
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Internships is required.",
                        Data = false
                    };
                }

                var response = await _internshipRepository.AssignInternshipsForMentorAsync(userId, mentorId, internshipIds);

                if (!response)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 400,
                        Message = "Failed to assign internships to the mentor.",
                        Data = false
                    };
                }

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Internships successfully assigned to the mentor.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"Error assigning internships to mentor: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<DataResponse<CreateInternshipForCompanyDTO>> CreateInternshipAsync(int? studentId)
        {
            try
            {
                if (studentId == null)
                {
                    return new DataResponse<CreateInternshipForCompanyDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var internship = await _internshipRepository.CreateInternshipAsync(studentId);
                var response = _mapper.Map<CreateInternshipForCompanyDTO>(internship);

                return new DataResponse<CreateInternshipForCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "An internship created successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateInternshipForCompanyDTO>
                {
                    StatusCode = 500,
                    Message = $"Error creating an internship.",
                    Data = null
                };
            }
        }
    }
}
