using FilmForumWebAPI.Validators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace FilmForumWebAPI.UnitTests.Validators;

public class PasswordValidatorTests
{
    private readonly IValidator<string> _validator = new PasswordValidator();

    [Fact]
    public void Validate_ForValidData_PassValidation()
    {
        string password = "Password123!";

        TestValidationResult<string> result = _validator.TestValidate(password);

        result.ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> InvalidPasswords()
    {
        yield return new object[] { "" };
        yield return new object[] { "123" };
        yield return new object[] { "123@" };
        yield return new object[] { "123KJ" };
        yield return new object[] { "1232kj" };
        yield return new object[] { new string('K', 200) };
    }

    [Theory]
    [MemberData(nameof(InvalidPasswords))]
    public void Validate_ForInvalidPassword_FailValidation(string password)
    {
        TestValidationResult<string> result = _validator.TestValidate(password);

        result.ShouldHaveAnyValidationError();
    }
}