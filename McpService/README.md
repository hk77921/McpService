# McpService

A .NET 9 microservice for dynamic tool registration, execution, and management via REST and gRPC APIs. Supports OpenTelemetry tracing, Redis or in-memory storage, and Swagger UI for API exploration.

## Features
- Register, execute, and manage tools dynamically
- RESTful API (with Swagger UI)
- gRPC API (see `Proto/tools.proto`)
- Redis or in-memory tool registry
- OpenTelemetry tracing
- Health checks

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/)
- (Optional) Redis server for distributed tool registry

### Build & Rundotnet build
cd McpService.API
dotnet run
### Configuration
- Redis connection string: set `ConnectionStrings:Redis` in `appsettings.json` or environment variables to enable Redis-backed registry.

### REST API
- Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger) (when running in Development)
- Example endpoints:
  - `POST /api/tools/register` — Register a new tool
  - `POST /api/tools/{id}/execute` — Execute a tool
  - `GET /api/tools` — List all tools
  - `GET /api/tools/{id}` — Get tool details

### gRPC API
- Proto file: [`Proto/tools.proto`](McpService.API/Proto/tools.proto)
- Service: `ToolService`
- Methods: `RegisterTool`, `ExecuteTool`, `GetTool`, `GetAllTools`

### Health Check
- `GET /health`

## Project Structure
- `McpService.API` — ASP.NET Core Web API & gRPC host
- `McpService.Application` — Application logic, DTOs, services
- `McpService.Domain` — Domain models and interfaces
- `McpService.Infrastructure` — Persistence, Redis, HTTP clients

## Telemetry
- OpenTelemetry tracing is enabled by default. Configure exporters as needed in `Program.cs`.

## License
MIT