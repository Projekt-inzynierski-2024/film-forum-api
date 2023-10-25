using FilmForumWebAPI.Models.Dtos.User;
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

    [HttpPost("/register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
    {
        if (await _userService.UserWithEmailExists(createUserDto.Email))
        {
            return BadRequest("Email already exists");
        }

        if (await _userService.UserWithUsernameExists(createUserDto.Username))
        {
            return BadRequest("Username already exists");
        }

        UserCreatedDto result = await _userService.CreateUserAsync(createUserDto);

        return Ok();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] LogInDto logInDto)
    {
        if(!await _userService.UserWithEmailExists(logInDto.Email))
        {
            return NotFound("User not found");
        }

        var result = await _userService.LogInAsync(logInDto);

        return Ok();
    }
}