using BuildingBlocks.Behaviors;
using BuildingBlocks.Extensions;
using Catalog.API.Exceptions;
using Catalog.API.Middleware;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs to listen on all interfaces (0.0.0.0) for Docker compatibility
// This ensures the app listens on IPv4 instead of just IPv6, which is required for Docker port mapping
builder.WebHost.UseDockerHosting();

// Register Carter for route discovery
// Note: If using centralized CatalogRoutes, consider disabling auto-discovery
// by removing ICarterModule from individual endpoint classes to avoid duplicate routes
builder.Services.AddCarter();

// Register FluentValidation validators from the assembly
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Register MediatR with pipeline behaviors
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    // Register validation behavior for all requests
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);
})
.UseLightweightSessions();

builder.Services.AddScoped<IExceptionFactory, ExceptionFactory>();

var app = builder.Build();

// Global exception handling middleware - must be registered before MapCarter
// Why This Approach:
// Each microservice handles its own domain exceptions
// The gateway (YARP) proxies responses; it doesn't interpret business logic
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapCarter();

app.Run();
