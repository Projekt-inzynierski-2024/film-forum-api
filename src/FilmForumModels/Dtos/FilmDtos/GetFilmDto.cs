using FilmForumModels.Entities;

namespace FilmForumModels.Dtos.FilmDtos;

public class GetFilmDto
{
    public GetFilmDto(Film film)
    {
        Id = film.Id;
        Title = film.Title;
        Description = film.Description;
        IsMovie = film.IsMovie;
    }

    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsMovie { get; set; }
}