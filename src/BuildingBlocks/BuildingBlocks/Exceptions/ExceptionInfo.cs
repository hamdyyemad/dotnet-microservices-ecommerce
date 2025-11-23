namespace BuildingBlocks.Exceptions;

/// <summary>
/// Contains all information needed to create an exception and its HTTP response.
/// Shared across all microservices for consistent error handling.
/// </summary>
public record ExceptionInfo(string Message, int StatusCode, string Title);

