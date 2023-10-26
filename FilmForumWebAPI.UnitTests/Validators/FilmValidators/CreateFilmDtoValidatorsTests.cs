using FilmForumWebAPI.Models.Dtos.Film;
using FilmForumWebAPI.Validators.FilmValidators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace FilmForumWebAPI.UnitTests.Validators.FilmValidators;

public class CreateFilmDtoValidatorsTests
{
    private readonly IValidator<CreateFilmDto> _validator = new CreateFilmValidator();

    [Fact]
    public void Validate_ForValidData_PassValidation()
    {
        CreateFilmDto createFilmDto = new("Title", "Description");

        TestValidationResult<CreateFilmDto> result = _validator.TestValidate(createFilmDto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> InvalidTitles()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { new string('K', 1000) };
    }

    [Theory]
    [MemberData(nameof(InvalidTitles))]
    public void Validate_ForInvalidTitle_FailValidation(string title)
    {
        CreateFilmDto createFilmDto = new(title, "Description");

        TestValidationResult<CreateFilmDto> result = _validator.TestValidate(createFilmDto);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    public static IEnumerable<object[]> InvalidDescriptions()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { new string('K', 1000) };
    }

    [Theory]
    [MemberData(nameof(InvalidDescriptions))]
    public void Validate_ForInvalidDescription_FailValidation(string description)
    {
        CreateFilmDto createFilmDto = new("Title", description);

        TestValidationResult<CreateFilmDto> result = _validator.TestValidate(createFilmDto);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}