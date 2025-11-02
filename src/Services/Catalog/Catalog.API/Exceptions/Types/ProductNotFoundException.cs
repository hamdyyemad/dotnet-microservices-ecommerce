namespace Catalog.API.Exceptions;

public class ProductNotFoundException : CatalogException
{
    public ProductNotFoundException(string message) : base(message)
    {
    }

    public ProductNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

