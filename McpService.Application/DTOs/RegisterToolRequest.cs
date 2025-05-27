using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McpService.Application.DTOs
{
    using System.Text.Json;

    public record RegisterToolRequest(
        string Name,
        string Description,
        string Version,
        JsonDocument InputSchema,
        JsonDocument OutputSchema,
        string Endpoint,
        string Protocol = "HTTP");
}
