using Catalog.API.Models;
using Catalog.API.Products.Orchestrator;
using Catalog.API.Routes;

namespace Catalog.API.Products.Commands.UpdateProduct
{
    internal record UpdateProductResponse(bool IsSucess);
    public class UpdateProductEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app, string routePath)
        {
            app.MapPut(routePath, async (Product product, ISender sender) =>
            {
                var command = new UpdateProductOrchestratorCommand(product);
                var result = await sender.Send(command);
                var response = new UpdateProductResponse(result);
                return Results.Ok(response);
            })
            .WithName("UpdateProduct")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Product")
            .WithDescription("Update Product");
        }
    }
}
