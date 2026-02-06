using FluentValidation.Results;

namespace Helpers;

public class DomainException : Exception
{
    public int StatusCode { get; }
    public DomainException(string message, int statusCode = 400) : base(message) { StatusCode = statusCode; }
}

public class ValException : DomainException
{
    public List<string> Errors { get; }
    public ValException(IEnumerable<ValidationFailure> failures) : base("Validation failed.")
    {
        Errors = failures
            .Where(f => f != null)
            //.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")
            .Select(f => $"{f.ErrorMessage}")
            .Distinct()
            .ToList();
    }

    public ValException(IEnumerable<string> errors) : base("Validation failed.")
    {
        Errors = errors.ToList();
    }

    public ValException(string error) : this([error]) { }
}

public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message, 409) { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Unauthorized request.") : base(message, 401) { }
}

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "Forbidden request.") : base(message, 403) { }
}