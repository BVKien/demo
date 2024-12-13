using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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
using static OJTEDU.Application.DTOs.JobDTO;
using static OJTEDU.Application.DTOs.MajorDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        public MajorService(IMajorRepository majorRepository, IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _majorRepository = majorRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        // Admin - DOET
        public async Task<DataResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>> GetAllMajorForAdminDoetAsync(string? majorCode, string? majorName, string? status, int? departmentId, int pageNumber, int pageSize)
        {
            try
            {
                var majors = await _majorRepository.GetAllMajorForAdminDoetAsync(majorCode, majorName, status, departmentId);

                var totalMajors = majors.Count();
                var totalPages = totalMajors == 0 ? 1 : (int)Math.Ceiling((double)totalMajors / pageSize);

                var majorDtos = totalMajors > 0 ? _mapper.Map<List<MajorListForAdminDoetDTO>>(majors)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<MajorListForAdminDoetDTO>();

                var pagedResponse = new PagedResponse<List<MajorListForAdminDoetDTO>>
                {
                    Items = majorDtos,
                    TotalCount = totalMajors,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Major list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<MajorListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving major list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<MajorDetailForAdminDoetDTO>> GetMajorIdDetailByIdForAdminDoetAsync(int majorId)
        {
            try
            {
                var major = await _majorRepository.GetMajorByIdAsync(majorId);

                if (major == null)
                {
                    return new DataResponse<MajorDetailForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major not found!",
                        StatusCode = 404
                    };
                }

                var majorDto = _mapper.Map<MajorDetailForAdminDoetDTO>(major);

                return new DataResponse<MajorDetailForAdminDoetDTO>
                {
                    Data = majorDto,
                    Message = "Major details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<MajorDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving major details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddMajorForAdminDoetDTO>> AddMajorForAdminDoetAsync(AddMajorForAdminDoetDTO addMajorForAdminDoetDTO)
        {
            try
            {
                // Kiểm tra department có tồn tại và đang active không
                var department = await _departmentRepository.GetDepartmentByIdAsync(addMajorForAdminDoetDTO.DepartmentId.Value);

                if (department == null)
                {
                    return new DataResponse<AddMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404 // Not Found
                    };
                }

                if (!department.Status.Equals("Active"))
                {
                    return new DataResponse<AddMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Cannot add major because the department is not active.",
                        StatusCode = 400 // Bad Request
                    };
                }

                var existingMajor = await _majorRepository.GetMajorByCodeAsync(addMajorForAdminDoetDTO.MajorCode);

                if (existingMajor != null)
                {
                    return new DataResponse<AddMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major code already exists!",
                        StatusCode = 400 // Bad Request
                    };
                }

                var major = _mapper.Map<Major>(addMajorForAdminDoetDTO);
                major.CreatedAt = DateTime.Now;
                major.UpdatedAt = DateTime.Now;
                major.Status = "Active";
                await _majorRepository.AddMajorAsync(major);

                var addedMajorDto = _mapper.Map<AddMajorForAdminDoetDTO>(major);

                return new DataResponse<AddMajorForAdminDoetDTO>
                {
                    Data = addedMajorDto,
                    Message = "Major added successfully!",
                    StatusCode = 201 // Created
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddMajorForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding major: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateMajorForAdminDoetDTO>> UpdateMajorForAdminDoetAsync(UpdateMajorForAdminDoetDTO updateMajorForAdminDoetDTO)
        {
            try
            {
                var major = await _majorRepository.GetMajorByIdAsync(updateMajorForAdminDoetDTO.MajorId);

                if (major == null)
                {
                    return new DataResponse<UpdateMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major not found!",
                        StatusCode = 404
                    };
                }

                if (updateMajorForAdminDoetDTO.DepartmentId.HasValue)
                {
                    var department = await _departmentRepository.GetDepartmentByIdAsync(updateMajorForAdminDoetDTO.DepartmentId.Value);

                    if (department == null)
                    {
                        return new DataResponse<UpdateMajorForAdminDoetDTO>
                        {
                            Data = null,
                            Message = "Department not found!",
                            StatusCode = 404 // Not Found
                        };
                    }

                    if (!department.Status.Equals("Active"))
                    {
                        return new DataResponse<UpdateMajorForAdminDoetDTO>
                        {
                            Data = null,
                            Message = "Cannot update major because the department is not active.",
                            StatusCode = 400 // Bad Request
                        };
                    }
                }

                var existingMajorWithCode = await _majorRepository.GetMajorByCodeAsync(updateMajorForAdminDoetDTO.MajorCode);
                if (existingMajorWithCode != null && existingMajorWithCode.MajorId != updateMajorForAdminDoetDTO.MajorId)
                {
                    return new DataResponse<UpdateMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major code already exists!",
                        StatusCode = 400
                    };
                }

                major.MajorCode = updateMajorForAdminDoetDTO.MajorCode ?? major.MajorCode;
                major.Name = updateMajorForAdminDoetDTO.Name ?? major.Name;
                major.Description = updateMajorForAdminDoetDTO.Description ?? major.Description;
                major.DepartmentId = updateMajorForAdminDoetDTO.DepartmentId ?? major.DepartmentId;
                major.UpdatedAt = DateTime.Now;

                await _majorRepository.UpdateMajorAsync(major);

                var updatedMajorDto = _mapper.Map<UpdateMajorForAdminDoetDTO>(major);

                return new DataResponse<UpdateMajorForAdminDoetDTO>
                {
                    Data = updatedMajorDto,
                    Message = "Major updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateMajorForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating major: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateMajorStatusForAdminDoetDTO>> UpdateMajorStatusForAdminDoetAsync(UpdateMajorStatusForAdminDoetDTO updateMajorStatusForAdminDoetDTO)
        {
            try
            {
                var major = await _majorRepository.GetMajorByIdAsync(updateMajorStatusForAdminDoetDTO.MajorId);

                if (major == null)
                {
                    return new DataResponse<UpdateMajorStatusForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major not found!",
                        StatusCode = 404
                    };
                }

                major.Status = updateMajorStatusForAdminDoetDTO.Status ?? major.Status;
                major.UpdatedAt = DateTime.Now;
                await _majorRepository.UpdateMajorAsync(major);

                var updatedMajorStatusDto = _mapper.Map<UpdateMajorStatusForAdminDoetDTO>(major);

                return new DataResponse<UpdateMajorStatusForAdminDoetDTO>
                {
                    Data = updatedMajorStatusDto,
                    Message = "Major status updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateMajorStatusForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating major status: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteMajorForAdminDoetDTO>> DeleteMajorForAdminDoetAsync(DeleteMajorForAdminDoetDTO deleteMajorForAdminDoetDTO)
        {
            try
            {
                var major = await _majorRepository.GetMajorByIdAsync(deleteMajorForAdminDoetDTO.MajorId);

                if (major == null)
                {
                    return new DataResponse<DeleteMajorForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Major not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra ràng buộc
                bool hasDependencies = await _majorRepository.CheckMajorDependenciesAsync(major.MajorId);

                if (hasDependencies)
                {
                    // Đổi trạng thái thành 'Unactive' thay vì xóa
                    major.Status = "Unactive";
                    major.UpdatedAt = DateTime.Now;
                    await _majorRepository.UpdateMajorAsync(major);

                    return new DataResponse<DeleteMajorForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteMajorForAdminDoetDTO>(major),
                        Message = "Major is in use and cannot be deleted. The status has been updated to 'Unactive' to disable it while keeping associated data intact.",
                        StatusCode = 200
                    };
                }
                else
                {
                    // Xóa nếu không có ràng buộc
                    await _majorRepository.DeleteMajorAsync(major);

                    return new DataResponse<DeleteMajorForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteMajorForAdminDoetDTO>(major),
                        Message = "Major deleted successfully.",
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteMajorForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error deleting major: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<MemoryStream>> GenerateMajorTemplateForAdminDoetAsync()
        {
            try
            {
                var memoryStream = new MemoryStream();
                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Major Template");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Major Code(*)";
                    worksheet.Cells[1, 2].Value = "Major Name(*)";
                    worksheet.Cells[1, 3].Value = "Description(*)";
                    worksheet.Cells[1, 4].Value = "Department Code(*)";

                    for (int col = 1; col <= 4; col++)
                    {
                        worksheet.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, col].Style.Font.Bold = true; // Đặt chữ in đậm
                    }

                    // Example rows
                    worksheet.Cells[2, 1].Value = "SE";
                    worksheet.Cells[2, 2].Value = "Kỹ thuật phần mềm";
                    worksheet.Cells[2, 3].Value = "Chuyên ngành kỹ thuật phần mềm.";
                    worksheet.Cells[2, 4].Value = "IT001";

                    // Thêm tiêu đề "Hướng dẫn điền Role" chiếm 2 cột
                    worksheet.Cells[4, 5].Value = "Hướng dẫn điền DepartmentCode";
                    worksheet.Cells[4, 5, 4, 6].Merge = true; // Ghép ô từ F4 đến G4
                    worksheet.Cells[4, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[4, 5].Style.Font.Bold = true; // Đặt chữ in đậm

                    // Thêm tiêu đề cho bảng hướng dẫn
                    worksheet.Cells[5, 5].Value = "Department Code";
                    worksheet.Cells[5, 6].Value = "Department Name";

                    // Định dạng tiêu đề Role ID và Role Name
                    worksheet.Cells[5, 5, 5, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[5, 5, 5, 6].Style.Font.Bold = true; // Đặt chữ in đậm

                    var departments = await _departmentRepository.GetAllDepartmentForCommonAsync();

                    // Thêm dữ liệu từ DB vào bảng hướng dẫn
                    int startRow = 6;
                    foreach (var department in departments)
                    {
                        worksheet.Cells[startRow, 5].Value = department.DepartmentCode;
                        worksheet.Cells[startRow, 6].Value = department.Name;
                        startRow++;
                    }

                    // Định dạng bảng hướng dẫn
                    worksheet.Cells[5, 5, startRow - 1, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksheet.Cells[5, 5, startRow - 1, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    // Căn giữa cho tất cả các ô
                    for (int row = 2; row <= 1000; row++)
                    {
                        for (int col = 1; col <= 4; col++)
                        {
                            worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }

                    // Thêm ghi chú dưới bảng hướng dẫn
                    worksheet.Cells[startRow + 1, 5].Value = "Ghi chú:";
                    worksheet.Cells[startRow + 2, 5].Value = "(*) : Bắt buộc điền.";
                    worksheet.Cells[startRow + 3, 5].Value = "Major Code: Không được quá 50 ký tự.";
                    worksheet.Cells[startRow + 4, 5].Value = "Major Name: Không được quá 255 ký tự.";
                    worksheet.Cells[startRow + 5, 5].Value = "Department Code: Không được quá 50 ký tự.";
                    worksheet.Cells[startRow + 6, 5].Value = "Hãy xóa dữ liệu mẫu trước khi điền tránh trùng lặp.";

                    for (int row = startRow + 1; row <= startRow + 6; row++)
                    {
                        worksheet.Cells[row, 5].Style.Font.Bold = true;
                        worksheet.Cells[row, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }

                    // Tự động kéo dãn cột
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 50;
                    worksheet.Column(4).Width = 30;
                    worksheet.Column(5).Width = 30;
                    worksheet.Column(6).Width = 30;

                    // Lưu file vào MemoryStream
                    package.Save();
                }

                memoryStream.Position = 0;
                return new DataResponse<MemoryStream>
                {
                    Data = memoryStream,
                    Message = "Template generated successfully.",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<MemoryStream>
                {
                    Data = null,
                    Message = $"Error generating template: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<object>> ImportMajorsForAdminDoetAsync(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (file == null || file.Length == 0)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = "File is empty or not provided.",
                    StatusCode = 400
                };
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        var majors = new List<Major>();
                        var errorMessages = new List<string>();

                        // Xác định số hàng cuối cùng trong phạm vi từ cột A đến cột E
                        int lastRow = 1;
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            bool hasDataInRange = false;
                            for (int col = 1; col <= 4; col++) // Chỉ từ cột A đến E (1 đến 6)
                            {
                                if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col]?.Text))
                                {
                                    hasDataInRange = true;
                                    break;
                                }
                            }
                            if (hasDataInRange)
                            {
                                lastRow = row;
                            }
                        }

                        // Tạo dictionary để lưu mối quan hệ giữa email và majorId
                        var majorMapping = new Dictionary<string, int?>(); // Lưu email và MajorId tương ứng

                        for (int row = 2; row <= lastRow; row++)
                        {
                            var code = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var name = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            var description = worksheet.Cells[row, 3].Value?.ToString().Trim();
                            var departmentCode = worksheet.Cells[row, 4].Value?.ToString().Trim();

                            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(departmentCode))
                            {
                                errorMessages.Add($"Row {row}: Missing required data.");
                                continue;
                            }

                            // Kiểm tra độ dài của UserCode
                            if (code.Length > 50)
                            {
                                errorMessages.Add($"Row {row}: MajorCode must not exceed 50 characters.");
                                continue;
                            }

                            if (name.Length > 255)
                            {
                                errorMessages.Add($"Row {row}: MajorName must not exceed 255 characters.");
                                continue;
                            }

                            if (departmentCode.Length > 50)
                            {
                                errorMessages.Add($"Row {row}: DepartmentCode must not exceed 50 characters.");
                                continue;
                            }

                            int? departmentId = null;

                            var department = await _departmentRepository.GetDepartmentByCodeAsync(departmentCode);
                            if (department == null)
                            {
                                errorMessages.Add($"Row {row}: Department code '{departmentCode}' does not exist.");
                                continue;
                            }

                            // Kiểm tra trạng thái Active của Major
                            if (department.Status.ToLower() != "active")
                            {
                                errorMessages.Add($"Row {row}: Department code '{departmentCode}' is not active.");
                                continue;
                            }

                            departmentId = department.DepartmentId;

                            majorMapping[code] = departmentId;

                            // Kiểm tra nếu UserCode hoặc Email đã tồn tại
                            var isMajorCodeExists = await _majorRepository.GetMajorByCodeAsync(code);

                            if (isMajorCodeExists != null)
                            {
                                errorMessages.Add($"Row {row}: MajorCode '{code}' already exists.");
                                continue;
                            }

                            var major = new Major
                            {
                                MajorCode = code,
                                Name = name,
                                Description = description,
                                DepartmentId = departmentId,
                                Status = "Active",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            majors.Add(major);
                        }

                        // Nếu có bất kỳ lỗi nào, trả về lỗi và không thêm vào DB
                        if (errorMessages.Any())
                        {
                            return new DataResponse<object>
                            {
                                Data = new
                                {
                                    SuccessCount = 0,
                                    ErrorCount = errorMessages.Count,
                                    Errors = errorMessages
                                },
                                Message = $"Import failed. There were {errorMessages.Count} errors. Please fix the reported errors to successfully add the file.",
                                StatusCode = 400
                            };
                        }


                        await _majorRepository.AddMajorsAsync(majors);

                        var successCount = majors.Count;

                        var resultMessage = $"Import completed. Successfully added {successCount} majors.";

                        return new DataResponse<object>
                        {
                            Data = new
                            {
                                SuccessCount = successCount,
                                ErrorCount = 0,
                                Errors = errorMessages
                            },
                            Message = resultMessage,
                            StatusCode = 200
                        };
                    }
                }
            }
            catch (UnauthorizedAccessException authEx)
            {
                // Xử lý lỗi quyền truy cập
                return new DataResponse<object>
                {
                    Data = null,
                    Message = $"Access denied while importing majors: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = $"Error importing majors: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<List<StatusMajorListForAdminDoetDTO>>> GetAllStatusesMajorForAdminDoetAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusMajorListForAdminDoetDTO>
                {
                    new StatusMajorListForAdminDoetDTO { Status = "Active" },
                    new StatusMajorListForAdminDoetDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusMajorListForAdminDoetDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusMajorListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusMajorListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Common

        public async Task<DataResponse<List<MajorListForCommonDTO>>> GetAllMajorForCommonAsync()
        {
            try
            {
                var majors = await _majorRepository.GetAllMajorForCommonAsync();

                var majorsDtos = _mapper.Map<List<MajorListForCommonDTO>>(majors);

                return new DataResponse<List<MajorListForCommonDTO>>
                {
                    Data = majorsDtos,
                    Message = "Major list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<MajorListForCommonDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<MajorListForCommonDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving major list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        // Student 
        public async Task<DataResponse<List<MajorListForStudentDTO>>> GetAllMajorsAsync()
        {
            try
            {
                var majors = await _majorRepository.GetAllMajorsAsync();
                var response = _mapper.Map<List<MajorListForStudentDTO>>(majors);

                return new DataResponse<List<MajorListForStudentDTO>>
                {
                    StatusCode = 200,
                    Message = "Major list retrieved successfully!",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<MajorListForStudentDTO>>
                {
                    StatusCode = 500,
                    Message = $"Error retrieving major list {ex.Message}. ",
                    Data = null
                };
            }
        }
    }
}
