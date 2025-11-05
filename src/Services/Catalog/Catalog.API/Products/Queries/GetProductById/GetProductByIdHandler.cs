using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Extensions;
using Catalog.API.Models;


namespace Catalog.API.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(Guid id) : IQuery<GetProductByIdResult>;
    public record GetProductByIdResult(Product product);
    
    internal class GetProductByIdHandlerQuery(IQuerySession session, IExceptionFactory exceptionFactory)
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = await session.GetProductByIdOrThrowAsync(query.id, exceptionFactory, cancellationToken);
            return new GetProductByIdResult(product);
        }
    }
}
