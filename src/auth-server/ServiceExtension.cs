using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace AuthServer;

public static class ServiceExtension
{
    internal static Configurations Configuration = new();
    public static void RegisterAuthServer(this IServiceCollection services, Func<Configurations, Configurations> config)
    {
        services.AddMemoryCache();
        Configuration = config(Configuration);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string GetHash(this string password)
    {
        var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

        return Convert.ToBase64String(hash);
    }
}
