using Catalog.API.Products.Orchestrator;
using Catalog.API.Routes;

namespace Catalog.API.Products.Commands.DeleteProduct
{
    internal record DeleteProductResponse(bool IsSucess);
    public class DeleteProductEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app, string routePath)
        {
            app.MapDelete(routePath, async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteProductOrchestratorCommand(id));
                var response = new DeleteProductResponse(result);
                return Results.Ok(response);
            })
            .WithName("DeleteProduct")
            .Produces<DeleteProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Product")
            .WithDescription("Delete Product");
        }
    }
}
