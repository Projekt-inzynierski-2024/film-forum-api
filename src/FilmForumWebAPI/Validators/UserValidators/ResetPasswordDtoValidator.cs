using FilmForumModels.Dtos.UserDtos;
using FluentValidation;

namespace FilmForumWebAPI.Validators.UserValidators;

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator())
            .Equal(x => x.ConfirmPassword)
            .WithMessage("Passwords must be identical");

        RuleFor(x => x.ResetPasswordToken)
            .NotEmpty()
            .WithMessage("Reset password token is required");
    }
}