using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Application.DTOs
{
    public class DataResponse<T>
    {
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    public class PagedResult<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public int? TotalPages { get; set; }
        public T? Data { get; set; }
    }
}
