namespace Catalog.API.Exceptions;

/// <summary>
/// Contains all information needed to create an exception and its HTTP response.
/// </summary>
public record ExceptionInfo(string Message, int StatusCode, string Title);

