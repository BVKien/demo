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
using static OJTEDU.Application.DTOs.CvDTO;
using System.Xml.Linq;
using AutoMapper;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class CvService : ICvService
    {
        private readonly ICvRepository _cvRepository;
        private readonly IMapper _mapper;

        public CvService(ICvRepository cvRepository, IMapper mapper)
        {
            _cvRepository = cvRepository;
            _mapper = mapper;
        }

        // Student 
        public async Task<DataResponse<string>> UploadCvAsync(int? userId, string? fileName, byte[] fileData)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<string>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var filePath = await _cvRepository.UploadCvAsync(userId, fileName, fileData);
                return new DataResponse<string>
                {
                    StatusCode = 200,
                    Message = "Cv file uploaded successfully!",
                    Data = filePath
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    StatusCode = 500,
                    Message = "An error occurred while uploading cv file.",
                    Data = ex.Message
                };
            }
        }

        public async Task<DataResponse<bool>> SetPrimaryCvAsync(int? userId, int? cvId)
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

                if (cvId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found CV.",
                        Data = false
                    };
                }

                var result = await _cvRepository.SetPrimaryCvAsync(userId, cvId);
                if (result)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 200,
                        Message = "Set primary CV successfully.",
                        Data = true
                    };
                }
                else
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 400,
                        Message = "Set primary CV failed.",
                        Data = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while set primary CV. {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<DataResponse<List<CvListForStudentDTO>>> GetAllCvByStudentIdAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<List<CvListForStudentDTO>>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var studentCvs = await _cvRepository.GetAllCvByStudentIdAsync(userId);
                var response = _mapper.Map<List<CvListForStudentDTO>>(studentCvs);

                return new DataResponse<List<CvListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "CV list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<CvListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving CV list: {ex.Message}. ",
                    Data = null
                };
            }
        }

        public async Task<DataResponse<bool>> DeleteAndStoredCvAsync(int? cvId)
        {
            try
            {
                if (cvId == null)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 404,
                        Message = "Not found CV.",
                        Data = false
                    };
                }

                var result = await _cvRepository.DeleteAndStoredCvAsync(cvId);
                if (result)
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 200,
                        Message = "Delete and stored CV successfully.",
                        Data = true
                    };
                }
                else
                {
                    return new DataResponse<bool>
                    {
                        StatusCode = 400,
                        Message = "Delete and stored CV failed.",
                        Data = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<bool>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while delete and stored CV. {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<DataResponse<string>> GetPrimaryCvFilePathAsync(int? userId)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<string>
                    {
                        StatusCode = 404,
                        Message = "Not found student.",
                        Data = null
                    };
                }

                var filePath = await _cvRepository.GetPrimaryCvFilePathAsync(userId);
                return new DataResponse<string>
                {
                    StatusCode = 200,
                    Message = "Cv file path retrieved successfully!",
                    Data = filePath
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<string>
                {
                    StatusCode = 500,
                    Message = $"An error occurred while retrieving cv file path. {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
