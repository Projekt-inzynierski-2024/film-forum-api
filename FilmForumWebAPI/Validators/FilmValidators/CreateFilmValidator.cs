using FilmForumWebAPI.Models.Dtos.Film;
using FluentValidation;

namespace FilmForumWebAPI.Validators.FilmValidators;

public class CreateFilmValidator : AbstractValidator<CreateFilmDto>
{
    public CreateFilmValidator()
    {
        RuleFor(film => film.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(100)
            .WithMessage("Title can't have more than 100 characters");
        RuleFor(film => film.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description can't have more than 500 characters");
    }
}
