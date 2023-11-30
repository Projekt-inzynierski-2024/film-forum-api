using AuthenticationManager.Interfaces;
using EmailSender.Factories.Email;
using EmailSender.Factories.Email.Interfaces;
using EmailSender.Interfaces;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Models.Email;
using FilmForumModels.Models.Enums;
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
    private readonly IUserDiagnosticsService _userDiagnosticsService;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly SmtpSettings _smtpSettings;
    private readonly EmailSenderDetails _emailSenderDetails;
    private readonly AdminDetails _adminDetails;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
    private readonly IValidator<CreateAdminDto> _createAdminValidator;
    private readonly IMultifactorAuthenticationService _multifactorAuthenticationService;

    public UserController(ILogger<UserController> logger,
                          IUserService userService,
                          IUserDiagnosticsService userDiagnosticsService,
                          IEmailService emailService,
                          IJwtService jwtService,
                          IPasswordResetTokenService passwordResetTokenService,
                          IOptions<SmtpSettings> smtpSettings,
                          IOptions<EmailSenderDetails> emailSenderDetails,
                          IOptions<AdminDetails> adminDetails,
                          IValidator<CreateUserDto> createUserValidator,
                          IValidator<ResetPasswordDto> resetPasswordValidator,
                          IValidator<ChangePasswordDto> changePasswordValidator,
                          IValidator<CreateAdminDto> createAdminValidator,
                          IMultifactorAuthenticationService multifactorAuthenticationService)
    {
        _logger = logger;
        _userService = userService;
        _createUserValidator = createUserValidator;
        _changePasswordValidator = changePasswordValidator;
        _jwtService = jwtService;
        _emailService = emailService;
        _smtpSettings = smtpSettings.Value;
        _emailSenderDetails = emailSenderDetails.Value;
        _passwordResetTokenService = passwordResetTokenService;
        _resetPasswordValidator = resetPasswordValidator;
        _userDiagnosticsService = userDiagnosticsService;
        _adminDetails = adminDetails.Value;
        _createAdminValidator = createAdminValidator;
        _multifactorAuthenticationService = multifactorAuthenticationService;
    }

    [HttpPost("/register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] CreateAdminDto createAdminDto)
    {
        ValidationResult validation = _createAdminValidator.Validate(createAdminDto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.GetMessagesAsString());
        }

        if (await _userService.UserWithEmailExistsAsync(createAdminDto.Email))
        {
            return Conflict("Email already exists");
        }

        if (await _userService.UserWithUsernameExistsAsync(createAdminDto.Username))
        {
            return Conflict("Username already exists");
        }

        UserCreatedDto result = await _userService.CreateAsync(createAdminDto, UserRole.Admin);

        await _userDiagnosticsService.CreateAsync(result.Id);

        IEmailMessageFactory emailMessageFactory = new UserCreatedAccountEmailMessageFactory();
        IEmailMessage emailMessage = emailMessageFactory.Create(result.Email);

        //We use try-catch block here as user should be created even if there is a problem with sending welcome e-mail
        try
        {
            await _emailService.SendEmailAsync(emailMessage, _emailSenderDetails, _smtpSettings);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while sending welcome email");
        }

        return Created(nameof(GetById), result);
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
            return Conflict("Email already exists");
        }

        if (await _userService.UserWithUsernameExistsAsync(createUserDto.Username))
        {
            return Conflict("Username already exists");
        }

        UserCreatedDto result = await _userService.CreateAsync(createUserDto, UserRole.User);

        await _userDiagnosticsService.CreateAsync(result.Id);

        IEmailMessageFactory emailMessageFactory = new UserCreatedAccountEmailMessageFactory();
        IEmailMessage emailMessage = emailMessageFactory.Create(result.Email);

        //We use try-catch block here as user should be created even if there is a problem with sending welcome e-mail
        try
        {
            await _emailService.SendEmailAsync(emailMessage, _emailSenderDetails, _smtpSettings);
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
        if (await _userService.LogInAsync(logInDto) is UserSignedInDto result)
        {
            await _userDiagnosticsService.UpdateLastSuccessfullSignInAsync(result.Id);
            return Ok(result);
        }

        await _userDiagnosticsService.UpdateLastFailedSignInAsync(logInDto.Email);
        return BadRequest("Invalid credentials");
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _userService.GetAllAsync());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => await _userService.GetAsync(id) is GetUserDto user ? Ok(user) : NotFound($"User not found");

    [Authorize]
    [HttpPut("/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        ValidationResult validation = _changePasswordValidator.Validate(changePasswordDto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.GetMessagesAsString());
        }
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value!);
        await _userService.ChangePasswordAsync(id, changePasswordDto);
        await _userDiagnosticsService.UpdateLastPasswordChangeAsync(id);

        return NoContent();
    }

    [Authorize]
    [HttpPut("/change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] EmailDto emailDto)
    {
        if (await _userService.UserWithEmailExistsAsync(emailDto.Email))
        {
            return Conflict("Email already exists");
        }
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value!);
        await _userService.ChangeEmailAsync(id, emailDto.Email);
        await _userDiagnosticsService.UpdateLastEmailChangeAsync(id);

        return NoContent();
    }

    [Authorize]
    [HttpPut("/change-username")]
    public async Task<IActionResult> ChangeUsername([FromBody] UsernameDto usernameDto)
    {
        if (await _userService.UserWithUsernameExistsAsync(usernameDto.Username))
        {
            return Conflict("User already exists");
        }
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value!);
        await _userService.ChangeUsernameAsync(id, usernameDto.Username);
        await _userDiagnosticsService.UpdateLastUsernameChangeAsync(id);

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
        IEmailMessage emailMessage = emailMessageFactory.Create(emailDto.Email, body: $"Your token to reset password: {tokenWithExpirationDate.Token}. The token expires {tokenWithExpirationDate.ExpirationDate.ToLocalTime}");
        await _emailService.SendEmailAsync(emailMessage, _emailSenderDetails, _smtpSettings);

        return Ok("Token was successfully sent. Check your e-mail.");
    }

    [HttpPut("/password-reset")]
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
            bool user2faAuth = await _userService.UserWithEmailAndMultifactorAuthOnExistsAsync(resetPasswordDto.Email);
            if (!user2faAuth || !await _multifactorAuthenticationService.VerifyCodeAsync(resetPasswordDto.Email, resetPasswordDto.ResetPasswordToken))
            {
                return BadRequest(validateResetPasswordTokenResult.Message);
            }
        }

        await _userService.ResetPasswordAsync(resetPasswordDto);

        return NoContent();
    }

    [Authorize]
    [HttpGet("2fa")]
    public async Task<IActionResult> Get2faUri()
    {
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value!);
        GetUserDto? userDto = await _userService.GetAsync(id);

        if (userDto is null)
        {
            return Unauthorized();
        }

        string uri = await _multifactorAuthenticationService.GenerateUriAsync(userDto.Email);
        return Ok(uri);
    }

    [Authorize]
    [HttpGet("2fa/png")]
    public async Task<IActionResult> Get2faPng()
    {
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value!);
        GetUserDto? userDto = await _userService.GetAsync(id);

        if (userDto is null)
        {
            return Unauthorized();
        }

        string uri = await _multifactorAuthenticationService.GenerateUriAsync(userDto.Email);
        byte[] qrCode = await _multifactorAuthenticationService.GenerateQRCodePNGAsync(uri);

        return File(qrCode, "image/png");
    }

    [Authorize]
    [HttpPost("2fa")]
    public async Task<IActionResult> SetMultifactorAuthentication([FromBody] ChangeMultifactorAuthenticationDto changeMultifactorAuthenticationDto)
    {
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value!);
        GetUserDto? userDto = await _userService.GetAsync(id);

        if (userDto is null)
        {
            return Unauthorized();
        }
        
        bool verify = await _multifactorAuthenticationService.VerifyCodeAsync(userDto.Email, changeMultifactorAuthenticationDto.TotpCode);

        if (!verify) {
            return BadRequest("Wrong code");
        }

        await _userService.ChangeMultifactorAuthAsync(id, changeMultifactorAuthenticationDto.MultifactorAuthentication);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _userService.UserWithIdExistsAsync(id))
        {
            return NotFound("User not found");
        }

        await _userService.RemoveAsync(id);

        return NoContent();
    }

    [Authorize]
    [HttpDelete()]
    public async Task<IActionResult> Delete()
    {
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value!);

        await _userService.RemoveAsync(id);

        return NoContent();
    }
}
