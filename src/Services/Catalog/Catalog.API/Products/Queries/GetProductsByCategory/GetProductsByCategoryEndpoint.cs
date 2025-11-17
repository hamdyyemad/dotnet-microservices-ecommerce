using Catalog.API.Models;
using Catalog.API.Routes;

namespace Catalog.API.Products.Queries.GetProductsByCategory
{
    // public record GetProductByCategoryRequest();
    internal record GetProductByCategoryResponse(IEnumerable<Product> products);
    public class GetProductsByCategoryEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app, string routePath)
        {
            app.MapGet(routePath, async (string category, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByCategoryQuery(category));
                var response = result.Adapt<GetProductByCategoryResponse>();
                return Results.Ok(response);

            })
            .WithName("GetProductsByCategory")
            .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products By Category")
            .WithDescription("Get Products By Category");
        }
    }
}
