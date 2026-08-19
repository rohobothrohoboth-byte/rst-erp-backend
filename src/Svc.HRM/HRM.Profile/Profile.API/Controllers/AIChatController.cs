using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.App.Interfaces;
using Profile.Domain.Entities;
using Common;
using Dapper;
using EthiopianCalendar;
using Profile.App.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Profile.App.Services.AI;
namespace Profile.API.Controllers;


[ApiController]
[Route("api/ai/chat")]
[Authorize]
public class AIChatController : ControllerBase
{
    private readonly EmployeeAIService _aiService;
    private readonly ILogger<AIChatController> _logger;

    public AIChatController(
        EmployeeAIService aiService,
        ILogger<AIChatController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    [HttpPost("query")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AskQuestion([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new { success = false, message = "Please enter a question." });
        }

        try
        {
            var response = await _aiService.ProcessQueryAsync(request.Query);

            return Ok(new
            {
                success = response.Success,
                data = new
                {
                    message = response.Message,
                    response.Data
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AI chat");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred. Please try again."
            });
        }
    }

    [HttpGet("suggestions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSuggestions()
    {
        var suggestions = new[]
        {
            "How many employees are male?",
            "Who is retiring this year?",
            "Show me HR department employees",
            "How many female employees are there?",
            "List employees eligible for retirement",
            "Employee count by gender"
        };

        return Ok(new { success = true, data = suggestions });
    }
}

public class ChatRequest
{
    public string Query { get; set; } = string.Empty;
}