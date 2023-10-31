using AuthenticationManager.Interfaces;
using AuthenticationManager.Services;
using FilmForumModels.Models.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthenticationManager.Extensions;

public static class AuthenticationManagerExtension
{
    public static IServiceCollection AddAuthenticationManager(this IServiceCollection services, JwtDetails jwtDetails)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtDetails.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtDetails.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtDetails.SecretKey)),
                        ValidateIssuerSigningKey = true
                    };
                });
        return services;
    }
}