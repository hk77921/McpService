using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McpService.Domain.ValueObjects;

namespace McpService.Domain.Entities
{
    public record ToolMetadata(
        ToolId Id,
        string Name,
        string Description,
        string Version,
        DateTime CreatedAt = default,
        DateTime UpdatedAt = default)
    {
        public ToolMetadata() : this(ToolId.New(), "", "", "", DateTime.UtcNow, DateTime.UtcNow) { }
    }
}
