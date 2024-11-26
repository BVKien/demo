namespace OJTEDU.Api.Configuration
{
    public class ApiResponse<T>
    {
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class ApiResponseTotalPaged<T>
    {
        public string? Message { get; set; }
        public int? TotalPageCount { get; set; }
        public T? Data { get; set; }
    }
}
