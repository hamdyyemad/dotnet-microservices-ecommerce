using BuildingBlocks.Exceptions;

namespace Catalog.API.Exceptions;

/// <summary>
/// Base exception class for all catalog-related exceptions.
/// Inherits from BuildingBlocks.BaseException to provide domain-specific exception handling.
/// All Catalog API domain exceptions should inherit from this class.
/// </summary>
public abstract class CatalogException : BaseException
{
    protected CatalogException(ExceptionInfo info) : base(info)
    {
    }

    protected CatalogException(ExceptionInfo info, Exception innerException) 
        : base(info, innerException)
    {
    }
}

