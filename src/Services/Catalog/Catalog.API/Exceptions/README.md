# Exception Factory Pattern

This folder contains the exception handling implementation using the Factory Design Pattern for the Catalog API.

## Overview

Instead of directly instantiating exceptions throughout the codebase, we use an exception factory pattern that provides a centralized way to create exceptions. This approach offers better testability, maintainability, and separation of concerns.

**Note**: Common exception infrastructure (`BaseException` and `ExceptionInfo`) is located in the `BuildingBlocks` project and shared across all microservices. This folder contains only Catalog API domain-specific exceptions.

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
        EM[ExceptionMessages<br/>Returns ExceptionInfo]
        EI[ExceptionInfo<br/>Record<br/>BuildingBlocks]
    end

    subgraph "Exception Hierarchy"
        EX[Exception<br/>.NET Framework]
        BE[BaseException<br/>BuildingBlocks]
        CE[CatalogException<br/>Domain Base Class]
        PNF[ProductNotFoundException<br/>Concrete Exception]
    end

    subgraph "Exception Handler"
        EH[Global Exception Handler<br/>catch BaseException]
    end

    EP -->|Sends Query| H
    H -->|Injected via DI| IF
    IF -.->|Implemented by| EF
    EF -->|Uses| EM
    EM -->|Returns| EI
    EI -->|Contains| PNF
    EF -->|Creates| PNF
    DI -->|Registers| EF
    H -->|Throws| PNF
    PNF -->|Caught by| EH

    EX -->|Inherits| BE
    BE -->|Inherits| CE
    CE -->|Inherits| PNF

    style CE fill:#e1f5ff,stroke:#01579b,stroke-width:2px
    style PNF fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style EF fill:#fff3e0,stroke:#e65100,stroke-width:2px
    style IF fill:#fff3e0,stroke:#e65100,stroke-width:2px
    style EM fill:#f3e5f5,stroke:#6a1b9a,stroke-width:2px
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
        Handler->>Factory: CreateProductNotFoundException(productId)
        Factory->>ExceptionMessages: NotFound(productId)
        ExceptionMessages-->>Factory: ExceptionInfo (message, statusCode, title)
        Factory->>Exception: new ProductNotFoundException(ExceptionInfo)
        Exception-->>Factory: Exception Instance
        Factory-->>Handler: ProductNotFoundException
        Handler->>Exception: throw
        Exception->>GlobalHandler: Exception Bubbles Up

        GlobalHandler->>GlobalHandler: catch (BaseException)
        GlobalHandler->>Response: Uses exception.StatusCode and exception.Title
        Response-->>Client: 404 Not Found + Error Message
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

    UseFactory --> GetExceptionInfo[Get ExceptionInfo<br/>from ExceptionMessages]
    GetExceptionInfo --> Create[Create Specific Exception<br/>ProductNotFoundException]
    Create --> Throw[Throw Exception]
    Throw --> Bubble[Exception Bubbles Up]

    Bubble --> Catch{Catch Block Type}

    Catch -->|catch BaseException| BusinessHandler[Business Exception Handler<br/>Uses exception.StatusCode & Title]
    Catch -->|catch Exception| SystemHandler[System Exception Handler<br/>Returns 500 Internal Server Error]
    Catch -->|No Catch| FrameworkHandler[Framework Default Handler<br/>Returns 500]

    BusinessHandler --> End1([Uses exception.StatusCode])
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
        EM[ExceptionMessages]
        EI[ExceptionInfo]
        IF -.->|implements| EF
        EF -->|uses| EM
        EM -->|returns| EI
    end

    subgraph "Exception Classes"
        EX[Exception<br/>.NET Framework]
        BE[BaseException<br/>BuildingBlocks]
        CE[CatalogException<br/>Domain Base]
        PNF[ProductNotFoundException]
        EX -->|inherits| BE
        BE -->|inherits| CE
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
    style EM fill:#f3e5f5,stroke:#6a1b9a,stroke-width:2px
