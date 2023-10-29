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
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roles[0])
        };
        
        if(roles?.Any() == true)
        {
            var rolesToClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
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
