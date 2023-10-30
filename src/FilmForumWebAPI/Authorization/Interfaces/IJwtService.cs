using FilmForumWebAPI.Models.Entities;

namespace FilmForumWebAPI.Authorization.Interfaces;

public interface IJwtService
{
    public string GenerateToken(User user, IEnumerable<string> roles);
}
