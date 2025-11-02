using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Models;

namespace Catalog.API.Products.GetProductByCategory
{
    public record GetProductByCategoryQuery(string category) : IQuery<GetProductByCategoryResult>;
    public record GetProductByCategoryResult(IEnumerable<Product> Products);
    public class GetProductsByCategoryHandler(IQuerySession session) 
        : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
        {
            var products = await session.Query<Product>()
            .Where(p => p.Category.Any(c => c.ToLower().Contains(query.category.ToLower())))
            .ToListAsync(cancellationToken);
            return new GetProductByCategoryResult(products);
        }
    }
}
