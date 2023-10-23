using FilmForumWebAPI.Models.Dtos;
using FluentValidation;

namespace FilmForumWebAPI.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(5)
            .WithMessage("Username must have at least 5 characters")
            .MaximumLength(50)
            .WithMessage("Username can't have more than 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(5)
            .WithMessage("Password must have at least 5 characters")
            .MaximumLength(50)
            .WithMessage("Password can't have more than 50 characters")
            .Equal(x => x.ConfirmPassword)
            .WithMessage("Passwords must be identical")
            .Must(y => y.Any(char.IsDigit))
            .WithMessage("Password must have at least one digit")
            .Must(y => y.Any(char.IsAsciiLetterLower))
            .WithMessage("Password must have at least one lower letter")
            .Must(y => y.Any(char.IsAsciiLetterUpper))
            .WithMessage("Password must have at least one upper letter");         
    }
}