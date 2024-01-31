using AuthenticationManager.Interfaces;
using AuthenticationManager.Services;
using FilmForumModels.Models.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace AuthenticationManager.Extensions;

public static class AuthenticationManagerExtension
{
    public static IServiceCollection AddAuthenticationManager(this IServiceCollection services, JwtDetails jwtDetails)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IMultifactorAuthenticationService, MultifactorAuthenticationService>();
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
                    options.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = context =>
                        {
                            string? userId = context?.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                            string? username = context?.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                            string? email = context?.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

                            //Avoid trying to authorize with user who doesn't exist
                            if (IsAnyClaimValueNull(new string?[] { userId, username, email }))
                            {
                                context?.Fail("Unauthorized. User is null");
                                return Task.CompletedTask;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        return services;
    }

    private static bool IsAnyClaimValueNull(IEnumerable<string?> claimsValues) => claimsValues.Any(string.IsNullOrWhiteSpace);
}