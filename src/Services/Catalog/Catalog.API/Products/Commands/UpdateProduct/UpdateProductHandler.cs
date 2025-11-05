using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Extensions;
using Catalog.API.Models;

namespace Catalog.API.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(Product Product) : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSucess);
    
    public class UpdateProductCommandHandler(
        IQuerySession querySession,
        IDocumentSession documentSession,
        IExceptionFactory exceptionFactory)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // Validate product exists
            await querySession.GetProductByIdOrThrowAsync(request.Product.Id, exceptionFactory, cancellationToken);
            
            // Update product
            documentSession.Update(request.Product);
            await documentSession.SaveChangesAsync(cancellationToken);
            
            return new UpdateProductResult(true);
        }
    }
}
