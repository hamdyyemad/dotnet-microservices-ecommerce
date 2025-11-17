using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Models;
using Catalog.API.Products.Commands.UpdateProduct;
using Catalog.API.Products.Queries.GetProductById;

namespace Catalog.API.Products.Observers
{
    public record UpdateProductObserverCommand(Product Product) : ICommand<bool>;
    public class UpdateProductObserver(ISender sender)
        : ICommandHandler<UpdateProductObserverCommand, bool>
    {
        public async Task<bool> Handle(UpdateProductObserverCommand request, CancellationToken cancellationToken)
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
