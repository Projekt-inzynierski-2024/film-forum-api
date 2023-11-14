using FluentValidation;

namespace FilmForumWebAPI.Validators;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator() => RuleFor(x => x)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(5)
            .WithMessage("Password must have at least 5 characters")
            .MaximumLength(50)
            .WithMessage("Password can't have more than 50 characters")
            .Must(x => x.Any(char.IsDigit))
            .WithMessage("Password must have at least one digit")
            .Must(x => x.Any(char.IsAsciiLetterLower))
            .WithMessage("Password must have at least one lower letter")
            .Must(x => x.Any(char.IsAsciiLetterUpper))
            .WithMessage("Password must have at least one upper letter");
}