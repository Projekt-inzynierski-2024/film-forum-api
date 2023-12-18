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

    /// <summary>
    /// Gets names of all roles assigned to user
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>Names of all roles assigned to user</returns>
    public async Task<List<string>> GetUserRolesNamesAsync(int userId)
        => await _usersDatabaseContext.Roles.Join(_usersDatabaseContext.UsersToRoles.Where(x => x.UserId == userId),
                                                 role => role.Id,
                                                 userToRole => userToRole.RoleId,
                                                 (role, userToRole) => role.Name).ToListAsync();

    /// <summary>
    /// Gets all roles assigned to user
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>All roles assigned to user</returns>
    public async Task<List<GetUserRoleDto>> GetUserRolesAsync(int userId)
        => await _usersDatabaseContext.Roles.Join(_usersDatabaseContext.UsersToRoles.Where(x => x.UserId == userId),
                                                  role => role.Id,
                                                  userToRole => userToRole.RoleId,
                                                  (role, userToRole) => new GetUserRoleDto(role.Id, role.Name, role.CreatedAt, userToRole.UserId)).ToListAsync();

    /// <summary>
    /// Changes user's roles
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <param name="roles">New roles for user</param>
    /// <returns>The total number of rows updated in the database</returns>
    /// <exception cref="InvalidRoleNameException">When <paramref name="roles"/> has invalid value</exception>
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

    /// <summary>
    /// Prepares user's roles to save in database returning proper list of roles to save
    /// </summary>
    /// <param name="userMainRole">Main role of user, because all subordinating roles needs to be attached</param>
    /// <returns>List of proper roles for user that can be saved in database</returns>
    /// <exception cref="InvalidRoleNameException">When <paramref name="userMainRole"/> has invalid value</exception>
    public List<UserRole> PrepareUserRolesToSaveInDatabase(UserRole userMainRole) => userMainRole switch
    {
        UserRole.Admin => new List<UserRole> { UserRole.Admin, UserRole.Moderator, UserRole.User },
        UserRole.Moderator => new List<UserRole> { UserRole.Moderator, UserRole.User },
        UserRole.User => new List<UserRole> { UserRole.User },
        _ => throw new InvalidRoleNameException()
    };
}