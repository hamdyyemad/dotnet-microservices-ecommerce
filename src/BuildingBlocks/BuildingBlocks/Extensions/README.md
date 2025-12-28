# Docker Hosting Extension

## Problem

When running ASP.NET Core applications in Docker containers, there's a common issue where the application listens on IPv6 (`[::]:8080`) instead of IPv4 (`0.0.0.0:8080`). This causes Docker port mapping to fail, making the service inaccessible from outside the container.

## Solution

The `UseDockerHosting()` extension method automatically configures your ASP.NET Core application to listen on all network interfaces (`0.0.0.0`) instead of just IPv6, ensuring proper Docker port mapping.

## How It Works

1. **Checks for `ASPNETCORE_URLS`**: If set, it converts any `localhost` or `127.0.0.1` bindings to `0.0.0.0`
2. **Falls back to `ASPNETCORE_HTTP_PORTS`**: If URLs aren't set, it reads the HTTP ports from environment variables
3. **Defaults to port 8080**: If neither is set, it defaults to `http://0.0.0.0:8080`
4. **Handles HTTPS**: Also processes `ASPNETCORE_HTTPS_PORTS` if specified

## Usage

Simply call `UseDockerHosting()` on your `WebApplicationBuilder`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure for Docker compatibility
builder.WebHost.UseDockerHosting();

// ... rest of your configuration
```

## For Multiple Services

**Yes, you need to add this to each service**, but now it's just one line! Simply add:

```csharp
builder.WebHost.UseDockerHosting();
```

to each service's `Program.cs` file. Since it's in the `BuildingBlocks` project, all services that reference `BuildingBlocks` can use it.

## Docker Compose Configuration

In your `docker-compose.yml`, you can set the port via environment variable:

```yaml
services:
  your-service:
    environment:
      - ASPNETCORE_HTTP_PORTS=8080
    ports:
      - "5050:8080"  # Maps host port 5050 to container port 8080
```

The extension method will automatically use this port and configure it to listen on `0.0.0.0:8080`.

