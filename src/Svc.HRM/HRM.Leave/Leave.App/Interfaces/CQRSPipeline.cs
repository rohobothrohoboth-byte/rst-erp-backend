using FluentValidation;
using Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Leave.App.Interfaces;

public interface ICommand : IRequest<Unit> { }
public interface ICommand<TResult> : IRequest<TResult> { }
public interface IQuery<TResult> : IRequest<TResult> { }

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit> where TCommand : ICommand
{
    async Task<Unit> IRequestHandler<TCommand, Unit>.Handle(TCommand request, CancellationToken cancellationToken)
    {
        await Execute(request, cancellationToken);
        return Unit.Value;
    }

    Task Execute(TCommand request, CancellationToken cancellationToken);
}
public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult> { }
public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult> { }



public sealed class ValBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any()) { return await next(); }

        var validationTasks = _validators.Select(v => v.ValidateAsync(request, ct));
        var results = await Task.WhenAll(validationTasks);
        var failures = results.SelectMany(x => x.Errors).Where(x => x != null).ToList();

        if (failures.Count != 0) { throw new ValException(failures); }
        return await next();
    }
}

public sealed class TransactionBehavior<TRequest, TResponse>(IUnitOfWork uow) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse>
{
    private readonly IUnitOfWork _uow = uow;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is not ICommand && request is not ICommand<TResponse>)
        {
            return await next();
        }

        TResponse response = default!;
        await _uow.ExecuteAsync(async token =>
        {
            response = await next();

        }, cancellationToken: ct);

        return response;
    }
}

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse>
{
    private static readonly string RequestName = typeof(TRequest).Name;

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {

        LogStart(RequestName);
        try
        {
            var response = await next();
            LogEnd(RequestName);
            return response;
        }
        catch (Exception ex)
        {
            LogFailure(RequestName, ex);
            throw;
        }
    }

    private void LogStart(string name)
    {
        _logger.LogInformation("Handling {Request}", name);
    }

    private void LogEnd(string name)
    {
        _logger.LogInformation("Handled {Request}", name);
    }

    private void LogFailure(string name, Exception ex)
    {
        _logger.LogError(ex, "Failed {Request}", name);
    }
}

public sealed class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private static readonly string RequestName = typeof(TRequest).Name;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger = logger;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await next();
        }
        finally
        {
            sw.Stop();
            if (sw.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning("{Request} executed in {Elapsed} ms", RequestName, sw.ElapsedMilliseconds);
            }
        }
    }
}

public sealed class ExceptionBehavior<TRequest, TResponse>(ILogger<ExceptionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse>
{

    private static readonly string RequestName = typeof(TRequest).Name;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (ValException ex)
        {
            logger.LogWarning(ex, "Validation failure {Request}", RequestName);
            throw;
        }
        catch (BusinessExc ex)
        {
            logger.LogWarning(ex, "Business rule failure {Request}", RequestName);
            throw;
        }
        catch (ConflictExc ex)
        {
            logger.LogWarning(ex, "Conflict exception {Request}", RequestName);
            throw;
        }
        catch (NotFoundExc ex)
        {
            logger.LogWarning(ex, "Requested data not found {Request}", RequestName);
            throw;
        }
        catch (UnauthorizedExc ex)
        {
            logger.LogWarning(ex, "You dont have permission to perform this request {Request}", RequestName);
            throw;
        }
        catch (ForbiddenExc ex)
        {
            logger.LogWarning(ex, "Request Forbidden {Request}", RequestName);
            throw;
        }
        catch (ConcurrencyExc ex)
        {
            logger.LogWarning(ex, "Concurrency conflict {Request}", RequestName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error {Request}", RequestName);
            throw;
        }
    }
}

public sealed class DbExceptionBehavior<TRequest, TResponse>(IDbExceptionTranslator translator) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse>
{
    private readonly IDbExceptionTranslator _translator = translator;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            throw ex.Translate(_translator);
        }
    }
}