using AuthenticationManager.Interfaces;
using AuthenticationManager.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationManager.Extensions;

public static class AuthenticationManagerExtension
{
    public static IServiceCollection AddAuthenticationManager(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        return services;
    }
}