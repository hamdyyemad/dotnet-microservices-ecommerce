using Catalog.API.Models;
using Catalog.API.Routes;

namespace Catalog.API.Products.Queries.GetProductById
{
    //public record GetProductByIdRequest();
    internal record GetProductByIdResponse(Product product);
    public class GetProductByIdEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app, string routePath)
        {
            app.MapGet(routePath, async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id));
                var response = new GetProductByIdResponse(result.product);
                return Results.Ok(response);
            })
            .WithName("GetProductById")
            .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Id")
            .WithDescription("Get Product By Id");
        }
    }
}
