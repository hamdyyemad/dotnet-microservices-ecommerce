using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Models;
using Catalog.API.Products.GetProducts;

namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdQuery(Guid id) : IQuery<GetProductByIdResult>;
    public record GetProductByIdResult(Product product);
    
    internal class GetProductByIdHandlerQuery(IQuerySession session, IExceptionFactory exceptionFactory)
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await session.LoadAsync<Product>(query.id, cancellationToken);
            if(product is null) {
                throw exceptionFactory.CreateProductNotFoundException("Product doesn't exist.");
            }
            return new GetProductByIdResult(product);
        }
    }
}
