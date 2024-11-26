using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.DocumentDTO;
using static OJTEDU.Application.DTOs.JobDTO;
using static OJTEDU.Application.DTOs.NewsFaqDTO;
using static OJTEDU.Application.DTOs.PolicyDTO;
using static OJTEDU.Application.DTOs.RoleDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class NewsFaqService : INewsFaqService
    {
        private readonly INewsFaqRepository _newsFaqRepository;
        private readonly IMapper _mapper;
        public NewsFaqService(INewsFaqRepository newsFaqRepository, IMapper mapper)
        {
            _newsFaqRepository = newsFaqRepository;
            _mapper = mapper;
        }

        // Admin - Parent News Management
        public async Task<DataResponse<PagedResponse<List<ParentNewsListForAdminDTO>>>> GetAllParentNewsForAdminAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var parentNews = await _newsFaqRepository.GetAllParentNewsForAdminAsync(title, roleId, status);

                var totalParentNews = parentNews.Count();
                var totalPages = totalParentNews == 0 ? 1 : (int)Math.Ceiling((double)totalParentNews / pageSize);

                var parentNewsDtos = parentNews
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ParentNewsListForAdminDTO
                    {
                        ParentNewsId = doc.NewsFaqid,
                        User = doc.User?.Name,
                        Title = doc.Title,
                        Image = doc.Image,
                        ParentNewscontent = doc.NewsFaqcontent,
                        Status = doc.Status,
                        ForRole = doc.NewsFaqroles != null && doc.NewsFaqroles.Any()
                            ? string.Join(", ", doc.NewsFaqroles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ParentNewsListForAdminDTO>>
                {
                    Items = parentNewsDtos,
                    TotalCount = totalParentNews,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ParentNewsListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Parent news list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ParentNewsListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ParentNewsListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving parent news list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ParentNewsDetailForAdminDTO>> GetParentNewsDetailByIdForAdminAsync(int newsId)
        {
            try
            {
                var parentNews = await _newsFaqRepository.GetParentNewsByIdForAdminAsync(newsId);

                var parentNewsDto = _mapper.Map<ParentNewsDetailForAdminDTO>(parentNews);

                return new DataResponse<ParentNewsDetailForAdminDTO>
                {
                    Data = parentNewsDto,
                    Message = "Parent news details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ParentNewsDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ParentNewsDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving Parent news details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddParentNewsForAdminDTO>> AddParentNewsForAdminAsync(AddParentNewsForAdminDTO addParentNewsForAdminDTO)
        {
            try
            {
                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (addParentNewsForAdminDTO.ForRoleIds.Contains(null) || addParentNewsForAdminDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    addParentNewsForAdminDTO.ForRoleIds = new List<int?> { null };
                }

                var newsFaq = new NewsFaq
                {
                    UserId = addParentNewsForAdminDTO.UserId,
                    Title = addParentNewsForAdminDTO.Title,
                    NewsFaqcontent = addParentNewsForAdminDTO.ParentNewscontent,
                    Image = addParentNewsForAdminDTO.Image
                };

                var addNewsFaqResult = await _newsFaqRepository.AddParentNewsForAdminAsync(newsFaq, addParentNewsForAdminDTO.ForRoleIds);

                var resultDto = _mapper.Map<AddParentNewsForAdminDTO>(addNewsFaqResult);

                return new DataResponse<AddParentNewsForAdminDTO>
                {
                    Data = resultDto,
                    Message = "Parent News added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddParentNewsForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding parent news: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentNewsForAdminDTO>> UpdateParentNewsForAdminAsync(UpdateParentNewsForAdminDTO updateParentNewsForAdminDTO)
        {
            try
            {
                var existingParentNews = await _newsFaqRepository.GetParentNewsByIdForAdminAsync(updateParentNewsForAdminDTO.ParentNewsId);
                if (existingParentNews == null)
                {
                    throw new KeyNotFoundException("Parent News not found");
                }

                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (updateParentNewsForAdminDTO.ForRoleIds.Contains(null) || updateParentNewsForAdminDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    updateParentNewsForAdminDTO.ForRoleIds = new List<int?> { null };
                }

                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateParentNewsForAdminDTO.ParentNewsId,
                    Title = updateParentNewsForAdminDTO.Title,
                    NewsFaqcontent = updateParentNewsForAdminDTO.ParentNewscontent,
                    Image = updateParentNewsForAdminDTO.Image
                };

                var updatedParentNewsResult = await _newsFaqRepository.UpdateParentNewsForAdminAsync(newsFaq, updateParentNewsForAdminDTO.ForRoleIds);

                var parentNewsDto = _mapper.Map<UpdateParentNewsForAdminDTO>(updatedParentNewsResult);

                return new DataResponse<UpdateParentNewsForAdminDTO>
                {
                    Data = parentNewsDto,
                    Message = "Parent News updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentNewsForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentNewsForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating parent news: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteParentNewsForAdminDTO>> DeleteParentNewsForAdminAsync(DeleteParentNewsForAdminDTO deleteParentNewsForAdminDTO)
        {
            try
            {
                // Lấy danh sách ChildNews
                var childNewsList = await _newsFaqRepository.GetAllChildNewsByParentIdForAdminAsync(deleteParentNewsForAdminDTO.ParentNewsId);

                var deletedParentNewsResult = await _newsFaqRepository.DeleteParentNewsForAdminAsync(deleteParentNewsForAdminDTO.ParentNewsId);

                var childNewsDtoList = _mapper.Map<List<DeleteChildNewsForAdminDTO>>(childNewsList);

                var parentNewsDto = _mapper.Map<DeleteParentNewsForAdminDTO>(deletedParentNewsResult);
                parentNewsDto.DeletedChildNews = childNewsDtoList;

                return new DataResponse<DeleteParentNewsForAdminDTO>
                {
                    Data = parentNewsDto,
                    Message = "Parent News has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteParentNewsForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteParentNewsForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting Parent News: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentNewsStatusForAdminDTO>> UpdateParentNewsStatusForAdminAsync(UpdateParentNewsStatusForAdminDTO updateParentNewsStatusForAdminDTO)
        {
            try
            {
                // Tạo đối tượng NewsFaq từ DTO
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateParentNewsStatusForAdminDTO.ParentNewsId,
                    Status = updateParentNewsStatusForAdminDTO.Status
                };

                // Cập nhật trạng thái cho bản tin cha và các bản tin con
                var updatedParentNewsStatusResult = await _newsFaqRepository.UpdateParentNewsStatusForAdminAsync(newsFaq);

                // Lấy lại danh sách ChildNews sau khi cập nhật
                var childNewsList = await _newsFaqRepository.GetAllChildNewsByParentIdForAdminAsync(updateParentNewsStatusForAdminDTO.ParentNewsId);

                // Ánh xạ danh sách ChildNews thành DTO
                var childNewsDtoList = _mapper.Map<List<UpdateChildNewsStatusForAdminDTO>>(childNewsList);

                // Ánh xạ bản tin cha thành DTO
                var parentNewsDto = _mapper.Map<UpdateParentNewsStatusForAdminDTO>(updatedParentNewsStatusResult);

                // Cập nhật danh sách ChildNews trong DTO của bản tin cha
                parentNewsDto.ChangedStatusChildNews = childNewsDtoList;

                return new DataResponse<UpdateParentNewsStatusForAdminDTO>
                {
                    Data = parentNewsDto,
                    Message = "Parent News updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentNewsStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentNewsStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating Parent News: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }


        public async Task<DataResponse<List<StatusNewsListForAdminDTO>>> GetAllStatusesNewsForAdminAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusNewsListForAdminDTO>
                {
                    new StatusNewsListForAdminDTO { Status = "Active" },
                    new StatusNewsListForAdminDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusNewsListForAdminDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusNewsListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<List<StatusNewsListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Access denied while get status list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusNewsListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Admin - Child News Management
        public async Task<DataResponse<PagedResponse<List<ChildNewsListForAdminDTO>>>> GetAllChildNewsForAdminAsync(int parentId, string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var childNews = await _newsFaqRepository.GetAllChildNewsForAdminAsync(parentId, title, roleId, status);

                var totalChildNews = childNews.Count();
                var totalPages = totalChildNews == 0 ? 1 : (int)Math.Ceiling((double)totalChildNews / pageSize);

                var childNewsDtos = childNews
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ChildNewsListForAdminDTO
                    {
                        ChildNewsId = doc.NewsFaqid,
                        User = doc.User?.Name,
                        Title = doc.Title,
                        Image = doc.Image,
                        ChildNewscontent = doc.NewsFaqcontent,
                        Status = doc.Status,
                        ForRole = doc.NewsFaqroles != null && doc.NewsFaqroles.Any()
                            ? string.Join(", ", doc.NewsFaqroles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ChildNewsListForAdminDTO>>
                {
                    Items = childNewsDtos,
                    TotalCount = totalChildNews,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ChildNewsListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Parent news list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ChildNewsListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ChildNewsListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving child news list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ChildNewsDetailForAdminDTO>> GetChildNewsDetailByIdForAdminAsync(int newsId)
        {
            try
            {
                var childNews = await _newsFaqRepository.GetChildNewsByIdForAdminAsync(newsId);

                var childNewsDto = _mapper.Map<ChildNewsDetailForAdminDTO>(childNews);

                return new DataResponse<ChildNewsDetailForAdminDTO>
                {
                    Data = childNewsDto,
                    Message = "Child news details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ChildNewsDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ChildNewsDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving child news details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddChildNewsForAdminDTO>> AddChildNewsForAdminAsync(AddChildNewsForAdminDTO addChildNewsForAdminDTO)
        {
            try
            {
                var newsFaq = new NewsFaq
                {
                    UserId = addChildNewsForAdminDTO.UserId,
                    ParentId = addChildNewsForAdminDTO.ParentId,
                    Title = addChildNewsForAdminDTO.Title,
                    NewsFaqcontent = addChildNewsForAdminDTO.ChildNewscontent,
                    Image = addChildNewsForAdminDTO.Image
                };

                var addNewsFaqResult = await _newsFaqRepository.AddChildNewsForAdminAsync(newsFaq);

                // Cập nhật thời gian tạo vào DTO trả về
                addChildNewsForAdminDTO.CreatedAt = addNewsFaqResult.CreatedAt;
                addChildNewsForAdminDTO.Status = addNewsFaqResult.Status;

                return new DataResponse<AddChildNewsForAdminDTO>
                {
                    Data = addChildNewsForAdminDTO,
                    Message = "Child News added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddChildNewsForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding child news: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildNewsForAdminDTO>> UpdateChildNewsForAdminAsync(UpdateChildNewsForAdminDTO updateChildNewsForAdminDTO)
        {
            try
            {
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateChildNewsForAdminDTO.ChildNewsId,
                    Title = updateChildNewsForAdminDTO.Title,
                    NewsFaqcontent = updateChildNewsForAdminDTO.ChildNewscontent,
                    Image = updateChildNewsForAdminDTO.Image
                };

                var updatedChildNewsResult = await _newsFaqRepository.UpdateChildNewsForAdminAsync(newsFaq);

                var childNewsDto = _mapper.Map<UpdateChildNewsForAdminDTO>(updatedChildNewsResult);

                return new DataResponse<UpdateChildNewsForAdminDTO>
                {
                    Data = childNewsDto,
                    Message = "Child News updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildNewsForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildNewsForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating child news: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteChildNewsForAdminDTO>> DeleteChildNewsForAdminAsync(DeleteChildNewsForAdminDTO deleteChildNewsForAdminDTO)
        {
            try
            {
                var deletedChildNewsResult = await _newsFaqRepository.DeleteChildNewsForAdminAsync(deleteChildNewsForAdminDTO.ChildNewsId);

                var childNewsDto = _mapper.Map<DeleteChildNewsForAdminDTO>(deletedChildNewsResult);

                return new DataResponse<DeleteChildNewsForAdminDTO>
                {
                    Data = childNewsDto,
                    Message = "Child News has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteChildNewsForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteChildNewsForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting child News: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildNewsStatusForAdminDTO>> UpdateChildNewsStatusForAdminAsync(UpdateChildNewsStatusForAdminDTO updateChildNewsStatusForAdminDTO)
        {
            try
            {
                var existingChildNews = await _newsFaqRepository.GetChildNewsByIdForAdminAsync(updateChildNewsStatusForAdminDTO.ChildNewsId);

                if (existingChildNews == null)
                {
                    throw new KeyNotFoundException("Child News not found");
                }

                var parentNews = await _newsFaqRepository.GetParentNewsByIdForAdminAsync(existingChildNews.ParentId.Value);

                if (parentNews == null)
                {
                    throw new KeyNotFoundException("Parent News not found");
                }

                if (parentNews.Status == "Unactive" && updateChildNewsStatusForAdminDTO.Status == "Active")
                {
                    throw new InvalidOperationException("Cannot update status child news to Active when parent news is Unactive");
                }

                // Tạo đối tượng NewsFaq từ DTO
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateChildNewsStatusForAdminDTO.ChildNewsId,
                    Status = updateChildNewsStatusForAdminDTO.Status
                };

                var updatedChildNewsStatusResult = await _newsFaqRepository.UpdateChildNewsForAdminAsync(newsFaq);

                var childNewsDto = _mapper.Map<UpdateChildNewsStatusForAdminDTO>(updatedChildNewsStatusResult);

                return new DataResponse<UpdateChildNewsStatusForAdminDTO>
                {
                    Data = childNewsDto,
                    Message = "Child News updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildNewsStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildNewsStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating child News: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        // Admin - Parent Faq Management
        public async Task<DataResponse<PagedResponse<List<ParentFaqListForAdminDTO>>>> GetAllParentFaqForAdminAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var parentFaq = await _newsFaqRepository.GetAllParentFaqForAdminAsync(title, roleId, status);

                var totalParentFaq = parentFaq.Count();
                var totalPages = totalParentFaq == 0 ? 1 : (int)Math.Ceiling((double)totalParentFaq / pageSize);

                var parentFaqDtos = parentFaq
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ParentFaqListForAdminDTO
                    {
                        ParentFaqId = doc.NewsFaqid,
                        User = doc.User?.Name,
                        Title = doc.Title,
                        Image = doc.Image,
                        ParentFaqcontent = doc.NewsFaqcontent,
                        Status = doc.Status,
                        ForRole = doc.NewsFaqroles != null && doc.NewsFaqroles.Any()
                            ? string.Join(", ", doc.NewsFaqroles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ParentFaqListForAdminDTO>>
                {
                    Items = parentFaqDtos,
                    TotalCount = totalParentFaq,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ParentFaqListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Parent faq list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ParentFaqListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ParentFaqListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving parent faq list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ParentFaqDetailForAdminDTO>> GetParentFaqDetailByIdForAdminAsync(int faqId)
        {
            try
            {
                var parentFaq = await _newsFaqRepository.GetParentFaqByIdForAdminAsync(faqId);

                var parentFaqDto = _mapper.Map<ParentFaqDetailForAdminDTO>(parentFaq);

                return new DataResponse<ParentFaqDetailForAdminDTO>
                {
                    Data = parentFaqDto,
                    Message = "Parent faq details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ParentFaqDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ParentFaqDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving Parent faq details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddParentFaqForAdminDTO>> AddParentFaqForAdminAsync(AddParentFaqForAdminDTO addParentFaqForAdminDTO)
        {
            try
            {
                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (addParentFaqForAdminDTO.ForRoleIds.Contains(null) || addParentFaqForAdminDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    addParentFaqForAdminDTO.ForRoleIds = new List<int?> { null };
                }

                var newsFaq = new NewsFaq
                {
                    UserId = addParentFaqForAdminDTO.UserId,
                    Title = addParentFaqForAdminDTO.Title,
                    NewsFaqcontent = addParentFaqForAdminDTO.ParentFaqcontent,
                    Image = addParentFaqForAdminDTO.Image
                };

                var addFaqResult = await _newsFaqRepository.AddParentFaqForAdminAsync(newsFaq, addParentFaqForAdminDTO.ForRoleIds);

                var resultDto = _mapper.Map<AddParentFaqForAdminDTO>(addFaqResult);

                return new DataResponse<AddParentFaqForAdminDTO>
                {
                    Data = resultDto,
                    Message = "Parent Faq added successfully!",
                    StatusCode = 201
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<AddParentFaqForAdminDTO>
                {
                    Data = null,
                    Message = $"Access denied while add parent faq: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddParentFaqForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding parent faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentFaqForAdminDTO>> UpdateParentFaqForAdminAsync(UpdateParentFaqForAdminDTO updateParentFaqForAdminDTO)
        {
            try
            {
                var existingParentFaq = await _newsFaqRepository.GetParentFaqByIdForAdminAsync(updateParentFaqForAdminDTO.ParentFaqId);
                if (existingParentFaq == null)
                {
                    throw new KeyNotFoundException("Parent Faq not found");
                }

                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (updateParentFaqForAdminDTO.ForRoleIds.Contains(null) || updateParentFaqForAdminDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    updateParentFaqForAdminDTO.ForRoleIds = new List<int?> { null };
                }

                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateParentFaqForAdminDTO.ParentFaqId,
                    Title = updateParentFaqForAdminDTO.Title,
                    NewsFaqcontent = updateParentFaqForAdminDTO.ParentFaqcontent,
                    Image = updateParentFaqForAdminDTO.Image
                };

                var updatedParentFaqResult = await _newsFaqRepository.UpdateParentFaqForAdminAsync(newsFaq, updateParentFaqForAdminDTO.ForRoleIds);

                var parentFaqDto = _mapper.Map<UpdateParentFaqForAdminDTO>(updatedParentFaqResult);

                return new DataResponse<UpdateParentFaqForAdminDTO>
                {
                    Data = parentFaqDto,
                    Message = "Parent Faq updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentFaqForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentFaqForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating parent faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteParentFaqForAdminDTO>> DeleteParentFaqForAdminAsync(DeleteParentFaqForAdminDTO deleteParentFaqForAdminDTO)
        {
            try
            {
                var childFaqList = await _newsFaqRepository.GetAllChildFaqByParentIdForAdminAsync(deleteParentFaqForAdminDTO.ParentFaqId);

                var deletedParentFaqResult = await _newsFaqRepository.DeleteParentFaqForAdminAsync(deleteParentFaqForAdminDTO.ParentFaqId);

                var childFaqDtoList = _mapper.Map<List<DeleteChildFaqForAdminDTO>>(childFaqList);

                var parentFaqDto = _mapper.Map<DeleteParentFaqForAdminDTO>(deletedParentFaqResult);
                parentFaqDto.DeletedChildFaq = childFaqDtoList;

                return new DataResponse<DeleteParentFaqForAdminDTO>
                {
                    Data = parentFaqDto,
                    Message = "Parent Faq has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteParentFaqForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteParentFaqForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting Parent Faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentFaqStatusForAdminDTO>> UpdateParentFaqStatusForAdminAsync(UpdateParentFaqStatusForAdminDTO updateParentFaqStatusForAdminDTO)
        {
            try
            {
                // Tạo đối tượng NewsFaq từ DTO
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateParentFaqStatusForAdminDTO.ParentFaqId,
                    Status = updateParentFaqStatusForAdminDTO.Status
                };

                var updatedParentFaqStatusResult = await _newsFaqRepository.UpdateParentFaqStatusForAdminAsync(newsFaq);

                var childFaqList = await _newsFaqRepository.GetAllChildFaqByParentIdForAdminAsync(updateParentFaqStatusForAdminDTO.ParentFaqId);

                var childFaqDtoList = _mapper.Map<List<UpdateChildFaqStatusForAdminDTO>>(childFaqList);

                var parentFaqDto = _mapper.Map<UpdateParentFaqStatusForAdminDTO>(updatedParentFaqStatusResult);

                parentFaqDto.ChangedStatusChildFaq = childFaqDtoList;

                return new DataResponse<UpdateParentFaqStatusForAdminDTO>
                {
                    Data = parentFaqDto,
                    Message = "Parent Faq updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentFaqStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentFaqStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating Parent Faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }


        public async Task<DataResponse<List<StatusFaqListForAdminDTO>>> GetAllStatusesFaqForAdminAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusFaqListForAdminDTO>
                {
                    new StatusFaqListForAdminDTO { Status = "Active" },
                    new StatusFaqListForAdminDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusFaqListForAdminDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusFaqListForAdminDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<List<StatusFaqListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Access denied while get status list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusFaqListForAdminDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Admin - Child Faq Management
        public async Task<DataResponse<PagedResponse<List<ChildFaqListForAdminDTO>>>> GetAllChildFaqForAdminAsync(int parentId, string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var childFaq = await _newsFaqRepository.GetAllChildFaqForAdminAsync(parentId, title, roleId, status);

                var totalChildFaq = childFaq.Count();
                var totalPages = totalChildFaq == 0 ? 1 : (int)Math.Ceiling((double)totalChildFaq / pageSize);

                var childFaqDtos = childFaq
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ChildFaqListForAdminDTO
                    {
                        ChildFaqId = doc.NewsFaqid,
                        User = doc.User?.Name,
                        Title = doc.Title,
                        Image = doc.Image,
                        ChildFaqcontent = doc.NewsFaqcontent,
                        Status = doc.Status,
                        ForRole = doc.NewsFaqroles != null && doc.NewsFaqroles.Any()
                            ? string.Join(", ", doc.NewsFaqroles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ChildFaqListForAdminDTO>>
                {
                    Items = childFaqDtos,
                    TotalCount = totalChildFaq,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ChildFaqListForAdminDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Child faq list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ChildFaqListForAdminDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ChildFaqListForAdminDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving child faq list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ChildFaqDetailForAdminDTO>> GetChildFaqDetailByIdForAdminAsync(int faqId)
        {
            try
            {
                var childFaq = await _newsFaqRepository.GetChildFaqByIdForAdminAsync(faqId);

                var childFaqDto = _mapper.Map<ChildFaqDetailForAdminDTO>(childFaq);

                return new DataResponse<ChildFaqDetailForAdminDTO>
                {
                    Data = childFaqDto,
                    Message = "Child faq details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ChildFaqDetailForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ChildFaqDetailForAdminDTO>
                {
                    Data = null,
                    Message = $"Error retrieving child faq details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddChildFaqForAdminDTO>> AddChildFaqForAdminAsync(AddChildFaqForAdminDTO addChildFaqForAdminDTO)
        {
            try
            {
                var newsFaq = new NewsFaq
                {
                    UserId = addChildFaqForAdminDTO.UserId,
                    ParentId = addChildFaqForAdminDTO.ParentId,
                    Title = addChildFaqForAdminDTO.Title,
                    NewsFaqcontent = addChildFaqForAdminDTO.ChildFaqcontent,
                    Image = addChildFaqForAdminDTO.Image
                };

                var addNewsFaqResult = await _newsFaqRepository.AddChildFaqForAdminAsync(newsFaq);

                // Cập nhật thời gian tạo vào DTO trả về
                addChildFaqForAdminDTO.CreatedAt = addNewsFaqResult.CreatedAt;
                addChildFaqForAdminDTO.Status = addNewsFaqResult.Status;

                return new DataResponse<AddChildFaqForAdminDTO>
                {
                    Data = addChildFaqForAdminDTO,
                    Message = "Child Faq added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddChildFaqForAdminDTO>
                {
                    Data = null,
                    Message = $"Error adding child faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildFaqForAdminDTO>> UpdateChildFaqForAdminAsync(UpdateChildFaqForAdminDTO updateChildFaqForAdminDTO)
        {
            try
            {
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateChildFaqForAdminDTO.ChildFaqId,
                    Title = updateChildFaqForAdminDTO.Title,
                    NewsFaqcontent = updateChildFaqForAdminDTO.ChildFaqcontent,
                    Image = updateChildFaqForAdminDTO.Image
                };

                var updatedChildFaqResult = await _newsFaqRepository.UpdateChildFaqForAdminAsync(newsFaq);

                var childFaqDto = _mapper.Map<UpdateChildFaqForAdminDTO>(updatedChildFaqResult);

                return new DataResponse<UpdateChildFaqForAdminDTO>
                {
                    Data = childFaqDto,
                    Message = "Child Faq updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildFaqForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildFaqForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating child faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteChildFaqForAdminDTO>> DeleteChildFaqForAdminAsync(DeleteChildFaqForAdminDTO deleteChildFaqForAdminDTO)
        {
            try
            {
                var deletedChildFaqResult = await _newsFaqRepository.DeleteChildFaqForAdminAsync(deleteChildFaqForAdminDTO.ChildFaqId);

                var childFaqDto = _mapper.Map<DeleteChildFaqForAdminDTO>(deletedChildFaqResult);

                return new DataResponse<DeleteChildFaqForAdminDTO>
                {
                    Data = childFaqDto,
                    Message = "Child Faq has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteChildFaqForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteChildFaqForAdminDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting child Faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildFaqStatusForAdminDTO>> UpdateChildFaqStatusForAdminAsync(UpdateChildFaqStatusForAdminDTO updateChildFaqStatusForAdminDTO)
        {
            try
            {
                var existingChildFaq = await _newsFaqRepository.GetChildFaqByIdForAdminAsync(updateChildFaqStatusForAdminDTO.ChildFaqId);

                if (existingChildFaq == null)
                {
                    throw new KeyNotFoundException("Child Faq not found");
                }

                var parentFaq = await _newsFaqRepository.GetParentFaqByIdForAdminAsync(existingChildFaq.ParentId.Value);

                if (parentFaq == null)
                {
                    throw new KeyNotFoundException("Parent Faq not found");
                }

                if (parentFaq.Status == "Unactive" && updateChildFaqStatusForAdminDTO.Status == "Active")
                {
                    throw new InvalidOperationException("Cannot update status child faq to Active when parent faq is Unactive");
                }

                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateChildFaqStatusForAdminDTO.ChildFaqId,
                    Status = updateChildFaqStatusForAdminDTO.Status
                };

                var updatedChildFaqStatusResult = await _newsFaqRepository.UpdateChildFaqForAdminAsync(newsFaq);

                var childFaqDto = _mapper.Map<UpdateChildFaqStatusForAdminDTO>(updatedChildFaqStatusResult);

                return new DataResponse<UpdateChildFaqStatusForAdminDTO>
                {
                    Data = childFaqDto,
                    Message = "Child Faq updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildFaqStatusForAdminDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildFaqStatusForAdminDTO>
                {
                    Data = null,
                    Message = $"Error updating child faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        // Doet - Parent News Management
        public async Task<DataResponse<PagedResponse<List<ParentNewsListForDoetDTO>>>> GetAllParentNewsForDoetAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var parentNews = await _newsFaqRepository.GetAllParentNewsForDoetAsync(title, roleId, status);

                var totalParentNews = parentNews.Count();
                var totalPages = totalParentNews == 0 ? 1 : (int)Math.Ceiling((double)totalParentNews / pageSize);

                var parentNewsDtos = parentNews
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ParentNewsListForDoetDTO
                    {
                        ParentNewsId = doc.NewsFaqid,
                        User = doc.User?.Name,
                        Title = doc.Title,
                        Image = doc.Image,
                        ParentNewscontent = doc.NewsFaqcontent,
                        Status = doc.Status,
                        ForRole = doc.NewsFaqroles != null && doc.NewsFaqroles.Any()
                            ? string.Join(", ", doc.NewsFaqroles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ParentNewsListForDoetDTO>>
                {
                    Items = parentNewsDtos,
                    TotalCount = totalParentNews,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ParentNewsListForDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Parent news list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ParentNewsListForDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ParentNewsListForDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving parent news list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ParentNewsDetailForDoetDTO>> GetParentNewsDetailByIdForDoetAsync(int newsId)
        {
            try
            {
                var parentNews = await _newsFaqRepository.GetParentNewsByIdForDoetAsync(newsId);

                var parentNewsDto = _mapper.Map<ParentNewsDetailForDoetDTO>(parentNews);

                return new DataResponse<ParentNewsDetailForDoetDTO>
                {
                    Data = parentNewsDto,
                    Message = "Parent news details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ParentNewsDetailForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ParentNewsDetailForDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving Parent news details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddParentNewsForDoetDTO>> AddParentNewsForDoetAsync(AddParentNewsForDoetDTO addParentNewsForDoetDTO)
        {
            try
            {
                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (addParentNewsForDoetDTO.ForRoleIds.Contains(null) || addParentNewsForDoetDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    addParentNewsForDoetDTO.ForRoleIds = new List<int?> { null };
                }

                var newsFaq = new NewsFaq
                {
                    UserId = addParentNewsForDoetDTO.UserId,
                    Title = addParentNewsForDoetDTO.Title,
                    NewsFaqcontent = addParentNewsForDoetDTO.ParentNewscontent,
                    Image = addParentNewsForDoetDTO.Image
                };

                var addNewsFaqResult = await _newsFaqRepository.AddParentNewsForDoetAsync(newsFaq, addParentNewsForDoetDTO.ForRoleIds);

                var resultDto = _mapper.Map<AddParentNewsForDoetDTO>(addNewsFaqResult);

                return new DataResponse<AddParentNewsForDoetDTO>
                {
                    Data = resultDto,
                    Message = "Parent News added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddParentNewsForDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding parent news: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentNewsForDoetDTO>> UpdateParentNewsForDoetAsync(UpdateParentNewsForDoetDTO updateParentNewsForDoetDTO)
        {
            try
            {
                var existingParentNews = await _newsFaqRepository.GetParentNewsByIdForDoetAsync(updateParentNewsForDoetDTO.ParentNewsId);
                if (existingParentNews == null)
                {
                    throw new KeyNotFoundException("Policy not found");
                }

                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (updateParentNewsForDoetDTO.ForRoleIds.Contains(null) || updateParentNewsForDoetDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    updateParentNewsForDoetDTO.ForRoleIds = new List<int?> { null };
                }

                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateParentNewsForDoetDTO.ParentNewsId,
                    Title = updateParentNewsForDoetDTO.Title,
                    NewsFaqcontent = updateParentNewsForDoetDTO.ParentNewscontent,
                    Image = updateParentNewsForDoetDTO.Image
                };

                var updatedParentNewsResult = await _newsFaqRepository.UpdateParentNewsForDoetAsync(newsFaq, updateParentNewsForDoetDTO.ForRoleIds);

                var parentNewsDto = _mapper.Map<UpdateParentNewsForDoetDTO>(updatedParentNewsResult);

                return new DataResponse<UpdateParentNewsForDoetDTO>
                {
                    Data = parentNewsDto,
                    Message = "Parent News updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentNewsForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentNewsForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating parent news: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteParentNewsForDoetDTO>> DeleteParentNewsForDoetAsync(DeleteParentNewsForDoetDTO deleteParentNewsForDoetDTO)
        {
            try
            {
                // Lấy danh sách ChildNews
                var childNewsList = await _newsFaqRepository.GetAllChildNewsByParentIdForDoetAsync(deleteParentNewsForDoetDTO.ParentNewsId);

                var deletedParentNewsResult = await _newsFaqRepository.DeleteParentNewsForDoetAsync(deleteParentNewsForDoetDTO.ParentNewsId);

                var childNewsDtoList = _mapper.Map<List<DeleteChildNewsForDoetDTO>>(childNewsList);

                var parentNewsDto = _mapper.Map<DeleteParentNewsForDoetDTO>(deletedParentNewsResult);
                parentNewsDto.DeletedChildNews = childNewsDtoList;

                return new DataResponse<DeleteParentNewsForDoetDTO>
                {
                    Data = parentNewsDto,
                    Message = "Parent News has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteParentNewsForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteParentNewsForDoetDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting Parent News: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentNewsStatusForDoetDTO>> UpdateParentNewsStatusForDoetAsync(UpdateParentNewsStatusForDoetDTO updateParentNewsStatusForDoetDTO)
        {
            try
            {
                // Tạo đối tượng NewsFaq từ DTO
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateParentNewsStatusForDoetDTO.ParentNewsId,
                    Status = updateParentNewsStatusForDoetDTO.Status
                };

                // Cập nhật trạng thái cho bản tin cha và các bản tin con
                var updatedParentNewsStatusResult = await _newsFaqRepository.UpdateParentNewsStatusForDoetAsync(newsFaq);

                // Lấy lại danh sách ChildNews sau khi cập nhật
                var childNewsList = await _newsFaqRepository.GetAllChildNewsByParentIdForDoetAsync(updateParentNewsStatusForDoetDTO.ParentNewsId);

                // Ánh xạ danh sách ChildNews thành DTO
                var childNewsDtoList = _mapper.Map<List<UpdateChildNewsStatusForDoetDTO>>(childNewsList);

                // Ánh xạ bản tin cha thành DTO
                var parentNewsDto = _mapper.Map<UpdateParentNewsStatusForDoetDTO>(updatedParentNewsStatusResult);

                // Cập nhật danh sách ChildNews trong DTO của bản tin cha
                parentNewsDto.ChangedStatusChildNews = childNewsDtoList;

                return new DataResponse<UpdateParentNewsStatusForDoetDTO>
                {
                    Data = parentNewsDto,
                    Message = "Parent News updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentNewsStatusForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentNewsStatusForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating Parent News: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }


        public async Task<DataResponse<List<StatusNewsListForDoetDTO>>> GetAllStatusesNewsForDoetAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusNewsListForDoetDTO>
                {
                    new StatusNewsListForDoetDTO { Status = "Active" },
                    new StatusNewsListForDoetDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusNewsListForDoetDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusNewsListForDoetDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<List<StatusNewsListForDoetDTO>>
                {
                    Data = null,
                    Message = $"Access denied while get status list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusNewsListForDoetDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Doet - Child News Management
        public async Task<DataResponse<PagedResponse<List<ChildNewsListForDoetDTO>>>> GetAllChildNewsForDoetAsync(int parentId, string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var childNews = await _newsFaqRepository.GetAllChildNewsForDoetAsync(parentId, title, roleId, status);

                var totalChildNews = childNews.Count();
                var totalPages = totalChildNews == 0 ? 1 : (int)Math.Ceiling((double)totalChildNews / pageSize);

                var childNewsDtos = childNews
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ChildNewsListForDoetDTO
                    {
                        ChildNewsId = doc.NewsFaqid,
                        User = doc.User?.Name,
                        Title = doc.Title,
                        Image = doc.Image,
                        ChildNewscontent = doc.NewsFaqcontent,
                        Status = doc.Status,
                        ForRole = doc.NewsFaqroles != null && doc.NewsFaqroles.Any()
                            ? string.Join(", ", doc.NewsFaqroles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ChildNewsListForDoetDTO>>
                {
                    Items = childNewsDtos,
                    TotalCount = totalChildNews,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ChildNewsListForDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Parent news list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ChildNewsListForDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ChildNewsListForDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving child news list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ChildNewsDetailForDoetDTO>> GetChildNewsDetailByIdForDoetAsync(int newsId)
        {
            try
            {
                var childNews = await _newsFaqRepository.GetChildNewsByIdForDoetAsync(newsId);

                var childNewsDto = _mapper.Map<ChildNewsDetailForDoetDTO>(childNews);

                return new DataResponse<ChildNewsDetailForDoetDTO>
                {
                    Data = childNewsDto,
                    Message = "Child news details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ChildNewsDetailForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ChildNewsDetailForDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving child news details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddChildNewsForDoetDTO>> AddChildNewsForDoetAsync(AddChildNewsForDoetDTO addChildNewsForDoetDTO)
        {
            try
            {
                var newsFaq = new NewsFaq
                {
                    UserId = addChildNewsForDoetDTO.UserId,
                    ParentId = addChildNewsForDoetDTO.ParentId,
                    Title = addChildNewsForDoetDTO.Title,
                    NewsFaqcontent = addChildNewsForDoetDTO.ChildNewscontent,
                    Image = addChildNewsForDoetDTO.Image
                };

                var addNewsFaqResult = await _newsFaqRepository.AddChildNewsForDoetAsync(newsFaq);

                // Cập nhật thời gian tạo vào DTO trả về
                addChildNewsForDoetDTO.CreatedAt = addNewsFaqResult.CreatedAt;
                addChildNewsForDoetDTO.Status = addNewsFaqResult.Status;

                return new DataResponse<AddChildNewsForDoetDTO>
                {
                    Data = addChildNewsForDoetDTO,
                    Message = "Child News added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddChildNewsForDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding child news: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildNewsForDoetDTO>> UpdateChildNewsForDoetAsync(UpdateChildNewsForDoetDTO updateChildNewsForDoetDTO)
        {
            try
            {
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateChildNewsForDoetDTO.ChildNewsId,
                    Title = updateChildNewsForDoetDTO.Title,
                    NewsFaqcontent = updateChildNewsForDoetDTO.ChildNewscontent,
                    Image = updateChildNewsForDoetDTO.Image
                };

                var updatedChildNewsResult = await _newsFaqRepository.UpdateChildNewsForDoetAsync(newsFaq);

                var childNewsDto = _mapper.Map<UpdateChildNewsForDoetDTO>(updatedChildNewsResult);

                return new DataResponse<UpdateChildNewsForDoetDTO>
                {
                    Data = childNewsDto,
                    Message = "Child News updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildNewsForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildNewsForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating child news: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteChildNewsForDoetDTO>> DeleteChildNewsForDoetAsync(DeleteChildNewsForDoetDTO deleteChildNewsForDoetDTO)
        {
            try
            {
                var deletedChildNewsResult = await _newsFaqRepository.DeleteChildNewsForDoetAsync(deleteChildNewsForDoetDTO.ChildNewsId);

                var childNewsDto = _mapper.Map<DeleteChildNewsForDoetDTO>(deletedChildNewsResult);

                return new DataResponse<DeleteChildNewsForDoetDTO>
                {
                    Data = childNewsDto,
                    Message = "Child News has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteChildNewsForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteChildNewsForDoetDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting child News: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildNewsStatusForDoetDTO>> UpdateChildNewsStatusForDoetAsync(UpdateChildNewsStatusForDoetDTO updateChildNewsStatusForDoetDTO)
        {
            try
            {
                var existingChildNews = await _newsFaqRepository.GetChildNewsByIdForDoetAsync(updateChildNewsStatusForDoetDTO.ChildNewsId);

                if (existingChildNews == null)
                {
                    throw new KeyNotFoundException("Child News not found");
                }

                var parentNews = await _newsFaqRepository.GetParentNewsByIdForDoetAsync(existingChildNews.ParentId.Value);

                if (parentNews == null)
                {
                    throw new KeyNotFoundException("Parent News not found");
                }

                if (parentNews.Status == "Unactive" && updateChildNewsStatusForDoetDTO.Status == "Active")
                {
                    throw new InvalidOperationException("Cannot update status child news to Active when parent news is Unactive");
                }

                // Tạo đối tượng NewsFaq từ DTO
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateChildNewsStatusForDoetDTO.ChildNewsId,
                    Status = updateChildNewsStatusForDoetDTO.Status
                };

                var updatedChildNewsStatusResult = await _newsFaqRepository.UpdateChildNewsForDoetAsync(newsFaq);

                var childNewsDto = _mapper.Map<UpdateChildNewsStatusForDoetDTO>(updatedChildNewsStatusResult);

                return new DataResponse<UpdateChildNewsStatusForDoetDTO>
                {
                    Data = childNewsDto,
                    Message = "Child News updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildNewsStatusForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildNewsStatusForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating child News: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        // Doet - Parent Faq Management
        public async Task<DataResponse<PagedResponse<List<ParentFaqListForDoetDTO>>>> GetAllParentFaqForDoetAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var parentFaq = await _newsFaqRepository.GetAllParentFaqForDoetAsync(title, roleId, status);

                var totalParentFaq = parentFaq.Count();
                var totalPages = totalParentFaq == 0 ? 1 : (int)Math.Ceiling((double)totalParentFaq / pageSize);

                var parentFaqDtos = parentFaq
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ParentFaqListForDoetDTO
                    {
                        ParentFaqId = doc.NewsFaqid,
                        User = doc.User?.Name,
                        Title = doc.Title,
                        Image = doc.Image,
                        ParentFaqcontent = doc.NewsFaqcontent,
                        Status = doc.Status,
                        ForRole = doc.NewsFaqroles != null && doc.NewsFaqroles.Any()
                            ? string.Join(", ", doc.NewsFaqroles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ParentFaqListForDoetDTO>>
                {
                    Items = parentFaqDtos,
                    TotalCount = totalParentFaq,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ParentFaqListForDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Parent faq list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ParentFaqListForDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ParentFaqListForDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving parent faq list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ParentFaqDetailForDoetDTO>> GetParentFaqDetailByIdForDoetAsync(int faqId)
        {
            try
            {
                var parentFaq = await _newsFaqRepository.GetParentFaqByIdForDoetAsync(faqId);

                var parentFaqDto = _mapper.Map<ParentFaqDetailForDoetDTO>(parentFaq);

                return new DataResponse<ParentFaqDetailForDoetDTO>
                {
                    Data = parentFaqDto,
                    Message = "Parent faq details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ParentFaqDetailForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ParentFaqDetailForDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving Parent faq details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddParentFaqForDoetDTO>> AddParentFaqForDoetAsync(AddParentFaqForDoetDTO addParentFaqForDoetDTO)
        {
            try
            {
                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (addParentFaqForDoetDTO.ForRoleIds.Contains(null) || addParentFaqForDoetDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    addParentFaqForDoetDTO.ForRoleIds = new List<int?> { null };
                }

                var newsFaq = new NewsFaq
                {
                    UserId = addParentFaqForDoetDTO.UserId,
                    Title = addParentFaqForDoetDTO.Title,
                    NewsFaqcontent = addParentFaqForDoetDTO.ParentFaqcontent,
                    Image = addParentFaqForDoetDTO.Image
                };

                var addFaqResult = await _newsFaqRepository.AddParentFaqForDoetAsync(newsFaq, addParentFaqForDoetDTO.ForRoleIds);

                var resultDto = _mapper.Map<AddParentFaqForDoetDTO>(addFaqResult);

                return new DataResponse<AddParentFaqForDoetDTO>
                {
                    Data = resultDto,
                    Message = "Parent Faq added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddParentFaqForDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding parent faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentFaqForDoetDTO>> UpdateParentFaqForDoetAsync(UpdateParentFaqForDoetDTO updateParentFaqForDoetDTO)
        {
            try
            {
                var existingParentFaq = await _newsFaqRepository.GetParentFaqByIdForDoetAsync(updateParentFaqForDoetDTO.ParentFaqId);
                if (existingParentFaq == null)
                {
                    throw new KeyNotFoundException("Parent Faq not found");
                }

                // Kiểm tra nếu danh sách RoleIds có chứa null hoặc 0 (tương ứng với role Guest)
                if (updateParentFaqForDoetDTO.ForRoleIds.Contains(null) || updateParentFaqForDoetDTO.ForRoleIds.Contains(0))
                {
                    // Nếu role Guest tồn tại, chỉ giữ lại role Guest (loại bỏ các role khác)
                    updateParentFaqForDoetDTO.ForRoleIds = new List<int?> { null };
                }

                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateParentFaqForDoetDTO.ParentFaqId,
                    Title = updateParentFaqForDoetDTO.Title,
                    NewsFaqcontent = updateParentFaqForDoetDTO.ParentFaqcontent,
                    Image = updateParentFaqForDoetDTO.Image
                };

                var updatedParentFaqResult = await _newsFaqRepository.UpdateParentFaqForDoetAsync(newsFaq, updateParentFaqForDoetDTO.ForRoleIds);

                var parentFaqDto = _mapper.Map<UpdateParentFaqForDoetDTO>(updatedParentFaqResult);

                return new DataResponse<UpdateParentFaqForDoetDTO>
                {
                    Data = parentFaqDto,
                    Message = "Parent Faq updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentFaqForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentFaqForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating parent faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteParentFaqForDoetDTO>> DeleteParentFaqForDoetAsync(DeleteParentFaqForDoetDTO deleteParentFaqForDoetDTO)
        {
            try
            {
                var childFaqList = await _newsFaqRepository.GetAllChildFaqByParentIdForDoetAsync(deleteParentFaqForDoetDTO.ParentFaqId);

                var deletedParentFaqResult = await _newsFaqRepository.DeleteParentFaqForDoetAsync(deleteParentFaqForDoetDTO.ParentFaqId);

                var childFaqDtoList = _mapper.Map<List<DeleteChildFaqForDoetDTO>>(childFaqList);

                var parentFaqDto = _mapper.Map<DeleteParentFaqForDoetDTO>(deletedParentFaqResult);
                parentFaqDto.DeletedChildFaq = childFaqDtoList;

                return new DataResponse<DeleteParentFaqForDoetDTO>
                {
                    Data = parentFaqDto,
                    Message = "Parent Faq has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteParentFaqForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteParentFaqForDoetDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting Parent Faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateParentFaqStatusForDoetDTO>> UpdateParentFaqStatusForDoetAsync(UpdateParentFaqStatusForDoetDTO updateParentFaqStatusForDoetDTO)
        {
            try
            {
                // Tạo đối tượng NewsFaq từ DTO
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateParentFaqStatusForDoetDTO.ParentFaqId,
                    Status = updateParentFaqStatusForDoetDTO.Status
                };

                var updatedParentFaqStatusResult = await _newsFaqRepository.UpdateParentFaqStatusForDoetAsync(newsFaq);

                var childFaqList = await _newsFaqRepository.GetAllChildFaqByParentIdForDoetAsync(updateParentFaqStatusForDoetDTO.ParentFaqId);

                var childFaqDtoList = _mapper.Map<List<UpdateChildFaqStatusForDoetDTO>>(childFaqList);

                var parentFaqDto = _mapper.Map<UpdateParentFaqStatusForDoetDTO>(updatedParentFaqStatusResult);

                parentFaqDto.ChangedStatusChildFaq = childFaqDtoList;

                return new DataResponse<UpdateParentFaqStatusForDoetDTO>
                {
                    Data = parentFaqDto,
                    Message = "Parent Faq updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateParentFaqStatusForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateParentFaqStatusForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating Parent Faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }


        public async Task<DataResponse<List<StatusFaqListForDoetDTO>>> GetAllStatusesFaqForDoetAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusFaqListForDoetDTO>
                {
                    new StatusFaqListForDoetDTO { Status = "Active" },
                    new StatusFaqListForDoetDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusFaqListForDoetDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusFaqListForDoetDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<List<StatusFaqListForDoetDTO>>
                {
                    Data = null,
                    Message = $"Access denied while get status list: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusFaqListForDoetDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Doet - Child Faq Management
        public async Task<DataResponse<PagedResponse<List<ChildFaqListForDoetDTO>>>> GetAllChildFaqForDoetAsync(int parentId, string? title, int? roleId, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var childFaq = await _newsFaqRepository.GetAllChildFaqForDoetAsync(parentId, title, roleId, status);

                var totalChildFaq = childFaq.Count();
                var totalPages = totalChildFaq == 0 ? 1 : (int)Math.Ceiling((double)totalChildFaq / pageSize);

                var childFaqDtos = childFaq
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(doc => new ChildFaqListForDoetDTO
                    {
                        ChildFaqId = doc.NewsFaqid,
                        User = doc.User?.Name,
                        Title = doc.Title,
                        Image = doc.Image,
                        ChildFaqcontent = doc.NewsFaqcontent,
                        Status = doc.Status,
                        ForRole = doc.NewsFaqroles != null && doc.NewsFaqroles.Any()
                            ? string.Join(", ", doc.NewsFaqroles.Select(dr => dr.Role?.Name ?? "Guest"))
                            : "Guest" // Nếu không có role, mặc định là Guest
                    })
                    .ToList();

                var pagedResponse = new PagedResponse<List<ChildFaqListForDoetDTO>>
                {
                    Items = childFaqDtos,
                    TotalCount = totalChildFaq,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<ChildFaqListForDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Parent faq list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<ChildFaqListForDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<ChildFaqListForDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving child faq list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<ChildFaqDetailForDoetDTO>> GetChildFaqDetailByIdForDoetAsync(int faqId)
        {
            try
            {
                var childFaq = await _newsFaqRepository.GetChildFaqByIdForDoetAsync(faqId);

                var childFaqDto = _mapper.Map<ChildFaqDetailForDoetDTO>(childFaq);

                return new DataResponse<ChildFaqDetailForDoetDTO>
                {
                    Data = childFaqDto,
                    Message = "Child faq details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<ChildFaqDetailForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<ChildFaqDetailForDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving child faq details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddChildFaqForDoetDTO>> AddChildFaqForDoetAsync(AddChildFaqForDoetDTO addChildFaqForDoetDTO)
        {
            try
            {
                var newsFaq = new NewsFaq
                {
                    UserId = addChildFaqForDoetDTO.UserId,
                    ParentId = addChildFaqForDoetDTO.ParentId,
                    Title = addChildFaqForDoetDTO.Title,
                    NewsFaqcontent = addChildFaqForDoetDTO.ChildFaqcontent,
                    Image = addChildFaqForDoetDTO.Image
                };

                var addNewsFaqResult = await _newsFaqRepository.AddChildFaqForAdminAsync(newsFaq);

                // Cập nhật thời gian tạo vào DTO trả về
                addChildFaqForDoetDTO.CreatedAt = addNewsFaqResult.CreatedAt;
                addChildFaqForDoetDTO.Status = addNewsFaqResult.Status;

                return new DataResponse<AddChildFaqForDoetDTO>
                {
                    Data = addChildFaqForDoetDTO,
                    Message = "Child Faq added successfully!",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddChildFaqForDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding child faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildFaqForDoetDTO>> UpdateChildFaqForDoetAsync(UpdateChildFaqForDoetDTO updateChildFaqForDoetDTO)
        {
            try
            {
                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateChildFaqForDoetDTO.ChildFaqId,
                    Title = updateChildFaqForDoetDTO.Title,
                    NewsFaqcontent = updateChildFaqForDoetDTO.ChildFaqcontent,
                    Image = updateChildFaqForDoetDTO.Image
                };

                var updatedChildFaqResult = await _newsFaqRepository.UpdateChildFaqForAdminAsync(newsFaq);

                var childFaqDto = _mapper.Map<UpdateChildFaqForDoetDTO>(updatedChildFaqResult);

                return new DataResponse<UpdateChildFaqForDoetDTO>
                {
                    Data = childFaqDto,
                    Message = "Child Faq updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildFaqForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildFaqForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating child faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<DeleteChildFaqForDoetDTO>> DeleteChildFaqForDoetAsync(DeleteChildFaqForDoetDTO deleteChildFaqForDoetDTO)
        {
            try
            {
                var deletedChildFaqResult = await _newsFaqRepository.DeleteChildFaqForDoetAsync(deleteChildFaqForDoetDTO.ChildFaqId);

                var childFaqDto = _mapper.Map<DeleteChildFaqForDoetDTO>(deletedChildFaqResult);

                return new DataResponse<DeleteChildFaqForDoetDTO>
                {
                    Data = childFaqDto,
                    Message = "Child Faq has been permanently deleted successfully.",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<DeleteChildFaqForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteChildFaqForDoetDTO>
                {
                    Data = null,
                    Message = $"Error permanently deleting child Faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        public async Task<DataResponse<UpdateChildFaqStatusForDoetDTO>> UpdateChildFaqStatusForDoetAsync(UpdateChildFaqStatusForDoetDTO updateChildFaqStatusForDoetDTO)
        {
            try
            {
                var existingChildFaq = await _newsFaqRepository.GetChildFaqByIdForDoetAsync(updateChildFaqStatusForDoetDTO.ChildFaqId);

                if (existingChildFaq == null)
                {
                    throw new KeyNotFoundException("Child Faq not found");
                }

                var parentFaq = await _newsFaqRepository.GetParentFaqByIdForDoetAsync(existingChildFaq.ParentId.Value);

                if (parentFaq == null)
                {
                    throw new KeyNotFoundException("Parent Faq not found");
                }

                if (parentFaq.Status == "Unactive" && updateChildFaqStatusForDoetDTO.Status == "Active")
                {
                    throw new InvalidOperationException("Cannot update status child faq to Active when parent faq is Unactive");
                }

                var newsFaq = new NewsFaq
                {
                    NewsFaqid = updateChildFaqStatusForDoetDTO.ChildFaqId,
                    Status = updateChildFaqStatusForDoetDTO.Status
                };

                var updatedChildFaqStatusResult = await _newsFaqRepository.UpdateChildFaqForDoetAsync(newsFaq);

                var childFaqDto = _mapper.Map<UpdateChildFaqStatusForDoetDTO>(updatedChildFaqStatusResult);

                return new DataResponse<UpdateChildFaqStatusForDoetDTO>
                {
                    Data = childFaqDto,
                    Message = "Child Faq updated successfully!",
                    StatusCode = 200 // OK
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<UpdateChildFaqStatusForDoetDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateChildFaqStatusForDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating child faq: {ex.Message}",
                    StatusCode = 500 // Internal Server Error
                };
            }
        }

        // Common - News
        public async Task<DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>> GetAllNewsAsync(string role, string? title, int pageNumber, int pageSize)
        {
            try
            {
                var newsList = await _newsFaqRepository.GetAllNewsAsync(role, title);

                var totalNews = newsList.Count();
                var totalPages = totalNews == 0 ? 1 : (int)Math.Ceiling((double)totalNews / pageSize);

                var NewsDtos = totalNews > 0 ? _mapper.Map<List<NewsFaqListForCommonDTO>>(newsList)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<NewsFaqListForCommonDTO>();

                var pagedResponse = new PagedResponse<List<NewsFaqListForCommonDTO>>
                {
                    Items = NewsDtos,
                    TotalCount = totalNews,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>
                {
                    Data = pagedResponse,
                    Message = "News list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving news list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        public async Task<DataResponse<NewsFaqDetailForCommonDTO>> GetNewsDetailAsync(int? newsId, string role)
        {
            try
            {
                var news = await _newsFaqRepository.GetNewsDetailAsync(newsId, role);
                var newsDto = _mapper.Map<NewsFaqDetailForCommonDTO>(news);

                return new DataResponse<NewsFaqDetailForCommonDTO>
                {
                    StatusCode = 200,
                    Message = "News detail retrieved successfully!",
                    Data = newsDto
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<NewsFaqDetailForCommonDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<NewsFaqDetailForCommonDTO>
                {
                    Data = null,
                    Message = $"Error retrieving news details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<List<NewsFaqListForCommonDTO>>> GetAllNewsContentForNewsParentAsync(int? parentId, string role)
        {
            try
            {
                var newsList = await _newsFaqRepository.GetAllNewsContentForNewsParentAsync(parentId, role);
                var newsListDto = _mapper.Map<List<NewsFaqListForCommonDTO>>(newsList);

                return new DataResponse<List<NewsFaqListForCommonDTO>>
                {
                    StatusCode = 200,
                    Message = "News content list for news parent retrieved successfully!",
                    Data = newsListDto
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<NewsFaqListForCommonDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<NewsFaqListForCommonDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving news content list for parent: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // Common - Faqs
        public async Task<DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>> GetAllFaqsAsync(string role, string? title, int pageNumber, int pageSize)
        {
            try
            {
                var faqsList = await _newsFaqRepository.GetAllFaqsAsync(role, title);

                var totalFaqs = faqsList.Count();
                var totalPages = totalFaqs == 0 ? 1 : (int)Math.Ceiling((double)totalFaqs / pageSize);

                var faqsDtos = totalFaqs > 0 ? _mapper.Map<List<NewsFaqListForCommonDTO>>(faqsList)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<NewsFaqListForCommonDTO>();

                var pagedResponse = new PagedResponse<List<NewsFaqListForCommonDTO>>
                {
                    Items = faqsDtos,
                    TotalCount = totalFaqs,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Faqs list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving faqs list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }


        public async Task<DataResponse<NewsFaqDetailForCommonDTO>> GetFaqsDetailAsync(int? faqId, string role)
        {
            try
            {
                var faqs = await _newsFaqRepository.GetFaqsDetailAsync(faqId, role);
                var faqsDto = _mapper.Map<NewsFaqDetailForCommonDTO>(faqs);

                return new DataResponse<NewsFaqDetailForCommonDTO>
                {
                    StatusCode = 200,
                    Message = "Faqs detail retrieved successfully!",
                    Data = faqsDto
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<NewsFaqDetailForCommonDTO>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<NewsFaqDetailForCommonDTO>
                {
                    Data = null,
                    Message = $"Error retrieving faqs details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<List<NewsFaqListForCommonDTO>>> GetAllFaqsContentForFaqsParentAsync(int? parentId, string role)
        {
            try
            {
                var faqsList = await _newsFaqRepository.GetAllFaqsContentForFaqsParentAsync(parentId, role);
                var faqsListDto = _mapper.Map<List<NewsFaqListForCommonDTO>>(faqsList);

                return new DataResponse<List<NewsFaqListForCommonDTO>>
                {
                    StatusCode = 200,
                    Message = "Faqs content list for news parent retrieved successfully!",
                    Data = faqsListDto
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<NewsFaqListForCommonDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<NewsFaqListForCommonDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving faqs content list for parent: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
