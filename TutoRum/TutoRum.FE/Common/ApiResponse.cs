namespace TutoRum.FE.Common
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public bool Success { get; set; }

        public string Message { get; set; }
        public T Data { get; set; }
        public List<ApiError> Errors { get; set; }
        public string Timestamp { get; set; }

        public ApiResponse(int status, bool success, string message, T data = default, List<ApiError> errors = null)
        {
            Status = status;
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<ApiError>();
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }

    public class ApiError
    {
        public string Code { get; set; }
        public string Detail { get; set; }

        public ApiError(string code, string detail)
        {
            Code = code;
            Detail = detail;
        }
    }
}
