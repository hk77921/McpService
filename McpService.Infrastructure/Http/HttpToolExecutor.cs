using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McpService.Domain.Entities;
using McpService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace McpService.Infrastructure.Http
{
  

    public class HttpToolExecutor : IToolExecutor
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpToolExecutor> _logger;

        public HttpToolExecutor(HttpClient httpClient, ILogger<HttpToolExecutor> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public bool CanExecute(Tool tool) => !tool.Definition.IsGrpc;

        public async Task<JsonDocument> ExecuteAsync(Tool tool, JsonDocument input, CancellationToken cancellationToken = default)
        {
            if (!CanExecute(tool))
                throw new InvalidOperationException("Tool is not configured for HTTP execution");

            try
            {
                var json = JsonSerializer.Serialize(input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Executing tool {ToolName} at {Endpoint}",
                    tool.Definition.Metadata.Name, tool.Definition.Endpoint);

                var response = await _httpClient.PostAsync(tool.Definition.Endpoint, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonDocument.Parse(responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute tool {ToolName}", tool.Definition.Metadata.Name);
                throw;
            }
        }
    }
}
