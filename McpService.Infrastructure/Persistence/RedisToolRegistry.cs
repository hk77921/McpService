using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McpService.Domain.Entities;
using McpService.Domain.Interfaces;
using McpService.Domain.ValueObjects;
using StackExchange.Redis;
using System.Text.Json;

namespace McpService.Infrastructure.Persistence
{
   

    public class RedisToolRegistry : IToolRegistry
    {
        private readonly IDatabase _database;
        private const string ToolKeyPrefix = "tool:";
        private const string ToolIndexKey = "tools";

        public RedisToolRegistry(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<Tool?> GetToolAsync(ToolId id, CancellationToken cancellationToken = default)
        {
            var json = await _database.StringGetAsync($"{ToolKeyPrefix}{id}");
            return json.HasValue ? JsonSerializer.Deserialize<Tool>(json!) : null;
        }

        public async Task<Tool?> GetToolByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var tools = await GetAllToolsAsync(cancellationToken);
            return tools.FirstOrDefault(t => t.Definition.Metadata.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Tool>> GetAllToolsAsync(CancellationToken cancellationToken = default)
        {
            var toolIds = await _database.SetMembersAsync(ToolIndexKey);
            var tools = new List<Tool>();

            foreach (var toolId in toolIds)
            {
                var tool = await GetToolAsync(ToolId.Parse(toolId!), cancellationToken);
                if (tool != null)
                    tools.Add(tool);
            }

            return tools;
        }

        public async Task RegisterToolAsync(Tool tool, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(tool);
            var key = $"{ToolKeyPrefix}{tool.Definition.Metadata.Id}";

            await Task.WhenAll(
                _database.StringSetAsync(key, json),
                _database.SetAddAsync(ToolIndexKey, tool.Definition.Metadata.Id.ToString())
            );
        }

        public async Task UpdateToolAsync(Tool tool, CancellationToken cancellationToken = default)
        {
            await RegisterToolAsync(tool, cancellationToken);
        }

        public async Task UnregisterToolAsync(ToolId id, CancellationToken cancellationToken = default)
        {
            var key = $"{ToolKeyPrefix}{id}";
            await Task.WhenAll(
                _database.KeyDeleteAsync(key),
                _database.SetRemoveAsync(ToolIndexKey, id.ToString())
            );
        }

        public async Task<bool> ExistsAsync(ToolId id, CancellationToken cancellationToken = default)
        {
            return await _database.KeyExistsAsync($"{ToolKeyPrefix}{id}");
        }
    }
}
