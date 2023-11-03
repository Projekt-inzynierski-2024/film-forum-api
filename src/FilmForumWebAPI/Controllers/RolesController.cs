using FilmForumWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRoleService _rolesService;

    public RolesController(IRoleService roleService)
    {
        _rolesService = roleService;
    }

    [HttpGet("user-roles-names{userId}")]
    public async Task<IActionResult> GetUserRolesNamesAsync(int userId) => Ok(await _rolesService.GetUserRolesNamesAsync(userId));

    [HttpGet("user-roles{userId}")]
    public async Task<IActionResult> GetUserRolesIdsAsync(int userId) => Ok(await _rolesService.GetUserRolesAsync(userId));
}