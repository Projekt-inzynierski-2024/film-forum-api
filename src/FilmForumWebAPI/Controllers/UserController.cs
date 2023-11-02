﻿using AuthenticationManager.Interfaces;
using EmailSender.Factories.Email;
using EmailSender.Factories.Email.Interfaces;
using EmailSender.Interfaces;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Models.Email;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Extensions;
using FilmForumWebAPI.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

    public UserController(ILogger<UserController> logger,
                          IUserService userService,
                          IValidator<CreateUserDto> createUserValidator,
                          IValidator<ChangePasswordDto> changePasswordValidator,
                          IJwtService jwtService,
                          IEmailService emailService,
                          IOptions<EmailSenderDetails> emailSenderDetails)
    {
        _logger = logger;
        _userService = userService;
        _createUserValidator = createUserValidator;
        _changePasswordValidator = changePasswordValidator;
        _jwtService = jwtService;
        _emailService = emailService;
        _emailSenderDetails = emailSenderDetails.Value;
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

        return !string.IsNullOrWhiteSpace(result) ? Ok(result) : BadRequest("Invalid credentials");
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

        if (!await _userService.UserWithIdExistsAsync(id))
        {
            return NotFound("User not found");
        }

        int result = await _userService.ChangePasswordAsync(id, changePasswordDto);

        return NoContent();
    }

    [Authorize(Roles = "User")]
    [HttpPut("/change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto changeEmailDto)
    {
        if (await _userService.UserWithEmailExistsAsync(changeEmailDto.Email))
        {
            return NotFound("Email already exists");
        }
        int id = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value!);
        int result = await _userService.ChangeEmailAsync(id, changeEmailDto.Email);

        return NoContent();
    }
}