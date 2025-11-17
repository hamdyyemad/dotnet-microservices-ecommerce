using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Models;
using Catalog.API.Products.Commands.UpdateProduct;
using Catalog.API.Products.Queries.GetProductById;

namespace Catalog.API.Products.Orchestrator
{
    public record UpdateProductOrchestratorCommand(Product Product) : ICommand<bool>;
    public class UpdateProductOrchestrator(ISender sender)
        : ICommandHandler<UpdateProductOrchestratorCommand, bool>
    {
        public async Task<bool> Handle(UpdateProductOrchestratorCommand request, CancellationToken cancellationToken)
        {
            var incomingProductId = request.Product.Id;

            // 1. Get Product by Id and Check if it exist first
            await sender.Send(new GetProductByIdQuery(incomingProductId));

            // We don't have to check existence of the product because we've done it inside the handler itself

            // 2. Update the Product after checking that it exists
            await sender.Send(new UpdateProductCommand(request.Product));

            return true;
        }
    }
}
