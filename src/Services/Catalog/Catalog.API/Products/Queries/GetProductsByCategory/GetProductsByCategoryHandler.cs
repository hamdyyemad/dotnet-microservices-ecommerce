using BuildingBlocks.CQRS;
using Catalog.API.Models;

namespace Catalog.API.Products.Queries.GetProductsByCategory
{
    public record GetProductByCategoryQuery(string category) : IQuery<GetProductByCategoryResult>;
    public record GetProductByCategoryResult(IEnumerable<Product> Products);
    public class GetProductsByCategoryQueryHandler(IQuerySession session) 
        : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
        {
            // Efficient SQL-based query using Marten's LINQ provider
            // StringComparison.OrdinalIgnoreCase is translated to SQL for case-insensitive matching
            // This executes entirely at the database level for optimal performance
            
            var products = await session.Query<Product>()
                .Where(p => p.Category != null && 
                           p.Category.Any(c => c.Contains(query.category, StringComparison.OrdinalIgnoreCase)))
                .ToListAsync(cancellationToken);
            
            return new GetProductByCategoryResult(products);
        }
    }
}
