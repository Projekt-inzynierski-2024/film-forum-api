using MongoFramework;

namespace FilmForumWebAPI.Database;

public class FilmsDatabaseContext : MongoDbContext
{
    public FilmsDatabaseContext(IMongoDbConnection connection) : base(connection)
    {
    }
}
