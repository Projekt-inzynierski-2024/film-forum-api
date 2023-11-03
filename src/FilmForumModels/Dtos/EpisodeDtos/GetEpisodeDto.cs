using FilmForumModels.Entities;

namespace FilmForumModels.Dtos.EpisodeDtos;

public class GetEpisodeDto
{
    public GetEpisodeDto(Episode episode)
    {
        Id = episode.Id;
        Title = episode.Title;
        Description = episode.Description;
        EpisodeNumber = episode.EpisodeNumber;
        SeasonNumber = episode.SeasonNumber;
        Length = episode.Length;
        Year = episode.Year;
        FilmId = episode.FilmId;
        DirectorIds = episode.DirectorIds;
        ActorIds = episode.ActorIds;
    }

    public string Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int EpisodeNumber { get; set; } = 0;

    public int SeasonNumber { get; set; } = 0;

    public float Length { get; set; } = 0.0f;

    public int Year { get; set; } = 2023;

    public string FilmId { get; set; } = string.Empty;

    public List<string> DirectorIds { get; set; } = new List<string>();

    public List<string> ActorIds { get; set; } = new List<string>();
}