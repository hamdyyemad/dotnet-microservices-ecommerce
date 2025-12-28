using Catalog.API.Products.Commands.CreateProduct;
using Catalog.API.Products.Commands.DeleteProduct;
using Catalog.API.Products.Commands.UpdateProduct;
using Catalog.API.Products.Queries.GetProductById;
using Catalog.API.Products.Queries.GetProducts;
using Catalog.API.Products.Queries.GetProductsByCategory;

namespace Catalog.API.Routes;

/// <summary>
/// Central entry point for all Catalog API routes with versioning support.
/// This class serves as a registry and documentation of all available endpoints.
/// Delegates route registration to individual endpoint classes.
/// 
/// To add a new version:
/// 1. Create a new RegisterV{X} method (e.g., RegisterV2)
/// 2. Copy the endpoint registrations and update the version number
/// 3. Call the new method in AddRoutes
/// 4. Update the endpoint strings documentation below
/// </summary>
public class CatalogRoutes : ICarterModule
{
    // ============================================
    // AVAILABLE ENDPOINTS - Catalog API Routes
    // ============================================
    // 
    // Version 1 (Current):
    // GET    /api/v1/products                     - Get all products
    // GET    /api/v1/products/{id}                - Get product by ID
    // GET    /api/v1/products/category/{category} - Get products by category
    // POST   /api/v1/products                     - Create a new product
    // PUT    /api/v1/products                     - Update an existing product
    // DELETE /api/v1/products                     - Delete an existing product
    //
    // Version 2 (Future - Example):
    // GET    /api/v2/products                     - Get all products (v2)
    // GET    /api/v2/products/{id}                - Get product by ID (v2)
    // ... (add new versions as needed)
    //
    // ============================================

    // Route path constants for each endpoint (relative to route group)
    // Note: These are relative paths that will be combined with the route group prefix
    private const string GetProductsPath = "products";
    private const string GetProductByIdPath = "products/{id:guid}";
    private const string GetProductsByCategoryPath = "products/category/{category}";
    private const string CreateProductPath = "products";
    private const string UpdateProductPath = "products";
    private const string DeleteProductPath = "products/{id:guid}";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Register Version 1 (Current)
        RegisterV1(app);

        // To add Version 2 in the future, uncomment and implement:
        // RegisterV2(app);
    }

    /// <summary>
    /// Registers all Version 1 endpoints.
    /// </summary>
    private static void RegisterV1(IEndpointRouteBuilder app)
    {
        const string version = "v1";

        var v1Group = app.MapGroup($"/api/{version}")
            .WithTags($"Catalog API {version}")
            .WithSummary($"Catalog API - Version {version}");

        // Simple health check endpoint (no dependencies)
        v1Group.MapGet("health", () => Results.Ok(new { status = "healthy", message = "API is running" }))
            .WithName("HealthCheck")
            .WithSummary("Health check endpoint");

        RegisterV1ProductQueries(v1Group);
        RegisterV1ProductCommands(v1Group);
    }

    /// <summary>
    /// Registers all Version 2 endpoints.
    /// Uncomment and implement when you need to add Version 2.
    /// </summary>
    // private static void RegisterV2(IEndpointRouteBuilder app)
    // {
    //     var v2Group = app.MapGroup("/api/v2/products")
    //         .WithTags("Products V2")
    //         .WithSummary("Product Management API - Version 2");
    //
    //     RegisterProductQueries(v2Group, "v2");
    //     RegisterProductCommands(v2Group, "v2");
    //     
    //     // Add any new V2-specific endpoints here
    // }

    private static void RegisterV1ProductQueries(RouteGroupBuilder group)
    {
        // GET /api/v{version}/products
        new GetProductsEndpoint().AddRoutes(group, GetProductsPath);

        // GET /api/v{version}/products/{id:guid}
        new GetProductByIdEndpoint().AddRoutes(group, GetProductByIdPath);

        // GET /api/v{version}/products/category/{category}
        new GetProductsByCategoryEndpoint().AddRoutes(group, GetProductsByCategoryPath);
    }

    private static void RegisterV1ProductCommands(RouteGroupBuilder group)
    {
        // POST /api/v{version}/products
        new CreateProductEndpoint().AddRoutes(group, CreateProductPath);

        // PUT /api/v{version}/products
        new UpdateProductEndpoint().AddRoutes(group, UpdateProductPath);

        // DELETE /api/v{version}/products/{id}
        new DeleteProductEndpoint().AddRoutes(group, DeleteProductPath);
    }
}

