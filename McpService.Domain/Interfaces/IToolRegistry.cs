using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McpService.Domain.Entities;
using McpService.Domain.ValueObjects;

namespace McpService.Domain.Interfaces
{ 
    public interface IToolRegistry
    {
        Task<Tool?> GetToolAsync(ToolId id, CancellationToken cancellationToken = default);
        Task<Tool?> GetToolByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Tool>> GetAllToolsAsync(CancellationToken cancellationToken = default);
        Task RegisterToolAsync(Tool tool, CancellationToken cancellationToken = default);
        Task UpdateToolAsync(Tool tool, CancellationToken cancellationToken = default);
        Task UnregisterToolAsync(ToolId id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(ToolId id, CancellationToken cancellationToken = default);
    }
}
