using System.Text.Json.Serialization;
using CoffeeCodex.Application;
using CoffeeCodex.API.Authentication;
using CoffeeCodex.Infrastructure;
using CoffeeCodex.Infrastructure.Persistence;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
var corsPolicyName = builder.Configuration["Cors:PolicyName"]
    ?? throw new InvalidOperationException("Missing configuration value 'Cors:PolicyName'.");
var corsAllowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .GetChildren()
    .Select(section => section.Value)
    .Where(value => !string.IsNullOrWhiteSpace(value))
    .Cast<string>()
    .ToArray();
if (corsAllowedOrigins.Length == 0)
{
    throw new InvalidOperationException("At least one CORS origin must be configured in 'Cors:AllowedOrigins'.");
}

var openTelemetryServiceName = builder.Configuration["OpenTelemetry:ServiceName"]
    ?? throw new InvalidOperationException("Missing configuration value 'OpenTelemetry:ServiceName'.");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins(corsAllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddHealthChecks();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.Configure<Auth0Settings>(
    builder.Configuration.GetSection(Auth0Settings.SectionName));
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(openTelemetryServiceName))
    .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.Services.InitializeDatabaseAsync();

app.UseCors(corsPolicyName);
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
