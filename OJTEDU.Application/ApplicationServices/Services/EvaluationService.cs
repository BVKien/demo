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
using static OJTEDU.Application.DTOs.EvaluationDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly IMapper _mapper;

        public EvaluationService(IEvaluationRepository evaluationRepository, IMapper mapper)
        {
            _evaluationRepository = evaluationRepository;
            _mapper = mapper;
        }

        // Univeristy, Company, Student
        public async Task<DataResponse<CreateEvaluationForUniversityCompanyDTO>> CreateEvaluationAsync(int? internshipId,
            CreateEvaluationForUniversityCompanyDTO? info)
        {
            try
            {
                if (internshipId == null)
                {
                    return new DataResponse<CreateEvaluationForUniversityCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "Internship is required.",
                        Data = null
                    };
                }

                if (info?.CompanyComment == null)
                {
                    return new DataResponse<CreateEvaluationForUniversityCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "Final assessment is required.",
                        Data = null
                    };
                }

                if (info?.CompanyScore == null)
                {
                    return new DataResponse<CreateEvaluationForUniversityCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "Final score is required.",
                        Data = null
                    };
                }

                if (internshipId == null)
                {
                    return new DataResponse<CreateEvaluationForUniversityCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "Participation ccore is required.",
                        Data = null
                    };
                }

                if (info?.DeanComment == null)
                {
                    return new DataResponse<CreateEvaluationForUniversityCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "Final assessment is required.",
                        Data = null
                    };
                }

                if (info?.CompanyScore == null)
                {
                    return new DataResponse<CreateEvaluationForUniversityCompanyDTO>
                    {
                        StatusCode = 400,
                        Message = "Final score is required.",
                        Data = null
                    };
                }

                var evaluationInfo = new Evaluation
                {
                    CompanyComment = info?.CompanyComment,
                    DeanComment = info?.DeanComment,
                    CompanyScore = info?.CompanyScore,
                    DeanScore = info?.CompanyScore,
                };

                var evaluation = await _evaluationRepository.CreateEvaluationAsync(internshipId, evaluationInfo);
                var response = _mapper.Map<CreateEvaluationForUniversityCompanyDTO>(evaluation);

                return new DataResponse<CreateEvaluationForUniversityCompanyDTO>
                {
                    StatusCode = 200,
                    Message = "Evaluation created successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateEvaluationForUniversityCompanyDTO>
                {
                    StatusCode = 500,
                    Message = "Error creating evaluation.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>> GetEvaluationDetailByUserId(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                    {
                        StatusCode = 400,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var evaluation = await _evaluationRepository.GetEvaluationDetailByUserId(userId);
                var response = _mapper.Map<GetEvaluationDetailForUniversityCompanyStudentDTO>(evaluation);

                return new DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Evaluation for student retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                {
                    StatusCode = 500,
                    Message = "Error retrieving evaluation for student.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>> GetEvaluationDetailByInternshipId(int? internshipId)
        {
            try
            {
                if (internshipId == null)
                {
                    return new DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                    {
                        StatusCode = 400,
                        Message = "Not found internship.",
                        Data = null
                    };
                }

                var evaluation = await _evaluationRepository.GetEvaluationDetailByInternshipId(internshipId);
                var response = _mapper.Map<GetEvaluationDetailForUniversityCompanyStudentDTO>(evaluation);

                return new DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Evaluation for student retrieved successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<GetEvaluationDetailForUniversityCompanyStudentDTO>
                {
                    StatusCode = 500,
                    Message = "Error retrieving evaluation for student.",
                    Data = null
                };
            }
        }
    }
}