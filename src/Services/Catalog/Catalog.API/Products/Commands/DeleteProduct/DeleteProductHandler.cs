using BuildingBlocks.CQRS;
using Catalog.API.Models;

namespace Catalog.API.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(Guid Id) : ICommand<bool>;
    
    public class DeleteProductCommandHandler(
        IDocumentSession documentSession)
        : ICommandHandler<DeleteProductCommand, bool>
    {
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            // Validation of Product Existence happens in the DeleteProductOrchestrator 
            
            // Delete product by ID - must specify the document type
            documentSession.Delete<Product>(request.Id);
            await documentSession.SaveChangesAsync(cancellationToken);
            
            return true;
        }
    }
}
