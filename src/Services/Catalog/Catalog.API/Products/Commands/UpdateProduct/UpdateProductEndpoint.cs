using Catalog.API.Models;
using Catalog.API.Products.Observers;

namespace Catalog.API.Products.Commands.UpdateProduct
{
    internal record UpdateProductResponse(bool IsSucess);
    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products", async (Product product, ISender sender) =>
            {
                var command = new UpdateProductObserverCommand(product);
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
