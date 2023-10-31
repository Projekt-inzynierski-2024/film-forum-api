using FilmForumModels.Entities;
using FilmForumModels.Models.Settings;

namespace AuthenticationManager.Interfaces;

public interface IJwtService
{
    public string GenerateToken(User user, IEnumerable<string> roles, JwtDetails options);
}