using AuthenticationManager.Interfaces;
using EmailSender.Factories.Email;
using EmailSender.Factories.Email.Interfaces;
using EmailSender.Interfaces;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Models.Email;
using FilmForumModels.Models.Password;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Extensions;
using FilmForumWebAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PasswordManager.Interfaces;
using System.Security.Claims;

namespace FilmForumWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly EmailSenderDetails _emailSenderDetails;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;
    private readonly IUserDiagnosticsService _userDiagnosticsService;

    public UserController(ILogger<UserController> logger,
                          IUserService userService,
                          IValidator<CreateUserDto> createUserValidator,
                          IValidator<ChangePasswordDto> changePasswordValidator,
                          IJwtService jwtService,
                          IEmailService emailService,
                          IOptions<EmailSenderDetails> emailSenderDetails,
                          IPasswordResetTokenService passwordResetTokenService,
                          IValidator<ResetPasswordDto> resetPasswordValidator,
                          IUserDiagnosticsService userDiagnosticsService)
    {
        _logger = logger;
        _userService = userService;
        _createUserValidator = createUserValidator;
        _changePasswordValidator = changePasswordValidator;
        _jwtService = jwtService;
        _emailService = emailService;
        _emailSenderDetails = emailSenderDetails.Value;
        _passwordResetTokenService = passwordResetTokenService;
        _resetPasswordValidator = resetPasswordValidator;
        _userDiagnosticsService = userDiagnosticsService;
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

        await _userDiagnosticsService.CreateAsync(result.Id);

        IEmailMessageFactory emailMessageFactory = new UserCreatedAccountEmailMessageFactory();
        IEmailMessage emailMessage = emailMessageFactory.Create(result.Email);

        //We use try-catch block here as user should be created even if there is a problem with sending welcome e-mail
        try
        {
            await _emailService.SendEmailAsync(emailMessage, _emailSenderDetails);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while sending welcome email");
        }

        return Created(nameof(GetById), result);
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] LogInDto logInDto)
    {
        if (!await _userService.UserWithEmailExistsAsync(logInDto.Email))
        {
            return NotFound("User not found");
        }

        string? result = await _userService.LogInAsync(logInDto);
        if (string.IsNullOrWhiteSpace(result))
        {
            await _userDiagnosticsService.UpdateLastFailedSignInAsync(logInDto.Email);
            return BadRequest("Invalid credentials");
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _userService.GetAllAsync());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => await _userService.GetUserAsync(id) is GetUserDto user ? Ok(user) : NotFound($"User not found");

    [Authorize(Roles = "User")]
    [HttpPut("/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value!);
        int result = await _userService.ChangePasswordAsync(id, changePasswordDto);

        return NoContent();
    }

    [Authorize(Roles = "User")]
    [HttpPut("/change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] EmailDto emailDto)
    {
        if (await _userService.UserWithEmailExistsAsync(emailDto.Email))
        {
            return Conflict("Email already exists");
        }
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value!);
        int result = await _userService.ChangeEmailAsync(id, emailDto.Email);

        return NoContent();
    }

    [Authorize(Roles = "User")]
    [HttpPut("/change-username")]
    public async Task<IActionResult> ChangeUsername([FromBody] UsernameDto usernameDto)
    {
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value!);
        int result = await _userService.ChangeUsernameAsync(id, usernameDto.Username);

        return NoContent();
    }

    [HttpPost("/password-reset-token")]
    public async Task<IActionResult> SendPasswordResetToken([FromBody] EmailDto emailDto)
    {
        if (!await _userService.UserWithEmailExistsAsync(emailDto.Email))
        {
            return NotFound("User not found");
        }

        PasswordResetTokenWithExpirationDate tokenWithExpirationDate = _passwordResetTokenService.CreatePasswordResetTokenWithExpirationDate();
        await _userService.UpdatePasswordResetTokenAsync(emailDto.Email, tokenWithExpirationDate);

        IEmailMessageFactory emailMessageFactory = new UserResetPasswordEmailMessageFactory();
        IEmailMessage emailMessage = emailMessageFactory.Create(emailDto.Email, body: $"Your token to reset password: {tokenWithExpirationDate.Token}. The token expires {tokenWithExpirationDate.ExpirationDate}");
        await _emailService.SendEmailAsync(emailMessage, _emailSenderDetails);

        return Ok("Token was successfully sent. Check your e-mail.");
    }

    [HttpPost("/password-reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        ValidationResult validation = _resetPasswordValidator.Validate(resetPasswordDto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.GetMessagesAsString());
        }

        if (!await _userService.UserWithEmailExistsAsync(resetPasswordDto.Email))
        {
            return NotFound("User not found");
        }

        ValidateResetPasswordTokenResult validateResetPasswordTokenResult = await _userService.ValidateResetPasswordTokenAsync(resetPasswordDto.ResetPasswordToken);
        if (!validateResetPasswordTokenResult.IsValid)
        {
            return BadRequest(validateResetPasswordTokenResult.Message);
        }

        await _userService.ResetPasswordAsync(resetPasswordDto);

        return Ok("New password has been set");
    }
}