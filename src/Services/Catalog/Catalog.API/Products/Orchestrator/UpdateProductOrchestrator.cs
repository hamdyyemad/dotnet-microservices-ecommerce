using BuildingBlocks.CQRS;
using Catalog.API.Exceptions;
using Catalog.API.Models;
using Catalog.API.Products.Commands.UpdateProduct;
using Catalog.API.Products.Queries.GetProductById;
using FluentValidation;

namespace Catalog.API.Products.Orchestrator
{
    public record UpdateProductOrchestratorCommand(Product Product) : ICommand<bool>;
    
    public class UpdateProductValidator : AbstractValidator<UpdateProductOrchestratorCommand>
    {
        public UpdateProductValidator() {
            RuleFor(x => x.Product.Id)
                .NotEmpty().WithMessage("Product Id is required.");

            RuleFor(x => x.Product.Name)
                .NotEmpty().WithMessage("Product Name is required.")
                .MinimumLength(3).WithMessage("Product Name must be at least 3 characters.")
                .MaximumLength(200).WithMessage("Product Name must not exceed 200 characters.");

            RuleFor(x => x.Product.Category)
                .NotNull().WithMessage("Product Category is required.")
                .NotEmpty().WithMessage("Product must have at least one category.");

            RuleFor(x => x.Product.Description)
                .NotEmpty().WithMessage("Product Description is required.")
                .MaximumLength(1000).WithMessage("Product Description must not exceed 1000 characters.");

            RuleFor(x => x.Product.ImageFile)
                .NotEmpty().WithMessage("Product ImageFile is required.");

            RuleFor(x => x.Product.Price)
                .GreaterThan(0).WithMessage("Product Price must be greater than 0.");
        }
    }

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
