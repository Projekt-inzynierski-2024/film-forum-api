using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FilmForumWebAPI.Extensions;

public static class ActionContextExtension
{
    /// <summary>
    /// Returns all errors' messages from <paramref name="actionContext"/> as string.
    /// </summary>
    /// <param name="actionContext">Action representing request</param>
    /// <returns>Errors' messages as string</returns>
    /// <exception cref="NullReferenceException"> When <paramref name="actionContext"/> is null</exception>
    public static string GetValidationErrorsMessagesAsString(this ActionContext actionContext)
        => string.Join(Environment.NewLine, actionContext.ModelState.Values
                 .Where(x => x.ValidationState == ModelValidationState.Invalid)
                 .SelectMany(x => x.Errors)
                 .Select(x => x.ErrorMessage));
}