# Exception Factory Pattern

This folder contains the exception handling implementation using the Factory Design Pattern for the Catalog API.

## Overview

Instead of directly instantiating exceptions throughout the codebase, we use an exception factory pattern that provides a centralized way to create exceptions. This approach offers better testability, maintainability, and separation of concerns.

## Visual Diagram Overview

### Architecture Overview

```mermaid
graph TB
    subgraph "API Layer"
        EP[Endpoint<br/>GetProductByIdEndpoint]
    end

    subgraph "Handler Layer"
        H[Handler<br/>GetProductByIdHandler]
    end

    subgraph "Dependency Injection"
        DI[Program.cs<br/>AddScoped IExceptionFactory]
    end

    subgraph "Exception Factory Pattern"
        IF[IExceptionFactory<br/>Interface]
        EF[ExceptionFactory<br/>Implementation]
    end

    subgraph "Exception Hierarchy"
        EX[Exception<br/>.NET Framework]
        CE[CatalogException<br/>Abstract Base]
        PNF[ProductNotFoundException<br/>Concrete Exception]
    end

    subgraph "Exception Handler"
        EH[Global Exception Handler<br/>catch CatalogException]
    end

    EP -->|Sends Query| H
    H -->|Injected via DI| IF
    IF -.->|Implemented by| EF
    EF -->|Creates| PNF
    DI -->|Registers| EF
    H -->|Throws| PNF
    PNF -->|Caught by| EH

    EX -->|Inherits| CE
    CE -->|Inherits| PNF

    style CE fill:#e1f5ff,stroke:#01579b,stroke-width:2px
    style PNF fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style EF fill:#fff3e0,stroke:#e65100,stroke-width:2px
    style IF fill:#fff3e0,stroke:#e65100,stroke-width:2px
```

### Sequence Diagram - Exception Flow

```mermaid
sequenceDiagram
    participant Client
    participant Endpoint as GetProductByIdEndpoint
    participant Handler as GetProductByIdHandler
    participant Factory as ExceptionFactory
    participant Exception as ProductNotFoundException
    participant GlobalHandler as Global Exception Handler
    participant Response as HTTP Response

    Client->>Endpoint: GET /products/{id}
    Endpoint->>Handler: Send(GetProductByIdQuery)

    Handler->>Handler: Query Database (Product = null)

    alt Product Not Found
        Handler->>Factory: CreateProductNotFoundException(message)
        Factory->>Exception: new ProductNotFoundException(message)
        Exception-->>Factory: Exception Instance
        Factory-->>Handler: ProductNotFoundException
        Handler->>Exception: throw
        Exception->>GlobalHandler: Exception Bubbles Up

        GlobalHandler->>GlobalHandler: catch (CatalogException)
        GlobalHandler->>Response: Results.Problem(400 Bad Request)
        Response-->>Client: 400 Bad Request + Error Message
    else Product Found
        Handler-->>Endpoint: GetProductByIdResult
        Endpoint->>Response: Results.Ok(product)
        Response-->>Client: 200 OK + Product Data
    end
```

### Exception Handling Flow Chart

```mermaid
flowchart TD
    Start([Request Received])
    Start --> Handler[Handler Processes Request]
    Handler --> Check{Business Logic Check}

    Check -->|Valid| Success[Return Success Result]
    Check -->|Invalid| UseFactory[Use ExceptionFactory]

    UseFactory --> Create[Create Specific Exception<br/>ProductNotFoundException]
    Create --> Throw[Throw Exception]
    Throw --> Bubble[Exception Bubbles Up]

    Bubble --> Catch{Catch Block Type}

    Catch -->|catch CatalogException| BusinessHandler[Business Exception Handler<br/>Returns 400 Bad Request]
    Catch -->|catch Exception| SystemHandler[System Exception Handler<br/>Returns 500 Internal Server Error]
    Catch -->|No Catch| FrameworkHandler[Framework Default Handler<br/>Returns 500]

    BusinessHandler --> End1([400 Response])
    SystemHandler --> End2([500 Response])
    FrameworkHandler --> End3([500 Response])
    Success --> End4([200 Response])

    style Create fill:#fff3e0,stroke:#e65100,stroke-width:2px
    style BusinessHandler fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style SystemHandler fill:#ffcdd2,stroke:#c62828,stroke-width:2px
```

### Component Relationships

```mermaid
graph LR
    subgraph "Factory Pattern"
        IF[IExceptionFactory]
        EF[ExceptionFactory]
        IF -.->|implements| EF
    end

    subgraph "Exception Classes"
        EX[Exception]
        CE[CatalogException<br/>abstract]
        PNF[ProductNotFoundException]
        EX -->|inherits| CE
        CE -->|inherits| PNF
    end

    subgraph "Usage"
        H[Handler]
        H -->|uses| IF
        IF -->|creates| PNF
    end

    subgraph "DI Container"
        DI[Program.cs]
        DI -->|registers| EF
    end

    style CE fill:#e1f5ff,stroke:#01579b,stroke-width:2px
    style IF fill:#fff3e0,stroke:#e65100,stroke-width:2px
```

### Key Points from Diagrams

1. **Dependency Injection**: `ExceptionFactory` is registered in `Program.cs` and injected into handlers
2. **Factory Pattern**: Handlers use `IExceptionFactory` interface, not concrete exception classes
3. **Exception Hierarchy**: All catalog exceptions inherit from `CatalogException`, which inherits from `Exception`
4. **Exception Handling**: Global handlers catch `CatalogException` for business errors (400) vs `Exception` for system errors (500)
5. **Flow**: Request → Handler → Factory → Exception → Global Handler → HTTP Response

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

## Class Diagram

```mermaid
classDiagram
    class Exception {
        <<.NET Framework>>
        +string Message
        +Exception InnerException
        +Exception()
        +Exception(string message)
        +Exception(string message, Exception innerException)
    }

    class CatalogException {
        <<abstract>>
        #CatalogException(string message)
        #CatalogException(string message, Exception innerException)
    }

    class ProductNotFoundException {
        +ProductNotFoundException(string message)
        +ProductNotFoundException(string message, Exception innerException)
    }

    class IExceptionFactory {
        <<interface>>
        +ProductNotFoundException CreateProductNotFoundException(string message)
    }

    class ExceptionFactory {
        +ProductNotFoundException CreateProductNotFoundException(string message)
    }

    Exception <|-- CatalogException : inherits
    CatalogException <|-- ProductNotFoundException : inherits
    IExceptionFactory <|.. ExceptionFactory : implements
    ExceptionFactory ..> ProductNotFoundException : creates
```

### Diagram Explanation

- **Exception**: Base .NET Framework class (shown for context)
- **CatalogException**: Abstract base class for all catalog domain exceptions (cannot be instantiated)
- **ProductNotFoundException**: Concrete exception class for product not found scenarios
- **IExceptionFactory**: Interface defining the contract for exception creation
- **ExceptionFactory**: Concrete implementation that creates exception instances

The diagram shows:

- **Inheritance**: `Exception` → `CatalogException` → `ProductNotFoundException`
- **Implementation**: `ExceptionFactory` implements `IExceptionFactory`
- **Dependency**: `ExceptionFactory` creates instances of `ProductNotFoundException`

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
