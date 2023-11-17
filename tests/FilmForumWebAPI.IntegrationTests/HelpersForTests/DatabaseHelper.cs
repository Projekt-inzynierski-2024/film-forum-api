using FilmForumWebAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.IntegrationTests.HelpersForTests;

public class DatabaseHelper
{
    public static async Task<UsersDatabaseContext> CreateAndPrepareUsersDatabaseContextForTesting()
    {
        DbContextOptionsBuilder<UsersDatabaseContext> builder = new();
        builder.UseSqlServer("Server=localhost,2433;Database=UsersDb;Uid=SA;Pwd=zaq1@WSX;TrustServerCertificate=True");
        UsersDatabaseContext usersDatabaseContext = new(builder.Options);
        await usersDatabaseContext.Database.EnsureDeletedAsync();
        await usersDatabaseContext.Database.EnsureCreatedAsync();
        return usersDatabaseContext;
    }
}