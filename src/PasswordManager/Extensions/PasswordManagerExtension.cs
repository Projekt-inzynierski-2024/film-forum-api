using Microsoft.Extensions.DependencyInjection;
using PasswordManager.Interfaces;
using PasswordManager.Services;

namespace PasswordManager.Extensions;

public static class PasswordManagerExtension
{
    public static IServiceCollection AddPasswordManager(this IServiceCollection services)
    {
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
        return services;
    }
}