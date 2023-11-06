using FilmForumWebAPI.Extensions;
using FluentAssertions;
using FluentValidation.Results;

namespace FilmForumWebAPI.UnitTests.Extensions;

public class ValidationFailureExtensionTests
{
    [Fact]
    public void GetMessagesAsString_ForEmptyList_ReturnEmptyString()
    {
        string NameErrorMessage = "Name was null";
        string SecondNameErrorMessage = "Second name was null";
        string EmailErrorMessage = "Email has less than 6 characters";

        List<ValidationFailure> validationFailures = new()
        {
            new ValidationFailure("Name", NameErrorMessage),
            new ValidationFailure("SecondName", SecondNameErrorMessage),
            new ValidationFailure("Email", EmailErrorMessage)
        };

        string expected = $"{NameErrorMessage}{Environment.NewLine}{SecondNameErrorMessage}{Environment.NewLine}{EmailErrorMessage}";

        //Act
        string result = validationFailures.GetMessagesAsString();

        //Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetMessagesAsString_ForEmptyFailureList_ReturnsEmptyString()
    {
        //Act
        string result = ValidationFailureExtension.GetMessagesAsString(Enumerable.Empty<ValidationFailure>());

        //Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetMessagesAsString_ForNullArguments_ThrowArgumentNullException()
    {
        //Act
        Func<string> action = () => ValidationFailureExtension.GetMessagesAsString(null!);

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }
}