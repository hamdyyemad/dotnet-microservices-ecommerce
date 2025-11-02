using Catalog.API.Exceptions;
using Catalog.API.Models;

namespace Catalog.API.Extensions;

/// <summary>
/// Extension methods for IQuerySession to add reusable functionality
/// </summary>
public static class SessionExtensions
{
    /// <summary>
    /// Extension method that loads a product by ID and throws an exception if not found.
    /// 
    /// ============================================
    /// HOW EXTENSION METHODS WORK - THE MAGIC:
    /// ============================================
    /// 
    /// 1. The keyword "this" before the first parameter makes this an extension method
    /// 2. "this IQuerySession session" means this method can be called on any IQuerySession object
    /// 3. When you write: querySession.GetProductByIdOrThrowAsync(...)
    ///    The compiler AUTOMATICALLY translates it to:
    ///    SessionExtensions.GetProductByIdOrThrowAsync(querySession, ...)
    /// 4. You DON'T need to write SessionExtensions. - the compiler does it for you!
    /// 5. Required: You MUST have "using Catalog.API.Extensions;" in your file
    /// 
    /// REAL EXAMPLE FROM YOUR CODE:
    /// -----------------------------
    /// In UpdateProductHandler.cs, you write:
    ///   await querySession.GetProductByIdOrThrowAsync(...)
    /// 
    /// The compiler automatically converts this to:
    ///   await SessionExtensions.GetProductByIdOrThrowAsync(querySession, ...)
    /// 
    /// But you NEVER see this - it happens behind the scenes!
    /// 
    /// WHY IT WORKS:
    /// -------------
    /// - Extension methods are just "syntactic sugar" - a convenient way to write code
    /// - The "this" keyword tells C#: "This method extends the IQuerySession type"
    /// - When you use it, C# looks for extension methods in imported namespaces
    /// - It finds this method and calls it as if it belonged to IQuerySession
    /// 
    /// EXAMPLE USAGE:
    /// Before (duplicated code in each handler):
    ///   var product = await session.LoadAsync<Product>(id, cancellationToken);
    ///   if (product is null) {
    ///       throw exceptionFactory.CreateProductNotFoundException(id);
    ///   }
    /// 
    /// After (reusable extension method):
    ///   var product = await session.GetProductByIdOrThrowAsync(id, exceptionFactory, cancellationToken);
    /// </summary>
    public static async Task<Product> GetProductByIdOrThrowAsync(
        this IQuerySession session,  // <-- "this" keyword makes it an extension method
        Guid productId,
        IExceptionFactory exceptionFactory,
        CancellationToken cancellationToken = default)
    {
        // Load the product from the database
        var product = await session.LoadAsync<Product>(productId, cancellationToken);
        
        // If product doesn't exist, throw an exception
        if (product is null)
        {
            throw exceptionFactory.CreateProductNotFoundException(productId);
        }
        
        // Return the product if found
        return product;
    }
}

