using BuildingBlocks.CQRS;
using Catalog.API.Models;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)

    : ICommand<CreateProductResult>;

    public record CreateProductResult(Guid Id);

    internal class CreateProductCommandHandler 
        (IDocumentStore store) 
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle
            (
                CreateProductCommand command, 
                CancellationToken cancellationToken
            )
        {
            // Step1. Create Product Entity from Command Object
            Product product = new Product 
            {
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price,
            };

            // Step2. Save to DB using a session
            using var session = store.LightweightSession();
            session.Store(product);
            await session.SaveChangesAsync(cancellationToken);

            // Step3. Return CreateProductResult
            return new CreateProductResult(product.Id);
        }
    }
}
