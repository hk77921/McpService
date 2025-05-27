using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McpService.Domain.Entities;
using McpService.Domain.Interfaces;
using McpService.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace McpService.Infrastructure.Persistence
{
   
    public class InMemoryToolRegistry : IToolRegistry
    {
        private readonly ConcurrentDictionary<ToolId, Tool> _tools = new();

        public Task<Tool?> GetToolAsync(ToolId id, CancellationToken cancellationToken = default)
        {
            _tools.TryGetValue(id, out var tool);
            return Task.FromResult(tool);
        }

        public Task<Tool?> GetToolByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var tool = _tools.Values.FirstOrDefault(t => t.Definition.Metadata.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(tool);
        }

        public Task<IEnumerable<Tool>> GetAllToolsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<Tool>>(_tools.Values.ToList());
        }

        public Task RegisterToolAsync(Tool tool, CancellationToken cancellationToken = default)
        {
            _tools.AddOrUpdate(tool.Definition.Metadata.Id, tool, (_, _) => tool);
            return Task.CompletedTask;
        }

        public Task UpdateToolAsync(Tool tool, CancellationToken cancellationToken = default)
        {
            _tools.AddOrUpdate(tool.Definition.Metadata.Id, tool, (_, _) => tool);
            return Task.CompletedTask;
        }

        public Task UnregisterToolAsync(ToolId id, CancellationToken cancellationToken = default)
        {
            _tools.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(ToolId id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_tools.ContainsKey(id));
        }
    }
}
