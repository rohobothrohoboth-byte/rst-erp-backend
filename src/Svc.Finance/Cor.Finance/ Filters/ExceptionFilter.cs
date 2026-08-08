// Filters/ExceptionFilter.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Helpers;
using System;
using Cor.Finance.Validators;
namespace Cor.Finance.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception: {Message}", context.Exception.Message);

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "An unexpected error occurred"
        };

        var statusCode = context.Exception switch
        {
            InvalidOperationException ex when ex.Message.Contains("not found") => 404,
            InvalidOperationException ex when ex.Message.Contains("already") => 400,
            NotFoundException => 404,
            DomainException => 400,
            ValidationException => 400,
            _ => 500
        };

        context.Result = new ObjectResult(response) { StatusCode = statusCode };
        context.ExceptionHandled = true;
    }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class ValidationException : Exception
    {
        public ValidationException() : base() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }