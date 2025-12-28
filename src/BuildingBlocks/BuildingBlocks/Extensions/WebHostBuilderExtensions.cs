using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Extensions;

/// <summary>
/// Extension methods for configuring web hosts, particularly for Docker compatibility.
/// </summary>
public static class WebHostBuilderExtensions
{
    /// <summary>
    /// Configures the web host to listen on all interfaces (0.0.0.0) for Docker compatibility.
    /// This ensures the application listens on IPv4 instead of just IPv6, which is required
    /// for proper Docker port mapping.
    /// 
    /// If ASPNETCORE_URLS is set, it will be used. Otherwise, it will use ASPNETCORE_HTTP_PORTS
    /// or default to port 8080.
    /// </summary>
    /// <param name="builder">The web host builder (from WebApplicationBuilder.WebHost)</param>
    /// <returns>The web host builder for chaining</returns>
    public static IWebHostBuilder UseDockerHosting(this IWebHostBuilder builder)
    {
        // Check if ASPNETCORE_URLS is explicitly set (highest priority)
        var urls = builder.GetSetting(WebHostDefaults.ServerUrlsKey);
        
        if (!string.IsNullOrEmpty(urls))
        {
            // If URLs are already set, ensure they use 0.0.0.0 instead of localhost/127.0.0.1
            var dockerUrls = urls
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(url => url.Trim())
                .Select(url => url.Replace("localhost", "0.0.0.0").Replace("127.0.0.1", "0.0.0.0"))
                .Where(url => !url.Contains("[::]")) // Remove IPv6-only bindings
                .ToList();
            
            if (dockerUrls.Any())
            {
                builder.UseUrls(dockerUrls.ToArray());
            }
        }
        else
        {
            // If ASPNETCORE_URLS is not set, check ASPNETCORE_HTTP_PORTS
            var httpPorts = builder.GetSetting("ASPNETCORE_HTTP_PORTS") ?? "8080";
            var httpsPorts = builder.GetSetting("ASPNETCORE_HTTPS_PORTS");
            
            var dockerUrls = new List<string>();
            
            // Add HTTP URLs
            foreach (var port in httpPorts.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                dockerUrls.Add($"http://0.0.0.0:{port.Trim()}");
            }
            
            // Add HTTPS URLs if specified
            if (!string.IsNullOrEmpty(httpsPorts))
            {
                foreach (var port in httpsPorts.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    dockerUrls.Add($"https://0.0.0.0:{port.Trim()}");
                }
            }
            
            if (dockerUrls.Any())
            {
                builder.UseUrls(dockerUrls.ToArray());
            }
        }
        
        return builder;
    }
}

