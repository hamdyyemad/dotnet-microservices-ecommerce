namespace Catalog.API.Exceptions;

/// <summary>
/// Base exception class for all catalog-related exceptions.
/// Contains HTTP response information (status code and title) along with the error message.
/// </summary>
public class CatalogException : Exception
{
    public int StatusCode { get; }
    public string Title { get; }

    public CatalogException(ExceptionInfo info) : base(info.Message)
    {
        StatusCode = info.StatusCode;
        Title = info.Title;
    }

    public CatalogException(ExceptionInfo info, Exception innerException) 
        : base(info.Message, innerException)
    {
        StatusCode = info.StatusCode;
        Title = info.Title;
    }
}

