using FilmForumWebAPI.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FilmForumWebAPI.UnitTests.Extensions;

public class ActionContextExtensionTests
{
    [Fact]
    public void GetValidationErrorsMessagesAsString_ForGivenActionContext_ReturnsMessagesAsString()
    {
        // Arrange
        string NameErrorMessage = "Name was empty";
        string SecondNameErrorMessage = "Second name has more than 60 characters";

        ActionContext actionContext = new();
        actionContext.ModelState.AddModelError("Name", NameErrorMessage);
        actionContext.ModelState.AddModelError("SecondName", SecondNameErrorMessage);

        string expected = $"{NameErrorMessage}{Environment.NewLine}{SecondNameErrorMessage}";

        // Act
        string result = actionContext.GetValidationErrorsMessagesAsString();

        //Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetValidationErrorsMessagesAsString_ForGivenActionContextWithoutErrors_ReturnsEmptyString()
    {
        // Arrange
        ActionContext actionContext = new();

        // Act
        string result = actionContext.GetValidationErrorsMessagesAsString();

        //Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetValidationErrorsMessagesAsString_ForNullActionContext_ThrowsNullReferenceException()
    {
        ActionContext actionContext = null!;

        // Act
        Func<string> action = actionContext.GetValidationErrorsMessagesAsString;

        //Assert
        action.Should().Throw<NullReferenceException>();
    }
}