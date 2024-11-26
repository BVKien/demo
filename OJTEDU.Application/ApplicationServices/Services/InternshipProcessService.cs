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
using static OJTEDU.Application.DTOs.InternshipProcessDTO;
using static OJTEDU.Application.DTOs.UserGuideDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class InternshipProcessService : IInternshipProcessService
    {
        private readonly IInternshipProcessRepository _internshipProcessRepository;
        private readonly IMapper _mapper;
        public InternshipProcessService(IInternshipProcessRepository internshipProcessRepository, IMapper mapper)
        {
            _internshipProcessRepository = internshipProcessRepository;
            _mapper = mapper;
        }


        public async Task<DataResponse<PagedResponse<List<InternshipProcessListForAdminDoetDTO>>>> GetAllInternshipProcessForAdminDoetAsync(string? title, bool? isVisible, int pageNumber, int pageSize)
        {
            try
            {
                var internProcess = await _internshipProcessRepository.GetAllInternshipProcessAsync(title, isVisible);

                var totalinternProcess = internProcess.Count();
                var totalPages = totalinternProcess == 0 ? 1 : (int)Math.Ceiling((double)totalinternProcess / pageSize);

                var internProcessDtos = totalinternProcess > 0 ? _mapper.Map<List<InternshipProcessListForAdminDoetDTO>>(internProcess)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<InternshipProcessListForAdminDoetDTO>();

                var pagedResponse = new PagedResponse<List<InternshipProcessListForAdminDoetDTO>>
                {
                    Items = internProcessDtos,
                    TotalCount = totalinternProcess,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<InternshipProcessListForAdminDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Internship Process list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<InternshipProcessListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<InternshipProcessListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving Internship Process list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<InternshipProcessDetailForAdminDoetDTO>> GetInternshipProcessDetailByIdForAdminDoetAsync(int InternshipProcessId)
        {
            try
            {
                var internProcess = await _internshipProcessRepository.GetInternshipProcessByIdAsync(InternshipProcessId);

                var internProcessDto = _mapper.Map<InternshipProcessDetailForAdminDoetDTO>(internProcess);

                return new DataResponse<InternshipProcessDetailForAdminDoetDTO>
                {
                    Data = internProcessDto,
                    Message = "Internship Process details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<InternshipProcessDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<InternshipProcessDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving Internship Process details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddInternshipProcessForAdminDoetDTO>> AddInternshipProcessForAdminDoetAsync(AddInternshipProcessForAdminDoetDTO addInternshipProcessForAdminDoetDTO)
        {
            try
            {
                var internProcess = new InternshipProcess
                {
                    Title = addInternshipProcessForAdminDoetDTO.Title,
                    FilePath = addInternshipProcessForAdminDoetDTO.FilePath,
                    CreatedBy = addInternshipProcessForAdminDoetDTO.CreatedBy
                };

                var addResult = await _internshipProcessRepository.AddInternshipProcessAsync(internProcess);

                var resultDto = _mapper.Map<AddInternshipProcessForAdminDoetDTO>(addResult);

                return new DataResponse<AddInternshipProcessForAdminDoetDTO>
                {
                    Data = resultDto,
                    Message = "Internship Process added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddInternshipProcessForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding Internship Process: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateInternshipProcessForAdminDoetDTO>> UpdateInternshipProcessForAdminDoetAsync(UpdateInternshipProcessForAdminDoetDTO updateInternshipProcessForAdminDoetDTO)
        {
            try
            {
                var existingInternProcess = await _internshipProcessRepository.GetInternshipProcessByIdAsync(updateInternshipProcessForAdminDoetDTO.IntershipProcessId);
                if (existingInternProcess == null)
                {
                    return new DataResponse<UpdateInternshipProcessForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Internship Process not found.",
                        StatusCode = 404 // Not Found
                    };
                }

                // Kiểm tra nếu file User Guide mới được tải lên để thay thế file cũ
                if (!string.IsNullOrWhiteSpace(updateInternshipProcessForAdminDoetDTO.FilePath))
                {
                    existingInternProcess.FilePath = updateInternshipProcessForAdminDoetDTO.FilePath;
                }

                existingInternProcess.Title = updateInternshipProcessForAdminDoetDTO.Title;


                // Thực hiện cập nhật trong cơ sở dữ liệu
                var updateResult = await _internshipProcessRepository.UpdateInternshipProcessAsync(existingInternProcess);

                // Mapping kết quả từ Entity sang DTO
                var resultDto = _mapper.Map<UpdateInternshipProcessForAdminDoetDTO>(updateResult);

                return new DataResponse<UpdateInternshipProcessForAdminDoetDTO>
                {
                    Data = resultDto,
                    Message = "Internship Process updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateInternshipProcessForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating Internship Process: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateInternshipProcessForAdminDoetDTO>> UpdateInternshipProcessVisibleForAdminDoetAsync(UpdateInternshipProcessForAdminDoetDTO updateInternshipProcessForAdminDoetDTO)
        {
            try
            {
                var existingInternProcess = await _internshipProcessRepository.GetInternshipProcessByIdAsync(updateInternshipProcessForAdminDoetDTO.IntershipProcessId);
                if (existingInternProcess == null)
                {
                    return new DataResponse<UpdateInternshipProcessForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "User Guide not found.",
                        StatusCode = 404 // Not Found
                    };
                }

                if (updateInternshipProcessForAdminDoetDTO.IsVisible == true)
                {
                    // Find and update other visible processes to false
                    var allProcesses = await _internshipProcessRepository.GetAllInternshipProcessAsync(null, true);
                    foreach (var process in allProcesses.Where(p => p.IntershipProcessId != existingInternProcess.IntershipProcessId))
                    {
                        process.IsVisible = false;
                        await _internshipProcessRepository.UpdateInternshipProcessAsync(process);
                    }
                }

                // Cập nhật trạng thái
                existingInternProcess.IsVisible = updateInternshipProcessForAdminDoetDTO.IsVisible;

                var updatedVisibleResult = await _internshipProcessRepository.UpdateInternshipProcessAsync(existingInternProcess);

                var internshipProcessDto = _mapper.Map<UpdateInternshipProcessForAdminDoetDTO>(updatedVisibleResult);

                return new DataResponse<UpdateInternshipProcessForAdminDoetDTO>
                {
                    Data = internshipProcessDto,
                    Message = "Internship Process updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateInternshipProcessForAdminDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateInternshipProcessForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating Internship Process: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteInternshipProcessForAdminDoetDTO>> DeleteInternshipProcessForAdminDoetAsync(DeleteInternshipProcessForAdminDoetDTO deleteInternshipProcessForAdminDoetDTO)
        {
            try
            {
                var deletedResult = await _internshipProcessRepository.DeleteInternshipProcessAsync(deleteInternshipProcessForAdminDoetDTO.IntershipProcessId);

                var InternshipProcessDto = _mapper.Map<DeleteInternshipProcessForAdminDoetDTO>(deletedResult);

                return new DataResponse<DeleteInternshipProcessForAdminDoetDTO>
                {
                    Data = InternshipProcessDto,
                    Message = "Internship Process has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteInternshipProcessForAdminDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteInternshipProcessForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting Internship Process: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<InternshipProcessDetailForAdminDoetDTO>> GetInternshipProcessByVisibleAsync()
        {
            try
            {
                var internProcess = await _internshipProcessRepository.GetInternshipProcessByVisibleAsync();

                var internProcessDto = _mapper.Map<InternshipProcessDetailForAdminDoetDTO>(internProcess);

                return new DataResponse<InternshipProcessDetailForAdminDoetDTO>
                {
                    Data = internProcessDto,
                    Message = "Internship Process details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<InternshipProcessDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<InternshipProcessDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving Internship Process details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
