using AuthenticationManager.Interfaces;
using FilmForumModels.Entities;
using FilmForumModels.Models.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationManager.Services;

public class JwtService : IJwtService
{
    /// <summary>
    /// Creates a JSON Web Token for the given user and roles
    /// <para><paramref name="roles"/> can be null due to basic authorization without roles for example <see cref="AuthorizeAttribute"/> without roles</para>
    /// </summary>
    /// <param name="user">User who has successfully signed in</param>
    /// <param name="roles">User's roles necessary for authorization</param>
    /// <param name="options">Jwt details</param>
    /// <returns>JSON Web Token</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="user"/> is null</exception>
    public string GenerateToken(User user, IEnumerable<string>? roles, JwtDetails options)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
        };

        if (roles?.Any() == true)
        {
            IEnumerable<Claim> rolesToClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
            claims.AddRange(rolesToClaims);
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(options.SecretKey));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
        DateTime expires = DateTime.Now.AddMinutes(Convert.ToDouble(options.LifetimeInMinutes) * 24 * 7);

        //FilmForumWebAPI
        JwtSecurityToken token = new(issuer: options.Issuer,
                                     audience: options.Audience,
                                     claims: claims,
                                     expires: expires,
                                     signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}