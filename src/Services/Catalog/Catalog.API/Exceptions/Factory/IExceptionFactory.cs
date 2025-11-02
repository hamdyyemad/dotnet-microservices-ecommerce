namespace Catalog.API.Exceptions;

public interface IExceptionFactory
{
    // Product Not Found
    ProductNotFoundException CreateProductNotFoundException(Guid productId);
    ProductNotFoundException CreateProductNotFoundException(string identifier);
}

