using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Models.Settings;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace FilmForumWebAPI.Validators.UserValidators;

public class CreateAdminDtoValidator : AbstractValidator<CreateAdminDto>
{
    public CreateAdminDtoValidator(IOptions<AdminDetails> adminDetails)
    {
        Include(new CreateUserDtoValidator());

        RuleFor(x => x.SecretKey)
            .NotEmpty()
            .WithMessage("Secret key can't be empty")
            .Equal(adminDetails.Value.SecretKey)
            .WithMessage("Secret key is not valid");
    }
}