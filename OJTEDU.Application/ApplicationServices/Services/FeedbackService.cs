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
using static OJTEDU.Application.DTOs.FeedbackDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;
        public FeedbackService(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        // Student
        public async Task<DataResponse<List<FeedbackListForStudentDTO>>> GetAllFeedbacksByStudentIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<FeedbackListForStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var feedback = await _feedbackRepository.GetAllFeedbacksByStudentIdAsync(userId);
                var response = _mapper.Map<List<FeedbackListForStudentDTO>>(feedback);

                return new DataResponse<List<FeedbackListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Retrieved feedback list successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<FeedbackListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while retrieving feedback list {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<FeedbackDetailForStudentDTO>> GetFeedbackByFeedbackIdAsync(int? feedbackId)
        {
            try
            {
                if (feedbackId == null)
                {
                    return new DataResponse<FeedbackDetailForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found feedback.",
                        Data = null
                    };
                }

                var feedback = await _feedbackRepository.GetFeedbackByFeedbackIdAsync(feedbackId);
                var response = _mapper.Map<FeedbackDetailForStudentDTO>(feedback);

                return new DataResponse<FeedbackDetailForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Retrieved feedback detail successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<FeedbackDetailForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while retrieving feedback detail {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CreateFeedbackForStudentDTO>> CreateFeedbackAsync(int? userId, CreateFeedbackForStudentDTO? info)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateFeedbackForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var feedbackInfo = new Feedback
                {
                    FeedbackContent = info?.FeedbackContent,
                };

                var feedback = await _feedbackRepository.CreateFeedbackAsync(userId, feedbackInfo);
                var response = _mapper.Map<CreateFeedbackForStudentDTO>(feedback);

                return new DataResponse<CreateFeedbackForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Create feedback successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateFeedbackForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while create feedback {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> DeleteForStoredFeedbackAsync(int? feedbackId)
        {
            try
            {
                if (feedbackId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found feedback.",
                        Data = false
                    };
                }

                var feedback = await _feedbackRepository.DeleteForStoredFeedbackAsync(feedbackId);
                var response = _mapper.Map<bool>(feedback);

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Delete feedback successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while delete feedback {ex.Message}.",
                    Data = false
                };
            }
        }
    }
}
