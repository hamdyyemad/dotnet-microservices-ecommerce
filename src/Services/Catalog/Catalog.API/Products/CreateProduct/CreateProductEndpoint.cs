namespace Catalog.API.Products.CreateProduct
{
    internal record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, decimal Price);
    internal record CreateProductResponse(Guid Id);

    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest productRequest, ISender sender) =>
            {
                var command = productRequest.Adapt<CreateProductCommand>();
                var result = await sender.Send(command);
                var response = new CreateProductResponse(result.Id);
                return Results.Created($"/products/{response.Id}", response);
            })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Product")
            .WithDescription("Create Product");
        }
    }
}
