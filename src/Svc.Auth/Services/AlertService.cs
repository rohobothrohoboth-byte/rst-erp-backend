using Shared.Helpers.Services;

namespace Svc.Auth.Services;

public class AlertService : IAlertService
{
    private readonly ILogger<AlertService> _logger;
    private readonly IConfiguration _configuration;

    public AlertService(ILogger<AlertService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendAlertAsync(string level, string title, string message, Exception? ex = null)
    {
        // Log the alert
        if (level == "Error")
        {
            _logger.LogError(ex, "{Title}: {Message}", title, message);
        }
        else if (level == "Warning")
        {
            _logger.LogWarning("{Title}: {Message}", title, message);
        }
        else
        {
            _logger.LogInformation("{Title}: {Message}", title, message);
        }

        // You can add additional notification channels here:
        // - Email
        // - Slack/Teams
        // - SMS
        // - PagerDuty

        // For now, log to console
        Console.WriteLine($"[{level}] {title}: {message}");
        if (ex != null)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        await Task.CompletedTask;
    }

    public async Task SendSuccessAsync(string title, string message)
    {
        await SendAlertAsync("Info", title, message);
    }

    public async Task SendWarningAsync(string title, string message)
    {
        await SendAlertAsync("Warning", title, message);
    }

    public async Task SendErrorAsync(string title, string message, Exception? ex = null)
    {
        await SendAlertAsync("Error", title, message, ex);
    }
}