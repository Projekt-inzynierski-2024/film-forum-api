using FilmForumModels.Dtos.RoleDtos;
using FilmForumModels.Entities;
using FilmForumModels.Models.Enums;
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
        => await _usersDatabaseContext.Roles.Join(_usersDatabaseContext.UsersToRoles.Where(x => x.UserId == userId),
                                                 role => role.Id,
                                                 userToRole => userToRole.RoleId,
                                                 (role, userToRole) => role.Name).ToListAsync();

    public async Task<List<GetUserRoleDto>> GetUserRolesAsync(int userId)
        => await _usersDatabaseContext.Roles.Join(_usersDatabaseContext.UsersToRoles.Where(x => x.UserId == userId),
                                                  role => role.Id,
                                                  userToRole => userToRole.RoleId,
                                                  (role, userToRole) => new GetUserRoleDto(role.Id, role.Name, role.CreatedAt, userToRole.UserId)).ToListAsync();

    public async Task ChangeUserRolesAsync(int userId, IEnumerable<RoleEnum> roleEnums)
    {
        List<UserToRole> userRoles = await _usersDatabaseContext.UsersToRoles.Where(x => x.UserId == userId).ToListAsync();
        _usersDatabaseContext.UsersToRoles.RemoveRange(userRoles);

        IEnumerable<string> rolesNames = roleEnums.Select(x => x.ToString());
        List<int> roleIds = await _usersDatabaseContext.Roles.Where(x => rolesNames.Contains(x.Name)).Select(x => x.Id).ToListAsync();

        roleIds.ForEach(async roleId => await _usersDatabaseContext.UsersToRoles.AddAsync(new UserToRole(roleId, userId)));

        await _usersDatabaseContext.SaveChangesAsync();
    }

    public List<RoleEnum> PrepareRolesForUser(RoleEnum roleEnum)
        => roleEnum switch
        {
            RoleEnum.Admin => new List<RoleEnum> { RoleEnum.Admin, RoleEnum.Moderator, RoleEnum.User },
            RoleEnum.Moderator => new List<RoleEnum> { RoleEnum.Moderator, RoleEnum.User },
            RoleEnum.User => new List<RoleEnum> { RoleEnum.User },
            _ => throw new ArgumentException("Invalid role value")
        };
}