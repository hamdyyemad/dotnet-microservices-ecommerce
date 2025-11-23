namespace BuildingBlocks.Exceptions;

/// <summary>
/// Base exception class for all microservice exceptions.
/// Contains HTTP response information (status code and title) along with the error message.
/// All domain-specific exceptions should inherit from this class.
/// </summary>
public abstract class BaseException : Exception
{
    public int StatusCode { get; }
    public string Title { get; }

    protected BaseException(ExceptionInfo info) : base(info.Message)
    {
        StatusCode = info.StatusCode;
        Title = info.Title;
    }

    protected BaseException(ExceptionInfo info, Exception innerException) 
        : base(info.Message, innerException)
    {
        StatusCode = info.StatusCode;
        Title = info.Title;
    }
}

