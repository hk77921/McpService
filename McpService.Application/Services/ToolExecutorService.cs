using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McpService.Application.Services
{
    using McpService.Application.DTOs;
    using McpService.Application.Validators;
    using McpService.Domain.Entities;
    using McpService.Domain.Interfaces;
    using McpService.Domain.ValueObjects;
    using Microsoft.Extensions.Logging;
    using System.Text.Json;

    public class ToolExecutorService
    {
        private readonly IToolRegistry _toolRegistry;
        private readonly IEnumerable<IToolExecutor> _executors;
        private readonly JsonSchemaValidator _validator;
        private readonly ILogger<ToolExecutorService> _logger;

        public ToolExecutorService(
            IToolRegistry toolRegistry,
            IEnumerable<IToolExecutor> executors,
            JsonSchemaValidator validator,
            ILogger<ToolExecutorService> logger)
        {
            _toolRegistry = toolRegistry;
            _executors = executors;
            _validator = validator;
            _logger = logger;
        }

        public async Task<ToolResponse> RegisterToolAsync(RegisterToolRequest request, CancellationToken cancellationToken = default)
        {
            var metadata = new ToolMetadata(
                ToolId.New(),
                request.Name,
                request.Description,
                request.Version,
                DateTime.UtcNow,
                DateTime.UtcNow);

            var definition = new ToolDefinition(
                metadata,
                request.InputSchema,
                request.OutputSchema,
                request.Endpoint,
                request.Protocol);

            var tool = new Tool(definition);
            await _toolRegistry.RegisterToolAsync(tool, cancellationToken);

            _logger.LogInformation("Registered tool {ToolName} with ID {ToolId}",
                request.Name, metadata.Id);

            return MapToResponse(tool);
        }

        public async Task<JsonDocument> ExecuteToolAsync(string toolId, ExecuteToolRequest request, CancellationToken cancellationToken = default)
        {
            var id = ToolId.Parse(toolId);
            var tool = await _toolRegistry.GetToolAsync(id, cancellationToken);

            if (tool == null)
                throw new ArgumentException($"Tool with ID {toolId} not found");

            if (!tool.IsEnabled)
                throw new InvalidOperationException($"Tool {tool.Definition.Metadata.Name} is disabled");

            // Validate input against schema
            var isValid = await _validator.ValidateAsync(tool.Definition.InputSchema, request.Input);
            if (!isValid)
                throw new ArgumentException("Input does not match the tool's input schema");

            var executor = _executors.FirstOrDefault(e => e.CanExecute(tool));
            if (executor == null)
                throw new InvalidOperationException($"No executor available for tool protocol: {tool.Definition.Protocol}");

            try
            {
                var result = await executor.ExecuteAsync(tool, request.Input, cancellationToken);
                tool.RecordExecution();
                await _toolRegistry.UpdateToolAsync(tool, cancellationToken);

                _logger.LogInformation("Successfully executed tool {ToolName}", tool.Definition.Metadata.Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute tool {ToolName}", tool.Definition.Metadata.Name);
                throw;
            }
        }

        public async Task<IEnumerable<ToolResponse>> GetAllToolsAsync(CancellationToken cancellationToken = default)
        {
            var tools = await _toolRegistry.GetAllToolsAsync(cancellationToken);
            return tools.Select(MapToResponse);
        }

        public async Task<ToolResponse?> GetToolAsync(string toolId, CancellationToken cancellationToken = default)
        {
            var id = ToolId.Parse(toolId);
            var tool = await _toolRegistry.GetToolAsync(id, cancellationToken);
            return tool != null ? MapToResponse(tool) : null;
        }

        private static ToolResponse MapToResponse(Tool tool) => new(
            tool.Definition.Metadata.Id.ToString(),
            tool.Definition.Metadata.Name,
            tool.Definition.Metadata.Description,
            tool.Definition.Metadata.Version,
            tool.Definition.InputSchema,
            tool.Definition.OutputSchema,
            tool.Definition.Endpoint,
            tool.Definition.Protocol,
            tool.IsEnabled,
            tool.Definition.Metadata.CreatedAt,
            tool.Definition.Metadata.UpdatedAt,
            tool.LastExecuted == default ? null : tool.LastExecuted,
            tool.ExecutionCount);
    }
}
