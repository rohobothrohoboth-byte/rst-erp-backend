namespace Svc.Lup.Helpers;

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
    
    public ApiResponse(string? message, List<string>? errors = null, int? statusCode = 400)
    {
        Success = false;
        Message = message;
        Errors = errors;
        StatusCode = statusCode;
    }

    public static ApiResponse<T> Fail(string? message, List<string>? errors = null, int? statusCode = 400) => new(message, errors, statusCode);
}