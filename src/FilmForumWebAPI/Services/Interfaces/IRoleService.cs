using FilmForumModels.Dtos.RoleDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IRoleService
{
    Task<List<string>> GetUserRolesNamesAsync(int userId);

    Task<List<GetUserRoleDto>> GetUserRolesAsync(int userId);
}