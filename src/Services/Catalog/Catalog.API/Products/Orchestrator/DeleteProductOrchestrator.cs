using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Models;
using Catalog.API.Products.Commands.DeleteProduct;
using Catalog.API.Products.Queries.GetProductById;

namespace Catalog.API.Products.Orchestrator
{
    public record DeleteProductOrchestratorCommand(Guid Id) : ICommand<bool>;
    public class DeleteProductOrchestrator(ISender sender)
        : ICommandHandler<DeleteProductOrchestratorCommand, bool>
    {
        public async Task<bool> Handle(DeleteProductOrchestratorCommand request, CancellationToken cancellationToken)
        {
            var incomingProductId = request.Id;

            // 1. Get Product by Id and Check if it exist first
            await sender.Send(new GetProductByIdQuery(incomingProductId));

            // We don't have to check existence of the product because we've done it inside the handler itself

            // 2. Update the Product after checking that it exists
            await sender.Send(new DeleteProductCommand(incomingProductId));

            return true;
        }
    }
}
