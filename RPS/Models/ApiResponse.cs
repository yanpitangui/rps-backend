namespace RPS.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }
        public T Data { get; set; } = default(T);

        public ApiResponse(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }

    public class ApiResponse
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }
        public ApiResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
