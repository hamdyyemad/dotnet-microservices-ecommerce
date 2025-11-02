namespace Catalog.API.Exceptions;

public class ExceptionFactory : IExceptionFactory
{
    public ProductNotFoundException CreateProductNotFoundException(Guid productId)
    {
        var message = ExceptionMessages.Product.NotFound(productId);
        return new ProductNotFoundException(message);
    }

    public ProductNotFoundException CreateProductNotFoundException(string identifier)
    {
        var message = ExceptionMessages.Product.NotFound(identifier);
        return new ProductNotFoundException(message);
    }
}

