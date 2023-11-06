using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Validators.UserValidators;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;

namespace FilmForumWebAPI.UnitTests.Validators.UserValidators;

public class CreateAdminDtoValidatorTests
{
    private readonly IValidator<CreateAdminDto> _validator = new CreateAdminDtoValidator(Options.Create(new AdminDetails() { SecretKey = "SuperSecretKeyForAdmin" }));

    [Fact]
    public void Validate_ForValidData_PassValidation()
    {
        CreateAdminDto dto = new("username", "email@email.com", "Password123!", "Password123!", "SuperSecretKeyForAdmin");

        TestValidationResult<CreateAdminDto> result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> InvalidUsernames()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { "1" };
        yield return new object[] { new string('K', 200) };
    }

    [Theory]
    [MemberData(nameof(InvalidUsernames))]
    public void Validate_ForInvalidUsername_FailValidation(string username)
    {
        CreateAdminDto dto = new(username, "email@email", "password123", "password123", "SecretKey");

        TestValidationResult<CreateAdminDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Username);
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
        CreateAdminDto dto = new("username", email, "password123", "password123", "SecretKey");

        TestValidationResult<CreateAdminDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    //More password tests in PasswordValidatorTests.cs
    [Fact]
    public void Validate_ForInvalidPassword_FailValidation()
    {
        CreateAdminDto dto = new("username", "email@email.com", "i", "i", "SecretKey");

        TestValidationResult<CreateAdminDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ForOtherPasswords_FailValidation()
    {
        CreateAdminDto dto = new("username", "email@email.com", "password123", "otherpassword321", "SecretKey");

        TestValidationResult<CreateAdminDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("Other_Key_That_Is_Not_Valid")]
    public void Validate_ForInvalidSecretKey_FailValidation(string secretKey)
    {
        CreateAdminDto dto = new("username", "email@email.com", "password123", "otherpassword321", secretKey);

        TestValidationResult<CreateAdminDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.SecretKey);
    }
}