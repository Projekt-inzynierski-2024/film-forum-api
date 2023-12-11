using AuthenticationManager.Interfaces;
using EmailSender.Interfaces;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Entities;
using FilmForumModels.Models.Enums;
using FilmForumModels.Models.Password;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Org.BouncyCastle.Crypto.Paddings;
using PasswordManager.Interfaces;
using SimpleBase;
using System.Security.Claims;
using static QRCoder.PayloadGenerator;

namespace FilmForumWebAPI.UnitTests.Controllers;

public class UserControllerTests
{
    private readonly UserController _userController;

    private readonly Mock<ILogger<UserController>> _loggerMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IUserDiagnosticsService> _userDiagnosticsServiceMock = new();
    private readonly Mock<IEmailService> _emaiServiceMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly Mock<IPasswordResetTokenService> _passwordResetTokenServiceMock = new();
    private readonly Mock<IOptions<SmtpSettings>> _smtpSettingsMock = new();
    private readonly Mock<IOptions<EmailSenderDetails>> _emailSenderDetailsMock = new();
    private readonly Mock<IOptions<AdminDetails>> _adminDetailsMock = new();
    private readonly Mock<IValidator<CreateUserDto>> _createUserDtoValidatorMock = new();
    private readonly Mock<IValidator<ResetPasswordDto>> _resetPasswordDtoValidatorMock = new();
    private readonly Mock<IValidator<ChangePasswordDto>> _changePasswordDtoValidatorMock = new();
    private readonly Mock<IValidator<CreateAdminDto>> _createAdminDtoValidatorMock = new();
    private readonly Mock<IMultifactorAuthenticationService> _multifactorAuthenticationServiceMock = new();

    public UserControllerTests() => _userController = new(_loggerMock.Object,
                              _userServiceMock.Object,
                              _userDiagnosticsServiceMock.Object,
                              _emaiServiceMock.Object,
                              _jwtServiceMock.Object,
                              _passwordResetTokenServiceMock.Object,
                              _smtpSettingsMock.Object,
                              _emailSenderDetailsMock.Object,
                              _adminDetailsMock.Object,
                              _createUserDtoValidatorMock.Object,
                              _resetPasswordDtoValidatorMock.Object,
                              _changePasswordDtoValidatorMock.Object,
                              _createAdminDtoValidatorMock.Object,
                              _multifactorAuthenticationServiceMock.Object);

