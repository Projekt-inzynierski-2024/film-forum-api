using FilmForumModels.Dtos.ActorDtos;
using FilmForumModels.Dtos.DirectorDtos;
using FilmForumModels.Dtos.FilmDtos;
using FilmForumModels.Entities;

namespace FilmForumModels.Dtos.EpisodeDtos;

public class GetDetailedEpisodeDto
{
    public GetDetailedEpisodeDto(Episode episode)
    {
        Id = episode.Id;
        Title = episode.Title;
        Description = episode.Description;
        EpisodeNumber = episode.EpisodeNumber;
        SeasonNumber = episode.SeasonNumber;
        Length = episode.Length;
        Year = episode.Year;
        Film = new GetFilmDto(episode.Film ?? new());
        Directors = episode.Directors?.Select(d => new GetDirectorDto(d)).ToList() ?? new();
        Actors = episode.Actors?.Select(a => new GetActorDto(a)).ToList() ?? new();
    }

    public string Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int EpisodeNumber { get; set; } = 0;

    public int SeasonNumber { get; set; } = 0;

    public float Length { get; set; } = 0.0f;

    public int Year { get; set; } = 2023;

    public GetFilmDto Film { get; }

    public List<GetDirectorDto> Directors { get; }

    public List<GetActorDto> Actors { get; }
}
