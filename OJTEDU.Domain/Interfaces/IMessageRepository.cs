using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IMessageRepository
    {
        /*
         + Message status: Đây là status của từng tin nhắn
        0: Deleted 
        1: Active 
         */

        // admin - company, doet, dean, lecturer
        // doet - company, doet, admin, doet, dean, lecturer
        // dean - admin, doet, dean, lecturer
        // lecturer - lec, admin, dean, mentor, student
        // student - student, mentor, lecturer
        // mentor - mentor, lecturer, company, student

        // Admin, DOET, Dean, Lecturer, Mentor, Company, Student
        Task<Message> CreateFirstMessageConversationAsync(int? userId, int? receiverId, string? messageFileName, string? imageFileName, Message? messageInfo);
        Task<Message> CreateMessageAsync(int? userId, string? messageFileName, string? imageFileName, Message? messageInfo);
        Task<IEnumerable<Message>> GetAllMessageInConversationAsync(int? conversationId);
        // check is read -> is seen
    }
}
