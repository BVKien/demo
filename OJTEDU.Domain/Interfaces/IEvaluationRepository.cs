using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IEvaluationRepository
    {
        /*
         + Evaluation status
        0: Failed
        1: Pass - condition evaluation score >= 5 && pass from company
         */

        // Thời gian chấm điểm trong khoảng thời gian trong kỳ,
        // từ lúc được chấp nhận thực tập cho đến khi sinh viên được đánh pass hoặc fail từ công ty
        // nếu đánh pass -> chấm điểm từ tổng của mentor và lec, dean + và chia đều vào từng ô điểm
        // Điểm thổng cuối cùng của sinh viên sẽ là ((company score + uni score)/2)*80% + điểm chuyên cần cty*20%
        // comment đến từ 2 phía company và uni

        // Nếu fail -> điểm = 0 -> đánh trường lý do từ công ty vào evaluation

        // University, Company
        Task<Evaluation> CreateEvaluationAsync(int? userId, int? internshipId, Evaluation? info);
        Task<Evaluation> UpdateEvaluationAsync(int? userId, int? internshipId, Evaluation? info);

        // University, Company, Student
        Task<Evaluation> GetEvaluationDetailByUserId(int? userId);
        Task<Evaluation> GetEvaluationDetailByInternshipId(int? internshipId);
        Task<Evaluation> GetEvaluationScoreAsync(int? userId);
    }
}
