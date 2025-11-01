# Exception Factory Pattern

This folder contains the exception handling implementation using the Factory Design Pattern for the Catalog API.

## Overview

Instead of directly instantiating exceptions throughout the codebase, we use an exception factory pattern that provides a centralized way to create exceptions. This approach offers better testability, maintainability, and separation of concerns.

## Structure

### `CatalogException.cs`

- **Abstract base class** for all catalog-related exceptions
- Inherits from `Exception`
- Provides constructors for message-only and message with inner exception scenarios
- Cannot be instantiated directly - must be inherited by specific exception types

#### Why Do We Need CatalogException?

Having a base `CatalogException` class provides several important benefits:

1. **Centralized Exception Handling**: You can catch all catalog-related exceptions at once in global exception handlers:

   ```csharp
   catch (CatalogException ex)
   {
       // Handle all catalog business exceptions (ProductNotFound, ProductAlreadyExists, etc.)
       return Results.Problem(ex.Message, statusCode: 400);
   }
   catch (Exception ex)
   {
       // Handle system/unexpected exceptions differently (500 error)
       return Results.Problem("An error occurred", statusCode: 500);
   }
   ```

2. **Separation of Concerns**: Distinguishes between:

   - **Business/Validation Exceptions** (CatalogException) → Usually return 400 Bad Request
   - **System/Technical Exceptions** (Exception) → Usually return 500 Internal Server Error

3. **Consistent Structure**: All catalog exceptions share the same base structure and can be extended with common properties if needed in the future (e.g., ErrorCode, Timestamp)

4. **API Identification**: Makes it immediately clear which exceptions originate from the Catalog API domain vs other services or system exceptions

5. **Future Extensibility**: Allows you to add common behavior to all catalog exceptions later (logging, error codes, etc.) without modifying each individual exception class

### `ProductNotFoundException.cs`

- **Specific exception** for when a product cannot be found
- Inherits from `CatalogException`
- Used when querying for a product that doesn't exist in the database

### `IExceptionFactory.cs`

- **Interface** defining the contract for exception creation
- Contains factory methods for creating different exception types
- Currently includes:
  - `CreateProductNotFoundException(string message)`

### `ExceptionFactory.cs`

- **Concrete implementation** of `IExceptionFactory`
- Implements all factory methods to create appropriate exception instances
- Registered as a scoped service in dependency injection container

## Usage

### In Handlers/Services

Instead of:

```csharp
throw new ProductNotFoundException("Product doesn't exist.");
```

Use:

```csharp
throw exceptionFactory.CreateProductNotFoundException("Product doesn't exist.");
```

### Dependency Injection

The factory is registered in `Program.cs`:

```csharp
builder.Services.AddScoped<IExceptionFactory, ExceptionFactory>();
```

Inject `IExceptionFactory` into your handlers or services via constructor injection:

```csharp
internal class GetProductByIdHandlerQuery(
    IDocumentStore store,
    IExceptionFactory exceptionFactory)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    // Use exceptionFactory.CreateProductNotFoundException(...)
}
```

## Benefits

1. **Decoupling**: Handlers don't need to know about specific exception types
2. **Testability**: Easy to mock `IExceptionFactory` in unit tests
3. **Centralized Logic**: Exception creation logic is in one place
4. **Extensibility**: Easy to add new exception types by extending the factory
5. **Type Safety**: Factory methods ensure correct exception types are used

## Adding New Exceptions

To add a new exception type:

1. Create a new exception class inheriting from `CatalogException`:

   ```csharp
   public class ProductAlreadyExistsException : CatalogException
   {
       public ProductAlreadyExistsException(string message) : base(message) { }
   }
   ```

2. Add a factory method to `IExceptionFactory`:

   ```csharp
   ProductAlreadyExistsException CreateProductAlreadyExistsException(string message);
   ```

3. Implement the method in `ExceptionFactory`:

   ```csharp
   public ProductAlreadyExistsException CreateProductAlreadyExistsException(string message)
   {
       return new ProductAlreadyExistsException(message);
   }
   ```

4. Use it in your handlers:
   ```csharp
   throw exceptionFactory.CreateProductAlreadyExistsException("Product already exists.");
   ```
