using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McpService.Application.DTOs
{
    using System.Text.Json;

    public record ExecuteToolRequest(JsonDocument Input);
}
