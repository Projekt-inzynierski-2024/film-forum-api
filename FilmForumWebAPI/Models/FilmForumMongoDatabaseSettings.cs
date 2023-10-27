namespace FilmForumWebAPI.Models;

public class FilmForumMongoDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ActorsCollectionName { get; set; } = null!;

    public string DirectorsCollectionName { get; set; } = null!;

    public string EpisodesCollectionName { get; set; } = null!;

    public string FilmsCollectionName { get; set; } = null!;

    public string ReviewsCollectionName { get; set; } = null!;
}