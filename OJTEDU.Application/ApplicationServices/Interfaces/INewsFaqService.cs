using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.NewsFaqDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface INewsFaqService
    {
        // Admin - Parent News Management
        Task<DataResponse<PagedResponse<List<ParentNewsListForAdminDTO>>>> GetAllParentNewsForAdminAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ParentNewsDetailForAdminDTO>> GetParentNewsDetailByIdForAdminAsync(int newsId);
        Task<DataResponse<AddParentNewsForAdminDTO>> AddParentNewsForAdminAsync(AddParentNewsForAdminDTO addParentNewsForAdminDTO);
        Task<DataResponse<UpdateParentNewsForAdminDTO>> UpdateParentNewsForAdminAsync(UpdateParentNewsForAdminDTO updateParentNewsForAdminDTO);
        Task<DataResponse<UpdateParentNewsStatusForAdminDTO>> UpdateParentNewsStatusForAdminAsync(UpdateParentNewsStatusForAdminDTO updateParentNewsStatusForAdminDTO);
        Task<DataResponse<DeleteParentNewsForAdminDTO>> DeleteParentNewsForAdminAsync(DeleteParentNewsForAdminDTO deleteParentNewsForAdminDTO);
        Task<DataResponse<List<StatusNewsListForAdminDTO>>> GetAllStatusesNewsForAdminAsync();

        // Admin - Child News Management
        Task<DataResponse<PagedResponse<List<ChildNewsListForAdminDTO>>>> GetAllChildNewsForAdminAsync(int parentId, string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ChildNewsDetailForAdminDTO>> GetChildNewsDetailByIdForAdminAsync(int newsId);
        Task<DataResponse<AddChildNewsForAdminDTO>> AddChildNewsForAdminAsync(AddChildNewsForAdminDTO addChildNewsForAdminDTO);
        Task<DataResponse<UpdateChildNewsForAdminDTO>> UpdateChildNewsForAdminAsync(UpdateChildNewsForAdminDTO updateChildNewsForAdminDTO);
        Task<DataResponse<UpdateChildNewsStatusForAdminDTO>> UpdateChildNewsStatusForAdminAsync(UpdateChildNewsStatusForAdminDTO updateChildNewsStatusForAdminDTO);
        Task<DataResponse<DeleteChildNewsForAdminDTO>> DeleteChildNewsForAdminAsync(DeleteChildNewsForAdminDTO deleteChildNewsForAdminDTO);

        // Admin - Parent Faq Management
        Task<DataResponse<PagedResponse<List<ParentFaqListForAdminDTO>>>> GetAllParentFaqForAdminAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ParentFaqDetailForAdminDTO>> GetParentFaqDetailByIdForAdminAsync(int faqId);
        Task<DataResponse<AddParentFaqForAdminDTO>> AddParentFaqForAdminAsync(AddParentFaqForAdminDTO addParentFaqForAdminDTO);
        Task<DataResponse<UpdateParentFaqForAdminDTO>> UpdateParentFaqForAdminAsync(UpdateParentFaqForAdminDTO updateParentFaqForAdminDTO);
        Task<DataResponse<UpdateParentFaqStatusForAdminDTO>> UpdateParentFaqStatusForAdminAsync(UpdateParentFaqStatusForAdminDTO updateParentFaqStatusForAdminDTO);
        Task<DataResponse<DeleteParentFaqForAdminDTO>> DeleteParentFaqForAdminAsync(DeleteParentFaqForAdminDTO deleteParentFaqForAdminDTO);
        Task<DataResponse<List<StatusFaqListForAdminDTO>>> GetAllStatusesFaqForAdminAsync();

        // Admin - Child Faq Management
        Task<DataResponse<PagedResponse<List<ChildFaqListForAdminDTO>>>> GetAllChildFaqForAdminAsync(int parentId, string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ChildFaqDetailForAdminDTO>> GetChildFaqDetailByIdForAdminAsync(int faqId);
        Task<DataResponse<AddChildFaqForAdminDTO>> AddChildFaqForAdminAsync(AddChildFaqForAdminDTO addChildFaqForAdminDTO);
        Task<DataResponse<UpdateChildFaqForAdminDTO>> UpdateChildFaqForAdminAsync(UpdateChildFaqForAdminDTO updateChildFaqForAdminDTO);
        Task<DataResponse<UpdateChildFaqStatusForAdminDTO>> UpdateChildFaqStatusForAdminAsync(UpdateChildFaqStatusForAdminDTO updateChildFaqStatusForAdminDTO);
        Task<DataResponse<DeleteChildFaqForAdminDTO>> DeleteChildFaqForAdminAsync(DeleteChildFaqForAdminDTO deleteChildFaqForAdminDTO);

        // Doet - Parent News Management
        Task<DataResponse<PagedResponse<List<ParentNewsListForDoetDTO>>>> GetAllParentNewsForDoetAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ParentNewsDetailForDoetDTO>> GetParentNewsDetailByIdForDoetAsync(int newsId);
        Task<DataResponse<AddParentNewsForDoetDTO>> AddParentNewsForDoetAsync(AddParentNewsForDoetDTO addParentNewsForDoetDTO);
        Task<DataResponse<UpdateParentNewsForDoetDTO>> UpdateParentNewsForDoetAsync(UpdateParentNewsForDoetDTO updateParentNewsForDoetDTO);
        Task<DataResponse<UpdateParentNewsStatusForDoetDTO>> UpdateParentNewsStatusForDoetAsync(UpdateParentNewsStatusForDoetDTO updateParentNewsStatusForDoetDTO);
        Task<DataResponse<DeleteParentNewsForDoetDTO>> DeleteParentNewsForDoetAsync(DeleteParentNewsForDoetDTO deleteParentNewsForDoetDTO);
        Task<DataResponse<List<StatusNewsListForDoetDTO>>> GetAllStatusesNewsForDoetAsync();

        // Doet - Child News Management
        Task<DataResponse<PagedResponse<List<ChildNewsListForDoetDTO>>>> GetAllChildNewsForDoetAsync(int parentId, string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ChildNewsDetailForDoetDTO>> GetChildNewsDetailByIdForDoetAsync(int newsId);
        Task<DataResponse<AddChildNewsForDoetDTO>> AddChildNewsForDoetAsync(AddChildNewsForDoetDTO addChildNewsForDoetDTO);
        Task<DataResponse<UpdateChildNewsForDoetDTO>> UpdateChildNewsForDoetAsync(UpdateChildNewsForDoetDTO updateChildNewsForDoetDTO);
        Task<DataResponse<UpdateChildNewsStatusForDoetDTO>> UpdateChildNewsStatusForDoetAsync(UpdateChildNewsStatusForDoetDTO updateChildNewsStatusForDoetDTO);
        Task<DataResponse<DeleteChildNewsForDoetDTO>> DeleteChildNewsForDoetAsync(DeleteChildNewsForDoetDTO deleteChildNewsForDoetDTO);

        // Doet - Parent Faq Management
        Task<DataResponse<PagedResponse<List<ParentFaqListForDoetDTO>>>> GetAllParentFaqForDoetAsync(string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ParentFaqDetailForDoetDTO>> GetParentFaqDetailByIdForDoetAsync(int faqId);
        Task<DataResponse<AddParentFaqForDoetDTO>> AddParentFaqForDoetAsync(AddParentFaqForDoetDTO addParentFaqForDoetDTO);
        Task<DataResponse<UpdateParentFaqForDoetDTO>> UpdateParentFaqForDoetAsync(UpdateParentFaqForDoetDTO updateParentFaqForDoetDTO);
        Task<DataResponse<UpdateParentFaqStatusForDoetDTO>> UpdateParentFaqStatusForDoetAsync(UpdateParentFaqStatusForDoetDTO updateParentFaqStatusForDoetDTO);
        Task<DataResponse<DeleteParentFaqForDoetDTO>> DeleteParentFaqForDoetAsync(DeleteParentFaqForDoetDTO deleteParentFaqForDoetDTO);
        Task<DataResponse<List<StatusFaqListForDoetDTO>>> GetAllStatusesFaqForDoetAsync();

        // Doet - Child Faq Management
        Task<DataResponse<PagedResponse<List<ChildFaqListForDoetDTO>>>> GetAllChildFaqForDoetAsync(int parentId, string? title, int? roleId, string? status, int pageNumber, int pageSize);
        Task<DataResponse<ChildFaqDetailForDoetDTO>> GetChildFaqDetailByIdForDoetAsync(int faqId);
        Task<DataResponse<AddChildFaqForDoetDTO>> AddChildFaqForDoetAsync(AddChildFaqForDoetDTO addChildFaqForDoetDTO);
        Task<DataResponse<UpdateChildFaqForDoetDTO>> UpdateChildFaqForDoetAsync(UpdateChildFaqForDoetDTO updateChildFaqForDoetDTO);
        Task<DataResponse<UpdateChildFaqStatusForDoetDTO>> UpdateChildFaqStatusForDoetAsync(UpdateChildFaqStatusForDoetDTO updateChildFaqStatusForDoetDTO);
        Task<DataResponse<DeleteChildFaqForDoetDTO>> DeleteChildFaqForDoetAsync(DeleteChildFaqForDoetDTO deleteChildFaqForDoetDTO);

        // Common - News
        Task<DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>> GetAllNewsAsync(string role, string? title, int pageNumber, int pageSize);
        Task<DataResponse<NewsFaqDetailForCommonDTO>> GetNewsDetailAsync(int? newsId, string role);
        Task<DataResponse<List<NewsFaqListForCommonDTO>>> GetAllNewsContentForNewsParentAsync(int? parentId, string role);

        // Common - Faqs
        Task<DataResponse<PagedResponse<List<NewsFaqListForCommonDTO>>>> GetAllFaqsAsync(string role, string? title, int pageNumber, int pageSize);
        Task<DataResponse<NewsFaqDetailForCommonDTO>> GetFaqsDetailAsync(int? faqId, string role);
        Task<DataResponse<List<NewsFaqListForCommonDTO>>> GetAllFaqsContentForFaqsParentAsync(int? parentId, string role);
    }
}
