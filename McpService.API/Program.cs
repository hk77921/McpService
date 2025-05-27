using McpService.API.Services;
using McpService.Application.Services;
using McpService.Application.Validators;
using McpService.Domain.Interfaces;
using McpService.Infrastructure.Http;
using McpService.Infrastructure.Persistence;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Activity source for OpenTelemetry
var activitySource = new ActivitySource("McpService");

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddGrpc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks
builder.Services.AddHealthChecks();

// HttpClient for tool execution
builder.Services.AddHttpClient<HttpToolExecutor>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Redis (optional)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(redisConnectionString));
    builder.Services.AddScoped<IToolRegistry, RedisToolRegistry>();
}
else
{
    builder.Services.AddSingleton<IToolRegistry, InMemoryToolRegistry>();
}

// Application services
builder.Services.AddScoped<JsonSchemaValidator>();
builder.Services.AddScoped<ToolExecutorService>();
builder.Services.AddScoped<IToolExecutor, HttpToolExecutor>();
builder.Services.AddHostedService<ToolSyncService>();

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("McpService"))
            .AddSource(activitySource.Name)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
            //.AddConsoleExporter();
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<ToolGrpcService>();
app.MapHealthChecks("/health");

app.Run();