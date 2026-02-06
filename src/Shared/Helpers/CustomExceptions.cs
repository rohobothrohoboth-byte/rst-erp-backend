using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Helpers;

public interface IExceptionHandler
{
    bool CanHandle(Exception ex);
    Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context);
}

public class UnauthorizedExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is UnauthorizedException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var uEx = (UnauthorizedException)ex;
        var response = ApiResponse<string>.Fail(uEx.Message, new List<string> { uEx.Message }, 401);
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}

public class ForbiddenExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is ForbiddenException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var fEx = (ForbiddenException)ex;
        var response = ApiResponse<string>.Fail(fEx.Message, new List<string> { fEx.Message }, 403);
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}

public class ConflictExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is ConflictException || ex is DbUpdateConcurrencyException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var message = ex is ConflictException cEx ? cEx.Message : "The resource was modified by another user. Please reload and try again.";
        var response = ApiResponse<string>.Fail(message, new List<string> { ex.Message }, 409);
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}

public class DomainExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is DomainException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var domainEx = (DomainException)ex;
        var errors = domainEx is ValException valEx ? valEx.Errors.ToList() : new List<string> { domainEx.Message };
        var response = new ApiResponse<string>(domainEx.Message, errors, domainEx.StatusCode)
        {
            TraceId = context.TraceIdentifier
        };
        return Task.FromResult(response);
    }
}

public class NotFoundExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is KeyNotFoundException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var response = ApiResponse<string>.Fail("Resource not found.", new List<string> { ex.Message }, 404);
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}

public class BadRequestExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is ArgumentException || ex is InvalidOperationException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var response = ApiResponse<string>.Fail(ex.Message, new List<string> { ex.Message });
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}

public class DatabaseExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is PostgresException || ex is NpgsqlException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var message = ex is PostgresException pgEx ? $"Database error: {pgEx.SqlState}" : "Database connection or execution error.";
        var errors = new List<string> { ex.Message };
        var response = ApiResponse<string>.Fail(message, errors);
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}

public class FallbackExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => true;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var response = ApiResponse<string>.Fail("An unexpected error occurred.", new List<string> { ex.Message }, 500);
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}