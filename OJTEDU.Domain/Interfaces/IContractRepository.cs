using OJTEDU.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Domain.Interfaces
{
    public interface IContractRepository
    {
        /*
         + Contract status: 
        0: Inactive 
        1: Active
         */

        // Mentor 
        Task<Contract> AssignContractAsync(int? userId, int? internshipId, string? fileName, Contract? info, string? employeeCode);
        //Task<Contract> UpdateContractAssignedAsync(int? contractId);
        //Task<Contract> DeleteForStoredContractAssignedAsync(int? contractId);
        // ký online 
    }
}
