namespace Catalog.API.Exceptions;

public interface IExceptionFactory
{
    // Product Not Found
    ProductNotFoundException CreateProductNotFoundException(string message);
}

