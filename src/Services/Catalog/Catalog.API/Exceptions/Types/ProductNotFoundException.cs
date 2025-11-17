namespace Catalog.API.Exceptions;

public class ProductNotFoundException : CatalogException
{
    public ProductNotFoundException(ExceptionInfo info) : base(info)
    {
    }

    public ProductNotFoundException(ExceptionInfo info, Exception innerException) 
        : base(info, innerException)
    {
    }
}

