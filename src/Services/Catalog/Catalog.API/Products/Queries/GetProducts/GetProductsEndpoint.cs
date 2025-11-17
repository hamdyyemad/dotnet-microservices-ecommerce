using Catalog.API.Models;
using Catalog.API.Routes;

namespace Catalog.API.Products.Queries.GetProducts
{
    //public record GetProductsRequest()
    internal record GetProductsResponse(IEnumerable<Product> Products);
    public class GetProductsEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app, string routePath)
        {
            app.MapGet(routePath, async (ISender sender) =>
            {
                var result = await sender.Send(new GetProductsQuery());
                var response = result.Adapt<GetProductsResponse>();
                return Results.Ok(response);

            })
            .WithName("GetProducts")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
        }
    }
}
