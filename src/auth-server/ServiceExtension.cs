using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace AuthServer;

/// <summary>
/// 
/// </summary>
public static class ServiceExtension
{
    internal static Configurations Configuration = new();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void RegisterAuthServer(this IServiceCollection services, Func<Configurations, Configurations>? config = null)
    {
        services.AddMemoryCache();
    
        if(config != null)
            Configuration =  config(Configuration);

        services.AddSignalR();

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