    [Fact]
    public async Task RegisterAdmin_ForValidData_CreatesAdmin()
    {
        // Arrange
        Mock<CreateAdminDto> createAdminDtoMock = new("", "", "", "", "");
        UserCreatedDto userCreatedDto = new(1, "NewAdmin", "myadminemail@mail.com", "jwttokenreturned123");
        _createAdminDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateAdminDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateAdminDto>(), It.IsAny<UserRole>())).ReturnsAsync(userCreatedDto);

        // Act
        IActionResult result = await _userController.RegisterAdmin(createAdminDtoMock.Object);

        // Assert
        CreatedResult createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Location.Should().Be(nameof(UserController.GetById));
        createdResult.Value.Should().Be(userCreatedDto);
    }

    [Fact]
    public async Task RegisterAdmin_ForInvalidData_ReturnsBadRequest()
    {
        // Arrange
        _createAdminDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateAdminDto>())).Returns(new ValidationResult() { Errors = new() { new ValidationFailure("Name", "Name was null") } });

        // Act
        IActionResult result = await _userController.RegisterAdmin(It.IsAny<CreateAdminDto>());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RegisterAdmin_ForExistingEmail_ReturnsConflict()
    {
        // Arrange
        Mock<CreateAdminDto> createAdminDtoMock = new("", "", "", "", "");
        _createAdminDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateAdminDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.UserWithEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        // Act
        IActionResult result = await _userController.RegisterAdmin(createAdminDtoMock.Object);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task RegisterAdmin_ForExistingUsername_ReturnsConflict()
    {
        // Arrange
        Mock<CreateAdminDto> createAdminDtoMock = new("", "", "", "", "");
        _createAdminDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateAdminDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.UserWithUsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        // Act
        IActionResult result = await _userController.RegisterAdmin(createAdminDtoMock.Object);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Register_ForValidData_CreatesUser()
    {
        // Arrange
        Mock<CreateUserDto> createUserDtoMock = new("", "", "", "");
        UserCreatedDto userCreatedDto = new(1, "NewUser", "myemail@mail.com", "jwttokenreturned123");
        _createUserDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateUserDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateUserDto>(), It.IsAny<UserRole>())).ReturnsAsync(userCreatedDto);

        // Act
        IActionResult result = await _userController.Register(createUserDtoMock.Object);

        // Assert
        CreatedResult createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Location.Should().Be(nameof(UserController.GetById));
        createdResult.Value.Should().Be(userCreatedDto);
    }

    [Fact]
    public async Task Register_ForInvalidData_ReturnsBadRequest()
    {
        // Arrange
        _createUserDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateUserDto>())).Returns(new ValidationResult() { Errors = new() { new ValidationFailure("Name", "Name was null") } });

        // Act
        IActionResult result = await _userController.Register(It.IsAny<CreateUserDto>());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_ForExistingEmail_ReturnsConflict()
    {
        // Arrange
        Mock<CreateUserDto> createUserDtoMock = new("", "", "", "");
        _createUserDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateUserDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.UserWithEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        // Act
        IActionResult result = await _userController.Register(createUserDtoMock.Object);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Register_ForExistingUsername_ReturnsConflict()
    {
        // Arrange
        Mock<CreateUserDto> createUserDtoMock = new("", "", "", "");
        _createUserDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateUserDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.UserWithUsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        // Act
        IActionResult result = await _userController.Register(createUserDtoMock.Object);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Login_ForValidData_LogsIn()
    {
        // Arrange
        UserSignedInDto userSignedInDto = new(1, "Username", "jwtotoken123");
        _userServiceMock.Setup(x => x.LogInAsync(It.IsAny<LogInDto>())).ReturnsAsync(userSignedInDto);

        // Act
        IActionResult result = await _userController.Login(It.IsAny<LogInDto>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().Be(userSignedInDto);
    }

    [Fact]
    public async Task Login_ForInValidCredentials_ReturnsBadRequest()
    {
        // Arrange
        Mock<LogInDto> logInDtoMock = new();
        _userServiceMock.Setup(x => x.LogInAsync(It.IsAny<LogInDto>())).ReturnsAsync(() => null);

        // Act
        IActionResult result = await _userController.Login(logInDtoMock.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAll_ForDataInDatabase_ReturnsListOfUsers()
    {
        // Arrange
        List<GetUserDto> getUserDtoList = new()
        {
            new GetUserDto(new User()),
            new GetUserDto(new User()),
            new GetUserDto(new User())
        };
        _userServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(getUserDtoList);

        // Act
        IActionResult result = await _userController.GetAll();

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        List<GetUserDto> users = okObjectResult.Value.Should().BeOfType<List<GetUserDto>>().Subject;
        users.Should().BeEquivalentTo(getUserDtoList);
    }

    [Fact]
    public async Task GetById_ForExistingUser_ReturnsUser()
    {
        // Arrange
        GetUserDto getUserDto = new(new User() { Id = 1, Username = "username", Email = "email@email.com" });
        _userServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(getUserDto);

        // Act
        IActionResult result = await _userController.GetById(It.IsAny<int>());

        // Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        GetUserDto user = okObjectResult.Value.Should().BeOfType<GetUserDto>().Subject;
        user.Should().Be(getUserDto);
    }

    [Fact]
    public async Task GetById_ForNonExistingUser_ReturnsNotFound()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(() => null);

        // Act
        IActionResult result = await _userController.GetById(It.IsAny<int>());

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ChangePassword_ForValidData_ChangesPassword()
    {
        //Arrange
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "1") }))
        };
        _changePasswordDtoValidatorMock.Setup(x => x.Validate(It.IsAny<ChangePasswordDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<ChangePasswordDto>())).ReturnsAsync(1);

        //Act
        IActionResult result = await _userController.ChangePassword(It.IsAny<ChangePasswordDto>());

        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ChangePassword_ForInvalidData_ReturnsBadRequest()
    {
        //Arrange
        _changePasswordDtoValidatorMock.Setup(x => x.Validate(It.IsAny<ChangePasswordDto>())).Returns(new ValidationResult() { Errors = new() { new ValidationFailure("Password", "Password was null") } });

        //Act
        IActionResult result = await _userController.ChangePassword(It.IsAny<ChangePasswordDto>());

        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ChangeEmail_ForValidData_ChangesEmail()
    {
        //Arrange
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "1") }))
        };
        Mock<EmailDto> emailDtoMock = new();
        _userServiceMock.Setup(x => x.ChangeEmailAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(1);

        //Act
        IActionResult result = await _userController.ChangeEmail(emailDtoMock.Object);

        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ChangeEmail_ForExistingEmail_ReturnsConflict()
    {
        //Arrange
        Mock<EmailDto> emailDtoMock = new();
        _userServiceMock.Setup(x => x.UserWithEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        //Act
        IActionResult result = await _userController.ChangeEmail(emailDtoMock.Object);

        //Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task ChangeUsername_ForValidData_ChangesUsername()
    {
        //Arrange
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "1") }))
        };
        Mock<UsernameDto> usernameDtoMock = new();
        _userServiceMock.Setup(x => x.ChangeUsernameAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(1);

        //Act
        IActionResult result = await _userController.ChangeUsername(usernameDtoMock.Object);

        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ChangeUsername_ForExistingUsername_ReturnsConflict()
    {
        //Arrange
        Mock<UsernameDto> usernameDtoMock = new();
        _userServiceMock.Setup(x => x.UserWithUsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        //Act
        IActionResult result = await _userController.ChangeUsername(usernameDtoMock.Object);

        //Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task SendPasswordResetToken_ForValidData_ReturnsSendsToken()
    {
        //Arrange
        Mock<EmailDto> emailDtoMock = new();
        _userServiceMock.Setup(x => x.UserWithEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        //Act
        IActionResult result = await _userController.SendPasswordResetToken(emailDtoMock.Object);

        //Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task SendPasswordResetToken_ForNonExistingUser_ReturnsNotFound()
    {
        //Arrange
        Mock<EmailDto> emailDtoMock = new();
        _userServiceMock.Setup(x => x.UserWithEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        //Act
        IActionResult result = await _userController.SendPasswordResetToken(emailDtoMock.Object);

        //Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ResetPassword_ForValidData_ResetsPassword()
    {
        //Arrange
        Mock<ResetPasswordDto> resetPasswordDtoMock = new("", "", "", "");
        _resetPasswordDtoValidatorMock.Setup(x => x.Validate(It.IsAny<ResetPasswordDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.UserWithEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userServiceMock.Setup(x => x.ValidateResetPasswordTokenAsync(It.IsAny<string>())).ReturnsAsync(new ValidateResetPasswordTokenResult(true, "Valid token"));

        //Act
        IActionResult result = await _userController.ResetPassword(resetPasswordDtoMock.Object);

        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ResetPassword_ForInvalidData_ReturnsBadRequest()
    {
        //Arrange
        Mock<ResetPasswordDto> resetPasswordDtoMock = new("", "", "", "");
        _resetPasswordDtoValidatorMock.Setup(x => x.Validate(It.IsAny<ResetPasswordDto>())).Returns(new ValidationResult() { Errors = new() { new ValidationFailure("Password", "Password was empty") } });

        //Act
        IActionResult result = await _userController.ResetPassword(resetPasswordDtoMock.Object);

        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ResetPassword_ForNotExistingUser_ReturnsNotFound()
    {
        //Arrange
        Mock<ResetPasswordDto> resetPasswordDtoMock = new("", "", "", "");
        _resetPasswordDtoValidatorMock.Setup(x => x.Validate(It.IsAny<ResetPasswordDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.UserWithEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        //Act
        IActionResult result = await _userController.ResetPassword(resetPasswordDtoMock.Object);

        //Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ResetPassword_ForInvalidToken_ReturnsBadRequest()
    {
        //Arrange
        Mock<ResetPasswordDto> resetPasswordDtoMock = new("", "", "", "");
        _resetPasswordDtoValidatorMock.Setup(x => x.Validate(It.IsAny<ResetPasswordDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.UserWithEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userServiceMock.Setup(x => x.ValidateResetPasswordTokenAsync(It.IsAny<string>())).ReturnsAsync(new ValidateResetPasswordTokenResult(false, "Invalid token"));

        //Act
        IActionResult result = await _userController.ResetPassword(resetPasswordDtoMock.Object);

        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Get2faUri_ForValidData_ReturnsUri()
    {
        //Arrange
        string uri = "otpauth://totp/FilmForum:email@email.com?secret=3123ddd123&issuer=FilmForum&algorithm=SHA512";
        GetUserDto getUserDto = new(new User() { Id = 1, Email = "email@email.com" });
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "1") }))
        };
        _userServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(getUserDto);
        _multifactorAuthenticationServiceMock.Setup(x => x.GenerateUriAsync(getUserDto.Email)).ReturnsAsync(() => uri);

        //Act
        IActionResult result = await _userController.Get2faUri();

        //Assert
        OkObjectResult okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
        string uriResult = okObjectResult.Value.Should().BeOfType<string>().Subject;
        uriResult.Should().Be(uri);
    }

    [Fact]
    public async Task Get2faUri_ForNonExistingUser_ReturnsUnauthorized()
    {
        //Arrange
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "999999") }))
        };
        _userServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(() => null);

        //Act
        IActionResult result = await _userController.Get2faUri();

        //Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Get2faPng_ForValidData_ReturnsUri()
    {
        //Arrange
        string uri = "otpauth://totp/FilmForum:email@email.com?secret=3123ddd123&issuer=FilmForum&algorithm=SHA512";
        byte[] qrCode = new byte[] { 1, 2, 3, 4, 5 };
        GetUserDto getUserDto = new(new User() { Id = 1, Email = "email@email.com" });
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "1") }))
        };
        _userServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(getUserDto);
        _multifactorAuthenticationServiceMock.Setup(x => x.GenerateUriAsync(getUserDto.Email)).ReturnsAsync(() => uri);
        _multifactorAuthenticationServiceMock.Setup(x => x.GenerateQRCodePNGAsync(uri)).ReturnsAsync(() => qrCode);

        //Act
        IActionResult result = await _userController.Get2faPng();

        //Assert
        FileContentResult fileContentResult = result.Should().BeOfType<FileContentResult>().Subject;
        byte[] qrCodeResult = fileContentResult.FileContents.Should().BeOfType<byte[]>().Subject;
        qrCodeResult.Should().BeEquivalentTo(qrCode);
        string contentTypeResult = fileContentResult.ContentType.Should().BeOfType<string>().Subject;
        contentTypeResult.Should().Be("image/png");
    }

    [Fact]
    public async Task Get2faPng_ForNonExistingUser_ReturnsUnauthorized()
    {
        //Arrange
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "999999") }))
        };
        _userServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(() => null);

        //Act
        IActionResult result = await _userController.Get2faPng();

        //Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task SetMultifactorAuthentication_ForValidData_ChangesMultifactorAuthentication()
    {
        //Arrange
        Mock<ChangeMultifactorAuthenticationDto> changeMultifactorAuthenticationDtoMock = new("top", true);
        GetUserDto getUserDto = new(new User() { Id = 1, Email = "email@email.com" });
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "1") }))
        };
        _userServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(getUserDto);
        _multifactorAuthenticationServiceMock.Setup(x => x.VerifyCodeAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _userServiceMock.Setup(x => x.ChangeMultifactorAuthAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(1);

        //Act
        IActionResult result = await _userController.SetMultifactorAuthentication(changeMultifactorAuthenticationDtoMock.Object);

        //Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SetMultifactorAuthentication_ForNonExistingUser_ReturnsUnauthorized()
    {
        //Arrange
        Mock<ChangeMultifactorAuthenticationDto> changeMultifactorAuthenticationDtoMock = new("top", true);
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "999999") }))
        };
        _userServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(() => null);

        //Act
        IActionResult result = await _userController.SetMultifactorAuthentication(changeMultifactorAuthenticationDtoMock.Object);

        //Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task SetMultifactorAuthentication_ForInvalidCode_ReturnsBadRequest()
    {
        //Arrange
        Mock<ChangeMultifactorAuthenticationDto> changeMultifactorAuthenticationDtoMock = new("top", true);
        GetUserDto getUserDto = new(new User() { Id = 1, Email = "email@email.com" });
        _userController.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, "1") }))
        };
        _userServiceMock.Setup(x => x.GetAsync(It.IsAny<int>())).ReturnsAsync(getUserDto);
        _multifactorAuthenticationServiceMock.Setup(x => x.VerifyCodeAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        //Act
        IActionResult result = await _userController.SetMultifactorAuthentication(changeMultifactorAuthenticationDtoMock.Object);

        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }


}