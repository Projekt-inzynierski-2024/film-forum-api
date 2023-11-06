using FluentValidation.Results;

namespace FilmForumWebAPI.Extensions;

public static class ValidationFailureExtension
{
    /// <summary>
    /// Returns all errors' messages from <paramref name="validationFailures"/> as string.
    /// </summary>
    /// <param name="validationFailures">Errors of validation</param>
    /// <returns>Errors' messages as string</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="validationFailures"/> is null</exception>
    public static string GetMessagesAsString(this IEnumerable<ValidationFailure> validationFailures)
        => string.Join(Environment.NewLine, validationFailures.Select(x => x.ErrorMessage));
}