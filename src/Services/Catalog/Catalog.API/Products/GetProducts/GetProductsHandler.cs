using BuildingBlocks.CQRS;
using Catalog.API.Models;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery() : IQuery<GetProductsResult>;
    public record GetProductsResult(IEnumerable<Product> Products);
    internal class GetProductsQueryHandler(IDocumentStore store)
        : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            using var session = store.LightweightSession();
            var products = await session.Query<Product>().ToListAsync(cancellationToken);
            return new GetProductsResult(products);
        }
    }
}
