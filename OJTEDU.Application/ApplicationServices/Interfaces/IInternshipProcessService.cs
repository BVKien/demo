using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.InternshipProcessDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IInternshipProcessService
    {
        // AdminDoet-DOET - Internship Process Management
        Task<DataResponse<PagedResponse<List<InternshipProcessListForAdminDoetDTO>>>> GetAllInternshipProcessForAdminDoetAsync(string? title, bool? isVisible, int pageNumber, int pageSize);
        Task<DataResponse<InternshipProcessDetailForAdminDoetDTO>> GetInternshipProcessDetailByIdForAdminDoetAsync(int InternshipProcessId);

        Task<DataResponse<AddInternshipProcessForAdminDoetDTO>> AddInternshipProcessForAdminDoetAsync(AddInternshipProcessForAdminDoetDTO addInternshipProcessForAdminDoetDTO);

        Task<DataResponse<UpdateInternshipProcessForAdminDoetDTO>> UpdateInternshipProcessForAdminDoetAsync(UpdateInternshipProcessForAdminDoetDTO updateInternshipProcessForAdminDoetDTO);

        Task<DataResponse<UpdateInternshipProcessForAdminDoetDTO>> UpdateInternshipProcessVisibleForAdminDoetAsync(UpdateInternshipProcessForAdminDoetDTO updateInternshipProcessForAdminDoetDTO);

        Task<DataResponse<DeleteInternshipProcessForAdminDoetDTO>> DeleteInternshipProcessForAdminDoetAsync(DeleteInternshipProcessForAdminDoetDTO deleteInternshipProcessForAdminDoetDTO); 

        // Common
        Task<DataResponse<InternshipProcessDetailForAdminDoetDTO>> GetInternshipProcessByVisibleAsync();
    }
}
