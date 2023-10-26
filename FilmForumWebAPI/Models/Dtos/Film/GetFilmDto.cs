namespace FilmForumWebAPI.Models.Dtos.Film
{
    public class GetFilmDto
    {
        public GetFilmDto(Entities.Film film)
        {
            Id = film.Id;
            Title = film.Title;
            Description = film.Description;
        }

        public string? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}