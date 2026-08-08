using Asp.Versioning;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Services;

namespace Svc.Auth.Controllers;

[AllowAnonymous]  // Webhooks can be called without authentication
[ApiController]
[Route("api/auth/v{version:apiVersion}/webhooks")]
[ApiVersion("1.0")]
public class WebhookController : ControllerBase
{
    private readonly ISetupService _setupService;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ISetupService setupService, ILogger<WebhookController> logger)
    {
        _setupService = setupService;
        _logger = logger;
    }

    /// <summary>
    /// Webhook endpoint for Core Module to trigger sync
    /// </summary>
    [HttpPost("core-module")]
    public async Task<IActionResult> CoreModuleWebhook([FromBody] WebhookPayload payload)
    {
        _logger.LogInformation("Webhook received from Core Module: {EventType}", payload.EventType);

        // Validate webhook signature (optional - add security)
        // var isValid = ValidateSignature(Request.Headers["X-Webhook-Signature"]);
        // if (!isValid) return Unauthorized();

        if (payload.EventType == "data.changed")
        {
            await _setupService.TriggerSyncAsync();
            return Ok(ApiResponse<object>.Ok(null, "Sync triggered successfully"));
        }

        return Ok(ApiResponse<object>.Ok(null, "No action taken"));
    }

    /// <summary>
    /// Webhook endpoint for HRM to trigger sync
    /// </summary>
    [HttpPost("hrm")]
    public async Task<IActionResult> HrmWebhook([FromBody] WebhookPayload payload)
    {
        _logger.LogInformation("Webhook received from HRM: {EventType}", payload.EventType);

        if (payload.EventType == "data.changed")
        {
            await _setupService.TriggerSyncAsync();
            return Ok(ApiResponse<object>.Ok(null, "Sync triggered successfully"));
        }

        return Ok(ApiResponse<object>.Ok(null, "No action taken"));
    }
}

public class WebhookPayload
{
    public string EventType { get; set; } = string.Empty;
    public string? Entity { get; set; }
    public string? EntityId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}