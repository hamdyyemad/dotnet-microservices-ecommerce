using FluentValidation;
using MediatR;


namespace BuildingBlocks.Behaviors;

/// <summary>
/// Pipeline behavior that validates requests using FluentValidation.
/// Automatically validates any request that has a corresponding validator registered.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // If no validators are registered for this request type, skip validation
        if (_validators.Any())
        {
            // Create validation context
            var context = new ValidationContext<TRequest>(request);

            // Run all validators
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Collect all failures
            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            // If there are validation failures, throw ValidationException
            if (failures.Any())
            {
                var errors = failures
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

                throw new BuildingBlocks.Exceptions.ValidationException(errors);
            }        
        }
        // Validation passed, continue to next handler
        return await next();
    }
}
