namespace McpService.API.Services;
using global::Grpc.Core;
using McpService.API.Grpc;
using McpService.Application.Services;
using System.Text.Json;

public class ToolGrpcService : ToolService.ToolServiceBase
{
    private readonly ToolExecutorService _toolService;
    private readonly ILogger<ToolGrpcService> _logger;

    public ToolGrpcService(ToolExecutorService toolService, ILogger<ToolGrpcService> logger)
    {
        _toolService = toolService;
        _logger = logger;
    }

    public override async Task<ToolResponse> RegisterTool(RegisterToolRequest request, ServerCallContext context)
    {
        try
        {
            var registerRequest = new Application.DTOs.RegisterToolRequest(
                request.Name,
                request.Description,
                request.Version,
                JsonDocument.Parse(request.InputSchema),
                JsonDocument.Parse(request.OutputSchema),
                request.Endpoint,
                request.Protocol);

            var response = await _toolService.RegisterToolAsync(registerRequest, context.CancellationToken);

            return new ToolResponse
            {
                Id = response.Id,
                Name = response.Name,
                Description = response.Description,
                Version = response.Version,
                InputSchema = response.InputSchema.RootElement.GetRawText(),
                OutputSchema = response.OutputSchema.RootElement.GetRawText(),
                Endpoint = response.Endpoint,
                Protocol = response.Protocol,
                IsEnabled = response.IsEnabled,
                CreatedAt = new DateTimeOffset(response.CreatedAt).ToUnixTimeSeconds(),
                UpdatedAt = new DateTimeOffset(response.UpdatedAt).ToUnixTimeSeconds(),
                LastExecuted = response.LastExecuted?.Ticks ?? 0,
                ExecutionCount = response.ExecutionCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC RegisterTool failed");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<ExecuteToolResponse> ExecuteTool(ExecuteToolRequest request, ServerCallContext context)
    {
        try
        {
            var executeRequest = new Application.DTOs.ExecuteToolRequest(JsonDocument.Parse(request.Input));
            var result = await _toolService.ExecuteToolAsync(request.ToolId, executeRequest, context.CancellationToken);

            return new ExecuteToolResponse
            {
                Result = result.RootElement.GetRawText()
            };
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC ExecuteTool failed");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<ToolResponse> GetTool(GetToolRequest request, ServerCallContext context)
    {
        try
        {
            var tool = await _toolService.GetToolAsync(request.ToolId, context.CancellationToken);
            if (tool == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Tool not found"));

            return new ToolResponse
            {
                Id = tool.Id,
                Name = tool.Name,
                Description = tool.Description,
                Version = tool.Version,
                InputSchema = tool.InputSchema.RootElement.GetRawText(),
                OutputSchema = tool.OutputSchema.RootElement.GetRawText(),
                Endpoint = tool.Endpoint,
                Protocol = tool.Protocol,
                IsEnabled = tool.IsEnabled,
                CreatedAt = new DateTimeOffset(tool.CreatedAt).ToUnixTimeSeconds(),
                UpdatedAt = new DateTimeOffset(tool.UpdatedAt).ToUnixTimeSeconds(),
                LastExecuted = tool.LastExecuted?.Ticks ?? 0,
                ExecutionCount = tool.ExecutionCount
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC GetTool failed");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<GetAllToolsResponse> GetAllTools(GetAllToolsRequest request, ServerCallContext context)
    {
        try
        {
            var tools = await _toolService.GetAllToolsAsync(context.CancellationToken);
            var response = new GetAllToolsResponse();

            foreach (var tool in tools)
            {
                response.Tools.Add(new ToolResponse
                {
                    Id = tool.Id,
                    Name = tool.Name,
                    Description = tool.Description,
                    Version = tool.Version,
                    InputSchema = tool.InputSchema.RootElement.GetRawText(),
                    OutputSchema = tool.OutputSchema.RootElement.GetRawText(),
                    Endpoint = tool.Endpoint,
                    Protocol = tool.Protocol,
                    IsEnabled = tool.IsEnabled,
                    CreatedAt = new DateTimeOffset(tool.CreatedAt).ToUnixTimeSeconds(),
                    UpdatedAt = new DateTimeOffset(tool.UpdatedAt).ToUnixTimeSeconds(),
                    LastExecuted = tool.LastExecuted?.Ticks ?? 0,
                    ExecutionCount = tool.ExecutionCount
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC GetAllTools failed");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}