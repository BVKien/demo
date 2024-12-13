using AutoMapper;
using Microsoft.AspNetCore.Http;
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
using static OJTEDU.Application.DTOs.DepartmentDTO;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        // Admin - Doet
        public async Task<DataResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>> GetAllDepartmentForAdminDoetAsync(string? departmentCode, string? departmentName, string? status, int pageNumber, int pageSize)
        {
            try
            {
                var departments = await _departmentRepository.GetAllDepartmentForAdminDoetAsync(departmentCode, departmentName, status);

                var totalDepartments = departments.Count();
                var totalPages = totalDepartments == 0 ? 1 : (int)Math.Ceiling((double)totalDepartments / pageSize);

                var departmentDtos = totalDepartments > 0 ? _mapper.Map<List<DepartmentListForAdminDoetDTO>>(departments)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList()
                                           : new List<DepartmentListForAdminDoetDTO>();

                var pagedResponse = new PagedResponse<List<DepartmentListForAdminDoetDTO>>
                {
                    Items = departmentDtos,
                    TotalCount = totalDepartments,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = totalPages
                };

                return new DataResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>
                {
                    Data = pagedResponse,
                    Message = "Department list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResponse<List<DepartmentListForAdminDoetDTO>>>
                {
                    Data = null,
                    Message = $"Error retrieving department list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DepartmentDetailForAdminDoetDTO>> GetDepartmentDetailByIdForAdminDoetAsync(int departmentId)
        {
            try
            {
                var department = await _departmentRepository.GetDepartmentByIdAsync(departmentId);

                if (department == null)
                {
                    return new DataResponse<DepartmentDetailForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404
                    };
                }

                var departmentDto = _mapper.Map<DepartmentDetailForAdminDoetDTO>(department);

                return new DataResponse<DepartmentDetailForAdminDoetDTO>
                {
                    Data = departmentDto,
                    Message = "Company details retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<DepartmentDetailForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error retrieving department details: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<AddDepartmentForAdminDoetDTO>> AddDepartmentForAdminDoetAsync(AddDepartmentForAdminDoetDTO addDepartmentForAdminDoetDTO)
        {
            try
            {
                // Kiểm tra xem departmentCode đã tồn tại hay chưa
                var existingDepartment = await _departmentRepository.GetDepartmentByCodeAsync(addDepartmentForAdminDoetDTO.DepartmentCode);

                if (existingDepartment != null)
                {
                    return new DataResponse<AddDepartmentForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department code already exists!",
                        StatusCode = 400 // Bad Request
                    };
                }

                // Thêm mới department
                var department = _mapper.Map<Department>(addDepartmentForAdminDoetDTO);
                department.CreatedAt = DateTime.Now;
                department.UpdatedAt = DateTime.Now;
                department.Status = "Active";
                await _departmentRepository.AddDepartmentAsync(department);

                var addedDepartmentDto = _mapper.Map<AddDepartmentForAdminDoetDTO>(department);

                return new DataResponse<AddDepartmentForAdminDoetDTO>
                {
                    Data = addedDepartmentDto,
                    Message = "Department added successfully!",
                    StatusCode = 201 // Created
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<AddDepartmentForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error adding department: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateDepartmentForAdminDoetDTO>> UpdateDepartmentForAdminDoetAsync(UpdateDepartmentForAdminDoetDTO updateDepartmentForAdminDoetDTO)
        {
            try
            {
                // Tìm Department theo Id
                var department = await _departmentRepository.GetDepartmentByIdAsync(updateDepartmentForAdminDoetDTO.DepartmentId);

                if (department == null)
                {
                    return new DataResponse<UpdateDepartmentForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra trùng mã departmentCode với Department khác
                var existingDepartmentWithCode = await _departmentRepository.GetDepartmentByCodeAsync(updateDepartmentForAdminDoetDTO.DepartmentCode);
                if (existingDepartmentWithCode != null && existingDepartmentWithCode.DepartmentId != updateDepartmentForAdminDoetDTO.DepartmentId)
                {
                    return new DataResponse<UpdateDepartmentForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department code already exists!",
                        StatusCode = 400
                    };
                }

                // Cập nhật thông tin Department
                department.DepartmentCode = updateDepartmentForAdminDoetDTO.DepartmentCode ?? department.DepartmentCode;
                department.Name = updateDepartmentForAdminDoetDTO.Name ?? department.Name;
                department.Detail = updateDepartmentForAdminDoetDTO.Detail ?? department.Detail;
                department.UpdatedAt = DateTime.Now;

                await _departmentRepository.UpdateDepartmentAsync(department);

                var updatedDepartmentDto = _mapper.Map<UpdateDepartmentForAdminDoetDTO>(department);

                return new DataResponse<UpdateDepartmentForAdminDoetDTO>
                {
                    Data = updatedDepartmentDto,
                    Message = "Department updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDepartmentForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating department: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<UpdateDepartmentStatusForAdminDoetDTO>> UpdateDepartmentStatusForAdminDoetAsync(UpdateDepartmentStatusForAdminDoetDTO updateDepartmentStatusForAdminDoetDTO)
        {
            try
            {
                // Tìm Department theo Id
                var department = await _departmentRepository.GetDepartmentByIdAsync(updateDepartmentStatusForAdminDoetDTO.DepartmentId);

                if (department == null)
                {
                    return new DataResponse<UpdateDepartmentStatusForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404
                    };
                }

                // Cập nhật trạng thái của Department
                department.Status = updateDepartmentStatusForAdminDoetDTO.Status ?? department.Status;
                department.UpdatedAt = DateTime.Now;
                await _departmentRepository.UpdateDepartmentAsync(department);

                var updatedDepartmentStatusDto = _mapper.Map<UpdateDepartmentStatusForAdminDoetDTO>(department);

                return new DataResponse<UpdateDepartmentStatusForAdminDoetDTO>
                {
                    Data = updatedDepartmentStatusDto,
                    Message = "Department status updated successfully!",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<UpdateDepartmentStatusForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error updating department status: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<DeleteDepartmentForAdminDoetDTO>> DeleteDepartmentForAdminDoetAsync(DeleteDepartmentForAdminDoetDTO deleteDepartmentForAdminDoetDTO)
        {
            try
            {
                var department = await _departmentRepository.GetDepartmentByIdAsync(deleteDepartmentForAdminDoetDTO.DepartmentId);

                if (department == null)
                {
                    return new DataResponse<DeleteDepartmentForAdminDoetDTO>
                    {
                        Data = null,
                        Message = "Department not found!",
                        StatusCode = 404
                    };
                }

                // Kiểm tra ràng buộc
                bool hasDependencies = await _departmentRepository.CheckDepartmentDependenciesAsync(department.DepartmentId);

                if (hasDependencies)
                {
                    // Đổi trạng thái thành 'Unactive' thay vì xóa
                    department.Status = "Unactive";
                    department.UpdatedAt = DateTime.Now;
                    await _departmentRepository.UpdateDepartmentAsync(department);

                    return new DataResponse<DeleteDepartmentForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteDepartmentForAdminDoetDTO>(department),
                        Message = "Department is in use and cannot be deleted. The status has been updated to 'Unactive' to disable it while keeping associated data intact.",
                        StatusCode = 200
                    };
                }
                else
                {
                    // Xóa nếu không có ràng buộc
                    await _departmentRepository.DeleteDepartmentAsync(department);

                    return new DataResponse<DeleteDepartmentForAdminDoetDTO>
                    {
                        Data = _mapper.Map<DeleteDepartmentForAdminDoetDTO>(department),
                        Message = "Department deleted successfully.",
                        StatusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataResponse<DeleteDepartmentForAdminDoetDTO>
                {
                    Data = null,
                    Message = $"Error deleting department: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<MemoryStream>> GenerateDepartmentTemplateForAdminDoetAsync()
        {
            try
            {
                var memoryStream = new MemoryStream();
                using (var package = new ExcelPackage(memoryStream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Department Template");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Department Code(*)";
                    worksheet.Cells[1, 2].Value = "Department Name(*)";
                    worksheet.Cells[1, 3].Value = "Department Detail(*)";

                    for (int col = 1; col <= 3; col++)
                    {
                        worksheet.Cells[1, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, col].Style.Font.Bold = true; // Đặt chữ in đậm
                    }

                    // Example rows
                    worksheet.Cells[2, 1].Value = "IT001";
                    worksheet.Cells[2, 2].Value = "Công nghệ thông tin";
                    worksheet.Cells[2, 3].Value = "Phòng ban quản lý hạ tầng IT và phát triển phần mềm.";

                    // Căn giữa cho tất cả các ô
                    for (int row = 2; row <= 1000; row++)
                    {
                        for (int col = 1; col <= 3; col++)
                        {
                            worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }

                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 50;

                    // Thêm ghi chú cho các trường
                    worksheet.Cells[1, 5].Value = "Ghi chú:"; // Cột 6
                    worksheet.Cells[2, 5].Value = "(*) : Bắt buộc điền."; // Cột 6
                    worksheet.Cells[3, 5].Value = "Department Code : Không được quá 50 ký tự."; // Cột 6
                    worksheet.Cells[4, 5].Value = "Department Name : Không được quá 255 ký tự.."; // Cột 6
                    worksheet.Cells[5, 5].Value = "Hãy xóa dữ liệu mẫu trước khi điền tránh trùng lặp."; // Cột 6

                    for (int row = 1; row <= 5; row++)
                    {
                        worksheet.Cells[row, 5].Style.Font.Bold = true;
                        worksheet.Cells[row, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    }

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

        public async Task<DataResponse<object>> ImportDepartmentsForAdminDoetAsync(IFormFile file)
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
                        var departments = new List<Department>();
                        var errorMessages = new List<string>();

                        // Xác định số hàng cuối cùng trong phạm vi từ cột A đến cột E
                        int lastRow = 1;
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            bool hasDataInRange = false;
                            for (int col = 1; col <= 3; col++) // Chỉ từ cột A đến E (1 đến 6)
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

                        for (int row = 2; row <= lastRow; row++)
                        {
                            var code = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var name = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            var detail = worksheet.Cells[row, 3].Value?.ToString().Trim();

                            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(detail))
                            {
                                errorMessages.Add($"Row {row}: Missing required data.");
                                continue;
                            }

                            // Kiểm tra độ dài của UserCode
                            if (code.Length > 50)
                            {
                                errorMessages.Add($"Row {row}: DepartmentCode must not exceed 50 characters.");
                                continue;
                            }

                            if (name.Length > 255)
                            {
                                errorMessages.Add($"Row {row}: DepartmentName must not exceed 255 characters.");
                                continue;
                            }

                            var isDepartmentCodeExists = await _departmentRepository.GetDepartmentByCodeAsync(code);

                            if (isDepartmentCodeExists != null)
                            {
                                errorMessages.Add($"Row {row}: DepartmentCode '{code}' already exists.");
                                continue;
                            }

                            var department = new Department
                            {
                                DepartmentCode = code,
                                Name = name,
                                Detail = detail,
                                Status = "Active",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            departments.Add(department);
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


                        await _departmentRepository.AddDepartmentsAsync(departments);

                        var successCount = departments.Count;

                        var resultMessage = $"Import completed. Successfully added {successCount} departments.";

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
                    Message = $"Access denied while importing deparments: {authEx.Message}",
                    StatusCode = 403 // Forbidden
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<object>
                {
                    Data = null,
                    Message = $"Error importing deparments: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<DataResponse<List<StatusDepartmentListForAdminDoetDTO>>> GetAllStatusesDepartmentForAdminDoetAsync()
        {
            try
            {
                // Tạo danh sách trạng thái
                var statuses = new List<StatusDepartmentListForAdminDoetDTO>
                {
                    new StatusDepartmentListForAdminDoetDTO { Status = "Active" },
                    new StatusDepartmentListForAdminDoetDTO { Status = "Inactive" }
                };

                return new DataResponse<List<StatusDepartmentListForAdminDoetDTO>>
                {
                    Data = statuses,
                    Message = "Status List retrieved successfully!",
                    StatusCode = 200 // Có thể tùy chỉnh theo nhu cầu
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<StatusDepartmentListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                return new DataResponse<List<StatusDepartmentListForAdminDoetDTO>>
                {
                    Data = null,
                    Message = $"Error occurred while retrieving statuses: {ex.Message}",
                    StatusCode = 500 // Lỗi server
                };
            }
        }

        // Common
        public async Task<DataResponse<List<DepartmentListForCommonDTO>>> GetAllDepartmentForCommonAsync()
        {
            try
            {
                var departments = await _departmentRepository.GetAllDepartmentForCommonAsync();

                var departmentDtos = _mapper.Map<List<DepartmentListForCommonDTO>>(departments);

                return new DataResponse<List<DepartmentListForCommonDTO>>
                {
                    Data = departmentDtos,
                    Message = "Department list retrieved successfully!",
                    StatusCode = 200
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new DataResponse<List<DepartmentListForCommonDTO>>
                {
                    Data = null,
                    Message = ex.Message,
                    StatusCode = 404
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<DepartmentListForCommonDTO>>
                {
                    Data = null,
                    Message = $"Error retrieving department list: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
