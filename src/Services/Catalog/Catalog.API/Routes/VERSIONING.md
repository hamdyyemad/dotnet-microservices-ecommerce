# API Versioning Guide

## Overview

The Catalog API supports versioning to allow backward compatibility when making breaking changes. All endpoints are versioned using URL path versioning (e.g., `/api/v1/products`).

## Current Version

- **Version 1**: `/api/v1/products/*` (Current/Default)

## How to Add a New Version

When you need to introduce breaking changes or new features, follow these steps:

### Step 1: Uncomment and Implement RegisterV2 in CatalogRoutes.cs

```csharp
private static void RegisterV2(IEndpointRouteBuilder app)
{
    var v2Group = app.MapGroup("/api/v2/products")
        .WithTags("Products V2")
        .WithSummary("Product Management API - Version 2");

    RegisterProductQueries(v2Group, "v2");
    RegisterProductCommands(v2Group, "v2");

    // Add any new V2-specific endpoints here
    // Example: RegisterNewV2Endpoint(v2Group, "v2");
}
```

### Step 2: Call RegisterV2 in AddRoutes Method

```csharp
public void AddRoutes(IEndpointRouteBuilder app)
{
    RegisterV1(app);
    RegisterV2(app); // Uncomment this line
}
```

### Step 3: Update Endpoint Documentation

Update the endpoint strings documentation at the top of `CatalogRoutes.cs`:

```csharp
// Version 2 (Current):
// GET    /api/v2/products                    - Get all products (v2)
// GET    /api/v2/products/{id}               - Get product by ID (v2)
// ... (add new endpoints)
```

### Step 4: Create Version-Specific Endpoints (If Needed)

If you need different behavior for V2, you can:

1. **Option A**: Create new endpoint classes (e.g., `GetProductsV2Endpoint`)
2. **Option B**: Modify existing endpoints to handle version-specific logic
3. **Option C**: Reuse existing endpoints if behavior is the same

### Step 5: Update Response Locations (If Needed)

If endpoints return `Created` responses with location headers, update them to use the correct version:

```csharp
return Results.Created($"/api/v2/products/{response.Id}", response);
```

## Versioning Strategy

- **URL Path Versioning**: Versions are in the URL path (`/api/v1/`, `/api/v2/`)
- **Backward Compatibility**: Old versions remain active alongside new versions
- **Gradual Migration**: Clients can migrate to new versions at their own pace
- **Deprecation**: Mark old versions as deprecated in Swagger/OpenAPI documentation

## Best Practices

1. **Don't break existing versions**: Keep V1 working when adding V2
2. **Document breaking changes**: Clearly document what changed in the new version
3. **Provide migration guides**: Help clients understand how to migrate
4. **Set deprecation timelines**: Give clients time to migrate before removing old versions
5. **Version constants**: Use route path constants (already implemented) for consistency

## Example: Adding Version 2

```csharp
// In CatalogRoutes.cs

// 1. Add V2 route path constants (if different from V1)
private const string GetProductsV2Path = "/"; // Same as V1

// 2. Implement RegisterV2
private static void RegisterV2(IEndpointRouteBuilder app)
{
    var v2Group = app.MapGroup("/api/v2/products")
        .WithTags("Products V2")
        .WithSummary("Product Management API - Version 2");

    // Reuse existing endpoints or create new ones
    new GetProductsEndpoint().AddRoutes(v2Group, GetProductsV2Path);
    // ... register other endpoints
}

// 3. Call it in AddRoutes
public void AddRoutes(IEndpointRouteBuilder app)
{
    RegisterV1(app);
    RegisterV2(app); // Enable V2
}
```

## Testing Versions

Test both versions to ensure:

- V1 endpoints still work correctly
- V2 endpoints work with new behavior
- No breaking changes affect V1 clients
- Swagger/OpenAPI shows both versions correctly
