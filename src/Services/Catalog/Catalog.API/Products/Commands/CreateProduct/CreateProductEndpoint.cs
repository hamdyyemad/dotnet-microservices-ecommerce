using Catalog.API.Routes;

namespace Catalog.API.Products.Commands.CreateProduct
{
    internal record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, decimal Price);
    internal record CreateProductResponse(Guid Id);

    public class CreateProductEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app, string routePath)
        {
            app.MapPost(routePath, async (CreateProductRequest productRequest, ISender sender) =>
            {
                var command = productRequest.Adapt<CreateProductCommand>();
                var result = await sender.Send(command);
                var response = new CreateProductResponse(result.Id);
                return Results.Created($"/api/v1/products/{response.Id}", response);
            })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Product")
            .WithDescription("Create Product");
        }
    }
}
