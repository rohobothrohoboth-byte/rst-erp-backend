using Microsoft.Extensions.Logging;
using Npgsql;

namespace Leave.App.Interfaces;

public interface IDbRetryHandler
{
    Task ExecuteAsync(Func<Task> operation, CancellationToken ct = default);
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken ct = default);
}

public sealed class DbRetryHandler : IDbRetryHandler
{
    private readonly ILogger<DbRetryHandler> _logger;
    private const int MaxRetries = 5;
    private const int BaseDelayMs = 200;

    public DbRetryHandler(ILogger<DbRetryHandler> logger) { _logger = logger; }

    public async Task ExecuteAsync(Func<Task> operation, CancellationToken ct = default)
    {
        await ExecuteAsync(async () =>
        {
            await operation();
            return true;
        }, ct);
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken ct = default)
    {
        var attempt = 0;
        while (true)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (IsTransient(ex) && attempt < MaxRetries)
            {
                attempt++;
                var delay = TimeSpan.FromMilliseconds(BaseDelayMs * attempt);
                _logger.LogWarning(ex, "Transient DB failure detected. Retry {Attempt}/{MaxRetries} after {Delay} ms", attempt, MaxRetries, delay.TotalMilliseconds);
                await Task.Delay(delay, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database operation FAILED permanently after {Attempts} retries", attempt);
                throw;
            }
        }
    }

    private static bool IsTransient(Exception ex)
    {
        if (ex is NpgsqlException npgsqlEx) { return npgsqlEx.IsTransient; }
        if (ex is TimeoutException) { return true; }
        if (ex.InnerException is NpgsqlException inner) { return inner.IsTransient; }
        return false;
    }
}