using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers;

#region Exception Types

public static class ExcCodes
{
    public const string Validation = "VALIDATION_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string AlreadyExists = "ALREADY_EXISTS";
    public const string Business = "BUSINESS_RULE";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string Concurrency = "CONCURRENCY";
    public const string Internal = "INTERNAL_SERVER_ERROR";
}

public abstract class DomainExc(string code, string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string Code { get; } = code;
}

public sealed class ValidationError
{
    public string Error { get; init; } = string.Empty;
}

public sealed class ValException : DomainExc
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValException(IEnumerable<ValidationFailure> failures)
        : base(ExcCodes.Validation, "One or more validation errors occurred.", StatusCodes.Status400BadRequest)
    {
        Errors = [.. failures.GroupBy(x => new { x.ErrorMessage }).Select(x => new ValidationError
        {
            Error = x.Key.ErrorMessage.Replace("'", string.Empty)
        })];
    }

    public ValException(IEnumerable<string> errors)
        : base(ExcCodes.Validation, "One or more validation errors occurred.", StatusCodes.Status400BadRequest)
    {
        Errors = [.. errors.Distinct().Select(x => new ValidationError
        {
            Error = x
        })];
    }

    public ValException(string error) : this([error]) { }
}

// Made non-sealed for compatibility
public class BusinessExc(string message)
    : DomainExc(ExcCodes.Business, message, StatusCodes.Status422UnprocessableEntity) { }

public class ConflictExc(string message)
    : DomainExc(ExcCodes.Conflict, message, StatusCodes.Status409Conflict) { }

public sealed class AlreadyExistsExc(string message) : ConflictExc(message) { }

public class NotFoundExc(string message)
    : DomainExc(ExcCodes.NotFound, message, StatusCodes.Status404NotFound) { }

public class UnauthorizedExc(string message = "Unauthorized.")
    : DomainExc(ExcCodes.Unauthorized, message, StatusCodes.Status401Unauthorized) { }

public class ForbiddenExc(string message = "Forbidden.")
    : DomainExc(ExcCodes.Forbidden, message, StatusCodes.Status403Forbidden) { }

public sealed class ConcurrencyExc(string message = "The record was modified by another user.")
    : DomainExc(ExcCodes.Concurrency, message, StatusCodes.Status409Conflict) { }

// Legacy compatibility wrappers (now work because base classes are non-sealed)
public class DomainException : DomainExc
{
    public DomainException(string message)
        : base(ExcCodes.Business, message, StatusCodes.Status422UnprocessableEntity)
    {
    }

    public DomainException(string message, int statusCode)
        : base(ExcCodes.Business, message, statusCode)
    {
    }
}

public class UnauthorizedException : UnauthorizedExc
{
    public UnauthorizedException(string message = "Unauthorized.") : base(message) { }
}

public class ForbiddenException : ForbiddenExc
{
    public ForbiddenException(string message = "Forbidden.") : base(message) { }
}

public class ConflictException : ConflictExc
{
    public ConflictException(string message) : base(message) { }
}

public class NotFoundException : NotFoundExc
{
    public NotFoundException(string message) : base(message) { }
}

#endregion

#region Database Error Handling

public static class DatabaseErrorCodes
{
    public const string UniqueViolation = "23505";
    public const string ForeignKeyViolation = "23503";
    public const string NotNullViolation = "23502";
    public const string CheckViolation = "23514";
    public const string SerializationFailure = "40001";
    public const string DeadlockDetected = "40P01";
}

public interface IDbExceptionTranslator
{
    Exception Translate(Exception exception);
}

public sealed class DbExceptionTranslator : IDbExceptionTranslator
{
    public Exception Translate(Exception exception)
    {
        if (exception is DbUpdateConcurrencyException)
        {
            return new ConcurrencyExc();
        }

        if (exception is DbUpdateException db && db.InnerException is PostgresException pg)
        {
            return TranslatePostgres(pg);
        }

        if (exception is PostgresException postgres)
        {
            return TranslatePostgres(postgres);
        }

        return exception;
    }

    private static Exception TranslatePostgres(PostgresException ex)
    {
        return ex.SqlState switch
        {
            DatabaseErrorCodes.UniqueViolation => new AlreadyExistsExc(BuildUniqueMessage(ex)),
            DatabaseErrorCodes.ForeignKeyViolation => new ConflictExc(BuildForeignKeyMessage(ex)),
            DatabaseErrorCodes.CheckViolation => new BusinessExc(BuildCheckMessage(ex)),
            DatabaseErrorCodes.NotNullViolation => new BusinessExc(BuildNullMessage(ex)),
            DatabaseErrorCodes.SerializationFailure => new ConflictExc("The operation could not be completed because another transaction modified the same data."),
            DatabaseErrorCodes.DeadlockDetected => new ConflictExc("The operation was cancelled because of a database deadlock."),
            _ => ex
        };
    }

    private static string BuildUniqueMessage(PostgresException ex)
        => $"Duplicate value violates unique constraint '{ex.ConstraintName}'.";
    private static string BuildForeignKeyMessage(PostgresException ex)
        => $"Operation violates foreign key constraint '{ex.ConstraintName}'.";
    private static string BuildCheckMessage(PostgresException ex)
        => $"Check constraint '{ex.ConstraintName}' was violated.";
    private static string BuildNullMessage(PostgresException ex)
        => $"A required value is missing.";
}

public static class DbExceptionExtensions
{
    public static Exception Translate(this Exception exception, IDbExceptionTranslator translator)
    {
        return translator.Translate(exception);
    }
}

#endregion

#region Exception Handlers

public interface IExceptionHandler
{
    bool CanHandle(Exception ex);
    Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context);
}

public class UnauthorizedExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is UnauthorizedExc || ex is UnauthorizedException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var uEx = (UnauthorizedExc)ex;
        var response = ApiResponse<string>.Fail(uEx.Message, new List<string> { uEx.Message }, 401);
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}

public class ForbiddenExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is ForbiddenExc || ex is ForbiddenException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var fEx = (ForbiddenExc)ex;
        var response = ApiResponse<string>.Fail(fEx.Message, new List<string> { fEx.Message }, 403);
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}

public class ConflictExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is ConflictExc || ex is ConflictException || ex is DbUpdateConcurrencyException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var message = ex is ConflictExc cEx ? cEx.Message :
                      ex is ConflictException confEx ? confEx.Message :
                      "The resource was modified by another user. Please reload and try again.";
        var response = ApiResponse<string>.Fail(message, new List<string> { ex.Message }, 409);
        response.TraceId = context.TraceIdentifier;
        return Task.FromResult(response);
    }
}

public class DomainExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is DomainExc || ex is DomainException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var domainEx = (DomainExc)ex;
        List<string> errors;

        if (domainEx is ValException valEx)
        {
            errors = valEx.Errors.Select(e => e.Error).ToList();
        }
        else
        {
            errors = new List<string> { domainEx.Message };
        }

        var response = new ApiResponse<string>(domainEx.Message, errors, domainEx.StatusCode)
        {
            TraceId = context.TraceIdentifier
        };
        return Task.FromResult(response);
    }
}

public class NotFoundExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is NotFoundExc || ex is NotFoundException || ex is KeyNotFoundException;

    public Task<ApiResponse<string>> HandleAsync(Exception ex, HttpContext context)
    {
        var message = ex is NotFoundExc ? ex.Message :
                      ex is NotFoundException ? ex.Message :
                      "Resource not found.";
        var response = ApiResponse<string>.Fail(message, new List<string> { ex.Message }, 404);
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
        var response = ApiResponse<string>.Fail(message, errors, 500);
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

#endregion