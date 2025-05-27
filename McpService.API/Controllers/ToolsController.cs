namespace McpService.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using McpService.Application.DTOs;
using McpService.Application.Services;
using System.Diagnostics;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class ToolsController : ControllerBase
{
    private readonly ToolExecutorService _toolService;
    private readonly ILogger<ToolsController> _logger;

    public ToolsController(ToolExecutorService toolService, ILogger<ToolsController> logger)
    {
        _toolService = toolService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ToolResponse>> RegisterTool([FromBody] RegisterToolRequest request, CancellationToken cancellationToken)
    {
        using var activity = Activity.Current?.Source.StartActivity("RegisterTool");
        activity?.SetTag("tool.name", request.Name);

        try
        {
            var response = await _toolService.RegisterToolAsync(request, cancellationToken);
            _logger.LogInformation("Tool {ToolName} registered successfully with ID {ToolId}",
                request.Name, response.Id);

            return CreatedAtAction(nameof(GetTool), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register tool {ToolName}", request.Name);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/execute")]
    public async Task<ActionResult<JsonDocument>> ExecuteTool(string id, [FromBody] ExecuteToolRequest request, CancellationToken cancellationToken)
    {
        using var activity = Activity.Current?.Source.StartActivity("ExecuteTool");
        activity?.SetTag("tool.id", id);

        try
        {
            var result = await _toolService.ExecuteToolAsync(id, request, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute tool {ToolId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToolResponse>>> GetAllTools(CancellationToken cancellationToken)
    {
        var tools = await _toolService.GetAllToolsAsync(cancellationToken);
        return Ok(tools);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ToolResponse>> GetTool(string id, CancellationToken cancellationToken)
    {
        try
        {
            var tool = await _toolService.GetToolAsync(id, cancellationToken);
            return tool != null ? Ok(tool) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get tool {ToolId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}