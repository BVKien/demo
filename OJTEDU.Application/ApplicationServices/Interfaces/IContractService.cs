using OJTEDU.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OJTEDU.Application.DTOs.ContractDTO;
using static OJTEDU.Application.DTOs.DocumentDTO;

namespace OJTEDU.Application.ApplicationServices.Interfaces
{
    public interface IContractService
    {
        // Mentor 
        Task<DataResponse<AssignContractInternshipForMentorDTO>> AssignContractAsync(int? userId, int? internshipId, string? fileName, AssignContractInternshipForMentorDTO? info);
    }
}
