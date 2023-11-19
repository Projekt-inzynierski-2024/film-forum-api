using FilmForumModels.Dtos.RoleDtos;
using FilmForumModels.Entities;
using FilmForumModels.Models.Enums;
using FilmForumModels.Models.Exceptions;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.Services;

public class RoleService : IRoleService
{
    private readonly UsersDatabaseContext _usersDatabaseContext;

    public RoleService(UsersDatabaseContext usersDatabaseContext) => _usersDatabaseContext = usersDatabaseContext;

    public async Task<List<string>> GetUserRolesNamesAsync(int userId)
        => await _usersDatabaseContext.Roles.Join(_usersDatabaseContext.UsersToRoles.Where(x => x.UserId == userId),
                                                 role => role.Id,
                                                 userToRole => userToRole.RoleId,
                                                 (role, userToRole) => role.Name).ToListAsync();

    public async Task<List<GetUserRoleDto>> GetUserRolesAsync(int userId)
        => await _usersDatabaseContext.Roles.Join(_usersDatabaseContext.UsersToRoles.Where(x => x.UserId == userId),
                                                  role => role.Id,
                                                  userToRole => userToRole.RoleId,
                                                  (role, userToRole) => new GetUserRoleDto(role.Id, role.Name, role.CreatedAt, userToRole.UserId)).ToListAsync();

    public async Task<int> ChangeUserRolesAsync(int userId, IEnumerable<UserRole> roles)
    {
        if (!await _usersDatabaseContext.Users.AnyAsync(x => x.Id == userId))
        {
            return 0;
        }
        if (roles is null || !roles.Any())
        {
            throw new InvalidRoleNameException();
        }

        List<UserToRole> userRoles = await _usersDatabaseContext.UsersToRoles.Where(x => x.UserId == userId).ToListAsync();
        _usersDatabaseContext.UsersToRoles.RemoveRange(userRoles);

        IEnumerable<string> rolesNames = roles.Select(x => x.ToString());
        List<int> roleIds = await _usersDatabaseContext.Roles.Where(x => rolesNames.Contains(x.Name)).Select(x => x.Id).ToListAsync();

        roleIds.ForEach(async roleId => await _usersDatabaseContext.UsersToRoles.AddAsync(new UserToRole(roleId, userId)));

        return await _usersDatabaseContext.SaveChangesAsync();
    }

    public List<UserRole> PrepareUserRolesToSaveInDatabase(UserRole userMainRole)
        => userMainRole switch
        {
            UserRole.Admin => new List<UserRole> { UserRole.Admin, UserRole.Moderator, UserRole.User },
            UserRole.Moderator => new List<UserRole> { UserRole.Moderator, UserRole.User },
            UserRole.User => new List<UserRole> { UserRole.User },
            _ => throw new InvalidRoleNameException()
        };
}