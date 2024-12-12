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
using static OJTEDU.Application.DTOs.SupportRequestDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class SupportRequestService : ISupportRequestService
    {
        private readonly ISupportRequestRepository _supportRequestRepository;
        private readonly IMapper _mapper;
        public SupportRequestService(ISupportRequestRepository supportRequestRepository, IMapper mapper)
        {
            _supportRequestRepository = supportRequestRepository;
            _mapper = mapper;
        }

        // Student
        public async Task<DataResponse<List<SupportRequestListForStudentDTO>>> GetAllSupportRequestByUserIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<SupportRequestListForStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var feedback = await _supportRequestRepository.GetAllSupportRequestByUserIdAsync(userId);
                var response = _mapper.Map<List<SupportRequestListForStudentDTO>>(feedback);

                return new DataResponse<List<SupportRequestListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Retrieved support request list successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<SupportRequestListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while retrieving support request list {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<SupportRequestDetailForStudentDTO>> GetSupportRequestDetailAsync(int? supportRequestId)
        {
            try
            {
                if (supportRequestId == null)
                {
                    return new DataResponse<SupportRequestDetailForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found support request.",
                        Data = null
                    };
                }

                var feedback = await _supportRequestRepository.GetSupportRequestDetailAsync(supportRequestId);
                var response = _mapper.Map<SupportRequestDetailForStudentDTO>(feedback);

                return new DataResponse<SupportRequestDetailForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Retrieved support request detail successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<SupportRequestDetailForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while retrieving support request detail {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<CreateSupportRequestForStudentDTO>> CreateSupportRequestAsync(int? userId, CreateSupportRequestForStudentDTO? info)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<CreateSupportRequestForStudentDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var supportRequestInfo = new SupportRequest
                {
                    RequestTitle = info?.RequestTitle,
                    RequestContent = info?.RequestContent,
                };

                var supportRequest = await _supportRequestRepository.CreateSupportRequestAsync(userId, supportRequestInfo);
                var response = _mapper.Map<CreateSupportRequestForStudentDTO>(supportRequest);

                return new DataResponse<CreateSupportRequestForStudentDTO>
                {
                    StatusCode = 200,
                    Message = "Create support request successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<CreateSupportRequestForStudentDTO>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while create support request {ex.Message}.",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> DeleteForStoredSupportRequestAsync(int? supportRequestId)
        {
            try
            {
                if (supportRequestId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found support request.",
                        Data = false
                    };
                }

                var supptrRequest = await _supportRequestRepository.DeleteForStoredSupportRequestAsync(supportRequestId);
                var response = _mapper.Map<bool>(supptrRequest);

                return new DataResponse<bool>
                {
                    StatusCode = 200,
                    Message = "Delete support request successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while delete support request {ex.Message}.",
                    Data = false
                };
            }
        }
        //DOET
        public async Task<DataResponse<PagedResponse<List<SupportRequestListForDOETDto>>>> GetAllSupportRequestsForDOETAsync(
         string? studentName,
         string? DOETName,
         string? status,
         int pageNumber,
         int pageSize,
         string? sortBy,
         bool? isDescending)
        {
            try
            {
                var supportRequests = await _supportRequestRepository.GetAllSupportRequestsForDOETAsync(studentName, DOETName, status, sortBy, isDescending);

                var totalRequests = supportRequests.Count;
                var totalPages = (int)Math.Ceiling((double)totalRequests / pageSize);

                var pagedRequests = supportRequests
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var requestDtos = _mapper.Map<List<SupportRequestListForDOETDto>>(pagedRequests);

                var pagedResponse = new PagedResponse<List<SupportRequestListForDOETDto>>
                {
                    Items = requestDtos,
                    TotalCount = totalRequests,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<SupportRequestListForDOETDto>>>
                {
                    Data = pagedResponse,
                    Message = "Support requests retrieved successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<SupportRequestListForDOETDto>>>
                {
                    Data = null,
                    Message = $"Error retrieving support requests: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
        public async Task<DataResponse<string>> UpdateSupportRequestForDOETAsync(int supportRequestId, UpdateSupportRequestForDOETDto dto, int universityUserId)
        {
            try
            {
                if (dto.Status != 1 && dto.Status != 2)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Invalid status. Allowed values are 1 (Approved) or 2 (Rejected).",
                        StatusCode = 400
                    };
                }

                var success = await _supportRequestRepository.UpdateSupportRequestForDOETAsync(supportRequestId, dto.FeedbackContent, dto.Status, universityUserId);

                if (!success)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Support request not found.",
                        StatusCode = 404
                    };
                }

                return new DataResponse<string>
                {
                    Data = "Support request updated successfully.",
                    Message = "Support request updated successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Error updating support request: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<string>> DeleteSupportRequestForDOETAsync(int supportRequestId)
        {
            try
            {
                var success = await _supportRequestRepository.DeleteSupportRequestForDOETAsync(supportRequestId);

                if (!success)
                {
                    return new DataResponse<string>
                    {
                        Data = null,
                        Message = "Support request not found or cannot delete requests with status = 0.",
                        StatusCode = 400
                    };
                }

                return new DataResponse<string>
                {
                    Data = "Support request deleted successfully.",
                    Message = "Support request deleted successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    Data = null,
                    Message = $"Error deleting support request: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


    }
}
