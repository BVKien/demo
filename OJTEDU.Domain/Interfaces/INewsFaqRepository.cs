using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface INewsFaqRepository
    {
        // Admin - Parent News Management
        Task<IEnumerable<NewsFaq>> GetAllParentNewsForAdminAsync(string? title, int? roleId, string? status);
        Task<NewsFaq> GetParentNewsByIdForAdminAsync(int newsId);
        Task<NewsFaq> AddParentNewsForAdminAsync(NewsFaq newsFaq, List<int?> roleIds);
        Task<NewsFaq> UpdateParentNewsForAdminAsync(NewsFaq newsFaq, List<int?> newRoleIds);
        Task<NewsFaq> UpdateParentNewsStatusForAdminAsync(NewsFaq newsFaq);
        Task<NewsFaq> DeleteParentNewsForAdminAsync(int newsId);

        // Admin - Child News Management
        Task<IEnumerable<NewsFaq>> GetAllChildNewsByParentIdForAdminAsync(int? parentId);
        Task<IEnumerable<NewsFaq>> GetAllChildNewsForAdminAsync(int parentId, string? title, int? roleId, string? status);
        Task<NewsFaq> GetChildNewsByIdForAdminAsync(int newsId);
        Task<NewsFaq> AddChildNewsForAdminAsync(NewsFaq newsFaq);
        Task<NewsFaq> UpdateChildNewsForAdminAsync(NewsFaq newsFaq);
        Task<NewsFaq> DeleteChildNewsForAdminAsync(int newsId);


        // Admin - Parent Faq Management
        Task<IEnumerable<NewsFaq>> GetAllParentFaqForAdminAsync(string? title, int? roleId, string? status);
        Task<NewsFaq> GetParentFaqByIdForAdminAsync(int faqId);
        Task<NewsFaq> AddParentFaqForAdminAsync(NewsFaq newsFaq, List<int?> roleIds);
        Task<NewsFaq> UpdateParentFaqForAdminAsync(NewsFaq newsFaq, List<int?> faqRoleIds);
        Task<NewsFaq> UpdateParentFaqStatusForAdminAsync(NewsFaq newsFaq);
        Task<NewsFaq> DeleteParentFaqForAdminAsync(int faqId);

        // Admin - Child Faq Management
        Task<IEnumerable<NewsFaq>> GetAllChildFaqByParentIdForAdminAsync(int? parentId);
        Task<IEnumerable<NewsFaq>> GetAllChildFaqForAdminAsync(int parentId, string? title, int? roleId, string? status);
        Task<NewsFaq> GetChildFaqByIdForAdminAsync(int faqId);
        Task<NewsFaq> AddChildFaqForAdminAsync(NewsFaq newsFaq);
        Task<NewsFaq> UpdateChildFaqForAdminAsync(NewsFaq newsFaq);
        Task<NewsFaq> DeleteChildFaqForAdminAsync(int faqId);

        // Doet - Parent News Management
        Task<IEnumerable<NewsFaq>> GetAllParentNewsForDoetAsync(string? title, int? roleId, string? status);
        Task<NewsFaq> GetParentNewsByIdForDoetAsync(int newsId);
        Task<NewsFaq> AddParentNewsForDoetAsync(NewsFaq newsFaq, List<int?> roleIds);
        Task<NewsFaq> UpdateParentNewsForDoetAsync(NewsFaq newsFaq, List<int?> newRoleIds);
        Task<NewsFaq> UpdateParentNewsStatusForDoetAsync(NewsFaq newsFaq);
        Task<NewsFaq> DeleteParentNewsForDoetAsync(int newsId);

        // Doet - Child News Management
        Task<IEnumerable<NewsFaq>> GetAllChildNewsByParentIdForDoetAsync(int? parentId);
        Task<IEnumerable<NewsFaq>> GetAllChildNewsForDoetAsync(int parentId, string? title, int? roleId, string? status);
        Task<NewsFaq> GetChildNewsByIdForDoetAsync(int newsId);
        Task<NewsFaq> AddChildNewsForDoetAsync(NewsFaq newsFaq);
        Task<NewsFaq> UpdateChildNewsForDoetAsync(NewsFaq newsFaq);
        Task<NewsFaq> DeleteChildNewsForDoetAsync(int newsId);


        // Doet - Parent Faq Management
        Task<IEnumerable<NewsFaq>> GetAllParentFaqForDoetAsync(string? title, int? roleId, string? status);
        Task<NewsFaq> GetParentFaqByIdForDoetAsync(int faqId);
        Task<NewsFaq> AddParentFaqForDoetAsync(NewsFaq newsFaq, List<int?> roleIds);
        Task<NewsFaq> UpdateParentFaqForDoetAsync(NewsFaq newsFaq, List<int?> faqRoleIds);
        Task<NewsFaq> UpdateParentFaqStatusForDoetAsync(NewsFaq newsFaq);
        Task<NewsFaq> DeleteParentFaqForDoetAsync(int faqId);

        // Doet - Child Faq Management
        Task<IEnumerable<NewsFaq>> GetAllChildFaqByParentIdForDoetAsync(int? parentId);
        Task<IEnumerable<NewsFaq>> GetAllChildFaqForDoetAsync(int parentId, string? title, int? roleId, string? status);
        Task<NewsFaq> GetChildFaqByIdForDoetAsync(int faqId);
        Task<NewsFaq> AddChildFaqForDoetAsync(NewsFaq newsFaq);
        Task<NewsFaq> UpdateChildFaqForDoetAsync(NewsFaq newsFaq);
        Task<NewsFaq> DeleteChildFaqForDoetAsync(int faqId);

        // Common - News
        Task<IEnumerable<NewsFaq>> GetAllNewsAsync(string role, string? title);
        Task<NewsFaq> GetNewsDetailAsync(int? newsId, string role);
        Task<IEnumerable<NewsFaq>> GetAllNewsContentForNewsParentAsync(int? parentId, string role);

        // Common - Faqs
        Task<IEnumerable<NewsFaq>> GetAllFaqsAsync(string role, string? title);
        Task<NewsFaq> GetFaqsDetailAsync(int? faqId, string role);
        Task<IEnumerable<NewsFaq>> GetAllFaqsContentForFaqsParentAsync(int? parentId, string role);
    }
}
