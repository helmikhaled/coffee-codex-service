using CoffeeCodex.Application;
using CoffeeCodex.API.Authentication;
using CoffeeCodex.Infrastructure;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.Configure<Auth0Settings>(
    builder.Configuration.GetSection(Auth0Settings.SectionName));
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("CoffeeCodex.API"))
    .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");

app.Run();
