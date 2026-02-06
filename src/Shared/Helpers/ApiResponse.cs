namespace Helpers;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int? StatusCode { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ApiResponse() { }

    public ApiResponse(T? data, string? message = null, int? statusCode = 200)
    {
        Success = true;
        Message = message ?? "Request successful.";
        Data = data;
        StatusCode = statusCode;
    }

    public ApiResponse(string? message, List<string>? errors = null, int? statusCode = 400)
    {
        Success = false;
        Message = message;
        Errors = errors;
        StatusCode = statusCode;
    }

    public static ApiResponse<T> Ok(T? data, string? message = null) => new(data, message);
    public static ApiResponse<T> Fail(string? message, List<string>? errors = null, int? statusCode = 400) => new(message, errors, statusCode);
}