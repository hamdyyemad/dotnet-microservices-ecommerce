namespace Catalog.API.Exceptions;

public class ExceptionFactory : IExceptionFactory
{
    public ProductNotFoundException CreateProductNotFoundException(Guid productId)
    {
        var exceptionInfo = ExceptionMessages.Product.NotFound(productId);
        return new ProductNotFoundException(exceptionInfo);
    }

    public ProductNotFoundException CreateProductNotFoundException(string identifier)
    {
        var exceptionInfo = ExceptionMessages.Product.NotFound(identifier);
        return new ProductNotFoundException(exceptionInfo);
    }
}

