using FilmForumModels.Dtos.EpisodeDtos;
using FilmForumModels.Entities;

namespace FilmForumModels.Dtos.FilmDtos;

public class GetDetailedFilmDto : GetFilmDto
{
    public GetDetailedFilmDto(Film film) : base(film)
    {
        Episodes = film.Episodes?.Select(e => new GetEpisodeDto(e)).ToList() ?? new List<GetEpisodeDto>();
    }

    public List<GetEpisodeDto> Episodes { get; set; }
}