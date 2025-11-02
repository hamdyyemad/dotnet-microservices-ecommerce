namespace Catalog.API.Exceptions;

public abstract class CatalogException : Exception
{
    protected CatalogException(string message) : base(message)
    {
    }

    protected CatalogException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

