using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.Services;

public class RoleService : IRoleService
{
    private readonly UsersDatabaseContext _usersDatabaseContext;

    public RoleService(UsersDatabaseContext usersDatabaseContext)
    {
        _usersDatabaseContext = usersDatabaseContext;
    }

    public async Task<List<string>> GetUserRolesNamesAsync(int userId)
        => await _usersDatabaseContext.Roles.Join(_usersDatabaseContext.UsersToRoles.Where(x => x.UserId == userId), role => role.Id, userToRole => userToRole.RoleId, (role, userToRole) => role.Name).ToListAsync();
}