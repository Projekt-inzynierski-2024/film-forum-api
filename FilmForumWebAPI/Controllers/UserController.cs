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
        if (await _userService.UserWithEmailExistsAsync(createUserDto.Email))
        {
            return BadRequest("Email already exists");
        }

        if (await _userService.UserWithUsernameExistsAsync(createUserDto.Username))
        {
            return BadRequest("Username already exists");
        }

        UserCreatedDto result = await _userService.CreateUserAsync(createUserDto);

        return Created(nameof(GetById), result);
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] LogInDto logInDto)
    {
        if (!await _userService.UserWithEmailExistsAsync(logInDto.Email))
        {
            return NotFound("User not found");
        }

        var result = await _userService.LogInAsync(logInDto);

        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        GetUserDto? user = await _userService.GetUserAsync(id);
        return user is not null ? Ok(user) : NotFound($"User not found");
    }

    [HttpPut("/change-password{id}")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
    {
        if (!await _userService.UserWithIdExistsAsync(id))
        {
            return NotFound("User not found");
        }

        int result = await _userService.ChangePasswordAsync(id, changePasswordDto);

        return NoContent();
    }

    [HttpPut("/change-email{id}")]
    public async Task<IActionResult> ChangeEmail(int id, [FromBody] string email)
    {
        if (await _userService.UserWithEmailExistsAsync(email))
        {
            return NotFound("Email already exists");
        }

        int result = await _userService.ChangeEmailAsync(id, email);

        return NoContent();
    }
}