```

### Key Points from Diagrams

1. **Dependency Injection**: `ExceptionFactory` is registered in `Program.cs` and injected into handlers
2. **Factory Pattern**: Handlers use `IExceptionFactory` interface, not concrete exception classes
3. **Unified Exception Data**: `ExceptionMessages` class centralizes all exception information (message, status code, title) for consistency
4. **Context-Based Creation**: Factory methods accept context (e.g., product ID) instead of raw messages
5. **Exception Hierarchy**: All catalog exceptions inherit from `CatalogException`, which inherits from `BaseException` (BuildingBlocks), which inherits from `Exception`
6. **Exception Handling**: Global middleware catches `BaseException` (which includes `CatalogException`) and uses its `StatusCode` and `Title` properties for HTTP responses
7. **Microservices Architecture**: Common infrastructure (`BaseException`, `ExceptionInfo`) is in BuildingBlocks and shared across all microservices
8. **Flow**: Request → Handler → Factory → ExceptionMessages (returns ExceptionInfo) → Exception → Global Handler → HTTP Response
9. **Simplified Structure**: `ExceptionInfo` record reduces boilerplate by bundling message, status code, and title together

## Folder Structure

The Exceptions folder is organized into logical subfolders for better maintainability:

```
Exceptions/
├── Base/
│   └── CatalogException.cs          # Domain-specific base exception (inherits from BuildingBlocks.BaseException)
├── Types/
│   └── ProductNotFoundException.cs   # Specific exception types
├── Messages/
│   └── ExceptionMessages.cs         # Returns ExceptionInfo (message, statusCode, title)
├── Factory/
│   ├── IExceptionFactory.cs         # Factory interface
│   └── ExceptionFactory.cs          # Factory implementation
└── README.md                         # Documentation

BuildingBlocks/BuildingBlocks/Exceptions/
├── BaseException.cs                  # Common base exception for all microservices
└── ExceptionInfo.cs                  # Record containing message, statusCode, and title (shared)
```

### Folder Organization

**Catalog.API/Exceptions/** (Domain-Specific):

- **`Base/`**: Contains the domain-specific base exception class (`CatalogException`) that inherits from `BuildingBlocks.BaseException`
- **`Types/`**: Contains specific exception types (e.g., `ProductNotFoundException`)
- **`Messages/`**: Contains centralized exception definitions that return `ExceptionInfo` (message, status code, and title)
- **`Factory/`**: Contains the factory interface and implementation

**BuildingBlocks/BuildingBlocks/Exceptions/** (Shared Infrastructure):

- **`BaseException.cs`**: Common base exception class for all microservices (contains HTTP response information)
- **`ExceptionInfo.cs`**: Record containing message, status code, and title for exceptions (shared across all microservices)

## Structure

### `BuildingBlocks.Exceptions.ExceptionInfo`

- **Record** (located in BuildingBlocks) containing all information needed for an exception and its HTTP response
- Contains: `Message`, `StatusCode`, and `Title`
- Used by `ExceptionMessages` to return complete exception information
- Simplifies exception creation by bundling all related data together
- **Shared across all microservices** for consistency

### `BuildingBlocks.Exceptions.BaseException`

- **Base class** (located in BuildingBlocks) for all microservice exceptions
- Inherits from `Exception`
- Contains `StatusCode` and `Title` properties for HTTP responses
- Takes `ExceptionInfo` in constructors to initialize all properties
- **Shared across all microservices** for consistent exception handling

### `Base/CatalogException.cs`

- **Domain-specific base class** for all catalog-related exceptions
- Located in `Base/` folder for domain-specific base exception types
- Inherits from `BuildingBlocks.Exceptions.BaseException`
- Provides a domain-specific layer for Catalog API exceptions
- All Catalog API exceptions should inherit from this class
- Takes `ExceptionInfo` in constructors and passes to base class

#### Why Do We Need CatalogException?

Having a domain-specific `CatalogException` class (in addition to `BuildingBlocks.BaseException`) provides several important benefits:

1. **Domain-Specific Exception Handling**: You can catch all catalog-related exceptions at once in domain-specific handlers if needed:

   ```csharp
   catch (CatalogException ex)
   {
       // Handle all catalog business exceptions (ProductNotFound, ProductAlreadyExists, etc.)
       // Uses the StatusCode and Title from the exception itself
       return Results.Problem(
           detail: ex.Message,
           statusCode: ex.StatusCode,
           title: ex.Title);
   }
   catch (BaseException ex)
   {
       // Handle other microservice exceptions
       return Results.Problem(
           detail: ex.Message,
           statusCode: ex.StatusCode,
           title: ex.Title);
   }
   catch (Exception ex)
   {
       // Handle system/unexpected exceptions differently (500 error)
       return Results.Problem("An error occurred", statusCode: 500);
   }
   ```

   **Note**: The global middleware catches `BaseException`, which includes `CatalogException` since it inherits from it.

2. **Separation of Concerns**: Provides a clear hierarchy:

   - **Domain Exceptions** (CatalogException) → Catalog API specific business exceptions
   - **Common Exceptions** (BaseException) → Shared across all microservices
   - **System/Technical Exceptions** (Exception) → Return 500 Internal Server Error

3. **Domain Identification**: Makes it immediately clear which exceptions originate from the Catalog API domain vs other services

4. **Future Extensibility**: Allows you to add Catalog-specific behavior to all catalog exceptions later (domain-specific error codes, catalog-specific logging, etc.) without modifying the shared `BaseException` class

5. **Microservices Architecture**: Keeps domain-specific logic in the service while sharing common infrastructure through BuildingBlocks

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

    class ExceptionInfo {
        <<record>>
        <<BuildingBlocks>>
        +string Message
        +int StatusCode
        +string Title
    }

    class BaseException {
        <<BuildingBlocks>>
        +int StatusCode
        +string Title
        +BaseException(ExceptionInfo info)
        +BaseException(ExceptionInfo info, Exception innerException)
    }

    class CatalogException {
        +CatalogException(ExceptionInfo info)
        +CatalogException(ExceptionInfo info, Exception innerException)
    }

    class ProductNotFoundException {
        +ProductNotFoundException(ExceptionInfo info)
        +ProductNotFoundException(ExceptionInfo info, Exception innerException)
    }

    class ExceptionMessages {
        <<static>>
        +Product NotFound(Guid productId) ExceptionInfo
        +Product NotFound(string identifier) ExceptionInfo
    }

    class IExceptionFactory {
        <<interface>>
        +ProductNotFoundException CreateProductNotFoundException(Guid productId)
    }

    class ExceptionFactory {
        +ProductNotFoundException CreateProductNotFoundException(Guid productId)
    }

    Exception <|-- BaseException : "BuildingBlocks"
    BaseException <|-- CatalogException : "Domain-specific"
    CatalogException <|-- ProductNotFoundException : inherits
    IExceptionFactory <|.. ExceptionFactory : implements
    ExceptionFactory ..> ExceptionMessages : uses
    ExceptionFactory ..> ProductNotFoundException : creates
```

