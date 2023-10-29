using FilmForumWebAPI.Authorization.Interfaces;
using FilmForumWebAPI.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FilmForumWebAPI.Authorization;

public class JwtService : IJwtService
{
    public string GenerateToken(User user, IList<string> roles)
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
        
        if(roles?.Any() == true)
        {
            IEnumerable<Claim> rolesToClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
            claims.AddRange(rolesToClaims);
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes("superSecretKey"));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
        DateTime expires = DateTime.Now.AddMinutes(Convert.ToDouble(60 * 24));

        JwtSecurityToken token = new(issuer: "FilmForumWebAPI",
                                     audience: "FilmForumWebAPI",
                                     claims: claims,
                                     expires: expires,
                                     signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
