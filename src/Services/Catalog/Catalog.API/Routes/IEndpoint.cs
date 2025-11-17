namespace Catalog.API.Routes;

/// <summary>
/// Interface for endpoints that accept a route path parameter.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Registers the endpoint route with the specified path.
    /// </summary>
    /// <param name="app">The endpoint route builder</param>
    /// <param name="routePath">The route path (e.g., "/", "/{id:guid}", "/category/{category}")</param>
    void AddRoutes(IEndpointRouteBuilder app, string routePath);
}

