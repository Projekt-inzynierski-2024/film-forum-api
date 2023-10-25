using FilmForumWebAPI.Models.Dtos.User;
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
            .SetValidator(new PasswordValidator())
            .Equal(x => x.ConfirmPassword)
            .WithMessage("Passwords must be identical");         
    }
}