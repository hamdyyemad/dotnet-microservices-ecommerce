using BuildingBlocks.Exceptions;

namespace Catalog.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = exception switch
        {
            ValidationException validationException => new
            {
                status = validationException.StatusCode,
                title = validationException.Title,
                detail = validationException.Message,
                type = validationException.GetType().Name,
                errors = validationException.Errors
            },
            BaseException baseException => new
            {
                status = baseException.StatusCode,
                title = baseException.Title,
                detail = baseException.Message,
                type = baseException.GetType().Name,
                errors = (Dictionary<string, string[]>?)null
            },
            _ => new
            {
                status = StatusCodes.Status500InternalServerError,
                title = "Internal Server Error",
                detail = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An error occurred while processing your request.",
                type = exception.GetType().Name,
                errors = (Dictionary<string, string[]>?)null
            }
        };

        context.Response.StatusCode = problemDetails.status;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}

