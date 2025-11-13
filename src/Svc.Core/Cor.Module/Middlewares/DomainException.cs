namespace Cor.Module.Middlewares;

// Base domain exception
public abstract class DomainException : Exception
{
    public int StatusCode { get; }

    protected DomainException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}

// Resource not found
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object key) : base($"{entityName} with Id {key} was not found.", 404) { }
    public EntityNotFoundException(string entityName) : base($"{entityName} was not found.", 404) { }
}

// Validation errors
public class ValidationException : DomainException
{
    public List<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors) : base("Validation failed.", 400)
    {
        Errors = errors.ToList();
    }
}

// Concurrency or conflict
public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message, 409) { }
}

// Unauthorized / forbidden
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Unauthorized request.") : base(message, 401) { }
}

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "Forbidden request.") : base(message, 403) { }
}