### Diagram Explanation

- **Exception**: Base .NET Framework class (shown for context)
- **BaseException**: Common base exception class in BuildingBlocks, shared across all microservices
- **ExceptionInfo**: Record (in BuildingBlocks) containing message, status code, and title for exceptions
- **CatalogException**: Domain-specific base class for all catalog domain exceptions (inherits from `BaseException`)
- **ProductNotFoundException**: Concrete exception class for product not found scenarios
- **ExceptionMessages**: Static class returning `ExceptionInfo` with complete exception data
- **IExceptionFactory**: Interface defining the contract for exception creation
- **ExceptionFactory**: Concrete implementation that creates exception instances

The diagram shows:

- **Inheritance**: `Exception` → `BaseException` (BuildingBlocks) → `CatalogException` (Domain) → `ProductNotFoundException`
- **Implementation**: `ExceptionFactory` implements `IExceptionFactory`
- **Data Flow**: `ExceptionMessages` returns `ExceptionInfo` → `ExceptionFactory` uses it → Creates exceptions
- **Dependency**: `ExceptionFactory` creates instances using `ExceptionInfo` from `ExceptionMessages`

### `Types/ProductNotFoundException.cs`

- **Specific exception** for when a product cannot be found
- Located in `Types/` folder for concrete exception types
- Inherits from `CatalogException`
- Takes `ExceptionInfo` in constructors
- Used when querying for a product that doesn't exist in the database

### `Messages/ExceptionMessages.cs`

