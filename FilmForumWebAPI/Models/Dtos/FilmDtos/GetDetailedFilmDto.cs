using FilmForumWebAPI.Models.Dtos.EpisodeDtos;
using FilmForumWebAPI.Models.Entities;

namespace FilmForumWebAPI.Models.Dtos.FilmDtos;

public class GetDetailedFilmDto : GetFilmDto
{
    public GetDetailedFilmDto(Film film): base(film)
    {
        Episodes = film.Episodes.Select(e => new GetEpisodeDto(e)).ToList();
    }

    public List<GetEpisodeDto> Episodes { get; set; }
}
