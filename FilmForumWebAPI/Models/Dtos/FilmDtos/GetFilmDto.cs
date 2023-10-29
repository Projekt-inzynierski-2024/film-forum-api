using FilmForumWebAPI.Models.Entities;

namespace FilmForumWebAPI.Models.Dtos.FilmDtos;

public class GetFilmDto
{
    public GetFilmDto(Film film)
    {
        Id = film.Id;
        Title = film.Title;
        Description = film.Description;
    }

    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}