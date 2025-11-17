using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Extensions;
using Catalog.API.Models;

namespace Catalog.API.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(Product Product) : ICommand<bool>;
    
    public class UpdateProductCommandHandler(
        IDocumentSession documentSession)
        : ICommandHandler<UpdateProductCommand, bool>
    {
        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // Validation of Product Existence happens in the UpdateProductObserver 
            
            // Update product
            documentSession.Update(request.Product);
            await documentSession.SaveChangesAsync(cancellationToken);
            
            return true;
        }
    }
}
