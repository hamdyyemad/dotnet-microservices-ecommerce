using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Catalog.API.Exceptions;

/// <summary>
/// Centralized exception definitions (messages, status codes, and titles) for the Catalog API.
/// This ensures consistent error responses across the application.
/// </summary>
internal static class ExceptionMessages
{
    public static class Product
    {
        public static ExceptionInfo NotFound(Guid productId) => new(
            Message: $"Product with ID '{productId}' was not found.",
            StatusCode: StatusCodes.Status404NotFound,
            Title: "Product Not Found"
        );

        public static ExceptionInfo NotFound(string identifier) => new(
            Message: $"Product with identifier '{identifier}' was not found.",
            StatusCode: StatusCodes.Status404NotFound,
            Title: "Product Not Found"
        );
    }
}

