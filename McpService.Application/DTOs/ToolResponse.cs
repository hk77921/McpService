using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace McpService.Application.DTOs
{
    public record ToolResponse(
     string Id,
     string Name,
     string Description,
     string Version,
     JsonDocument InputSchema,
     JsonDocument OutputSchema,
     string Endpoint,
     string Protocol,
     bool IsEnabled,
     DateTime CreatedAt,
     DateTime UpdatedAt,
     DateTime? LastExecuted = null,
     int ExecutionCount = 0);
}
