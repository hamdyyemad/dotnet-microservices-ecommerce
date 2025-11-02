namespace Catalog.API.Exceptions;

/// <summary>
/// Centralized exception messages for the Catalog API.
/// This ensures consistent and unified error messages across the application.
/// </summary>
internal static class ExceptionMessages
{
    public static class Product
    {
        public static string NotFound(Guid productId) 
            => $"Product with ID '{productId}' was not found.";

        public static string NotFound(string identifier) 
            => $"Product with identifier '{identifier}' was not found.";
    }
}

