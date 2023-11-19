using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FilmForumWebAPI.IntegrationTests.HelpersForTests;

public class DatabaseHelper
{
    #region UsersDatabase

    private static readonly string _usersDatabaseConnectionString = "Server=localhost,2433;Database=UsersDb;Uid=SA;Pwd=zaq1@WSX;TrustServerCertificate=True";

    private static async Task ResetUsersDatabaseAsync(UsersDatabaseContext usersDatabaseContext)
    {
        await usersDatabaseContext.Database.EnsureDeletedAsync();
        await usersDatabaseContext.Database.EnsureCreatedAsync();
    }

    public static async Task<UsersDatabaseContext> CreateAndPrepareUsersDatabaseContextForTestingAsync()
    {
        DbContextOptionsBuilder<UsersDatabaseContext> builder = new();
        builder.UseSqlServer(_usersDatabaseConnectionString);
        UsersDatabaseContext usersDatabaseContext = new(builder.Options);
        await ResetUsersDatabaseAsync(usersDatabaseContext);
        return usersDatabaseContext;
    }

    #endregion UsersDatabase

    #region FilmsDatabase

    private static async Task ResetFilmsDatabaseAsync(FilmsDatabaseContext filmsDatabaseContext)
    {
        await filmsDatabaseContext.ActorCollection.Database.DropCollectionAsync("actor");
        await filmsDatabaseContext.DirectorCollection.Database.DropCollectionAsync("director");
        await filmsDatabaseContext.EpisodeCollection.Database.DropCollectionAsync("episode");
        await filmsDatabaseContext.FilmCollection.Database.DropCollectionAsync("film");
        await filmsDatabaseContext.ReviewCollection.Database.DropCollectionAsync("review");
    }

    private static readonly FilmForumMongoDatabaseSettings _filmForumMongoDatabaseSettings = new()
    {
        ConnectionString = "mongodb://root:123456@localhost:37017",
        DatabaseName = "filmForum",
        ActorsCollectionName = "actor",
        DirectorsCollectionName = "director",
        EpisodesCollectionName = "episode",
        FilmsCollectionName = "film",
        ReviewsCollectionName = "review"
    };

    public static async Task<FilmsDatabaseContext> CreateAndPrepareFilmsDatabaseContextForTestingAsync()
    {
        FilmsDatabaseContext filmsDatabaseContext = new(Options.Create(_filmForumMongoDatabaseSettings));
        await ResetFilmsDatabaseAsync(filmsDatabaseContext);
        return filmsDatabaseContext;
    }

    #endregion FilmsDatabase
}