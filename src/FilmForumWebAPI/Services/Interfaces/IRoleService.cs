using FilmForumModels.Dtos.RoleDtos;
using FilmForumModels.Models.Enums;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IRoleService
{
    Task<List<string>> GetUserRolesNamesAsync(int userId);

    Task<List<GetUserRoleDto>> GetUserRolesAsync(int userId);

    Task ChangeUserRolesAsync(int userId, IEnumerable<RoleEnum> roleEnums);

    List<RoleEnum> PrepareRolesForUser(RoleEnum roleEnum);
}