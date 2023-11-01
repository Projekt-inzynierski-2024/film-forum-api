using FilmForumModels.Dtos.UserDtos;
using FilmForumWebAPI.Extensions;
using FilmForumWebAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;

    public UserController(IUserService userService,
                          IValidator<CreateUserDto> createUserValidator,
                          IValidator<ChangePasswordDto> changePasswordValidator)
    {
        _userService = userService;
        _createUserValidator = createUserValidator;
        _changePasswordValidator = changePasswordValidator;
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
    {
        ValidationResult validation = _createUserValidator.Validate(createUserDto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.GetMessagesAsString());
        }

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

        string result = await _userService.LogInAsync(logInDto);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _userService.GetAllAsync());

    //[Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => await _userService.GetUserAsync(id) is GetUserDto user ? Ok(user) : NotFound($"User not found");

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
    public async Task<IActionResult> ChangeEmail(int id, [FromBody] ChangeEmailDto changeEmailDto)
    {
        if (await _userService.UserWithEmailExistsAsync(changeEmailDto.Email))
        {
            return NotFound("Email already exists");
        }

        int result = await _userService.ChangeEmailAsync(id, changeEmailDto.Email);

        return NoContent();
    }
}