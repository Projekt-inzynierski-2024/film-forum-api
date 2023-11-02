using EmailSender.Interfaces;
using EmailSender.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EmailSender.Extensions;

public static class EmailSenderExtension
{
    public static IServiceCollection AddEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}