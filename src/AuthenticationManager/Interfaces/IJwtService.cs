using FilmForumModels.Entities;

namespace AuthenticationManager.Interfaces;

public interface IJwtService
{
    public string GenerateToken(User user, IEnumerable<string> roles);
}