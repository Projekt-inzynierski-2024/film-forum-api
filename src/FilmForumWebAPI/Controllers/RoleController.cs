using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRoleService _rolesService;

    public RoleController(IRoleService roleService,
                          IUserService userService)
    {
        _rolesService = roleService;
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("user-roles-names{userId}")]
    public async Task<IActionResult> GetUserRolesNames(int userId)
        => !await _userService.UserWithIdExistsAsync(userId)
           ? NotFound("User not found")
           : Ok(await _rolesService.GetUserRolesNamesAsync(userId));

    [Authorize(Roles = "Admin")]
    [HttpGet("user-roles{userId}")]
    public async Task<IActionResult> GetUserRoles(int userId)
        => !await _userService.UserWithIdExistsAsync(userId)
           ? NotFound("User not found")
           : Ok(await _rolesService.GetUserRolesAsync(userId));
}