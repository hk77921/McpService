# McpService

A modular .NET 9 service for registering, managing, and executing tools via HTTP and gRPC APIs. The solution is organized into four main projects:

- **McpService.Domain**: Core domain entities, value objects, and interfaces.
- **McpService.Application**: Application logic, DTOs, validators, and service orchestration.
- **McpService.Infrastructure**: Implementations for HTTP/gRPC tool execution, logging, and external integrations.
- **McpService.API**: ASP.NET Core Web API and gRPC endpoints for tool registration, management, and execution.

## Features
- Register new tools with input/output schemas, endpoint, and protocol (HTTP/gRPC)
- Execute registered tools with schema validation
- Retrieve tool metadata and execution statistics
- Extensible executor model for supporting new protocols
- OpenAPI (Swagger) and gRPC service definitions

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

### Build & Run
# Restore dependencies
dotnet restore
# Build all projects
dotnet build
# Run the API (from McpService.API directory)
dotnet run --project McpService.API/McpService.API.csproj
### API Usage
- REST endpoints and Swagger UI available at `/swagger` when running the API
- gRPC endpoints defined in `McpService.API/Proto/tools.proto`

#### Register a Tool (REST)
`POST /api/tools`
- Body: `{ name, description, version, inputSchema, outputSchema, endpoint, protocol }`

#### Execute a Tool (REST)
`POST /api/tools/{toolId}/execute`
- Body: `{ input }`

#### List Tools (REST)
`GET /api/tools`

#### gRPC
- See `tools.proto` for service and message definitions

## Project Structure
- `Domain/Entities`: Tool, ToolDefinition, ToolMetadata, etc.
- `Application/Services`: ToolExecutorService, validators, DTOs
- `Infrastructure/Http`, `Infrastructure/Grpc`: Executors for tool protocols
- `API/Controllers`, `API/Services`: REST and gRPC endpoints

## Extending
- Add new executors by implementing `IToolExecutor` in Infrastructure
- Register new protocols in DI and update ToolExecutorService

## License
MIT (or specify your license)
