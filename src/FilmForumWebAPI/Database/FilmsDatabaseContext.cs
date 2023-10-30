using FilmForumModels.Entities;
using FilmForumModels.Models.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FilmForumWebAPI.Database;

public class FilmsDatabaseContext
{
    public FilmsDatabaseContext(IOptions<FilmForumMongoDatabaseSettings> mongoDatabaseSettings)
    {
        MongoClient mongoClient = new(mongoDatabaseSettings.Value.ConnectionString);
        IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoDatabaseSettings.Value.DatabaseName);

        ActorCollection = mongoDatabase.GetCollection<Actor>(mongoDatabaseSettings.Value.ActorsCollectionName);
        DirectorCollection = mongoDatabase.GetCollection<Director>(mongoDatabaseSettings.Value.DirectorsCollectionName);
        EpisodeCollection = mongoDatabase.GetCollection<Episode>(mongoDatabaseSettings.Value.EpisodesCollectionName);
        FilmCollection = mongoDatabase.GetCollection<Film>(mongoDatabaseSettings.Value.FilmsCollectionName);
        ReviewCollection = mongoDatabase.GetCollection<Review>(mongoDatabaseSettings.Value.ReviewsCollectionName);
    }

    public IMongoCollection<Actor> ActorCollection { get; private set; }
    public IMongoCollection<Director> DirectorCollection { get; private set; }
    public IMongoCollection<Episode> EpisodeCollection { get; private set; }
    public IMongoCollection<Film> FilmCollection { get; private set; }
    public IMongoCollection<Review> ReviewCollection { get; private set; }
}