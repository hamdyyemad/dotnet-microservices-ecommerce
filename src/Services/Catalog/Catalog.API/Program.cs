using Catalog.API.Exceptions;
using Catalog.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Register Carter for route discovery
// Note: If using centralized CatalogRoutes, consider disabling auto-discovery
// by removing ICarterModule from individual endpoint classes to avoid duplicate routes
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
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
