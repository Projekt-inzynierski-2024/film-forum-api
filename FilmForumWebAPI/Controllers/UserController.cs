using FilmForumWebAPI.Models.Dtos;
using FilmForumWebAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    public UserController(IValidator<CreateUserDto> createUserValidator,
                          IUserService userService)
    {
        _createUserValidator = createUserValidator;
        _userService = userService;
    }

    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IUserService _userService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
    {
        if (await _userService.EmailExists(createUserDto.Email))
        {
            return BadRequest("Email already exists");
        }

        if (await _userService.UsernameExists(createUserDto.Username))
        {
            return BadRequest("Username already exists");
        }

        UserCreatedDto result = await _userService.CreateUserAsync(createUserDto);

        return Ok();
    }
}