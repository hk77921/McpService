using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace McpService.Domain.Entities
{
    public record ToolDefinition(
        ToolMetadata Metadata,
        JsonDocument InputSchema,
        JsonDocument OutputSchema,
        string Endpoint,
        string Protocol = "HTTP") // HTTP or GRPC
    {
        public bool IsGrpc => Protocol.Equals("GRPC", StringComparison.OrdinalIgnoreCase);
    }

    public class Tool
    {
        public ToolDefinition Definition { get; private set; }
        public bool IsEnabled { get; private set; } = true;
        public DateTime LastExecuted { get; private set; }
        public int ExecutionCount { get; private set; }

        public Tool(ToolDefinition definition)
        {
            Definition = definition;
        }

        public void UpdateDefinition(ToolDefinition newDefinition)
        {
            Definition = newDefinition with
            {
                Metadata = newDefinition.Metadata with
                {
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }

        public void Disable() => IsEnabled = false;
        public void Enable() => IsEnabled = true;

        public void RecordExecution()
        {
            LastExecuted = DateTime.UtcNow;
            ExecutionCount++;
        }
    }
}