- **Centralized exception definitions** for all catalog exceptions
- Located in `Messages/` folder for exception definitions
- Returns `ExceptionInfo` containing message, status code, and title
- Ensures consistent and unified error responses across the application
- Contains static methods that generate complete exception information from context (e.g., product ID)
- All exception metadata (message, status code, title) is defined in one place
- Example: `ExceptionMessages.Product.NotFound(productId)` → Returns `ExceptionInfo` with:
  - Message: `"Product with ID '{productId}' was not found."`
  - StatusCode: `404`
  - Title: `"Product Not Found"`

### `Factory/IExceptionFactory.cs`

- **Interface** defining the contract for exception creation
- Located in `Factory/` folder for factory-related interfaces
- Contains factory methods for creating different exception types
- Methods accept context (e.g., IDs) rather than raw messages for unification
- Currently includes:
  - `CreateProductNotFoundException(Guid productId)`

### `Factory/ExceptionFactory.cs`

- **Concrete implementation** of `IExceptionFactory`
- Located in `Factory/` folder for factory implementations
- Implements all factory methods to create appropriate exception instances
- Gets `ExceptionInfo` from `ExceptionMessages` and passes it to exception constructors
- Simple and clean - just retrieves exception info and creates the exception
- Registered as a scoped service in dependency injection container

## Usage

### In Handlers/Services

Instead of:

```csharp
// ❌ Bad: Inconsistent messages, no centralization
throw new ProductNotFoundException("Product doesn't exist.");
throw new ProductNotFoundException("Product not found.");
throw new ProductNotFoundException("Could not find product.");
```

Use:

```csharp
// ✅ Good: Unified message, passes context (product ID)
throw exceptionFactory.CreateProductNotFoundException(productId);
```

**Benefits of this approach:**

- **Unified Responses**: All `ProductNotFoundException` instances use the same message, status code, and title
- **Context-Aware**: Messages include relevant context (e.g., product ID) automatically
- **Maintainable**: Change message, status code, or title in one place (`ExceptionMessages.cs`)
- **Consistent**: No more variation in exception responses across developers
- **Complete Information**: Exception contains all HTTP response data (status code, title, message)

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
4. **Unified Responses**: All exception data (message, status code, title) is centralized and consistent
5. **Context-Aware**: Messages automatically include relevant context (e.g., IDs, identifiers)
6. **Maintainability**: Change exception responses in one place (`ExceptionMessages.cs`)
7. **Extensibility**: Easy to add new exception types by extending the factory and messages
8. **Type Safety**: Factory methods ensure correct exception types are used
9. **Less Boilerplate**: Simple structure with `ExceptionInfo` record reducing code duplication
10. **HTTP-Aware**: Exceptions contain all HTTP response information (status code, title)

## Adding New Exceptions

To add a new exception type:

1. Add an exception definition method to `Messages/ExceptionMessages.cs`:

   ```csharp
   public static class Product
   {
       public static ExceptionInfo AlreadyExists(string productName) => new(
           Message: $"Product with name '{productName}' already exists.",
           StatusCode: StatusCodes.Status409Conflict,
           Title: "Product Already Exists"
       );
   }
   ```

2. Create a new exception class inheriting from `CatalogException` in the `Types/` folder:

   ```csharp
   // File: Types/ProductAlreadyExistsException.cs
   namespace Catalog.API.Exceptions;

   public class ProductAlreadyExistsException : CatalogException
   {
       public ProductAlreadyExistsException(ExceptionInfo info) : base(info) { }

       public ProductAlreadyExistsException(ExceptionInfo info, Exception innerException)
           : base(info, innerException) { }
   }
   ```

3. Add a factory method to `Factory/IExceptionFactory.cs`:

   ```csharp
   ProductAlreadyExistsException CreateProductAlreadyExistsException(string productName);
   ```

4. Implement the method in `Factory/ExceptionFactory.cs`:

   ```csharp
   public ProductAlreadyExistsException CreateProductAlreadyExistsException(string productName)
   {
       var exceptionInfo = ExceptionMessages.Product.AlreadyExists(productName);
       return new ProductAlreadyExistsException(exceptionInfo);
   }
   ```

5. Use it in your handlers:
   ```csharp
   throw exceptionFactory.CreateProductAlreadyExistsException(product.Name);
   ```

**Note**: The exception will automatically return the correct HTTP status code (409 Conflict) and title when caught by the global exception handler middleware.
