using AuthenticationManager.Interfaces;
using EmailSender.Interfaces;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Models.Enums;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Controllers;
using FilmForumWebAPI.Services.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PasswordManager.Interfaces;

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

    public UserControllerTests()
    {
        _userController = new(_loggerMock.Object,
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
                              _createAdminDtoValidatorMock.Object);
    }

    [Fact]
    public async Task RegisterAdmin_ForValidData_CreatesAdmin()
    {
        // Arrange
        CreateAdminDto createAdminDto = new("NewAdmin", "myadminemail@mail.com", "AdminStrongPassword123", "AdminStrongPassword123", "SuperSecretKey");
        UserCreatedDto userCreatedDto = new(1, "NewAdmin", "myadminemail@mail.com", "jwttokenreturned123");
        _createAdminDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateAdminDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateAdminDto>(), It.IsAny<UserRole>())).ReturnsAsync(userCreatedDto);

        // Act
        IActionResult result = await _userController.RegisterAdmin(createAdminDto);

        // Assert
        CreatedResult createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Location.Should().Be(nameof(UserController.GetById));
        createdResult.Value.Should().Be(userCreatedDto);
    }

    [Fact]
    public async Task RegisterAdmin_ForInvalidData_ReturnsBadRequest()
    {
        // Arrange
        _createAdminDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateAdminDto>())).Returns(new ValidationResult() { Errors = new() {new ValidationFailure("Name", "Name was null") } });

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
        CreateUserDto createUserDto = new("NewUser", "myemail@mail.com", "StrongPassword123", "StrongPassword123");
        UserCreatedDto userCreatedDto = new(1, "NewUser", "myemail@mail.com", "jwttokenreturned123");
        _createUserDtoValidatorMock.Setup(x => x.Validate(It.IsAny<CreateUserDto>())).Returns(new ValidationResult());
        _userServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateUserDto>(), It.IsAny<UserRole>())).ReturnsAsync(userCreatedDto);

        // Act
        IActionResult result = await _userController.Register(createUserDto);

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
}