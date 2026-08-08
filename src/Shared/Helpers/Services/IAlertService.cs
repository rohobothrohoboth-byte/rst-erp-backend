namespace Shared.Helpers.Services;

public interface IAlertService
{
    Task SendAlertAsync(string level, string title, string message, Exception? ex = null);
    Task SendSuccessAsync(string title, string message);
    Task SendWarningAsync(string title, string message);
    Task SendErrorAsync(string title, string message, Exception? ex = null);
}
