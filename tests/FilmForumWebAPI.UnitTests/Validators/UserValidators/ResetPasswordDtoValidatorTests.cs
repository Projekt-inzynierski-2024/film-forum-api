using FilmForumModels.Dtos.UserDtos;
using FilmForumWebAPI.Validators.UserValidators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace FilmForumWebAPI.UnitTests.Validators.UserValidators;

public class ResetPasswordDtoValidatorTests
{
    private readonly IValidator<ResetPasswordDto> _validator = new ResetPasswordDtoValidator();

    [Fact]
    public void Validate_ForValidData_PassValidation()
    {
        // Arrange
        ResetPasswordDto resetPasswordDto = new("validemail@email.com", "ValidPassword123!", "ValidPassword123!", "sdadsad2321131sadhadhjk2333213h12jhad");

        // Act
        TestValidationResult<ResetPasswordDto> result = _validator.TestValidate(resetPasswordDto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> InvalidEmails()
    {
        yield return new object[] { "" };
        yield return new object[] { "123" };
        yield return new object[] { new string('K', 100) };
        yield return new object[] { new string('K', 10) + "@" };
    }

    [Theory]
    [MemberData(nameof(InvalidEmails))]
    public void Validate_ForInvalidEmail_FailValidation(string email)
    {
        // Arrange
        ResetPasswordDto resetPasswordDto = new(email, "ValidPassword123!", "ValidPassword123!", "sdadsad2321131sadhadhjk2333213h12jhad");

        // Act
        TestValidationResult<ResetPasswordDto> result = _validator.TestValidate(resetPasswordDto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    //More password tests in PasswordValidatorTests.cs
    [Fact]
    public void Validate_ForInvalidPassword_FailValidation()
    {
        ResetPasswordDto resetPasswordDto = new("email@email.com", "i", "i", "PasswordToken321kj3as");

        TestValidationResult<ResetPasswordDto> result = _validator.TestValidate(resetPasswordDto);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ForOtherPasswords_FailValidation()
    {
        ResetPasswordDto resetPasswordDto = new("email@email.com", "Password123", "Password123098", "PasswordToken321kj3as");

        TestValidationResult<ResetPasswordDto> result = _validator.TestValidate(resetPasswordDto);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ForInvalidResetPasswordToken_FailValidation()
    {
        // Arrange
        ResetPasswordDto resetPasswordDto = new("email@email.com", "ValidPassword123!", "ValidPassword123!", "");

        // Act
        TestValidationResult<ResetPasswordDto> result = _validator.TestValidate(resetPasswordDto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ResetPasswordToken);
    }
}