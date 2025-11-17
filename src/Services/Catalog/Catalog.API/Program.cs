using Catalog.API.Exceptions;
using Catalog.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

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
