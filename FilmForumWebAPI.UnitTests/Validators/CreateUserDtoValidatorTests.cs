using FilmForumWebAPI.Models.Dtos;
using FilmForumWebAPI.Validators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace FilmForumWebAPI.UnitTests.Validators;

public class CreateUserDtoValidatorTests
{
    private readonly IValidator<CreateUserDto> _validator;

    public CreateUserDtoValidatorTests()
    {
        _validator = new CreateUserDtoValidator();
    }

    [Fact]
    public void Validate_ForValidData_PassValidation()
    {
        CreateUserDto dto = new("username", "email@email.com", "Password123!", "Password123!");

        TestValidationResult<CreateUserDto> result = _validator.TestValidate(dto);

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
        CreateUserDto dto = new(username, "email@email", "password123", "password123");

        TestValidationResult<CreateUserDto> result = _validator.TestValidate(dto);

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
        CreateUserDto dto = new("username", email, "password123", "password123");

        TestValidationResult<CreateUserDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    public static IEnumerable<object[]> InvalidPasswords()
    {
        yield return new object[] { "" };
        yield return new object[] { "123" };
        yield return new object[] { new string('K', 200) };
    }

    [Theory]
    [MemberData(nameof(InvalidPasswords))]
    public void Validate_ForInvalidPassword_FailValidation(string password)
    {
        CreateUserDto dto = new("username", "email@email.com", password, password);

        TestValidationResult<CreateUserDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ForOtherPasswords_FailValidation()
    {
        CreateUserDto dto = new("username", "email@email.com", "password123", "otherpassword321");

        TestValidationResult<CreateUserDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}