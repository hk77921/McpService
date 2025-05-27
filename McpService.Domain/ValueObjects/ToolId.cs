using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McpService.Domain.ValueObjects
{
    public record ToolId(Guid Value)
    {
        public static ToolId New() => new(Guid.NewGuid());
        public static ToolId Parse(string value) => new(Guid.Parse(value));
        public override string ToString() => Value.ToString();
    }
}
