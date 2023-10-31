using FilmForumModels.Dtos.UserDtos;
using FluentValidation;

namespace FilmForumWebAPI.Validators.UserValidators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator())
            .Equal(x => x.ConfirmPassword)
            .WithMessage("Passwords must be identical");
    }
}
