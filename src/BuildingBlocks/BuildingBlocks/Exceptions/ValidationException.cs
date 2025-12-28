namespace BuildingBlocks.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// Contains validation errors from FluentValidation.
/// </summary>
public class ValidationException : BaseException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base(new ExceptionInfo(
            "One or more validation errors occurred.",
            400,
            "Validation Error"))
    {
        Errors = errors;
    }

    public ValidationException(Dictionary<string, string[]> errors, Exception innerException) 
        : base(new ExceptionInfo(
            "One or more validation errors occurred.",
            400,
            "Validation Error"),
            innerException)
    {
        Errors = errors;
    }
}

