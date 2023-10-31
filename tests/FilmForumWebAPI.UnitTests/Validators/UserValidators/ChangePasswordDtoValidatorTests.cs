using FilmForumModels.Dtos.UserDtos;
using FilmForumWebAPI.Validators.UserValidators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace FilmForumWebAPI.UnitTests.Validators.UserValidators;

public class ChangePasswordDtoValidatorTests
{
    private readonly IValidator<ChangePasswordDto> _validator = new ChangePasswordDtoValidator();

    [Fact]
    public void Validate_ForValidData_PassValidation()
    {
        ChangePasswordDto dto = new("Password123!", "Password123!");

        TestValidationResult<ChangePasswordDto> result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ForDifferentPasswords_FailValidation()
    {
        ChangePasswordDto dto = new("Password123!", "Password1234567!");

        TestValidationResult<ChangePasswordDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    public static IEnumerable<object[]> InvalidPasswords()
    {
        yield return new object[] { "", "" };
        yield return new object[] { null!, "Password123@!" };
        yield return new object[] { "", "Password123@!" };
        yield return new object[] { "Password123!@", null! };
        yield return new object[] { "Password123!@", "" };
    }

    [Theory]
    [MemberData(nameof(InvalidPasswords))]
    public void Validate_ForInvalidPasswords_FailValidation(string password, string confirmPassword)
    {
        ChangePasswordDto dto = new(password, confirmPassword);

        TestValidationResult<ChangePasswordDto> result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}