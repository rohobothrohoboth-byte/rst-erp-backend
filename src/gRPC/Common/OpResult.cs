// Common/OpResult.cs
namespace Common;

public class OpResult
{
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public string? Message { get; set; }  // Added for backward compatibility
    public string? Error { get; set; }    // Added for backward compatibility

    public static OpResult Ok() => new() { IsSuccess = true };

    public static OpResult Success(string message = "")
    {
        return new OpResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static OpResult Fail(string error)
    {
        return new OpResult
        {
            IsSuccess = false,
            Errors = new List<string> { error },
            Error = error
        };
    }

    public static OpResult Fail(IEnumerable<string> errors)
    {
        return new OpResult
        {
            IsSuccess = false,
            Errors = errors.ToList(),
            Error = errors.FirstOrDefault()
        };
    }

    // Add implicit conversion for string errors
    public static implicit operator OpResult(string error) => Fail(error);

    // Add extension method to add errors
    public OpResult AddError(string error)
    {
        Errors.Add(error);
        IsSuccess = false;
        return this;
    }

    public OpResult AddErrors(IEnumerable<string> errors)
    {
        Errors.AddRange(errors);
        IsSuccess = false;
        return this;
    }
}