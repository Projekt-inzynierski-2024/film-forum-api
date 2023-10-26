namespace FilmForumWebAPI.Models;

public class FilmForumMongoDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string FilmsCollectionName { get; set; } = null!;
}
