using AutoMapper;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.DTOs;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static OJTEDU.Application.DTOs.ContractDTO;
using static OJTEDU.Application.DTOs.DocumentDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IMapper _mapper;
        public ContractService(IContractRepository contractRepository, IMapper mapper)
        {
            _contractRepository = contractRepository;
            _mapper = mapper;
        }

        // Mentor 
        public async Task<DataResponse<AssignContractInternshipForMentorDTO>> AssignContractAsync(int? userId, int? internshipId, string? fileName,
            AssignContractInternshipForMentorDTO? info)
        {
            try
            {
                if (userId == null)
                {
                    return new DataResponse<AssignContractInternshipForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found mentor.",
                        Data = null
                    };
                }

                if (internshipId == null)
                {
                    return new DataResponse<AssignContractInternshipForMentorDTO>
                    {
                        StatusCode = 404,
                        Message = "Not found internship.",
                        Data = null
                    };
                }

                var contractInfo = new Contract
                {
                    Name = info?.Name
                };

                var contract = await _contractRepository.AssignContractAsync(userId, internshipId, fileName, contractInfo);
                var response = _mapper.Map<AssignContractInternshipForMentorDTO>(contract);

                return new DataResponse<AssignContractInternshipForMentorDTO>
                {
                    StatusCode = 200,
                    Message = "Assign contract successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AssignContractInternshipForMentorDTO>
                {
                    StatusCode = 500,
                    Message = $"Error assign contract: {ex.Message}. ",
                    Data = null
                };
            }
        }
    }
}
