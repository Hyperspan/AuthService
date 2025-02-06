using Microsoft.Extensions.DependencyInjection;

namespace AuthServer;

public static class ServiceExtension
{
    internal static Configurations Configuration = new();
    public static void RegisterAuthServer(this IServiceCollection services, Func<Configurations, Configurations> config)
    {
        services.AddMemoryCache();
        Configuration = config(Configuration);
    }
}
