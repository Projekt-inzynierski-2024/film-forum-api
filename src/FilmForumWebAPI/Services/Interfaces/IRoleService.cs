namespace FilmForumWebAPI.Services.Interfaces;

public interface IRoleService
{
    Task<List<string>> GetUserRolesNamesAsync(int userId);
}