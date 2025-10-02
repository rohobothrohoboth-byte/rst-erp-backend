using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Profile.Utility.Extensions;

public static class ResiliencePolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(4, retryAttempt)), // 2, 4, 8 seconds
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Log.Warning("Retry {RetryAttempt} after {Delay}s due to {ErrorMessage}", retryAttempt, timespan.TotalSeconds, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString() ?? "Unknown error");
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5), onTimeoutAsync: (context, timespan, task, exception) =>
        {
            Log.Error("HTTP request timed out after {TimeoutSeconds}s", timespan.TotalSeconds);
            return Task.CompletedTask;
        });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, durationOfBreak: TimeSpan.FromSeconds(30), onBreak: (result, breakDelay) =>
                {
                    Log.Error("Circuit opened for {BreakDelay}s due to {Reason}", breakDelay.TotalSeconds, result.Exception?.Message ?? result.Result?.StatusCode.ToString());
                },
                onReset: () => Log.Information("Circuit closed, requests are flowing normally"),
                onHalfOpen: () => Log.Information("Circuit is half-open; testing next call"));
    }
}
