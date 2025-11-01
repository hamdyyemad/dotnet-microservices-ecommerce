namespace Catalog.API.Exceptions;

public class ExceptionFactory : IExceptionFactory
{
    public ProductNotFoundException CreateProductNotFoundException(string message)
    {
        return new ProductNotFoundException(message);
    }
}

