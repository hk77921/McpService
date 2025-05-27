using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McpService.Domain.Entities;
using System.Text.Json;


namespace McpService.Domain.Interfaces
{
    
    public interface IToolExecutor
    {
        Task<JsonDocument> ExecuteAsync(Tool tool, JsonDocument input, CancellationToken cancellationToken = default);
        bool CanExecute(Tool tool);
    }
}
