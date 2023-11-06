using FilmForumWebAPI.Validators;
using FluentAssertions;
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
        yield return new object[] { " " };
        yield return new object[] { "1" };
        yield return new object[] { "   2   " };
        yield return new object[] { "z" };
        yield return new object[] { "a      " };
        yield return new object[] { "D" };
        yield return new object[] { "       K" };
        yield return new object[] { "123" };
        yield return new object[] { "123z" };
        yield return new object[] { "y12313z" };
        yield return new object[] { "K13" };
        yield return new object[] { "K1223" };
        yield return new object[] { "123@" };
        yield return new object[] { "123K" };
        yield return new object[] { "123K       " };
        yield return new object[] { "123KJ" };
        yield return new object[] { "123      kj" };
        yield return new object[] { "_      kj" };
        yield return new object[] { "_123" };
        yield return new object[] { "1232kj" };
        yield return new object[] { new string('K', 200) };
        yield return new object[] { "1" + new string('K', 200) };
        yield return new object[] { "     " + new string('K', 200) };
    }

    [Theory]
    [MemberData(nameof(InvalidPasswords))]
    public void Validate_ForInvalidPassword_FailValidation(string password)
    {
        TestValidationResult<string> result = _validator.TestValidate(password);

        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_ForNullPassword_ThrowsArgumentNullException()
    {
        //Act
        Func<TestValidationResult<string>> action = () => _validator.TestValidate(null!);

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }
}